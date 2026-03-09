using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Traktuj produkty z platformy jako produkty z Erpa",FriendlyOpis = "Moduł działa tylko dla systemów które nie mają systemu księgowego (provider - Brak)")]
    public class ProduktyZPlatformyJakoProduktyZErpa : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (SyncManager.PobierzInstancje.Konfiguracja.ProviderERP != ERPProviderzy.Brak)
            {
                Log.ErrorFormat("Moduł nie obsługiwany dla aktualnego providera.");
                return;
            }
            var apiProdukty = ApiWywolanie.PobierzProdukty();
            lacznikiCech = ApiWywolanie.PobierzCechyProdukty().Values.Where(x => !x.RecznieDodany()).ToDictionary(x => x.Id, x => x);
            var apiSlowniki = ApiWywolanie.GetSlowniki();
            var apiJednostki = ApiWywolanie.PobierzJednostki();
            var apiLaczniki = ApiWywolanie.PobierzProduktyJednostki().Values;
            var apiKategorieProdukty = ApiWywolanie.PobierzProduktyKategoriePolaczenia();
            var apiProduktyUkryte = ApiWywolanie.PobierzProduktyUkryte();
            var apiZamienniki = ApiWywolanie.PobierzZamienniki();

            var typSlownikow = typeof(ProduktBazowy).PobierzOpisTypu();


            listaWejsciowa = apiProdukty.Values.ToList();
            produktyTlumaczenia = apiSlowniki.Values.Where(x=>x.Typ==typSlownikow).ToList();
            lacznikiKategorii = apiKategorieProdukty.Values.ToList();
            produktuUkryteErp = apiProduktyUkryte.Values.ToList();
            zamienniki = apiZamienniki.Values.ToList();

            foreach (var lacznikiJednostki in apiLaczniki)
            {
                JednostkaProduktu jp = new JednostkaProduktu
                {
                    Podstawowa = lacznikiJednostki.Podstawowa,
                    Id = lacznikiJednostki.JednostkaId,
                    Nazwa = apiJednostki.First(x => x.Key == lacznikiJednostki.JednostkaId).Value.Nazwa,
                    ProduktId = lacznikiJednostki.ProduktId,
                    Przelicznik = lacznikiJednostki.PrzelicznikIlosc
                };
                jednostki.Add(jp);
            }
        }


      
    }
}
