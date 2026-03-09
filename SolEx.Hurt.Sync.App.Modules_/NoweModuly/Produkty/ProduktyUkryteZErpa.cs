using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Produkty ukryte z Erpa",FriendlyOpis = "Pobiera widoczność produktów z Erpa dla providera Tema, Hermes")]
    public class ProduktyUkryteZErpa : SyncModul,IModulRabaty, IModulProdukty
    {
        public override string uwagi => "Wymaga licencji katalog_klienta";

        public IConfigSynchro Konfiguracja = SyncManager.PobierzInstancje.Konfiguracja;

        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryte, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B,Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechyNaPlatformie, 
            Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            ISyncProvider provider = SyncManager.PobierzInstancje.GetProvider(SyncManager.PobierzInstancje.Konfiguracja.ProviderERP, SyncManager.PobierzInstancje.Konfiguracja);
            PrzetworzMain(provider, ref produktyUkryte, kategorie);
        }

        private void PrzetworzMain(ISyncProvider provider, ref List< ProduktUkryty> produktyUkryte, Dictionary<long, KategoriaProduktu> kategorie)
        {
            var prov = provider as IPobieranieProduktowUkrytych;
            produktyUkryte = prov.PobierzProduktyUkryte(kategorie);
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki,ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii,
            ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
           PrzetworzMain(provider,ref produktuUkryteErp, kategorie);
        }
    }

}

