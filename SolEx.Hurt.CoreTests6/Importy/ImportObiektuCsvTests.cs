using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.Importy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Web;
using Xunit;
using Assert = Xunit.Assert;

namespace SolEx.Hurt.CoreTests.Importy
{
    public class ImportObiektuCsvTests
    {
        [Fact]
        public void PrzetworzTest()
        {
            string dane = @"Id;Tłumaczenie;Tekst w języku podstawowym;Miejsce wystąpienia tłumaczenia
-8864674407917973724;Waga;Waga;Newsletter
-7642922369095706311;Błąd importu zamówienia: {0};Błąd importu zamówienia: {0};Newsletter
;Błąd importu zamówienia: {0};Błąd importu zamówienia: {0};Newsletter";

            List<Komunikat> komunikaty;
            ImportObiektuCsv import = new ImportObiektuCsv();
            Dictionary<string, List<OpisPolaObiektu>> wynik = import.Przetworz(typeof(TlumaczeniePole), dane, out komunikaty);
            Assert.True(wynik.Count ==2);
            Assert.NotNull(wynik["-8864674407917973724"]);
            List<OpisPolaObiektu> tmp1 = wynik["-8864674407917973724"];
           // Assert.True(tmp1.First().Wartosc.ToString().Equals("Waga",StringComparison.InvariantCultureIgnoreCase));

            Assert.NotNull(wynik["-7642922369095706311"]);
            List<OpisPolaObiektu> tmp2 = wynik["-7642922369095706311"];
      //      Assert.True(tmp2.First().Wartosc.ToString().Equals("Błąd importu zamówienia: {0}",StringComparison.InvariantCultureIgnoreCase));
            Assert.True(komunikaty.Count==1);

        }
    }
}