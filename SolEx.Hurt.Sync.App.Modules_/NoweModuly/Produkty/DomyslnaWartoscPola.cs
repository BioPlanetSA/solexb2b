using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{

    [FriendlyName("Domyślne wartości dla pola",FriendlyOpis = "Określa które pola produktów z systemu ERP będą nadpisane wartością domyślną")]
    class DomyslnaWartoscPola:  DomyslnaWartoscPolaBase, SolEx.Hurt.Model.Interfaces.SyncModuly.IModulProdukty
    {

        public DomyslnaWartoscPola()
        {
            Pola = new List<string>();
        }


        [FriendlyName("Lista pól produktów które mają być nadpisane przez wartość domyślną")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }
       


        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (Pola.Count == 0)
            {
                return;
            }

            Przetworz<Produkt>(listaWejsciowa, Pola);
        }
    }

}
