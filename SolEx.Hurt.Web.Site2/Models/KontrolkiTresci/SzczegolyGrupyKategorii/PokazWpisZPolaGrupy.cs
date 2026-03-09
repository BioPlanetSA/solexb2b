using SolEx.Hurt.Core;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyGrupyKategorii
{
    public class PokazWpisZPolaGrupy : WpisGrupaBaza
    {
        public override string Kontroler
        {
            get { return "SzczegolyGrupaKategorii"; }
        }

        public override string Akcja
        {
            get { return "PoleGrupy"; }
        }

        public override string Grupa
        {
            get { return "Grupa kategorii"; }
        }

        public override string Nazwa
        {
            get { return "Pola grupy kategorii"; }
        }
        [SyncSlownikNaPodstawieInnegoTypu(typeof(GrupaBLL))]

        [FriendlyName("Pole")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleDropDown)]
        public string Pole { get; set; }

       
    }

    public abstract class WpisGrupaBaza : PokazWpisBaza
    {

        public override string SymbolIdentyfikatora
        {
            get { return "gpid"; }
        }

        public override string ModelObiektu
        {
            get { return "Grupa kategorii"; }
        }

        //public object IdentyfikatorObiektu
        //{
        //    get
        //    {
        //        return PobierzIdentyfikator("gpid", false);
        //    }
        //}
    }
}