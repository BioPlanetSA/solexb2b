using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using System.Linq;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using System.Text;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{
    [Alias("KoszykPozycje")]
    public class KoszykPozycjaWyliczonaCenaBLL : KoszykPozycje
    {
        public KoszykPozycjaWyliczonaCenaBLL(IKoszykPozycja baza,IProduktKlienta produkt)
            : base(baza)
        {
            Produkt = produkt;
        }
        [Ignore]
        public override IProduktKlienta Produkt { get; }
    }

    [NieSprawdzajCzyIsnieje]
    public class KoszykPozycje : IKoszykPozycja, IHasLongId
    {
        //public KoszykPozycje(IKoszykPozycja baza):base(baza) { }

        private ParametryIlosciProduktu _dodawanieProduktu = null;

        [Ignore]
        public ParametryIlosciProduktu DodawanieProduktu
        {
            get {
                //warunek z id != 0 dlatego zeby podczas bindowanie mvc nie ladowac smieci
                if (_dodawanieProduktu == null && (this.Id != 0 || this.TypPozycji== TypPozycjiKoszyka.Automatyczny))
                {
                    _dodawanieProduktu = new ParametryIlosciProduktu(this, false, null, true);
                }
                return _dodawanieProduktu;
            }
            set { _dodawanieProduktu = value; } 
        }

        [Ignore]
        public bool ZmianianaIlosc { get; set; }

        public void ZmienDodatkowyRabat(decimal wartosc, string opis, TrybLiczeniaRabatuWKoszyku tryb, string dodatkoweInfoDymek = null)
        {
            if (Produkt.NiePodlegaRabatowaniu)
            {
                return;
            }
            switch (tryb)
            {
                    case TrybLiczeniaRabatuWKoszyku.SUMUJ:
                    RabatDodatkowy += wartosc;
                    break;
                    case TrybLiczeniaRabatuWKoszyku.NADPISZ:
                    RabatDodatkowy = wartosc;
                    break;
            }

            PowodDodatkowegoRabatu_DodatkoweInfoDymek = dodatkoweInfoDymek;
            PowodDodatkowegoRabatu = opis;
        }

        public bool KupowanyWJednostcePodstawowej()
        {
            return Jednostka().Id == Produkt.JednostkaPodstawowa.Id;
        }
        public JednostkaProduktu Jednostka()
        {
            var j = Produkt.Jednostki.FirstOrDefault(x => x.Id == JednostkaId);
            if (j != null)
            {
                return j;
            }
            return Produkt.JednostkaPodstawowa;
        }

        [FriendlyName("Opis dodatkowego rabatu")]
        [Ignore]
        public string OpisRabatu
        {
            get
            {
                WartoscLiczbowaZaokraglana wartoscnetto = WyliczDodatowyRabat();
                if (wartoscnetto.Wartosc.ToString("0.##") == "0")
                {
                    return "";
                }
                string fraza = "";
                string powod = string.IsNullOrEmpty(PowodDodatkowegoRabatu) ? (RabatDodatkowy > 0 ? SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(Klient.JezykId,"rabat") :  SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(Klient.JezykId, "dopłatę")) : PowodDodatkowegoRabatu;
                switch (SolexBllCalosc.PobierzInstancje.Konfiguracja.PokazywanieRabatu)
                {
                    case SposobPokazywaniaDodatkowegoRabatu.RabatKwota:
                        fraza =  SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(Klient.JezykId, "Zawiera {0} w wysokości <strong>{1}</strong>% (<strong>{2}</strong> netto / <strong>{3}</strong> brutto)");
                        break;
                    case SposobPokazywaniaDodatkowegoRabatu.Kwota:
                        fraza =  SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(Klient.JezykId, "Zawiera {0} w wysokości (<strong>{2}</strong> netto / <strong>{3}</strong> brutto)");
                        break;
                    case SposobPokazywaniaDodatkowegoRabatu.Rabat:
                        fraza =  SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(Klient.JezykId, "Zawiera {0} w wysokości <strong>{1}</strong>%");
                        break;
                }
                return string.Format(fraza, powod, Math.Abs(RabatDodatkowy).ToString("0.##"), Math.Round(wartoscnetto,2,MidpointRounding.AwayFromZero), Math.Round(WyliczDodatowyRabatBrutto(), 2, MidpointRounding.AwayFromZero));
            }
        }
        private WartoscLiczbowaZaokraglana WyliczDodatowyRabat()
        {
            return new WartoscLiczbowaZaokraglana(Math.Abs(Produkt.FlatCeny.CenaNettoDokladna * RabatDodatkowy / 100M), Waluta());
        }
        private WartoscLiczbowaZaokraglana WyliczDodatowyRabatBrutto()
        {
            return new WartoscLiczbowaZaokraglana(Math.Abs(Produkt.FlatCeny.CenaBrutto * RabatDodatkowy / 100M), Waluta());
        }

        public string Waluta()
        {
            return SolexBllCalosc.PobierzInstancje.Konfiguracja.SlownikWalut[Produkt.FlatCeny.WalutaId].WalutaB2b;
        }

        protected IProduktKlienta _produkt;

        [Ignore]
        public virtual IProduktKlienta Produkt
        {
            get
            {
                if (_produkt == null)
                {
                    _produkt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(ProduktId, Klient);
                    if (_produkt == null) //chcemy dodać produkt do koszyka, który nie jest oficjalnie dla niego  dostępny
                    {
                        var bazowy = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(ProduktId);
                        _produkt = new ProduktKlienta(bazowy, Klient);
                        //_produkt.IdCechPRoduktu = new HashSet<long>();
                    }
                }
                return _produkt;
            }
        }

        [Ignore]
        public IKlient Klient { get; set; }

        [Ignore]
        public virtual decimal IloscWJednostcePodstawowej
        {
            get { return Ilosc * Jednostka().Przelicznik; }
            set { Ilosc = value / Jednostka().Przelicznik; }
        }

        [Ignore]
        [FriendlyName("Wartość netto")]
        public virtual  WartoscLiczbowa WartoscNetto
        {
            get
            {
                decimal wartosc =DoklanaCenaNetto()*Ilosc;
                return new WartoscLiczbowa(wartosc, Waluta());
            }
        }
        [Ignore]
        [FriendlyName("Wartość brutto")]
        public virtual WartoscLiczbowa WartoscBrutto
        {
            get
            {
                decimal wartosc_Pozycji =Math.Round(Kwoty.WyliczBrutto(WartoscNetto.Wartosc, Produkt.Vat),2);
                return new WartoscLiczbowa(wartosc_Pozycji, Waluta());
            }
        }

        [Ignore]
        [FriendlyName("Wartość vat")]
        public WartoscLiczbowa WartoscVat
        {
            get { return new WartoscLiczbowa((decimal)WartoscBrutto - (decimal)WartoscNetto, Waluta()); }
        }

        [FriendlyName("Wartość detaliczna netto")]
        public WartoscLiczbowa WartoscDetalicznaNetto()
        {
            return new WartoscLiczbowa(Math.Round(Produkt.FlatCeny.CenaDetalicznaNetto.Wartosc*Ilosc, 2), Waluta());
        }

        [FriendlyName("Wartość detaliczna brutto")]
        public WartoscLiczbowa WartoscDetalicznaBrutto()
        {
            decimal wartosc_Pozycji = Math.Round(Kwoty.WyliczBrutto(WartoscDetalicznaNetto().Wartosc, Produkt.Vat), 2);
            return new WartoscLiczbowa(wartosc_Pozycji, Waluta());
        }

        [Ignore]
        [FriendlyName("Wartość detaliczna vat")]
        public WartoscLiczbowa WartoscDetalicznaVat
        {
            get { return new WartoscLiczbowa((decimal)WartoscDetalicznaBrutto() - (decimal)WartoscDetalicznaNetto(), Waluta()); }
        }

        [FriendlyName("Waga")]
        public decimal? WagaPozycji()
        {
            if (Produkt.Waga != null)
            {
                return IloscWJednostcePodstawowej * Produkt.Waga.Value;
            }
            return null;
        }

        [FriendlyName("Cena brutto")]
        public virtual WartoscLiczbowa CenaBrutto()
        {
            if (WymuszonaCenaNettoPrzedstawiciel.HasValue)
            {
                return new WartoscLiczbowa(Kwoty.WyliczBrutto(WymuszonaCenaNettoPrzedstawiciel.Value, Produkt.Vat),
                    Waluta());
            }
            return
                new WartoscLiczbowa(
                    ((WymuszonaCenaNettoModul.HasValue
                        ? (Kwoty.WyliczBrutto(WymuszonaCenaNettoModul.Value, Produkt.Vat))
                        : Produkt.FlatCeny.CenaBrutto.Wartosc)*(100M - RabatDodatkowy) /100M)*Jednostka().Przelicznik, Waluta());
        }

        [Ignore]
        [FriendlyName("Cena netto")]
        public virtual WartoscLiczbowa CenaNetto
        {
            get { return new WartoscLiczbowa(DoklanaCenaNetto(), Waluta()); }
        }

        public decimal DoklanaCenaNetto()
        {
            return CenaNettoPodstawowa*Jednostka().Przelicznik;
        }
        [Ignore]
        public StanKoszyk StanKoszyk { get; set; }
        public decimal DoklanaCenaBruto()
        {
            return Kwoty.WyliczBrutto(DoklanaCenaNetto(), Produkt.Vat);
        }

        [Ignore]
        [FriendlyName("Cena po rabacie netto w jednostce podstawowej - niezaokrąglana")]
        public decimal CenaNettoPodstawowa
        {
            get
            {
                if (WymuszonaCenaNettoPrzedstawiciel.HasValue)
                {
                    return WymuszonaCenaNettoPrzedstawiciel.Value;
                }
                return CenaNettoPodstawowa_BezCenyPrzedstawiciela();
            }
        }

        [FriendlyName("Cena po rabacie netto w jednostce podstawowej - niezaokrąglana - bez uwzględnienia ceny przedstawiciela")]
        public decimal CenaNettoPodstawowa_BezCenyPrzedstawiciela()
        {
        //   var netto = ((WymuszonaCenaNettoModul ?? Produkt.FlatCeny.CenaNettoDokladna)); zmiana bo cena netto dokładna jest ceną w walucie obcej a nie poprzewalutowaniu 
            var netto = ((WymuszonaCenaNettoModul ?? Produkt.FlatCeny.CenaNetto));
            return (netto*(100M - RabatDodatkowy) /100M);
        }

        [FriendlyName("Cena w punktach")]
        public decimal CenaWPunktach()
        {
            return Produkt.CenaWPunktach;
        }

        [FriendlyName("Cena po rabacie brutto w jednostce podstawowej - niezaokrąglana")]
        public decimal CenaBruttoPodstawowa()
        {
            return Kwoty.WyliczBrutto(CenaNettoPodstawowa, Produkt.Vat);
        }

        //[Ignore]
        //[FriendlyName("Dodatkowy rabat")]
        //public WartoscLiczbowaZaokraglana rabat_dodatkowy
        //{
        //    get
        //    {
        //        // decimal? wartoscprocent = KonfekcjaBLL.PobierzInstancje.UstawKonfekcje(Koszyk,this);             
        //        return new WartoscLiczbowaZaokraglana(RabatDodatkowy);//+wartoscprocent.GetValueOrDefault());
        //    }
        //    set { RabatDodatkowy = value; }
        //}

        [FriendlyName("Całkowity rabat")]
        public WartoscLiczbowa CalkowityRabat()
        {
            decimal wsp = Produkt.FlatCeny.CenaHurtowaNetto == 0 ? 0 : (1 - (CenaNettoPodstawowa/Produkt.FlatCeny.CenaHurtowaNetto));
            return new WartoscLiczbowa(wsp*100);
        }

       
        

        //[Ignore]
        //public ParametryIlosciProduktu ParametryIlosciProduktu
        //{
        //    get { return new ParametryIlosciProduktu(this, false, null , true); }
        //}

        [Ignore]
        public string KolorTla { get; set; }
          [Ignore]
        public int KolejnoscSortowania { get; set; }

          [Ignore]
          public string PieczatkaGrafika { get; set; }


        [PrimaryKey]
        [UpdateColumnKey]
        [AutoIncrement]
        [FriendlyName("ID pozycji")]
        public long Id { get; set; }

        public long KoszykId { get; set; }

        [FriendlyName("Numer id produkt")]
        public long ProduktId { get; set; }

        [FriendlyName("Ilość w koszyku")]
        public virtual decimal Ilosc { get; set; }

        [FriendlyName("Data dodania")]
        public DateTime DataDodania { get; set; }

        [FriendlyName("Wybrana jednostka(liczba)")]
        public long? JednostkaId { get; set; }

        public string PowodDodatkowegoRabatu { get; set; }

        [Ignore]
        public string PowodDodatkowegoRabatu_DodatkoweInfoDymek { get; set; }

        [FriendlyName("Dodatkowy rabat")]
        public decimal RabatDodatkowy { get; set; }

        public long? PrzedstawicielId { get; set; }

        public decimal? WymuszonaCenaNettoPrzedstawiciel { get; set; }

        public DateTime? DataZmiany { get; set; }

        public int? Hash { get; set; }

        public IndywidualizacjaWartosc[] Indywidualizacja { get; set; }

        public string OpisIndywidualizacji()
        {
            if (Indywidualizacja == null)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var indywidualizacjaWartosc in Indywidualizacja)
            {
                string nazwa = indywidualizacjaWartosc.Indywidualizacja.DodajNazweDoOpisu ? $"{indywidualizacjaWartosc.Indywidualizacja.Nazwa}:" : "";
                sb.AppendLine($"{nazwa}{indywidualizacjaWartosc.Wartosc}");
            }
            return sb.ToString();
        }
        //[Ignore]
        //public IndywidualizacjaWartosc[] TablicaIndywidualizacji
        //{
        //    get
        //    {
        //        IndywidualizacjaWartosc[] wynik = new IndywidualizacjaWartosc[Indywidualizacja.Count];
        //        if (Indywidualizacja == null && !Indywidualizacja.Any())
        //        {
        //            return null;
        //        }
        //        int i = 0;
        //        foreach (KeyValuePair<long, string> parametr in Indywidualizacja)
        //        {
        //            var indywidualizacja = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Indywidualizacja>(x => x.Id == parametr.Key, Klient);
        //            IndywidualizacjaWartosc wartosc = new IndywidualizacjaWartosc(indywidualizacja, ProduktId, parametr.Value);
        //            wynik[i] = wartosc;
        //            i++;
        //        }
        //        return wynik;
        //    }
        //}

        public int? DodajaceZadanie { get; set; }
        public string Opis { get; set; }
        public decimal? WymuszonaCenaNettoModul { get; set; }
        public long ProduktBazowyId { get; set; }
        public KoszykPozycje()
        {
            TypPozycji = TypPozycjiKoszyka.Zwykly;
        }

        public KoszykPozycje(IKoszykPozycja baza)
        {
            if (baza == null) return;
            Id = baza.Id;
            KoszykId = baza.KoszykId;
            ProduktId = baza.ProduktId;
            Ilosc = baza.Ilosc;
            DataDodania = baza.DataDodania;
            JednostkaId = baza.JednostkaId;
            PowodDodatkowegoRabatu = baza.PowodDodatkowegoRabatu;
            RabatDodatkowy = baza.RabatDodatkowy;
            PrzedstawicielId = baza.PrzedstawicielId;
            WymuszonaCenaNettoPrzedstawiciel = baza.WymuszonaCenaNettoPrzedstawiciel;
            WymuszonaCenaNettoModul = baza.WymuszonaCenaNettoModul;
            DataZmiany = baza.DataZmiany;
            Hash = baza.Hash;
            Indywidualizacja = baza.Indywidualizacja;
            DodajaceZadanie = baza.DodajaceZadanie;
            TypPozycji = baza.TypPozycji;
            ProduktBazowyId = baza.ProduktBazowyId;
        }

        public TypPozycjiKoszyka TypPozycji { get; set; }
    }
}
