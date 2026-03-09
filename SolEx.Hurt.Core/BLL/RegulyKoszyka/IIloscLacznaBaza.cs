using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public abstract class IloscLacznaBaza : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji, ITestowalna
    {
        public ICechyAtrybuty CechyAtrybuty = SolexBllCalosc.PobierzInstancje.CechyAtrybuty;
        public IConfigBLL Config = SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public enum IleProduktow
        {
            NieWszystkie = 1,
            Wszystkie = 10
        }

        public IloscLacznaBaza()
        {
            Ilosc = IleProduktow.NieWszystkie;
        }

        [FriendlyName("Minimalna ilość, jeśli pusta to brak ograniczeń")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? Minimum { get; set; }

        [FriendlyName("Maksymalna ilość, jeśli pusta to brak ograniczeń")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? Maksimum { get; set; }

        [FriendlyName("Określamy czy w koszyku muszą znaleść się wszystkie/nie wszystkie produkty z określoną cechą/atrybutem aby warunek został spełniony")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public IleProduktow Ilosc { get; set; }

        public abstract List<string> TestPoprawnosci();

        public abstract decimal Wylicz(IKoszykiBLL koszyk, out int ilosc);

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            int iloscZ = 0;
            decimal liczba = Wylicz(koszyk, out iloscZ);
            bool dolnylimit = !Minimum.HasValue || liczba >= Minimum.Value;
            bool gornylimic = !Maksimum.HasValue || liczba < Maksimum.Value;
            bool wszystkie = Ilosc == IleProduktow.NieWszystkie || iloscZ == koszyk.PobierzPozycje.Count;
            return wszystkie && dolnylimit && gornylimic;
            //return Ilosc == IleProduktowZCecha.Wszystkie ? iloscZCecha == koszyk.Pozycje.Count && liczba>Minimum && liczba<Maksimum : (Minimum == 0 || Minimum <= liczba) && (Maksimum == 0 || Maksimum >= liczba);
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
             return KoszykSpelniaRegule(koszyk);
        }
    }
}