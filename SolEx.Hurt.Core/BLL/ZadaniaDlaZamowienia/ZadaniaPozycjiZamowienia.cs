using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;

namespace SolEx.Hurt.Core.BLL.ZadaniaDlaZamowienia
{
    public abstract class ZadaniaPozycjiZamowienia : ZadanieKoszyka
    {
        public abstract bool Wykonaj(IKoszykPozycja pozycja, ZamowieniaProduktyBLL zamowieniePozycja);
        public override Type TypWarunkow => typeof(IRegulaPozycji);
        public bool WykonajZadanie(IKoszykPozycja pozycja, ZamowieniaProduktyBLL zamowieniePozycja)
        {
            bool wynik = false;
            if (CzySpelniaKryteria(pozycja,zamowieniePozycja))
            {
                wynik = Wykonaj(pozycja, zamowieniePozycja);
            }
            return wynik;
        }

        public bool CzySpelniaKryteria(IKoszykPozycja pozycja, ZamowieniaProduktyBLL zamowieniePozycja, List<Type> typyRegulDoPomieniac = null)
        {
            bool czyWykonac = true;
            var warunki = Warunki().Select(x => (IRegulaPozycji)x.Modul()).ToList();
            foreach (var zadanie in warunki)
            {
                if (typyRegulDoPomieniac != null && typyRegulDoPomieniac.Contains(zadanie.GetType()))
                {
                    continue;
                }
                czyWykonac = zadanie.OdwrocenieWarunku ? !zadanie.PozycjaSpelniaRegule(pozycja,null) : zadanie.PozycjaSpelniaRegule(pozycja,null);

                if (!czyWykonac)
                {
                    break;
                }
            }
            return czyWykonac;
        }

    }
}
