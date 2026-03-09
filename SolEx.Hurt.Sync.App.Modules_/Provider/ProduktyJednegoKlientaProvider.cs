using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.Core;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class ProduktyJednegoKlientaProvider : IImportDataModule
    {
        #region IImportDataModule Members

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie produktów klienta"));
            }
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
            string attrib_name = configuration["ProduktyJednegoKlienta_artybut_symbol"];
            string trait_symbol = configuration["ProduktyJednegoKlienta_cecha_symbol"];
            string customer_symbol = configuration["ProduktyJednegoKlienta_klient_symbol"];
            if (!string.IsNullOrEmpty(attrib_name) && (!string.IsNullOrEmpty(customer_symbol) || !string.IsNullOrEmpty(trait_symbol)))
            {
                int customerID = 0;
                var c = db.Customers.FirstOrDefault(p => p.symbol == customer_symbol);
                if (c != null)
                {
                    customerID = c.klient_id;
               
                }
               
                var atr = db.Attributes.FirstOrDefault(p => p.symbol == trait_symbol || p.nazwa == attrib_name);
                if (atr != null)
                {
                    for (int i = 0; i < db.Products.Count; i++)
                    {
                        var pr = db.Products.Where(p => p.AttributeIds.Contains(atr.cecha_id) || p.AttributeSymbols.Contains(atr.symbol));
                        foreach (var p in pr)
                        {
                            data.Add(new KeyValuePair<string, string>(p.produkt_id.ToString(), db.Customers[i].klient_id.ToString()));
                        }
                    }
                }
                string fileName = "csv_produkty_ukryte.csv";
                SyncManager.SaveFile(fileName, CreateCSV(data)
                , AppDomain.CurrentDomain.BaseDirectory + (configuration["program_mode"] == "-auto" ? "paczka_temp_auto\\" : "paczka_temp\\"));

            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie produktów klienta koniec"));
            }
        }
        private string CreateCSV(List<KeyValuePair<string, string>> data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> d in data)
            {
                sb.Append(d.Key);
                sb.Append("|");
                sb.Append(d.Value);
                sb.Append(";");
            }

            return sb.ToString();
        }
        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}