namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    using SolEx.Hurt.Sync.App.Modules_;
    using SolEx.Hurt.Model;
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;
    using SolEx.Hurt.Sync.Core;

    public class DrugiOpiekunProvider : IImportDataModule
    {
        public event ProgressChangedEventHandler ProgresChanged;

        public void DoWork(NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                this.ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie drugiego opiekuna z cech"));
            }
         
            //string prefix = configuration["drugiOpiekun_prefiks"];
            //if (string.IsNullOrEmpty(prefix))
            //{
            //    prefix = "#";
            //}
            //for (int i = 0; i < db.Customers.Count; i++)
            //{
            //    db.Customers[i].DrugiOpiekunEmail = "";
            //    if (db.Customers[i].Attibutes != null)
            //    {
            //        Trait s = db.Customers[i].Attibutes.FirstOrDefault<Trait>(p => p.symbol.StartsWith(prefix));
            //        if (s != null)
            //        {
            //            db.Customers[i].DrugiOpiekunEmail = s.symbol;
            //        }
            //    }
            //}
        }
    }
}

