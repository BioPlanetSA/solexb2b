using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Warunek kredytu kupieckiego", FriendlyOpis= "Warunek porównujący wartość netto koszyka z wartością pozostałego kredytu kupieckiego (limit kredytu * współczynnik - wartość wykorzystana). Jeśli klient nie ma kredytu kupieckiego, to moduł jest niespełniony")]
    public class KredytKupiecki : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji
    {
        public KredytKupiecki()
        {
            Mnoznik = (decimal)1.1;
        }
        
        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            if (koszyk.Klient.LimitKredytu == 0)
            {
                return false;
            }
            //limit kredytu pomnozony w  wposlczynnik
            decimal maxymalnaWartoscKredytu = koszyk.Klient.LimitKredytu * Mnoznik;

            //kwota które juz wykorzystał
            decimal wykorzystanaKwota = koszyk.Klient.IloscWykorzystanegoKredytu;

            //porownujemy wartosc kredytu ktora mu pozostała z kwotą koszyka
            return koszyk.CalkowitaWartoscHurtowaBruttoPoRabacie().Wartosc.PorownajWartosc(maxymalnaWartoscKredytu- wykorzystanaKwota, WartoscWarunek);
        }

        [FriendlyName("Wartość kredytu ma być")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Wartosc WartoscWarunek { get; set; }

        [FriendlyName("Mnożnik limitu kredytu kupieckiego - wpisanie np. 1.1 powoduje aktywacje warunku dopiero po przekroczeniu 10% ponad pozostały limit kredytu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Mnoznik { get; set; }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return KoszykSpelniaRegule(koszyk);
        }
    }
}