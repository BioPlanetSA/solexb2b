using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class DodajDoUwag : ZadanieCalegoKoszyka, IFinalizacjaKoszyka
    {
        public override string PokazywanaNazwa
        {
            get { return "Dodawanie treści do uwag"; }
        }

        [FriendlyName("Treść do dodania")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string TrescDoDodania { get; set; }

        [FriendlyName("Gdzie dodać treść do uwag")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public GdzieDodac GdzieDodac { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            if (GdzieDodac == GdzieDodac.Koniec)
            {
                koszyk.Uwagi = koszyk.Uwagi + " " + TrescDoDodania;
            }
            else
            {
                koszyk.Uwagi = TrescDoDodania + " " + koszyk.Uwagi;
            }
            return true;
        }
    }

    public enum GdzieDodac
    {
        Poczatek, Koniec
    }
}