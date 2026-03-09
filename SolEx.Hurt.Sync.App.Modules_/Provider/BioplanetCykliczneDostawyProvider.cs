using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class BioplanetCykliczneDostawyProvider : IImportDataModule
    {
        #region IImportDataModule Members
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            string[] Days = new string[] { "Niedziela", "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela" };
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Początek cyklicznych dostaw"));
            }
          string fieldName = configuration["BioplanetCykliczneDostawy_pole"];
          string textformat = configuration["BioplanetCykliczneDostawy_fraza"];
           
          for (int i = 0; i < db.Products.Count; i++)
          {
              int day = (int)DateTime.Now.DayOfWeek;
              //if (db.Products[i].ErpPars != null && !string.IsNullOrEmpty(db.Products[i].ErpPars[fieldName]))
              //{
              //    string val = db.Products[i].ErpPars[fieldName];
              //    try
              //    {
              //        string[] days = val.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

              //        string nextDay="";
              //        foreach(string s in days)
              //        {
              //            int deliveryDay=int.Parse(s)%7;
                   
              //            if (deliveryDay > day)
              //            {
              //                nextDay = Days[deliveryDay];
              //                break;
              //            }
              //        }
              //        if (string.IsNullOrEmpty(nextDay))//jeśli nie ma już dnia w tym tygodniu to ustawiamy na następny tydzień
              //        {
              //            day = 0;
              //            foreach (string s in days)
              //            {
              //                int deliveryDay = int.Parse(s) % 7;

              //                if (deliveryDay > day)
              //                {
              //                    nextDay = Days[deliveryDay];
              //                    break;
              //                }
              //            }
              //        }
              //        db.Products[i].opis_krotki=string.Format(textformat,nextDay);
                        
              //    }
              //    catch (Exception ex) { log.Error(string.Format("Błąd parsowania cyklicznej dostawy {0} Błąd {1} Stack trace {2}",db.Products[i].nazwa,ex.Message,ex.StackTrace)); }
              //}
          }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Koniec cyklicznych dostaw"));
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
