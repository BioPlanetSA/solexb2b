using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class PozycjaIlosc : RegulaKoszyka, IRegulaPozycji, IRegulaCalegoKoszyka, ITestowalna
    {
        public PozycjaIlosc()
        {
            SposobUwzgledniania = SposobUwzgledniania.Wszystkie;
        }

        public override string Opis
        {
            get { return "Warunek ilość produktów"; }
        }

        [FriendlyName("Minimalna ilość, jeśli pusta to brak ograniczeń")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int? Minimum { get; set; }

        [FriendlyName("Maksymalna ilość, jeśli pusta to brak ograniczeń")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int? Maksimum { get; set; }

        [FriendlyName("Jakie pozycje uwzględniać")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public SposobUwzgledniania SposobUwzgledniania { get; set; }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            decimal ile = IloscPozycji(pozycja);
            return TestWarunku(ile);
        }

        public bool TestWarunku(decimal ile)
        {
            bool dolnylimit = !Minimum.HasValue || ile >= Minimum.Value;
            bool gornylimic = !Maksimum.HasValue || ile <= Maksimum.Value;

            return dolnylimit && gornylimic;
        }

        private decimal IloscPozycji(IKoszykPozycja pozycja)
        {
            decimal ile = pozycja.IloscWJednostcePodstawowej;
            if (SposobUwzgledniania == SposobUwzgledniania.TylkoDostepne)
            {
                ile = ile < pozycja.Produkt.IloscLaczna ? pozycja.Produkt.IloscLaczna : ile;
            }
            return ile;
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            decimal ile = koszyk.PobierzPozycje.Sum(x => IloscPozycji(x));
            return TestWarunku(ile);
        }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            if (Minimum != null || Maksimum != null)
            {
                listaBledow = Przedzial.Sprawdzenie(Minimum, Maksimum, "Minimalna ilość", "Maksymalna ilość");
            }
            return listaBledow;
        }
    }
}