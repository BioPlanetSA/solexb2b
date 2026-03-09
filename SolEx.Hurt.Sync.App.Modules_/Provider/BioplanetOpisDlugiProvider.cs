using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class BioplanetOpisDlugiProvider : IImportDataModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region IImportDataModule Members
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Początek długich opisów"));
            }
            for (int i = 0; i < db.Products.Count; i++)
            {
                //if (db.Products[i].ErpPars != null)
                //{
                //    try
                //    {
                //        string desc = db.Products[i].ErpPars["tw_Charakter"];
                //        string informacjaAlergiczna = db.Products[i].ErpPars["tw_Uwagi"];
                //        string wartoscEnergetyczna = db.Products[i].ErpPars["1.Wartość energet."];
                //        string bialko = db.Products[i].ErpPars["2. Białko"];
                //        string weglowodany = db.Products[i].ErpPars["3. Węglowodany"];
                //        string tluszcz = db.Products[i].ErpPars["4.Tłuszcz"];
                //        StringBuilder sb = new StringBuilder();
                //        sb.Append(desc);
                //        if (!string.IsNullOrEmpty(wartoscEnergetyczna) || !string.IsNullOrEmpty(bialko) || !string.IsNullOrEmpty(weglowodany) || !string.IsNullOrEmpty(tluszcz) )
                //        sb.Append("</br><b>Wartość odżywcza w 100 g:</b></br>");
                //        if (!string.IsNullOrEmpty(wartoscEnergetyczna))
                //            sb.AppendFormat("WARTOŚĆ ENERGETYCZNA - {0}</br>", wartoscEnergetyczna);
                //        if (!string.IsNullOrEmpty(bialko))
                //            sb.AppendFormat("BIAŁKO - {0}</br>", decimal.Parse(bialko.Replace(".",",")).ToString("0.####"));
                //        if (!string.IsNullOrEmpty(weglowodany))
                //            sb.AppendFormat("WĘGLOWODANY - {0}</br>", decimal.Parse(weglowodany.Replace(".", ",")).ToString("0.####"));
                //        if (!string.IsNullOrEmpty(tluszcz))
                //            sb.AppendFormat("TŁUSZCZ - {0}</br>", decimal.Parse(tluszcz.Replace(".", ",")).ToString("0.####"));
                //        if (!string.IsNullOrEmpty(informacjaAlergiczna))
                //            sb.AppendFormat("</br><b>Informacja alergiczna:</b></br>{0}", informacjaAlergiczna);
                //        db.Products[i].opis = sb.ToString();
                //    }
                //    catch (Exception ex) { log.Error(string.Format("Błąd parsowania długiego opisu  {0} Błąd {1} Stack trace {2}", db.Products[i].nazwa, ex.Message, ex.StackTrace)); }
                //}
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Koniec długich opisów"));
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
