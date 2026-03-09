using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;
using System.Data.SqlClient;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    class BioplanetStanyCykliczneProvider: IImportDataModule
    {
        #region IImportDataModule Members
    
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
          
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Początek cyklicznych dostaw"));
            }
            string SQL = @"SELECT distinct symbol=tw_symbol,id=v.tw_id,rodzaj=tw_rodzaj,min=isnull(tw_StanMin,0),Stan1=10000,Stan2=10000,Stan3=10000,Stan4=10000,Stan5=10000,grupa=tw_IdGrupa,dostawa=tw_CzasDostawy
 
from vwTowar v where isnull(tw_Pole3,'')<>''";
            SqlConnection conn = null;
            System.Data.SqlClient.SqlCommand cmd = null;
            SqlDataReader rd = null;
            try
            {
                conn = new SqlConnection(configuration["erp_cs"]);
                conn.Open();

                cmd = new System.Data.SqlClient.SqlCommand(SQL, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int id = SolEx.Hurt.Helpers.DataHelper.dbi("id", rd);// q[qidx].id;
                    flat_stany item = db.LiteProducts.FirstOrDefault(p => p.produkt_id == id);
                   if (item != null)
                   {
                       item.stan = 10000;
                       item.stan2 = 10000;
                       item.stan3 = 10000;
                       item.stan5 = 10000;
                       item.stan4 = 10000;
                   }
                }

            }
            finally
            {
                if (conn != null) conn.Close();
                if (rd != null) rd.Dispose();
                if (cmd != null) cmd.Dispose();
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
