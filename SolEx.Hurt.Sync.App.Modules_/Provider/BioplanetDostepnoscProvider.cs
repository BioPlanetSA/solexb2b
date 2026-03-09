using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    class BioplanetDostepnoscProvider : IImportDataModule
    {
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Poczatek opis dostepnosc"));
            }
            string avilableFrom =configuration["BioplanetDostepnosc_od_kiedy"];
            string text1 = configuration["BioplanetDostepnosc_tekst_z_Data"];
            string text2 = configuration["BioplanetDostepnosc_tekst"];
            for (int i = 0; i < db.Products.Count; i++)
            {
                //if (db.Products[i].ErpPars!=null)
                //{
                //    DateTime avilablefromDate = DateTime.MinValue;
                //   // string text = "";
                  
                //    //{

                //    //    log.Error(db.Products[i].nazwa + " " + db.Products[i].ErpPars[avilableFrom] + " " + avilableFrom + " "
                //    //        + DateTime.TryParse(db.Products[i].ErpPars[avilableFrom], out avilablefromDate).ToString() + " " + avilablefromDate.ToString() + " " + DateTime.Now + " "
                //    //        +(avilablefromDate>DateTime.Now).ToString());

                //    //}

                //    if (DateTime.TryParse(db.Products[i].ErpPars[avilableFrom], out avilablefromDate) && avilablefromDate>DateTime.Now)
                //    {
                //        db.Products[i].opis_krotki = string.Format(text1, avilablefromDate.ToShortDateString());
                //     //   log.Error(db.Products[i].nazwa + " " + db.Products[i].DescShort);
                //    }
             //   }
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Koniec opis dostepnosc"));
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;
    }
}
