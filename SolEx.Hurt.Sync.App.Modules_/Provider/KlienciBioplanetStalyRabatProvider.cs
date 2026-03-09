using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class KlienciBioplanetStalyRabatProvider : IImportDataModule
    {
        public event ProgressChangedEventHandler ProgresChanged;

        public void DoWork(NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                this.ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie stałych rabatów, ilość rabatów " + db.Discounts.Count.ToString()));
            }
            for (int i = 0; i < db.Customers.Count; i++)
            {
                int source = -(19 + 1000000 + (100000 + (db.Customers[i].klient_id * 50)));
                // if (db.Discounts.Where(p => p.id == source).Count() > 0) continue;
                DiscountItem tmp = new DiscountItem();
                tmp.Value1 = 20;
                tmp.ValueType = DiscountItemValueType.Percent;
                tmp.CustomerId = db.Customers[i].klient_id;
                // tmp.AttributeID = 19;
                tmp.ProductCategoryId = 93;
                tmp.DiscountType = DiscountItemType.Advance;
                tmp.Id = source;
                db.Discounts.Add(tmp);
            }
            this.ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie stałych rabatów koniec, ilość rabatów " + db.Discounts.Count.ToString()));
        }
    }
}
