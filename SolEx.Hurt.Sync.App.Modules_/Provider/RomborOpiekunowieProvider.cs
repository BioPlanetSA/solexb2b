using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class RomborOpiekunowieProvider : IImportDataModule
    {
        public event ProgressChangedEventHandler ProgresChanged;
        public void DoWork(NameValueCollection configuration, SourceDB db)
        {
            //for (int i = 0; i < db.Customers.Count; i++)
            //{
            //    if (db.Customers[i].Category.ToUpper() == "CENTRUM")
            //    {
            //        db.Customers[i].PrzedstawicielEmail = "centrum1";
            //        db.Customers[i].OpiekunEmail = "centrum2";
            //    }
            //    else if (db.Customers[i].Category.ToUpper() == "ZACHÓD")
            //    {
            //        db.Customers[i].PrzedstawicielEmail = "zachod1";
            //        db.Customers[i].OpiekunEmail = "zachod2";
            //    }
            //    else if (db.Customers[i].Category.ToUpper() == "WSCHÓD")
            //    {
            //        db.Customers[i].PrzedstawicielEmail = "wschod1";
            //        db.Customers[i].OpiekunEmail = "wschod2";
            //    }
            //    else if (db.Customers[i].Category.ToUpper() == "POŁUDNIE")
            //    {
            //        db.Customers[i].PrzedstawicielEmail = "poludnie1";
            //        db.Customers[i].OpiekunEmail = "poludnie2";
            //    }
            //}
        }
    }
}

