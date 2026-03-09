using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.ModelBLL
{
    [FriendlyClassName("aktualny koszyk")]
    [Alias("Koszyk")]
    public class KoszykBll : IKoszykiBLL, IHasLongId, IPoleKlient
    {
        private Dictionary<string,List<KoszykPozycje>> _pozycje;
        //private SlownikParametrowKoszyka _parametry;

        public string Parametry
        {
            get { return JSonHelper.Serialize(DodatkoweParametry); }
            set
            {
                if (value == null)
                {
                    DodatkoweParametry = null;
                    return;
                }
                DodatkoweParametry = (Dictionary<int, DodatkowePoleKoszyka>)JSonHelper.Deserialize(value, typeof(Dictionary<int, DodatkowePoleKoszyka>));
            }
        }
        [Ignore]
        public Dictionary<int, DodatkowePoleKoszyka> DodatkoweParametry { get; set; }
        public KoszykBll()
        {
            MoznaFinalizowacKoszyk = true;
        }

        [Ignore]
        public bool MoznaFinalizowacKoszyk { get; set; }
      
        [Ignore]
        public string Hash
        {
            get { return string.Format("{0}_{1}", Id, Tools.PobierzInstancje.GetMd5Hash(Id.ToString(CultureInfo.InvariantCulture))); }
        }
 
        [FriendlyName("Wartość netto koszyka wraz z kosztem dostawy")]
        public virtual WartoscLiczbowa LacznaWartoscNetto()
        {
            return new WartoscLiczbowa(CalkowitaWartoscHurtowaNettoPoRabacie().Wartosc + CenaDostawyNetto().Wartosc, WalutaKoszyka().WalutaB2b);
        }

        [FriendlyName("Wartość brutto koszyka wraz z kosztem dostawy")]
        public virtual WartoscLiczbowa LacznaWartoscBrutto()
        {
            return new WartoscLiczbowa(CalkowitaWartoscHurtowaBruttoPoRabacie().Wartosc + CenaDostawyBrutto().Wartosc, WalutaKoszyka().WalutaB2b);
        }

        public WartoscLiczbowa CenaDostawyNetto()
        {
            return new WartoscLiczbowa(KosztDostawy() == null ? 0M : KosztDostawy().WyliczCene(this), WalutaKoszyka().WalutaB2b);
        }
        
        public WartoscLiczbowa CalkowitaWartoscCenaKatalogowaNetto()
        {
            var wynik = this.PobierzPozycje.Sum(x => x.WartoscDetalicznaNetto());
            string waluta = this.PobierzPozycje.Any() ? this.PobierzPozycje.First().Produkt.FlatCeny.CenaDetalicznaNetto.Waluta : "";
            return new WartoscLiczbowa(wynik, waluta);
        }
        
        public WartoscLiczbowa CalkowitaWartoscCenaKatalogowaBrutto()
        {
            var wynik = this.PobierzPozycje.Sum(x => x.WartoscDetalicznaBrutto());
            string waluta = this.PobierzPozycje.Any() ? this.PobierzPozycje.First().Produkt.FlatCeny.CenaDetalicznaBrutto.Waluta : "";
            return new WartoscLiczbowa(wynik, waluta);
        }

        public WartoscLiczbowa WartoscVatCenaKatalogowa()
        {
            return CalkowitaWartoscCenaKatalogowaBrutto() - CalkowitaWartoscCenaKatalogowaNetto();
        }
        
        public WartoscLiczbowa WartoscVatCenaHurtowa()
        {
            var a = this.PobierzPozycje.Sum(koszykPozycja => Math.Round(Math.Round((koszykPozycja.Produkt.FlatCeny.CenaHurtowaNetto * koszykPozycja.IloscWJednostcePodstawowej), 2) * (koszykPozycja.Produkt.Vat / 100), 2));
            string waluta = this.PobierzPozycje.Any() ? this.PobierzPozycje.First().CenaNetto.Waluta : "";
            return new WartoscLiczbowa(a, waluta);
        }


        public WartoscLiczbowa CalkowitaWartoscCenaKatalogowaNettoZysk()
        {
            WartoscLiczbowa calkowitaWartoscCenaKatalogowaNetto = CalkowitaWartoscCenaKatalogowaNetto();
            WartoscLiczbowa calkowitaWartoscHurtowaNettoPoRabacie = CalkowitaWartoscHurtowaNettoPoRabacie();
            return calkowitaWartoscCenaKatalogowaNetto.Waluta != calkowitaWartoscHurtowaNettoPoRabacie.Waluta ? new WartoscLiczbowa(0, calkowitaWartoscHurtowaNettoPoRabacie.Waluta) : calkowitaWartoscCenaKatalogowaNetto - calkowitaWartoscHurtowaNettoPoRabacie;
        }


        public WartoscLiczbowa CalkowitaWartoscCenaKatalogowaBruttoZysk()
        {
            return CalkowitaWartoscCenaKatalogowaBrutto() - CalkowitaWartoscHurtowaBruttoPoRabacie();
        }


        public WartoscLiczbowa VatZyskKlienta()
        {
            return WartoscVatCenaKatalogowa() - VatRabat();
        }

        public WartoscLiczbowa CalkowitaWartoscHurtowaBrutto()
        {
            var wynik = this.PobierzPozycje.Aggregate<ModelBLL.Interfejsy.IKoszykPozycja, WartoscLiczbowa>(0, (current, pozycja) => (WartoscLiczbowa)(current + pozycja.Produkt.FlatCeny.CenaHurtowaBrutto * pozycja.IloscWJednostcePodstawowej));
            string waluta = this.PobierzPozycje.Any() ? this.PobierzPozycje.First().Produkt.FlatCeny.CenaHurtowaBrutto.Waluta : "";
            return new WartoscLiczbowa(wynik, waluta);
        }

        public WartoscLiczbowa CalkowitaWartoscHurtowaNetto()
        {
            var wynik = this.PobierzPozycje.Aggregate<ModelBLL.Interfejsy.IKoszykPozycja, WartoscLiczbowa>(0, (current, pozycja) => (WartoscLiczbowa)(current + pozycja.Produkt.FlatCeny.CenaHurtowaNetto * pozycja.IloscWJednostcePodstawowej));
            string waluta = this.PobierzPozycje.Any() ? SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Waluta>(this.PobierzPozycje.First().Produkt.FlatCeny.WalutaId).WalutaB2b : "";
            return new WartoscLiczbowa(wynik, waluta);
        }

        [FriendlyName("Całkowita wartość hutrowa netto (po rabacie)")]
        public WartoscLiczbowa CalkowitaWartoscHurtowaNettoPoRabacie()
        {
            var a = this.PobierzPozycje.Sum(x => x.WartoscNetto);
            string waluta = this.PobierzPozycje.Any() ? this.PobierzPozycje.First().CenaNetto.Waluta : "";
            return new WartoscLiczbowa(a, waluta);
        }

        [FriendlyName("Całkowita wartość hutrowa netto (po rabacie)")]
        public WartoscLiczbowa CalkowitaWartoscHurtowaBruttoPoRabacie()
        {
            var a = this.PobierzPozycje.Sum(x => x.WartoscBrutto);
            string waluta = this.PobierzPozycje.Any() ? this.PobierzPozycje.First().CenaNetto.Waluta : "";
            return new WartoscLiczbowa(a, waluta);
        }


        public WartoscLiczbowa VatRabat()
        {
            var pozycje = PobierzPozycje;
            var a = pozycje.Sum(koszykPozycja => Math.Round(Math.Round((koszykPozycja.Produkt.FlatCeny.CenaHurtowaNetto * koszykPozycja.IloscWJednostcePodstawowej) * (1 - (koszykPozycja.CalkowityRabat() / 100)), 2) * (koszykPozycja.Produkt.Vat / 100), 2));
            string waluta = pozycje.Any() ? pozycje.First().CenaNetto.Waluta : "";
            return new WartoscLiczbowa(a, waluta);
        }
        public decimal CalkowitaObjetoscKoszyka()
        {
            return PobierzPozycje.Where(pozycja => pozycja.Produkt.Objetosc.HasValue).Sum(p => p.Produkt.Objetosc.Value*p.Ilosc);
        }

        public WartoscLiczbowa CalkowityRabatNetto()
        {
            WartoscLiczbowa calkowitaWartoscHurtowaNetto = CalkowitaWartoscHurtowaNetto();
            var wynik = Math.Round(calkowitaWartoscHurtowaNetto - CalkowitaWartoscHurtowaNettoPoRabacie(), 2);
            return new WartoscLiczbowa(wynik, calkowitaWartoscHurtowaNetto.Waluta);
        }

        public WartoscLiczbowa CalkowityRabatBrutto()
        {
            WartoscLiczbowa calkowitaWartoscHurtowaBrutto = CalkowitaWartoscHurtowaBrutto();
            var wynik = Math.Round(calkowitaWartoscHurtowaBrutto - CalkowitaWartoscHurtowaBruttoPoRabacie(), 2);
            return new WartoscLiczbowa(wynik, calkowitaWartoscHurtowaBrutto.Waluta);
        }

        public WartoscLiczbowa CalkowityRabatVat()
        {
            var wartoscVatCenaHurtowa = WartoscVatCenaHurtowa();
            var wynik = Math.Round(wartoscVatCenaHurtowa - VatRabat(), 2);
            return new WartoscLiczbowa(wynik, wartoscVatCenaHurtowa.Waluta);
        }

        /// <summary>
        /// Wylicza ile całkowicie procent rabatu ma klient
        /// </summary>
        /// 
        public decimal CalkowityRabatProcent()
        {
            var calkowitaWartoscHurtowaNetto = CalkowitaWartoscHurtowaNetto();
            if (calkowitaWartoscHurtowaNetto != 0)
            {
                return Math.Round(CalkowityRabatNetto()/calkowitaWartoscHurtowaNetto*100, 2);
            }
            return 0;
        }

        public WartoscLiczbowa CenaDostawyBrutto()
        {
            var cenaDostawyNetto = CenaDostawyNetto();
            if (KosztDostawy() != null)
            {
                if (KosztDostawy().ProduktDostawy != null)
                {
                    return new WartoscLiczbowa(Kwoty.WyliczBrutto(cenaDostawyNetto, KosztDostawy().ProduktDostawy.Vat, Klient),
                        WalutaKoszyka().WalutaB2b);
                }
                return cenaDostawyNetto;
            }
            return 0;
        }

        //Używane w module RozbijanieDostawyNaPozycje
        [Ignore]
        public bool NieDodawajDostawyDoKoszyka { get; set; }

        private ISposobDostawy _sposoby;

        public ISposobDostawy KosztDostawy()
        {
            if (Id != 0 && _sposoby == null && KosztDostawyId != null)
            {
                try
                {
                    ZadanieCalegoKoszyka zk =SolexBllCalosc.PobierzInstancje.ZadaniaBLL.PobierzZadania<ISposobDostawy,ZadanieCalegoKoszyka >(Klient.JezykId,Klient).FirstOrDefault(x=>x.Id== KosztDostawyId);
                    
                    if (zk!=null && zk.WykonajZadanie(this))
                    {
                        _sposoby = (ISposobDostawy)zk;
                    }
                    else
                    {
                        KosztDostawyId = null;
                        SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(this);
                    }

                }
                catch (Exception)
                {

                    KosztDostawyId = null;
                    SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(this);
                }

            }
            return _sposoby;
        }
        private ISposobPlatnosci _sposobyPlatnosci;

        [Ignore]
        public  ISposobPlatnosci PlatnoscObiekt
        {
            get
            {
                if (_sposobyPlatnosci == null && PlatnoscId != null)
                {
                    try
                    {
                        ZadanieCalegoKoszyka zk = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.PobierzZadania<ISposobPlatnosci, ZadanieCalegoKoszyka>(Klient.JezykId, Klient).FirstOrDefault(x => x.Id == PlatnoscId); 
                      
                        if (zk!=null&& zk.WykonajZadanie(this))
                        {
                            _sposobyPlatnosci = (ISposobPlatnosci)zk;
                        }
                        else
                        {
                            PlatnoscId = null;
                            SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(this);
                        }

                    }
                    catch (Exception)
                    {

                        PlatnoscId = null;
                        SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(this);
                    }

                }
                return _sposobyPlatnosci;
            }

            set { _sposobyPlatnosci = value; }
        }

        [Ignore]
        public string Platnosc
        {
            get { return PlatnoscObiekt == null ? null : string.Format("{0} {1}", PlatnoscObiekt.Nazwa, PlatnoscObiekt.Termin == 0 ? "" : PlatnoscObiekt.Termin.ToString(CultureInfo.InvariantCulture)); }
        }


        //autouzupelnienianie przy selecie
        [Ignore]
        public IKlient Klient { get; set; }

        [Ignore]
        public SzablonAkceptacjiBll AktualnySzablonAkceptacji
        {
            get { return SolexBllCalosc.PobierzInstancje.Klienci.PobierzSzablonAkceptacji(Klient,SolexBllCalosc.PobierzInstancje.Koszyk.PrzekroczoneLimityKoszyka(this)); }
        }

        /// <summary>
        /// Sumuje
        /// </summary>
        /// <returns></returns>
        public decimal PobierzWartoscNetto()
        {
            decimal wartosc = 0;
            var pozycje = PobierzPozycje;
            for (int i = 0; i < pozycje.Count; i++)
            {
                decimal wartosc_Pozycji = pozycje[i].WartoscNetto;
                wartosc += wartosc_Pozycji;
            }
            return wartosc;
        }
        public decimal PobierzWartoscBrutto()
        {
            decimal wartosc = 0;
            var pozycje = PobierzPozycje;
            for (int i = 0; i < pozycje.Count; i++)
            {
                decimal wartosc_Pozycji = pozycje[i].WartoscBrutto;
                wartosc += wartosc_Pozycji;
            }
            return wartosc;
        }

        public virtual WartoscLiczbowa WagaCalokowita()
        {
            decimal wartosc = 0;
            var pozycje = PobierzPozycje;
            for (int i = 0; i < pozycje.Count; i++)
            {
                var waga = pozycje[i].WagaPozycji();
                if (waga.HasValue)
                {
                    wartosc += waga.Value;
                }
              
            }
            return wartosc;
        }

       

        private Adres _wybrany;

        [Ignore]
        public Adres Adres
        {
            get
            {

                if (_wybrany == null && AdresId != null)
                {
                    _wybrany = Klient.Adresy.FirstOrDefault(p => p.Id == AdresId) as Adres;
                }
                return _wybrany;
            }
        }

        
        /// <summary>
        /// Całkowita wartość hutrowa vat (po rabacie)
        /// </summary>
        [FriendlyName("Całkowita wartość hutrowa vat (po rabacie)")]
        public WartoscLiczbowa WartoscVat()
        {
            return new WartoscLiczbowa(PobierzWartoscBrutto() - PobierzWartoscNetto(), WalutaKoszyka().WalutaB2b);
        }

        public Waluta WalutaKoszyka()
        {
            if (PobierzPozycje.Count > 0)
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Waluta>(PobierzPozycje[0].Produkt.FlatCeny.WalutaId);
            }
            //po co waluta koszyka jak nie ma koszyka?
            //    throw new Exception("po co to skoro nie ma koszyka");
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Waluta>(null).First();
        }

        
        public void DodajaAutomatyczny(int produkt, decimal ilosc, decimal cena, int dodajacezadanie)
        {
            ProduktBazowy pb = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(produkt,  SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
            int typ = -((ilosc + produkt + cena + dodajacezadanie).GetHashCode());

            KoszykPozycje kp = PobierzPozycje.FirstOrDefault(x => x.ProduktId == produkt && x.TypPozycji == TypPozycjiKoszyka.Automatyczny && x.Hash == typ);

            if (kp == null)
            {
                kp = new KoszykPozycje { Hash = typ,TypPozycji = TypPozycjiKoszyka.Automatyczny,Klient = Klient};
                DodajPozycjeDoKoszyka(kp);
            }
            kp.DodajaceZadanie = dodajacezadanie;
            kp.Ilosc = ilosc;
            kp.JednostkaId = pb.JednostkaPodstawowa.Id;
            kp.KoszykId = Id;
            kp.ProduktBazowyId = pb.Id;
            kp.ProduktId = produkt;
            kp.DataDodania = DateTime.Now;
            if (cena > 0)
            {
                kp.WymuszonaCenaNettoModul = cena;
            }
        }

        [Ignore]
        public bool WEdycji { get; set; }


        public virtual string Uwagi { get; set; }

        
        public virtual string MagazynDlaMm { get; set; }


        //private string _magazynRealizujacy = null;
        /// <summary>
        /// Magazyn z jakiego należy wystawić dokument sprzedaży. Jeśli magazyn podstawowy jest inny niż realizujący to wystawiana jest MM z podstawowego na realizujący
        /// </summary>
        [WidoczneListaAdmin(true, false, false, false)]
        [FriendlyName("Magazyn realizujący (na jaki będzi zapisany dokument)", FriendlyOpis = "Magazyn z jakiego należy wystawić dokument sprzedaży. Jeśli magazyn dla mm-ek jest inny niż realizujący to wystawiana jest MM z podstawowego na realizujący")]
        public string MagazynRealizujacy { get; set;
            //get
            //{
            //    if (!string.IsNullOrEmpty(_magazynRealizujacy))
            //    {
            //        return _magazynRealizujacy;
            //    }
            //    var magazyny = SolexBllCalosc.PobierzInstancje.Koszyk.PobierzDostepneMagazyny(this, Klient).FirstOrDefault();
            //    return magazyny;
            //}
            //set { _magazynRealizujacy = value; }
        }

        //private int? _idPlatnosci=null;
        public int? PlatnoscId { get; set; }
        //public int? PlatnoscId
        //{
        //    get
        //    {
        //        if (_idPlatnosci.HasValue)
        //        {
        //            return _idPlatnosci;
        //        }
        //        var platn = SolexBllCalosc.PobierzInstancje.Koszyk.DostepneSposobyPlantosci(this);
        //        //if (platn.Count == 1)
        //        //{
        //        //    return platn[0].Id;
        //        //}
        //        return null;
        //    }
        //    set { _idPlatnosci = value; }
        //}

       // private int? _kosztDostawy=null;
        public int? KosztDostawyId { get; set; }
        //public int? KosztDostawyId
        //{
        //    get
        //    {
        //        if (_kosztDostawy.HasValue)
        //        {
        //            return _kosztDostawy;
        //        }
        //        var mod = SolexBllCalosc.PobierzInstancje.Koszyk.DostepneSposobyDostawy(this);

        //        if (mod.Count == 1)
        //        {
        //            return mod[0].Id;
        //        }
        //        return null;
        //    }
        //    set { _kosztDostawy = value; }
        //}
        private long? _adresId;
        public long? AdresId
        {
            get
            {
                if (_adresId.HasValue)
                {
                    return _adresId;
                }
                return Klient.Adresy.Count(x => x.TypAdresu != TypAdresu.Jednorazowy) == 1 ? Klient.Adresy.First(x => x.TypAdresu != TypAdresu.Jednorazowy).Id : (long?)null;
            }
            set { _adresId = value; }
        }

        /// <summary>
        /// Pole jest ignorowane ze względu na fakt iż jest ono wykorzystywane po finalizacji przez modul - nigdy by ta wartość nie była uzupełniona w bazie
        /// </summary>
        [Ignore]
        public string DefinicjaDokumentuERP { get; set; }

        public void DodajPozycjeDoKoszyka(KoszykPozycje pozycja)
        {
            Sortowanie sortowanie = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzSortowanie(Klient, TypUstawieniaKlienta.KolumnaSortowaniaKoszykLista);
            if (_pozycje==null || _pozycje.First().Value == null)
            {
                _pozycje = new Dictionary<string, List<KoszykPozycje>>() { { sortowanie.WersjaTekstowa, new List<KoszykPozycje>() } };
            }
            _pozycje.First().Value.Add(pozycja);
        }

        private Dictionary<string, int> _rodzinyWPozycjachIIleDzieci = null;

        public Dictionary<string, int> PozycjeRodzinyIIleDzieci()
        {
            if (_rodzinyWPozycjachIIleDzieci == null)
            {
                _rodzinyWPozycjachIIleDzieci = this.PobierzPozycje.Where(x=>x.Produkt.Rodzina != null).GroupBy(x => x.Produkt.Rodzina).ToDictionary(x => x.Key, x => x.Count());
            }
            return _rodzinyWPozycjachIIleDzieci;
        }

        //tu nie moze byc interefejsu bo MVC nie radzi sobie
        [Ignore]
        public virtual List<KoszykPozycje> PobierzPozycje
        {
            get
            {
                Sortowanie sortowanie = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzSortowanie(Klient, TypUstawieniaKlienta.KolumnaSortowaniaKoszykLista);
                if (_pozycje==null || _pozycje.First().Key !=sortowanie.WersjaTekstowa)
                {
                    _pozycje = new Dictionary<string, List<KoszykPozycje>>() { { sortowanie.WersjaTekstowa, new List<KoszykPozycje>() } };
                    if (this.Id == 0)
                    {
                        return _pozycje.First().Value;
                    }
                    List<KoszykPozycje> produktyWKoszyku = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KoszykPozycje>(Klient.JezykId, Klient, x => x.KoszykId == Id, this).ToList();
                    if (!produktyWKoszyku.Any())
                    {
                         return _pozycje.First().Value;
                    }
                    HashSet<long> idProduktowDostepnychDlaKlienta = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(Klient);
                    List<KoszykPozycje> pozycjeDostepneDlaKlienta = produktyWKoszyku.Where(x => idProduktowDostepnychDlaKlienta.Contains(x.ProduktId)).ToList();

                    if (sortowanie.Pola.Any())
                    {
                        try
                        {
                            pozycjeDostepneDlaKlienta = SolexBllCalosc.PobierzInstancje.Szukanie.SortujObiekty(pozycjeDostepneDlaKlienta, sortowanie).ToList();
                        }
                        catch
                        {
                            //jesli się nie udało posortować wg ustawień koszyka to próbuje ustawić wg ustawień domyślnych
                            sortowanie = SolexBllCalosc.PobierzInstancje.Konfiguracja.DostepneSortowanieKoszyka.First();
                            pozycjeDostepneDlaKlienta = SolexBllCalosc.PobierzInstancje.Szukanie.SortujObiekty(pozycjeDostepneDlaKlienta, sortowanie).ToList();
                        }
                    }
                    Dictionary<string,List< KoszykPozycje >>wynik = new Dictionary<string, List<KoszykPozycje>>();
                    //Posortowane elementy skladamy w rodziny
                    foreach (var pozycja in pozycjeDostepneDlaKlienta)
                    {
                        string rodzina = pozycja.Produkt.Rodzina ?? "";
                        if (!wynik.ContainsKey(rodzina))
                        {
                            wynik.Add(rodzina, new List<KoszykPozycje>());
                        }
                        wynik[rodzina].Add(pozycja);
                    }
                    

                    _pozycje = new Dictionary<string, List<KoszykPozycje>>() { { sortowanie.WersjaTekstowa, wynik.SelectMany(x=>x.Value).ToList()} };
                }
                return _pozycje.First().Value;
            }
            set
            {
                string klucz = _pozycje.First().Key ?? "";
                _pozycje = new Dictionary<string, List<KoszykPozycje>>() { { klucz, value } };
            }
        }

        private Dictionary<long, decimal> _iloscProduktow;
        public Dictionary<long, decimal> PobierzIlosciProduktowWKoszyku()
        {
            if (_iloscProduktow == null)
            {
                _iloscProduktow= PobierzPozycje.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => x.Sum(y => y.Ilosc));
            }
            return _iloscProduktow;
        }


        public void UstawStatus(IKlient aktualnyKlient, StatusKoszyka statusKoszyka)
        {
            List<StatusKoszykaHistoria> historia = HistoriaZmianStatusow??new List<StatusKoszykaHistoria>();

            historia.Add(new StatusKoszykaHistoria{Data = DateTime.Now,KlientId = aktualnyKlient.Id,Staus = statusKoszyka});
            HistoriaZmianStatusow = historia;
        }

        public bool CzyKoszykDoAkceptacji(IKlient aktualnyKlient, out SzablonAkceptacjiPoziomy poziom)
        {
            poziom = null;
            var szablon = AktualnySzablonAkceptacji;
            if (szablon == null)
            {
                return false;
            }
            var poziomy = szablon.Poziomy;
            int pozioml = 0;
            for (int i = 0; i < poziomy.Count; i++)
            {
                if (poziomy[i].Klienci.Contains(aktualnyKlient.Id))
                {
                    pozioml = i+1;
                    break;
                }
            }
            if (pozioml == poziomy.Count)//ostatni poziom
            {
          
                return false;
            }
            poziom = poziomy[pozioml];
            return true;
        }

        [Ignore]
        public bool UkryjAdresy { get; set; }

        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        public string Nazwa { get; set; }
        public long KlientId { get; set; }
        public TypKoszyka Typ { get; set; }
        //public int StatusId{get;set;}
        public bool Aktywny { get; set; }
        //public virtual string Parametry { get; set; }
        public DateTime DataModyfikacji { get; set; }

        private Komunikat[] _komunikaty = null;
        private long idKlientaDlaKtoregoKoszykBylPRzeliczany = 0;
        /// <summary>
        /// metoda przelicza zadania koszykowe - zwraca tablice komunikatów - uwaga! dane sa cachowana, dopiero po zmianie czegos w koszyku zostaja przeladowane
        /// </summary>
        /// <returns></returns>
        public Komunikat[] PrzeliczModulyKoszykowe_PobierzKomunikaty()
        {
            long idKlienta = this.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta?.Id ?? this.KlientId;
            if (_komunikaty == null || idKlienta != idKlientaDlaKtoregoKoszykBylPRzeliczany)
            {
                _komunikaty = SolexBllCalosc.PobierzInstancje.Koszyk.WykonajModulyKoszykowe(this);
                idKlientaDlaKtoregoKoszykBylPRzeliczany = idKlienta;
            }
            return _komunikaty;
        }

        [Ignore]
        public IKlient PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta { get; set; }

        public List<StatusKoszykaHistoria> HistoriaZmianStatusow { get; set; }

        public List<long> KlienciMogacyAkceptowacKoszyk { get; set; }

        private int? _numerZamowieniaId;
        [Ignore]
        public string NumerZamowienia {
            get
            {
                DodatkowePoleKoszyka numer;
                if (DodatkoweParametry == null)
                {
                    return null;
                }
                if (_numerZamowieniaId.HasValue)
                {
                    if (DodatkoweParametry.TryGetValue(_numerZamowieniaId.Value, out numer))
                    {
                        return numer.WybraneWartosciString;
                    }
                    return null;
                }
                numer = DodatkoweParametry.Values.FirstOrDefault(p => p.Symbol == SymboleModulowKoszyka.NumerWlasny.ToString());
                if (numer != null)
                {
                    _numerZamowieniaId = numer.IdModulu;
                    return numer.WybraneWartosciString;
                }
                return null;
            }
        }

        /// <summary>
        /// Pole jest ignorowane ze względu na fakt iż jest ono wykorzystywane po finalizacji przez modul - nigdy by ta wartość nie była uzupełniona w bazie
        /// </summary>
        [Ignore]
        public string KategoriaZamowienia { get; set; }

        private int? _dostawaModultId;
        [Ignore] 
        public DateTime? TerminDostawy {
            get
            {
                DateTime wynik;
                DodatkowePoleKoszyka termin;
                if (DodatkoweParametry == null)
                {
                    return null;
                }
                if (_dostawaModultId.HasValue)
                {
                    if (DodatkoweParametry.TryGetValue(_dostawaModultId.Value, out termin))
                    {
                        
                        if (DateTime.TryParse(termin.WybraneWartosciString, out wynik))
                        {
                            return wynik;
                        }
                    }
                    return null;
                }
                termin = DodatkoweParametry.Values.FirstOrDefault(p => p.Symbol == SymboleModulowKoszyka.DataRealizacjiZamowienia.ToString());
                if (termin != null && DateTime.TryParse(termin.WybraneWartosciString, out wynik))
                {
                    _dostawaModultId = termin.IdModulu;
                    return wynik;
                }
                return null;
            }
        }

        /// <summary>
        /// Dodatkowe pola które będą ustawiane na zamówieniu przy imporcie do ERP-a
        /// </summary>
        public string DodatkowePolaErp { get; set; }

        public DodatkowePoleKoszyka PobierzDodatkowyParemetr(int idModulu)
        {
            if (DodatkoweParametry!=null && DodatkoweParametry.ContainsKey(idModulu))
            {
                return DodatkoweParametry[idModulu];
            }
            return null;
        }

        public void DodajDodatkowyParametr(int idModulu, string symbol, string[] wartosc)
        {
            Dictionary<int, DodatkowePoleKoszyka> tmp = DodatkoweParametry?? new Dictionary<int, DodatkowePoleKoszyka>();

            DodatkowePoleKoszyka wpis = new DodatkowePoleKoszyka() { Symbol = symbol, WybraneWartosci = wartosc, IdModulu = idModulu };
            if (tmp.ContainsKey(idModulu))
            {
                tmp[idModulu] = wpis;
            }
            else
            {
                tmp.Add(idModulu, wpis);
            }
            DodatkoweParametry = tmp;
        }
    }
}

           


