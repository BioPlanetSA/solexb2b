using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.DAO;

namespace SolEx.Hurt.Sync.App.Modules_.Provider.Prices
{
    public class NowaCenaZPoziomuCenProvider : IImportPricesModule
    {
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, List<Model.flat_ceny> list)
        {
            using (DAO.SubiektDataContext db = new DAO.SubiektDataContext(configuration["erp_cs"]))
            {
                string[] s_activity = configuration["subiekt_widocznosc_produktow"] == null || configuration["subiekt_widocznosc_produktow"] == "0" ? new string[] { } : configuration["subiekt_widocznosc_produktow"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var tw = (from p in db.tw__Towars
                          join c in db.tw_Cenas on p.tw_Id equals c.tc_IdTowar
                          where ((p.tw_Rodzaj == 1) || (p.tw_Rodzaj == 8)) && p.tw_Zablokowany == false
                            && (s_activity.Length == 0 ? true : (s_activity.Any(a => a == "1") && (p.tw_SklepInternet == true)) || (s_activity.Any(a => a == "2") && (p.tw_SerwisAukcyjny == true))
                            || (s_activity.Any(a => a == "3") && (p.tw_SprzedazMobilna == true)))
                          select new
                          {
                           
                        c. tc_CenaBrutto1, c.tc_CenaBrutto2, c.tc_CenaBrutto3, c.tc_CenaBrutto4, c.tc_CenaBrutto5, c.tc_CenaBrutto6, c.tc_CenaBrutto7, c.tc_CenaBrutto8, 
                        c.tc_CenaBrutto9, c.tc_CenaBrutto10, c.tc_CenaNetto1, c.tc_CenaNetto2, c.tc_CenaNetto3, c.tc_CenaNetto4, c.tc_CenaNetto5, c.tc_CenaNetto6, 
                        c.tc_CenaNetto7, c.tc_CenaNetto8, c.tc_CenaNetto9, c.tc_CenaNetto10,
                              tw_Id = p.tw_Id
                          }).ToList();
            

                

             
 
                Dictionary<int, List<ceny_poziomy>> products = new Dictionary<int, List<ceny_poziomy>>();
                foreach(var v in tw)
                {
                    List<ceny_poziomy> prices = new List<ceny_poziomy>();
                    var e = db.vwPoziomyCenWaluties.OrderBy(p => p.IDENT).ToList<vwPoziomyCenWaluty>();
                    for (int i = 0; i < 10; ++i)
                    {
                        vwPoziomyCenWaluty vwpl = e.ElementAtOrDefault(i);

                        ceny_poziomy price = new ceny_poziomy();
                        if (v != null)
                        {
                            price.waluta = vwpl.WALUTA;
                            price.nazwa = vwpl.NAZWA;
                            price.id = vwpl.IDENT == null ? 0 : vwpl.IDENT.Value;
                        }
                        prices.Add(price);
                    }
                
                    prices[0].brutto = v.tc_CenaBrutto1;
                    prices[1].brutto = v.tc_CenaBrutto2;
                    prices[2].brutto = v.tc_CenaBrutto3;
                    prices[3].brutto = v.tc_CenaBrutto4;
                    prices[4].brutto = v.tc_CenaBrutto5;
                    prices[5].brutto = v.tc_CenaBrutto6;
                    prices[6].brutto = v.tc_CenaBrutto7;
                    prices[7].brutto = v.tc_CenaBrutto8;
                    prices[8].brutto = v.tc_CenaBrutto9;
                    prices[9].brutto = v.tc_CenaBrutto10;
                    prices[0].netto = v.tc_CenaNetto1;
                    prices[1].netto = v.tc_CenaNetto2;
                    prices[2].netto = v.tc_CenaNetto3;
                    prices[3].netto = v.tc_CenaNetto4;
                    prices[4].netto = v.tc_CenaNetto5;
                    prices[5].netto = v.tc_CenaNetto6;
                    prices[6].netto = v.tc_CenaNetto7;
                    prices[7].netto = v.tc_CenaNetto8;
                    prices[8].netto = v.tc_CenaNetto9;
                    prices[9].netto = v.tc_CenaNetto10;
                    products.Add(v.tw_Id, prices);
                }
                string priceLevelsNames = configuration["NowaCenaZPoziomuCenNazwa"].ToLower();
                for (int i = 0; i < list.Count; i++)
                {
                    var product = products.First(p => p.Key == list[i].produkt_id);
                    var price = product.Value.FirstOrDefault(p => p.netto > 0 && p.nazwa.ToLower() == priceLevelsNames);
                    if (price != null)
                    {
                        list[i].cena_netto = price.netto.Value ;
                        list[i].cena_brutto = price.brutto.Value;
                        list[i].typ_rabatu = 2;
                    }
                }
            }
        }

        public event ProgressChangedEventHandler ProgresChanged;
    }
}
