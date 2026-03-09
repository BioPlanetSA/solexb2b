using System.Web;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.DAL
{
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Data;
    using Model.CustomSearchCriteria;
    using ServiceStack.OrmLite;

    public static class MainDAO
    {
        public static string SqliteMemoryDb = ":memory:";
        
        private static OrmLiteConnectionFactory _dbFactory = null;

        public static OrmLiteConnectionFactory DbFactory
        {
            get
            {
                if (_dbFactory == null)
                {
                        _dbFactory = new OrmLiteConnectionFactory(Config.PobierzInstancje.MainCS,
                            SqlServerDialect.Provider);
                        _dbFactory.DialectProvider.UseUnicode = true;
                    

                }
                return _dbFactory;
            }
        }

        private static IDbConnection _polaczenie;

        public static IDbConnection db
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Items["dbpolaczenie"] == null)
                    {
                        HttpContext.Current.Items["dbpolaczenie"] = DbFactory.OpenDbConnection();
                    }
                    return (IDbConnection) HttpContext.Current.Items["dbpolaczenie"];
                }

                if (_polaczenie == null)
                {
                    _polaczenie = DbFactory.OpenDbConnection();

                }
                return _polaczenie;
            }
        }


       
    }
}

