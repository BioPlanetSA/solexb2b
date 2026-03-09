using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class KolorowaniePozycji : ZadaniePozycjiKoszyka, IModulStartowy, IFinalizacjaKoszyka
    {
        [FriendlyName("Kolor tla - należy wpisać w formie hex czyli np #ff0000. Przydkładowy edytor: http://www.colorpicker.com/")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string KolorTla { get; set; }

        public override bool Wykonaj(ModelBLL.Interfejsy.IKoszykPozycja pozycja, ModelBLL.Interfejsy.IKoszykiBLL koszyk)
        {
            pozycja.KolorTla = KolorTla;
            return true;
        }
    }
}