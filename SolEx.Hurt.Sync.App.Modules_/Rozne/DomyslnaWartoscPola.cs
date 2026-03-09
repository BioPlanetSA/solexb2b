using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Helpers;
using log4net;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Model.Extensions;

namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    
    public abstract  class DomyslnaWartoscPolaBase : SyncModul
    {
      
        [Niewymagane]
        [FriendlyName("Domyślna wartość kopiowana do pól")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string WartoscDomyslna { get; set; }

        [FriendlyName("Jeśli domyślna wartość jest pusta to wstaw NULL")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool NullZamiastPustegoStringa { get; set; }

        public void Przetworz<T>(List<T> listaWejsciowa, List<string> Pola)
        {
            PropertyInfo[] propertisy = typeof(T).GetProperties();

            foreach (T k in listaWejsciowa)
            {
                foreach (var p in propertisy)
                {
                    if (Pola.Contains(p.Name))
                    {
                        try
                        {

                            if (NullZamiastPustegoStringa && string.IsNullOrEmpty(WartoscDomyslna) && p.PropertyType.IsNullableType())
                            {
                                p.SetValue(k, null, null);
                                continue;
                            }

                            p.SetValueExt(k, WartoscDomyslna);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("Nie udało się zapisać wartości {0} do pola {1} z powodu błędu: {2}", WartoscDomyslna, p.Name, ex.Message), ex);
                        }
                    }
                }
            }
        }
    }

}
