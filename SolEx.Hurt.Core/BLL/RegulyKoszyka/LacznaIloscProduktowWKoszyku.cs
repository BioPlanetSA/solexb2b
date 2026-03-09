using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class LacznaIloscProduktowWKoszyku : RegulaKoszyka, IRegulaCalegoKoszyka, ITestowalna
    {
        public LacznaIloscProduktowWKoszyku()
        {
            SposobUwzgledniania = SposobUwzgledniania.Wszystkie;
        }

        public override string Opis
        {
            get { return "Ilość pozycji w koszyku musi znajdować się przedziale"; }
        }

        [FriendlyName("Jakie pozycje uwzględniać")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public SposobUwzgledniania SposobUwzgledniania { get; set; }

        [FriendlyName("Ilość minimalna - Jeśli pole niewypełnione to brak ograniczeń co do ilości minimalnej")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? IloscMinimalna { get; set; }

        [Niewymagane]
        [FriendlyName("Ilość maksymalna- Jeśli pole niewypełnione to brak ograniczeń co do ilości maksymalnej")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? IloscMaksymalna { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            listaBledow = Przedzial.Sprawdzenie(IloscMinimalna, IloscMaksymalna);
            return listaBledow;
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            decimal wartosc = WyliczWartosc(koszyk);

            bool dolnylimit = !IloscMinimalna.HasValue || wartosc >= IloscMinimalna.Value;
            bool gornylimic = !IloscMaksymalna.HasValue || wartosc < IloscMaksymalna.Value;

            return dolnylimit && gornylimic;
        }

        public decimal WyliczWartosc(IKoszykiBLL koszyk)
        {
            decimal wartosc = 0;
            switch (SposobUwzgledniania)
            {
                case SposobUwzgledniania.Wszystkie:
                    wartosc = CalkowitaIloscPozycjiKoszyka(koszyk);
                    break;

                case SposobUwzgledniania.TylkoDostepne:
                    wartosc = WyliczIloscPozycjiKoszykTylkoDostepnychPozycji(koszyk);
                    break;

                case SposobUwzgledniania.TylkoNiedostepne:
                    wartosc = CalkowitaIloscPozycjiKoszyka(koszyk) - WyliczIloscPozycjiKoszykTylkoDostepnychPozycji(koszyk); //niedostepne to calkowita wartosc - wartosc dostepnych
                    break;

                case SposobUwzgledniania.ProduktyZeStanemWiekszymOdZera:
                    wartosc = WyliczIloscPozycjiKoszykPozycjeStanWiekszyOdZera(koszyk);
                    break;
            }
            return wartosc;
        }

        private decimal WyliczIloscPozycjiKoszykPozycjeStanWiekszyOdZera(IKoszykiBLL koszyk)
        {
            return koszyk.PobierzPozycje.Where(pozycja => pozycja.Produkt.IloscLaczna > 0).Sum(pozycja => pozycja.IloscWJednostcePodstawowej);
        }

        private decimal CalkowitaIloscPozycjiKoszyka(IKoszykiBLL koszyk)
        {
            return koszyk.PobierzPozycje.Sum(x => x.IloscWJednostcePodstawowej);
        }

        private decimal WyliczIloscPozycjiKoszykTylkoDostepnychPozycji(IKoszykiBLL koszyk)
        {
            return koszyk.PobierzPozycje.Sum(pozycja => pozycja.IloscWJednostcePodstawowej > pozycja.Produkt.IloscLaczna ? pozycja.Produkt.IloscLaczna : pozycja.IloscWJednostcePodstawowej);
        }
    }
}