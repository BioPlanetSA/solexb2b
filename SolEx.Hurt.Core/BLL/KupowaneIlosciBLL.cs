using log4net;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public class KupowaneIlosciBLL: LogikaBiznesBaza
    {
        public List<KupowaneIlosci> ZnajdzPasujacy(long klient, HashSet<long> produkt, HashSet<ZCzegoLiczycGradacje> rodzaj, DateTime odKiedy)
        {
            string zapytanie = string.Format("SELECT * FROM vKupowaneIlosci where KlientId ={0} and RodzajDokumentu in ('{1}') and DataZakupu>='{2}' and ProduktId in ({3})", klient, rodzaj.Join("','"), odKiedy.ToString("yyyy-MM-dd"), produkt.ToCsv() );
            List<KupowaneIlosci>kupowaneIlosci = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.SqlList<KupowaneIlosci>(zapytanie);
            return kupowaneIlosci;
        }

        protected object lok = new object();

        public virtual decimal SumaKupowanychIlosci(long klient, HashSet<long> produkt, HashSet<ZCzegoLiczycGradacje> rodzaj, DateTime odKiedy)
        {
           return SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<decimal>(() =>
            {
                string zapytanie = string.Format("SELECT ISnull(SUM(ilosc),0) FROM vKupowaneIlosci where KlientId={0} and RodzajDokumentu in ('{1}') and DataZakupu>='{2}' and ProduktId in ({3})", klient, rodzaj.Join("','"),
                    odKiedy.ToString("yyyy-MM-dd"), produkt.ToCsv());
                decimal kupowaneIlosci = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Scalar<decimal>(zapytanie);
                return kupowaneIlosci;
            }, lok, string.Format("{0}-{1}-{2}-{3}", klient, produkt.OrderBy(x => x).ToCsv(), rodzaj.ToCsv(), odKiedy.ToString("yyyy-MM-dd")));
        }

        /// <summary>
        /// Lączna kupiona ilość
        /// </summary>
        /// <param name="produktyIds"></param>
        /// <param name="klient"></param>
        /// <param name="typDokumentu">Na podstawie jakiego rodzaju dokumentów liczyć</param>
        /// <param name="odKiedy">Od jakiej daty uwzglądanimy dokumenty</param>
        /// <param name="doKiedy">Do jakiej daty uwzględniamy dokumenty</param>
        /// <returns></returns>
        public decimal PobierzKupowanaIlosc(HashSet<long> produktyIds, IKlient klient, HashSet<ZCzegoLiczycGradacje> typDokumentu, DateTime odKiedy)
        {
            var produktyDostepneDlaKlienta =  SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(klient).Intersect(produktyIds);
            if (!produktyDostepneDlaKlienta.Any())
            {
                return 0;
            }
            List<KupowaneIlosci> kl = ZnajdzPasujacy(klient.Id, produktyIds, typDokumentu, odKiedy);
            decimal ilosc = kl.Sum(x => x.Ilosc);
            return ilosc;
        }

        public KupowaneIlosciBLL(ISolexBllCalosc calosc) : base(calosc)
        {}
    }
}