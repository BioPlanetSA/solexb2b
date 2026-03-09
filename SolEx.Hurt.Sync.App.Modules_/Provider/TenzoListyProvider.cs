using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Sync.App.Modules_.DAO;
using SolEx.Hurt.Sync.App.Modules_.Helpers;
using System.Collections.Specialized;
using SolEx.Hurt.Sync.Core;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class TenzoListyProvider : IImportDataModule
    {
        #region IImportDataModule Members

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            DateTime start;
            if (!DateTime.TryParse(configuration["program_import_start_date"], out start))
            {
                start = CoreManager.GetDocumentsStartDate(null);
            }
            SubiektDataContext dc = null;
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie listów przewozowych"));
            }

            try
            {
                dc = new SubiektDataContext(configuration["erp_cs"]);
                string fieldName = string.IsNullOrEmpty(configuration["b2b_list_przewozowy"]) ? "b2b_list_przewozowy" : configuration["b2b_list_przewozowy"];
                string subiektName = Subiekt_DoPoprawki.GetFieldName(fieldName, dc, -2);
                var docs = (from d in dc.dok__Dokuments
                            join p in dc.pw_Danes on d.dok_Id equals p.pwd_IdObiektu
                            where  (d.dok_Typ == 2 || d.dok_Typ == 6)
                            && (start != DateTime.MinValue ? d.dok_DataWyst >= start : true)
                            select new
                            { d.dok_Id, p, d.dok_NrPelny }).ToList();
                List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>(docs.Count);
                for (int i = 0; i < docs.Count; i++)
                {
                    string value = Subiekt_DoPoprawki.GetFieldValue(subiektName, docs[i].p);
                    if (string.IsNullOrEmpty(value))
                        continue;
                    string key = docs[i].dok_NrPelny;
                    KeyValuePair<string, string> tmp = new KeyValuePair<string, string>(key, value);
                    data.Add(tmp);
                }
                if (this.ProgresChanged != null)
                {
                    ProgresChanged(this, new ProgressChangedEventArgs("Liczba listów " + data.Count.ToString()));
                }

                string fileName = db == null ? "csv_dokumenty_listy_mini.csv" : "csv_dokumenty_listy.csv";
                SyncManager.SaveFile(fileName, CreateCSV(data)
                , AppDomain.CurrentDomain.BaseDirectory + (configuration["program_mode"] == "-auto" ? "paczka_temp_auto\\" : "paczka_temp\\"));
            }
            catch (Exception ex)
            {
                if (this.ProgresChanged != null)
                {
                    ProgresChanged(this, new ProgressChangedEventArgs("Błąd " + ex.Message + " " + ex.StackTrace));
                }
            }
            finally
            {
                if (dc != null)
                    dc.Dispose();
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie listów przewozowych koniec"));
            }
        }

        private string CreateCSV(List<KeyValuePair<string, string>> data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> d in data)
            {
                sb.Append(d.Key);
                sb.Append("|");
                sb.Append(d.Value);
                sb.Append(";");
            }

            return sb.ToString();
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
