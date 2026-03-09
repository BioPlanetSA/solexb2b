using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Rodziny z pola produktu - BHP", FriendlyOpis = "Moduł tworzący rodziny podstawie wybranego pola produktu")]
    public class RodzinyZPolaProduktuBHP : SyncModul, IModulProdukty
    {
        public override string uwagi
        {
            get { return ""; }
        }

        //public override string Opis
        //{
        //    get { return "Moduł tworzący rodziny podstawie wybranego pola produktu"; }
        //}

        [FriendlyName("Pole produktu z ERP wg ktorego będzie stworzona rodzina")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Pole { get; set; }

        [FriendlyName("Pole z ERP wg ktorego będzie wybrana nazwa rodziny")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdmin(false, false, true, false)]
        public string PoleNazwaRodziny { get; set; }

        [Niewymagane]
        [FriendlyName("Separator oddzielający rodzinę od innych danych w polu produktu (rodzina będzie brana z pierwszego członu)")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Separator { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            List<string> dostepneRodziny = new List<string>();

            foreach (Produkt produkt in listaWejsciowa)
            {
                object nazwa = produkt.PobierzWartoscPolaObiektu(Pole);
                
                if (nazwa == null)
                {
                    Log.ErrorFormat("Nie udało się pobrać wartości pola {0} dla produktu o symbolu {1}", Pole,
                        produkt.Kod);
                    continue;
                }
                
                string nazwaRodziny = PobierzNazweRodziny(nazwa);
                
                if(!dostepneRodziny.Contains(nazwaRodziny))
                    dostepneRodziny.Add(nazwaRodziny);
            }

            HashSet<long> wykorzystaneIdProduktow = new HashSet<long>();
            foreach (string rodzina in dostepneRodziny)
            {
                List<Produkt> produktyWrodzinie = new List<Produkt>();

                for (int i = 0; i < listaWejsciowa.Count; i++)
                {
                    if (!wykorzystaneIdProduktow.Contains(listaWejsciowa[i].Id) && PobierzNazweRodziny(listaWejsciowa[i].PobierzWartoscPolaObiektu(Pole, false)) == rodzina)
                    {
                        produktyWrodzinie.Add(listaWejsciowa[i]);
                        wykorzystaneIdProduktow.Add(listaWejsciowa[i].Id);
                    }
                }
                if (produktyWrodzinie.Count > 1)
                {
                    PropertyInfo propertis = produktyWrodzinie.First().GetType().GetProperties().First(x => x.Name == PoleNazwaRodziny);
                    var nazwaRodziny = produktyWrodzinie.Select(produkty => propertis.GetValue(produkty, null)).FirstOrDefault(a => a != null);
                    //string nazwaRodziny =
                    //    produktyWrodzinie.First().PobierzWartoscPolaObiektu(PoleNazwaRodziny).ToString();
                    foreach (Produkt produkty in produktyWrodzinie)
                    {
                        produkty.Rodzina = nazwaRodziny==null?null:nazwaRodziny.ToString();
                    }
                }
            }
        }

        public string PobierzNazweRodziny(object nazwa)
        {
            string nazwaRodziny;
            if (!string.IsNullOrEmpty(Separator))
            {
                string[] wartosciPola = nazwa.ToString()
                    .Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

                nazwaRodziny = wartosciPola.First();
            }
            else
            {
                nazwaRodziny = nazwa.ToString();
            }
            return nazwaRodziny;
        }
    }
}
