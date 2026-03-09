using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.OrmLite;
using SolEx.DBHelper;
using log4net;

namespace SolEx.ERP.SubiektGT
{
    public static class Grupy
    {
        private static  ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        public static int? DodajNowaGrupe(string nazwaGrupy)
        {
            const string SQL_INSERT = "declare @id int;select @id=ISNULL(max(grt_Id)+1,1) from sl_GrupaTw;insert into sl_GrupaTw(grt_Id,grt_Nazwa,grt_NrAnalityka) values(@id,'{0}','{1}');update ins_ident set ido_wartosc=(@id+1) where  ido_nazwa='sl_GrupaTw';";
            try
            {
                Baza.db.ExecuteSql(string.Format(SQL_INSERT, nazwaGrupy, string.Empty));
                return PobierzIDGrupy(nazwaGrupy);
            }
            catch (Exception ex)
            {
               Log.Error(ex);
                return null;
            }
        }

        public static int? PobierzIDGrupy(string grupa)
        {
            const string SQL_SELECT = "SELECT grt_Id from sl_GrupaTw where grt_Nazwa={0}";
            int? idGrupy = Baza.db.Scalar<int?>(SQL_SELECT, grupa);
            return idGrupy;
        }
    }
}
