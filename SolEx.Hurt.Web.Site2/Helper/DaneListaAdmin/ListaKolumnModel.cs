using SolEx.Hurt.Helpers;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin
{
    public class ListaKolumnModel
    {
        public IList<OpisPolaObiektuBaza> WszystkieKolumny { get; set; }
        public string[] WidoczneKolumny { get; set; }
        public long? AktywnySzablon { get; set; }
        public Type Typ { get; set; }

        public IList<Model.UkladKolumn> IstniejaceSzablony { get; set; }

        public string Akcja { get; set; }
    }
}