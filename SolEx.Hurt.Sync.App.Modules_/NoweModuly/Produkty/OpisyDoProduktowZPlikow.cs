using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class OpisyDoProduktowZPlikow : Rozne.OpisyDoProduktowZPlikowBaza, Model.Interfaces.SyncModuly.IModulProdukty
    {
        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            PrzetworzBaza(ref listaWejsciowa, ref produktyTlumaczenia);
        }
    }
}
