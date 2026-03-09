using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SolEx.Hurt.Model;
using System.IO;
using SolEx.Hurt.Core.Helpers;
using SolEx.Hurt.Core;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class PobieranieZdjecZOptimyProvider : IImportDataModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region IImportDataModule Members
        private static string[] IllegalChars = new string[] { "/", "\\", ":", "*", "?", "|", "<", ">" };
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            int productCode = 0;
            Int32.TryParse(configuration["product_symbol"], out productCode);
            if ((configuration["program_mode2"] == "-lite") || (configuration["program_mode2"] == "-prices")) return;
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie zdjęć z optimy do katalogu"));
            }
            string path = configuration["PobieranieZdjecZOptimy_katalog_docelowy"];
            string import_file = AppDomain.CurrentDomain.BaseDirectory + "PobieranieZdjecZOptimyID.txt";
            int ids = 0;
            log.Error("b2b_obrazek " +configuration["b2b_obrazek"]);
            if (!string.IsNullOrEmpty(path))
            {
                if (File.Exists(import_file))
                {

                    string content = File.ReadAllText(import_file);
                    if (!int.TryParse(content, out ids))
                    {
                        ids = 0;
                    }
                    //     File.Delete(import_file);
                }

                SqlConnection conn = new SqlConnection(configuration["erp_cs"]);
                conn.Open();
                SqlCommand cmd = new SqlCommand(String.Format(" select a.twa_twAid,t.Twr_NumerKat,t.Twr_Kod,glowne=1 from cdn.TwrAtrybuty a join cdn.DefAtrybuty da on a.TwA_DeAId=da.DeA_DeAId join cdn.Towary t on a.TwA_TwrId=t.Twr_TwrId where da.DeA_Kod=@symbol and a.twa_twAid<={0}  order by a.twa_twAid", ids), conn);
                cmd.Parameters.AddWithValue("@symbol",configuration["b2b_obrazek"]);
                SqlDataReader rd = cmd.ExecuteReader();
                //usuwanie usuniętych zdjęć
                List<string> filesInDir = Directory.GetFiles(path).ToList();
                while (rd.Read())
                {
                   //  productCode == 2 ? v.Twr_NumerKat : v.Twr_Kod
                    int tmpid = rd.GetInt32(0);
                    bool main = true;
                    string fileName = String.Format("{0}_{1}{2}", rd.GetString(productCode == 2?1:2), main ? "m" : "n", tmpid);
                    string char_replace = configuration["import_illegal_char_replace"];
                    if (char_replace == null) char_replace = "_";
                    foreach (var x in IllegalChars)
                    {
                        fileName = fileName.Replace(x, char_replace);
                    }
                    // string fileName = String.Format("{0}_{1}{2}", rd.GetString(1), main ? "m" : "n", ids);
                    string file = filesInDir.FirstOrDefault(plik => plik.Contains(fileName));
                    if (!string.IsNullOrEmpty(file))
                    {
                        filesInDir.Remove(file);
                    }
                }
                rd.Close();
                rd.Dispose();
                if (this.ProgresChanged != null)
                {
                    ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie zdjęć z optimy do katalogu, usuniętych:" + filesInDir.Count.ToString()));
                    for (int i = 0; i < filesInDir.Count; i++)
                    {
                        ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie zdjęć z optimy do katalogu, plik:" + filesInDir[i]));
                    }

                }

                for (int i = 0; i < filesInDir.Count; i++)
                {
                    if (File.Exists(filesInDir[i]))
                    {
                        File.Delete(filesInDir[i]);
                    }
                }


                //tworzenie nowych zdjęć
                cmd = new SqlCommand(String.Format("select a.twa_twAid,t.Twr_NumerKat,t.Twr_Kod,glowne=1,dane=DAB_Wartosc from cdn.TwrAtrybuty a join cdn.DefAtrybuty da on a.TwA_DeAId=da.DeA_DeAId join cdn.Towary t on a.TwA_TwrId=t.Twr_TwrId join cdn.danebinarne db on db.DAB_DABID=a.TwA_DABID where da.DeA_Kod=@symbol and a.twa_twAid>{0} order by a.twa_twAid", ids), conn);
                cmd.Parameters.AddWithValue("@symbol", configuration["b2b_obrazek"]);
                rd = cmd.ExecuteReader();
                int iteration = 0;
                while (rd.Read())
                {
                    try
                    {
                        ids = rd.GetInt32(0);
                        bool main = true;
                        string fileName = String.Format("{0}_{1}{2}", rd.GetString(productCode == 2 ? 1 : 2), main ? "m" : "n", ids);
                        string char_replace = configuration["import_illegal_char_replace"];
                        if (char_replace == null) char_replace = "_";
                        foreach (var x in IllegalChars)
                        {
                            fileName = fileName.Replace(x, char_replace);
                        }
                        byte[] bytes = (byte[])rd[4];
                        ImgInfo info = new ImgInfo();
                        info.Path = path;

                        info.Orgingal = fileName;
                        try
                        {

                            info = Images.SaveImage(bytes, info, new WatermarkPars() { Enabled = false }, true);
                        }
                        catch (Exception ex) { log.Error("Pobieranie zdjęcia z optimy " + fileName); }
                        info = null;
                        if (iteration % 100 == 0)
                        {
                            if (File.Exists(import_file))
                            {
                                File.Delete(import_file);
                            }
                            File.WriteAllText(import_file, ids.ToString());
                        }
                    }
                    catch (Exception ex) { log.Error("Pobieranie zdjęcia z optimy " + ex.Message); }
                    iteration++;
                }
                rd.Close();
                rd.Dispose();
                if (File.Exists(import_file))
                {
                    File.Delete(import_file);
                }
                File.WriteAllText(import_file, ids.ToString());
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Zakończenie pobierania zdjęć z optimy do katalogu"));
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
