using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametrIndywidualizacji
    {
        public ParametrIndywidualizacji(ProduktKlienta pr, decimal ilosc, long jednostkaID, TypPozycjiKoszyka typPozycji)
        {
            this.TypPozycji = typPozycji;
            this.Produkt = pr;
            this.Ilosc = ilosc;
            this.JednostkaId = jednostkaID;
        }

        public decimal Ilosc { get; set; }
        public IProduktKlienta Produkt { get; set; }

        //public object[] Indywidualizacja { get; set; }

        public TypPozycjiKoszyka TypPozycji { get; set; }

        public long? JednostkaId { get; set; }
    }
}