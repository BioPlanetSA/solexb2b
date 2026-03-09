using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    class EssaProducenciProvider : IImportDataModule
    {
        #region IImportDataModule Members

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Dodawanie atrybutu producent"));
            }
            string group_prefix = configuration["group_prefix"];
            bool gr = !string.IsNullOrEmpty(group_prefix);
            if (gr)
            {
                string[] prefix = group_prefix.Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < db.Attributes.Count; i++)
                {
                    if (db.Attributes[i].symbol.StartsWith(prefix[0]))
                    {
                        db.Attributes[i].symbol = db.Attributes[i].symbol.Replace(prefix[0], "");
                        db.Attributes[i].atrybut_id = 9999999;
                        db.Attributes[i].nazwa = "Producent";
                    }
                }
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Zakończenie dodawania atrybutu producent"));
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
