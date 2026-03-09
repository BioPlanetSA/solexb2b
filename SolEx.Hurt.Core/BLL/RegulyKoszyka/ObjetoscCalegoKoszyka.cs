using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class ObjetoscCalegoKoszyka : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji, ITestowalna
    {
        public override string Opis
        {
            get { return "Czy objętość danej pozycji jest odpowiednia"; }
        }

        [FriendlyName("Objętość minimalna - Jeśli pole niewypełnione to brak ograniczeń dla minimalnej objętości koszyka")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? ObjetoscMinimalna { get; set; }

        [FriendlyName("Objętość maksymalna - Jeśli pole niewypełnione to brak ograniczeń dla maksymalnej objętości koszyka")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? ObjetoscMaksymalna { get; set; }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return KoszykSpelniaRegule(koszyk);
        }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = Przedzial.Sprawdzenie(ObjetoscMinimalna, ObjetoscMaksymalna, "Objętość minimalna koszyka", "Objętość maksymalna koszyka"); ;
            return listaBledow;
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            decimal objetosc = 0;
            foreach (var poz in koszyk.PobierzPozycje)
            {
                var o = poz.Produkt.Objetosc;
                if (o != null)
                    objetosc += poz.IloscWJednostcePodstawowej * o.Value;
            }
            bool dolnylimit = !ObjetoscMinimalna.HasValue || objetosc >= ObjetoscMinimalna.Value;
            bool gornylimic = !ObjetoscMaksymalna.HasValue || objetosc < ObjetoscMaksymalna.Value;

            return dolnylimit && gornylimic;
        }
    }
}