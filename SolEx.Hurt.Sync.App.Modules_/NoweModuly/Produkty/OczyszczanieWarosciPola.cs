using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Oczyść wartości pola")]
    public class OczyszczanieWarosciPola : BazaOczyszczaniePola,IModulProdukty
    {
        [FriendlyName("Pole, które oczyszczamy")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleZrodlowe { get; set; }


        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            var propertisy = typeof(Produkt).Properties().Values.ToList();
            var akcesor = typeof(Produkt).PobierzRefleksja();

            foreach (Produkt produkt in listaWejsciowa)
            {
                Oczysc(produkt, propertisy,PoleZrodlowe, akcesor);
            }
        }

    }
}
