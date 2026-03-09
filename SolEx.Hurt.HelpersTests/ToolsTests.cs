using System;
using System.Collections.Generic;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Helpers.Tests
{
    public class ToolsTests
    {
        [Fact()]
        public void DoLadnejCyfryTest()
        {

            double i = 45.43;
            string wynik = Tools.PobierzInstancje.DoLadnejCyfry(i).Replace(',','.');
            Assert.Equal(wynik, "45.43");

            i = 45;
            wynik = Tools.PobierzInstancje.DoLadnejCyfry(i).Replace(',', '.');
            Assert.Equal(wynik, "45");

            i = 45.00;
            wynik = Tools.PobierzInstancje.DoLadnejCyfry(i).Replace(',', '.');
            Assert.Equal(wynik, "45");

            i = 45.0001;
            wynik = Tools.PobierzInstancje.DoLadnejCyfry(i).Replace(',', '.');
            Assert.Equal(wynik, "45");

            decimal d = (decimal)45.00;
            wynik = Tools.PobierzInstancje.DoLadnejCyfry(d).Replace(',', '.');
            Assert.Equal(wynik, "45");

            d = (decimal)45.764;
            wynik = Tools.PobierzInstancje.DoLadnejCyfry(d).Replace(',', '.');
            Assert.Equal(wynik, "45.76");

            d = (decimal)45.769;
            wynik = Tools.PobierzInstancje.DoLadnejCyfry(d).Replace(',', '.');
            Assert.Equal(wynik, "45.77");
        }

        [Fact(DisplayName = "Widoczność produktów optimy na podstawie atrybutów")]
        public void CzyUkrycProduktTest()
        {
            //[ Produkt OXO.B.1KBO, ukrywanie: , odkrywanie: 1, ukryj: False, wyslij: True, widoczny na b2b: False ]  
            CzyUkrycProdukt(false, false, null, null, false);

            CzyUkrycProdukt(false, false, "AS", null, true);

            CzyUkrycProdukt(false, false, "AS", "BB", false);

            CzyUkrycProdukt(false, true, "AS", "BB", true);

            CzyUkrycProdukt(false, false, "AS", "BB", false);

            CzyUkrycProdukt(false, false, null, "BB", false);

            CzyUkrycProdukt(true, false, null, null, false);

            CzyUkrycProdukt(true, false, "AS", null, false);

            CzyUkrycProdukt(true, false, "AS", "BB", false);

            CzyUkrycProdukt(true, true, "AS", "BB", true);

            CzyUkrycProdukt(true, false, "AS", "BB", false);

            CzyUkrycProdukt(true, false, null, "BB", false);
        }

        private void CzyUkrycProdukt(bool widoczny, bool ukryty, string widocznyustawienie, string ukrytyustawienie,
            bool oczekiwanyWynik)
        {
            bool wynik1 = Helpers.Tools.PobierzInstancje.CzyUkrycProdukt(widoczny, ukryty, widocznyustawienie, ukrytyustawienie);
            Assert.Equal(oczekiwanyWynik, wynik1);
        }

        [Fact(DisplayName = "Sprawdza długość wygenerowanego stringa")]
        public void WygenerujStringDlugoscTest()
        {
            int dlugosc = 8;
            string wynik = Helpers.Tools.PobierzInstancje.WygenerujString(dlugosc);

            Assert.Equal(dlugosc, wynik.Length);
        }

        [Fact(DisplayName = "Sprawdza unikalność wygenerowanych stringów")]
        public void WygenerujStringUnikalnoscTest()
        {
            int dlugosc = 10;

            //przy długości 8 zdubluje się przy ponad - 0.5mln wygenerowaniach
            int ilePowtorzen = 1000000;

            HashSet<string> slownik = new HashSet<string>();

            for (int i = 0; i < ilePowtorzen; i++)
            {
                string wynik = Helpers.Tools.PobierzInstancje.WygenerujString(dlugosc);

                if (slownik.Contains(wynik))
                    throw new Exception(string.Format("Zdublowany wygenerowany string przy {0} iteracji, dubel: {1}", i,
                        wynik));

                else slownik.Add(wynik);
            }
        }

        [Fact()]
        public void GetContentTest()
        {
           // StreamReader sr = new StreamReader("c:\\ZMPD2R5_DE.txt", true);
           //var wynik= Tools.PobierzInstancje.GetContent(sr.BaseStream);
           //sr = new StreamReader("c:\\ZMPD2R5_PL.txt", true);
           //var wynik2= Tools.PobierzInstancje.GetContent(sr.BaseStream);
            
        }

        [Fact(DisplayName = "Sprawdza poprawność ucinania znaków dla propertisów większych niż max")]
        public void SprawdzMaksymalnaLiczbeZnakowTest()
        {
            Produkt prod = new Produkt(1);
            prod.Nazwa = "Test1";
            prod.PoleTekst1 = "jakis tekst1";
            prod.PoleTekst2 = @"Dla skóry suchej i wrażliwej:
1.Z pomocą łyżeczki wymieszać w plastikowym kubeczku sześciu kropli bazy kremowej i dwie krople serum;
            2.Tak utworzony bio kompleks nałożyć cienką warstwę na oczyszczoną twarz;
            3.Po upływie 10 minut, jeśli to konieczne, zetrzeć nadmiar produktu z twarzy przy użyciu wacika kosmetycznego.

           Dla skóry tłustej i mieszanej:
            1.Z pomocą łyżeczki wymieszać w plastikowym kubeczku pięć kropli bazy kremowej i trzy krople serum;
            2.Tak utworzony bio kompleks nałożyć cienką warstwę na oczyszczoną twarz;
            3.Po upływie 10 minut, jeśli to konieczne, zetrzeć nadmiar produktu z twarzy przy użyciu wacika kosmetycznego.

           Aby osiągnąć pożądane efekty należy stosować bio kompleks codziennie przez 14 dni.W celu podtrzymania uzyskanych rezultatów należy używać bio kompleks dwa razy w tygodniu.W razie konieczności należy powtórzyć cykl po upływie dwóch miesięcy.
W skład opakowania wchodzą:
Intensywna bio-baza(tak zwana „pierwsza faza”) - pojemność: 30ml
Aktywizująca emulsja(tak zwana „druga faza”) - pojemność:  30ml
10 szklaneczek i 10 łopatek do mieszania bio-bazy i emulsji.";

            List<string> wynik = Tools.SprawdzIloscZnakow(prod);

            Assert.True(wynik.Count == 1);
            Assert.True(wynik.Contains("PoleTekst2"));
            Assert.True(prod.PoleTekst2.Length<=1000);


        }

        [Fact(DisplayName = "SprawdzANIE WYLICZNAI SUMY KONTROLNEJ DLA KODÓW KRESKOWYCH")]
        public void WyliczSumeDoKoduEanTest()
        {
            Dictionary<long, int> kody = new Dictionary<long, int>();
            kody.Add(779038104111, 4);
            kody.Add(779038104112, 1);
            kody.Add(779038104113, 8);
            kody.Add(779038104114, 5);
            kody.Add(779038104115, 2);
            kody.Add(779038104116, 9);
            kody.Add(779038104117, 6);
            kody.Add(779038104118, 3);
            kody.Add(779038104119, 0);
            kody.Add(779038104120, 6);
            foreach (var kod  in kody)
            {
                int wynik = Tools.WyliczSumeDoKoduEan(kod.Key);
                Assert.True(wynik == kod.Value, $"Wynik wynosi: {wynik}, a powinien {kod.Value}");
            }
        }
    }
}
