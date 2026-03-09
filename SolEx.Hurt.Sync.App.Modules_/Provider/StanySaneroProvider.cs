using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.DAO;
namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    class StanySaneroProvider : IImportDataModule
    {
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, Model.SourceDB db)
        {
            List<flat_stany> itemsTMP = GetLiteProducts(configuration);
            for (int i = 0; i < db.LiteProducts.Count; i++)
            {
               var source=   itemsTMP.FirstOrDefault();
                if(source!=null)
                {
 
                    db.LiteProducts[i].stan = source.stan;
                  
                    db.LiteProducts[i].stan2 = source.stan;
                    db.LiteProducts[i].stan3 = source.stan;

                    if (db.LiteProducts[i].stan < 0)
                    {
                        db.LiteProducts[i].stan = 0;
                    }
                    if (db.LiteProducts[i].stan2 < 0)
                    {
                        db.LiteProducts[i].stan2 = 0;
                    }
                    if (db.LiteProducts[i].stan3 < 0)
                    {
                        db.LiteProducts[i].stan3 = 0;
                    }
                }
            }
        }
        internal List<flat_stany> GetLiteProducts(System.Collections.Specialized.NameValueCollection configuration)
        {
            //List<produkty> items = new List<produkty>();
            //// magazyny
            //string sid = configuration["symbole_magazynow_zrodlo"];
            //if (string.IsNullOrEmpty(sid))
            //    throw new Exception("Brak zdefiniowanego parametru, symbole_magazynow.");
            
            //string set_prefix = configuration["set_prefix"];
            //bool sumStock = configuration["magazyny_sumuj"] == "1";
            //int takeFrom = 0;
            //Int32.TryParse(configuration["subiekt_pobierz_stany"], out takeFrom);



            //string[] sids = sid.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            //using (SubiektDataContext context = new SubiektDataContext(configuration["erp_cs2"]))
            //{
            //    for (int i = 0; i < sids.Length; i++)
            //    {
            //        string[] group_max = sids[i].Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            //        var m = context.sl_Magazyns.Where(p => group_max.Contains(p.mag_Symbol)).Select(p => p.mag_Id).ToList<int>();
            //        if (m.Count == 0)
            //            throw new Exception("Niepoprawny parametr, symbole_magazynow: " + sid);
            //        // parametry
            //        //  towary
            //        var q = from p in context.vwTowars
            //                where p.st_MagId != null && m.Contains(p.st_MagId.Value) && (p.tw_Rodzaj == 1 || p.tw_Rodzaj == 8 || p.tw_Rodzaj == 2)
            //                group p by p.tw_Id
            //                    into g
            //                    select new { id = g.Key, jednostka = g.Max(p => p.tw_JednMiary), symbol = g.Max(p => p.tw_Symbol), qs = g.Where(z => z.Dostepne != (decimal?)null).Select(z => z.Dostepne), qs2 = g.Where(z => z.st_Stan != (decimal?)null).Select(z => z.st_Stan), min = g.Max(z => z.tw_StanMin), grupa = g.Min(z => z.tw_IdGrupa), dostawa = g.Max(p => p.tw_CzasDostawy), rodzaj = g.Max(p => p.tw_Rodzaj), cechy = context.tw_CechaTws.Where(a => a.cht_IdTowar == g.Key).Select(a => a.cht_IdCecha).ToList<int>() };
            //        foreach (var v in q)
            //        {
            //            produkty item = null;
            //            item = items.FirstOrDefault(p => p.produkt_id == v.id);
            //            if (item == null)
            //            {
            //                // log.Error(v.symbol + " " + v.min.ToString());
            //                item = new produkty();
            //                item.produkt_id = v.id;
            //                item.jednostka_miary = v.jednostka;
            //                item.kod = v.symbol;
            //                item.stan_min = v.min == null ? 0 : v.min.Value;
            //                if (v.grupa != null)
            //                    item.CategoryIds.Add(v.grupa.Value);
            //                item.DeliveryTime = v.dostawa;
            //                item.AttributeIds = v.cechy;
            //                items.Add(item);
            //            }
            //            decimal? d = null;
            //            switch (takeFrom)
            //            {
            //                case 0:
            //                    d = v.qs.Sum();
            //                    break;
            //                case 1:
            //                    d = v.qs2.Sum();
            //                    break;
            //            }
            //            if (i == 0)
            //            {
            //                item.flat_stany.stan = (d.HasValue ? d.Value : 0)-item.stan_min;
            //            }
            //            else
            //                if (i == 1)
            //                {
            //                    item.flat_stany.stan2 = (d.HasValue ? d.Value : 0)-item.stan_min;
            //                }
            //                else
            //                {
            //                    item.flat_stany.stan3 = (d.HasValue ? d.Value : 0)-item.stan_min;
            //                }
            //            if (v.rodzaj == 2)
            //                item.flat_stany.stan = 1000;
            //        }
            //    }
            //}
            //return items;
            return null;
        }
        public event ProgressChangedEventHandler ProgresChanged;
    }
}
