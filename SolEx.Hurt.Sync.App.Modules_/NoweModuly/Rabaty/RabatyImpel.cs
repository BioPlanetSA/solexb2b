using System.Text.RegularExpressions;
using CsvHelper;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Enums;

using SolEx.Hurt.Model.Helpers;
using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty
{
    public class RabatyImpel : SyncModul, Model.Interfaces.SyncModuly.IModulRabaty
    {

        public override string uwagi
        {
            get { return ""; }
        }

        public RabatyImpel()
        {
            Sciezka = "";
            KolumnaZCena = "Ceny Katalogowe";
            KolumnaPrzedPierwszymKlientem = "VAT";
        }

        [FriendlyName("Ścieżka do pliku CSV z rabatami")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Sciezka { get; set; }

        [FriendlyName("Nazwa kolumny przed pierwszym klientem")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string KolumnaPrzedPierwszymKlientem { get; set; }

        [FriendlyName("Nazwa kolumny z ceną netto towaru")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string KolumnaZCena { get; set; }

        Regex regex = new Regex(@"[^0-9\,.]+", RegexOptions.Compiled);

        public override string Opis
        {
            get { return "Importuje rabaty z pliku CSV Impela."; }
        }

        #region kolumny produktu

        private int kKod = 0;

        #endregion

        private int kKlienciPoczatek = 16;

        private void ObsluzKomorke(long idklienta, long idproduktu, string reg, ref Dictionary<long, Rabat> wszystkieRabaty, ref Dictionary<string, ProduktUkryty> produktyUkryte, ref Dictionary<long, Konfekcje> listaKonfekcji, decimal domyslnaCenaProduktu, object pola,
            string poprzedniaWartosc)
        {
            decimal wr = 0;

            bool sparsowano = false;
            try
            {
                sparsowano = decimalParse(PobierzLiczbePrzezRegex(reg), out wr);
            }
            catch
            {
                Log.Error("błąd prasowania wartości: " + reg);
            }

            if (reg.Contains(":") || reg.Contains("^"))
            {
                string[] konfekcja = reg.Split(':', '^');
                decimal rabat1;
                decimal ilosc;
                bool wartoscProcent = konfekcja[1].EndsWith("%");

                if (wartoscProcent)
                    konfekcja[1] = konfekcja[1].Replace("%", "");

                if (decimalParse(PobierzLiczbePrzezRegex(konfekcja[1]).Trim(), out rabat1) &&
                    decimalParse(PobierzLiczbePrzezRegex(konfekcja[0]).Trim(), out ilosc))
                {
                    //wyjątek dla sytuacji 20%;24^ 25% gdzie domyślnie klient ma 20% rabaty ale przy 24 sztukach ma mieć 25% ale od ceny głównej a nie po rabacie 20%
                    if (poprzedniaWartosc.Contains("%"))
                    {
                        rabat1 = Kwoty.WyliczWartosc(domyslnaCenaProduktu, rabat1, RabatSposob.Procentowy);
                        //rabat1 = domyslnaCenaProduktu - nowyrabat;
                        wartoscProcent = false;
                    }

                    dodajKonfekcje(idklienta, idproduktu, ref listaKonfekcji, ilosc, rabat1, wartoscProcent);
                    dodajProduktUkryty(idklienta, idproduktu, ref produktyUkryte, KatalogKlientaTypy.Dostepne, pola);
                    sparsowano = false;
                }
            }
            else if (reg.Trim().EndsWith("%"))
            {
                if (sparsowano && wr > 0)
                {
                    dodajRabat(idklienta, idproduktu, wr, ref wszystkieRabaty, RabatSposob.Procentowy);
                }
            }

            else if (wr > 0)
            {
                dodajRabat(idklienta, idproduktu, wr, ref wszystkieRabaty, RabatSposob.StalaCena);
            }

            if (sparsowano)
                dodajProduktUkryty(idklienta, idproduktu, ref produktyUkryte, KatalogKlientaTypy.Dostepne, pola);

            if (reg == "koszyk")
            {
                dodajProduktUkryty(idklienta, idproduktu, ref produktyUkryte, KatalogKlientaTypy.MojKatalog, pola);
            }
        }

        private static bool decimalParse(string wartoscrabat, out decimal wr)
        {

            bool wynik =
                TextHelper.PobierzInstancje.SprobojSparsowac(
                    wartoscrabat.Replace("%", "").Replace(",", ".").Replace(" ", ""), out wr);
            if (!wynik)
            {
                //  Log.Error("Nie udało się poprawnie sparsować wartości " + wartoscrabat);
            }
            return wynik;
        }

        private void dodajKonfekcje(long idklienta, long idproduktu, ref Dictionary<long, Konfekcje> konfekcje,
            decimal ilosc, decimal rabat, bool wartoscProcent)
        {
            //Log.Debug(string.Format("Dodawanie konfekcji dla klienta {0} na produkt {1}", idklienta, idproduktu));
            Konfekcje konfekcja = new Konfekcje();
            konfekcja.WyliczonePrzezModul = this.Id;
            konfekcja.Ilosc = ilosc;
            konfekcja.KlientId = idklienta;
            konfekcja.ProduktId = idproduktu;
            decimal wartoscRabatu = decimal.Round(rabat, 2);
            if (wartoscProcent)
                konfekcja.Rabat = wartoscRabatu;

            else konfekcja.RabatKwota = wartoscRabatu;

            if (!konfekcje.ContainsKey(konfekcja.Id))
                konfekcje.Add(konfekcja.Id, konfekcja);

            else
                Log.ErrorFormat("Zdublowany klucz konfekcji dla klienta o ID {0}  i produktu o ID {1}, klucz: {2}",
                    idklienta, idproduktu, konfekcja.Id);
        }

        private static void dodajProduktUkryty(long idklienta, long produktid,
            ref Dictionary<string, ProduktUkryty> produktyUkryte, KatalogKlientaTypy tryb, object pola)
        {
            ProduktUkryty pu = new ProduktUkryty();
            pu.Tryb = tryb;
            //Log.Debug(string.Format("Dodawanie produktu ukrytego dla klienta {0} na produkt {1} o typie:{2}", idklienta, produktid, pu.Tryb));
            pu.KlientZrodloId = idklienta;
            pu.ProduktZrodloId = produktid;
            //pu.id = 0;
            //pu.id = pu.WygenerujIDObiektuSHAWersjaLong();
            string klucz = pu.ZbudujKlucz(pola);

            if (!produktyUkryte.ContainsKey(klucz))
                produktyUkryte.Add(klucz, pu);
        }

        private static void dodajRabat(long idklienta, long idproduktu, decimal wr,
            ref Dictionary<long, Rabat> wszystkieRabaty, RabatSposob sposob)
        {
            Rabat rabat = new Rabat();
            rabat.KlientId = idklienta;
            rabat.ProduktId = idproduktu;
            rabat.TypWartosci = sposob;
            rabat.TypRabatu = RabatTyp.Zaawansowany;
            wr = decimal.Round(wr, 2);
            rabat.Wartosc4 = rabat.Wartosc5 = rabat.Wartosc1 = rabat.Wartosc2 = rabat.Wartosc3 = wr;
            if (!wszystkieRabaty.ContainsKey(rabat.Id))
                wszystkieRabaty.Add(rabat.Id, rabat);
        }

        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B,ref Dictionary<long,Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B, Dictionary<long, Produkt> produkty,List<PoziomCenowy> ceny, List<Cecha> cechy, Dictionary<long, ProduktCecha> cechyProdukty,
            Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie,
            ref IDictionary<int, KategoriaKlienta> kategorieKlientow,
            ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            if (!File.Exists(Sciezka))
            {
                Log.Error("Nie znaleziono pliku CSV w podanej ścieżce");
                return;
            }
            Log.Debug("Uruchamianie modułu ImportCSVImpelRabaty");

            if (produktyUkryteNaB2B == null)
                produktyUkryteNaB2B = new List<ProduktUkryty>(); // new Dictionary<long, produkty_ukryte>();

            ProduktUkryty pu = new ProduktUkryty();
            object polaProduktyUkryte =
                new {klient_zrodlo_id = pu.KlientZrodloId, produkt_zrodlo_id = pu.ProduktZrodloId, pu.Tryb};

            CsvReader r = new CsvReader(new StreamReader(Sciezka, Encoding.ASCII)); // GetEncoding())
            Log.Debug("Załadowano plik " + Sciezka);
            r.Configuration.Delimiter = ";";
            r.Configuration.Encoding = Encoding.Default;

            r.Configuration.TrimFields = true;
            r.Configuration.HasHeaderRecord = true;

            Dictionary<long, Rabat> wszystkieRabaty = new Dictionary<long, Rabat>();
            Dictionary<string, ProduktUkryty> produktyUkryte = new Dictionary<string, ProduktUkryty>();
            Dictionary<long, Konfekcje> listaKonfekcji = new Dictionary<long, Konfekcje>();

            foreach (Konfekcje konfekcje in konfekcjaNaB2B.Values)
            {
               if (!listaKonfekcji.ContainsKey(konfekcje.Id))
                    listaKonfekcji.Add(konfekcje.Id, konfekcje);
            }

            //foreach (var produktukryty in produktyUkryteNaB2B.Values)
            //{
            //    string klucz = produktukryty.ZbudujKlucz(polaProduktyUkryte);

            //    if (!produktyUkryte.ContainsKey(klucz))
            //        produktyUkryte.Add(klucz, produktukryty);
            //}

            //do sprawdzania dubli
            List<long> idPrzetworzonychKlientow = new List<long>();
            Dictionary<long, int> idPrzetworzonychProduktow = new Dictionary<long, int>();
            HashSet<int> linieDoPominiecia = new HashSet<int>();

            int? i = null;
            int? max = null;
            bool oczyscNaglowki = true;
            int pierwszakolumna = 0;
            while (true)
            {
                Klient k = null;

                if (r.Read())
                {
                    if (oczyscNaglowki)
                    {
                        r.ReadHeader();
                        for (int hi = 0; hi < r.FieldHeaders.Count(); hi++)
                        {
                            r.FieldHeaders[hi] = r.FieldHeaders[hi].Trim();
                        }
                        oczyscNaglowki = false;
                    }


                    if (!i.HasValue)
                    {
                        i = r.FieldHeaders.ToList().IndexOf(KolumnaPrzedPierwszymKlientem) + 1;
                        pierwszakolumna = i.Value - 1;
                    }

                    if (!max.HasValue)
                        max = r.FieldHeaders.Count();

                    LogiFormatki.PobierzInstancje.LogujInfo(string.Format("Klient {1} z {0}", (max - pierwszakolumna),
                        (i - pierwszakolumna)));

                    if (i.Value >= max)
                        break;


                    string klient = "";
                    try
                    {
                        // Log.Debug("Próba odczytania klienta z kolumny nr " + i);
                        klient = r.GetField(i.Value);
                    }
                        //jeśli wywali się w tym miejscu tzn że index jest za duży (koniec klientów) więc można IleMinutCzekacDoKolejnegoUruchomieniać pętle
                        //nie miałem innego pomysłu jak wyciągnąć max kolumn nie tworząc innego NORMALNEGO obiektu
                    catch (CsvReaderException exception)
                    {
                        Log.ErrorFormat( "Nie udało się zładować klienta z kolumny nr {0}. Najprawdopodobniej kolumna nie istnieje. Błąd: {1}",
                            (kKlienciPoczatek + i), exception.Message);
                        break;
                    }

                    if (string.IsNullOrEmpty(klient) && r.FieldHeaders[i.Value].Contains("@"))
                    {
                        string mail = r.FieldHeaders[i.Value];
                        k = kliencib2B.Values.FirstOrDefault(a => a.Email == mail);
                    }

                    if (k == null && !string.IsNullOrEmpty(klient))
                    {
                        int idklienta = 0;
                        if (int.TryParse(klient, out idklienta))
                        {
                            k = kliencib2B.Values.FirstOrDefault(a => a.Id == idklienta);
                        }
                    }
                }

                //obojętnie czy klient jest czy nie to odczytujemy kolejną linijkę
                if (k == null)
                {
                    string klient = r.FieldHeaders[i.Value];
                    if (!string.IsNullOrEmpty(klient))
                    {
                        k = kliencib2B.Values.FirstOrDefault(a => a.Email == klient);
                    }
                }

                if (k != null) // && k.klient_id == 10306448)
                {
                    if (!idPrzetworzonychKlientow.Contains(k.Id))
                    {
                        // Log.Debug("Znaleziono klienta  " + k.nazwa);
                        while (r.Read())
                        {
                            int linia = r.Row + 2;
                            if (linieDoPominiecia.Contains(linia))
                                continue;

                            string kod = r.GetField(kKod);
                            Produkt p = produkty.Values.FirstOrDefault(a => a.Kod == kod);
                            if (p == null)
                            {
                                // Log.Debug("Nie znaleziono produktu o symbolu " + kod);
                                continue;
                            }

                            if (!idPrzetworzonychProduktow.ContainsKey(p.Id))
                            {
                                idPrzetworzonychProduktow.Add(p.Id, linia);
                            }
                            else
                            {
                                LogiFormatki.PobierzInstancje.LogujInfo(
                                    string.Format(
                                        "Znaleziono zdublowany produkt o ID {0} w wierszu {1}, produkt znajduje się w pierwszej kolejności w wierszu {2}",
                                        p.Id, linia, idPrzetworzonychProduktow[p.Id]));

                                linieDoPominiecia.Add(linia);
                                continue;
                            }

                            string scena = r.GetField(KolumnaZCena);
                            decimal cena = 0;
                            decimalParse(scena, out cena);

                            string wartoscrabat = r.GetField(i.Value);

                            PrzetworzPobranaWartosc(wartoscrabat, k, p, wszystkieRabaty, produktyUkryte, listaKonfekcji,
                                cena, polaProduktyUkryte);
                        }

                        idPrzetworzonychKlientow.Add(k.Id);
                    }
                    else
                    {
                        LogiFormatki.PobierzInstancje.LogujInfo(string.Format("Klient o ID {0} jest zdublowany", k.Id));
                    }
                }
                idPrzetworzonychProduktow = new Dictionary<long, int>();
                r = new CsvReader(new StreamReader(Sciezka));
                r.Configuration.Delimiter = ";";
                r.Configuration.Encoding = Encoding.Default;
                r.Configuration.TrimFields = true;
                r.Configuration.HasHeaderRecord = true;
                oczyscNaglowki = true;
                i++;
            }


            //foreach (var rabat in wszystkieRabaty.Values )
            //{
            //    ////czy juz jest dubel
            //    //if (rabatyNaB2B.Any(x => x.klient_id == rabat.klient_id && x.produkt_id==))


            //    //    rabat.klient_id = idklienta;
            //    //rabat.TypWartosci = sposob;
            //    //wr = decimal.Round(wr, 2);
            //    ////Log.Debug(string.Format("Dodawanie rabatu dla klienta od ID {0} na produkt {1}", rabat.klient_id, idproduktu));
            //    //rabat.wartosc4 = rabat.wartosc5 = rabat.wartosc1 = rabat.wartosc2 = rabat.wartosc3 = wr;

            //    //rabat.produkt_id = idproduktu;
            //    //rabat.rabat_id = rabat.WygenerujIDObiektu();

            //    rabatyNaB2B.Add(rabat);
            //}
            rabatyNaB2B.AddRange(wszystkieRabaty.Values);
            konfekcjaNaB2B.Clear();
            foreach (var konfekcje in listaKonfekcji)
            {

                konfekcjaNaB2B.Add(konfekcje.Key,konfekcje.Value);
            }
            produktyUkryteNaB2B.Clear();
            //foreach (var produktukryty in produktyUkryte.Values)
            //{
            //    if (!produktyUkryteNaB2B.ContainsKey(produktukryty.id))
            //        produktyUkryteNaB2B.Add(produktukryty.id, produktukryty);

            //    else
            //    {
            //        string klucz1 = produktyUkryteNaB2B[produktukryty.id].ZbudujKlucz(polaProduktyUkryte);
            //        string klucz2 = produktukryty.ZbudujKlucz(polaProduktyUkryte);

            //        Log.ErrorFormat("Zdublowane klucze o id {0}", produktukryty.id);
            //        Log.ErrorFormat("Klucz 1: {0}", klucz1);
            //        Log.ErrorFormat("Klucz 2: {0}", klucz2);
            //    }
            //}

            Log.Debug("zakończenie pracy modułu ImportCSVImpelRabaty");
        }

        public string PobierzLiczbePrzezRegex(string dane)
        {
            return regex.Split(dane).Where(c => c != "." && c.Trim() != "").FirstOrDefault();
        }

        public void PrzetworzPobranaWartosc(string wartoscrabat, Klient k, Produkt p,
            Dictionary<long, Rabat> wszystkieRabaty,
            Dictionary<string, ProduktUkryty> produktyUkryte, Dictionary<long, Konfekcje> listaKonfekcji, decimal cena,
            object pola)
        {
            string[] reguly = wartoscrabat.Split(new string[] {";"},
                StringSplitOptions.RemoveEmptyEntries);
            string poprzedniaWartosc = string.Empty;
            if (!string.IsNullOrEmpty(wartoscrabat))
            {
                foreach (string reg in reguly)
                {
                    ObsluzKomorke(k.Id, p.Id, reg, ref wszystkieRabaty,
                        ref produktyUkryte,
                        ref listaKonfekcji, cena, pola, poprzedniaWartosc);

                    poprzedniaWartosc = reg;
                }
            }
        }
    }
}
