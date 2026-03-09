using System;
using System.Collections.Generic;
using System.IO;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class SprawdzKatalogZdjec : TestKonfiguracjiBaza
    {
        public override string Opis
        {
            get { return "Test dostêpu do katalogu zdjec"; }
        }

        /// <summary>
        /// Test sprawdzaj¹cy czy jest dostêp do katalogu do przechowywania zdjêæ produktów
        /// </summary>
        /// <returns></returns>
        public override List<string> Test()
        {
            string sciezka = "zasoby/import/";
            return sprawdzKatalog(sciezka);
        }

        public List<string> sprawdzKatalog(string sciezka)
        {
            List<string> listaBledow = new List<string>();
            string docelowa = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sciezka);
            if (!Directory.Exists(docelowa))
            {
                listaBledow.Add(string.Format("Katalog {0} nie istnieje", docelowa));
            }
            string pliksciezka = Path.Combine(docelowa, "test.txt");
            try
            {
                File.WriteAllText(pliksciezka, "testowa zawartosc");
            }
            catch (Exception)
            {
                listaBledow.Add(string.Format("W katalogu {0} nie mo¿na utworzyæ pliku", docelowa));
            }
            try
            {
                File.Delete(pliksciezka);
            }
            catch (Exception)
            {
                listaBledow.Add(string.Format("Z katalogu {0} nie mo¿na usun¹æ pliku", docelowa));
            }
            return listaBledow;
        }
    }
}