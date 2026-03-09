using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin
{
    public class DaneEdycjaAdmin
    {
        public DaneEdycjaAdmin()
        {
            AkcjaZapisz = "ZapiszObiekt";
        }

        public DaneEdycjaAdmin(DaneObiekt daneObiektu)
        {
            AkcjaZapisz = "ZapiszObiekt";
            this.DaneObiektu = daneObiektu;
        }

        public DaneEdycjaAdmin(DaneEdycjaAdmin baza) : this(baza.DaneObiektu)
        {
            Typ = baza.Typ;
            KluczWartosc = baza.KluczWartosc;
            PolaObiektu = (OpisPolaObiektu[])baza.PolaObiektu.Clone();
            Nadrzedny = baza.Nadrzedny;
            JezykId = baza.JezykId;
            Jezyki = baza.Jezyki != null ? new List<Jezyk>(baza.Jezyki) : null;
        }

        public string AkcjaZapisz { get; set; }
        public Type Typ { get; set; }
        public string KluczWartosc { get; set; }
        public OpisPolaObiektu[] PolaObiektu { get; set; }

        public string Nadrzedny { get; set; }

        public Type TypNadrzednych { get; set; }
        public int JezykId { get; set; }
        public IList<Jezyk> Jezyki { get; set; }

        public DaneObiekt DaneObiektu { get; private set; }
    }
}