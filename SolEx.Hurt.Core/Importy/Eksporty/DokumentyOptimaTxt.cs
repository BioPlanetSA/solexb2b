using System;
using System.Collections.Generic;
using System.Text;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    public class DokumentyOptimaTxt : GenerowanieDokumentu
    {
        public override bool MoznaGenerowac(DokumentyBll dokument)
        {
            return true;

        }

        public override Licencje? WymaganaLicencja
        {
            get { return Licencje.DokumentyOptimaTXT; }
        }

        public override Encoding Kodowanie
        {
            get { return Encoding.GetEncoding(28592); }
        }

        public override string Nazwa
        {
            get { return "Optima2016"; }
        }

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + "-Optima2016.txt";
        }

        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {
            List<object> obiektyDoCSV = new List<object>();
            foreach (DokumentyPozycje poz in dokument.PobierzPozycjeDokumentu())
            {
                object obiekt = new
                {
                    EAN = poz.ProduktBazowy!=null?poz.ProduktBazowy.KodKreskowy:"",
                    Ilosc = poz.Ilosc.DoLadnejCyfry("0.###"),
                    Cena = poz.CenaNettoPoRabacie.DoLadnejCyfry("0.##")
                };
                obiektyDoCSV.Add(obiekt);
            }
            return Kodowanie.GetBytes(new CSVHelperExt().WygenerujCsvDlaListyObiektow(obiektyDoCSV,false).ToString());
        }
    }
}
