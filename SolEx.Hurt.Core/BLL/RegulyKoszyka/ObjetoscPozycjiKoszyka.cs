using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class ObjetoscPozycjiKoszyka : RegulaKoszyka, IRegulaPozycji
    {
        public override string Opis
        {
            get { return "Czy objętość danej pozycji jest odpowiednia"; }
        }

        [FriendlyName("Objętość minimalna")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? ObjetoscMinimalna { get; set; }

        [FriendlyName("Objętość maksymalna")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? ObjetoscMaksymalna { get; set; }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            decimal objetosc = 0m;
            var o = pozycja.Produkt.Objetosc;
            if (o.HasValue)
                objetosc = pozycja.IloscWJednostcePodstawowej*o.Value;
            bool dolnylimit = !ObjetoscMinimalna.HasValue || objetosc >= ObjetoscMinimalna.Value;
            bool gornylimic = !ObjetoscMaksymalna.HasValue || objetosc < ObjetoscMaksymalna.Value;

            return dolnylimit && gornylimic;
        }
    }
}