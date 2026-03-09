using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    class BioplanetMinimumOpakowanieProvider : IImportDataModule
    {
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Kopiowanie dodatkowej jednostki jako opakowania zbiorczego i minimum logistycznego"));
            }
            for (int i = 0; i < db.Products.Count; i++)
            {
                decimal inbox = db.Products[i].jednostka_miary1 == "OZ"?db.Products[i].jednostka_miary_przelicznik1:(db.Products[i].jednostka_miary2 == "OZ"?db.Products[i].jednostka_miary_przelicznik2:(db.Products[i].jednostka_miary3 == "OZ"?db.Products[i].jednostka_miary_przelicznik3:db.Products[i].ilosc_w_opakowaniu));
                decimal minimum = db.Products[i].jednostka_miary1 == "ML" ? db.Products[i].jednostka_miary_przelicznik1 : (db.Products[i].jednostka_miary2 == "ML" ? db.Products[i].jednostka_miary_przelicznik2 : (db.Products[i].jednostka_miary3 == "ML" ? db.Products[i].jednostka_miary_przelicznik3 : db.Products[i].ilosc_minimalna));
                db.Products[i].ilosc_w_opakowaniu = inbox;
                db.Products[i].ilosc_minimalna = minimum;
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Kopiowanie dodatkowej jednostki jako opakowania zbiorczego i minimum logistycznego koniec"));
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;
    }
}
