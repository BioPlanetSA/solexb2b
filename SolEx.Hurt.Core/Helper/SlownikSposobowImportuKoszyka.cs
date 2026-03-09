using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.Importy;
using SolEx.Hurt.Core.Importy.Koszyk;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.Helper
{
   public  class SlownikSposobowImportuKoszyka: SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var sposob in  ManagerImportow.PobierzDostepneModuly())
                {
                    Type t = sposob.GetType();
                    wynik.Add(sposob.LadnaNazwa,t.PobierzOpisTypu());
                }
                return wynik;
            }
        }
    }
}
