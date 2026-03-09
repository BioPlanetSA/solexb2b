using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.DAL
{
    public class Config 
    {

        static Config(){}
        protected Config(){}

        private static readonly Config _config = new Config();

        public static Config PobierzInstancje 
        {
            get { return _config; }
        }

        //public  DateTime PobierzDateAktualizacjiBazy()
        //{
        //    return MainDAO.GetObjects<DateTime>("SELECT max(modify_date)FROM sys.tables ").FirstOrDefault();
        //}

        //public  DateTime PobierzDateAktualizacjiPlikow()
        //{
        //    string katalog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"bin");
        //    DateTime max = DateTime.MinValue;
        //    foreach (var assembly in Directory.GetFiles(katalog))
        //    {
        //        var data = File.GetLastWriteTime(assembly);
        //        if (data > max)
        //        {
        //            max = data;
        //        }
        //    }
        //    return max;
        //}

        //public  Dictionary<int, string> ListRol
        //{
        //    get
        //    {
        //        Dictionary<int, string> role = new Dictionary<int, string>();
        //        foreach (RoleType foo in Enum.GetValues(typeof(RoleType)))
        //        {
        //            role.Add((int)foo, foo.ToString());
        //        }
        //        return role;
        //    }
        //}
        public  string MainCS //przeniesiono do konfiguracji
        {
            get
            {
                ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings["MainConnectionString"];
                if (conn == null)
                    return "";
                return conn.ConnectionString;
            }
        }
    }
}
