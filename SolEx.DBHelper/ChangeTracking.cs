using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using ServiceStack.OrmLite;
using System.Data;

namespace SolEx.DBHelper
{

    public static class ChangeTracking
    {
        private static string sqlDoOdczytuZmian = "SELECT {0} FROM CHANGETABLE(CHANGES {1}, {2}) t";

        public class PodwojnyKlucz<T,TT>{
            public T klucz1{get;set;}
            public TT klucz2{get;set;}
        }

        public class PotrujnyKlucz<T, TT, TTT>
        {
            public T klucz1 { get; set; }
            public TT klucz2 { get; set; }
            public TTT klucz3 { get; set; }
        }

        

        public static void WlaczTrackingBazyDanych(string baza, int ileDni = 4)
        {
            //string sqlSprawdzenie = string.Format("select * from sys.change_tracking_databases c left join sys.databases d on c.database_id = d.database_id where d.name = '{0}'", baza);
            string sqlUstaw = string.Format("ALTER DATABASE {0} SET CHANGE_TRACKING = ON (CHANGE_RETENTION = {1} DAYS, AUTO_CLEANUP = ON)", baza, ileDni);
            //if (db.ExecuteSql(sqlSprawdzenie) == 0)
            
            Baza.db.ExecuteSql(sqlUstaw);
            
        }

        public static void WlaczTrackingTabeli(string tabela)
        {
            string sqlUstaw = string.Format("ALTER TABLE tw_stan ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);", tabela);
            Baza.db.ExecuteSql(sqlUstaw);
        }

        public static List<T> PobierzZmianyTabeli<T>(string tabela, string nazwaKlucz, int wersjaBazowa)
        {
            string sql = string.Format(sqlDoOdczytuZmian, nazwaKlucz, tabela, wersjaBazowa);
            return Baza.db.List<T>(sql);
        }

        public static List<PodwojnyKlucz<T, TT>> PobierzZmianyTabeli<T, TT>(string tabela, string nazwaKlucz, string nazwaKlucza2, int wersjaBazowa)
        {
            string sql = string.Format(sqlDoOdczytuZmian, nazwaKlucz + ", " + nazwaKlucza2, tabela, wersjaBazowa);
            return Baza.db.List<PodwojnyKlucz<T, TT>>(sql);
        }

        public static List<PotrujnyKlucz<T, TT, TTT>> PobierzZmianyTabeli<T, TT, TTT>(string tabela, string nazwaKlucz, string nazwaKlucza2, string nazwaKlucza3, int wersjaBazowa)
        {
            string sql = string.Format(sqlDoOdczytuZmian, nazwaKlucz + "," + nazwaKlucza2 + "," + nazwaKlucza3, tabela, wersjaBazowa);
            return Baza.db.List<PotrujnyKlucz<T, TT, TTT>>(sql);
        }


        public static int PobierzAktualnaWersjeZmian()
        {
            string sql = "select ISNULL(CHANGE_TRACKING_CURRENT_VERSION(),0)";
            return Baza.db.Scalar<int>(sql);
        }

        public static int SprawdzJakaWersjaMoznaPobrac(string tabela, int ostatniaPobranaWersja)
        {
            string sql = string.Format("SELECT ISNULL(MIN(SYS_CHANGE_VERSION),0), ISNULL(MAX(SYS_CHANGE_VERSION),0) FROM CHANGETABLE(CHANGES {0}, 0) t", tabela);
            PodwojnyKlucz<int, int> obiekt = Baza.db.First<PodwojnyKlucz<int, int>>(sql);

            //czy wersja minimum jest zgodna z tym co ostanio pobralem
            if (ostatniaPobranaWersja < obiekt.klucz1)
            {
                return 0; //reinicjalizacja wymagana
            }

            if (ostatniaPobranaWersja > obiekt.klucz2)
            {
                throw new Exception("Ostatnio pobrana wersja jest za wysoka. Prawdopodobnie problem z różnicowością");
            }

            //maxa
            return obiekt.klucz2; 
        }
    }
}
