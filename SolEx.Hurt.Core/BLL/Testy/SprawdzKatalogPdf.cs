using System;
using System.Collections.Generic;
using System.IO;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class SprawdzKatalogPdf : TestKonfiguracjiBaza
    {
        public override string Opis
        {
            get { return "Test dostêpu do katalogu pdf"; }
        }

        /// <summary>
        /// Test dostêpu do katalogu
        /// </summary>
        public override List<string> Test()
        {
            string sciezka = "sfera/dokumenty/";
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