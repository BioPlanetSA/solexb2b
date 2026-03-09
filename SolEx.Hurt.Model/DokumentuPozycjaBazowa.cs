using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Model
{
    public interface IDokumentuPozycjaBazowa
    {
        int Id { get; set; }
        long ProduktId { get; set; }
        decimal Ilosc { get; set; }
        string Opis { get; set; }
        string Opis2 { get; set; }
        decimal CenaNetto { get; set; }
        decimal CenaBrutto { get; set; }
        int DokumentId { get; set; }
        WartoscLiczbowa PozycjaDokumentuWartoscNetto { get; }
        WartoscLiczbowa PozycjaDokumentuWartoscBrutto { get; }
        WartoscLiczbowa PozycjaDokumentuCenaNetto { get; }
        WartoscLiczbowa PozycjaDokumentuCenaBrutto { get; }

        /// <summary>
        /// AUTOuzupelnienie przy pobieraniu DAL
        /// </summary>
        string Jednostka { get; }

        /// <summary>
        /// metoda tylko po to zeby nie zaposywać bezposrednio do jednostki - bo jest polem ignorowanym
        /// </summary>
        /// <param name="nazwaJednostki"></param>
        void UstawJednostke(string nazwaJednostki);

        long JednostkaMiary { get; set; }
        WartoscLiczbowaZaokraglana PozycjaDokumentuIlosc { get; }
        string walutaB2b { get; set; }
    }

    public class DokumentuPozycjaBazowa: IHasIntId, IDokumentuPozycjaBazowa
    {
        public DokumentuPozycjaBazowa() { }

        public DokumentuPozycjaBazowa(DokumentuPozycjaBazowa baza)
        {
            this.Id = baza.Id;
            this.ProduktId = baza.ProduktId;
            this.Ilosc = baza.Ilosc;
            this.Opis2 = baza.Opis2;
            this.Opis = baza.Opis;
            this.CenaNetto = baza.CenaNetto;
            this.CenaBrutto = baza.CenaBrutto;
            this.DokumentId = baza.DokumentId;
            this.walutaB2b = baza.walutaB2b;
            this.Jednostka = baza.Jednostka;
            this.JednostkaMiary = baza.JednostkaMiary;
            this.ProduktIdBazowy = baza.ProduktIdBazowy;
        }

        [UpdateColumnKey]
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [FriendlyName("Numer id produkt")]
        public long ProduktId { get; set; }

        [FriendlyName("Ilość")]
        public virtual decimal Ilosc { get; set; }

        [FriendlyName("Opis pozycji")]
        public virtual string Opis { get; set; }

        [FriendlyName("Opis pozycji 2")]
        public string Opis2 { get; set; }

        /// <summary>
        /// tutaj jest bałagan - cena dla pozycji zamówienai to cena PO rabacie, ale w pozycji historai dokumentów - cena PRZED rabatem
        /// </summary>
        [FriendlyName("Cena netto")]
        public decimal CenaNetto { get; set; }

        /// <summary>
        /// tutaj jest bałagan - cena dla pozycji zamówienai to cena PO rabacie, ale w pozycji historai dokumentów - cena PRZED rabatem
        /// </summary>
        [FriendlyName("Cena brutto")]
        public decimal CenaBrutto { get; set; }


        [FriendlyName("Id dokumentu")]
        public int DokumentId { get; set; }

        [Ignore]
        public string walutaB2b { get; set; }

        /// <summary>
        /// tutaj jest bałagan - cena netto dla pozycji zamówienai to cena PO rabacie, ale w pozycji historai dokumentów - cena PRZED rabatem
        /// </summary>
        [FriendlyName("Wartość netto")]
        [Ignore]
        public virtual WartoscLiczbowa PozycjaDokumentuWartoscNetto => new WartoscLiczbowa(Ilosc * CenaNetto, walutaB2b);

        /// <summary>
        /// tutaj jest bałagan - cena netto dla pozycji zamówienai to cena PO rabacie, ale w pozycji historai dokumentów - cena PRZED rabatem
        /// </summary>
        [FriendlyName("Wartość brutto")]
        [Ignore]
        public virtual WartoscLiczbowa PozycjaDokumentuWartoscBrutto => new WartoscLiczbowa(Ilosc * CenaBrutto, walutaB2b);

        /// <summary>
        /// tutaj jest bałagan - cena netto dla pozycji zamówienai to cena PO rabacie, ale w pozycji historai dokumentów - cena PRZED rabatem
        /// </summary>
        [Ignore]
        public virtual WartoscLiczbowa PozycjaDokumentuCenaNetto => new WartoscLiczbowa(CenaNetto, walutaB2b);

        /// <summary>
        /// tutaj jest bałagan - cena netto dla pozycji zamówienai to cena PO rabacie, ale w pozycji historai dokumentów - cena PRZED rabatem
        /// </summary>
        [Ignore]
        public virtual WartoscLiczbowa PozycjaDokumentuCenaBrutto => new WartoscLiczbowa(CenaBrutto, walutaB2b);

        public void UstawJednostke(string jednostkaNazwa)
        {
            this.Jednostka = jednostkaNazwa;
        }

        /// <summary>
        /// AUTOuzupelnienie przy pobieraniu DAL
        /// </summary>
        [Ignore]
        [FriendlyName("Jednostka")]
        public virtual string Jednostka { get; private set; }


        [FriendlyName("Jednostka miary")]
        public long JednostkaMiary { get; set; }

        [FriendlyName("Id produktu Bazowego")]
        public long ProduktIdBazowy { get; set; }


        [Ignore]
        public WartoscLiczbowaZaokraglana PozycjaDokumentuIlosc => new WartoscLiczbowaZaokraglana(Ilosc, Jednostka);
    }
}
