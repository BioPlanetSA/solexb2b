using SolEx.Hurt.Core.BLL.WarunkiRegulPunktow;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.BLL.RegulyPunktowe
{
    public abstract class RegulaPunktowaCalegoDokumentu : RegulaPunktowa
    {
        public abstract int WyliczPunkty(DokumentyBll dokument, decimal punktyPoprzednieReguly);

        public bool CzySpelniaKryteria(DokumentyBll dokument, List<Type> typyRegulDoPomieniac = null)
        {
            bool czyWykonac = true;
            var warunki = Warunki().Select(x => (IWarunekRegulyCalegoDokumentu)x.Modul()).ToList();
            foreach (var zadanie in warunki)
            {
                if (typyRegulDoPomieniac != null && typyRegulDoPomieniac.Contains(zadanie.GetType()))
                {
                    continue;
                }
                if (!zadanie.SpelniaWarunek(dokument) && !zadanie.OdwrocenieWarunku)
                {
                    czyWykonac = false;
                    break;
                }
            }
            return czyWykonac;
        }

        public override Type TypWarunkow
        {
            get { return typeof(IWarunekRegulyCalegoDokumentu); }
        }
    }
}