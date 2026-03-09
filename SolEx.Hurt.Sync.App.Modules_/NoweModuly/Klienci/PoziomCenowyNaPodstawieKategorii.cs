using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [FriendlyName("Poziom cenowy na postawie kategorii")]
    public class PoziomCenowyNaPodstawieKategorii : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
        public override string uwagi => "";

        [FriendlyName("Kategoria Klienta")]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<int> ListaKategoriiKlienta { get; set; }

        [FriendlyName("Poziom cenowy dla klientów posiadających określoną kategorię")]
        [PobieranieSlownika(typeof(SlownikPoziomuCen))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int PoziomCenowy { get; set; }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            HashSet<long> idKlientowZKategori =new HashSet<long>( laczniki.Where(x => ListaKategoriiKlienta.Contains(x.KategoriaKlientaId)).Select(x => x.KlientId) );
            
            foreach (var klienci in listaWejsciowa)
            {
                if (idKlientowZKategori.Contains(klienci.Key))
                {
                    klienci.Value.PoziomCenowyId = PoziomCenowy;
                }
            }
        }

       
    }
}
