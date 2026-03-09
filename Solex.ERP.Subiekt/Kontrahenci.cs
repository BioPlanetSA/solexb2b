using ServiceStack.OrmLite;
using SolEx.DBHelper;
namespace SolEx.ERP.SubiektGT
{
    public static class Kontrahenci
    {
        public static int GetIDCecha(string cecha)
        {
            const string SQL_SELECT = "SELECT ckh_Id from sl_cechakh where ckh_Nazwa={0}";
            const string SQL_INSERT = "declare @id int;select @id=ISNULL(max(ckh_Id)+1,1) from sl_cechakh;insert into sl_cechakh(ckh_Id,ckh_Nazwa) values(@id,'{0}');update ins_ident set ido_wartosc=@id where  ido_nazwa='sl_CechaKh';";
            int? idCecha = Baza.db.Scalar<int?>( SQL_SELECT,cecha);
            if (idCecha == null)
            {
                Baza.db.ExecuteSql(string.Format(SQL_INSERT, cecha));
                idCecha = Baza.db.Scalar<int>(SQL_SELECT, cecha);
            }
            return (int)idCecha;
        }

        public static void DodajCeche(int klientId, int cechaId)
        {
            const string SQL_SELECT = "SELECT count(*) from kh_CechaKh where ck_IdCecha={0} and ck_IdKhnt={1}";
            const string SQL_INSERT = "declare @id int;select @id=ISNULL(max(ck_Id)+1,1) from kh_CechaKh;insert into kh_CechaKh(ck_Id,ck_IdCecha,ck_IdKhnt) values(@id,{0},{1});update ins_ident set ido_wartosc=@id where  ido_nazwa='kh_CechaKh';";
            int count = Baza.db.Scalar<int>(SQL_SELECT, cechaId, klientId);
            if (count==0)
            {
                Baza.db.ExecuteSql(string.Format( SQL_INSERT, cechaId, klientId) );
            }
        }
        /// <summary>
        /// Sprawdza czy taki nip jest już użyty
        /// </summary>
        /// <param name="nip">Sprawdzany nip</param>
        /// <returns></returns>
        public static bool IstniejeNIP(string nip)
        {
            const string SQL_SELECT = @"SELECT count(*) FROM kh__Kontrahent kh LEFT JOIN adr__Ewid adr ON adr.adr_IdObiektu = kh.kh_Id WHERE ltrim(rtrim(replace(replace(adr.adr_NIP,'-',''),' ',''))) =  ltrim(rtrim(replace(replace('{0}','-',''),' ','')))";
            int count = Baza.db.Scalar<int>( string.Format(SQL_SELECT,nip));
            return count > 0;
        }
        /// <summary>
        /// Sprawdza czy taki mail jest już użyty
        /// </summary>
        /// <param name="email">Sprawdzany mail</param>
        /// <returns></returns>
        public static bool IstniejeMail(string email)
        {
            const string SQL_SELECT = @"SELECT count(*) FROM kh__Kontrahent kh  WHERE ltrim(rtrim(kh_email)) = ltrim(rtrim('{0}'))";
            int count = Baza.db.Scalar<int>( string.Format(SQL_SELECT, email) );
            return count > 0;
        }
        /// <summary>
        /// Sprawdza czy taki symbol klienta jest już użyty
        /// </summary>
        /// <param name="symbol">Sprawdzany symbol</param>
        /// <returns></returns>
        public static bool IstniejeSymbol(string symbol)
        {
            const string SQL_SELECT = @"SELECT count(*) FROM kh__Kontrahent kh  WHERE ltrim(rtrim(kh_symbol)) = ltrim(rtrim('{0}'))";
            int count = Baza.db.Scalar<int>(string.Format(SQL_SELECT, symbol) );
            return count > 0;
        }
        /// <summary>
        /// Zwraca id klient o wybranym nipie
        /// </summary>
        /// <param name="nip"></param>
        /// <returns></returns>
        public static int? PobierzID(string nip)
        {
            const string SQL_SELECT = @"SELECT kh_id FROM kh__Kontrahent kh LEFT JOIN adr__Ewid adr ON adr.adr_IdObiektu = kh.kh_Id WHERE ltrim(rtrim(replace(replace(adr.adr_NIP,'-',''),' ',''))) =  ltrim(rtrim(replace(replace('{0}','-',''),' ','')))";
            int? count = Baza.db.Scalar<int>( string.Format(SQL_SELECT,  nip));
            return count;
        }
        /// <summary>
        /// Pobiera id po symbolu
        /// </summary>
        /// <param name="symbol">Sprawdzany symbol</param>
        /// <returns></returns>
        public static int PobierzIDPoSymbolu(string symbol)
        {
            const string SQL_SELECT = @"SELECT kh_id FROM kh__Kontrahent kh  WHERE ltrim(rtrim(kh_symbol)) = ltrim(rtrim('{0}'))";
            int count = Baza.db.Scalar<int>(string.Format(SQL_SELECT, symbol));
            return count;
        }
    }
}
