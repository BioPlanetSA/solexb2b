using System.Collections.Generic;
using System.Data.SqlClient;
using SolEx.Hurt.Helpers;

namespace SolEx.Hurt.DAL
{
    public class InformacjeBaza
    {
        public static Dictionary<string, int> TabelaOrazJejRozmiar()
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rd = null;
            Dictionary<string, int> wynik = new Dictionary<string, int>();
            try
            {

                conn = new SqlConnection(Config.PobierzInstancje.MainCS);
                string zapytanie = string.Format("SELECT o.NAME, i.rowcnt FROM sysindexes AS i  INNER JOIN sysobjects AS o ON i.id = o.id WHERE i.indid < 2  AND OBJECTPROPERTY(o.id, 'IsMSShipped') = 0 ORDER BY o.NAME");
                conn.Open();
                cmd = new SqlCommand(zapytanie, conn);
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    int iloscRekordow = DataHelper.dbi("rowcnt", rd);
                    string nazwaTabeli = DataHelper.dbs("NAME", rd);
                    wynik.Add(nazwaTabeli,iloscRekordow);
                }
            }
            finally
            {
                if (rd != null) { rd.Close(); rd.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            return wynik;
        } 
    }
}
