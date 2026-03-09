using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    class SaturnRabatyProvider : IImportDataModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region IImportDataModule Members

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie rabatów klientów"));
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Liczba rabatów przed " + db.Discounts.Count.ToString()));
            }
            //for (int i = 0; i < db.Customers.Count; i++)
            //{
            //    for (int j = 0; j < db.Customers[i].Parameters.Count; j++)
            //    {
            //        if (!db.Customers[i].aktywny || string.IsNullOrEmpty(db.Customers[i].haslo_docelowe) || string.IsNullOrEmpty(db.Customers[i].email)) continue;
            //        decimal additionalDiscount = 0;
            //        if (!string.IsNullOrEmpty(configuration["rabaty_dodatkowy"]))
            //        {
            //            string additional = db.Customers[i].Parameters[configuration["rabaty_dodatkowy"]];
            //            decimal.TryParse(additional!=null?additional.Replace(".",","):"", out additionalDiscount);
            //        }
            //     //   log.Error(string.Format("Klient {0} Klucz {1} prefiks {2} dodatkowy rabat {3}",db.Customers[i].nazwa, db.Customers[i].Parameters.Keys[j],configuration["rabaty_prefiks"],additionalDiscount));

            //        if (db.Customers[i].Parameters.Keys[j].ToLower().StartsWith(configuration["rabaty_prefiks"].ToLower()) && configuration["rabaty_dodatkowy"] != db.Customers[i].Parameters.Keys[j])
            //        {
            //            string val = db.Customers[i].Parameters[j];
            //            // log.Error("Wartość " + val);
            //            decimal discount = 0;
            //            decimal.TryParse(val!=null?val.Replace(".",","):"", out discount);

            //            if (db.Customers[i].Parameters.Keys[j] != "RABAT1;FŁT")
            //            {
            //                discount += additionalDiscount;
            //            }
            //            if (discount == 0) continue;
            //            var category = db.SourceCategories.FirstOrDefault(p => p.Name.ToLower().Trim() == db.Customers[i].Parameters.Keys[j].ToLower().Trim());
            //            if (category == null)
            //                continue;


            //            int source = -(category.Id + 1000000 + (100000 + (db.Customers[i].klient_id * 50)));
            //            DiscountItem tmp = new DiscountItem();
            //            tmp.CustomerId = db.Customers[i].klient_id;
            //            tmp.Value1 = discount;
            //            tmp.Value2 = discount;
            //            tmp.Value3 = discount;
            //            tmp.DiscountType = DiscountItemType.Simple;
            //            tmp.ValueType = DiscountItemValueType.Percent;
            //            tmp.ProductCategoryId = category.Id;
            //            tmp.Id = source;
            //            tmp.PriceLevelId = db.Customers[i].poziom_ceny;
            //            db.Discounts.Add(tmp);
            //            if (tmp.CustomerId == 1816)
            //            {
            //                log.Error(string.Format("{0}-{1}-{2}",tmp.Value1,tmp.ProductCategoryId,tmp.CustomerId));
            //            }
            //        }
            //    }
           // }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Liczba rabatów po " + db.Discounts.Count.ToString()));
            }

            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie rabatów klientów koniec"));
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
