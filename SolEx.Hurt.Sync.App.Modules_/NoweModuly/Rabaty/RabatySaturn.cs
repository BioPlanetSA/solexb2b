using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty
{
    public class RabatySaturn : SyncModul, IModulRabaty, IModulKategorieKlientow, IModulCechyIAtrybuty, IModulProdukty
    {
        
        public override string uwagi
        {
            get { return ""; }
        }


        [FriendlyName("Ścieżka do pliku xls/csv z rabatami")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SciezkaDoPliku { get; set; }

        [FriendlyName("Symbole cech oddzielone średnikiem określające czy dany produkt będzie zawsze widoczny")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string CechyOdkrywajace { get; set; }

        public RabatySaturn()
        {
            SciezkaDoPliku = "";
            CechyOdkrywajace = "";
        }

        public override string Opis
        {
            get { return "Moduł dla Saturna liczący rabaty."; }
        }

        private int wierszCech = 0;
        private int wierszKategorii = 1;
        private int wierszKlientow = 2;
        private int kolumnaKategoriiKlienta = 4;
        private int kolumnaRabatow = 5;
        private int kolumnaGrupy = 3;

        /// <summary>
        /// główna metoda do przetworzenia danych z pliku csv. jest uruchamiana bezpośrednio w zadaniu rabatów i w kilku innych zadaniach przez metodę Przetworz odpowiednią dla danego zadania.
        /// </summary>
        /// <param name="rabatyNaB2B"></param>
        /// <param name="produktyUkryteNaB2B"></param>
        /// <param name="konfekcjaNaB2B"></param>
        /// <param name="kliencib2B"></param>
        /// <param name="produkty"></param>
        /// <param name="ceny"></param>
        /// <param name="cechy"></param>
        /// <param name="cechyProdukty"></param>
        /// <param name="kategorie"></param>
        /// <param name="produktyKategorie"></param>
        /// <param name="kategorieKlientow"></param>
        /// <param name="klienciKategorie"></param>
        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, 
            IDictionary<long, Klient> kliencib2B, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechyPRoduktow, 
            Dictionary<long,ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, 
            ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            if (!CzyMoznaUruchomicModul())
                return;

            if(produktyUkryteNaB2B == null)
                produktyUkryteNaB2B = new List<ProduktUkryty>();


            #region pola obiektów
            //pola obiektów używane do refleksji
            KategoriaKlienta kategorieklientowpola = new KategoriaKlienta();
            var polakatkl = new { grupa = kategorieklientowpola.Grupa, nazwa = kategorieklientowpola.Nazwa };

            KlientKategoriaKlienta kliencikategoriepola = new KlientKategoriaKlienta();
            var polalacznikkatkl = new { kategoria_klientow_id = kliencikategoriepola.KategoriaKlientaId, klient_id = kliencikategoriepola.KlientId };

            Rabat rabatpola = new Rabat();
            var polarabaty = new { aktywny = rabatpola.Aktywny, odKiedy = rabatpola.OdKiedy, doKiedy = rabatpola.DoKiedy, kategoria_klientow_id = rabatpola.KategoriaKlientowId, kategoria_produktow_id = rabatpola.KategoriaProduktowId, cecha_id = rabatpola.CechaId, klient_id = rabatpola.KlientId, produkt_id = rabatpola.ProduktId, rabatpola.WalutaId, rabatpola.TypRabatu };

            ProduktUkryty produktyUkrytePola = new ProduktUkryty();
            var polaproduktyukryte = new { klient_zrodlo_id = produktyUkrytePola.KlientZrodloId, produkt_zrodlo_id = produktyUkrytePola.ProduktZrodloId, produktyUkrytePola.CechaProduktuId, produktyUkrytePola.Tryb };
            #endregion

            Dictionary<string, KategoriaKlienta> kategorieklientowpolaNaB2Bslownik = kategorieKlientow.Values.ZbudojSlownikZKluczemPropertisowym(polakatkl.Properties());
            Dictionary<string, KlientKategoriaKlienta> lacznikkliencikategoriepolaNaB2Bslownik = klienciKategorie.Values.ZbudojSlownikZKluczemPropertisowym(polalacznikkatkl.Properties());

            //wyciągamy cechy zaczynające się na 'rabat'
            HashSet<long> cechyMarkowe = new HashSet<long>( cechyPRoduktow.Where(a => a.Symbol.StartsWith("marka")).Select(a => a.Id) );
            //wyciągamy id produktów z cechami rabatowymi
            HashSet<long> produktyZCechamiMarkowymi= new HashSet<long>( cechyProdukty.Where(a =>  cechyMarkowe.Contains(a.Value.CechaId)).Select(a => a.Value.ProduktId) );

            //wyciągamy wszystkie id produktów które nie są w kolekcji z cechami rabatowymi
            HashSet<long> produktyDostepneDlaWszystkich = new HashSet<long>( produkty.Keys );
            produktyDostepneDlaWszystkich.ExceptWith(produktyZCechamiMarkowymi);

            Cecha cechaOdkrywajaca = StworzCecheOdkrywajaca();

            //jeśli cechy odkrywającej nie było na b2b to zostanie dodana w tej metodzie
            DodajBrakujaceCechyAtrybuty(ref cechyPRoduktow, cechaOdkrywajaca);

            
            Dictionary<string, Rabat> rabatyNaB2Bslownik = rabatyNaB2B.ZbudojSlownikZKluczemPropertisowym(polarabaty.Properties());
            
            Dictionary<string, ProduktUkryty> produktyUkryteSlownik = produktyUkryteNaB2B.ZbudojSlownikZKluczemPropertisowym(polaproduktyukryte.Properties());

            string[] listaCechOdkrywajacych = CechyOdkrywajace.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            //dodawanie produktów dostępnych dla wszystkich do produktów ukrytych - jeśli produkt ma wybraną cechę to będzie widoczny dla wszystkich
            foreach (var produktID in produktyDostepneDlaWszystkich)
            {
                DodajLacznikCechyProdukty(ref cechyProdukty, cechaOdkrywajaca.Id, produktID);
            }


            //pobieranie zawartości pliku csv
            var wierszepliku = PobierzLinijkiZPliku(SciezkaDoPliku);
            string[] kategoriezpliku = wierszepliku[wierszKategorii].Split(';');
            string[] cechyzpliku = wierszepliku[wierszCech].Split(';');
            Dictionary<string, Cecha> cechywgSymbolu = cechyPRoduktow.ToDictionary(x => x.Symbol, x => x);
            //lecimy po wierszach pliku zaczynając od linijki gdzie są klienci. kod leci po każdym wierszu/kliencie i potem po kolumnach ze sposobami dostaw i rabatami dla wybranego sposobu dostawy
            for (int i = wierszKlientow; i < wierszepliku.Count; i++)
            {
                //wyciągamy kolumny dla danego klienta
                string[] kolumnyKlienta = wierszepliku[i].Split(';');
                
                int idKlienta;

                if (!int.TryParse(kolumnyKlienta[0].Trim(), out idKlienta))
                {
                    continue;
                }
                if (! kliencib2B.ContainsKey(idKlienta))
                {
                    continue;
                }
                Klient klient = kliencib2B[idKlienta];
                if (klient.KlientNadrzednyId.HasValue)
                {
                    Log.DebugFormat("Pomijam klienta {0} ma ustawione konto nadrzędne", klient.Nazwa);
                    continue;
                }
                //dodawanie produktów widocznych na podstawie cech - produkty z wybranymi cechami będą widoczne dla klientów z pliku csv (nie dla wszystkich)


                foreach (string s in listaCechOdkrywajacych)
                {
                  
                    if (cechywgSymbolu.ContainsKey(s))
                    {
                        DodajProduktUkryty(ref produktyUkryteSlownik, klient.Id, null, cechywgSymbolu[s].Id, polaproduktyukryte, KatalogKlientaTypy.Dostepne);
                    }
                }
                DodajProduktUkryty(ref produktyUkryteSlownik, klient.Id, null, cechaOdkrywajaca.Id, polaproduktyukryte, KatalogKlientaTypy.Dostepne);

                        //pobieranie kategorii klienta z csv
                        string kategoriaklienta = kolumnyKlienta[kolumnaKategoriiKlienta];

                        //pobieranie grupy klienta z csv
                        string grupa = kolumnyKlienta[kolumnaGrupy].ToUpper();

                        //sprawdzamy czy klient jest przypisany do danej kategorii
                        KategoriaKlienta katkltemp = new  KategoriaKlienta();
                        katkltemp.Nazwa = kategoriaklienta;
                        katkltemp.Grupa = grupa;
                        string kluczkatkltemp = katkltemp.ZbudujKlucz(polakatkl);
                        KategoriaKlienta aktywnaKategoria = kategorieklientowpolaNaB2Bslownik.ContainsKey(kluczkatkltemp) ? kategorieklientowpolaNaB2Bslownik[kluczkatkltemp] : null;
                        if (aktywnaKategoria == null)  //jeśli nie ma takiej kategorii klienta na b2b to jest dodawana nowa
                        {
                            aktywnaKategoria = DodajKategorieKlienta(ref kategorieKlientow, klient.Id, kategoriaklienta, grupa,ref kategorieklientowpolaNaB2Bslownik, polakatkl);
                        }

                        if (aktywnaKategoria != null)
                        {
                            //jeśli klient nie jest w tej kategorii (a plik sugeruje inaczej) to jest do niej dodawany
                            DodajLacznikKategorii(ref klienciKategorie, klient.Id, aktywnaKategoria.Id,ref lacznikkliencikategoriepolaNaB2Bslownik, polalacznikkatkl);
                                //jeśli klient jest przypisany do kategorii z pliku 
                                for (int kolumna = kolumnaRabatow; kolumna < kolumnyKlienta.Length; kolumna++)
                                {

                                    decimal rabat;
                                    TextHelper.PobierzInstancje.SprobojSparsowac(kolumnyKlienta[kolumna].Replace("%", ""), out rabat);

                                    string cecha = cechyzpliku[kolumna].ToLower(); 
                                    string kategoriadostawy = kategoriezpliku[kolumna];

                                    var c = cechywgSymbolu.ContainsKey(cecha)?cechywgSymbolu[cecha]:null;

                                    string symbolwidocznosc = cecha.Replace("rabat_", "marka_");

                                    int idx = cecha.IndexOf('_', 6);
                                    if (idx>0)
                                    {
                                        symbolwidocznosc = symbolwidocznosc.Substring(0, idx);
                                    }
                                    var cw = cechywgSymbolu.ContainsKey(symbolwidocznosc) ? cechywgSymbolu[symbolwidocznosc] : null;
                                    //jeśli istnieje cecha która jest w pierwszej linijce w pliku csv to dodaje widoczny produkt dla danego klienta
                                    if (cw != null)
                                    {
                                        string aktualnaKolumna = kolumnyKlienta[kolumna];
                                        DodajProduktUkryty(ref produktyUkryteSlownik, klient.Id, aktualnaKolumna, cw.Id, polaproduktyukryte);
                                    }

                                    if (string.IsNullOrEmpty(kolumnyKlienta[kolumna]))
                                        continue;

                                    //jeśli kategoria klienta jest taka sama jak sposób dostawy z drugiej linijki to jest dodawana kategoria klienta w grupie DOSTAWA i będzie to wejściowa kategoria klienta potrzebna do wyliczenia rabatu za dostawy w koszyku
                                    //to jest obowiązkowe bo moduł koszykowy RabatySaturn z tego korzysta
                                    if (kategoriezpliku[kolumna] == kategoriaklienta)
                                    {
                                        if (c != null && rabat > 0)
                                        {
                                            //jeśli rabat jest większy od 0 to dla tej cechy produktu jest dodawany rabat dla danego klienta
                                            DodajRabat(ref rabatyNaB2Bslownik, ref rabatyNaB2B, klient.Id, c.Id, rabat, polarabaty);
                                        }
                                        
                                        string nazwa = "Dostawa_" + kategoriaklienta + ":" + rabat;
                                        string grupaklienta = "DOSTAWY";
                                        
                                        //dodawanie kategorii klienta z dostawą
                                        var dostawa = DodajKategorieKlienta(ref kategorieKlientow, klient.Id,
                                            nazwa, grupaklienta, ref kategorieklientowpolaNaB2Bslownik, polakatkl);

                                        DodajLacznikKategorii(ref klienciKategorie, klient.Id, dostawa.Id,
                                           ref lacznikkliencikategoriepolaNaB2Bslownik, polalacznikkatkl);
                                    }

                                    else
                                    {
                                        //jeśli wybrana kategoria klienta nie jest główną dostawą klienta to jest dodawana kategoria klienta która kończy się wartością rabatu - potem moduł RabatySaturn wyciąga tą wartość i dodaje ją do głównego rabatu (z grupy DOSTAWA) w koszyku
                                        string nowakategoriaklienta = cecha + "_" + kategoriadostawy + ":" +
                                                                      rabat;

                                        var kategoriaklientab2b =
                                            kategorieKlientow.Values.FirstOrDefault(
                                                a =>
                                                    a.Nazwa == nowakategoriaklienta &&
                                                    a.Grupa == kolumnyKlienta[kolumnaGrupy]);

                                        if (kategoriaklientab2b == null)
                                        {
                                            string nowagrupa = kolumnyKlienta[kolumnaGrupy];
                                            kategoriaklientab2b = DodajKategorieKlienta(ref kategorieKlientow,klient.Id, nowakategoriaklienta, nowagrupa,ref kategorieklientowpolaNaB2Bslownik, polakatkl);
                                        }

                                        DodajLacznikKategorii(ref klienciKategorie, klient.Id, kategoriaklientab2b.Id,ref lacznikkliencikategoriepolaNaB2Bslownik, polalacznikkatkl);

                                    }
                                }
                        
                }
            }

            produktyUkryteNaB2B = produktyUkryteSlownik.Values.ToList();
            int iloscCechPrzed = cechyProdukty.Count;
            cechyProdukty.Clear();
            Log.InfoFormat("łączników cech przed: {0}, po: {1}", iloscCechPrzed, cechyProdukty.Count);
        }

        /// <summary>
        /// pobiera zawartość pliku csv podanego w ścieżce do listy stringów
        /// </summary>
        /// <param name="sciezka">ścieżka do pliku csv</param>
        /// <returns>lista stringów z linijkami z pliku csv</returns>
        public virtual List<string> PobierzLinijkiZPliku(string sciezka)
        {
            List<string> wierszepliku = new List<string>();
            var plik = File.Open(sciezka, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(plik, Encoding.Default);
            while (!sr.EndOfStream)
            {
                wierszepliku.Add(sr.ReadLine());
            }
            sr.Close();
            plik.Close();
            return wierszepliku;
        }

        private void DodajBrakujaceCechyAtrybuty(ref List<Cecha> cechy, Cecha cechaOdkrywajaca)
        {
            if (cechy.All(a => a.Id != cechaOdkrywajaca.Id))
                cechy.Add(cechaOdkrywajaca);
        }

        private void DodajLacznikCechyProdukty(ref Dictionary<long, ProduktCecha> slownikCechyProdukty , long cechaID, long produktID )
        {
            ProduktCecha nowyLacznik = new ProduktCecha();
            nowyLacznik.ProduktId = produktID;
            nowyLacznik.CechaId = cechaID;


            if (!slownikCechyProdukty.ContainsKey(nowyLacznik.Id))
            {
                slownikCechyProdukty.Add(nowyLacznik.Id, nowyLacznik);
            }
        }

        private ProduktUkryty DodajProduktUkryty(ref Dictionary<string, ProduktUkryty> slownikPU, long idklienta, string aktualnaKolumna, long? idCechy, object pola, KatalogKlientaTypy? tryb = null, int? idProduktu = null)
        {
            ProduktUkryty pu = new ProduktUkryty();
       //     pu.id = 0;

            if (tryb != null)
            {
                pu.Tryb = (KatalogKlientaTypy)tryb;
            }
            else
            {
                if (string.IsNullOrEmpty(aktualnaKolumna))
                {
                    pu.Tryb = KatalogKlientaTypy.Wykluczenia;
                }
                else
                {
                    pu.Tryb = KatalogKlientaTypy.Dostepne;
                }
            }
            pu.CechaProduktuId = idCechy;
            pu.KlientZrodloId = idklienta;
            pu.ProduktZrodloId = idProduktu;
            string klucz = pu.ZbudujKlucz(pola);
            if (!slownikPU.ContainsKey(klucz))
            {
                slownikPU.Add(klucz, pu);
            }
            return pu;

        }

        /// <summary>
        /// tworzy cechę, którą musi mieć produkt jeśli ma być zawsze widoczny dla wszystkich
        /// </summary>
        /// <returns>obiekt cechy</returns>
        private Cecha StworzCecheOdkrywajaca()
        {
            Cecha nowaCecha = new Cecha();
            nowaCecha.Symbol = "produktywidoczne:tak";
            nowaCecha.Nazwa = "TAK";
            nowaCecha.Widoczna = false;
            nowaCecha.Id = nowaCecha.WygenerujIDObiektuSHA(1);
            return nowaCecha;
        }

        private Rabat DodajRabat(ref Dictionary<string, Rabat> slownik,ref List<Rabat> listarabatowerp,  long idklienta, long idCechy, decimal rabat, object pola)
        {
            Rabat r = new Rabat {Aktywny = true, CechaId = idCechy, KlientId = idklienta};
            r.Wartosc1 = r.Wartosc2 = rabat;
            r.Wartosc3 = rabat;
            r.Wartosc4 = rabat;
            r.Wartosc5 = rabat;
            r.TypWartosci = RabatSposob.Procentowy;
            r.TypRabatu = RabatTyp.Zaawansowany;
            string klucz = r.ZbudujKlucz(pola);
            if (!slownik.ContainsKey(klucz))
            {
                slownik.Add(klucz, r);
                listarabatowerp.Add(r);
            }
            return r;

        }

        private KlientKategoriaKlienta DodajLacznikKategorii(ref IDictionary<long, KlientKategoriaKlienta> wszystkielaczniki, long idklienta, int aktywnaKategoria,ref Dictionary<string, KlientKategoriaKlienta> slowniklacznikow, object pola)
        {
            KlientKategoriaKlienta klka = new KlientKategoriaKlienta();
            klka.KategoriaKlientaId = aktywnaKategoria;
            klka.KlientId = idklienta;
        
            string klucz = klka.ZbudujKlucz(pola);

            if (!wszystkielaczniki.ContainsKey(klka.Id) && !slowniklacznikow.ContainsKey(klucz))
            {
                wszystkielaczniki.Add(klka.Id, klka);
                slowniklacznikow.Add(klucz, klka);
            }

            return klka;
        }

        private KategoriaKlienta DodajKategorieKlienta(ref IDictionary<int, KategoriaKlienta> wszystkiekategorie, long idklienta, string nazwa, string grupa,ref Dictionary<string, KategoriaKlienta> slownikKategorii, object pola)
        {
            KategoriaKlienta kat = new KategoriaKlienta();
            kat.Nazwa = nazwa;
            kat.Grupa = grupa.ToUpper();
            kat.PokazujKlientowi = false;
            kat.Id = kat.WygenerujIDObiektuSHA(1);
            string klucz = kat.ZbudujKlucz(pola);
            if (!wszystkiekategorie.ContainsKey(kat.Id) && !slownikKategorii.ContainsKey(klucz))
            {
                wszystkiekategorie.Add(kat.Id, kat);
                slownikKategorii.Add(klucz, kat);
            }
            return kat;
        }

        /// <summary>
        /// na podstawie pliku csv dodaje kategorie klientów i łączniki kategorii klientów. jest to potrzebne żeby kategorie stworzone przez ten moduł w zadaniu rabatów nie zostały usunięte w zadaniu kategorii klientów
        /// </summary>
        /// <param name="kategorie">kategorie klientów z b2b</param>
        /// <param name="laczniki">łączniki kategorii z b2b</param>
        public void Przetworz(ref List<KategoriaKlienta> kategorie, ref List<KlientKategoriaKlienta> laczniki)
        {
            if (CzyMoznaUruchomicModul())
            {
                List<Rabat> rabaty = new List<Rabat>(0);
                List<ProduktUkryty> produktyUkryte = new List<ProduktUkryty>(0);
                Dictionary<long, Konfekcje> konfekcje = new Dictionary<long, Konfekcje>();
                IDictionary<long, Klient> klienci = ApiWywolanie.PobierzKlientow();
                Dictionary<long, Produkt> produkty = new Dictionary<long, Produkt>(0);
                List<PoziomCenowy> ceny = new List<PoziomCenowy>(0);
                List<Cecha> cechy = ApiWywolanie.PobierzCechy().Values.ToList();
                Dictionary<long, ProduktCecha> cechyProdukty = ApiWywolanie.PobierzCechyProdukty();
                Dictionary<long, KategoriaProduktu> kategorieproduktow = new Dictionary<long, KategoriaProduktu>(0);

                List<ProduktKategoria> produktyKategorie = new List<ProduktKategoria>(0);
                IDictionary<int, KategoriaKlienta> kategorieKlientow = kategorie.ToDictionary(a => a.Id, a => a);
                IDictionary<long, KlientKategoriaKlienta> klienciKategorie = laczniki.ToDictionary(a => a.Id, a => a);

                Przetworz(ref rabaty, ref produktyUkryte, ref konfekcje, klienci, produkty, ceny, cechy, cechyProdukty,
                    kategorieproduktow, produktyKategorie, ref kategorieKlientow, ref klienciKategorie);

                kategorie = kategorieKlientow.Values.ToList();
                laczniki = klienciKategorie.Values.ToList();
            }
        }

        /// <summary>
        ///  na podstawie pliku csv dodaje cechy i atrybuty. jest to potrzebne żeby cechy i atrybuty stworzone przez ten moduł w zadaniu rabatów nie zostały usunięte w zadaniu kategorii klientów
        /// </summary>
        /// <param name="atrybuty"></param>
        /// <param name="cechy"></param>
        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            Cecha cechaOdkrywajaca = StworzCecheOdkrywajaca();
            DodajBrakujaceCechyAtrybuty(ref cechy, cechaOdkrywajaca);
        }

        /// <summary>
        /// na podstawie pliku csv dodaje produkty ukryte. jest to potrzebne żeby produkty ukryte stworzone przez ten moduł w zadaniu rabatów zostały usunięte w zadaniu produktów
        /// </summary>
        /// <param name="listaWejsciowa"></param>
        /// <param name="produktyTlumaczenia"></param>
        /// <param name="produktyNaB2B"></param>
        /// <param name="jednostki"></param>
        /// <param name="lacznikiCech"></param>
        /// <param name="lacznikiKategorii"></param>
        /// <param name="provider"></param>
        /// <param name="produktuUkryteErp"></param>
        /// <param name="zamienniki"></param>
        /// <param name="kategorie"></param>
        /// <param name="cechy"></param>
        /// <param name="atrybuty"></param>
        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (CzyMoznaUruchomicModul())
            {
                List<Rabat> rabatyB2B = new List<Rabat>();
                Dictionary<long, Konfekcje> konfekcje = new Dictionary<long, Konfekcje>();
                IDictionary<long, Klient> dlakogoliczyc = ApiWywolanie.PobierzKlientow();
                //List<Cecha> cechyB2B = ApiWywolanie.PobierzCechy().Values.ToList();
                IDictionary<int, KategoriaKlienta> kategorieklientow =
                    ApiWywolanie.PobierzKategorieKlientow();
                IDictionary<long, KlientKategoriaKlienta> kategorieklientowlaczniki =
                    ApiWywolanie.PobierzKlienciKategorie(new Dictionary<string, object>());
                //var kategorie = ApiWywolanie.PobierzKategorie(new KategorieSearchCriteria());
                Przetworz(ref rabatyB2B, ref produktuUkryteErp, ref konfekcje, dlakogoliczyc, produktyNaB2B,
                    new List<PoziomCenowy>(), cechy, lacznikiCech, kategorie, lacznikiKategorii,
                    ref kategorieklientow,
                    ref kategorieklientowlaczniki);
            }
        }
        /// <summary>
        /// sprawdza czy moduł może być uruchomiony żeby nie potrzebnie nie pobierało dodatkowych danych z b2b jeśli np ścieżka do pliku csv jest pusta lub taki plik nie istnieje
        /// </summary>
        /// <returns></returns>
        public virtual bool CzyMoznaUruchomicModul()
        {
            if (string.IsNullOrEmpty(SciezkaDoPliku))
            {
                Log.Error(
                    "Ścieżka do pliku jest pusta! Musisz podać ścieżkę do importowanego pliku przed włączeniem automatu.");
                return false;
            }

            if (!File.Exists(SciezkaDoPliku))
            {
                Log.ErrorFormat(
                    "Plik {0} nie istnieje!.", SciezkaDoPliku);
                return false;
            }
            return true;
        }
    }
}
