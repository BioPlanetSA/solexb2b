using SolEx.Hurt.Core.BLL.WarunkiRegulPunktow;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.RegulyPunktowe
{
    public abstract class RegulaPunktowaPozycjiDokumentu : RegulaPunktowa
    {
        public abstract decimal WyliczPunkty(DokumentyPozycje pozycjaWyliczna, DokumentyBll dokumentNaKtorymJestPozycja, decimal punktyPoprzednieReguly);

        public bool CzySpelniaKryteria(DokumentyPozycje pozycjaWyliczna, DokumentyBll dokument, List<Type> typyRegulDoPomieniac = null)
        {
            bool czyWykonac = true;
            var warunki = Warunki().Select(x => (IWarunekRegulyPozycjiDokumentu)x.Modul()).ToList();
            foreach (var zadanie in warunki)
            {
                if (typyRegulDoPomieniac != null && typyRegulDoPomieniac.Contains(zadanie.GetType()))
                {
                    continue;
                }
                if (!zadanie.SpelniaWarunek(pozycjaWyliczna, dokument) && !zadanie.OdwrocenieWarunku)
                {
                    czyWykonac = false;
                    break;
                }
            }
            return czyWykonac;
        }

        public override Type TypWarunkow
        {
            get { return typeof(IWarunekRegulyPozycjiDokumentu); }
        }
    }
}