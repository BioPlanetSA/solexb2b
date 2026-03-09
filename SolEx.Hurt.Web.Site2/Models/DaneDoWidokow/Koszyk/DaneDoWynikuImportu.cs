using System;
using System.Collections.Generic;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Koszyk
{
    public class DaneDoWynikuImportu
    {

        public DaneDoWynikuImportu(List<Model.Web.Komunikat> komunikat, string zrodlo, DateTime czas)
        {
            Komunikaty =komunikat;
            Zrodlo = zrodlo;
            Czas = czas;
        }
        public List<Model.Web.Komunikat> Komunikaty { get; set; }
        public string Zrodlo { get; set; }
        public DateTime Czas{ get; set; }
}
    
}
