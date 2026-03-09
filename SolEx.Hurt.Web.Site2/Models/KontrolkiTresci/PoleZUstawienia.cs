using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class PoleZUstawienia : KontrolkaTresciBaza
    {
        public PoleZUstawienia()
        {
            DomyslneWartosciDlaNowejKontrolki = new TrescKolumna();
            DomyslneWartosciDlaNowejKontrolki.Paddingi = "10px 0 10px 0";
        }

        public override string Nazwa
        {
            get { return "Pole z ustawienia"; }
        }

        public override string Kontroler
        {
            get { return "Tresci"; }
        }

        public override string Akcja
        {
            get { return "PoleZUstawienia"; }
        }

        public override string Opis
        {
            get { return "Kontrolka wyświetla wybraną wartość ustawienia"; }
        }

        [FriendlyName("Wartość do pokazania")]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikUstawienieWartosc))]
        [Niewymagane]
        public string WartoscDoPokazania { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Przedrostek { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Przyrostek { get; set; }
    }
}