using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.DBHelper
{
    public static class Baza
    {
       
        private static OrmLiteConnectionFactory _dbFactory ;
        private static string _connectionString;
        public static string ConnectionString
        {
            set
            {
                _connectionString = value;
                OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            }
        }

        public static OrmLiteConnectionFactory DbFactory
        {
            get
            {
                if (_dbFactory == null)
                {
                    if (_connectionString == null)
                    {
                        throw new Exception("ustaw connection string najpierw !!");
                    }
                    _dbFactory = new OrmLiteConnectionFactory(_connectionString);
                }
                return _dbFactory;
            }
        }

        public static void Zresetuj(string cs)
        {
            _connectionString = cs;
            _dbFactory = null;
            _conn = null;
        }
        private static IDbConnection _conn;
        public static IDbConnection db
        {
            get
            {
                if (_conn == null)
                {
                    _conn = DbFactory.OpenDbConnection();
                }
                return _conn;
            }
        }

        public static void SprawdzPolaczenie()
        {
            db.TableExists("tabela");
        }
    }
}
