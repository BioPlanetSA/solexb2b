using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Helpers;
using Xunit;
namespace SolEx.Hurt.Helpers.Tests
{
    public class CSVHelperExtTests
    {
        [Fact()]
        public void OdczytajCSVTest()
        {
            CSVHelperExt csv = new CSVHelperExt();

            string[,] daneTestowe = new string[3,3];
            daneTestowe[0, 0] = "symbol";
            daneTestowe[0, 1] = "cena";
            daneTestowe[0, 2] = "ilosc";

            daneTestowe[1, 0] = "fajny";
            daneTestowe[1, 1] = "20.44";
            daneTestowe[1, 2] = "2";

            daneTestowe[2, 0] = "testowy";
            daneTestowe[2, 1] = "69.66";
            daneTestowe[2, 2] = "10";

            StreamWriter sw = new StreamWriter("CSVTest.csv");

            for (int i = 0; i < 3; i++)
            {
                string linijka = "";
                for (int j = 0; j < 3; j++)
                {
                    linijka += daneTestowe[i, j] + ";";
                }
                sw.WriteLine(linijka.TrimEnd(';'));
            }
            sw.Close();

            string[,] zawartoscCSV = csv.OdczytajCSV("CSVtest.csv");

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Assert.Equal(daneTestowe[i, j], zawartoscCSV[i, j]);
                }
            }
        }
    }
}
