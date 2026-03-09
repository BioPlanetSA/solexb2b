using System;
using System.Collections.Generic;
using System.Text;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    [FriendlyName("Format CSV jest dedykowany do budowy własnych rozwiązań informatycznych. Plik można otworzyć np. w Excelu, OpenOffice lub notatniku. Zawartość pliku to po prostu pozycje dokumentu gdzie identyfikatorem produktu będzie kod kreskowy")]
    public class Csv : GenerowanieDokumentu
    {
        public override bool MoznaGenerowac(DokumentyBll dokument)
        {
            return true;
        }

        public override Licencje? WymaganaLicencja => Licencje.DokumentyCSV;

        public override Encoding Kodowanie => Encoding.UTF8;

        public override string Nazwa => "Csv - utf8";

        public virtual string IdentyfikatorProduktu => "EAN";

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + "-b2b.csv";
        }

        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {
            List<object> obiektyDoCSV = new List<object>();
            
            foreach (DokumentyPozycje poz in dokument.PobierzPozycjeDokumentu())
            {
                string identyfikator = poz.ProduktBazowy != null ? poz.ProduktBazowy.KodKreskowy : "";
                if (IdentyfikatorProduktu.Equals("Kod"))
                {
                    identyfikator = poz.ProduktBazowy != null ? poz.ProduktBazowy.Kod : poz.KodProduktu;
                }
                object obiekt = new
                {
                    NazwaDokumentu = dokument.NazwaDokumentu,
                    ProduktID = poz.ProduktId,
                    Kod = identyfikator,
                    ProduktNazwa = poz.NazwaProduktu,
                    Ilosc = Math.Round(poz.Ilosc,base.Zaokraglenia[poz.JednostkaMiary]),
                    Jednostka = poz.Jednostka,
                    Cena = poz.CenaNettoPoRabacie.ToString("F2"),
                    Wartosc = poz.WartoscNetto.ToString("F2"),
                    VAT = poz.Vat.ToString("F2"),
                    CenaBrutto = poz.CenaBruttoPoRabacie.ToString("F2"),
                    WartoscBrutto = poz.WartoscBrutto.ToString("F2")
                };
                obiektyDoCSV.Add(obiekt);
            }
            return Kodowanie.GetBytes(new CSVHelperExt().WygenerujCsvDlaListyObiektow(obiektyDoCSV,true).ToString());
        }
    }
}
