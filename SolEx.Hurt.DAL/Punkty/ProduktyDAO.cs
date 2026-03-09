namespace SolEx.Hurt.Core.Points.DAL
{
    using SolEx.Hurt.Core.Configuration;
    using System;
    using System.Data.Linq;
    using System.Linq;
    using SolEx.Hurt.DAL;

    public class ProduktyDAO
    {
        internal static decimal? PobierzCeneWPunktach(int product)
        {
            decimal? result = (decimal?)null;
            MainDataContext DB = null;
            try
            {
                DB = new MainDataContext(GlobalConfig.MainCS);
                result = DB.produkties.FirstOrDefault<produkty>(p => (p.produkt_id == product)).cena_punkty;
            }
            finally
            {
                if (DB != null)
                    DB.Dispose();
            }
            return result;
        }

        internal static int[] PobierzKategorieProduktu(int p)
        {
            MainDataContext DB = null;
            int[] list = null;
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
               list= DB.ExecuteQuery<int>("select kkz.kategoria_id from kategorie_kategorie_zrodlowe kkz join kategorie_zrodlowe kz on kkz.kategoria_zrodlowa_id=kz.kategoria_id join produkty_kategorie_Zrodlowe pkz on kz.kategoria_id=pkz.kategoria_zrodlowa_id  where pkz.produkt_id="+p.ToString()).ToArray();
            }
            return list;
        }

        internal static string PobierzNazwe(int id)
        {
            MainDataContext DB = null;
            string list = string.Empty;
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                produkty res = DB.produkties.SingleOrDefault<produkty>(p => p.zrodlo_id == id);
                if (res != null)
                {
                    list = res.nazwa + "[" + res.kod + "]";
                }
            }
            return list;
        }

        internal static bool ProduktNalezyDoKategoriiZrodlowej(int produktID, int kategoriaZrodlowaPremium)
        {
            using (MainDataContext DB = new MainDataContext(GlobalConfig.MainCS))
            {
                if ((from p in DB.produkty_kategorie_zrodlowes
                     where (p.kategoria_zrodlowa_id == kategoriaZrodlowaPremium) && (p.produkt_id == produktID)
                     select p) != null)
                {
                    return true;
                }
            }
            return false;
        }

        internal static int ZnajdzIdProduktu(string nazwa, string kod)
        {
            MainDataContext DB = null;
            int id = -1;
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                produkty res = DB.produkties.SingleOrDefault<produkty>(r => (r.kod.ToLower() == kod.ToLower()) && (nazwa.ToLower() == r.nazwa.ToLower()));
                if (res != null)
                {
                    id = res.zrodlo_id;
                }
            }
            return id;
        }

        internal static string[] ZnajdzProduktyPoNazwie(string prefixText, int count)
        {
            MainDataContext DB = null;
            string[] list = null;
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                IQueryable<string> res = from x in DB.produkties
                                         where x.nazwa.ToLower().Contains(prefixText.ToLower())
                                         select (x.nazwa + " [") + x.kod + "]";
                if (res != null)
                {
                    list = res.ToArray<string>();
                }
            }
            return list;
        }
    }
}

