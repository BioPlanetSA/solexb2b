using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{

    public class MailNoweProdukty : BllBazaCalosc
    {
        public MailNoweProdukty(ISolexBllCalosc calosc) : base(calosc){ }

        /// <summary>
        /// Metoda która przefiltruje produkty względem widoczoności, czy mail był wysłany, względem cech które musi posiadać oraz tych których nie może posiadać. Zwracany jest slownik gdzie klucz to nazwa rodziny a wartosc to produkty w nich - musza być wszystkie bo wszystkie produkty trzeba zaktualizowac
        /// Rozbite filtrowanie gdyż nie ma siltrowania sqlowego
        /// </summary>
        /// <param name="idCechKoniecznych"></param>
        /// <param name="idCechNieMozePosiadac">Posiada wyższy priorytet - jeżeli produkt bedzie miał cechę zabronioną to nie ważne czy ma cechę konieczną i tak mail z nim nie zostanie wysłany</param>
        /// <returns></returns>
        protected Dictionary<string, List<ProduktBazowy>> PobierzWyfiltrowaneProdukty(long[] idCechKoniecznych, long[] idCechNieMozePosiadac)
        {
            var produty = Calosc.DostepDane.Pobierz<ProduktBazowy>(null);
            Dictionary<string, List<ProduktBazowy>> slownikProduktow = new Dictionary<string, List<ProduktBazowy>>();
            foreach (var produktBazowy in produty)
            {
                //Filtrujemy produkty wzgledem widocznosci, czy mail był wysłany, względem widocznych kategorii
                if (!produktBazowy.Widoczny || produktBazowy.WyslanoMailNowyProdukt || produktBazowy.ZdjecieGlowne == null || (produktBazowy.Kategorie!=null && !produktBazowy.Kategorie.Any(x => x.Widoczna)))
                {
                    continue;
                }
                //sprawdzamy czy posiada cechę zabroniona jak tak to pomijamy
                if(idCechNieMozePosiadac != null && idCechNieMozePosiadac.Any() && produktBazowy.IdCechPRoduktu!=null && produktBazowy.IdCechPRoduktu.Any(idCechNieMozePosiadac.Contains))
                {
                    continue;
                }
                //sprawdzamy czy produkt ma ceche konieczna jak nie ma to pomijamy
                if (idCechKoniecznych != null && idCechKoniecznych.Any() && (produktBazowy.IdCechPRoduktu==null || !produktBazowy.IdCechPRoduktu.Any(idCechKoniecznych.Contains)))
                {
                    continue;
                }
                List<ProduktBazowy> prod;
                //pobieramy rodzine dla produktu
                string rodzina = produktBazowy.Rodzina ?? "";
                //jest to produkt do wyslania
                if (!slownikProduktow.TryGetValue(rodzina, out prod))
                {
                    slownikProduktow.Add(rodzina, new List<ProduktBazowy>() {produktBazowy});
                }
                else
                {
                    prod.Add(produktBazowy);
                }
            }
            return slownikProduktow;
        }


        /// <summary>
        /// Metoda pobierająca które produkty były wysłane dla jakieg klienta (zabezpieczenie na wypadek błędu podczas wysyłania gdzie nie zaktualizujemy produktów, żeby klientowi nie wysłać dwa razy maila z tym samym produktem)
        /// </summary>
        /// <param name="dataOdKiedyPobierac">jest to najwczesniejsza data dodania z produktów do wysłania</param>
        /// <returns></returns>
        protected Dictionary<string, HashSet<long>> PobierzWyslaneProduktyDlaKlientow(DateTime dataOdKiedyPobierac)
        {
            return Calosc.DostepDane.Pobierz<DzialaniaUzytkownikow>(null, x => x.ZdarzenieGlowne == ZdarzenieGlowne.NoweProduktyWSystemie && x.Data >= dataOdKiedyPobierac && x.EmailKlienta != null).GroupBy(x => x.EmailKlienta).
                ToDictionary(x => x.Key, x => new HashSet<long>( x.Select(y => y.Parametry["Produkty id"]).SelectMany(y => y.Split(',').Select(long.Parse)) ) );
        }

        public void MailONowychProduktach(long[] idCechKoniecznych, long[] idCechNieMozePosiadac, bool wysylajDoSubkont)
        {
            //Zakomentowane ze względu na fakt że nie ma to sensu bo za pierwszym razem faktycznie sie nie odpali - ale ze nie aktualizujemy produktów to za drugim razem również wyśle się dla wszystkich prduktów
            //DateTime ostatnieuruchomienieModulu = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.TerminOstatniegoUruchomienia("MailNoweProdukty");
            //if (ostatnieuruchomienieModulu == DateTime.MinValue) //minimalna data, czyli nigdy moduł nie był uruchamiany
            //{
            //    Calosc.Log.InfoFormat("Moduł nowych produktów uruchamiany pierwszy raz, koniec - nie będziemy wysyłać przecież info o wszystkich możliwych produktach.");
            //    return;
            //}


            //Pobieramy tylko te produkty które są widoczne, nie były wysłane, posiada zdjęcie główne, posiada widoczne kategorie, posiadaja wymagane i nie maja zakazanych cech. Slownik zawiera nazwę rodziny i produkty z niej
            Dictionary<string, List<ProduktBazowy>> produkty = PobierzWyfiltrowaneProdukty(idCechKoniecznych, idCechNieMozePosiadac);
            if (!produkty.Any())
            {
                return;
            }
            //Sprawdzamy czy jest aktywne powiadomienie do klienta
            long idPowiadomienia = typeof(NoweProduktyWSystemie).FullName.WygenerujIDObiektuSHAWersjaLong();
            ParametryWyslania parametry = Calosc.DostepDane.PobierzPojedynczy<UstawieniePowiadomienia>(idPowiadomienia).ParametryWysylania.First(x => x.DoKogo == TypyPowiadomienia.Klient);

            bool pusteBcc = string.IsNullOrEmpty(parametry.EmailBcc);

            //Jezeli nie ma aktywniego maila i bcc jest puste aktualizujemy produkty i konczymy dzialanie modulu
            if (parametry.Aktywny || !pusteBcc)
            {
                //Wyciagamu date produktu najwczesniej dodanego
                DateTime dataOdKiedySzukac = produkty.Values.SelectMany(x => x).Min(x => x.DataDodania);

                //Wyciagamy tylko te produkty dla których trzeba wysłać maila - przefiltrowane wzgledem rodzin (jeden produkt z rodziny)
                List<long> produktyDoWyslania = produkty.SelectMany(x => x.Key == "" ? x.Value : new List<ProduktBazowy>() { x.Value.First() }).Select(x => x.Id).ToList();

                var aktywniklienci = Calosc.DostepDane.Pobierz<Klient>(null, x => x.Aktywny && x.Email!=null && x.Id != 0 && (wysylajDoSubkont || x.KlientNadrzednyId == null) && x.DataOstatniegoLogowania != null); //mail tylko do aktywnych klientów którzy logowali sie w systemie

                bool wartoscDomyslna;
                Dictionary<long,bool> klienciZProfili = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzKlientowZWartosciaUstawienia<bool>(TypUstawieniaKlienta.PowiadomieniaMailowe, "NoweProduktyWSystemie", AccesLevel.Zalogowani);

                if (!klienciZProfili.TryGetValue(0, out wartoscDomyslna))
                {
                    wartoscDomyslna = true;
                }


                //Pobieramy działania użytkowników oraz maile z nowymi produktami
                Dictionary<string, HashSet<long>> dzialaniaUzytkownikow = PobierzWyslaneProduktyDlaKlientow(dataOdKiedySzukac);


                foreach (var klient in aktywniklienci)
                {
                    bool wartosc;
                    if (!klienciZProfili.TryGetValue(klient.Id, out wartosc))
                    {
                        wartosc = wartoscDomyslna;
                    }


                    if (!wartosc && pusteBcc)
                    {
                        continue;
                    }
                    IList<ProduktKlienta> produktyWirtualne = null;
                    //Pobieramy id produktow które faktycznie sa dostepne dla klienta
                    HashSet<long> dostepne = new HashSet<long>( Calosc.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(klient, out produktyWirtualne).Where(x => produktyDoWyslania.Contains(x)) );
                    //Jezeli sa produkty wirtualne to musimy wyciagnac wszystkie te produkty wirtualne których bazowe id znajduje sie w produktach do wysłania
                    if (produktyWirtualne != null)
                    {
                        dostepne.UnionWith(produktyWirtualne.Where(x => produktyDoWyslania.Contains(x.Id)).Select(x => x.Id));
                    }
                    //Wyciagamy tylko te których jeszcze nie wysłaliśmy klientowi
                    HashSet<long> wyslane;
                    if (dzialaniaUzytkownikow!=null && dzialaniaUzytkownikow.TryGetValue(klient.Email, out wyslane))
                    {
                        dostepne.ExceptWith(wyslane);
                    }
                    if (dostepne.Any())
                    {
                        //pobieramy produkty wirtualne i zwykłe dla klienta i wysyłamy je w mailu
                        IList<ProduktKlienta> produktyKlientaDoWyslania = Calosc.DostepDane.Pobierz<ProduktKlienta>(klient.JezykId, klient, x => dostepne.Contains(x.Id));

                        produktyKlientaDoWyslania = produktyKlientaDoWyslania
                            .Where(x => x.FlatCeny.CenaNetto != 0 && x.FlatCeny.CenaBrutto != 0)
                            .ToList();

                        if (!produktyKlientaDoWyslania.Any())
                        {
                            continue;
                        }

                        try
                        {
                            if (produktyDoWyslania.Any())
                            {
                                Calosc.Statystyki.ZdarzenieNoweProduktyWSystemie(produktyKlientaDoWyslania, klient);
                            }
                        }
                        catch (Exception ex)
                        {
                            Calosc.Log.Error($"Błąd wysyłania maila o nowych produktach klient id: {klient.Id}", ex);
                            throw ex;
                        }
                        finally
                        {
                            //czyszczenie smieci - zaczytanych produktow klienta - po api czyscimy cache z produktow klienta - moze tu byc balagan do nieczego nam nie potrzebny juz
                            Calosc.ProduktyKlienta.WyczyscCacheProduktyKlienta(klient);
                        }
                    }
                }
            }
            var prod = produkty.SelectMany(x => x.Value).ToList();
            prod.ForEach(x => x.WyslanoMailNowyProdukt = true);
            Calosc.DostepDane.AktualizujListe(prod);
            return;
        }
        
    }



    public class NoweProduktyWSystemie : SzablonMailaBaza
    {
        public NoweProduktyWSystemie(IEnumerable<IProduktKlienta> listaProduktow, IKlient klient) : base(klient)
        {
            ListaProduktow = listaProduktow;
            PokazujeZdjecia = ListaProduktow.Count() < 20;
        }
        public NoweProduktyWSystemie() : base(null)
        {
            this.ZgodaNaZmianyPrzezKlienta = true;
        }
        public override string NazwaFormatu()
        {
            return "Nowe produkty w systemie";
        }
        public IEnumerable<IProduktKlienta> ListaProduktow { get;set; }
        public override string OpisFormatu()
        {
            return "Mail informujący o pojawieniu się nowych produktach w systemie. Przy wysyłaniu sprawdzane jest czy klient zobaczy dany produkt (np. mój katalog / specyficzna widoczność produktów). " +
                   "Powiadomienia są wysyłane przez moduł synchronizacji 'Wyślij mail o nowych produktach do klientów' lub " +
                   "przez API np: /api2/maile/MailNoweProdukty?idCechyKoniecznej=1,2&idCechyNieMozePosiadac=3,4&wysylajDoSubkont=true <br/>" +
                   "Jeżeli ma nie być zawężania na podstawie cech link powinien wyglądać np tak: /api2/maile/MailNoweProdukty?wysylajDoSubkont=true <br />" +
                   "Mail z produktem zostanie wysłany gdy będzie on widoczny, bedzie posiadać zdjęcie oraz jego cena dla klienta bedzie > 0. " +
                   "UWAGA! Kolumny w tabeli produktów ustawione są wg. ustawień dla tabeli powiadomienia o nowym zamówieniu!!";
        }

        public override string OpisDlaKlienta()
        {
            return "Mail informujący o pojawieniu się nowych produktach w systemie.";
        }


        public override TypyPowiadomienia[] ObslugiwaneRodzajePowiadomien
        {
            get { return new[]{TypyPowiadomienia.Klient }; }
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return null; }
        }

        public bool PokazujeZdjecia { get; set; }

    }
}
