using System;
using System.Collections.Generic;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Stany
{
    public class StanyZPlikuCsvTests:StanyZPlikuCsv
    {
        [Fact(DisplayName = "Zamiana wartości na liczbę")]
        public void ZamienSlowoNaWartoscTest()
        {
            this.slownikFraz = new Dictionary<string, decimal>(StringComparer.InvariantCultureIgnoreCase);
            slownikFraz.Add("malo",10);
            slownikFraz.Add("srednio",30);
            slownikFraz.Add("duzo",100);
            slownikFraz.Add("",0);
            slownikFraz.Add("brak",0);
            var wynik = ZamienSlowoNaWartosc("malo");
            Assert.Equal(wynik,10);

            wynik = ZamienSlowoNaWartosc("");
            Assert.Equal(wynik, 0);

           wynik = ZamienSlowoNaWartosc("jakos mtak");
            Assert.Null(wynik);
        }

        [Fact(DisplayName = "Przygotowanie słownika")]
        public void PrzygotujSlownikTest()
        {
            this.SlownikDoZmiany = "malo:10;srednio:20;duzo:100";
            PrzygotujSlownik();
            Assert.NotNull(slownikFraz);
            Assert.Equal(slownikFraz.Count,3);

            this.SlownikDoZmiany = "malo:10;srednio:20;duzo:100;malo:20";
            PrzygotujSlownik();
            Assert.NotNull(slownikFraz);
            Assert.Equal(slownikFraz.Count, 3);

            this.SlownikDoZmiany = "malo:10;srednio:20;duzo:100,malo:20";
            bool bylBlad = false;
            try
            {
                PrzygotujSlownik();
            }
            catch
            {
                bylBlad = true;
            }
            Assert.True(bylBlad);
        }

        [Fact(DisplayName = "Pobieranie stanów z pliku CSV")]
        public void PobierzStanyProduktuTest()
        {

            string tesk = "SYMBOL;STAN\r\n1141510128;114\r\n1141510140;134\r\n62144010G;849";
            NrKolumnyZIdentyfikatorem = 1;
            NrKolumnyZeStanem = 2;
            LiczbaWierszyDoPominiecia = 1;

            //masita ma w subiekcie kody bez myślników, w pliku xml są z myślnikami
            Dictionary<string, long> listaproduktow = new Dictionary<string, long> {{"1141510128", 1}, {"1141510140", 2}, {"62144010G", 3}};

            Dictionary<long, decimal> stany = PobierzStanyProduktu(tesk, listaproduktow);

            Assert.Equal(3, stany.Count);
            Assert.Equal(114, stany[1]);
            Assert.Equal(134, stany[2]);
            Assert.Equal(849, stany[3]);
        }
    }
}
