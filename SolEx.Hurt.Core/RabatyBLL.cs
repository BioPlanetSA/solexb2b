using System;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;

namespace SolEx.Hurt.Core
{
    public class RabatyBll : LogikaBiznesBaza, IRabatyBll
    {
        public RabatyBll(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        

        public List<decimal> PobierzPrzedzialyCenowe()
        {
            string przdzialy = Calosc.Konfiguracja.PrzedzialyCenowe;

            if (przdzialy == null)
            {
              throw new Exception("Brak przedziałów cenowych -nie powinno tu nigdy wejść w takiej sytaucji. Trzeba sprawdzać czy sa poziomy zanim sie tu wejdzie");
            }

            List<decimal> count = new List<decimal>();
            foreach (string s in przdzialy.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries))
            {
                decimal liczba;
                if (TextHelper.PobierzInstancje.SprobojSparsowac(s, out liczba))
                {
                    count.Add(liczba);
                }
            }

            if (count.IsEmpty())
            {
                throw  new Exception("Nieporpawna wartość poziomów cenowych - wygląda na złe wartości");
            }

            return count.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Waluty występujące w rabatowaniu
        /// </summary>
        /// <returns></returns>
        private HashSet<long> WalutyWystepujace()
        {
            HashSet<long> wynik = Calosc.Cache.PobierzObiekt<HashSet<long>>(RabatyWRabach);
            if (wynik == null)
            {
                wynik = new HashSet<long>( SlownikRabatow().Values.Where(x => x.WalutaId.HasValue).Select(x => x.WalutaId.Value) );
                Calosc.Cache.DodajObiekt_Kluczowy(RabatyWRabach, wynik);
            }
            return wynik;
        }

        /// <summary>
        /// kategorie klientów występujące w rabatowaniu
        /// </summary>
        /// <returns></returns>
        private HashSet<int> KategorieKlientowWystepujace()
        {
            HashSet<int> wynik = Calosc.Cache.PobierzObiekt<HashSet<int>>(KategorieKlientowWRabach);
            if (wynik == null)
            {
                wynik =  new HashSet<int>( SlownikRabatow().Values.Where(x => x.KategoriaKlientowId.HasValue).Select(x => x.KategoriaKlientowId.Value) );
                Calosc.Cache.DodajObiekt_Kluczowy(KategorieKlientowWRabach, wynik);
            }
            return wynik;
        }

        /// <summary>
        /// kategorie produktów występujące w rabatowaniu
        /// </summary>
        /// <returns></returns>
        private HashSet<long> KategoriePRoduktowWystepujace()
        {
            HashSet<long> wynik = Calosc.Cache.PobierzObiekt<HashSet<long>>(KategorieProduktowWRabach);
            if (wynik == null)
            {
                wynik = new HashSet<long>(SlownikRabatow().Values.Where(x => x.KategoriaProduktowId.HasValue).Select(x => x.KategoriaProduktowId.GetValueOrDefault()));
                Calosc.Cache.DodajObiekt_Kluczowy(KategorieProduktowWRabach, wynik);
            }
            return wynik;
        }

        /// <summary>
        /// produkty występujące w rabatowaniu
        /// </summary>
        /// <returns></returns>
        private HashSet<long> ProduktyWystepujace()
        {
            HashSet<long> wynik = Calosc.Cache.PobierzObiekt<HashSet<long>>(PRoduktyWRabach);
            if (wynik == null)
            {
                wynik = new HashSet<long>( SlownikRabatow().Values.Where(x => x.ProduktId.HasValue).Select(x => x.ProduktId.GetValueOrDefault()) );
                Calosc.Cache.DodajObiekt_Kluczowy(PRoduktyWRabach, wynik);
            }
            return wynik;
        }

        /// <summary>
        /// klienci występujący w rabatowaniu
        /// </summary>
        /// <returns></returns>
        private HashSet<long> KlienciWystepujace()
        {
            HashSet<long> wynik = Calosc.Cache.PobierzObiekt<HashSet<long>>(KlienciWRabach);
            if (wynik == null)
            {
                wynik = new HashSet<long>( SlownikRabatow().Values.Where(x => x.KlientId.HasValue).Select(x => x.KlientId.GetValueOrDefault()) );
                Calosc.Cache.DodajObiekt_Kluczowy(KlienciWRabach, wynik);
            }
            return wynik;
        }

        /// <summary>
        /// Typy rabatów występujące w rabatowaniu
        /// </summary>
        /// <returns></returns>
        private HashSet<RabatTyp> TypyWystepujace()
        {
            HashSet<RabatTyp> wynik = Calosc.Cache.PobierzObiekt<HashSet<RabatTyp>>(TypyWRabach);
            if (wynik == null)
            {
                wynik = new HashSet<RabatTyp>( SlownikRabatow().Values.Select(x => x.TypRabatu) );
                Calosc.Cache.DodajObiekt_Kluczowy(TypyWRabach, wynik);
            }
            return wynik;
        }

        private const string RabatyWRabach = "rabaty_waluty";
        private const string CechyWRabach = "rabaty_cechy";
        private const string KategorieKlientowWRabach = "rabaty_kategoriklientow";
        private const string KategorieProduktowWRabach = "rabaty_kategorieproduktow";
        private const string PRoduktyWRabach = "rabaty_produkty";
        private const string KlienciWRabach = "rabaty_klienci";
        private const string TypyWRabach = "rabaty_typy";

        //todo: przy aktualizacji cen czyscic cache tego slownika
        private Dictionary<long, Dictionary<long, FlatCeny>> _slownikCenDlaKlientowIProduktow = new Dictionary<long, Dictionary<long, FlatCeny>>();

        /// <summary>
        /// Pobiera z bazy danych wyliczona wczesniej flat cene dla klienta
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="produktID"></param>
        /// <returns></returns>
        private FlatCeny Get(IKlient klient, long produktID)
        {
            //pobieramy tylko klienta ktorego potrzebujemy - wydaje mi sie ze tak jest szybciej i lepiej - czesc klientow moze sie nigdy nie zalogowac i nie ma sensu ich trzymac
            Dictionary<long, FlatCeny> slownikProduktowDlaKlienta = null;

            if (!this._slownikCenDlaKlientowIProduktow.TryGetValue(klient.Id, out slownikProduktowDlaKlienta))
            {
                slownikProduktowDlaKlienta = Calosc.DostepDane.Pobierz<FlatCeny>(null, x => x.KlientId == klient.Id).ToDictionary(x => x.ProduktId, x => x);

                if (slownikProduktowDlaKlienta.IsEmpty())
                {
                    SolexBllCalosc.PobierzInstancje.Log.InfoFormat("Brak przeliczonych cen dla klienta {0}", klient.Id );
                    return null;
                }

                _slownikCenDlaKlientowIProduktow.Add(klient.Id, slownikProduktowDlaKlienta);
            }

            FlatCeny flatceny = null;
            if (!slownikProduktowDlaKlienta.TryGetValue(produktID, out flatceny))
            {
                SolexBllCalosc.PobierzInstancje.Log.InfoFormat("Brak przeliczonych cen dla klienta {0} i produktu: {1}", klient.Id, produktID);
                return null;
            }
            

            return flatceny;
        }

        private FlatCeny WyliczCenyDlaPromocji(IKlient klient, IProduktBazowy produkt, HashSet<long> cechy, IEnumerable<int> kategorieKlienta, long? waluta)
        {
            return WyliczCeneDlaTypow(klient, produkt, cechy, new List<RabatTyp> {RabatTyp.Promocja}, kategorieKlienta, false, waluta);
        }

        private FlatCeny WyliczCenyDlaRabatow(IKlient klient, IProduktBazowy produkt, HashSet<long> cechy, IEnumerable<int> kategorieKlienta, long? waluta)
        {
            return WyliczCeneDlaTypow(klient, produkt, cechy, new List<RabatTyp> {RabatTyp.Zaawansowany, RabatTyp.Prosty}, kategorieKlienta, true, waluta);
        }

        private CenaPoziomu PobierzPoziomCenyKlienta(IProduktBazowy produkt, IKlienci klient, RabatBLL rabat = null)
        {
            var ceny = produkt.CenyPoziomy;
            int klucz = 0;
            if (rabat != null && rabat.PoziomCenyId.HasValue)
            {
                klucz = rabat.PoziomCenyId.Value;
            }
            else
            {
                klucz = klient.PoziomCenowyId ?? Calosc.Konfiguracja.GetPriceLevelHurt;
            }

            CenaPoziomu cena;
            if (!ceny.TryGetValue(klucz, out cena))
            {
                long? walutaId = klient.WalutaId;

                if (rabat != null && rabat.PoziomCenyId.HasValue)
                {
                    walutaId = Calosc.DostepDane.PobierzPojedynczy<PoziomCenowy>(rabat.PoziomCenyId.Value).WalutaId;
                }

                if (walutaId.HasValue)
                {
                    cena = new CenaPoziomu(klucz, 0, produkt.Id, walutaId.Value);
                }
                else
                {
                    throw new Exception(string.Format("Brak ceny dla produktu {0}. Klient: {1} nie ma ustawionej waluty", produkt.Id, klient.Email));
                }
            }
            return cena;
        }

        private FlatCeny WyliczCeneDlaTypow(IKlient klient, IProduktBazowy produkt, HashSet<long> cechy, List<RabatTyp> typy, IEnumerable<int> kategorieKlienta, bool rabatZkartyklienta, long? waluta)
        {
            FlatCeny tmp;

            RabatBLL pasujaceRabaty = Znajdz(produkt.bazoweID, cechy, klient, kategorieKlienta, typy, waluta);
            CenaPoziomu poziomCeny = PobierzPoziomCenyKlienta(produkt, klient, pasujaceRabaty);

            decimal? rabat = null;
            var typWartosc = RabatSposob.Procentowy;

            if (pasujaceRabaty != null)
            {
                rabat = pasujaceRabaty.PobierzWartoscRabatu(poziomCeny.Netto);
                typWartosc = pasujaceRabaty.TypWartosci;
            }

            if (!rabat.HasValue)
            {
                if (rabatZkartyklienta)
                {
                    rabat = klient.Rabat;
                }
                else rabat = 0;
            }

            //nie ma promocji
            if (pasujaceRabaty == null && typy.Contains(RabatTyp.Promocja) && rabat == 0)
                return null;

            decimal netto = Kwoty.WyliczWartosc(poziomCeny.Netto, rabat.Value, typWartosc);

            if (netto < 0)
            {
                netto = 0;
            }

            tmp = new FlatCeny(klient.Id, Kwoty.WyliczRabat(poziomCeny.Netto, netto, Calosc.Konfiguracja.RabatZaokraglacDoIluMiejsc), poziomCeny, netto);
            if (pasujaceRabaty != null)
            {
                tmp.TypRabatu = (int) pasujaceRabaty.TypRabatu;
            }
            return tmp;
        }

        /// <summary>
        /// liczenie ceny TYLKO dla zalogowanych klientów - z uwzglednieniem gradacji
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="produkt"></param>
        /// <returns></returns>
        public FlatCenyBLL WyliczCeneDlaKlientaZalogowanegoZGradacja(IKlient klient, IProduktKlienta produkt)
        {
            //konfekcje tylko dla klientow zalogowanych liczymy
            if (klient.Dostep == AccesLevel.Niezalogowani)
            {
                throw new Exception("Liczenie ceny jest możliwe tylko dla klientów zalogowanych");
            }

            FlatCenyBLL wynik = WyliczCeneDlaKlientaZalogowanego(klient, produkt);

            if (produkt.GradacjePosortowane == null)
            {
                //todo: czemu robimy kopie a nie dziedziczenie?!
                return wynik;
            }

            decimal kupionejuz = 0;
            var poziomyGradacji = WyliczoneGradacje(produkt, klient, wynik, out kupionejuz);
            
            //zeby nie zmieniac juz wyliczonych cenby w cache
            wynik = new FlatCenyBLL(wynik, produkt, klient) { GradacjaUzytaDoLiczeniaCeny_Poziomy = poziomyGradacji, GradacjaUzytaDoLiczeniaCeny_KupioneIlosci = kupionejuz };

            //moze byc null wtedy gdy klient ma lepsza cene juz teraz po normalnych rabatch niz by mial po gradacjach
            if (poziomyGradacji != null)
            {
                GradacjaWidok wynikk = poziomyGradacji.First(x => x.AktualnaCena && !x.Spelniny);
                if (wynikk != null)
                {
                    wynik.CenaNetto = wynikk.CenaNetto;
                    wynik.CenaNettoDokladna = wynikk.CenaNetto;
                }
            }

            //todo: czemu robimy kopie a nie dziedziczenie?!
            return wynik;
        }


        public void WyczyscCacheFlatCenKlienta(long idKlienta)
        {
          //  Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd($"flatcena_{idKlienta}_");
        }

        /// <summary>
        /// licze cene produktu dla klienta - bez uwzgledniania gradacji
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="produkt"></param>
        /// <returns></returns>
        public FlatCenyBLL WyliczCeneDlaKlientaZalogowanego(IKlient klient, IProduktKlienta produkt)
        {
            //nie ma sensu tu cachowac bo i tak jest w obeickei cahowana cena dla klienta
            //string kluczCache = $"flatcena_{klient.Id}_{produkt.Id}";
            FlatCeny wynik = null;

            if (!Calosc.Konfiguracja.CzyFlatCenyWBazieUzupelniane)
            {
                //czy jest w cache
            //    var cacheCena = Calosc.Cache.PobierzObiekt<FlatCenyBLL>(kluczCache);
                //if (cacheCena != null)
                //{
                //    return cacheCena;
                //}
            }
            else
            {
                wynik = Get(klient, produkt.bazoweID);
            }
         
            if (wynik == null)
            {
                if (produkt.NiePodlegaRabatowaniu)
                {
                    CenaPoziomu cp = PobierzPoziomCenyKlienta(produkt, klient);
                    return new FlatCenyBLL(new FlatCeny(klient.Id, 0, cp, cp.Netto), produkt, klient);
                }

                long? walutaKlienta = null;
                if (WalutyWystepujace().Any()) //olewamy waluty jeśli nie ma rabatów na waluty
                {
                    walutaKlienta = klient.WalutaId;
                    if (!WalutyWystepujace().Contains(walutaKlienta.GetValueOrDefault()))
                    {
                        walutaKlienta = null; //czyścimy jeśli klient ma walutę inną niż istnijące
                    }
                }
                FlatCeny tmpzpromocja = WyliczCenyDlaPromocji(klient, produkt, produkt.CechyProduktuWystepujaceWRabatach, klient.Kategorie, walutaKlienta);
                FlatCeny tmpbezpromocji = WyliczCenyDlaRabatow(klient, produkt, produkt.CechyProduktuWystepujaceWRabatach, klient.Kategorie, walutaKlienta);

                DoliczRabatDodatkowy(tmpbezpromocji, produkt.CechyProduktuWystepujaceWRabatach, klient, klient.Kategorie, walutaKlienta);
                if (tmpzpromocja == null || tmpzpromocja.CenaNetto == 0 || tmpbezpromocji.CenaNetto <= tmpzpromocja.CenaNetto)
                {
                    wynik = tmpbezpromocji;
                }
                else
                {
                    var walutab2b = Calosc.Konfiguracja.SlownikWalut[tmpbezpromocji.WalutaId];
                    wynik = tmpzpromocja;
                    wynik.CenaNettoPrzedPromocja = tmpbezpromocji.CenaNetto;
                    wynik.CenaNettoPrzedPromocja.Waluta = walutab2b.WalutaB2b;

                    wynik.CenaHurtowaNetto = tmpbezpromocji.CenaHurtowaNetto;
                }
            }

            //przeliczenie waluty jak sa rozne waluty
            if (klient.WalutaKlienta.Id != wynik.WalutaId)
            {
                if (wynik.CenaNetto == 0 && wynik.CenaHurtowaNetto == 0)
                {
                    wynik.WalutaId = klient.WalutaKlienta.Id;
                }
                else
                {
                    //musi byc przeliczenie
                    Waluta walutaCeny = Calosc.Konfiguracja.SlownikWalut[wynik.WalutaId];

                    if (!walutaCeny.Kurs.HasValue)
                    {
                        throw new Exception($"Brak przelicznika kursu dla waluty: {walutaCeny.WalutaErp}. Przelicznenie wymagane dla produktu ID: {produkt.Id}, klienta ID: {klient.Id}. " +
                                            $"Klient wymaga waluty: {klient.WalutaKlienta.WalutaErp}. Cena wyciągnieta dla klienta (wartość): {wynik.CenaNetto}, waluta: {wynik.WalutaId}.");
                    }

                    // decimal kursCeny = walutaCeny.Kurs.Value;
                    //decimal kursWalutyKlienta = klient.WalutaKlienta.Kurs.Value;
                    //todo: dodac do konfiguracji slownik opcji
                    var kursCalkowity = Calosc.Konfiguracja.PobierzKursWalut(walutaCeny, klient.WalutaKlienta);//kursCeny*(1M/kursWalutyKlienta);


                    //przepisanie starej ceny kopii
                    wynik.PrzeliczenieWaluty_CenaNettoBazowa = wynik.CenaNetto;
                    wynik.PrzeliczenieWaluty_WalutaIdBazowa = wynik.WalutaId;
                    wynik.PrzeliczenieWaluty_Kurs = kursCalkowity;

                    wynik.CenaNetto *= kursCalkowity;
                    wynik.CenaHurtowaNetto *= kursCalkowity;
                    wynik.WalutaId = klient.WalutaKlienta.Id;
                }
            }

            //ostateczna cene przepisujemy do bez gradacji
            wynik.CenaNettoBezGradacji = wynik.CenaNetto;

            FlatCenyBLL wynikowaCena = new FlatCenyBLL(wynik, produkt, klient);
         //   Calosc.Cache.DodajObiekt(kluczCache, wynikowaCena);
            return wynikowaCena;
        }

        private void DoliczRabatDodatkowy(FlatCeny tmp, HashSet<long> cechy, IKlient klient, IEnumerable<int> kategorieKlienta, long? walutaKlienta)
        {
            if (tmp == null)
                return;

            RabatBLL r = Znajdz(tmp.ProduktId, cechy, klient, kategorieKlienta, new List<RabatTyp> {RabatTyp.RabatDodatkowyDoliczanyDoTegoCoMaKlient}, walutaKlienta);
            if (r != null)
            {
                if (r.TypWartosci != RabatSposob.Procentowy)
                {
                    throw new InvalidOperationException("Rabat dodatkowy może tylko obsługiwać rabat procentowy");
                }
                decimal rabat = r.PobierzWartoscRabatu(tmp.CenaNetto).GetValueOrDefault() + tmp.Rabat;
                decimal nowanetto = Kwoty.WyliczWartosc(tmp.CenaHurtowaNetto, rabat, r.TypWartosci);
                decimal nowyrabat = Kwoty.WyliczRabat(tmp.CenaHurtowaNetto, nowanetto);
                tmp.CenaNetto = nowanetto;
                tmp.Rabat = nowyrabat;
            }
        }

        private RabatBLL RabatDlaKlucza(RabatTyp r, long? klient, int? kategoriaKlienta, long? produkt, long? kategorie, long? cecha, long? waluta, IDictionary<long, RabatBLL> wszystkieRabaty)
        {
            long klucz = Rabat.WyliczID(r, klient, kategoriaKlienta, produkt, kategorie, cecha, waluta); //klucz klient produkt
            RabatBLL wynik;

            if (wszystkieRabaty.TryGetValue(klucz, out wynik))
            {
                return wynik;
            }
          
            if (waluta.HasValue) //jak był null to klucz byłyby identyczny
            {
                klucz = Rabat.WyliczID(r, klient, kategoriaKlienta, produkt, kategorie, cecha, null); //klucz klient produkt
                if (wszystkieRabaty.TryGetValue(klucz, out wynik))
                {
                    return wynik;
                }
            }
            return null;
        }

        private const string CacheRabatySlownik = "slownik_rabatow";

        public IDictionary<long, RabatBLL> SlownikRabatow()
        {
            return LockHelper.PobierzInstancje.PobierzDaneWLocku_zUcyciemCache(CacheRabatySlownik, () =>
            {
                //jeszcze raz zeby sie upewnic ze nikt nie stoi i czeka
                IDictionary<long, RabatBLL> wynik = null;
                
                //spokojnie juz budujemy slownik
                var wynikTemp = Calosc.DostepDane.Pobierz<RabatBLL>(null,
                    x => (x.DoKiedy == null || x.DoKiedy >= DateTime.Now) &&
                         (x.OdKiedy == null || x.OdKiedy <= DateTime.Now));
                
                wynik = new Dictionary<long, RabatBLL>(wynikTemp.Count);
                
                foreach (RabatBLL rabat in wynikTemp)
                {
                    wynik.Add(rabat.Id, rabat);
                }

                return wynik;
            });
        }

        public void UsunCache(IList<object> obj)
        {
            Calosc.Cache.UsunObiekt(CacheRabatySlownik);
            Calosc.Cache.UsunObiekt(RabatyWRabach);
            Calosc.Cache.UsunObiekt(CechyWRabach);
            Calosc.Cache.UsunObiekt(KategorieKlientowWRabach);
            Calosc.Cache.UsunObiekt(KategorieProduktowWRabach);
            Calosc.Cache.UsunObiekt(PRoduktyWRabach);
            Calosc.Cache.UsunObiekt(KlienciWRabach);
            Calosc.Cache.UsunObiekt(TypyWRabach);
        }


        public void WyczyscGradacjePoZmianieKoszyka(IKlient klient)
        {
            string klucz = null;
            //po zmianie koszyka musimy przeleciec op produktach klienta i wyczyscic ceny
            foreach (var jezykId in  Calosc.Konfiguracja.JezykiWSystemie.Keys)
            {
                var produkty = Calosc.ProduktyKlienta.PobierzProduktyKlientaZCache(jezykId, klient);

                if (produkty != null)
                {
                    foreach (var p in produkty)
                    {
                        p.WymusPrzeliczanieCeny();
                    }
                }
            }
        }

        class KategorieICechyRabatowe
        {
            public HashSet<long> CechyProduktu { get; set; }
            public HashSet<int> idKategoriiKlienta { get; set; }
        }

        private KategorieICechyRabatowe PobierzCechyIKategorieRabatowe()
        {
            string klucz = "KategorieICechyRabatowe";
            KategorieICechyRabatowe result = Calosc.Cache.PobierzObiekt<KategorieICechyRabatowe>(klucz);
            if (result == null)
            {
                result = new KategorieICechyRabatowe();
                result.idKategoriiKlienta = new HashSet<int> ( Calosc.DostepDane.DbORM.SqlList<int>("select Id from KategoriaKlienta where Grupa like 'Rabat'") );
                result.CechyProduktu = new HashSet<long>( Calosc.DostepDane.DbORM.SqlList<long>("select Id from Cecha where Symbol like 'Rabat:%'") );
                Calosc.Cache.DodajObiekt(klucz, result);
            }

            return result;
        }

        public RabatBLL Znajdz(long produktId, HashSet<long> cechyid, IKlient klient, IEnumerable<int> wszystkiekatkl, List<RabatTyp> typy, long? walutaKlienta)
        {
            bool sprawdzacprodukt = ProduktyWystepujace().Contains(produktId);
            bool sprawdzacklient = KlienciWystepujace().Contains(klient.Id);
            var kategorieKlienta = new HashSet<int>(KategorieKlientowWystepujace());
            kategorieKlienta.IntersectWith(wszystkiekatkl);
            HashSet<long> kategorieProduktu;
            KategorieICechyRabatowe rabatoweKategorieiCechy = PobierzCechyIKategorieRabatowe();
            if (Calosc.ProduktyKategorieDostep.ProduktyKategorieGrupowanePoProdukcie.TryGetValue(produktId, out kategorieProduktu))
            {
                kategorieProduktu = new HashSet<long>( kategorieProduktu.Intersect(KategoriePRoduktowWystepujace()) );
            }

            HashSet<RabatTyp> rt = new HashSet<RabatTyp>(TypyWystepujace());
            rt.IntersectWith(typy);
            var wszystkie = SlownikRabatow();

            foreach (RabatTyp r in rt)
            {
                if (sprawdzacklient)
                {
                    if (sprawdzacprodukt)
                    {
                        RabatBLL rabat = RabatDlaKlucza(r, klient.Id, null, produktId, null, null, walutaKlienta, wszystkie); //klucz klient produkt
                        if (rabat != null)
                        {
                            return rabat;
                        }

                    }
                    if (kategorieProduktu != null && kategorieProduktu.Any())
                    {
                        foreach (var kp in kategorieProduktu)
                        {
                            var rabat = RabatDlaKlucza(r, klient.Id, null, null, kp, null, walutaKlienta, wszystkie); //klucz klient kat produktów
                            if (rabat != null)
                            {
                                return rabat;
                            }
                        }
                    }
                    foreach (var c in cechyid)
                    {
                        var rabat = RabatDlaKlucza(r, klient.Id, null, null, null, c, walutaKlienta, wszystkie); //klucz klient  cecha produktu
                        if (rabat != null)
                        {
                            return rabat;
                        }
                    }
                    var rabatt = RabatDlaKlucza(r, klient.Id, null, null, null, null, walutaKlienta, wszystkie); //klucz klienta reszta null
                    if (rabatt != null)
                    {
                        return rabatt;
                    }
                }
                Dictionary<bool, Dictionary<int,List<RabatBLL>>>slownikRabatow = new Dictionary<bool, Dictionary<int, List<RabatBLL>>>();
                foreach (var kk in kategorieKlienta) //na kate klienta
                {
                    bool rabatowaKategoria = rabatoweKategorieiCechy.idKategoriiKlienta.Contains(kk);
                    if (sprawdzacprodukt)
                    {
                        var rabat = RabatDlaKlucza(r, null, kk, produktId, null, null, walutaKlienta, wszystkie); //klucz  produkt
                        if (rabat != null)
                        {
                            DodajRabatDoKolekcji(1, false, slownikRabatow, rabat);
                        }
                    }
                    if (kategorieProduktu != null)
                    {
                        foreach (int kp in kategorieProduktu)
                        {
                            var rabat = RabatDlaKlucza(r, null, kk, null, kp, null, walutaKlienta, wszystkie); //klucz  kat produktów
                            if (rabat != null)
                            {
                                DodajRabatDoKolekcji(2, false, slownikRabatow, rabat);
                            }
                        }
                    }
                    foreach (int c in cechyid)
                    {
                        var rabat = RabatDlaKlucza(r, null, kk, null, null, c, walutaKlienta, wszystkie); //klucz   cecha produktu
                        if (rabat != null)
                        {
                            DodajRabatDoKolekcji(3, rabatowaKategoria && rabatoweKategorieiCechy.CechyProduktu.Contains(c), slownikRabatow, rabat);
                        }
                    }
                    var rabatt = RabatDlaKlucza(r, null, kk, null, null, null, walutaKlienta, wszystkie); //klucz reszta null
                    if (rabatt != null)
                    {
                        DodajRabatDoKolekcji(4, false, slownikRabatow, rabatt);
                    }
                }

                if (slownikRabatow.Any())
                {
                    if (!slownikRabatow.TryGetValue(true, out Dictionary<int, List<RabatBLL>> rabatyDlaPoziomu))
                    {
                        rabatyDlaPoziomu = slownikRabatow[false];
                    }

                    return rabatyDlaPoziomu.OrderBy(x => x.Key).First().Value.OrderByDescending(x => x.Wartosc1).First();
                }

                if (sprawdzacprodukt)
                {
                    var rabatt = RabatDlaKlucza(r, null, null, produktId, null, null, walutaKlienta, wszystkie); //klucz reszta null
                    if (rabatt != null)
                    {
                        return rabatt;
                    }
                }
                if (kategorieProduktu != null)
                {
                    foreach (int kp in kategorieProduktu)
                    {
                        var rabatt = RabatDlaKlucza(r, null, null, null, kp, null, walutaKlienta, wszystkie); //klucz klient kat produktów
                        if (rabatt != null)
                        {
                            return rabatt;
                        }
                    }
                }
                foreach (int c in cechyid)
                {
                    var rabatt = RabatDlaKlucza(r, null, null, null, null, c, walutaKlienta, wszystkie); //klucz klient  cecha produktu
                    if (rabatt != null)
                    {
                        return rabatt;
                    }
                }
                var rabaty = RabatDlaKlucza(r, null, null, null, null, null, walutaKlienta, wszystkie); //klucz klient  cecha produktu
                if (rabaty != null)
                {
                    return rabaty;
                }
            }
            return null;
        }

        private void DodajRabatDoKolekcji(int poziom, bool rozbijany, Dictionary<bool, Dictionary<int, List<RabatBLL>>> slownikRabatow, RabatBLL rabat)
        {
            if (!slownikRabatow.TryGetValue(rozbijany, out Dictionary<int, List<RabatBLL>> rab))
            {
                rab = new Dictionary<int, List<RabatBLL>>();
                slownikRabatow.Add(rozbijany, rab);
            }

            if (!rab.TryGetValue(poziom, out List<RabatBLL> ra))
            {
                ra = new List<RabatBLL>();
                rab.Add(poziom, ra);
            }

            ra.Add(rabat);
        }

        /// <summary>
        /// Główna metoda do pobierani ceny produktu dla klienta
        /// </summary>
        /// <param name="produktKlienta"></param>
        /// <returns></returns>
        public IFlatCenyBLL PobierzCeneProduktuDlaKlienta(IProduktKlienta produktKlienta)
        {
            IKlient klient = produktKlienta.Klient.KlientPodstawowy();
            FlatCenyBLL flatCeny;
            if (klient.Dostep == AccesLevel.Niezalogowani)
            {
                flatCeny = new FlatCenyBLL(produktKlienta, klient);
                if (Calosc.Konfiguracja.GetPriceLevelDetal == null)
                {
                    throw new Exception("Brak poziomu cenowego Detalicznego dla klientów nie zalogowanych.");
                }
                try
                {
                   
                    PoziomCenowy domslnyPoz = Calosc.DostepDane.PobierzPojedynczy<PoziomCenowy>(Calosc.Konfiguracja.GetPriceLevelDetal);
                    CenaPoziomu cenaWWybranymPoziomieDlaProduktu = null;
                    try
                    {
                        cenaWWybranymPoziomieDlaProduktu = produktKlienta.CenyPoziomy.Values.First(x => x.PoziomId == Calosc.Konfiguracja.GetPriceLevelDetal);
                    } catch
                    {
                        cenaWWybranymPoziomieDlaProduktu = Calosc.CenyPoziomy.SztucznyPoziomCenowyZerowy[Calosc.Konfiguracja.GetPriceLevelDetal.Value];
                    }

                    flatCeny.CenaNetto = cenaWWybranymPoziomieDlaProduktu.Netto;
                    flatCeny.CenaHurtowaNetto = cenaWWybranymPoziomieDlaProduktu.Netto;

                    //czy jest waluta do ceny dopieta
                    if (cenaWWybranymPoziomieDlaProduktu.WalutaId.HasValue)
                    {
                        flatCeny.WalutaId = cenaWWybranymPoziomieDlaProduktu.WalutaId.Value;
                    }
                    else
                    {
                        //waluta z poziomu cenowego
                        if (domslnyPoz.WalutaId.HasValue)
                        {
                            flatCeny.WalutaId = domslnyPoz.WalutaId.Value;
                        }
                    }

                    if (flatCeny.WalutaId == 0)
                    {
                        throw new Exception($"Brak waluty dla produktu: {produktKlienta.Id} i ceny w poziomie detalicznym");
                    }
                }
                catch
                {
                    throw new Exception(
                        $"Nie można pobrać ceny lub waluty dla klienta niezalogowanego i produktu id: {produktKlienta.Id} i poziomu cenowego detalicznego {Calosc.Konfiguracja.GetPriceLevelDetal}. Produkt posiada {produktKlienta.CenyPoziomy.Count} poziomów cenowych");
                }
            }
            else
            {
                flatCeny = WyliczCeneDlaKlientaZalogowanegoZGradacja(klient, produktKlienta);
            }
            flatCeny.KlientId = klient.Id;
            flatCeny.ProduktId = produktKlienta.Id;
            return flatCeny;
        }

        public List<GradacjaWidok> WyliczoneGradacje(IProduktKlienta produkt, IKlient klient, IFlatCeny flatCenaDlaProduktu, out decimal zakupionaDotychczasIlosc)
        {
            if (klient.Dostep == AccesLevel.Niezalogowani)
            {
                throw new Exception("Nie można liczyć gradacji dla klientów niezalogowanych");
            }

            if (produkt.GradacjePosortowane == null)
            {
                throw new Exception("Produkt nie ma zdefiniowanej gradacji - nie można liczyć poziomów cen gradacji");
            }

            var walutaB2b = Calosc.Konfiguracja.SlownikWalut[flatCenaDlaProduktu.WalutaId];
            decimal netto = flatCenaDlaProduktu.CenaNettoBezGradacji;
            string waluta = walutaB2b.WalutaB2b;
            zakupionaDotychczasIlosc = WyliczIloscProduktow(klient, null, produkt);

            List<KeyValuePair< Konfekcje, decimal> > konfekcjeCenyNetto =  new List<KeyValuePair<Konfekcje, decimal>>( produkt.GradacjePosortowane.Count);
            konfekcjeCenyNetto.Add(new KeyValuePair<Konfekcje, decimal>(produkt.GradacjePosortowane[0], netto));

            //pierwsza iteracja po gradacjach i policzenie cen gradacji - po to zeby wyeliminować te z złą ceną (zawyżoną). Zaczynmay od 1 bo pierwsza zawsze jest wirtualna - bez cen żeby tylko dobrze liczyć ilości brakujące do 2 poziomu
            for (int i = 1; i < produkt.GradacjePosortowane.Count; i++)
            {
                decimal cenaNetto = produkt.GradacjePosortowane[i].PoliczCeneZGradacji(netto);
                //jesli cena jest gorsza, lub taka sama jak juz cena klienta to omijamy ten poziom - poza pierwszym poziomem bo on jest zawsze bez rabatu - sztuczny
                if (cenaNetto >= netto)
                {
                    continue;
                }                

                konfekcjeCenyNetto.Add( new KeyValuePair<Konfekcje, decimal>(produkt.GradacjePosortowane[i], cenaNetto) );
            }

            //czy sa jakies poziomy do liczenia - jedna jest na pewno bo jest wirtualna
            if (konfekcjeCenyNetto.Count <= 1)
            {
                //odlaczenie gradacji z produktu zeby juz jej nie liczyc w przyszlosci - nie jesrt pewny czy to dobrze ze robimy, moze cos sie zepsuc dalej
                produkt.GradacjePosortowane = null;
                return null;
                //brak gradacji
                //chce pokazać id produktu bazowego a nie wirtualnego
                //string produktId = produkt.Id == produkt.bazoweID ? produkt.Id + "" : produkt.bazoweID+"";
                //throw new Exception( $"Brak gradacji dla klienta: {klient.Id} i produktu id: {produktId}.");
            }

            List<GradacjaWidok> dane = new List<GradacjaWidok>();

            for (int i = 0; i < konfekcjeCenyNetto.Count; i++)
            {
                GradacjaWidok tmp = new GradacjaWidok();
                tmp.CenaNetto = new WartoscLiczbowa(konfekcjeCenyNetto[i].Value, waluta);
                tmp.CenaBrutto = new WartoscLiczbowa(Kwoty.WyliczBrutto(tmp.CenaNetto, produkt.Vat, klient), tmp.CenaNetto.Waluta);
                tmp.PrzedzialOdRzeczywisty = konfekcjeCenyNetto[i].Key.Ilosc;

                //jesli nie jest ostatni przedzial to ilosc koncowa uzupelniamy
                if (i == konfekcjeCenyNetto.Count - 1)
                {
                    tmp.PrzedzialDoRzeczywisty = decimal.MaxValue;
                }
                else
                {
                    tmp.PrzedzialDoRzeczywisty = konfekcjeCenyNetto[i + 1].Key.Ilosc;
                }
                
                decimal gornagranicaprzedzialu = tmp.PrzedzialDoRzeczywisty - zakupionaDotychczasIlosc;

                if (gornagranicaprzedzialu > 0)
                {
                    decimal dolnagranicaprzedzialu = konfekcjeCenyNetto[i].Key.Ilosc - zakupionaDotychczasIlosc;
                    if (dolnagranicaprzedzialu > 0 && gornagranicaprzedzialu == decimal.MaxValue) //od x do nieskonczonosci
                    {
                        tmp.PrzedzialOd = dolnagranicaprzedzialu;
                        tmp.IleBrakujeDoSpelnieniaPoziomu = dolnagranicaprzedzialu;
                    }
                    else if (dolnagranicaprzedzialu > 0 && gornagranicaprzedzialu > 0) //od x do y
                    {
                        tmp.PrzedzialOd = dolnagranicaprzedzialu;
                        tmp.PrzedzialDo = gornagranicaprzedzialu;
                        tmp.IleBrakujeDoSpelnieniaPoziomu = dolnagranicaprzedzialu;
                    }
                    else
                    {
                        tmp.AktualnaCena = true;
                        tmp.IleBrakujeDoSpelnieniaPoziomu = 0;
                    }
                }
                else
                {
                    tmp.Spelniny = true;
                    tmp.IleBrakujeDoSpelnieniaPoziomu = 0;
                }
                dane.Add(tmp);
            }

            return dane;
        }

        /// <summary>
        /// Główna metoda do wyliczania ilosci jakie kupione sa produktu. Metoda dolicza iloścu kupione innych produktów wg. ustawien GradacjeUwzgledniajRodziny i GradacjeUzgledniaProduktyZCecha 
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="koszyk">aktualny koszyk względem którego liczyć gradacje - jeśli NULL koszyk zostanie pobrany z bazy danych (WPŁYWA NA WYDAJNOŚ)</param>
        /// <param name="produkt"></param>
        /// <param name="wymuszajPomijanieAktualnegoKoszyka">parametr jest wykorzystywany do obliczenia gradacji w aktualnym koszyku - gdzie musimy liczyc bez koszyka</param>
        /// <returns></returns>
        public virtual decimal WyliczIloscProduktow(IKlient klient, IKoszykiBLL koszyk, IProduktKlienta produkt, bool wymuszajPomijanieAktualnegoKoszyka = false)
        {
           return WyliczIloscProduktow(klient, koszyk, produkt.GradacjeProduktyKtorychZakupyLiczycWspolnie, wymuszajPomijanieAktualnegoKoszyka);
        }

        /// <summary>
        /// wyliczenie ilości kupionych produktów dla gradacji - NIE korzystać z tej wersji metody
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="koszyk">aktualny koszyk względem którego liczyć gradacje - jeśli NULL koszyk zostanie pobrany z bazy danych (WPŁYWA NA WYDAJNOŚ)</param>
        /// <param name="idProduktow"></param>
        /// <param name="wymuszajPomijanieAktualnegoKoszyka">parametr jest wykorzystywany do obliczenia gradacji w aktualnym koszyku - gdzie musimy liczyc bez koszyka</param>
        /// <returns></returns>
        protected decimal WyliczIloscProduktow(IKlient klient, IKoszykiBLL koszyk, HashSet<long> idProduktow, bool wymuszajPomijanieAktualnegoKoszyka = false)
        {
            HashSet<ZCzegoLiczycGradacje> rodzaj = Calosc.Konfiguracja.ZCzegoLiczycGradacje;

            if (rodzaj.IsEmpty())
            {
                throw new Exception("Nie ustawione z czego liczyć gradacje (ustawienie Calosc.Konfiguracja.ZCzegoLiczycGradacje). Nie można liczyć gradacji!");
            }

            decimal iloscproduktu = 0;

            if (!wymuszajPomijanieAktualnegoKoszyka && rodzaj.Contains(ZCzegoLiczycGradacje.Koszyk))
            {
                if (koszyk == null)
                {
                    //todo:!!! koszyk ma byc z sesji pobrany a nie z bazy!!!!
                    koszyk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<KoszykBll>(x => x.Typ == TypKoszyka.Koszyk && x.KlientId == klient.Id, klient);
                }
                iloscproduktu += koszyk.PobierzIlosciProduktowWKoszyku().WhereKeyIsIn(idProduktow).Sum();
            }

            if (rodzaj.Contains(ZCzegoLiczycGradacje.Faktura) || rodzaj.Contains(ZCzegoLiczycGradacje.Zamowienie))
            {
                DateTime odKiedy = DateTime.Now.AddDays(-Calosc.Konfiguracja.GradacjeIleDniWsteczLiczyc);
                //if (odKiedy < Calosc.Konfiguracja.GradacjeOdKiedyLiczyc)
                //{
                //    odKiedy = Calosc.Konfiguracja.GradacjeOdKiedyLiczyc;
                //}

                iloscproduktu += Calosc.KupowaneIlosciBLL.SumaKupowanychIlosci(klient.Id, idProduktow, rodzaj, odKiedy);
            }

            return iloscproduktu;
        }



    }
}
