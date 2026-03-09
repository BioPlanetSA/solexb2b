using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Blokada finalizacji koszyka", FriendlyOpis = "Blokada finalizacji koszyka")]
    public class BlokadaFinalizacjiKoszyka : ZadanieCalegoKoszyka, IFinalizacjaKoszyka, IModulStartowy
    {
        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            if (!string.IsNullOrEmpty(Komunikat))
            {
                WyslijWiadomosc(Komunikat, KomunikatRodzaj.danger);
            }
            return false;
        }
    }
}