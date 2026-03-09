using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.Helper
{
    public class Slownik_Integracja_Szablony: SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                //tylko DODATNIE iD w slowniku pokazujemy
                return SolexBllCalosc.PobierzInstancje.Konfiguracja.TablicaPlikowIntegracjiWgId.Values.Distinct().Where(x => x.IdSzablonu > 0).OrderBy(x=> x.typDanych).
                    ThenBy(x=> x.IdSzablonu).ToDictionary(x => string.Format("[{0} {2}] {1}", x.typDanych , x.Szablon , x.Format), x => x.IdSzablonu as object);
            }
        }
    }
}
