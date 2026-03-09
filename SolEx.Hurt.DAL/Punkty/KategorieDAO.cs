namespace SolEx.Hurt.Core.Points.DAL
{
    using SolEx.Hurt.Core.Configuration;
    using System;
    using System.Linq;
    using SolEx.Hurt.DAL;

    public class KategorieDAO
    {
        internal static string PobierzNazwe(int id)
        {
            MainDataContext DB = null;
            string list = string.Empty;
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                kategorie res = DB.kategories.SingleOrDefault<kategorie>(p => p.kategoria_id == id);
                if (res != null)
                {
                    list = res.nazwa + "[" + res.grupy.nazwa + "]";
                }
            }
            return list;
        }

        internal static int ZnajdzIdKategorii(string nazwa, string grupa)
        {
            MainDataContext DB = null;
            int id = -1;
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                kategorie res = DB.kategories.SingleOrDefault<kategorie>(r => (r.grupy.nazwa.ToLower() == grupa.ToLower()) && (nazwa.ToLower() == r.nazwa.ToLower()));
                if (res != null)
                {
                    id = res.kategoria_id;
                }
            }
            return id;
        }

        internal static string[] ZnajdzKategoriePoNazwie(string prefixText, int count)
        {
            MainDataContext DB = null;
            string[] list = null;
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                IQueryable<string> res = from x in DB.kategories
                                         where ((x.nazwa.ToLower().Contains(prefixText.ToLower()) && (x.grupy.typ_id == 1))) && x.widoczna
                                         select (x.nazwa + " [") + x.grupy.nazwa + "]";
                if (res != null)
                {
                    list = res.ToArray<string>();
                }
            }
            return list;
        }
    }
}

