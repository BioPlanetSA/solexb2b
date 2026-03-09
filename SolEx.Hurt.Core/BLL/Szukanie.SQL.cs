using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Extensions;
using SolEx.Hurt.Model.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Core.BLL
{
    public class HasheProduktowDoSzukania : IHasLongId
    {
        public HasheProduktowDoSzukania(long idProduktu, int jezykId, string hashProduktu)
        {
            this.IdProduktu = idProduktu;
            this.JezykId = jezykId;
            this.HashProduktu = hashProduktu;
            this.Id = WyliczId(idProduktu, jezykId);
        }

        public HasheProduktowDoSzukania() { }
        public long Id { get; set; }
        public long IdProduktu { get; set; }
        public int JezykId { get; set; }
        public string HashProduktu { get; set; }

        public long WyliczId(long idProduktu, int jezyk) => $"{idProduktu}_{jezyk}".WygenerujIDObiektuSHAWersjaLong();
    }

    public partial class Szukanie : LogikaBiznesBaza, ISzukanie
    {


        public class SzukaneProduktyWithRank
        {
            [Alias("pid")]
            public long ProduktId { get; set; }

            [Alias("rank")]
            public int Rank { get; set; }
        }

        private char[] forbidenChars = new[] { '"', '\'', '[', ']', ';', '&', '%', '/', '?', '+', '=', '!', '@', '\\', '|', '*', '^', '~' };

        /// <summary>
        /// Metoda która zwraca produkty które spełniają warunki wyszukiwania 
        /// (metoda zwraca id produktów na podstawie całej oferty - zawsze szuka w całych produktach)
        /// </summary>
        /// <param name="produkty"></param>
        /// <param name="szukanaFraza"></param>
        /// <param name="szukanePola"></param>
        /// <param name="klient"></param>
        /// <param name="jezyk"></param>
        /// <returns></returns>
        public long[] FiltrowanieProduktowWedlugSzukanejFrazy(string szukanaFraza, IKlient klient, int jezyk, HashSet<long> filtrujWgProduktowIds = null, int maxLiczbaWynikow = 1000)
        {
            szukanaFraza = UsunZnakiZakazaneZSzukania(szukanaFraza);

            if (string.IsNullOrEmpty(szukanaFraza))
            {
                throw new Exception("Brak frazy po której szukamy - nie można szukać pustych fraz, lub brak pola w których szukamy.");
            }

            //szukamy w SQL -> szukarka zwraca ID produktów już w sugerowanej kolejności
            long[] wynikSzukania = this.Calosc.DostepDane.DbORM.SqlList<SzukaneProduktyWithRank>("EXEC szukajProduktow @szukanaFraza, @jezykId, @klientId", new { szukanaFraza, jezykId = jezyk, klientId = klient.Id }).Select(x => x.ProduktId).ToArray();

            if (filtrujWgProduktowIds != null)
            {
                return wynikSzukania.Intersect(filtrujWgProduktowIds).Take(maxLiczbaWynikow).ToArray();
            }

            ////Filtrujemy produkty i ustawiamy rangi w zaleznosci od poziomu trafnosci
            //    return FiltrujProduktyDlaSzukania(hasheProduktow, szukanaFraza, sposob).ToDictionary(x => x.IdProduktu, x => x.Ranga);

            //metoda gwarantuje kolejność
            //List<ProduktKlienta> produktyWyszukane = produkty.WhereKeyIsIn(wynikSzukania);

            return wynikSzukania.Take(maxLiczbaWynikow).ToArray();
        }

        public string UsunZnakiZakazaneZSzukania(string szukanaFraza)
        {
            szukanaFraza = forbidenChars.Aggregate(szukanaFraza, (c1, c2) => c1.Replace(c2.ToString(), string.Empty)); // zmiana \n na USUWANIE - tak chce bio planet
            return szukanaFraza;
        }




        /// <summary>
        /// Metoda która generuje hashe dla produktów
        /// </summary>
        /// <param name="produktyKlienta"></param>
        /// <param name="listaPol"></param>
        /// <param name="jezykId"></param>
        /// <param name="slownikHashy"></param>
        /// <param name="slownikProduktowIPodbic"></param>
        /// <returns></returns>
        public List<HasheProduktowDoSzukania> WygenerujSlownikHashy(IList<ProduktKlienta> produktyKlienta,
            string[] listaPol, int jezykId, Dictionary<long, string> slownikHashy)
        {
            List<HasheProduktowDoSzukania> wynik = new List<HasheProduktowDoSzukania>();
            var akcesor = Refleksja.PobierzRefleksja(typeof(ProduktKlienta));
            foreach (var produkt in produktyKlienta)
            {
                StringBuilder str = new StringBuilder();
                foreach (var pole in listaPol.Select(x => akcesor[produkt, x]))
                {
                    string wartosc = pole?.ToString();
                    if (pole is IEnumerable wartosci && !(wartosci is string))
                    {
                        wartosc = string.Join("_", wartosci.Cast<object>().Select(x => x.ToString()));
                    }

                    if (string.IsNullOrEmpty(wartosc))
                    {
                        continue;
                    }

                    str.Append(wartosc);
                    //str.Append(pole is IEnumerable wartosci ? string.Join("_", (IEnumerable)wartosci) : str.ToString());
                    str.Append(" || ");
                }

                long? podbicie = null;

                var hash = new HasheProduktowDoSzukania(produkt.Id, jezykId, str.ToString().Trim(" || ".ToCharArray()));
                if (!slownikHashy.TryGetValue(hash.Id, out string hashproduktu) || !hashproduktu.Equals(hash.HashProduktu, StringComparison.CurrentCultureIgnoreCase) )
                {
                    wynik.Add(hash);
                }
            }
            return wynik;
        }

        /// <summary>
        /// Słownik priorytetów dla pól wyszukiwania produktu
        /// </summary>
        private readonly Dictionary<string, int> priorytetyPolWyszukiwania = new Dictionary<string, int>
        {
            {"Nazwa", 1},
            {"Kod", 2},
            {"KodKreskowy", 3},
            {"Kategorie", 4},
            {"MarkaNazwa", 5},
            {"Marka", 6}
        };

        /// <summary>
        /// Metoda która zwraca pola posortowane względem priorytetu (chcemy aby niektóre pola jeżeli są wybrane w ustawieniu były zawsze na początku)
        /// </summary>
        /// <param name="szukanePola"></param>
        /// <param name="propertisy"></param>
        /// <returns></returns>
        public string[] PobierzPolaDoSzukania(IEnumerable<string> szukanePola, HashSet<string> propertisy)
        {
            Dictionary<int, string> wynik = new Dictionary<int, string>();
            int prior = 1000;
            foreach (string pole in szukanePola)
            {
                if (!propertisy.Contains(pole))
                {
                    continue;
                }

                if (priorytetyPolWyszukiwania.TryGetValue(pole, out int priorytet))
                {
                    wynik.Add(priorytet, pole);
                }
                else
                {
                    wynik.Add(prior, pole);
                    prior++;
                }
            }

            return wynik.OrderBy(x => x.Key).Select(x => x.Value).ToArray();
        }

        public void UsunHasheNieaktywnychProduktow()
        {
            //Usuwamy hashe dla produktów które są nieaktywne
            this.Calosc.DostepDane.DbORM.ExecuteNonQuery("DELETE from HasheProduktowDoSzukania WHERE IdProduktu in (select Id from Produkt where Widoczny=0) OR " +
                                                         "IdProduktu NOT IN (select lacznik.ProduktId from KategoriaProduktu kategoria join ProduktKategoria " +
                                                         "lacznik on lacznik.KategoriaId = kategoria.Id join Grupa grupa on grupa.Id = kategoria.GrupaId   " +
                                                         "WHERE grupa.PomijajProduktyWWyszukiwaniu = 0 )");
        }


        //dla bio: ALTER FULLTEXT CATALOG FTS_HasheProduktowDoSzukania REBUILD WITH ACCENT_SENSITIVITY = ON
        //private void UstawSzukanieWgSposobuDokladneZPolskimiZnakamiCzybez()
        //{
        //    //sprawdzanie najpierw czy szukanie ma szukać z polskimi znakami czy bez polskich znaków
        //    int szukanieZPolskimiZnakami = this.Calosc.DostepDane.DbORM.SqlScalar<int>("SELECT FULLTEXTCATALOGPROPERTY('FTS_HasheProduktowDoSzukania', 'accentsensitivity')");

        //    if (this.Calosc.Konfiguracja.PodczasWyszukiwaniaZmienPolskeZnaki SposobSzukaniaProduktow == SposobSzukaniaProduktow.Dokladne && szukanieZPolskimiZnakami == 0)
        //    {
        //        this.Calosc.DostepDane.DbORM.ExecuteSql("ALTER FULLTEXT CATALOG FTS_HasheProduktowDoSzukania REBUILD WITH ACCENT_SENSITIVITY = ON");
        //    }

        //    if (this.Calosc.Konfiguracja.SposobSzukaniaProduktow != SposobSzukaniaProduktow.Dokladne && szukanieZPolskimiZnakami == 1)
        //    {
        //        this.Calosc.DostepDane.DbORM.ExecuteSql("ALTER FULLTEXT CATALOG FTS_HasheProduktowDoSzukania REBUILD WITH ACCENT_SENSITIVITY = OFF");
        //    }
        //}


        //Metoda zwracająca produkty klienta dla admin
        private IList<ProduktKlienta> PobierzWszystkieProduktyAdmina(int jezyk)
        {
            var sztucznyAdmin = SolexBllCalosc.PobierzInstancje.Klienci.SztucznyAdministrator();
            var admin = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(x => x.Email == sztucznyAdmin.Email, sztucznyAdmin);
            admin.JezykId = jezyk;
            return this.Calosc.DostepDane.Pobierz<ProduktKlienta>(jezyk, admin);
        }



        /// <summary>
        /// Metoda która odpalana jestw global.asax przy starcie aplikacji w celu wyliczenia hashy produktow
        /// </summary>
        public void PrzeliczHashDlaProduktow(bool wymuszoneWyliczenieWszystkichHashy = false)
        {
         //   UstawSzukanieWgSposobuDokladneZPolskimiZnakamiCzybez();

            List<long> idsProduktowDoHashowania = null;

            //Pobieramy wszystkie pola które są zaznaczone w ustawieniu (po których będziemy szukać)
            string[] pola = PobierzPolaDoSzukania(this.Calosc.Konfiguracja.ProduktyWyszukiwanie, new HashSet<string>( Refleksja.Properties(typeof(ProduktKlienta)).Keys ) );

            //lista hashy która jest kolekcja wynikową i ją będziemy aktualizować
            List<HasheProduktowDoSzukania> wygenerowaneHashe = new List<HasheProduktowDoSzukania>();

            //pobieramy słownik wygenerowanych hashy
            HasheProduktowDoSzukania[] wpisyHashe = null;

            if (idsProduktowDoHashowania == null)
            {
                wpisyHashe = this.Calosc.DostepDane.Pobierz<HasheProduktowDoSzukania>(null).ToArray();
            }
            else
            {
                wpisyHashe = this.Calosc.DostepDane.Pobierz<HasheProduktowDoSzukania>(null, x => Sql.In(x.Id, idsProduktowDoHashowania)).ToArray();
            }

            Dictionary<int, Dictionary<long, string>> wpisyWBazie = wpisyHashe.GroupBy(x => x.JezykId)
                .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.Id, y => y.HashProduktu));

            //musimy przejść po wszystkich językach systemu w celu wygenerowania hashów dla produktów
            foreach (var jezyk in this.Calosc.Konfiguracja.JezykiWSystemie)
            {
                IList<ProduktKlienta> produktyKient = PobierzWszystkieProduktyAdmina(jezyk.Key);

                if (idsProduktowDoHashowania != null)
                {
                    produktyKient = produktyKient.Where(x => idsProduktowDoHashowania.Contains(x.Id)).ToList();
                }

                if (!wpisyWBazie.TryGetValue(jezyk.Key, out Dictionary<long, string> wpisyDlaJezyka))
                {
                    wpisyDlaJezyka = new Dictionary<long, string>();
                }
                var dane = WygenerujSlownikHashy(produktyKient, pola, jezyk.Key, wpisyDlaJezyka);
                wygenerowaneHashe.AddRange(dane);
            }

            //Jeżeli są jakieś hashe to je aktualizujemy
            int rozmiarPaczki = 100;
            while (wygenerowaneHashe.Count > 0)
            {
                int ilosc = wygenerowaneHashe.Count > rozmiarPaczki ? rozmiarPaczki : wygenerowaneHashe.Count;
                List<HasheProduktowDoSzukania> hasheDoWyslania = wygenerowaneHashe.Take(ilosc).ToList();
                this.Calosc.DostepDane.AktualizujListe(hasheDoWyslania);
                wygenerowaneHashe.RemoveRange(0, ilosc);
            }

            //  this.Calosc.DostepDane.PobierzParametrBazyDanych("dataOdswierzeniaIndeksowWyszukiwaniaProduktow", DateTime.MinValue);
        }






    }
}
