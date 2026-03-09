using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    public class ZalacznikiDoProduktowLinki : SyncModul, IModulPliki
    {
        [Lokalizowane]
        [FriendlyName("Jak pokazywać link na szczegolach")]
        public string LinkProduktu { get; set; }
        public void Przetworz(System.Collections.Generic.IDictionary<long, Produkt> produktyNaB2B, ref System.Collections.Generic.List<ProduktPlik> plikiLokalnePowiazania, ref System.Collections.Generic.List<Plik> plikiLokalne, Model.Interfaces.Sync.ISyncProvider provider, ref System.Collections.Generic.List<Cecha> cechyB2B, ref System.Collections.Generic.List<KategoriaProduktu> kategorieB2B, ref System.Collections.Generic.List<Klient> klienciB2B)
        {
            throw new System.NotImplementedException();
        }

        public override string uwagi
        {
            get { return ""; }
        }
    }
}
