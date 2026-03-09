using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    

    public class UsuwanieRabatowKlientaZPoziomemCenowym : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
        [FriendlyName("Poziomy cenowe, gdy brak wyboru wszystkim klientom bedzie usunięty rabat")]
        [PobieranieSlownika(typeof(SlownikPoziomuCen))]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<int> PoziomyCenowe { get; set; }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            HashSet<long> listaKlientow;
            if (PoziomyCenowe != null && PoziomyCenowe.Any())
            {
                listaKlientow = new HashSet<long>( listaWejsciowa.Where(x => x.Value.PoziomCenowyId.HasValue && PoziomyCenowe.Contains(x.Value.PoziomCenowyId.Value)).Select(x => x.Key) );
            }
            else
            {
                listaKlientow = new HashSet<long>( listaWejsciowa.Select(x => x.Key) );

            }
            foreach (var klient in listaKlientow)
            {
                listaWejsciowa[klient].Rabat = 0;
            }
        }

        public override string Opis
        {
            get { return "Moduł, który na podstawie wybranego poziomu cenowego, usuwa rabat klienta"; }
        }
    }


}
