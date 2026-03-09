using System.Data;
using System.Web;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.DostepDane
{
    public class BazaDostepDane
    {
        private OrmLiteConnectionFactory _dbFactory;

        public virtual OrmLiteConnectionFactory DbFactory
        {
            get
            {
                if (_dbFactory == null)
                {
                    _dbFactory = new OrmLiteConnectionFactory(SolexBllCalosc.PobierzInstancje.Konfiguracja.MainCS, SqlServerDialect.Provider) { DialectProvider = { UseUnicode = true } };
                    //OrmLiteConfig.OrmLiteConfig.StripUpperInLike
                }
                return _dbFactory;
            }
            set { _dbFactory = value; }
        }

        protected IDbConnection Db
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Items["dbpolaczenie"] == null)
                    {
                        HttpContext.Current.Items["dbpolaczenie"] = DbFactory.OpenDbConnection();
                    }

                    return (IDbConnection)HttpContext.Current.Items["dbpolaczenie"];
                }
                else
                {
                    return DbFactory.OpenDbConnection();
                }
            }

        }

        /// <summary>
        /// dupiata imlemetancja na szybko do zamykania polaczenia w webowce
        /// </summary>
        public void ZamknijPolaczenieDoBazy()
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            var db = ((IDbConnection)HttpContext.Current.Items["dbpolaczenie"]);

            if ( db != null)
            {
                db.Dispose();
                HttpContext.Current.Items["dbpolaczenie"] = null;
            }

            //nie mozna robić .CLOSE bo korzystamy z connectionPuli - i wtedy Close tylko zamyka transakcje a nie zamyka polaczenia realnie
        }

    }

}
