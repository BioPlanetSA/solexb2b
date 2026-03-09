using System.Web;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyBloga
{
    public class PokazWpisZPolaBloga : WpisBlogBaza
    {
        public override string Kontroler
        {
            get { return "Blog"; }
        }

        public override string Akcja
        {
            get { return "WpisBloga"; }
        }

        public override string Grupa
        {
            get { return "Blog"; }
        }

        public override string Nazwa
        {
            get { return "Pojedynczy wpis bloga"; }
        }
        [PobieranieSlownika(typeof(SlownikPolBlogWpis))]

        [FriendlyName("Pole")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleDropDown)]
        public string Pole { get; set; }

       
    }
}