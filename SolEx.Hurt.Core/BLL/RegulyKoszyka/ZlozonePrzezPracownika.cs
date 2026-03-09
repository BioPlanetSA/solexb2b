using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName( "Czy koszyk jest edytowany przez konkretnego pracownika")]
    public class ZlozonePrzezPracownika : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji
    {
        [FriendlyName("Czy pracownik jest/nie jest wśród wybranych")]
        [WidoczneListaAdmin(false, false, true, false)]
        public RelacjaJestNieJest Relacja { get; set; }

        [FriendlyName("Wybierz pracownikow")]
        [PobieranieSlownika(typeof(SlownikPracownikow))]
        [WidoczneListaAdmin(false, false, true, false)]
        public List<long> Pracownicy { get; set; }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return KoszykSpelniaRegule(koszyk);
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            if (koszyk.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta != null )
            {
                return false;
            }
            bool wynik = Pracownicy.Any(x => x  == koszyk.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta.Id);
            return Relacja == RelacjaJestNieJest.NieJest ? !wynik : wynik;
        }
    }
}