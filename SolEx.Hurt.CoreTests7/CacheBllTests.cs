using Xunit;
using SolEx.Hurt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Tests
{
    public class CacheBllTests
    {
        [Fact()]
        public void WyliczKluczDlaMenuTest()
        {
            CacheBll cache;
            Inicjalizacja(out cache);

            int id = 11;
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 0;
            int idJezyka = 9;
            bool czySaUkrywaneTresci = false;
            string kluczWynikowy = "Menu_N_9_11";
            string klucz = cache.WyliczKluczDlaMenu(id, klient.Id, idJezyka, czySaUkrywaneTresci);

            Assert.True(klucz.Equals(kluczWynikowy), "Klucz niezgodny");

            klient.Id = 22;
            kluczWynikowy = "Menu_Z_9_11";
            klucz = cache.WyliczKluczDlaMenu(id, klient.Id, idJezyka, czySaUkrywaneTresci);

            Assert.True(klucz.Equals(kluczWynikowy), "Klucz niezgodny");

            czySaUkrywaneTresci = true;
            kluczWynikowy = "Menu_Z_9_11_22";
            klucz = cache.WyliczKluczDlaMenu(id, klient.Id, idJezyka, czySaUkrywaneTresci);

            Assert.True(klucz.Equals(kluczWynikowy), "Klucz niezgodny");
        }

        private ISolexBllCalosc Inicjalizacja(out CacheBll cache)
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            cache = new CacheBll(calosc);
            cache.Calosc = calosc;
            return calosc;
        }

        [Fact(DisplayName = "Test dla SlownikPrywatny_PobierzObiekt")]
        public void SlownikPrywatny_PobierzObiektTest()
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            var cache = new CacheBll(calosc);
            int juzByloWykonane = 0;
            Func<long, object> funkcja = (long x) =>
            {
                ++juzByloWykonane;
                return new Random().Next().ToString();
            };

            A.CallTo(() => calosc.Konfiguracja.JezykiWSystemie).Returns(new Dictionary<int, Jezyk> {{1, new Jezyk() {Id = 1, Symbol = "pl"} }});

            cache.InicjalizujPrywatnySlownik(new List<string> { "testowy" });


            string wynik = cache.SlownikPrywatny_PobierzObiekt<string>(funkcja, 1, 1, "testowy");

            //drug raz to to samo wykoanie ma NIE uruchomić funkcji testowej - czyli bedzie ten sam wynik co wczesniej
            string wynik2 = cache.SlownikPrywatny_PobierzObiekt<string>(funkcja, 1, 1, "testowy");

            Assert.True(wynik == wynik2);
            Assert.True(juzByloWykonane == 1);
            
        }

        [Fact()]
        public void WyliczKluczDlaKategoriiTest()
        {
            CacheBll cache;
            ISolexBllCalosc calosc = Inicjalizacja(out cache);
            IProfilKlientaBll profil = A.Fake<IProfilKlientaBll>();
            string idStaleFiltry = "22,55,77,99";
            A.CallTo(() => profil.PobierzStaleFiltryString(A<IKlient>.Ignored)).Returns(idStaleFiltry);
            A.CallTo(()=> calosc.ProfilKlienta).Returns(profil);

            int idKontrolki = 11;
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 7;
            klient.OfertaIndywidualizowana = false;
            string szukane = "szukaj";
            string kluczWynikowy = "Kategorie_11_22,55,77,99_szukaj";
            string klucz = cache.WyliczKluczDlaKategorii(idKontrolki, klient,  szukane);

            Assert.True(klucz.Equals(kluczWynikowy), "Klucz niezgodny");

            klient.OfertaIndywidualizowana = true;
            kluczWynikowy = "Kategorie_11_22,55,77,99_szukaj_7";
            klucz = cache.WyliczKluczDlaKategorii(idKontrolki, klient,  szukane);

            Assert.True(klucz.Equals(kluczWynikowy), "Klucz niezgodny");
        }
    }
}