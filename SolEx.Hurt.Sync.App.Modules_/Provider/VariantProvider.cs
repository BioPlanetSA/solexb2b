using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Sync.App.Modules_;
using System.Collections.Specialized;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.Core;
using System.Data.SqlClient;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class VariantProvider : IImportDataModule
    {
        public void DoWork(NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
                ProgresChanged(this, new ProgressChangedEventArgs("Przekształcanie danych: Variant"));

            // opisy produktów
            foreach (var v in db.Products.Where(p => !string.IsNullOrEmpty(p.opis)))
            {
                int ind = v.opis.IndexOf("*!");
                while (ind > -1)
                {
                    int ind2 = v.opis.IndexOf("!*");
                    if (ind2 > ind)
                    {
                        if (v.opis.Substring(ind, ind2 - ind + 2).Contains("zamowienie"))
                            v.pole_liczba5 = 1;
                        v.opis = v.opis.Remove(ind, ind2 - ind + 2);
                        ind = v.opis.IndexOf("*!");
                    }
                    else
                        ind = -1;
                }
            }

            // kategorie produktów
            foreach (var v in db.SourceCategories)
            {
                if (v.nazwa.IndexOf(".") == 2)
                {
                    int order = 0;
                    if (Int32.TryParse(v.nazwa.Substring(0, 2), out order))
                    {
                        v.Lp = order - 100;
                        v.nazwa = v.nazwa.Remove(0, 3).Trim();
                    }
                }
            }

            // parametry partnerskie klienta (obrót, bonus itd.)
            //AddCustomerParameters(db.Customers, configuration);

            AddCurrencyRates(db.Products, configuration);

            if (this.ProgresChanged != null)
                ProgresChanged(this, new ProgressChangedEventArgs("Przekształcanie danych: Variant - koniec"));
        }

        //private void AddCustomerParameters(List<Customer> list, NameValueCollection Config)
        //{
        //    SqlDataReader r = null;
        //    SqlCommand cmd = null;
        //    SqlConnection conn = null;

        //    string cs = String.IsNullOrEmpty( Config["erp_cs2"]) ? "Data Source=CDN\\CDN; Database=Dodatki; User id=XLr; haslo_docelowe=" : Config["erp_cs2"];
        //    string pars = String.IsNullOrEmpty( Config["customer_params1"]) ? " MinProc, MaxProc, MinB, MaxB, Brak, BONUS, KwP, PotrzKw, Wzr " : Config["customer_params1"];
        //    decimal item = 0;
        //    try
        //    {
        //        conn = new SqlConnection(cs);
        //        conn.Open();

        //        cmd = new SqlCommand("select " + pars + ", kntnumer, calyp, poprz, biez from bonus", conn);
        //        r = cmd.ExecuteReader();

        //        while (r.Read())
        //        {
        //            var v = list.FirstOrDefault(p => p.klient_id == (int)r["kntnumer"]);
        //            if (v == null)
        //                continue;

        //            for (int i = 0; i < pars.Split(',').Length; ++i)
        //                v.pole_tekst1 += dbi(i, r) + "^";
        //            v.pole_tekst1 = v.pole_tekst1.Trim('^');
        //            v.TurnOverCurrent = dbd("biez", r);
        //            v.TurnOverLastCurrent = dbd("poprz", r);
        //            v.TurnOverLastFull = dbd("calyp", r);
        //        }
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //            cmd.Dispose();
        //        if (conn != null)
        //        {
        //            conn.Close();
        //            conn.Dispose();
        //        }
        //    }
        //}

        private void AddCurrencyRates(List<produkty> list, NameValueCollection Config)
        {
            SqlDataReader r = null;
            SqlCommand cmd = null;
            SqlConnection conn = null;

            string cs = Config["erp_cs"] == null ? "Data Source=CDN\\CDN; Database=Variant; User id=XLrw; haslo_docelowe=xlrw" : Config["erp_cs"];
            decimal item = 0;
            try
            {
                conn = new SqlConnection(cs);
                conn.Open();

                cmd = new SqlCommand("select top 1 kurs = wae_kursl / wae_kursm from cdn.walelem where wae_symbol = 'EUR' order by wae_kursts desc", conn);
                r = cmd.ExecuteReader();

                while (r.Read())
                {
                    item = dbd("kurs", r);
                }

                foreach (var v in list.Where(p => p.kod.StartsWith("FS") || p.kod.StartsWith("ZFS")))
                {
                    v.pole_liczba3 = item;
                }
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        private string dbi(int i, SqlDataReader r)
        {
            object o = r[i];
            return o == DBNull.Value || o == null ? "" : o.ToString();
        }
        private string dbs(string p, SqlDataReader r)
        {
            return r[p] == DBNull.Value ? null : (string)r[p];
        }
        private int dbi(string p, SqlDataReader r)
        {
            return r[p] == DBNull.Value ? 0 : Int32.Parse(r[p].ToString());
        }
        private decimal dbd(string p, SqlDataReader r)
        {
            return dbd(p, r, 2);
        }
        private decimal dbd(string p, SqlDataReader r, int decimals)
        {
            return r[p] == DBNull.Value ? 0 : Decimal.Round(decimal.Parse(r[p].ToString()), decimals);
        }



        public event ProgressChangedEventHandler ProgresChanged;
    }
}
