using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class UstawieniaDomyslnejDostepnosciProduktowDlaKlientow : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        [FriendlyName("Określające czy produkty będą dostępne dla wszystkich.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool DomysnlaDostepnoscDlaKlientow { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia,
            Dictionary<int, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki,
            ref List<ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii,
            Model.Interfaces.Sync.ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp,
            ref List<ProduktyZamienniki> zamienniki, Dictionary<int, KategoriaProduktu> kategorie,
            ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            foreach (var produkt in listaWejsciowa)
            {
                produkt.DostepnyDlaWszystkich = DomysnlaDostepnoscDlaKlientow;
            }
        }

        public override string Opis
        {
            get { return "Modul ustawiajacy widocznośc produktów dla wszystkich klientów - przydatny jeśli chcemy żeby wszystkie produkty były niedostępne, a odkrywane np. na podstawie cech (indywidualna oferta produtktów)"; }
        }
        public override string uwagi
        {
            get { return ""; }
        }
    }
}
