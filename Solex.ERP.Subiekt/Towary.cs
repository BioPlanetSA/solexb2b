using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Core;
using SolEx.ERP.Model;
using System.Data.SqlClient;
using SolEx.DBHelper;
using ServiceStack.OrmLite;
using System.Data;
using log4net;
namespace SolEx.ERP.SubiektGT
{
    public static class Towary
    {
        private static  ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        public static int PobierzIdTowaru(string symbolTowaru)
        {
            string zapytanie = "select top 1 tw_id from tw__Towar where tw_Symbol = @symbolTw";
            int result = Baza.db.Scalar<int>(zapytanie, new SqlParameter("@symbolTw", symbolTowaru));

            return result;
        }
        public static int? GetIDCecha(string cecha)
        {
            const string SQL_SELECT = "SELECT ctw_Id from sl_CechaTw where ctw_Nazwa={0}";
            const string SQL_INSERT = "declare @id int;select @id=ISNULL(max(ctw_Id)+1,1) from sl_CechaTw;insert into sl_CechaTw(ctw_Id,ctw_Nazwa) values(@id,'{0}');update ins_ident set ido_wartosc=@id where  ido_nazwa='sl_CechaTw';";
            int? idCecha = Baza.db.Scalar<int?>(SQL_SELECT, cecha);
            if (idCecha == null)
            {
                Baza.db.ExecuteSql(string.Format(SQL_INSERT, cecha));
                idCecha = Baza.db.Scalar<int>(SQL_SELECT, cecha);
            }
            return idCecha;
        }

        public static int? DodajNowaCeche(string nazwaCechy)
        {
            const string SQL_INSERT = "declare @id int;select @id=ISNULL(max(ctw_Id)+1,1) from sl_CechaTw;insert into sl_CechaTw(ctw_Id,ctw_Nazwa) values(@id,'{0}');update ins_ident set ido_wartosc=(@id+1) where  ido_nazwa='sl_CechaTw';";
            try
            {
                Baza.db.ExecuteSql(string.Format(SQL_INSERT, nazwaCechy, string.Empty));
                return GetIDCecha(nazwaCechy);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
    }
}
