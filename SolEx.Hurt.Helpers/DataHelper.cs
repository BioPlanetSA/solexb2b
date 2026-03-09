using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Helpers
{
    /// <summary>
    /// Klasa pomocnicza do operacji na bazie
    /// </summary>
    public static class DataHelper
    {
        /// <summary>
        /// Zwraca string
        /// </summary>
        /// <param name="p">Nazwa kolumny</param>
        /// <param name="r">Data reader</param>
        /// <returns></returns>
        public static string dbs(string p, DbDataReader r)
        {
            return dbs(p, r, "");
        }
        /// <summary>
        /// Zwraca string, lub domyœln¹ wartoœæ jeœli odczytana to null
        /// </summary>
        /// <param name="p">Nazwa kolumny</param>
        /// <param name="r">Data reader</param>
        /// <param name="default_Value">Domyœlna wartoœæ</param>
        /// <returns></returns>
        public static string dbs(string p, DbDataReader r, string defaultValue)
        {
            
            return r[p] == DBNull.Value ? defaultValue : (string)r[p];
        }
        /// <summary>
        /// Zwaraca int
        /// </summary>
        /// <param name="p">Nazwa kolumny</param>
        /// <param name="r">Data reader</param>
        /// <returns></returns>
        public static int dbi(string p, DbDataReader r)
        {
            return r[p] == DBNull.Value ? 0 : Int32.Parse(r[p].ToString());
        }
        public static long dbl(string p, SqlDataReader r)
        {
            return r[p] == DBNull.Value ? 0 :long.Parse(r[p].ToString());
        }
        /// <summary>
        /// Zwraca decimal z dok³adnoœci¹ do trzech miejsc
        /// </summary>
        /// <param name="p">Nazwa kolumny</param>
        /// <param name="r">Data reader</param>
        /// <returns></returns>
        public static decimal dbd(string p, DbDataReader r)
        {
            return dbd(p, r, 4);
        }
        public static decimal? dbdn(string p, DbDataReader r)
        {
              return r[p] == DBNull.Value ? (decimal?)null: dbd(p, r, 3);
        }

        public static decimal dbd(string p, DbDataReader r, int decimals)
        {

            if (r[p] == DBNull.Value)
            {
                return 0;
            }
            decimal tmp;
            TextHelper.PobierzInstancje.SprobojSparsowac(r[p].ToString(),out tmp);
            return decimal.Round(tmp,decimals);
        }
  
        /// <summary>
        /// Zwarca decimal?
        /// </summary>
        /// <param name="p">Nazwa kolumny</param>
        /// <param name="r">Data reader</param>
        /// <param name="decimals">Dok³adnoœæ</param>
        /// <returns></returns>
        public static decimal? dbdn(string p, DbDataReader r, int decimals)
        {
            if (r[p] == DBNull.Value)
            {
                return null;
            }
            return dbd(p, r, decimals);
        }
        /// <summary>
        /// Zwaraca dateTime?
        /// </summary>
        /// <param name="p">Nazwa kolumny</param>
        /// <param name="r">Data reader</param>
        /// <returns></returns>
        public static DateTime? dbdtn(string p, DbDataReader r)
        {
            return r[p] == DBNull.Value ? (DateTime?)null : DateTime.Parse(r[p].ToString());
        }
        /// <summary>
        /// Zwaraca dateTime, jeœli wartoœæ null to zwraca wartoœæ minimaln¹
        /// </summary> <param name="p">Nazwa kolumny</param>
        /// <param name="r">Data reader</param>
        /// <returns></returns>
        public static DateTime dbdt(string p, DbDataReader r)
        {
            return dbdt(p, r, DateTime.MinValue);
        }
        /// <summary>
        /// Zwaraca dateTime, jeœli wartoœæ null to zwraca wartoœæ domyœln¹
        /// </summary>
        /// <param name="p">Nazwa kolumny</param>
        /// <param name="r">Data reader</param>
        /// <param name="def">Wartoœæ domyœlna</param>
        /// <returns></returns>
        public static DateTime dbdt(string p, DbDataReader r,DateTime def)
        {
            return r[p] == DBNull.Value ? def : DateTime.Parse(r[p].ToString());
        }
        public static int? dbin(string p, DbDataReader r)
        {

            return r[p] == DBNull.Value ? (int?)null : Int32.Parse(r[p].ToString());
        }

        public static bool? dbbn(string p, DbDataReader r)
        {
            return r[p] == DBNull.Value ? (bool?)null :(bool)r[p];
        }

        public static bool dbb(string p, DbDataReader r)
        {
            return r[p] == DBNull.Value ? false : (bool)r[p];
        }
        /// <summary>
        /// Czysci wybrana tabele w bazie
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static void ClearTable(SqlConnection conn, string tableName,string where)
        {
            SqlCommand cmd = new SqlCommand(string.Format(
                "WHILE EXISTS ( SELECT * FROM {0}  {1} ) BEGIN BEGIN TRAN; DELETE TOP(20000) FROM {0}  {1} ; COMMIT TRAN; END", tableName,where), conn
                
                );
            cmd.CommandTimeout = 100000;
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SqlCommand(string.Format("Delete from {0} {1} ", tableName, where), conn);
            cmd.ExecuteNonQuery();
        }
      
        public static bool CzyKolumnaIstnieje(string tabela, string kolumna, string connectionstring)
        {
            SqlConnection conn = new SqlConnection(connectionstring); 
            SqlCommand cmd =
                    new SqlCommand(
                        string.Format(
                            "select 1 from sysobjects so inner join syscolumns sc on sc.id=so.id where so.name = '{0}' and sc.name = '{1}'",
                            tabela, kolumna), conn);
            SqlDataReader rd = null;
            
                conn.Open();
                
                rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                return true;
            }
            return false;
        }

        public static bool CzyIstniejeTabela(string nazwaTabeli, string connectionString)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(string.Format("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE table_name = '{0}'", nazwaTabeli),conn);
            SqlDataReader rd = null;
            conn.Open();
            rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                return true;
            }
            return false;
        }

       
    }
}
