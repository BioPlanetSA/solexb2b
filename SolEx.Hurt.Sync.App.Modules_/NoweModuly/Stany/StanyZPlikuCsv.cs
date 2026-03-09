using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CsvHelper;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany
{
    [FriendlyName("Pobieranie stanów z pliku CSV", FriendlyOpis = "")]
    public class StanyZPlikuCsv : StanyBaza
    {
        public StanyZPlikuCsv()
        {
            Separator = ";";
            LiczbaWierszyDoPominiecia = 1;
        }
        [FriendlyName("Ścieżka do pliku ze stanami", FriendlyOpis = "Jeśli plik ma być pobrany z internetu to musi zaczynać się od http:// lub https://. Aby pobrać plik który wymaga autoryzacji należy korzystać z narzędzia wget")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SciezkaDoPliku { get; set; }

        [FriendlyName("Separator")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }

        [FriendlyName("Liczba wierszy do pominięcia")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int LiczbaWierszyDoPominiecia { get; set; }

        [FriendlyName("Numer kolumny (licząc od 1) z identyfikatorem produktu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int NrKolumnyZIdentyfikatorem { get; set; }

        [FriendlyName("Numer kolumny (licząc od 1) ze stanem")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int NrKolumnyZeStanem { get; set; }

        [FriendlyName("Zastępuj pobrane frazy na stany liczbowe", FriendlyOpis = "Jeśli w pliku zamiast konkretnych liczb są frazy np. dużo, mało, możesz ustawić tu jak zastępować te frazy na liczby np.: 'dużo:10;mało:4;brak:0'")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Niewymagane]
        public string SlownikDoZmiany { get; set; }

        public override void Przetworz(ref Dictionary<int, List<ProduktStan>> listaWejsciowa, List<Magazyn> magazyny, List<Produkt> produktyB2B)
        {
            if (string.IsNullOrEmpty(SciezkaDoPliku))
            {
                Log.Error("Ścieżka do pliku jest pusta. Moduł zakończy działanie");
                return;
            }
            //Sprawdzamy czy w slowniku jest taki magazyn
            if (!listaWejsciowa.ContainsKey(IdMagazynu))
            {
                listaWejsciowa.Add(IdMagazynu, new List<ProduktStan>());
            }

            string zawartosc = PobierzZawartoscPliku(SciezkaDoPliku);
            var slownikProduktow = SlownikProduktow(produktyB2B);

            //Pobieramy stany produktów z pliku, klucz to id produktu wartość to stan
            Dictionary<long, decimal> stany = PobierzStanyProduktu(zawartosc, slownikProduktow);

            //Aktualizujemy liste wejsciową o stany które pobraliśmy z pliku
            ZaktualizujListeStanowOStanyZPliku(stany, ref listaWejsciowa, produktyB2B);

        }

        /// <summary>
        /// pobieramy zawartość pliku ze stanami
        /// </summary>
        /// <param name="sciezka"></param>
        /// <returns></returns>
        public string PobierzZawartoscPliku(string sciezka)
        {
            string zawartosc = string.Empty;

            if (Hurt.Helpers.Tools.PobierzInstancje.CzyPlikJestLokalny(sciezka) && File.Exists(sciezka))
                zawartosc = File.ReadAllText(sciezka);

            else
            {
                using (var webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    zawartosc = webClient.DownloadString(string.Format(SciezkaDoPliku));
                }
            }
            return zawartosc;
        }

        /// <summary>
        /// Pobieramy stany z pliku
        /// </summary>
        /// <param name="zawartoscpliku"></param>
        /// <param name="slownikProduktow"></param>
        /// <returns></returns>
        public Dictionary<long, decimal> PobierzStanyProduktu(string zawartoscpliku, Dictionary<string, long> slownikProduktow)
        {
            Dictionary<long, decimal> stany = new Dictionary<long, decimal>();
            int nrIdentyfikatora = NrKolumnyZIdentyfikatorem-1;
            int nrStanu = NrKolumnyZeStanem - 1;

            CsvReader csv = new CsvReader(new StringReader(zawartoscpliku));
            csv.Configuration.Delimiter = Separator;
            csv.Configuration.HasHeaderRecord = LiczbaWierszyDoPominiecia>0;
            int doPominiecia = LiczbaWierszyDoPominiecia>0?LiczbaWierszyDoPominiecia--:0;
            while (csv.Read()) //petla wyciagajaca naglowki kolumna
            {
                if (doPominiecia > 0)
                {
                    doPominiecia--;
                    continue;
                }
                string kod = csv[nrIdentyfikatora].Trim();
                string stan = csv[nrStanu].Trim();
                if (string.IsNullOrEmpty(stan)) continue;

                decimal stanWPliku;
                if (string.IsNullOrEmpty(kod)) continue;
                if (!TextHelper.PobierzInstancje.SprobojSparsowac(stan, out stanWPliku))
                {
                //jesli nie mogliśmy sparsować stanu do liczby a mamy słownik do zmiam próbujemy zamienić slówo na wartosć liczbową zgodnie ze słownikiwm 
                    if (!string.IsNullOrEmpty(SlownikDoZmiany))
                    {
                        decimal? stanTmp = ZamienSlowoNaWartosc(stan);
                        if(stanTmp== null) continue;
                        stanWPliku = stanTmp.Value;
                    }
                    else
                    {
                        continue;
                    }
                }
                long idProduktu;
                if (!slownikProduktow.TryGetValue(kod, out idProduktu)) continue;

                if (!stany.ContainsKey(idProduktu))
                {
                    stany.Add(idProduktu, stanWPliku);
                }
            }
            return stany;
        }

        /// <summary>
        /// Zamienia wartość textową na liczbę zgodnie z ustawieniem SlownikDoZmiany
        /// </summary>
        /// <param name="wartosc"></param>
        /// <returns></returns>
        public decimal? ZamienSlowoNaWartosc(string wartosc)
        {
            if (slownikFraz == null)
            {
                slownikFraz = new Dictionary<string, decimal>();
                PrzygotujSlownik();
            }

            decimal wynik;
            if (!slownikFraz.TryGetValue(wartosc, out wynik))
            {
                Log.Error($"Brak wartości:{wartosc} w słowniku. Sprawdź poprawności danych. Słownik:{SlownikDoZmiany}");
                return null;
            }
            return wynik;
        }

        protected Dictionary<string, decimal> slownikFraz;
        /// <summary>
        /// Przygotowywuje słownik z frazami do zmiany
        /// </summary>
        protected void PrzygotujSlownik()
        {
            slownikFraz = new Dictionary<string, decimal>(StringComparer.InvariantCultureIgnoreCase);
            var regula = SlownikDoZmiany.Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in regula)
            {
                var wartosci = s.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                decimal stan;
                if (decimal.TryParse(wartosci[1], out stan))
                {
                    if (slownikFraz.ContainsKey(wartosci[0]))
                    {
                        slownikFraz[wartosci[0]] = stan;
                    }
                    else
                    {
                        slownikFraz.Add(wartosci[0], stan);
                    }
                }
                else
                {
                    throw new Exception($"Bład konwertowania wartości:{wartosci[1]} do wartości liczbowej. Reguła:{s}");
                }
            }
        }
    }
}
