using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model
{

    [FriendlyClassName("Pozycja na dokumencie")]
    [ApiTypeDescriptor(ApiGrupy.Dokumenty, "Model pozycji dokumentu")]
    public class HistoriaDokumentuProdukt : DokumentuPozycjaBazowa
    {
        public HistoriaDokumentuProdukt(int id, string nazwaProduktu, string kodProduktu)
        {
            this.Id = id;
            this.NazwaProduktu = nazwaProduktu;
            this.KodProduktu = kodProduktu;
        }

        public HistoriaDokumentuProdukt(){}

        public HistoriaDokumentuProdukt(int dokument, long produkt, decimal ilosc, long jednostka,decimal porabacienetto, decimal porabaciebrutto, decimal netto, decimal brutto, string nazwa, string kod,
            decimal wartoscnetto, decimal wartoscbrutto,decimal wartoscvat, decimal stawakvat, TypPozycjiZamowienia typ)
        {
            DokumentId = dokument;
            ProduktId = produkt;
            this.Ilosc = ilosc;
            JednostkaMiary = jednostka;
            CenaNettoPoRabacie = porabacienetto;
            CenaBruttoPoRabacie = porabaciebrutto;
            CenaNetto = netto;
            CenaBrutto = brutto;
            NazwaProduktu = nazwa;
            ProduktIdBazowy = produkt;
            KodProduktu = kod;
            WartoscNetto = wartoscnetto;
            WartoscBrutto = wartoscbrutto;
            WartoscVat = wartoscvat;
            Vat = stawakvat;
            TypProduktu = typ;
        }

        public HistoriaDokumentuProdukt(HistoriaDokumentuProdukt baza):base(baza)
        {
            KodProduktu = baza.KodProduktu;
            NazwaProduktu = baza.NazwaProduktu;
            CenaNettoPoRabacie = baza.CenaNettoPoRabacie;
            CenaBruttoPoRabacie = baza.CenaBruttoPoRabacie;
            WartoscNetto = baza.WartoscNetto;
            WartoscBrutto = baza.WartoscBrutto;
            WartoscVat = baza.WartoscVat;
            Vat = baza.Vat;
            Rabat = baza.Rabat;
            TypProduktu = baza.TypProduktu;
        }


        [FriendlyName("Kod produktu")]
        public string KodProduktu { get; set; }

        [FriendlyName("Nazwa produktu")]
        public string NazwaProduktu { get; set; }
        
        [FriendlyName("Wartość zakupu brutto")]
        public decimal WartoscBrutto { get; set; }

        [FriendlyName("Wartość zakupu netto")]
        public decimal WartoscNetto { get; set; }

        /// <summary>
        /// VAT - pełna liczba np. 23
        /// </summary>
        public decimal Vat { get; set; }

        public decimal WartoscVat { get; set; }

        public decimal Rabat { get; set; }

        [FriendlyName("Cena brutto po rabacie")]
        public decimal CenaBruttoPoRabacie { get; set; }

        public decimal CenaNettoPoRabacie { get; set; }
        
        [Ignore]
        public TypPozycjiZamowienia TypProduktu { get; set; }

    }
}
