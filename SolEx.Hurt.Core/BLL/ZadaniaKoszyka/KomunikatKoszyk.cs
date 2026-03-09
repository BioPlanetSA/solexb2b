using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class KomunikatKoszyk : ZadanieCalegoKoszyka, IModulStartowy, IModulKoszykPusty
    {
        public override string Opis
        {
            get { return "Wyświetla komunikat na górze koszyka"; }
        }

        [FriendlyName("Rodzaj komunikatu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KomunikatRodzaj Rodzaj { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            WyslijWiadomosc(string.Format(Komunikat, koszyk.WagaCalokowita()), Rodzaj);
            return true;
        }
    }
}