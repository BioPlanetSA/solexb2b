using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models
{
    public enum TypDrukowaniaKatalogu
    {
        Koszyk,
        Dokument,
        ListaProduktow
    }

    public class ParametrySzablonyKatalogow
    {
        public long KatalogSzablon { get; set; }
        public TypDrukowaniaKatalogu TypKatalogu { get; set; }
        //public string Sortowanie { get; set; }
        //public string SortowanieKierunek { get; set; }
        public string Opis { get; set; }
        public KatalogFormatZapisu Format { get; set; }
        public List<KatalogSzablonModelBLL> DostepneSzablony { get; set; }
    }
}