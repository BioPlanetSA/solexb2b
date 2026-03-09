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
    public class NadpiszPoziomCenowyKlienta : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
        [FriendlyName("Poziom cenowy który zostanie ustawiony klientom")]
        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikPoziomuCen))]
        public int PoziomCenowy { get; set; }

        [FriendlyName("Kategorie klientów którym przypisany zostanie poziom cenowy - brak wyboru wszystkim klientom zostanie przeypisany poziom cenowy wybrany w module")]
        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [Niewymagane]
        public List<int> KategorieKlientow { get; set; }
        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            HashSet<long>idKlientowZKategoria = null;
            if (KategorieKlientow.Any())
            {
                idKlientowZKategoria = new HashSet<long>( laczniki.Where(x => KategorieKlientow.Contains(x.KategoriaKlientaId)).Select(x => x.KlientId) );
            }
            idKlientowZKategoria = idKlientowZKategoria ?? new HashSet<long>( listaWejsciowa.Keys );
            foreach (var klient in idKlientowZKategoria)
            {
                listaWejsciowa[klient].PoziomCenowyId = PoziomCenowy;
            }

        }

        public override string Opis
        {
            get { return "Moduł,ustawiający klientom określony poziom cenowy"; }
        }
    }
}
