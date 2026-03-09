using System;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [FriendlyName("Aktywacja, deaktywacja klientów na podstawie cechy / kategorii klientów", FriendlyOpis = "Moduł, który filtruje eksportowanych klientów po cesze.")]
    public class KlienciZCecha : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
       [FriendlyName("Poczatek cechy którą musi mieć klient aby był eksportowany.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoczatekCechy { get; set; }

        public KlienciZCecha()
        {
            PoczatekCechy = string.Empty;
        }     

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            if (string.IsNullOrEmpty(PoczatekCechy))
            {
             throw new Exception("Brak parametru PoczatekCechy.");
            }

            int przed = listaWejsciowa.Values.Count(x => x.Aktywny);

            PoczatekCechy = PoczatekCechy.ToLower();

            Dictionary<int, KategoriaKlienta> wyfiltrowaneKategorieKlientow = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, PoczatekCechy, false);
            HashSet<long> lacznikiPrzefiltrowaneWgKategorii = new HashSet<long>( laczniki.Where(x => wyfiltrowaneKategorieKlientow.ContainsKey(x.KategoriaKlientaId)).Select(x => x.KlientId) );

            foreach (KeyValuePair<long, Klient> k in listaWejsciowa)
            {
                if (!lacznikiPrzefiltrowaneWgKategorii.Contains(k.Key))
                {
                    k.Value.Aktywny = false;
                }
            }
            int po = listaWejsciowa.Values.Count(x => x.Aktywny);
            LogiFormatki.PobierzInstancje.LogujInfo("Klientów przed filtracją {0}, klientów po filtracji {1}", przed, po);

        }
    }
}
