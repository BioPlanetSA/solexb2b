using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Sync.Core;
using SolEx.Hurt.Sync.App.Modules_.DAO;
using SolEx.Hurt.Sync.App.Modules_.Helpers;
using SolEx.Hurt.Model;
using System.Data.SqlClient;
using SolEx.Hurt.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class UkrywanieProduktowProvider : IImportDataModule
    {
        #region IImportDataModule Members

        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            //for (int i = 0; i < configuration.Count; i++)
            //{
            //    log.Error(configuration.Keys[i] + configuration[configuration.Keys[i]]);
            //}
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie ukrytych produktów"));
            }
            if (configuration["ukrywanieproduktow_wszystkie_ukryte"] == "1")
            {
                for (int i = 0; i < db.Products.Count; i++)
                {
                    db.Products[i].dostepny_dla_wszystkich = false;
                 
                }
                for (int i = 0; i < db.Customers.Count; i++)
                {
                    db.Customers[i].pelna_oferta = false;
                }
            }
            string prefix = configuration["ukrywanieproduktow_prefiks"];
            if (string.IsNullOrEmpty(prefix))
                prefix = "%";
            //Nowa wersja przy urzyciu sql
            String SQL =@"select distinct tw_Id,kh_Id from (


select distinct t.tw_Id,t.tw_Nazwa,ctw.ctw_Nazwa from tw__Towar t 
join tw_CechaTw ct on t.tw_Id=ct.cht_IdTowar join sl_CechaTw ctw on ct.cht_IdCecha=ctw.ctw_Id 


where ctw.ctw_Nazwa like '{0}%' ) towary join 

(
select distinct kh_Id,kh_Symbol,sck.ckh_Nazwa from kh__Kontrahent k join kh_CechaKh ck 
on k.kh_Id=ck.ck_IdKhnt join sl_CechaKh sck on ck.ck_IdCecha=sck.ckh_Id where sck.ckh_Nazwa like '{0}%') klienci on towary.ctw_Nazwa=klienci.ckh_Nazwa";
            if (!string.IsNullOrEmpty(configuration["ukrywanieproduktow_katalog"]))
            {
                SQL+=@" union select distinct t.tw_Id,0 from tw__Towar t 
join tw_CechaTw ct on t.tw_Id=ct.cht_IdTowar join sl_CechaTw ctw on ct.cht_IdCecha=ctw.ctw_Id 


where ctw.ctw_Nazwa like '{1}' ";
            }
            SqlConnection conn = null;
            SqlCommand cmd = null;
                SqlDataReader r = null;

            try
            {
                conn = new SqlConnection(configuration["erp_cs"]);
                conn.Open();
                cmd = new SqlCommand(string.Format(SQL, prefix, configuration["ukrywanieproduktow_katalog"]), conn);
                r=cmd.ExecuteReader();
                while(r.Read())
                {
                    data.Add(new KeyValuePair<string,string>(DataHelper.dbi("tw_Id",r).ToString(),DataHelper.dbi("kh_Id",r).ToString()));
                }
            }
            finally
            {
                if(conn!=null){ conn.Close(); conn.Dispose();}
                if(cmd!=null) cmd.Dispose();
                if(r!=null) r.Dispose();
            }
            //for (int i = 0; i < db.Customers.Count; i++)
            //{
            //    List<KeyValuePair<string, string>> datatmp = new List<KeyValuePair<string, string>>();
            //    if (db.Customers[i].Attibutes != null)
            //    {
            //        var ctraits = db.Customers[i].Attibutes.Where(p => p.symbol.StartsWith(prefix));
            //        foreach (var t in ctraits)
            //        {
            //            var patr = db.Attributes.FirstOrDefault(p => p.symbol.ToLower() == t.symbol.ToLower());
            //            if (patr != null)
            //            {
            //                var pr = db.Products.Where(p => p.AttributeIds.Contains(patr.id) || p.AttributeSymbols.Contains(patr.symbol));
            //                foreach (var p in pr)
            //                {
            //                    if (!datatmp.Any(x => x.Key == p.id.ToString() && x.Value == db.Customers[i].id.ToString()))
            //                        datatmp.Add(new KeyValuePair<string, string>(p.id.ToString(), db.Customers[i].id.ToString()));
            //                }
            //            }
            //        }
            //    }
            //    data.AddRange(datatmp);
            //}
            string fileName =  "csv_produkty_ukryte.csv";
            //SyncManager.SaveFile(fileName, CreateCSV(data)
            //, AppDomain.CurrentDomain.BaseDirectory + (configuration["program_mode"] == "-auto" ? "paczka_temp_auto\\" : "paczka_temp\\"));

    
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie ukrytych produktów koniec"));
            }
        }

        private string CreateCSV(List<KeyValuePair<string, string>> data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> d in data)
            {
                sb.Append("|");
                sb.Append(d.Key);
                sb.Append("|");
                sb.Append(d.Value);
                sb.Append("|");
                sb.Append("|");
                sb.Append("-1");
                sb.Append("|");                
                sb.Append(";");

                sb.Append("|");
                sb.Append(d.Key);
                sb.Append("|");
                sb.Append(d.Value);
                sb.Append("|");
                sb.Append("|");
                sb.Append("1");
                sb.Append("|");
                sb.Append(";");
            }

            return sb.ToString();
        }

        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
}
