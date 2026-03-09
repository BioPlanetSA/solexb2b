using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    class OpiekunowieProvider : IImportDataModule
    {
        #region IImportDataModule Members

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie opiekunów z cech"));
            }
            //string prefix = configuration["opiekunowie_prefiks"];
            //if (string.IsNullOrEmpty(prefix))
            //    prefix = "%";
            //for (int i = 0; i < db.Customers.Count; i++)
            //{
            //    db.Customers[i].OpiekunEmail = "";
            //    if (db.Customers[i].Attibutes != null)
            //    {
            //        Trait s = db.Customers[i].Attibutes.FirstOrDefault(p => p.symbol.StartsWith(prefix));
            //        if (s != null)
            //        {
            //            db.Customers[i].OpiekunEmail = s.symbol;

            //        }
            //    }
            //}


        }
        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
