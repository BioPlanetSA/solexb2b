using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.DBHelper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    [FriendlyName("Wystawianie WZ Inny Podmiot", FriendlyOpis = "Ładuje inny provider księgowy i wystawia w nim WZ. Mapuje id produktów z drugiego podmiotu do wybranego pola produktu na b2b")]
    public class WystawianieWZInnyPodmiot : SyncModul, IModulZamowienia, IModulProdukty
    {
        private IConfigSynchro PobierzKonfiguracje()
        {
            IConfigSynchro cont = new ConfigWzInnyPoziom(ParametryLaczenia, Operator, Haslo);
            cont.JezykiWSystemie = SyncManager.PobierzInstancje.Konfiguracja.JezykiWSystemie;

            return cont;
        }

        public WystawianieWZInnyPodmiot()
        {
            RodzajDokumentu = TypDokumentu.Wz;
        }

        public SyncManager SyncManager = SyncManager.PobierzInstancje;

        public void Przetworz(ZamowienieSynchronizacja listaWejsciowa, ref List<ZamowienieSynchronizacja> wszystkie,
            ISyncProvider provider, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> laczniki,
            Dictionary<long, Produkt> produktyB2B, List<Cecha> cechy, List<ProduktCecha> cechyProduktyNaPlatfromie)
        {
            ISyncProvider moduldowz = SyncManager.GetProvider(Provider, PobierzKonfiguracje());
            Baza.Zresetuj(ParametryLaczenia);
            try
            {
                ZamowienieSynchronizacja zamdowz = new ZamowienieSynchronizacja(listaWejsciowa) {pozycje = new List<ZamowienieProdukt>()};
                foreach (var poz in listaWejsciowa.pozycje)
                {
                    ZamowienieProdukt dowz = new ZamowienieProdukt(poz);

                    if (produktyB2B.ContainsKey(poz.ProduktId))
                    {
                        object wart = produktyB2B[dowz.ProduktId].PobierzWartoscPolaObiektu(PoleDoWpisaniaZrodlowy);
                        int id;
                        if (wart != null && int.TryParse(wart.ToString(), out id))
                        {
                            dowz.ProduktId = id;
                            zamdowz.pozycje.Add(dowz);
                        }
                    }
                }

                IWystawianieDokumentu wzmod = moduldowz as IWystawianieDokumentu;
                if (wzmod != null && zamdowz.pozycje.Any())
                {
                    string braki;
                    listaWejsciowa.Uwagi += " " + RodzajDokumentu + ": " + wzmod.WystawDokument(zamdowz, 0, SymbolKlienta, SymbolMagazyn, RodzajDokumentu, out braki);
                }
                else
                {
                    listaWejsciowa.Uwagi += " " + RodzajDokumentu + ": Brak zmapowanych pozycji";
                }
            }
            catch (Exception ex)
            {

                LogiFormatki.PobierzInstancje.LogujError(ex);
                Log.Error(ex);
                listaWejsciowa.Uwagi += "Błąd generowania " + RodzajDokumentu;
            }
            finally
            {

                moduldowz.CleanUp();
            }
            Baza.Zresetuj(provider.SourceCS);
        }

        [FriendlyName("Rodzja dokumentu do wystawienia")]
        public TypDokumentu RodzajDokumentu { get; set; }

        [FriendlyName("Nazwa providera księgowego")]
        public ERPProviderzy Provider { get; set; }

        [FriendlyName("Parametry połączenia")]
        public string ParametryLaczenia { get; set; }

        [FriendlyName("Operator")]
        public string Operator { get; set; }

        [FriendlyName("Haslo")]
        [Niewymagane]
        public string Haslo { get; set; }

        [FriendlyName("Symbol klienta na którego wystawiamy dokument")]

        public string SymbolKlienta { get; set; }

        [FriendlyName("Symbol magazynu na którym wystawiamy dokument")]

        public string SymbolMagazyn { get; set; }

        public override string uwagi => "Ładuje inny provider księgowy i wystawia w nim WZ. Mapuje id produktów z drugiego podmiotu do wybranego pola produktu na b2b";

        [FriendlyName("Pole w producie w systemie zrodlowy(podpietym do b2b) w które wpisujemy znalezione id produktu")]
        [SyncSlownikNaPodstawieInnegoTypu("SolEx.Hurt.Model.produkty, SolEx.Hurt.Model")]
        public string PoleDoWpisaniaZrodlowy { get; set; }

        [FriendlyName("Pole w producie w systemie zrodlowy(podpietym do b2b) wg którego mapujemy produkty")]
        [SyncSlownikNaPodstawieInnegoTypu("SolEx.Hurt.Model.produkty, SolEx.Hurt.Model")]
        public string PoleMapowanieZrodlowy { get; set; }

        [FriendlyName("Pole w producie w systemie docelowym wg którego mapujemy produkty")]
        [SyncSlownikNaPodstawieInnegoTypu("SolEx.Hurt.Model.produkty, SolEx.Hurt.Model")]
        public string PoleMapowanieDocelowy { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            ISyncProvider moduldowz = SyncManager.GetProvider(Provider, PobierzKonfiguracje());
            Baza.Zresetuj(ParametryLaczenia);
            try
            {
                List<Tlumaczenie> slownikiwz;
                List<JednostkaProduktu> jednostkiwz;
                var produktywz = moduldowz.PobierzProdukty(out slownikiwz, out jednostkiwz, new HashSet<string>());

                Dictionary<string, Produkt> produktywzwgSzukanego = new Dictionary<string, Produkt>();
                foreach (var p in produktywz)
                {
                    object wart = p.PobierzWartoscPolaObiektu(PoleMapowanieDocelowy);
                    string klucz = wart?.ToString();
                    if (!string.IsNullOrEmpty(klucz))
                    {
                        produktywzwgSzukanego.Add(klucz, p);
                    }
                }

                foreach (var p in listaWejsciowa)
                {
                    object wart = p.PobierzWartoscPolaObiektu(PoleMapowanieZrodlowy);
                    string klucz = wart?.ToString();
                    if (!string.IsNullOrEmpty(klucz))
                    {
                        if (produktywzwgSzukanego.ContainsKey(klucz))
                        {
                            long id = produktywzwgSzukanego[klucz].Id;
                            p.UstawWartoscPolaObiektu(PoleDoWpisaniaZrodlowy, id);
                        }
                    }
                }
            }
            finally
            {

                moduldowz.CleanUp();
            }
            Baza.Zresetuj(provider.SourceCS);
        }
    }
}
