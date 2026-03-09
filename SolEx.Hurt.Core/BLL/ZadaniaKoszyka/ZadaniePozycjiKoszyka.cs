using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public abstract class ZadaniePozycjiKoszyka : ZadanieKoszyka
    {
        public abstract bool Wykonaj(IKoszykPozycja pozycja, IKoszykiBLL koszyk);

        public bool WykonajZadanie(KoszykPozycje pozycje, KoszykBll koszyk)
        {
            bool wynik = false;
            if (CzySpelniaKryteria(pozycje, koszyk))
            {
                wynik = Wykonaj(pozycje, koszyk);
            }
            return wynik;
        }

        public bool CzySpelniaKryteria(IKoszykPozycja pozycja,IKoszykiBLL koszyk, List<Type> typyRegulDoPomieniac = null)
        {
            bool czyWykonac = true;
            var warunki = Warunki().Select(x => (IRegulaPozycji)x.Modul()).ToList();
            foreach (var zadanie in warunki)
            {
                if (typyRegulDoPomieniac != null && typyRegulDoPomieniac.Contains(zadanie.GetType()))
                {
                    continue;
                }
                if ((zadanie.PozycjaSpelniaRegule(pozycja, koszyk) && zadanie.OdwrocenieWarunku) || (!zadanie.PozycjaSpelniaRegule(pozycja, koszyk) && !zadanie.OdwrocenieWarunku))
                {
                    czyWykonac = false;
                    break;
                }
            }
            return czyWykonac;
        }

        public override Type TypWarunkow
        {
            get { return typeof(IRegulaPozycji); }
        }
    }
}