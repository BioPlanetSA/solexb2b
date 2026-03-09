using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class TrojkaKonfekcjaProvider : IImportDataModule
    {
        #region IImportDataModule Members
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {

            //    b2b_dodatkowa_jednostka || B2B_1KG

            string prefiks = configuration["b2b_konfekcja_prefiks"];
            for (int i = 0; i < db.Products.Count; i++)
            {
                var s =   db.Products[i].AttributeSymbols.Where(p => p.ToLower().Contains(prefiks.ToLower())).ToList();
                for (int j = 0; j < s.Count; j++)
                {
                    int pos = s[j].LastIndexOfAny(new char[] { '+', '=' });
                    string name = "";
                    if (pos >= 0)
                    {
                        name = s[j].ToLower().Substring(pos);
                    }
                    log.Error(name);
                    //if (j == 0)
                    //{
                    //    db.Products[i].QuantityUnit = name;
                    // //   db.Products[i].UnitP1 = 1;
                    //}
                    if (j == 0)
                    {
                        db.Products[i].jednostka_miary1 = name;
                        db.Products[i].jednostka_miary_przelicznik1 = (decimal)1.0001;
                    }
                    if (j == 1)
                    {
                        db.Products[i].jednostka_miary2 = name;
                        db.Products[i].jednostka_miary_przelicznik2 = (decimal)1.0001;
                    }
                    if (j == 2)
                    {
                        db.Products[i].jednostka_miary3 = name;
                        db.Products[i].jednostka_miary_przelicznik3 = (decimal)1.0001;
                    }
                }
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
