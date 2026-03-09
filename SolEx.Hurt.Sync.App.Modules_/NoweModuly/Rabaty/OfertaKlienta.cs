using System;
using System.CodeDom;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Common;
using ServiceStack.Text;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{

    [FriendlyName("Indywidualna oferta klienta",
         FriendlyOpis = "Ukrywanie, odkrywanie produktów dla wybranych klientów wg. wspólnych cech, które równocześnie posiada klient oraz towar (cechy muszą nazywać się tak samo - np. producent:X). Wymaga licencji katalog_klienta")]
    public class OfertaKlienta : SyncModul, IModulRabaty, IModulProdukty
    {
        public IConfigSynchro Konfiguracja = SyncManager.PobierzInstancje.Konfiguracja;

        [FriendlyName("Atrybut (lub początek cechy) z ERP, który jest wspólny dla klientów i towarów")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string PoczatekCechy { get; set; }

        [FriendlyName("Łączenie tylko jeśli klient ma przedstawiciela")]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool TylkoZPrzedstawicielem { get; set; }

        public OfertaKlienta()
        {
            PoczatekCechy = string.Empty;
            TypWidocznosci = KatalogKlientaTypy.Dostepne;
            TylkoZPrzedstawicielem = false;
        }

        [FriendlyName("Typ widoczności towarów jaki ustawiać gdy cechy do siebie pasują (klient ma tą samą ceche co towar)")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KatalogKlientaTypy TypWidocznosci { get; set; }

        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryte, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B, Dictionary<long, Produkt> produkty,
                              List<PoziomCenowy> ceny, List<Cecha> cechyNaPlatformie, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie,
                              ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            PrzetworzMain(ref produktyUkryte, cechyNaPlatformie, kliencib2B, ref kategorieKlientow, ref klienciKategorie);
        }

        private void PrzetworzMain(ref List<ProduktUkryty> produktyUkryte, List<Cecha> cechyNaPlatformie, IDictionary<long, Klient> klienci, ref IDictionary<int, KategoriaKlienta> kategorieKlientow,
                                   ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            if (string.IsNullOrEmpty(PoczatekCechy))
            {
                LogiFormatki.PobierzInstancje.LogujError(new Exception("Początek cechy jest pusty, moduł zakończy działanie."));
                return;
            }

            char[] separatoryCechProduktow = Konfiguracja.SeparatorAtrybutowWCechach;

            if (separatoryCechProduktow.IsEmpty())
            {
                LogiFormatki.PobierzInstancje.LogujError(new Exception("Ustawienie 'separator_atrybutow_w_cechach' jest puste, moduł przerwie działanie."));
                return;
            }

            List<Klient> kliencieDoSprawdzenia = klienci.Values.Where(x => x.Aktywny && (TylkoZPrzedstawicielem == false || x.PrzedstawicielId.HasValue)).ToList();

            if (kliencieDoSprawdzenia.IsEmpty())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak klientów do sprawdzenia (klienci muszą być aktywni /i/lub mieć przedstawiciela - w zależności od ustawień modułu. Koniec działania");
                return;
            }

            if (produktyUkryte == null)
            {
                produktyUkryte = new List<ProduktUkryty>();
            }

            int licznikProduktowUkrytychPrzedStaremModulu = produktyUkryte.Count;

            Dictionary<int, KategoriaKlienta> wyfiltrowaneKategorieKlientow = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorieKlientow.Values.ToList(), PoczatekCechy, false);

            if (wyfiltrowaneKategorieKlientow.IsEmpty())
            {
                LogiFormatki.PobierzInstancje.LogujInfo(
                    $"Brak kategorii klientów spełaniających warunek początku cechy: '{PoczatekCechy}'. Wszystkich kategorii w sumie: {kategorieKlientow.Count}. Separator kategorii klientów: {Konfiguracja.SeparatorGrupKlientow.ToCsv()} Moduł kończy działanie.");
                return;
            }

            Dictionary<int, long> cechyTowarowDopasowaneDoKategoriiKlientow = new Dictionary<int, long>(wyfiltrowaneKategorieKlientow.Count);

            foreach (KategoriaKlienta kat in wyfiltrowaneKategorieKlientow.Values)
            {
                foreach (char separator in separatoryCechProduktow)
                {
                    string wlasciwaCecha = kat.Grupa + separator + kat.Nazwa;
                    var cecha = cechyNaPlatformie.FirstOrDefault(a => a.Symbol.Equals(wlasciwaCecha, StringComparison.InvariantCultureIgnoreCase));
                    if (cecha != null)
                    {
                        cechyTowarowDopasowaneDoKategoriiKlientow.Add(kat.Id, cecha.Id);
                    }
                }
            }

            List<KlientKategoriaKlienta> lacznikiPrzefiltrowaneWgKategoriiKlientowDopasowanychDoCechProduktow = klienciKategorie.Values.Where(x => cechyTowarowDopasowaneDoKategoriiKlientow.ContainsKey(x.KategoriaKlientaId)).ToList();

            LogiFormatki.PobierzInstancje.LogujInfo(
                $"Wszystkich kategorii: {kategorieKlientow.Count}. Wyfiltrowano: {wyfiltrowaneKategorieKlientow.Count} kategorii klientów dopasowanych do frazy: {PoczatekCechy}. Z tego wynika: {lacznikiPrzefiltrowaneWgKategoriiKlientowDopasowanychDoCechProduktow.Count} łączników kat. klientów, oraz {cechyTowarowDopasowaneDoKategoriiKlientow.Count} dopasowań do cech produktów (identycznych kategorii i cech produktów)");

            if (wyfiltrowaneKategorieKlientow.IsEmpty())
            {
                throw new Exception("Brak cech które pasują do siebie (klientów = towarów).");
            }

            var klienciIDsWyfiltrowani = new HashSet<long>( lacznikiPrzefiltrowaneWgKategoriiKlientowDopasowanychDoCechProduktow.Select(x => x.KlientId) );
            LogiFormatki.PobierzInstancje.LogujInfo($"Wszystkich klientów aktywnych: {kliencieDoSprawdzenia.Count}, ale po przefiltrowaniu wg. wymaganych kategorii klientów wybrano: {klienciIDsWyfiltrowani.Count}");

            if (lacznikiPrzefiltrowaneWgKategoriiKlientowDopasowanychDoCechProduktow.IsEmpty())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak kategorii klientów dopasowanych do produktów - kończe dzialanie modułu.");
            }

            if (cechyTowarowDopasowaneDoKategoriiKlientow.Count != wyfiltrowaneKategorieKlientow.Count)
            {
                List<string> KategorieKlientaNazwyKtorychNieMaWProduktach = wyfiltrowaneKategorieKlientow.Values.Where(x => !cechyTowarowDopasowaneDoKategoriiKlientow.ContainsKey(x.Id)).Select(x => $"{x.Id} [{x.Nazwa}]").ToList();
                LogiFormatki.PobierzInstancje.LogujInfo($"Brakuje cech w produktach do budowania oferty klientów. Istnieją kategorie w klientach, ale nie ma ich w produktach: {KategorieKlientaNazwyKtorychNieMaWProduktach.Join(",")}");
            }


            kliencieDoSprawdzenia = kliencieDoSprawdzenia.Where(x => klienciIDsWyfiltrowani.Contains(x.Id)).ToList();

            foreach (var k in kliencieDoSprawdzenia)
            {
                HashSet<int> kategorieKlientaIds = new HashSet<int>( lacznikiPrzefiltrowaneWgKategoriiKlientowDopasowanychDoCechProduktow.Where(x => x.KlientId == k.Id).Select(x => x.KategoriaKlientaId) );
                foreach (int katID in kategorieKlientaIds)
                {
                    try
                    {
                        long cechaTowaruId = cechyTowarowDopasowaneDoKategoriiKlientow[katID];
                        //Log.DebugFormat("Właściwa cecha zbudowana na podstawie kategorii klientów: {0}", wlasciwaCecha);
                        ProduktUkryty pu = new ProduktUkryty();
                        pu.KlientZrodloId = k.Id;
                        pu.Tryb = TypWidocznosci;
                        pu.CechaProduktuId = cechaTowaruId;
                        produktyUkryte.Add(pu);
                    } catch (Exception e)
                    {
                        throw new Exception($"Błąd tworzenia powiązania produktu ukrytego dla kategorii klientów id: {katID}. Zrzut kluczy kategorii klientów: {cechyTowarowDopasowaneDoKategoriiKlientow.ToCsv()}", e);
                    }
                }
            }

            if (licznikProduktowUkrytychPrzedStaremModulu == produktyUkryte.Count)
            {
                LogiFormatki.PobierzInstancje.LogujInfo(
                    $"Moduł oferty klienta nie wykonał żadnych zmian dla cechy: {PoczatekCechy}. Ilość klientów: {klienci.Values.Count}, ilość cech z platformy: {cechyNaPlatformie.Count}. Ilość kategorii klientów: {kategorieKlientow.Count}");
            }
            else
            {
                LogiFormatki.PobierzInstancje.LogujInfo(string.Format($"Moduł oferty klienta wykonał: {produktyUkryte.Count - licznikProduktowUkrytychPrzedStaremModulu} wpisów w produkty ukryte."));
            }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            var klienci = ApiWywolanie.PobierzKlientow();
            var kategorieKlientow = (IDictionary<int, KategoriaKlienta>)ApiWywolanie.PobierzKategorieKlientow();
            IDictionary<long, KlientKategoriaKlienta> klienciKategorie = ApiWywolanie.PobierzKlienciKategorie(new Dictionary<string, object>());

            PrzetworzMain(ref produktuUkryteErp, cechy, klienci, ref kategorieKlientow, ref klienciKategorie);
        }
    }

}

