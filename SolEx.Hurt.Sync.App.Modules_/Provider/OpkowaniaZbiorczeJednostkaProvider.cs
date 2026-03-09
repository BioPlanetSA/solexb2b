using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    class OpkowaniaZbiorczeJednostkaProvider : IImportDataModule
    {
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Kopiowanie dodatkowej jednostki jako opakowania zbiorczego"));
            }
            for (int i = 0; i < db.Products.Count; i++)
            {
                db.Products[i].ilosc_w_opakowaniu =(db.Products[i].jednostka_miary_przelicznik1 > 0 ? db.Products[i].jednostka_miary_przelicznik1 : db.Products[i].ilosc_w_opakowaniu);
                if (db.Products[i].ilosc_w_opakowaniu > 1)
                {
                    db.Products[i].ilosc_w_opakowaniu_tryb =2;
                }
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Zakończenie kopiowanie dodatkowej jednostki jako opakowania zbiorczego"));
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;
    }
}
