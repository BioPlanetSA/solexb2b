using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Helpers
{
    /// <summary>
    /// Klasa nie jest statyczna bo w przyszłości chciałbym tutaj dodać jezscze jakieś bajery do obsługi csv ale żeby były dostępne tylko dla danego pliku
    /// </summary>
    public class CSVHelperExt
    {
        /// <summary>
        /// Wrapper do biblioteki do obsługi CSV. Sama biblioteka nie potrafiła się poruszać do tyłu po pliku co utrudnia obsługe csv
        /// </summary>
        /// <param name="sciezka">Ścieżka do pliku CSV</param>
        /// <returns>Zwraca dwuwymiarową tablicę stringów z zawartością pliku CSV razem z nagłówkami</returns>
        public string[,] OdczytajCSV(string sciezka)
        {
            CsvReader r = PobierzReadera(sciezka);
            
            int liczbaWierszy = 1;
            r.Read();
            r.ReadHeader();
            int liczbaKolumn = r.FieldHeaders.Length;
            while (r.Read())
            {
                liczbaWierszy++;
            }

            r.Dispose();
            r = PobierzReadera(sciezka);
            string[,] zawartoscCSV = new string[liczbaWierszy+1,liczbaKolumn];

            bool sanaglowki = false;
            int aktualnaKolumna = 0;
            int aktualnyWiersz = 0;
            while (r.Read())
            {
                if (!sanaglowki)
                {
                    for (int i = 0; i < r.FieldHeaders.Length; i++)
                    {
                        zawartoscCSV[aktualnyWiersz, aktualnaKolumna++] = r.FieldHeaders[i];
                    }
                    sanaglowki = true;
                    aktualnaKolumna = 0;
                    aktualnyWiersz++;
                }

                while (aktualnaKolumna < liczbaKolumn)
                {
                    zawartoscCSV[aktualnyWiersz,aktualnaKolumna] = r.GetField(aktualnaKolumna++);
                }
                aktualnaKolumna = 0;
                aktualnyWiersz++;
            }
            r.Dispose();

            return zawartoscCSV;
        }

        private CsvReader PobierzReadera(string sciezka)
        {
            FileStream fs=new FileStream(sciezka,FileMode.Open,FileAccess.Read,FileShare.Read);
            CsvReader r = new CsvReader(new StreamReader(fs, Encoding.Default));
            r.Configuration.Delimiter = ";";
            r.Configuration.Encoding = Encoding.Default;
            r.Configuration.TrimFields = true;
            r.Configuration.HasHeaderRecord = true;
            return r;
        }

        public StringWriter WygenerujCsvDlaListyObiektow(List<object> listaObiektow, bool dodawacNaglowek=true)
        {
            StringWriter zawartosc = new StringWriter();
            var config = new CsvConfiguration()
            {
                AllowComments = false,
                CultureInfo = CultureInfo.InvariantCulture,
                Delimiter = ";",
                HasHeaderRecord = dodawacNaglowek,
                TrimFields = true,
                Encoding = Encoding.Unicode,
                //zakomentowane powodwało problem zamiast 0 wtawiało się ="0"
                //UseExcelLeadingZerosFormatForNumerics = true
            };
            CsvWriter csv = new CsvWriter(zawartosc, config);

            

            csv.WriteRecords(listaObiektow);
            return zawartosc;
        }


        public HashSet<int> KolumnyImportowane(string[] dostepne, string kolumnyDoImportu)
        {
            HashSet<int> idKolumnDoImportu = new HashSet<int>();
            string[] tablicaKolumn = kolumnyDoImportu.ToLower().Split(';');
            for (int i = 1; i < dostepne.Length; i++)
            {
                if (tablicaKolumn.Contains(dostepne[i].ToLower()))
                {
                    idKolumnDoImportu.Add(i);
                }
            }
            return idKolumnDoImportu;
        }

        /// <summary>
        /// Odczytanie pliku tekstowego
        /// </summary>
        /// <param name="sciezka">scieżka do pliku</param>
        /// <returns></returns>
        public virtual TextReader OtworzPlik(string sciezka)
        {
            Encoding encoding;
            return OtworzPlik(sciezka, out encoding);
        }

        /// <summary>
        /// Odczytanie plik tekstowy wraz z rozpoznaniem kodowania 
        /// </summary>
        /// <param name="sciezka">scieżka do pliku</param>
        /// <param name="encoding">kodowanie</param>
        /// <returns></returns>
        public virtual TextReader OtworzPlik(string sciezka, out Encoding encoding)
        {
            if (!File.Exists(sciezka))
            {
                throw new FileNotFoundException($"Nie znaleziono pliku: {sciezka}{Directory.GetCurrentDirectory()}");
            }
            var text = Tools.PobierzInstancje.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezka,out encoding, Opisy.KodowanieOpisow.Dopasuj);
            return new StringReader(text);
            //return new StreamReader(new FileStream(sciezka, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Default);
        }

        /// <summary>
        /// Tworzymy CSV readera ze strumienia z domyślnym kodowaniem.
        /// </summary>
        /// <param name="textReader">strumień</param>
        /// <returns></returns>
        public CsvReader StworzCsvReaderZKonfiguracja(TextReader textReader)
        {
            Encoding encoding = Encoding.Default;
            return StworzCsvReaderZKonfiguracja(textReader, encoding);
        }

        /// <summary>
        /// Tworzymy CSV readera ze strumienia z wybranym kodowaniem.
        /// </summary>
        /// <param name="textReader">strumień</param>
        /// <param name="encoding">kodowanie</param>
        /// <returns></returns>
        public CsvReader StworzCsvReaderZKonfiguracja(TextReader textReader, Encoding encoding)
        {
            CsvReader reader = new CsvReader(textReader);
            reader.Configuration.Delimiter = ";";
            reader.Configuration.Encoding = encoding;
            reader.Configuration.TrimFields = true;
            reader.Configuration.HasHeaderRecord = true;
            return reader;
        }





    }
}
