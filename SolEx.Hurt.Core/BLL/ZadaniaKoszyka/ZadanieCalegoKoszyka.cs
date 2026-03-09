using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public abstract class ZadanieCalegoKoszyka : ZadanieKoszyka
    {
        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;

        public abstract bool Wykonaj(IKoszykiBLL koszyk);

        public bool WykonajZadanie(KoszykBll koszyk)
        {
            bool wynik = false;
            if (CzySpelniaKryteria(koszyk))
            {
                wynik = Wykonaj(koszyk);
            }
            return wynik;
        }

        public bool CzySpelniaKryteria(IKoszykiBLL koszyki, List<Type> typyRegulDoPomieniac = null)
        {
            bool czyWykonac = true;
            var warunki = Warunki().Select(x => (IRegulaCalegoKoszyka)x.Modul()).ToList();
            foreach (var zadanie in warunki)
            {
                if (typyRegulDoPomieniac != null && typyRegulDoPomieniac.Contains(zadanie.GetType()))
                {
                    continue;
                }
                czyWykonac = zadanie.OdwrocenieWarunku ? !zadanie.KoszykSpelniaRegule(koszyki) : zadanie.KoszykSpelniaRegule(koszyki);

                if (!czyWykonac)
                {
                    break;
                }
                //if (!zadanie.KoszykSpelniaRegule(koszyki) && !zadanie.OdwrocenieWarunku)
                //{
                //    czyWykonac = false;
                //    break;
                //}
            }
            return czyWykonac;
        }

        public override Type TypWarunkow => typeof(IRegulaCalegoKoszyka);
    }
    
}