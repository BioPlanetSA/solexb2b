using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class MailProduktyPrzyjeteNaMagazyn : BllBazaCalosc
    {
        public MailProduktyPrzyjeteNaMagazyn(ISolexBllCalosc calosc) : base(calosc) { }

        public void MailOProduktachPrzyjetychNaMagazyn(long[] idCechKoniecznych, long[] idCechZabronionych,
            decimal minimalneZwiekszenieStanuPrzelicznik, decimal minimalnaIloscBrakuPrzelicznik, int[] idMagazynow)
        {
            if (idMagazynow == null || idMagazynow.IsEmpty())
            {
                string msg = "W module musi być wybrany conajmniej jeden magazyn, z którego będą pobierane stany produktów!";
                this.Calosc.Log.Error(msg);
                throw new Exception(msg);
            }

            if (minimalneZwiekszenieStanuPrzelicznik <= 0m)
            {
                string msg = "W pole współczynnika minimalnego zwiększenia stanu produktu wpisano niedopuszczalną wartość (<= 0)";
                this.Calosc.Log.Error(msg);
                throw new Exception(msg);
            }

            string sciezkaPlikuZeStanamiProduktow = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "zmianyStanowProduktow.json");
            List<ProduktBazowy> produktyB2B = Calosc.DostepDane.Pobierz<ProduktBazowy>(null).ToList();

            FiltrujProdukty(idCechKoniecznych, idCechZabronionych, produktyB2B, out List<ProduktBazowy> produktyPrzefiltrowane);

            //odfiltrowano wszystkie produkty
            if (!produktyPrzefiltrowane.Any())
            {
                if (File.Exists(sciezkaPlikuZeStanamiProduktow))
                {
                    File.Delete(sciezkaPlikuZeStanamiProduktow);
                }

                this.Calosc.Log.Info("Brak produktów spełniających wymagania modułu");
                return;
            }

            List<ProduktStan> produktyStany = Calosc.DostepDane.DbORM.SqlList<ProduktStan>("SELECT * FROM ProduktStan");

            //brak stanów produktów
            if (produktyStany == null || produktyStany.IsEmpty())
            {
                if (File.Exists(sciezkaPlikuZeStanamiProduktow))
                {
                    File.Delete(sciezkaPlikuZeStanamiProduktow);
                }

                this.Calosc.Log.Info("Brak stanów produktów na platformie");
                return;
            }

            //klucz - id produktu, wartość - stan (ilość)
            Dictionary<long, decimal> produktyStanySumy = new Dictionary<long, decimal>();

            //jeśli więcej niż jeden magazyn, to należy sumować stany produktów z wybranych magazynów
            if (idMagazynow.Length > 1)
            {
                Dictionary<int, List<ProduktStan>> produktyStanyWgMagazynow = produktyStany.GroupBy(x => x.MagazynId).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var stanyWMagazynie in produktyStanyWgMagazynow)
                {
                    List<ProduktStan> produktyStanyMagazynu = stanyWMagazynie.Value;
                    foreach (ProduktStan produktStanMagazynu in produktyStanyMagazynu)
                    {
                        if (produktyStanySumy.ContainsKey(produktStanMagazynu.ProduktId))
                        {
                            produktyStanySumy[produktStanMagazynu.ProduktId] += produktStanMagazynu.Stan;
                        }
                        else
                        {
                            produktyStanySumy.Add(produktStanMagazynu.ProduktId, produktStanMagazynu.Stan);
                        }
                    }
                }
            }
            else
            {
                produktyStanySumy = produktyStany.Where(x => x.MagazynId == idMagazynow.First()).ToDictionary(x => x.ProduktId, x => x.Stan);
            }



            HashSet<long> produktyZeStanamiZerowymiAktualne = SprawdzStanZerowyProduktow(minimalnaIloscBrakuPrzelicznik, produktyPrzefiltrowane, produktyStanySumy);

            //nie ma produktów ze stanami zerowymi
            if (!produktyZeStanamiZerowymiAktualne.Any() && !File.Exists(sciezkaPlikuZeStanamiProduktow))
            {
                return;
            }

            //nie ma pliku ale są jakieś aktualne produkty ze stanami zerowymi
            if (!File.Exists(sciezkaPlikuZeStanamiProduktow))
            {
                //wypelnienie pliku id produktów ze stanami zerowymi - wg wspolczynnika braku produktu
                string produktyStanyZeroweJson = Newtonsoft.Json.JsonConvert.SerializeObject(produktyZeStanamiZerowymiAktualne);
                File.WriteAllText(sciezkaPlikuZeStanamiProduktow, produktyStanyZeroweJson);

                return;
            }
            //sprawdzamy stany produktów, których id należy wczytać z pliku
            TextReader trescPliku = new CSVHelperExt().OtworzPlik(sciezkaPlikuZeStanamiProduktow);
            HashSet<long> produktyZeStanamiZerowymiPlik = Newtonsoft.Json.JsonConvert.DeserializeObject<HashSet<long>>(trescPliku.ReadToEnd());

            var produktyStanyDoMaila = new List<ProduktPrzyjetyNaMagazyn>();
            Dictionary<long, ProduktBazowy> slownikProduktow = produktyPrzefiltrowane.ToDictionary(x => x.Id, x => x);

            foreach (long produktStanZerowyPlik in produktyZeStanamiZerowymiPlik)
            {
                if (slownikProduktow.TryGetValue(produktStanZerowyPlik, out ProduktBazowy produkt) && produktyStanySumy.TryGetValue(produktStanZerowyPlik, out decimal aktualnaIlosc))
                {
                    var stanMinimalnyProduktu = produkt.StanMin == default(decimal) ? 1m : produkt.StanMin;
                    if (aktualnaIlosc >= stanMinimalnyProduktu * minimalneZwiekszenieStanuPrzelicznik)
                    {
                        var produktStanMail = new ProduktPrzyjetyNaMagazyn
                        {
                            ProduktId = produktStanZerowyPlik,
                            KodProduktu = produkt.Kod,
                            KodKreskowy = produkt.KodKreskowy,
                            NazwaProduktu = produkt.Nazwa,
                            Stan = aktualnaIlosc
                        };

                        produktyStanyDoMaila.Add(produktStanMail);
                    }
                }
            }

            //są produkty ze stanami zerowymi na platformie - wpisujemy do pliku
            if (produktyZeStanamiZerowymiAktualne.Any())
            {
                string produktyStanyZeroweAktualneJson = Newtonsoft.Json.JsonConvert.SerializeObject(produktyZeStanamiZerowymiAktualne);
                File.WriteAllText(sciezkaPlikuZeStanamiProduktow, produktyStanyZeroweAktualneJson);
            }
            //nie ma żadnych produktów ze stanami zerowymi na platformie - plik trzeba usunąć
            else
            {
                File.Delete(sciezkaPlikuZeStanamiProduktow);
            }


            long mailId = typeof(ProduktyPrzyjeteNaMagazyn).FullName.WygenerujIDObiektuSHAWersjaLong();
            ParametryWyslania ustawienieMaila = Calosc.DostepDane.PobierzPojedynczy<UstawieniePowiadomienia>(mailId).ParametryWysylania.First(x => x.DoKogo == TypyPowiadomienia.Klient); ;
            bool pusteBcc = string.IsNullOrEmpty(ustawienieMaila.EmailBcc);

            //mail ma nie być wysyłany do żadnego klienta i nie ma wpisanego BCC
            if (!ustawienieMaila.Aktywny && pusteBcc)
            {
                return;
            }

            bool wartoscDomyslna;
            Dictionary<long, bool> klienciZProfili = Calosc.ProfilKlienta.PobierzKlientowZWartosciaUstawienia<bool>(TypUstawieniaKlienta.PowiadomieniaMailowe, "NoweCenyPoziomuProduktow", AccesLevel.Zalogowani);

            if (!klienciZProfili.TryGetValue(0, out wartoscDomyslna))
            {
                wartoscDomyslna = true;
            }

            //mail tylko do aktywnych klientów którzy logowali sie w systemie
            IList<Klient> aktywniklienci = Calosc.DostepDane.Pobierz<Klient>(null, x => x.Aktywny && x.Email != null && x.Id != 0 && x.KlientNadrzednyId == null)
                .Where(x => x.DataOstatniegoLogowania != null)
                .ToList();

            foreach (Klient klient in aktywniklienci)
            {
                bool wartosc;

                if (!klienciZProfili.TryGetValue(klient.Id, out wartosc))
                {
                    wartosc = false;
                }

                if (!wartosc && !wartoscDomyslna && pusteBcc)
                {
                    continue;
                }

                try
                {
                    Calosc.Statystyki.ZdarzenieProduktyPrzyjeteNaMagazyn(produktyStanyDoMaila, klient);
                }
                catch (Exception ex)
                {
                    Calosc.Log.Error($"Błąd wysyłania maila o nowych cenach produktu dla klienta o id [{klient.Id}]", ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Sprawdza czy stan produktu jest mniejszy niż oczekiwany (oczekiwany = stan minimalny produktu * współczynnik)
        /// </summary>
        /// <param name="minimalnaIloscBrakuPrzelicznik"></param>
        /// <param name="produktyPrzefiltrowane"></param>
        /// <param name="produktyStanySumy"></param>
        /// <returns></returns>
        private HashSet<long> SprawdzStanZerowyProduktow(decimal minimalnaIloscBrakuPrzelicznik, List<ProduktBazowy> produktyPrzefiltrowane, Dictionary<long, decimal> produktyStanySumy)
        {
            var produktyZeStanamiZerowymi = new HashSet<long>();
            foreach (ProduktBazowy produkt in produktyPrzefiltrowane)
            {
                if (produktyStanySumy.TryGetValue(produkt.Id, out decimal ilosc))
                {
                    decimal stanMinimalnyProduktu;
                    if (produkt.StanMin == null || produkt.StanMin == 0)
                    {
                        stanMinimalnyProduktu = 1m;
                    }
                    else
                    {
                        stanMinimalnyProduktu = produkt.StanMin;
                    }
                    if (ilosc <= stanMinimalnyProduktu * minimalnaIloscBrakuPrzelicznik)
                    {
                        produktyZeStanamiZerowymi.Add(produkt.Id);
                    }
                }
            }

            return produktyZeStanamiZerowymi;
        }

        /// <summary>
        /// Filtruje produkty względem widoczności, cech zakazanych i cech wymaganych
        /// </summary>
        /// <param name="idCechKoniecznych"></param>
        /// <param name="idCechZabronionych"></param>
        /// <param name="produktyBazowe"></param>
        /// <param name="produktyPrzefiltrowane"></param>
        protected virtual void FiltrujProdukty(long[] idCechKoniecznych, long[] idCechZabronionych, List<ProduktBazowy> produktyBazowe, out List<ProduktBazowy> produktyPrzefiltrowane)
        {
            produktyPrzefiltrowane = new List<ProduktBazowy>();
            foreach (ProduktBazowy produktBazowy in produktyBazowe)
            {
                //Filtrujemy produkty wzgledem widocznosci, względem widocznych kategorii
                if (!produktBazowy.Widoczny || (produktBazowy.Kategorie != null && !produktBazowy.Kategorie.Any(x => x.Widoczna)))
                {
                    return;
                }

                //sprawdzamy czy posiada cechę zabroniona jak tak to pomijamy
                if (idCechZabronionych != null && idCechZabronionych.Any() && produktBazowy.IdCechPRoduktu != null && produktBazowy.IdCechPRoduktu.Any(idCechZabronionych.Contains))
                {
                    return;
                }

                //sprawdzamy czy produkt ma ceche konieczna jak nie ma to pomijamy
                if (idCechKoniecznych != null && idCechKoniecznych.Any() && (produktBazowy.IdCechPRoduktu == null || !produktBazowy.IdCechPRoduktu.Any(idCechKoniecznych.Contains)))
                {
                    return;
                }

                //produkt przeszedł weryfikację - dodajemy do przefiltrowanych
                produktyPrzefiltrowane.Add(produktBazowy);
            }
        }
    }

    public class ProduktPrzyjetyNaMagazyn
    {
        public long ProduktId { get; set; }
        public string KodProduktu { get; set; }
        public string NazwaProduktu { get; set; }
        public string KodKreskowy { get; set; }
        public decimal Stan { get; set; }
    }

    public class ProduktyPrzyjeteNaMagazyn : SzablonMailaBaza
    {
        public ProduktyPrzyjeteNaMagazyn() : base(null)
        {
            this.ZgodaNaZmianyPrzezKlienta = true;
        }

        public ProduktyPrzyjeteNaMagazyn(IList<ProduktPrzyjetyNaMagazyn> produkty, IKlient klient) : base(klient)
        {
            this.ProduktyPrzyjeteNaMagazynWOstatnimCzasie = produkty;
        }

        [Ignore]
        public IList<ProduktPrzyjetyNaMagazyn> ProduktyPrzyjeteNaMagazynWOstatnimCzasie { get; set; }

        public override string NazwaFormatu() => "Produkty przyjęte na magazyn w ostatnim czasie.";

        public override string OpisFormatu() => "Mail informujący o przyjęciu na magazyn(y) produktów, których stan nie przekraczał określonej ilości. " +
                                                "Zmiany stanów są obserwowane na podstawie stanów 'zerowych' produktów dodanych do pliku z danymi.";

        public override string OpisDlaKlienta() => "Mail informujący o przyjęciu na magazyn(y) produktów, których stan nie przekraczał określonej ilości.";

        public override TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien => new[] { TypyPowiadomienia.Klient };

        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne => new[] { TypyPowiadomienia.Klient };

    }
}
