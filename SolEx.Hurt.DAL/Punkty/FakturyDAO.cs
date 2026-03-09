namespace SolEx.Hurt.Core.Points.DAL
{
    using SolEx.Hurt.Core.Configuration;
    using SolEx.Hurt.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SolEx.Hurt.Core.Points.DAL;
    using SolEx.Hurt.DAL;

    public class FakturyDAO
    {
        internal static List<Document> PobierzFakturyOdDaty(DateTime first, int? langId, int type, bool sprawdzacCzyJuzPrzetworzone,List<int> idKlientow)
        {
            MainDataContext DB = null;
            List<Document> items = null;
            Document item = new Document();
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                
                string select_docs = 
                    "select CreateDate=data_utworzenia,klient_id= klient_id,id=id,IsPaid=zaplacono,nazwa= nazwa_dokumentu,ValueBrutto=wartosc_brutto,ValueNetto=wartosc_netto from historia_dokumenty where rodzaj_id=1 and data_utworzenia>"
                    + first.ToShortDateString() + " " + (sprawdzacCzyJuzPrzetworzone ? " and id not in(select dokument_id from punkty where dokument_id is not null) " : "") + DodajKlientow(idKlientow);
 
                items = DB.ExecuteQuery<Document>(select_docs).ToList();
                string select_items = "SELECT DocumentId=dokument_id,id=id,PriceBrutto=cena_brutto,PriceNetto=cena_netto,ValueBrutto=wartosc_brutto,ValueNetto=wartosc_netto,ValueVat=wartosc_vat,ProductId=p.produkt_id,"
              + "ProductName=nazwa_produktu,ProductSymbol=kod_produktu,Quantity=ilosc,VAT=hdp.vat,QuantityUnit=hdp.jednostka_miary,PointPrice=p.cena_punkty FROM historia_dokumenty_produkty hdp"
               + "     join produkty p on hdp.produkt_id=p.zrodlo_id";
                List<OrderItem> order_items = DB.ExecuteQuery<OrderItem>(select_items).ToList();
                for (int i = 0; i < items.Count; i++)
                {


                    items[i].Items = order_items.Where(p => p.DocumentId == items[i].Id).ToList();
                }
            }
            return items;
        }

        private static string DodajKlientow(List<int> idKlientow)
        {
            StringBuilder sb = new StringBuilder();
            if (idKlientow != null)
            {
                sb.Append("and klient_id in (select zrodlo_id from klienci where klient_id in (");
                for (int i = 0; i < idKlientow.Count; i++)
                {
                    sb.Append(idKlientow[i]);
                    sb.Append(",");
                }

                    sb.Append("-1))");
            }

            return sb.ToString();
        }
    }
}

