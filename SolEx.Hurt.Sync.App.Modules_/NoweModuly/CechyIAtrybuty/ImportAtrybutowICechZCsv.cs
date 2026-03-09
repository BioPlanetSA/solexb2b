using System;
using System.IO;
using CsvHelper;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    [FriendlyName("Import atrybutów i cech z CSV",
        FriendlyOpis = "Importuje cechy i atrybyty z pliku CSV. <br/> W pierwszej kolumnie MUSI znajdować się kod produktu/kod kreskowy <br/> PLIK MUSI POSIADAC NAGŁÓWKI")]
    public class ImportAtrybutowICechZCsv : CechyModulBaza, IKonfiguracjaDlaModulowCsv
    {
        public IConfigSynchro Config = SyncManager.PobierzInstancje.Konfiguracja;
        public CSVHelperExt HelperCsv = new CSVHelperExt();
        public ImportAtrybutowICechZCsv()
        {
            PoCzymSzukacProdukty = TypyPolDoDopasowaniaZdjecia.Kod;
            PomijajProduktyNieaktywne = true;
        }
        
        [FriendlyName("Po jakim polu z produktu powiązać")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public TypyPolDoDopasowaniaZdjecia PoCzymSzukacProdukty { get; set; }

        [FriendlyName("Ścieżka do pliku CSV z którego importujemy cechy.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SciezkaDoPlikuCsv { get; set; }

        [FriendlyName("Czy tworzyć cechy nadrzędne - musisz zaznaczyć na TAK jeśli mają być tworzone filtry konfiguratora")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyTworzeniaCechNadrzednych { get; set; }

        [FriendlyName("Kolumny z których będą importowane cechy - rozdzielone ;. (Np.: Marka;model;silnik;rocznik)")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string KolumnyDoImportu { get; set; }

        [FriendlyName("Czy pomijać cechy dla produktów nieaktywnych")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool PomijajProduktyNieaktywne{ get; set; }


        public override void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            Dictionary<long, ProduktCecha> laczniki = new Dictionary<long, ProduktCecha>();
            WyciagnijCeche(ref atrybuty, ref cechy,produktyNaB2B, ref laczniki);
        }

        public override void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            WyciagnijCeche(ref atrybuty, ref cechy,produktyNaB2B, ref lacznikiCech);
        }

        public void WyciagnijCeche(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B, ref Dictionary<long, ProduktCecha> lacznikiCech)
        {
            if (string.IsNullOrEmpty(KolumnyDoImportu.Trim()))
            {
                throw new Exception("Brak Kolumn do importu");
            }
            Log.DebugFormat($"Ilosc produktów na b2b {produktyNaB2B.Count}");
            Log.DebugFormat($"Ilosc Cech {cechy.Count}");
            Log.DebugFormat($"Szukam produktów po {PoCzymSzukacProdukty}");
                    
                if (produktyNaB2B.Any())
                {
                    Log.DebugFormat($"Ilosc produktów na b2b {produktyNaB2B.Count}");
                }
                else
                {
                    Log.ErrorFormat("Brak produktów B2B - moduł nie może dalej działać.");
                    return;
                }

            Dictionary<string, long> slownikPomocniczyProduktow = new Dictionary<string, long>();// = produktyNaB2B.Where(x=>!string.IsNullOrEmpty(PobierzKlucz(x.Value))).ToDictionary(x => PobierzKlucz(x.Value), x => x.Key);
            foreach (var produkt in produktyNaB2B)
            {
                string klucz = PobierzKlucz(produkt.Value);
                if (string.IsNullOrEmpty(klucz))
                {
                    continue;
                }
                if (slownikPomocniczyProduktow.ContainsKey(klucz))
                {
                    Log.InfoFormat($"Produkt: {klucz}[{produkt.Key}] jest już w słowniku. DUBLIKAT SYMBOLU");
                }
                else
                {
                    slownikPomocniczyProduktow.Add(klucz,produkt.Key);
                }
            }

            Log.Debug("Rozpoczynam pobieranie odczyt pliku CSV");
            TextReader textreader = HelperCsv.OtworzPlik(SciezkaDoPlikuCsv);
            CsvReader r = HelperCsv.StworzCsvReaderZKonfiguracja(textreader);
            HashSet<int> idKolumnDoImportu = null;
            HashSet<string> pominete = new HashSet<string>();

            //pobieram nagłówek 
            r.Read();
            r.ReadHeader();
            string[] naglowek = r.FieldHeaders;
            Log.DebugFormat($"Nagłówek pliku:{naglowek.ToCsv()}");

            //pobieranie id Kolumn które bedizemy importować
            idKolumnDoImportu = HelperCsv.KolumnyImportowane(naglowek, KolumnyDoImportu);
            Log.DebugFormat($"Kolumny do importu:{idKolumnDoImportu.ToCsv()}");

            //szukamy lub dodajemy atrybuty które są używane w tym pliku, nazwy atrybutów są w nagłówku
            //w słowniku przechowujemy nazwe i id atrybutów które są używane w pliku
            Dictionary<string, int> slownikAtrybutow= new Dictionary<string, int>();
            foreach (int i in idKolumnDoImportu)
            {
                //szukaj lub Dodaj Atrybut 
                Atrybut tmp = ZnajdzAtrybut(naglowek[i], atrybuty) ?? DodajBrakujacyAtrybut(naglowek[i], atrybuty);
                slownikAtrybutow.Add(tmp.Nazwa, tmp.Id);
            }

            while (r.Read())
            {
                string symbolProd = r[0];

                if (string.IsNullOrEmpty(symbolProd) || pominete.Contains(symbolProd))
                {
                    continue;
                }

                Produkt produkt = null;
                try
                {
                    produkt = produktyNaB2B[slownikPomocniczyProduktow[symbolProd]];
                }
                catch (KeyNotFoundException)
                {
                    if (PomijajProduktyNieaktywne)
                    {
                        pominete.Add(symbolProd);
                        continue;
                    }
                }

                List<Cecha> noweCechy = new List<Cecha>();
                HashSet<long> idCechyNadrzednej = new HashSet<long>();
                foreach (int i in idKolumnDoImportu)
                {
                    string nazwaCechy = r[i];
                    if (string.IsNullOrEmpty(nazwaCechy))
                    {
                        continue;
                    }
                    //szukamy id potrzebnego atrybutu
                    int idAtrybutu = slownikAtrybutow[naglowek[i]];
                    string symbolCechy = naglowek[i] + ":" + nazwaCechy;
                    //szukaj lub dodaj Cechę 
                    Cecha nowa = ZnajdzCeche(symbolCechy, cechy) ?? DodajBrakujacaCeche(symbolCechy, nazwaCechy, idAtrybutu, cechy);

                    if (CzyTworzeniaCechNadrzednych && idCechyNadrzednej.Any())
                    {
                        nowa = DodajCecheNadrzedna(nowa, idCechyNadrzednej);
                    }
                    
                    if (Config.TrybPokazywaniaFiltrow == TrybPokazywaniaFiltrow.WymuszajSciezke) idCechyNadrzednej = new HashSet<long> {nowa.Id};
                    if (Config.TrybPokazywaniaFiltrow == TrybPokazywaniaFiltrow.WszystkieAtrybutyJednoczesnie) idCechyNadrzednej.Add(nowa.Id);
                    noweCechy.Add(nowa);
                }

                foreach (Cecha cecha in noweCechy)
                {
                    DodajBrakujaceLaczniki(cecha.Id, produkt.Id, lacznikiCech);
                }
            }

            if (pominete.Any()) Log.ErrorFormat($"Nie udało się odnaleźć produktów: {pominete.ToCsv()}");

        }

        private string PobierzKlucz(Produkt produkt)
        {
            string klucz = null;
                switch (PoCzymSzukacProdukty)
                {
                    case TypyPolDoDopasowaniaZdjecia.Idproduktu:
                    klucz = produkt.Id.ToString();
                        break;
                    case TypyPolDoDopasowaniaZdjecia.Kod:
                        klucz = produkt.Kod;
                    break;
                    case TypyPolDoDopasowaniaZdjecia.KodKreskowy:
                        klucz = produkt.KodKreskowy;
                        break;
                    case TypyPolDoDopasowaniaZdjecia.Rodzina:
                        klucz = produkt.Rodzina;
                        break;
                    case TypyPolDoDopasowaniaZdjecia.PoleTekst1:
                        klucz = produkt.PoleTekst1;
                        break;
                    case TypyPolDoDopasowaniaZdjecia.PoleTekst2:
                        klucz = produkt.PoleTekst2;
                        break;
                }
            return klucz;
        }

        
    }
}
