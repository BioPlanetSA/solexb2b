using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class CechaNowoscDlaOstatnichProduktowProvider : IImportDataModule
    {
        #region IImportDataModule Members

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Dodawanie cechy nowość"));
            }
            string attrib_name = configuration["CechaNowoscDlaOstatnichProduktow_nazwa_cechy"];
            int count = int.Parse(configuration["CechaNowoscDlaOstatnichProduktow_ilosc_produktow"]);
            if (!string.IsNullOrEmpty(attrib_name) && count > 0)
            {
                if (!db.Attributes.Any(p => p.symbol == attrib_name))
                {
                    SolEx.Hurt.Model.cechy tmp_a = new Model.cechy();
                    tmp_a.symbol = attrib_name;
                    tmp_a.cecha_id = 9999;
                    db.Attributes.Add(tmp_a);
                }
                db.Products = db.Products.OrderByDescending(p => p.produkt_id).ToList();
                int max = Math.Min(count,db.Products.Count);
                int sel = 0;
                for (int i = 0; i < db.Products.Count && sel<max; i++)
                {
                    if (db.Products[i].widoczny)
                    {
                        if (!db.Products[i].AttributeSymbols.Contains(attrib_name))
                        {
                            db.Products[i].AttributeSymbols.Add(attrib_name);
                        }
                        sel++;
                    }
                }
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Zakończenie dodawania cechy nowość"));
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
