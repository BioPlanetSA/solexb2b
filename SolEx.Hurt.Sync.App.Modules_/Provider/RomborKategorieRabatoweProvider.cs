using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    class RomborKategorieRabatoweProvider : IImportDataModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region IImportDataModule Members

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Liczba kategorii przed " + db.SourceCategories.Count.ToString()));
            }
            int id=14999;
            List<int> categoriesIDs = db.SourceCategories.Where(p => p.nazwa.StartsWith("RABAT1;")).Select(p => p.id).ToList();
            if (!db.SourceCategories.Any(p => p.nazwa == "RABAT1;INNE"))//dodajemy kategorie zrodlowa rabat1;inne
            {
                Category tmp = new Category();
                tmp.nazwa = "RABAT1;INNE";
                tmp.id = id;
                db.SourceCategories.Add(tmp);
            }
            else
            {
                id = db.SourceCategories.First(p => p.nazwa == "RABAT1;INNE").id;
            }
            if (db.Products!=null && db.Products.Count > 0) //dodajemy łącznik do tej kategorii zrodlowej
            {
                if (this.ProgresChanged != null)
                {
                  ProgresChanged(this, new ProgressChangedEventArgs("Dodawanie łącznika do kategorii rabatowej" + db.SourceCategories.Count.ToString()));
                }
                for (int i = 0; i < db.Products.Count; i++)
                {
                    for (int j = 0; j < db.Products[i].CategoryIds.Count; j++)
                    {
                       if(!categoriesIDs.Any(p=>db.Products[i].CategoryIds.Contains(p)))///jeśli produkt nie jest przypisany do żadnej z kategorii rabatowych to dodajemy go do domyślnej
                       {
                           db.Products[i].CategoryIds.Add(id);
                           log.Error(string.Format("Dodano produkt {0} do kategorii zrodlowej {1} ",db.Products[i].nazwa,id));
                       }
                    }
                }
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Liczba kategorii po " + db.SourceCategories.Count.ToString()));
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
