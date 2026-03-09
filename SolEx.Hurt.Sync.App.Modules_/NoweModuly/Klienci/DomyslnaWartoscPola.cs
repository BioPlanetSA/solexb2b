using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [FriendlyName("Domyślne wartości dla pól",FriendlyOpis = "Określa które pola klientów z systemu ERP będą nadpisane wartością domyślną")]
    public class DomyslnaWartoscPola:  DomyslnaWartoscPolaBase, Model.Interfaces.SyncModuly.IModulKlienci
    {

        public DomyslnaWartoscPola()
        {
            Pola = new List<string>();
        }



        [FriendlyName("Lista pól klientów które mają być nadpisane przez wartość domyślną")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }
       
        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            if (Pola.Count == 0)
            {
                return;
            }

            IEnumerable<Klient> klienci = ApiWywolanie.PobierzKlientow().Values;
            
            Przetworz<Klient>(listaWejsciowa.Values.Where(a => klienci.AsParallel().Any(b=> b.Id == a.Id)).ToList(), Pola);
        }
    }

}
