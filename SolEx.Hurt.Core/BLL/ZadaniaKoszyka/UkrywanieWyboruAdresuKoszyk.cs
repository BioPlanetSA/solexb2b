namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class UkrywanieWyboruAdresuKoszyk : ZadanieCalegoKoszyka, IModulStartowy
    {
        public override bool Wykonaj(ModelBLL.Interfejsy.IKoszykiBLL koszyk)
        {
            koszyk.UkryjAdresy = true;
            return true;
        }
    }
}