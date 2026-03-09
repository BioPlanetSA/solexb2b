using System;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;

using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [FriendlyName("Język klienta z cechy", FriendlyOpis = "Moduł, który przypisuje język klientowi na podstawie cechy z B2B.")]
    public class JezykZCechy : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
        public IConfigSynchro Config = SyncManager.PobierzInstancje.Konfiguracja;

        public enum PolaJezyka
        {
            Symbol, Nazwa
        }

        [FriendlyName("Grupa lub początek cechy z językiem z platformy B2B")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string GrupaKlienta { get; set; }

        [FriendlyName("Pole języka wg którego będzie dopasowywana grupa klienta")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public PolaJezyka WybranePoleJezyka { get; set; }

        public JezykZCechy() 
        {
            GrupaKlienta = string.Empty;
            WybranePoleJezyka = PolaJezyka.Nazwa;
        }
        
        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            if (string.IsNullOrEmpty(GrupaKlienta))
                return;

            GrupaKlienta = GrupaKlienta.ToLower();
            var jezykiwsystemie = Config.JezykiWSystemie;

            Dictionary<int, KategoriaKlienta> wyfiltrowaneKategorieKlientow = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, GrupaKlienta, false);
            List<KlientKategoriaKlienta> lacznikiPrzefiltrowaneWgKategorii = laczniki.Where(x => wyfiltrowaneKategorieKlientow.ContainsKey(x.KategoriaKlientaId)).ToList();

            Parallel.ForEach(listaWejsciowa, k =>
            {
                KlientKategoriaKlienta idKategorii = lacznikiPrzefiltrowaneWgKategorii.FirstOrDefault(x => x.KlientId == k.Key);
                if (idKategorii == null)
                {
                    return;
                }

                KategoriaKlienta kategoriaKlienta = wyfiltrowaneKategorieKlientow[idKategorii.KategoriaKlientaId];

                Jezyk jezyk = null;
                switch (WybranePoleJezyka)
                {
                    case PolaJezyka.Nazwa:
                    {
                        jezyk = jezykiwsystemie.Values.FirstOrDefault(a => a.Nazwa.Equals(kategoriaKlienta.Nazwa, StringComparison.InvariantCultureIgnoreCase));
                    }
                        break;

                    case PolaJezyka.Symbol:
                    {
                        jezyk = jezykiwsystemie.Values.FirstOrDefault(a => a.Symbol.Equals(kategoriaKlienta.Nazwa, StringComparison.InvariantCultureIgnoreCase));
                    }
                        break;
                }

                if (jezyk == null)
                {
                    throw new Exception($"Nie można dopasować języka dla klienta w oparciu o kategorie: {kategoriaKlienta.Grupa} {kategoriaKlienta.Nazwa}. Złe ustawienia modułu, brak języka na B2B?");
                }
                k.Value.JezykId = jezyk.Id;
            });
        }
    }
}
