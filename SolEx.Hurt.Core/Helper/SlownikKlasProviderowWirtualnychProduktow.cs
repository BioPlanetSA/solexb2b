using System;
using System.Collections.Generic;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKlasProviderowWirtualnychProduktow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                var listaKlas = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.ProduktyWirtualneProvider));
                foreach (Type  klasa in listaKlas)
                {
                    wynik.Add(klasa.Name, klasa.PobierzOpisTypu());
                }
                return wynik;
            }
        }
    }
}
