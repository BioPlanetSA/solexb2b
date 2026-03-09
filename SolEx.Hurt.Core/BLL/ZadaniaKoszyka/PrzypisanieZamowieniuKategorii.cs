using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class PrzypisanieZamowieniuKategorii : ZadanieCalegoKoszyka, IZadaniePoFinalizacji
    {
        public override string PokazywanaNazwa => "Przypisanie zamówienie podczas importu do odpowiedniej kategorii w erp";

        [FriendlyName("Nazwa kategorii. Musi istnieć w erp.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaKategorii { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            koszyk.KategoriaZamowienia = NazwaKategorii;
            return true;
        }
    }
}