using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.ERP.SubiektGT;
using SolEx.Hurt.Sync.Core.Configuration;
using SolEx.Hurt.Model;
using SolEx.ERP.Model;
using SolEx.Hurt.DAL;

namespace SolEx.Hurt.Sync.App.Modules_.Provider.ImportingOrders
{
    class WZSubiektProvider : IImportingOrderModule
    {
        public void DoWork(ref List<Model.Order> orders)
        {
            string u, haslo, c  = null;
            int klientID, magazynID = 0;
            int? poziomCenyDomyslny = null;
            try
            {
                u = DAL.Config.Settings.GetSettingString("modulWZ-erp_login","");
                haslo = DAL.Config.Settings.GetSettingString("modulWZ-erp_haslo", "");
                c = DAL.Config.Settings.GetSettingString("modulWZ-erp_cs", "");
                klientID = DAL.Config.Settings.GetSettingInt("modulWZ-klientID", 0);
                magazynID = DAL.Config.Settings.GetSettingInt("modulWZ-magazynID", 0);
                try
                {
                    poziomCenyDomyslny = DAL.Config.Settings.GetSettingInt("modulWZ-poziomCenyId", 1);
                }
                catch
                {
                    poziomCenyDomyslny = null;
                }

                Polaczenie.KillSubiekt();
                Polaczenie.UstawParametryPolaczenia(u, haslo, c);
            }
            catch (Exception e)
            {
                throw new Exception("ModulWZ- Niepoprawne lub brak parametrów konfiguracyjnych Subiekta", e);
            }

            string bledy = "";
            foreach (Order o in orders)
            {
                List<Produkt> produkty = new List<Produkt>(o.Items.Count);
                string nieOdnalezioneTowary = "";

                foreach (OrderItem item in o.Items)
                {
                    try
                    {
                        Produkt p = new Produkt();
                        p.Id = Towary.PobierzIdTowaru(item.ProductSymbol);
                        p.Nazwa = item.ProductName;
                        p.Ilosc = item.Quantity;
                        p.PodstJmiary = item.QuantityUnit;
                        p.Symbol = item.ProductSymbol;
                        produkty.Add(p);
                    }
                    catch
                    {
                        //pomijamy towary ktorych nie ma w sanero
                        nieOdnalezioneTowary += item.ProductSymbol + " ; ";
                        continue;
                    }
                }

                if (produkty.Count == 0)
                {
                    o.Desc = "  BRAK PRODUKTÓW DO WZ";
                    continue;
                }
                if (!string.IsNullOrEmpty(nieOdnalezioneTowary))
                {
                    nieOdnalezioneTowary = "  BRAK KARTOTEK: " + nieOdnalezioneTowary;
                }
                
                bledy = Dokumenty.DodajDokument(SubiektGTDokumentEnum.gtaSubiektDokumentWZ, magazynID, produkty, "B2B nr " + o.Id, klientID, nieOdnalezioneTowary, false, poziomCenyDomyslny);
                
                bledy = bledy + nieOdnalezioneTowary;

                if (!string.IsNullOrEmpty(bledy))
                {
                    bledy = "WZ_STATUS:" + bledy + "|||";
                }
                else
                {
                    bledy = "WZ_STATUS: OK";
                }

                o.Desc = o.Desc + "    " + bledy;
            }

            Polaczenie.KillSubiekt();            

        }
    }

}
