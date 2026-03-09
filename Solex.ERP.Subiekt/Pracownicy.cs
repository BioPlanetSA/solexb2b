using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.ERP.Model;
using System.Data.SqlClient;
using SolEx.DBHelper;
using System.Data;
using ServiceStack.OrmLite;

namespace SolEx.ERP.SubiektGT
{
    public class Pracownicy
    {
        public static int PobierzIdPracownika(string symbol)
        {
            int result = 0;
            using (SqlConnection conn = new SqlConnection(Polaczenie.ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand("select top 1 uz_Id from pd_Uzytkownik where uz_Identyfikator = '" + symbol + "'"))
                {
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader["uz_Id"] == DBNull.Value ? 0 : (int)reader["uz_Id"];

                        }
                    }
                }

                conn.Close();
            }

            if (result == 0)
            {
                throw new Exception("Brak uzytkownika o symbolu " + symbol);
            }
            return result;

            //string zapytanie = "select top 1 uz_Id from pd_Uzytkownik where uz_Identyfikator = '"+symbol+"'";
            //object result = Baza.db.Scalar<int>(Polaczenie.ConnectionString, zapytanie);
            //if (result == null)
            //{
            //    throw new Exception("Brak uzytkownika o symbolu " + symbol);
            //}
            //return (int)result;
        }
    }
}
