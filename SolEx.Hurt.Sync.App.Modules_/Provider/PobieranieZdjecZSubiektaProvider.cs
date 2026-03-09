using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.Helpers;
using SolEx.Hurt.Sync.App.Modules_.DAO;
using System.IO;
using System.Drawing;
using SolEx.Hurt.Core.Helpers;
using System.Data.SqlClient;
using System.Data.SQLite;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Core;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class PobieranieZdjecZSubiektaProvider : IImportDataModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region IImportDataModule Members
        private static string[] IllegalChars = new string[] { "/", "\\", ":", "*", "?", "|", "<", ">", ";", "^","\"","%","&","!","@","#","$","'","\"","`","~" };
        const string SELECT_SUBIEKT_BYTES = @"select zd_Zdjecie  from tw_ZdjecieTw where zd_Id={0}";
        const string DELETE_SQLLITE = @"delete from zdjecia where id={0}";
        const string INSERT_SQLLITE = @"insert into zdjecia(id,crc,glowny,symbol,nazwa) values({0},{1},{2},{3},{4})";
        public class Pic
        {
            public int ID{get;set;}
            public bool Main { get; set; }
            public int CRC { get; set; }
            public string Symbol { get; set; }
            public string Name { get; set; }
        }
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {

            if ((configuration["program_mode2"] == "-lite") || (configuration["program_mode2"] == "-prices")) return;
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie zdjęć z subiekta do katalogu"));
            }
              SQLiteCommand cmd = null;
                SQLiteConnection conn = null;
             
              SqlConnection subConn =null;
             SqlCommand subcmd =null;
             string path = configuration["PobieranieZdjecZSubiekta_katalog_docelowy"].EndsWith("\\") ? configuration["PobieranieZdjecZSubiekta_katalog_docelowy"] : configuration["PobieranieZdjecZSubiekta_katalog_docelowy"] + "\\";
            try
            {

                List<string> files = Directory.GetFiles(path).ToList();//pliki znalezione w katalogu
                subConn = new SqlConnection(configuration["erp_cs"]);
                subConn.Open();
                subcmd = new SqlCommand(String.Format("select zd_id,t.tw_symbol,zd_Glowne,zd_Crc,t.tw_Nazwa from tw_ZdjecieTw zt join tw__towar t on zt.zd_IdTowar=t.tw_Id  order by zd_id"), subConn);
                List<Pic> subiektPics = new List<Pic>(10000);
                using (SqlDataReader subrd = subcmd.ExecuteReader())
                {
                    while (subrd.Read())
                    {
                        string symbol = DataHelper.dbs("tw_symbol", subrd);
                        int id = DataHelper.dbi("zd_id", subrd);
                        int crc = DataHelper.dbi("zd_Crc", subrd);
                        bool main = DataHelper.dbb("zd_Glowne", subrd);
                        string name = DataHelper.dbs("tw_Nazwa", subrd);
                        Pic tmp = new Pic() { Symbol = symbol, ID = id, CRC = crc, Main = main,Name=name };
                        subiektPics.Add(tmp);
                    }
                } //wyciagniecie danych z suba
                List<Pic> sqlPics = new List<Pic>(10000);
                conn = new SQLiteConnection(configuration["pobieranie_zdjec_cs"]);
                conn.Open();
                cmd = new SQLiteCommand("select id,crc,symbol,glowny,nazwa  from zdjecia",conn);
                using (   SQLiteDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        string symbol = DataHelper.dbs("symbol", r);
                        string name = DataHelper.dbs("nazwa", r);
                        int id = DataHelper.dbi("id", r);
                        int crc = DataHelper.dbi("crc", r);
                        bool main = DataHelper.dbi("glowny", r)==1;
                        Pic tmp = new Pic() { Symbol = symbol, ID = id, CRC = crc, Main = main,Name=name };
                        sqlPics.Add(tmp);
                    }
                } //wyciagniecie danych z sqllite

                for (int i = 0; i < subiektPics.Count; i++) // porownywanie zdjec
                {
                    string fileName = GenerateName(subiektPics[i].Symbol, subiektPics[i].Main, subiektPics[i].ID,subiektPics[i].Name, configuration);
                    if (sqlPics.Any(p => p.CRC == subiektPics[i].CRC && p.ID == subiektPics[i].ID && p.Main == subiektPics[i].Main && p.Symbol == subiektPics[i].Symbol && p.Name == subiektPics[i].Name)) // sprawdzanie czy istnieje taki wpis w sql lite
                    {
                        //wpis istnieje nic nie robimy, usuwamy z kolekcji sqllite
                        sqlPics.RemoveAll(p => p.ID == subiektPics[i].ID); 
                      //upewniam się czy plik zdjęcia na pewno istnieje, jeśli nie to je pobieram
                        if (!files.Any(p => p.Contains(fileName)))
                        {
                            try
                            {
                        //    log.Error("Nie znalezion obrazka" +subiektPics[i].ID.ToStringDoSerializacji());
                            CreatePic(subConn, fileName, path, subiektPics[i].ID);
                            }
                            catch (Exception ex) { log.Error("Pobieranie zdjęcia z subiekta " + fileName +" "+ ex.Message + " " +ex.StackTrace); }    
                        }
                        files.RemoveAll(p => p.Contains(fileName));
                    }
                    else
                    {

                        try
                        {
                            //wpis nie istnieje, dodajemy nowy do sql lite, pobieramy bajty z suba, usuwamy plik zdjecia i tworzymy nowy  ///id,crc,glowny,symbol
                            CreatePic(subConn, fileName, path, subiektPics[i].ID);
                            //usuwamy zdjecie o tym id z bazy sqllite i kolekcji pobranej z sql lite
                            cmd = new SQLiteCommand(string.Format(DELETE_SQLLITE, subiektPics[i].ID.ToStringDoSerializacji()), conn);
                            cmd.ExecuteNonQuery();
                            sqlPics.RemoveAll(p => p.ID == subiektPics[i].ID);
                        //    log.Error("Usuwanie " + subiektPics[i].ID.ToStringDoSerializacji() + " sqlPics" + sqlPics.Where(p=>p.ID==subiektPics[i].ID).Count().ToString() );
                        //    log.Error("Dodawanie " + string.Format(INSERT_SQLLITE, subiektPics[i].ID.ToStringDoSerializacji(), subiektPics[i].CRC.ToStringDoSerializacji(),
                           //     subiektPics[i].Main ? 1 : 0.ToStringDoSerializacji(), subiektPics[i].symbol.ToStringDoSerializacji(), subiektPics[i].nazwa.ToStringDoSerializacji()));
                          
                            cmd = new SQLiteCommand(string.Format(INSERT_SQLLITE, subiektPics[i].ID.ToStringDoSerializacji(), subiektPics[i].CRC.ToStringDoSerializacji(), (subiektPics[i].Main ? 1 : 0).ToStringDoSerializacji(), 
                                subiektPics[i].Symbol.ToStringDoSerializacji(), subiektPics[i].Name.ToStringDoSerializacji()), conn);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex) { log.Error("Pobieranie zdjęcia z subiekta " + fileName + " " + ex.Message + " " + ex.StackTrace); }    
                    }
                }
                for (int i = 0; i < sqlPics.Count; i++) //usuwamy wpisy ktore pozostaly w sql lite, skoro zostaly to nie znaleziono zdjec dla nich
                {
                //    log.Error("Usuwanie " + sqlPics[i].ID.ToStringDoSerializacji());
                    cmd = new SQLiteCommand(string.Format(DELETE_SQLLITE, sqlPics[i].ID.ToStringDoSerializacji()), conn);
                    cmd.ExecuteNonQuery();
                }
                for (int i = 0; i < files.Count; i++) //usuwamy pozostale plik
                {
                    if (File.Exists(files[i]))
                    {
                        File.Delete(files[i]);
                    }
                }
            }
            catch (Exception ex) { log.Error(ex.Message + " " + ex.StackTrace); }
            finally
            {
                if (conn != null) conn.Close();
                if (subConn != null) subConn.Close();
                if (cmd != null) cmd.Dispose();
                if (subcmd != null) subcmd.Dispose();
            }
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Zakończenie pobierania zdjęć z subiekta do katalogu"));
            }
        }
        public void CreatePic(SqlConnection subConn,string fileName,string path,int id)
        {
          SqlCommand subcmd = new SqlCommand(string.Format(SELECT_SUBIEKT_BYTES, id.ToStringDoSerializacji()),subConn);
                        byte[] bytes = (byte[])subcmd.ExecuteScalar();
                      
                        ImgInfo info = new ImgInfo();
                        info.Path = path;
                        info.Orgingal = fileName;
                            info = Images.SaveImage(bytes, info, new WatermarkPars() { Enabled = false }, true);
            fileName=info.Orgingal;
            //Konwersja tiff
                            if (Path.GetExtension(fileName).ToLower()==".tiff")
                            {
                                string jpgPAth = path + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".jpg";
                                string orginalPAth = path + "\\" + fileName;
                                if(File.Exists(jpgPAth))
                                {
                                    File.Delete(jpgPAth);
                                }
                                Image i = ImageHelper.ConvertToJpeg(orginalPAth);
                                i.Save(jpgPAth);
                            }
        }
        public string GenerateName(string symbol, bool isMain,int id,string name, System.Collections.Specialized.NameValueCollection configuration)
        {
            string char_replace = configuration["import_illegal_char_replace"];
            if (char_replace == null) char_replace = "_";
            foreach (var x in IllegalChars)
            {
                symbol = symbol.Replace(x, char_replace);
            }
            foreach (var x in IllegalChars)
            {
                name = name.Replace(x, char_replace);
            }
            if (char_replace == "_") throw new Exception("Separator jest taki sam jak znak zamieniający niedopuszczalne znaki");
            return TextHelper.ReplacePolishChars( string.Format("{3}#{0}#{1}_{2}", symbol, isMain ? "0" : "1",id,name));
        }
        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
