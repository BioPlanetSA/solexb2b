using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core
{
    [Alias("HistoriaDokumentuProdukt")]
    public class DokumentyPozycje : Model.HistoriaDokumentuProdukt, IPozycjaDokumentuBll
    {   
        public DokumentyPozycje() { }
        public DokumentyPozycje(HistoriaDokumentuProdukt baza): base(baza) { }

        public DokumentyPozycje(ZamowieniaProduktyBLL baza)
        {
            //pola z obiektu HistoriaDokumentuProdukt
            this.ProduktBazowy = baza.ProduktBazowy;
            this.KodProduktu = baza.ProduktBazowy.Kod;
            NazwaProduktu = baza.ProduktBazowy.Nazwa;
            CenaNettoPoRabacie = baza.CenaNetto;
            WartoscNetto = baza.PozycjaDokumentuWartoscNetto;
            WartoscBrutto = baza.PozycjaDokumentuWartoscBrutto;
            WartoscVat = baza.PozycjaDokumentuWartoscBrutto - baza.PozycjaDokumentuWartoscNetto;
            CenaBruttoPoRabacie = baza.CenaBrutto;
            
            //przechwyt beznadzieny bo nie chcemy ciagnac obiektu produktu klienta - mamy tylko bazowy i nie wiemy jaki jest VAT obecnie dla klienta
            if (baza.CenaBrutto == baza.CenaNetto)
            {
                Vat = 0;
            }
            else
            {
                Vat = baza.ProduktBazowy.Vat;
            }

            Rabat = 0;
            TypProduktu = TypPozycjiZamowienia.DodanyPrzezKlienta;

            //dane z obiektu DokumentuPozycjaBazowa

            this.Id = baza.Id;
            this.ProduktId = baza.ProduktId;
            this.ProduktIdBazowy = baza.ProduktIdBazowy;
            this.Ilosc = baza.Ilosc;
            this.Opis2 = baza.Opis2;
            this.Opis = baza.Opis;
            this.DokumentId = baza.DokumentId;
            this.walutaB2b = baza.walutaB2b;
            this.UstawJednostke(baza.Jednostka);
            this.JednostkaMiary = baza.JednostkaMiary;
        }
        
        /// <summary>
        /// ustawiane przy wyciaganiu automatycznie
        /// </summary>
        [Ignore]
        public ProduktBazowy ProduktBazowy { get; set; }
        
        [Ignore]
        public WartoscLiczbowa PozycjaDokumentuCenaNetto
        {
             get
             {
                 return new WartoscLiczbowa(CenaNettoPoRabacie, walutaB2b);
             }
        }
         [Ignore]
        public WartoscLiczbowa PozycjaDokumentuCenaBrutto
        {
            get
            {
                return new WartoscLiczbowa(CenaBruttoPoRabacie, walutaB2b);
            }
        }
      
         [Ignore]
        public WartoscLiczbowa PozycjaDokumentuWartoscNetto
        {
            get
            {
                return new WartoscLiczbowa(WartoscNetto, walutaB2b);
            }
        }
         [Ignore]
        public WartoscLiczbowa PozycjaDokumentuWartoscBrutto
        {
            get
            {
                return new WartoscLiczbowa(WartoscBrutto, walutaB2b);
            }
        }

         [Ignore]
        public WartoscLiczbowa PozycjaDokumentuWartoscVat
        {
            get {return PozycjaDokumentuWartoscBrutto - PozycjaDokumentuWartoscNetto; }
        }

        [Ignore]
        public WartoscLiczbowaZaokraglana PozycjaDokumentuRabat
        {
            get { return new WartoscLiczbowaZaokraglana(Rabat);}
        }

        public string ToCsv()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", NazwaProduktu.Trim(), ProduktBazowy!=null?ProduktBazowy.KodKreskowy:"",Vat.ToString("0.##").Replace(",", "."), Jednostka, ProduktBazowy!=null?ProduktBazowy.PKWiU:"", 
                PozycjaDokumentuIlosc.Wartosc.ToString("0.##").Replace(",", "."),PozycjaDokumentuCenaBrutto.Wartosc.ToString("0.##").Replace(",", "."),PozycjaDokumentuRabat.Wartosc.ToString("0.##").Replace(",","."),
                PozycjaDokumentuWartoscBrutto.Wartosc.ToString("0.##").Replace(",", "."),ProduktId);
        }

        //[Ignore]
        //public decimal JednostkaPrzelicznik 
        //{
        //    get
        //    {
        //        var prod = PobierzProdukt( SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);
        //        if (prod == null)
        //        {
        //            return 1.0m;
        //        }
        //        var jednostka = prod.Jednostki.FirstOrDefault(x => x.Id == JednostkaMiary);
        //        return jednostka == null ? 1.0m : jednostka.Przelicznik;
        //    }
        //}
    }
}
