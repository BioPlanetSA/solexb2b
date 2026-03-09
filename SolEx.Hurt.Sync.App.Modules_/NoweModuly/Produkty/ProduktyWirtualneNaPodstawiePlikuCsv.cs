using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Tworzenie produktów wirtualnych na podstawie pliku CSV",
    FriendlyOpis = "Moduł tworzy produkty wirtualne dla produktów z pliku CSV + ustawia parametry do tworzenia wirtualnych produktów z kolumn pliku CSV. Aby klient zobaczył produkty wirtualne musi być włączony jeszcze odpowiedni provider produktów wirualnych w ustawieniach")]
    public class ProduktyWirtualneNaPodstawiePlikuCsv : CechyModulBaza, IKonfiguracjaDlaModulowCsv
    {
        public IConfigSynchro Config = SyncManager.PobierzInstancje.Konfiguracja;
        public CSVHelperExt HelperCsv = new CSVHelperExt();
        [FriendlyName("Kolumny z których będą tworzone prodykty wirtualne - rozdzielone ;. (Np.: Marka;model;silnik;rocznik)")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string KolumnyDoImportu { get; set; }

        [FriendlyName("Po jakim polu z produktu powiązać")]
        [WidoczneListaAdmin(false, false, true, false)]
        public TypyPolDoDopasowaniaZdjecia PoCzymSzukacProdukty { get; set; }


        [FriendlyName("Sposób tworzenia parametrów")]
        [WidoczneListaAdmin(false, false, true, false)]
        public SposobTworzeniaParametrow TworzenieParametrow { get; set; }

        private string[] Naglowek { get; set; }

        [FriendlyName("Ścieżka do pliku CSV z którego importujemy cechy.")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string SciezkaDoPlikuCsv { get; set; }

        public override void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            List<Produkt> listaProduktow = produktyNaB2B.Values.ToList();
            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha>();
            Przetworz(ref listaProduktow, ref atrybuty, ref cechy, ref lacznikiCech);
        }
        public override void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech,
            ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            Przetworz(ref listaWejsciowa, ref atrybuty, ref cechy, ref lacznikiCech);
        }


        public char SeparatorCech;
        public TrybPokazywaniaFiltrow TrybPokazywaniaFiltrow;

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Atrybut> atrybuty, ref List<Cecha> cechy, ref Dictionary<long, ProduktCecha> lacznikiCech)
        {
            if (!listaWejsciowa.Any())
            {
                Log.InfoFormat("Brak produktów moduł przerywa działanie");
                return;
            }
            SeparatorCech = Config.SeparatorAtrybutowWCechach.First();
            TrybPokazywaniaFiltrow = Config.TrybPokazywaniaFiltrow;
            PrzetworzPlik(ref listaWejsciowa, ref atrybuty, ref cechy, ref lacznikiCech);
        }

        private Dictionary<string, long> StworzSLownikProduktowZKluczemZUstawienia(List<Produkt> listaProduktow)
        {
            PropertyInfo pi = listaProduktow.First().GetType().GetProperty(PoCzymSzukacProdukty.ToString());
            Dictionary<string, long> slownikIdProduktow = new Dictionary<string, long>();
            foreach (var produkt in listaProduktow)
            {
                object wartosc = pi.GetValue(produkt);
                if (wartosc != null && !string.IsNullOrEmpty(wartosc.ToString()))
                {
                    slownikIdProduktow.Add(wartosc.ToString(), produkt.Id);
                }
            }
            return slownikIdProduktow;
        }
       
        /// <summary>
        /// Pobieramy pozycje i i nazwy kolum z Dostepnych nagłówków pliku CSV
        /// </summary>
        /// <param name="dostepne"></param>
        /// <param name="kolumnyDoImportu"></param>
        /// <returns></returns>
        private Dictionary<int, string> PobierzNaglowkiIPozycje(string[] dostepne, string kolumnyDoImportu)
        {
            Dictionary<int, string> wynik = new Dictionary<int, string>();
            string[] tablicaKolumn = kolumnyDoImportu.ToLower().Split(';');
            for (int i = 1; i < dostepne.Length; i++)
            {
                if (tablicaKolumn.Contains(dostepne[i], StringComparer.InvariantCultureIgnoreCase))
                {
                    wynik.Add(i, dostepne[i].ToLower());
                }
            }
            return wynik;
        }

        public enum SposobTworzeniaParametrow
        {
            [FriendlyName("Nazwa z pliku CSV")]
            NazwaZPliku = 1,
            [FriendlyName("Twórz cechy i atrybuty")]
            TworzCechyIAtrybuty = 10,
            [FriendlyName("Twórz cechy i atrybuty + ustawiaja nadrzędność cech")]
            TworzCechyIAtrybutyINadrzedne = 20,
        }


        private ConcurrentDictionary<long, ProduktCecha> _lacznikiKonkurencyjne ;
        private ConcurrentDictionary<long, Cecha> _cechyKonkurencyjne;

        public void PrzetworzPlik( ref List<Produkt> listaWejsciowa, ref List<Atrybut> atrybuty, ref List<Cecha> cechy, ref Dictionary<long, ProduktCecha> lacznikiCech)
        {
            Encoding kodowanie;
            Naglowek = null;
            TextReader textreader = HelperCsv.OtworzPlik(SciezkaDoPlikuCsv, out kodowanie);

            List<string[]> zawartosc = new List<string[]>();
            string linia;
            while ((linia = textreader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(linia.Trim()))
                    zawartosc.Add(linia.Split(';'));
            }
            textreader.Close();
            textreader = null;
            ConcurrentDictionary<long, HashSet<Dictionary<object, object>>> slownikDanychProduktowWirtualnych = new ConcurrentDictionary<long, HashSet<Dictionary<object, object>>>();

            //// pozycja i nagłówek kolumny dostępnych w pliku
            Dictionary<int, string> kolumny = new Dictionary<int, string>();

            //pobieranie nagłowku z pliku
            Naglowek = zawartosc[0];
            kolumny = PobierzNaglowkiIPozycje(Naglowek, KolumnyDoImportu);
            
            Log.Debug($"Nagłowek:{Naglowek.ToJson()}, kolumny:{kolumny.ToJson()}. Liczba lini: {zawartosc.Count}.");
            //usuwamy pierwszy wiesz czyli nagłówek
            zawartosc.RemoveAt(0);

            //szukamy lub dodajemy atrybuty które są używane w tym pliku, nazwy atrybutów są w nagłówku, atrybuty które faktycznie nie bedą używane usuwamu później
            //w słowniku przechowujemy nazwe i id atrybutów które są używane w pliku
            Dictionary<string, int> slownikAtrybutow = new Dictionary<string, int>();
            foreach (var i in kolumny.Values)
            {
                //szukaj lub Dodaj Atrybut 
                Atrybut tmp = ZnajdzAtrybut(i, atrybuty) ?? DodajBrakujacyAtrybut(i, atrybuty);
                if (!slownikAtrybutow.ContainsKey(tmp.Nazwa))
                {
                    slownikAtrybutow.Add(tmp.Nazwa, tmp.Id);
                }
                else
                {
                    throw  new Exception($"Kolumna: {tmp.Nazwa} jest już dodana. W pliku: {SciezkaDoPlikuCsv} musi być dwie kolumny o takiej nazwie. Proszę o poprawienie pliku");
                }
            }

            _cechyKonkurencyjne = new ConcurrentDictionary<long, Cecha>();
            foreach (Cecha cecha in cechy)
            {
                _cechyKonkurencyjne.TryAdd(cecha.Id, cecha);
            }
            _lacznikiKonkurencyjne = new ConcurrentDictionary<long, ProduktCecha>();
            //_cechyKonkurencyjne = new ConcurrentDictionary<long, Cecha>(cechy.ToDictionary(x=>x.Id,x=>x));
            Dictionary<string, long> slownikIdProduktow = StworzSLownikProduktowZKluczemZUstawienia(listaWejsciowa);
            ConcurrentBag<string> pominete = new ConcurrentBag<string>();
       //     Parallel.ForEach(zawartosc, liniaDoOdcz =>
         //   {
         foreach(var liniaDoOdcz in zawartosc) { 
                PrzetworzLinie( kolumny, liniaDoOdcz, slownikDanychProduktowWirtualnych, slownikAtrybutow, pominete, slownikIdProduktow);
                }
         //   });

            if (!pominete.IsEmpty)
            {
                Log.ErrorFormat($"Nie znaleziono produktów o symbolu: [{pominete.ToCsv()}].");
            }

            cechy = _cechyKonkurencyjne.Values.ToList();

            lacznikiCech.AddRange(_lacznikiKonkurencyjne.ToDictionary(x => x.Key, x => x.Value));
            //usuwamy atrybuty które były stworzone z nagłówków ale które faktycznie nie zostały użytę
            var idAtrybutowUzywanych = cechy.Select(x => x.AtrybutId);
            var atrybutyDoUsuniecia = atrybuty.Where(x => !idAtrybutowUzywanych.Contains(x.Id)).ToList();
            foreach (Atrybut atrybut in atrybutyDoUsuniecia)
            {
                atrybuty.Remove(atrybut);
            }

            if (!slownikDanychProduktowWirtualnych.Any())
            {
                Log.DebugFormat("Według ustawionych parametrów nie zostały utworzene żadne produkty wirtualne");
            }

            //dzieki slownikowi nie musiszmy tworzyc warunku ktory bedzie sprawdzal id
            Dictionary<long, Produkt> lista = listaWejsciowa.ToDictionary(x => x.Id, x => x);
            Log.Info($"ilośc elementów w słowniku: {slownikDanychProduktowWirtualnych.Count}");
            Parallel.ForEach(slownikDanychProduktowWirtualnych, produkt =>
            {
                lista[produkt.Key].Abstrakcyjny = true;
                //sortowanie i usuwanie dubli, by zawsze wyliczało taki sam klucz dla pola
                Dictionary<long, Dictionary<object,object>> doSortowania = new Dictionary<long, Dictionary<object, object>>();
                foreach (var wartosci in produkt.Value)
                {
                    if(wartosci== null) continue;
                    var klucz = wartosci.ToCsv().WygenerujIDObiektuSHAWersjaLong();
                    if (!doSortowania.ContainsKey(klucz))
                        doSortowania.Add(klucz, wartosci);
                }
                List<Dictionary<object, object>> dane = doSortowania.OrderBy(x => x.Key).Select(x=>x.Value).ToList();

                lista[produkt.Key].DaneDlaProduktowWirtualnych = dane;
            });

            Log.Info($"Atrybutow:{atrybuty.Count} - Cech: {cechy.Count}");
        }
       
        /// <summary>
        /// Przetwaramy jedną linie pliku
        /// </summary>
        /// <param name="kolumny"></param>
        /// <param name="liniaDoOdcz"></param>
        /// <param name="slownikDanychProduktowWirtualnych"></param>
        /// <param name="slownikAtrybutow"></param>
        /// <param name="pominete"></param>
        /// <param name="slownikIdProduktow"></param>
        private void PrzetworzLinie(Dictionary<int, string> kolumny, string[] liniaDoOdcz, ConcurrentDictionary<long, HashSet<Dictionary<object, object>>> slownikDanychProduktowWirtualnych,Dictionary<string, int> slownikAtrybutow, ConcurrentBag<string> pominete, Dictionary<string, long> slownikIdProduktow)
        {
            string kluczZPlikuCsv = liniaDoOdcz[0];
            if (string.IsNullOrEmpty(kluczZPlikuCsv) || pominete.Contains(kluczZPlikuCsv))
            {
                return;
            }
            //List<Dictionary<object, object>> listaSlownikaParametrow = new List<Dictionary<object, object>>();
            Dictionary<object, object> parametry= new Dictionary<object, object>();
            long idProduktu;
            if (slownikIdProduktow.TryGetValue(kluczZPlikuCsv, out idProduktu))
            {
                HashSet<long> idCech = new HashSet<long>();
                switch (TworzenieParametrow)
                {
                    case SposobTworzeniaParametrow.NazwaZPliku:
                        foreach (var i in kolumny)
                        {
                            if (string.IsNullOrEmpty(liniaDoOdcz[i.Key].Trim()))
                            {
                                continue;
                            }
                            parametry.Add(i.Value, liniaDoOdcz[i.Key].Trim());
                        }
                        break;
                    case SposobTworzeniaParametrow.TworzCechyIAtrybuty:
                        foreach (var i in kolumny)
                        {
                            string nazwaCechy = liniaDoOdcz[i.Key].Trim();
                            if (string.IsNullOrEmpty(nazwaCechy))
                            {
                                continue;
                            }
                            string nazwaAtrybutu = i.Value;
                            string symbolcechy = $"{nazwaAtrybutu}{SeparatorCech}{nazwaCechy}";
                            int idAtrybutu = slownikAtrybutow[nazwaAtrybutu];
                            Cecha cecha = DodajCeche(symbolcechy, nazwaCechy, idAtrybutu);
                            idCech.Add(cecha.Id);
                            parametry.Add(idAtrybutu, cecha.Id);
                        }
                        break;
                    case SposobTworzeniaParametrow.TworzCechyIAtrybutyINadrzedne:
                        HashSet<long> cechyNadrzedne = new HashSet<long>();
                        foreach (var i in kolumny)
                        {
                            string nazwaCechy = liniaDoOdcz[i.Key].Trim();
                            if (string.IsNullOrEmpty(nazwaCechy))
                            {
                                continue;
                            }
                            string nazwaAtrybutu = i.Value;
                            string symbolcechy = $"{nazwaAtrybutu}{SeparatorCech}{nazwaCechy}";

                            int idAtrybutu = slownikAtrybutow[nazwaAtrybutu];

                            Cecha cecha = DodajCeche(symbolcechy, nazwaCechy, idAtrybutu);
                            idCech.Add(cecha.Id);
                            if (!parametry.ContainsKey(idAtrybutu))
                            {
                                parametry.Add(idAtrybutu, cecha.Id);
                            }
                            if (cechyNadrzedne.Any())
                            {
                                //if (cecha.Id == 4030826352844114646)
                                //{
                                //    Log.ErrorFormat($"Log Bartek - doadwanie nadrzednych cech dla cechy id '4030826352844114646': [{cechyNadrzedne.ToCsv()}]. Aktualna zawartość: [{cecha.CechyNadrzedne.ToCsv()}].");
                                //}

                                DodajCecheNadrzedna(cecha, cechyNadrzedne);
                            }
                            if (TrybPokazywaniaFiltrow == TrybPokazywaniaFiltrow.WymuszajSciezke)
                            {
                                cechyNadrzedne = new HashSet<long>{cecha.Id};
                            }
                            else
                            {
                                cechyNadrzedne.Add(cecha.Id);
                            }
                        }
                        break;
                }
                if (parametry.Any())
                {
                    slownikDanychProduktowWirtualnych.AddOrUpdate(idProduktu, new HashSet<Dictionary<object, object>>() {parametry}, (l, set) => DodajDoKolekcji(set, parametry));
                }

                
                //dodajemy łaczniki do cech które utworzylićmy
                foreach (long l in idCech)
                {
                    DodajLacznik(idProduktu, l);
                }
            }
            else
            {
                pominete.Add(kluczZPlikuCsv);             
            }
        }

        private HashSet<Dictionary<object, object>> DodajDoKolekcji(HashSet<Dictionary<object, object>> old, Dictionary<object, object> add)
        {
            if (add == null) return old;
            //sprawdzenie czy old nie jest nullem, jak jest to tworzymy nowy hashset
            HashSet<Dictionary<object, object>> nowe = old ?? new HashSet<Dictionary<object, object>>();
            nowe.Add(add);
            return nowe;
        }

        private void DodajLacznik(long idProduktu, long idCecha)
        {
            ProduktCecha pc = new ProduktCecha(idProduktu, idCecha);
            _lacznikiKonkurencyjne.TryAdd(pc.Id, pc);
        }
        /// <summary>
        /// Dodawanie cech za pomocą słownika konkurencyjnego
        /// </summary>
        /// <param name="symbolCechy"></param>
        /// <param name="nazwaCechy"></param>
        /// <param name="atrybutId"></param>
        /// <returns></returns>
        private Cecha DodajCeche(string symbolCechy, string nazwaCechy, int atrybutId)
        {        
            symbolCechy = symbolCechy.Trim().ToLower();
            long idWyliczoneHash = symbolCechy.WygenerujIDObiektuSHAWersjaLong(1);

            Cecha wynik = null;


            if (_cechyKonkurencyjne.TryGetValue(idWyliczoneHash, out wynik))
            {
                return wynik;
            }

            wynik = new Cecha(nazwaCechy, idWyliczoneHash.ToString(NumberFormatInfo.InvariantInfo))
            {
                Widoczna = true,
                AtrybutId = atrybutId,
                Id = idWyliczoneHash
            };

            _cechyKonkurencyjne.TryAdd(wynik.Id, wynik);
            return wynik;
        }

    }
}
