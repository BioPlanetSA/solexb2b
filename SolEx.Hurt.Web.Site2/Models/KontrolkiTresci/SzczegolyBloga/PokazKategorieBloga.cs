using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyBloga
{
    public class PokazKategorieBloga : WpisBlogBaza
    {
        public PokazKategorieBloga()
        {
            PokazywacNazweGrupy = false;
        }
        public override string Kontroler
        {
            get { return "Blog"; }
        }

        public override string Akcja
        {
            get { return "PokazKategorieBloga"; }
        }

        public override string Grupa
        {
            get { return "Blog"; }
        }
        public override string Nazwa
        {
            get { return "Pokaz kategorie bloga"; }
        }

        [FriendlyName("Grupy kategorii")]
        [WidoczneListaAdmin(true, false, true, false)]
        [PobieranieSlownika(typeof(SlownikGrupyBlogow))]
        [Niewymagane]
        public int[] Grupy { get; set; }

        [FriendlyName("Pokazywać nazwe grupy? ")]
        [WidoczneListaAdmin(true, false, true, false)]
        public bool PokazywacNazweGrupy { get; set; }

     
    }
}