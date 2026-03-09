using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Model.Interfaces.Modele;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoPracownika
    {
        public ParametryDoPracownika(IKlienci klient, string dodatkowyTelefon, string tekstNadZdjeciem = null)
        {
            Klient = klient;
            DodatkowyTelefon = dodatkowyTelefon;
            TekstNadZdjeciem = tekstNadZdjeciem;
        }

        public ParametryDoPracownika(IKlienci klient, string dodatkowyTelefon,List<string>dodatkowePola, string rozmiar, string tekstNadZdjeciem = null)
        {
            Klient = klient;
            DodatkowyTelefon = dodatkowyTelefon;
            TekstNadZdjeciem = tekstNadZdjeciem;
            DodatkowePola = dodatkowePola;
            Preset = rozmiar;
        }
        public IKlienci Klient { get; set; }
        public string TekstNadZdjeciem { get; set; }
        public string DodatkowyTelefon { get; set; }
        public List<string> DodatkowePola { get; set; }
        public string Preset { get; set; }

    }
}