//using System.Collections.Generic;
//using FakeItEasy;
//using SolEx.Hurt.Core.Pomocnicze;
//using SolEx.Hurt.Model;
//using SolEx.Hurt.Model.Interfaces;
//using Xunit;
//using SolEx.Hurt.Core.BLL;
//namespace SolEx.Hurt.Sync.App.Modules_.Helpers.Tests
//{
//    public class KlienciHelperTests
//    {


//        [Fact(DisplayName = "Czy klient ma cechę - test sprawdzający każdą kombinację cech klienta")]
//        public void PobierzKategorieKlientaTest()
//        {
//            IConfigBLL config = A.Fake<IConfigBLL>();
//            A.CallTo(() => config.CechaAuto).Returns("--auto--");
//            A.CallTo(() => config.SeparatorGrupKlientow).Returns("_:");
//            KlienciWyszukiwanie modul = new KlienciWyszukiwanie();
//            modul.Konfiguracja = config;
//            klienci k = new klienci();
//            k.nazwa = "mietek";
//            k.klient_id = 69;

//            List<kategorie_klientow> kategorieklientow = StworzKategorieKlientow();
//            List<klienci_kategorie> laczniki = StworzLacznikiDoKategoriiDlaKlienta(kategorieklientow, k);

//            string frazaSprawdzajaca = "OPIEKUN";

//            kategorie_klientow znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);
//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(1, znalezionaKategoria.Id);

//            frazaSprawdzajaca = "przedstawiciel";
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(2, znalezionaKategoria.Id);

//            frazaSprawdzajaca = StworzSymbolCechy("--AUTO--", "OPIEKUN");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(1, znalezionaKategoria.Id);

//            frazaSprawdzajaca = StworzSymbolCechy("--auto--", "przedstawiciel");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(2, znalezionaKategoria.Id);

//            frazaSprawdzajaca = StworzSymbolCechy("--AUTO--", "przedstawiciel");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(2, znalezionaKategoria.Id);

//            frazaSprawdzajaca = StworzSymbolCechy("--auto--", "opieKUn");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(1, znalezionaKategoria.Id);

//            frazaSprawdzajaca = StworzSymbolCechy("@pracownik", "--auto--");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(4, znalezionaKategoria.Id);

//            frazaSprawdzajaca = StworzSymbolCechy("@pracownik", "--AUTO--");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(4, znalezionaKategoria.Id);

//            frazaSprawdzajaca = StworzSymbolCechy("rola-klienta", "OPIEKUN");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(7, znalezionaKategoria.Id);

//            frazaSprawdzajaca = StworzSymbolCechy("rola-klienta", "@pracownik");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(8, znalezionaKategoria.Id);

//            frazaSprawdzajaca = StworzSymbolCechy("rola-klienta", "@");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.NotNull(znalezionaKategoria);
//            Assert.Equal(8, znalezionaKategoria.Id);

//            frazaSprawdzajaca = StworzSymbolCechy("rola-klienta", "@");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(k, kategorieklientow, laczniki, frazaSprawdzajaca, true);
//            Assert.Null(znalezionaKategoria);


//            klienci innyKlient = new klienci();
//            innyKlient.klient_id = 123;

//            //klient nie ma łączników, powinno zwrócić null
//            frazaSprawdzajaca = StworzSymbolCechy("rola-klienta", "@pracownik");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(innyKlient, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.Null(znalezionaKategoria);

//            frazaSprawdzajaca = StworzSymbolCechy("grupa", "sklepy mma");
//            znalezionaKategoria = modul.PobierzKategorieKlienta(innyKlient, kategorieklientow, laczniki, frazaSprawdzajaca);

//            Assert.Null(znalezionaKategoria);

//        }

//        private List<kategorie_klientow> StworzKategorieKlientow()
//        {
//            List<kategorie_klientow> katkl = new List<kategorie_klientow>()
//            {
//                new kategorie_klientow() {Id = 1, grupa = ConfigBLL.PobierzInstancje.CechaAuto, nazwa = "OPIEKUN"},
//                new kategorie_klientow(){Id = 2,grupa = ConfigBLL.PobierzInstancje.CechaAuto,nazwa = "przedstawiciel"},
//                new kategorie_klientow() {Id = 3, grupa = ConfigBLL.PobierzInstancje.CechaAuto, nazwa = "@pracownik"},
//                new kategorie_klientow() {Id = 4, grupa = "@pracownik", nazwa = ConfigBLL.PobierzInstancje.CechaAuto},
//                new kategorie_klientow() {Id = 5, grupa = "przedstawiciel", nazwa = ConfigBLL.PobierzInstancje.CechaAuto },
//                new kategorie_klientow() {Id = 6, grupa = "opiekun", nazwa =ConfigBLL.PobierzInstancje.CechaAuto},
//                new kategorie_klientow() {Id = 7, grupa = "rola-klienta", nazwa = "OPIEKUN"},
//                new kategorie_klientow() {Id = 8, grupa = "rola-klienta", nazwa = "@pracownik"},
//                new kategorie_klientow() {Id = 9, grupa = "rola-klienta", nazwa = "rola-klienta:OPIEKUN"},
//                new kategorie_klientow() {Id = 10, grupa = "rola-klienta", nazwa = "rola-klienta:@pracownik"},
//                new kategorie_klientow() {Id = 11, grupa = "GRUPA", nazwa = "KLUBY MMA"}
//            };

//            return katkl;
//        }

//        private List<klienci_kategorie> StworzLacznikiDoKategoriiDlaKlienta(List<kategorie_klientow> kategorieklientow,
//            klienci klient)
//        {
//            List<klienci_kategorie> laczniki = new List<klienci_kategorie>(kategorieklientow.Count);

//            foreach (var kategorie_Klientow in kategorieklientow)
//            {
//                laczniki.Add(new klienci_kategorie() { kategoria_klientow_id = kategorie_Klientow.Id, klient_id = klient.klient_id });
//            }

//            return laczniki;
//        }


//        private string StworzSymbolCechy(string grupa, string nazwa)
//        {
//            return string.Format("{0}:{1}", grupa, nazwa);
//        }
//    }
//}

