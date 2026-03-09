using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Helper
{
    public class EppHelper
    {
        private  static readonly EppHelper Instancja=new EppHelper();

        public static EppHelper PobierzInstancje 
        {
            get
            {
                return Instancja;
            }
        }
        private const string Cechy = @"""{kod}"",""{cecha}""";
        private const string Ceny = @"""{code}"",""{nazwa_ceny}"",{netto},{brutto},0.0000,0.0000,0.0000";
        private const string DocumentInfo = @"""1.05"",3,1250,""SOLEX B2B"",""GT"",""{seller_symbol}"",""{seller_name}"",""{seller_city}"",""{seller_zipcode}"",""{seller_street}"",""{seller_nip}"",""{magazyn_code}"",""{magazyn_name}"",""{magazyn_desc}"",,1,{created_date},{created_date},""{seller_person}"",{created_date},""{seller_country}"",""{seller_country_code}"",""{seller_EU_nip}"",{is_seller_EU_nip}";
        private const string ProductItem = @"{type},""{code}"",""{sellerCode}"",""{ean}"",""{name}"",""{desc}"",""{fiscal_name}"",""{SWW}"",""{PKWIU}"",""{jm}"",""{vat_symbol}"",{vat},""{vat_symbol}"",{vat},{CenaKartotekowa},0.0000,""{jm}"",0,""PLN"",,,0.0000,0,""{SymbolDostawcy}"",,0,""{jm}"",0.0000,0.0000,,0,""{uwagi}"",0,0,,,,,,,,";
        public string WygenerujNaglowek(Owner seller, DateTime dataWystawienia)
        {
            string naglowek = "[INFO]\r\n";
            string cd = string.Format("{0:yyyyMMddHHmmss}", dataWystawienia);
            naglowek+=
                new
                {
                    seller_symbol = seller.Symbol, //jest
                    seller_name = seller.Name, //jest
                    seller_city = seller.Address.Miasto, //jest
                    seller_zipcode = seller.Address.KodPocztowy, //jest
                    seller_street = seller.Address.UlicaNr, //jest
                    seller_nip = seller.NIP, //jest
                    magazyn_code = "",//seller.Depot.Symbol, //jest
                    magazyn_name = "",//seller.Depot.Name, //jest
                    magazyn_desc = "",//seller.Depot.Desc, //jest
                    created_date = cd,
                    seller_person = seller.User, //jest
                    seller_country = seller.Address.Kraj, //jest
                    seller_country_code = "", // seller.Adress.CountrySymbol,//jest
                    seller_EU_nip = seller.NIPEU, //jest
                    is_seller_EU_nip = seller.IsEU
                }.ToString(DocumentInfo) + "\r\n";
            return naglowek;
        }

        public string WygenrujSekcjeTowary(IEnumerable<TowarDoGeneracjiEpp> pozycje )
        {
           string document = "\r\n[NAGLOWEK]\r\n";
            document += "\"TOWARY\"\r\n";
            document += "\r\n[ZAWARTOSC]\r\n";
            foreach (var i in pozycje)
            {
                document += new
                {
                    type = "1",
                    code = i.Kod,
                    sellerCode = i.SymbolTowaruDostawcy,
                    ean = i.KodKreskowy,
                    name = i.Nazwa,  //MA Byc pusta !! bartek, zeby durny subiekt nie zalozyl kartoteki
                    desc =(i.Opis??"").UsunFormatowanieHTML(), //MA Byc pusta !! bartek, zeby durny subiekt nie zalozyl kartoteki
                    fiscal_name = "",
                    SWW = "",
                    i.PKWIU,
                    jm = i.Jednostka,
                    vat_symbol = i.StawkaVat != 0 ? i.StawkaVat.ToString(CultureInfo.InvariantCulture).Replace(",00", "").Replace(".00", ""):"0",
                    vat = i.StawkaVat != 0 ?  i.StawkaVat.ToString(CultureInfo.InvariantCulture).Replace(",", "."):"0",
                    seller_code = i.Kod,
                    uwagi=i.Uwagi,
                    i.SymbolDostawcy,
                    CenaKartotekowa = i.CenaKartotekowa?.ToString(CultureInfo.InvariantCulture).Replace(",", ".") ?? "0.000",
                }.ToString(ProductItem) + "\r\n";
            }
            return document + "\r\n";
        }

        private string WygerujSekcjeCeny(List<TowarDoGeneracjiEpp> produkty)
        {
            List<TowarDoGeneracjiCenyEpp> pozycje=new List<TowarDoGeneracjiCenyEpp>();
            foreach (var p in produkty)
            {
                if ( p.PoziomCenowyHurtowaBrutto.HasValue || p.PoziomCenowyHurtowaNetto.HasValue)
                {
                    pozycje.Add(new TowarDoGeneracjiCenyEpp(p.Kod, p.PoziomCenowyHurtowaNetto, p.PoziomCenowyHurtowaBrutto, "Hurtowa-B2B"));
                }
                if ( p.PoziomCenowyDetalicznaBrutto.HasValue || p.PoziomCenowyDetalicznaNetto.HasValue)
                {
                    pozycje.Add(new TowarDoGeneracjiCenyEpp(p.Kod, p.PoziomCenowyDetalicznaNetto, p.PoziomCenowyDetalicznaBrutto, "Detaliczna-B2B"));
                }
                if ( p.PoziomCenowyZakupBrutto.HasValue || p.PoziomCenowyZakupNetto.HasValue)
                {
                    pozycje.Add(new TowarDoGeneracjiCenyEpp(p.Kod, p.PoziomCenowyZakupNetto, p.PoziomCenowyZakupBrutto, "Zakup-B2B"));
                }
            }
            if (!pozycje.Any())
            {
                return "";
            }

            string document = "\r\n[NAGLOWEK]\r\n";
            document += "\"CENNIK\"\r\n";
            document += "\r\n[ZAWARTOSC]\r\n";
            foreach (var i in pozycje)
            {
                if (i.Netto == 0 && i.Brutto == 0)
                {
                    continue;
                }

                document += new
                {
                    code = i.Kod,
                    nazwa_ceny = i.Nazwa,
                    netto =i.Netto.ToString("F2", CultureInfo.InvariantCulture).Replace(",", "."),
                    brutto =  i.Brutto.ToString("F2", CultureInfo.InvariantCulture).Replace(",", ".")
                }.ToString(Ceny) + "\r\n";
            }
            return document + "\r\n";
        }

        private string WygenerujSekcjeCechy(List<TowarDoGeneracjiEpp> produkty)
        {
            string document = "\r\n[NAGLOWEK]\r\n";
            document += "\"CECHYTOWAROW\"\r\n";
            document += "\r\n[ZAWARTOSC]\r\n";
            foreach (var i in produkty)
            {
                if (!string.IsNullOrEmpty(i.Cecha1))
                {
                    document += ZrobCeche(i.Kod, i.Cecha1);
                }
                if (!string.IsNullOrEmpty(i.Cecha2))
                {
                    document += ZrobCeche(i.Kod, i.Cecha2);
                }
                if (!string.IsNullOrEmpty(i.Cecha3))
                {
                    document += ZrobCeche(i.Kod, i.Cecha3);
                }
                if (!string.IsNullOrEmpty(i.Cecha4))
                {
                    document += ZrobCeche(i.Kod, i.Cecha4);
                }
                if (!string.IsNullOrEmpty(i.Cecha5))
                {
                    document += ZrobCeche(i.Kod, i.Cecha5);
                }
            }
            return document + "\r\n";
        }

        private string ZrobCeche(string kod, string cecha)
        {
            return  new
            {
                kod,
                cecha,
            }.ToString(Cechy) + "\r\n";
        }

        //todo: testy sylwester powinien napisac do tego - i porzadek z heleprze EPP - wykorzystujemy 
        public string WygenerujEpp(List<EppHelper.TowarDoGeneracjiEpp> produkty)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.WygenerujNaglowek(SolexBllCalosc.PobierzInstancje.Konfiguracja.GetOwner(), DateTime.Now));
            sb.Append(this.WygenrujSekcjeTowary(produkty));
            sb.Append(this.WygenerujSekcjeCechy(produkty));
            sb.Append(this.WygerujSekcjeCeny(produkty));
            return sb.ToString();
        }

        public class TowarDoGeneracjiCenyEpp
        {
            public TowarDoGeneracjiCenyEpp() {}

            public string Kod { get; set; }
            public decimal Netto { get; set; }
            public decimal Brutto { get; set; }
            public string Nazwa { get; set; }
            public TowarDoGeneracjiCenyEpp(string kod, decimal? netto, decimal? brutto, string nazwa)
            {
                Netto = 0;
                Brutto = 0;
                Kod = kod;
                if (netto.HasValue)
                {
                    Netto = netto.Value;
                }
                if (brutto.HasValue)
                {
                    Brutto = brutto.Value;
                }
                Nazwa = nazwa;
            }
        }

        public class TowarDoGeneracjiEpp
        {
            public string SymbolTowaruDostawcy { get; set; }
            public string SymbolDostawcy { get; set; }
            public string NazwaUrzadzeniaFiskalne { get; set; }
            public string Opis { get; set; }
            public string Uwagi { get; set; }
            public string Charakterystyka { get; set; }
            public string Cecha1 { get; set; }
            public string Cecha2 { get; set; }
            public string Cecha3 { get; set; }
            public string Cecha4 { get; set; }
            public string Cecha5 { get; set; }
            public string Kod { get; set; }
            public string KodKreskowy { get; set; }
            public string PKWIU { get; set; }
            public string Nazwa { get; set; }
            public string Jednostka { get; set; }
            public decimal StawkaVat { get; set; }
            [FriendlyName("Cena w poziomie cenowym o nazwie 'Hurtowa-B2B' netto bez waluty")]
            public decimal? PoziomCenowyHurtowaNetto { get; set; }
            [FriendlyName("Cena w poziomie cenowym o nazwie 'Detaliczna-B2B' brutto bez waluty")]
            public decimal? PoziomCenowyDetalicznaBrutto { get; set; }
            [FriendlyName("Cena w poziomie cenowym o nazwie 'Detaliczna-B2B' netto bez waluty")]
            public decimal? PoziomCenowyDetalicznaNetto { get; set; }
            [FriendlyName("Cena w poziomie cenowym o nazwie 'Hurtowa-B2B' brutto bez waluty")]
            public decimal? PoziomCenowyHurtowaBrutto { get; set; }
            [FriendlyName("Cena w poziomie cenowym o nazwie 'Zakup-B2B' netto bez waluty")]
            public decimal? PoziomCenowyZakupNetto { get; set; }
            [FriendlyName("Cena w poziomie cenowym o nazwie 'Zakup-B2B' brutto bez waluty")]
            public decimal? PoziomCenowyZakupBrutto { get; set; }
            [FriendlyName("Cena kartotekowa - ostatnia cena zakupu netto")]
            public decimal? CenaKartotekowa { get; set; }
            public TowarDoGeneracjiEpp() { }

            public TowarDoGeneracjiEpp(IProduktKlienta produktKlienta)
            {
                Kod = produktKlienta.Kod;
                Nazwa = produktKlienta.Nazwa;
                KodKreskowy = produktKlienta.KodKreskowy;
                Jednostka = produktKlienta.JednostkaPodstawowa.Nazwa;
                StawkaVat = produktKlienta.Vat;
                PKWIU = produktKlienta.PKWiU;
                SymbolTowaruDostawcy = produktKlienta.Kod;
                NazwaUrzadzeniaFiskalne = produktKlienta.Nazwa;

                PoziomCenowyDetalicznaBrutto = produktKlienta.FlatCeny.CenaDetalicznaBrutto;
                PoziomCenowyDetalicznaNetto = produktKlienta.FlatCeny.CenaDetalicznaNetto;

                this.PoziomCenowyZakupBrutto = produktKlienta.FlatCeny.CenaBrutto;
                this.PoziomCenowyZakupNetto = produktKlienta.FlatCeny.CenaNetto;

                this.PoziomCenowyHurtowaBrutto = produktKlienta.FlatCeny.CenaHurtowaBrutto;
                this.PoziomCenowyHurtowaNetto = produktKlienta.FlatCeny.CenaHurtowaNetto;

                this.Cecha1 = produktKlienta.MarkaNazwa;

                Charakterystyka = produktKlienta.Opis;
            }

            public TowarDoGeneracjiEpp(string kod, string nazwa, string kodKReskowy, string jednostka, decimal stawka,
                string pkwiu, string cecha1, string cecha2, string cecha3, string cecha4, string cecha5, string uwagi, 
                string opis, string symbolTowaruDostawcy, string nazwaFiskalne, string symbolDostawcy)
            {
                Kod = kod;
                Nazwa = nazwa;
                KodKreskowy = kodKReskowy;
                Jednostka = jednostka;
                StawkaVat = stawka;
                PKWIU = pkwiu;
                Cecha1 = cecha1;
                Cecha2 = cecha2;
                Cecha3 = cecha3;
                Cecha4 = cecha4;
                Cecha5 = cecha5;
                Uwagi = uwagi;
                Opis = opis;
                SymbolTowaruDostawcy = symbolTowaruDostawcy;
                NazwaUrzadzeniaFiskalne = nazwaFiskalne;
                SymbolDostawcy = symbolDostawcy;
            }
        }


    }
    
}
