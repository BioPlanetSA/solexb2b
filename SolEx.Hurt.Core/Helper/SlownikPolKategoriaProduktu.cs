using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPolKategoriaProduktu : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                KategorieBLL p = new KategorieBLL();
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var propertyInfo in p.GetType().GetProperties())
                {
                    if (!propertyInfo.Name.Contains("Id") && propertyInfo.Name != "PoziomWidocznosci" && propertyInfo.Name != "Aktywny" && !wynik.Keys.Contains(propertyInfo.Name))
                    {
                        wynik.Add(propertyInfo.Name, propertyInfo.Name);
                    }
                }
                return wynik;
            }
        }
    }
}
