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
    public static class Magazyny
    {
        public static int PobierzIdMagazynu(string symbolMagazynu)
        {
            string zapytanie = "select top 1 mag_Id from sl_Magazyn where  RTRIM(LTRIM(Mag_Symbol)) = '{0}'";
            object result = Baza.db.Scalar<int?> (string.Format( zapytanie, symbolMagazynu) );
            if (result == null)
            {
                throw new Exception("Brak magazynu o symbolu " + symbolMagazynu);
            }
            return (int)result;
        }

        //public static int PobierzIdMagazynuGlownego()
        //{
        //    string zapytanie = "select top 1 mag_Id from sl_Magazyn where mag_Glowny = 1";
        //    object result = Baza.db.Scalar<int>(zapytanie);
        //    if (result == null)
        //    {
        //        throw new Exception("Bład pobierania magazynu głównego");
        //    }
        //    return (int)result;
        //}

        //public static Dictionary<long, decimal> PobierzStanyProduktow(int idMagazynu, bool minusRezerwacje)
        //{
        //    string stan = " st_Stan " +  (minusRezerwacje ? "- st_StanRez  " : "");
        //    string sql = $"select st_TowId, {stan} from tw_Stan where st_MagId = {idMagazynu}";
        //    var test = Baza.db.Dictionary<int, decimal>(sql);
        //    return test.ToDictionary(x => Convert.ToInt64(x.Key), x => x.Value);
        //}

        public static Dictionary<long, decimal> PobierzStanyProduktowZPolaWlasnego(string nazwaPola)
        {
            List<PoleWlasneWartosc> temp = PolaWlasne.PobierzPoleDlaWszystkichObiektow(TypObiektu.Towar, nazwaPola).ToList();
            return temp.ToDictionary( a => (long)a.IdObiektu, a => a.WartoscDecimal);
        }


        public static decimal PobierzStanProduktu(int idTowaru, int idMagazynu, bool minusRezerwacje)
        {
            string zapytanie = "select st_Stan " +  (minusRezerwacje ? "- st_StanRez  " : "")  + " from tw_Stan where st_MagId = {0} AND st_TowId = {1}";
            object result = Baza.db.Scalar<decimal>( string.Format(zapytanie, idMagazynu, idTowaru));
            if (result == null)
            {
                throw new Exception("Brak towaru o id= " + idTowaru + " lub magazynu o id= " + idMagazynu);
            }
            return (decimal)result;
        }

        
    }
}
