using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class ProduktWIlosciZbiorczej : RegulaKoszyka, IRegulaPozycji
    {
        public override string Opis
        {
            get { return "Czy ilość w koszyku jest wielokrotnością opakowania zbiorczego"; }
        }

        [FriendlyName("Czy uzględniać produkty z opakowaniem zbiorczym równym 1")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool UwagledniacJednosci { get; set; }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            if (pozycja.Produkt.IloscWOpakowaniu == 1 && !UwagledniacJednosci)
            {
                return false;
            }
            return pozycja.IloscWJednostcePodstawowej % pozycja.Produkt.IloscWOpakowaniu == 0;
        }
    }
}