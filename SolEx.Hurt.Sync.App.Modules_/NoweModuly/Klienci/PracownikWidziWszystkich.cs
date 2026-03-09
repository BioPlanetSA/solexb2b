using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    public enum Widocznosc { WidziWszyskich,WidziTylkowSwoich}

    [FriendlyName("Widoczność klientów w adminie - pracownik ma widzieć tylko swoich (przypisanych do pracownika) czy wszystkich.")]
    public class PracownikWidziWszystkich : SyncModul,IModulKlienci
    {
        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {

            Dictionary<int, KategoriaKlienta> wyfiltrowaneKategorieKlientow = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, GrupaKlienta, false);
            HashSet<long> lacznikiPrzefiltrowaneWgKategorii = new HashSet<long>( laczniki.Where(x => wyfiltrowaneKategorieKlientow.ContainsKey(x.KategoriaKlientaId)).Select(x => x.KlientId) );

            foreach (KeyValuePair<long, Klient> k in listaWejsciowa)
            {
                if (lacznikiPrzefiltrowaneWgKategorii.Contains(k.Key))
                {
                    k.Value.WidziWszystkich = Widocznosc == Widocznosc.WidziWszyskich;
                }
            }
        }

        [FriendlyName("Grupa klienta z B2B zawierająca wybraną cechę")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string GrupaKlienta { get; set; }
        
        [FriendlyName("Widoczność klientów ustawiana dla pracownika który POSIADA wybrane cechy / grupy / kategorie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Widocznosc Widocznosc { get; set; }
    }
}
