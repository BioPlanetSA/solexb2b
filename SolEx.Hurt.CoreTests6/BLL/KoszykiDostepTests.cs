using System;
using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class KoszykiDostepTests
    {
        [Fact(DisplayName = "Sprawdzenie czy w koszyku jest produkto  wybranym ID")]

        public void ZawieraProduktTest()
        {
            //var koszyk = A.Fake<KoszykiBLL>(a => a.WithArgumentsForConstructor(new Object[] {null}));
            //Assert.False( KoszykiDostep.PobierzInstancje.ZawieraProdukt(koszyk,1,TypPozycjiKoszyka.Zwykly), "gdy nie ma nic");

            //var pozycja = A.Fake<KoszykPozycje>(a => a.WithArgumentsForConstructor(new Object[] { null,1,null,null, null }));
            //pozycja.produkt_id = 1;
            //List<KoszykPozycje> pozycje = new List<KoszykPozycje>();
            //pozycje.Add(pozycja);
            //var koszyk2 = A.Fake<KoszykiBLL>(a => a.WithArgumentsForConstructor(new Object[] { null, 1, new List<IKoszykPozycja>(pozycje) }));
            //Assert.True(KoszykiDostep.PobierzInstancje.ZawieraProdukt(koszyk2, 1, TypPozycjiKoszyka.Zwykly), "gdy jest produkt o tym id");

            //var pozycja2 = A.Fake<KoszykPozycje>(a => a.WithArgumentsForConstructor(new Object[] { null, 1, null, null, null }));
            //pozycja.produkt_id = 2;
            //List<KoszykPozycje> pozycje2 = new List<KoszykPozycje>();
            //pozycje2.Add(pozycja2);
            //var koszyk3 = A.Fake<KoszykiBLL>(a => a.WithArgumentsForConstructor(new Object[] { null, 1, new List<IKoszykPozycja>(pozycje) }));
            //Assert.False(KoszykiDostep.PobierzInstancje.ZawieraProdukt(koszyk3, 1, TypPozycjiKoszyka.Zwykly), "gdy jest produkt o nie tym id");
        }

        [Fact(DisplayName = "Sprawdzenie przekroczonego stanu")]
        public void CzyJestPrzektoczonyStanTest()
        {
            TestPrzekroczone1();
            TestPrzekroczone2();
            TestPrzekroczone3();
            TestPrzekroczone4();
        }

        private void TestPrzekroczone1()
        {
            //var ConfigBllFake = A.Fake<IConfigBLL>();
            //var stany = A.Fake<ISposobyPokazywaniaStanowBLL>();
            //var zadania = A.Fake<IZadaniaBLL>();
            //A.CallTo(() => stany.SaStany(null, null)).Returns(true);
            //var kd = A.Fake<KoszykiDostep>();
            //kd.Konfiguracja = ConfigBllFake;
            //kd.Stany = stany;
            //kd.Zadania = zadania;
            //var pk = A.Fake<ProduktKlienta>(a => a.WithArgumentsForConstructor(new Object[] { new ProduktBazowy(1), 1 }));
            //A.CallTo(() => pk.IloscLaczna).Returns(20);

            //var jednostka = new JednostkaProduktu() { Przelicznik = 1, Calkowitoliczowa = true, Podstawowa = true, JednostkaId = 1 };

            //var kp_baza = new koszyki_pozycje() { jednostka = jednostka.JednostkaId, ilosc = 5 };
            //KoszykPozycje pozycjaBLL = A.Fake<KoszykPozycje>(a => a.WithArgumentsForConstructor(new Object[] { kp_baza, 1, new Klient(null) { klient_id = 1 }, jednostka, null }));

            //A.CallTo(() => pozycjaBLL.Produkt).Returns(pk);

            //var koszyk = new KoszykiBLL(null, 1, new List<IKoszykPozycja> { pozycjaBLL });
            //A.CallTo(() => kd.CzyJestWlaczonyModulPrzekroczonychStanow).Returns(false);

            //var wynik = kd.CzyJestPrzekroczonyStan(koszyk, pozycjaBLL);

            //Assert.False(wynik, "stany nie przekroczone, bez zadania  i zdefiniowanymi sposoobami pokazywania stanów");
        }

        private void TestPrzekroczone2()
        {
            //var ConfigBllFake = A.Fake<IConfigBLL>();
            //var stany = A.Fake<ISposobyPokazywaniaStanowBLL>();
            //var zadania = A.Fake<IZadaniaBLL>();
            //A.CallTo(() => stany.SaStany(null, null)).Returns(true);
            //var kd = A.Fake<KoszykiDostep>();
            //kd.Konfiguracja = ConfigBllFake;
            //kd.Stany = stany;
            //kd.Zadania = zadania;
            //var pk = A.Fake<ProduktKlienta>(a => a.WithArgumentsForConstructor(new Object[] { new ProduktBazowy(1), 1 }));
            //A.CallTo(() => pk.IloscLaczna).Returns(20);

            //var jednostka = new JednostkaProduktu() { Przelicznik = 1, Calkowitoliczowa = true, Podstawowa = true, JednostkaId = 1 };

            //var kp_baza = new koszyki_pozycje() { jednostka = jednostka.JednostkaId, ilosc = 30 };
            //KoszykPozycje pozycjaBLL = A.Fake<KoszykPozycje>(a => a.WithArgumentsForConstructor(new Object[] { kp_baza, 1, new Klient(null) { klient_id = 1 }, jednostka, null }));

            //A.CallTo(() => pozycjaBLL.Produkt).Returns(pk);

            //var koszyk = new KoszykiBLL(null, 1, new List<IKoszykPozycja> { pozycjaBLL });
            //A.CallTo(() => kd.CzyJestWlaczonyModulPrzekroczonychStanow).Returns(false);

            //var wynik = kd.CzyJestPrzekroczonyStan(koszyk, pozycjaBLL);
            ////wcześniej ten test sprawdzał stany przekroczone bez zadania i ze zd. spo. pok. st. ale logika tej metody przecież tego nie obsługuje - nie ma takiego przypadku
            //Assert.False(wynik, "stany nie przekroczone, bez zadania  i zdefiniowanymi sposoobami pokazywania stanów");
        }

        private void TestPrzekroczone3()
        {
            //var ConfigBllFake = A.Fake<IConfigBLL>();
            //var stany = A.Fake<ISposobyPokazywaniaStanowBLL>();
            //var zadania = A.Fake<IZadaniaBLL>();

            //A.CallTo(() => stany.SaStany(null, null)).Returns(true);
            //var kd = A.Fake<KoszykiDostep>();
            //kd.Konfiguracja = ConfigBllFake;
            //kd.Stany = stany;
            //kd.Zadania = zadania;
            //var pk = A.Fake<ProduktKlienta>(a => a.WithArgumentsForConstructor(new Object[] { new ProduktBazowy(1), 1 }));
            //A.CallTo(() => pk.IloscLaczna).Returns(20);

            //var jednostka = new JednostkaProduktu() { Przelicznik = 1, Calkowitoliczowa = true, Podstawowa = true, JednostkaId = 1 };

            //var kp_baza = new koszyki_pozycje() { jednostka = jednostka.JednostkaId, ilosc = 30 };
            //KoszykPozycje pozycjaBLL = A.Fake<KoszykPozycje>(a => a.WithArgumentsForConstructor(new Object[] { kp_baza, 1, new Klient(null) { klient_id = 1 }, jednostka, null }));

            //A.CallTo(() => pozycjaBLL.Produkt).Returns(pk);

            //A.CallTo(() => kd.CzyJestWlaczonyModulPrzekroczonychStanow).Returns(true);

            //var koszyk = new KoszykiBLL(null, 1, new List<IKoszykPozycja> { pozycjaBLL });

            //A.CallTo(() => zadania.ZnajdzZadaniaWgTypu<PrzekroczoneStany>()).Returns(new List<PrzekroczoneStany> { new PrzekroczoneStany() });
            //var wynik =kd.CzyJestPrzekroczonyStan(koszyk, pozycjaBLL);

            //Assert.True(wynik, "stany przekroczone, z zadaniem i zdefiniowanymi sposoobami pokazywania stanów");
        }

        private void TestPrzekroczone4()
        {
            var ConfigBllFake = A.Fake<IConfigBLL>();
            //var stany = A.Fake<ISposobyPokazywaniaStanowBLL>();
            //var zadania = A.Fake<IZadaniaBLL>();

            //A.CallTo(() => stany.SaStany(null, null)).Returns(false);
            //var kd = A.Fake<KoszykiDostep>();
            //kd.Konfiguracja = ConfigBllFake;
            //kd.Stany = stany;
            //kd.Zadania = zadania;
            //var pk = A.Fake<ProduktKlienta>(a => a.WithArgumentsForConstructor(new Object[] { new ProduktBazowy(1), 1 }));
            //A.CallTo(() => pk.IloscLaczna).Returns(20);

            //var jednostka = new JednostkaProduktu() { Przelicznik = 1, Calkowitoliczowa = true, Podstawowa = true, Id = 1 };

            //var kp_baza = new KoszykPozycje() { JednostkaId= jednostka.Id, Ilosc = 30 };
            //KoszykPozycje pozycjaBLL = A.Fake<KoszykPozycje>(a => a.WithArgumentsForConstructor(new Object[] { kp_baza, 1, new Klient(null) { Id = 1 }, jednostka, null }));

            //A.CallTo(() => pozycjaBLL.Produkt).Returns(pk);
            //A.CallTo(() => kd.CzyJestWlaczonyModulPrzekroczonychStanow).Returns(true);


            //var koszyk = new KoszykiBLL(null, 1, new List<IKoszykPozycja> { pozycjaBLL });

            //A.CallTo(() => zadania.PobierzZadaniaCalegoKoszykaKtorePasuja<IModulStartowy>(koszyk)).Returns(new List<ZadanieCalegoKoszyka> { new PrzekroczoneStany() });
            //var wynik = kd.CzyJestPrzekroczonyStan(koszyk, pozycjaBLL);

            //Assert.False(wynik, "stany przekroczone, z zadaniem i bez sposoobami pokazywania stanów");
        }

        [Fact(DisplayName = "Sprawdzanie wyliczonej ilości")]
        public void SprawdzIloscTest()
        {
            SprawdzIlosc(2, 0, false, 3, true, false, 3,2);
            SprawdzIlosc(2, 0, false, 3, true, false,0, 4);
            SprawdzIlosc(2, 0, false, 3, true, false, 0, 3);
            SprawdzIlosc(2, 0, false, 3, true, false, 2, 2, false);
            SprawdzIlosc(2, 0, false, 1, true, false, 2, 2);
            SprawdzIlosc(2, 2, true, 1, true, false, 2, 2);
            SprawdzIlosc(2, 2, true, 1, true, false, 2, 3);

            SprawdzIlosc(1, 2, true, 1, true, false, 0, 2);


            SprawdzIlosc(2, 2, true, 3, true, false, 4, 2);

            SprawdzIlosc(2, 2, true, 3, true, false, 2, 2, false);
            SprawdzIlosc(2.55M, 2, true, 2, true, false, 4, 2);//tu jest błąd opakowanie jest 2 sztuki i jest wymagane więć wynik p[owienien być 4 a nie 2.55
            SprawdzIlosc(0.3M, 1, true, 1, true, true, 1, 2);
            SprawdzIlosc(4, 6, true, 6, true, true, 6, 2);
            SprawdzIlosc(2, 0, false, 3, false, false, 2, 2);
            SprawdzIlosc(2, 0, false, 1, false, false, 2, 2);
            SprawdzIlosc(2, 2, true, 1, false, false, 2, 2);
            SprawdzIlosc(2, 2, true, 3, false, false, 2, 2);
            SprawdzIlosc(2.55M, 2, true, 2, false, true, 4, 2);
            SprawdzIlosc(0.3M, 1, true, 1, false, true, 1, 2);
            SprawdzIlosc(4, 6, true, 6, false, false, 6, 2);
            SprawdzIlosc(34444, 43, true, 43, true, true, 9999, 2);

            //SprawdzIlosc(true,5,5,0,false);
            //SprawdzIlosc(true, 5.5M, 6, 0, false);
            //SprawdzIlosc(true, 5, 5, 0, true);
            //SprawdzIlosc(true, 5.5M, 6, 0, true);
            //SprawdzIlosc(true, 5, 8, 4, true);
            //SprawdzIlosc(true, 5.5M, 8, 4, true);

            //SprawdzIlosc(true, 5,3, 0, false,3);
            //SprawdzIlosc(true, 5.5M, 3, 0, false, 3);
            //SprawdzIlosc(true, 5, 3, 0, true, 3);
            //SprawdzIlosc(true, 5.5M,3, 0, true, 3);
            //SprawdzIlosc(true, 5, 3, 4, true, 3);
            //SprawdzIlosc(true, 5.5M, 3, 4, true, 3);

            //SprawdzIlosc(false, 5, 5, 0, false);
            //SprawdzIlosc(false, 5.5M, 5.5M, 0, false);
        }

        private void SprawdzIlosc(decimal iloscObecna, int oz, bool wymaganeOZ, int iloscminimalna, bool jednostkaPodstowa, bool calkowitoliczbowa,
            decimal iloscOczekiwana,decimal poprzednia,bool  minimumwymagane=true,decimal? limit=null)
        {
            ProduktBazowy pb = new ProduktBazowy(1);
            
            pb.IloscWOpakowaniu = oz;
            pb.WymaganeOz = wymaganeOZ;
            pb.IloscMinimalna = iloscminimalna;
            pb.JezykId = 1;
            int idjednostki = 1;
            IKlient klient = A.Fake<IKlient>();
            ProduktKlienta pk = A.Fake<ProduktKlienta>(a => a.WithArgumentsForConstructor(new Object[] { pb, klient }));
           // A.CallTo(() => pk.IloscLaczna).Returns(20);
           // A.CallTo(() => pk.DostepnyLimit).Returns(limit);

            //var jednostka = new JednostkaProduktu { Przelicznik = 1, Calkowitoliczowa = calkowitoliczbowa, Podstawowa = jednostkaPodstowa, Id = 1};
            
            //pb.Jednostki.Add(jednostka);
            //if (!jednostkaPodstowa)
            //{
            //    var jednostka2 = new JednostkaProduktu { Przelicznik = 1, Calkowitoliczowa = false, Podstawowa = true, Id = 2 };

            //    pb.Jednostki.Add(jednostka2);
            //}
            
            ISolexBllCalosc sol = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => sol.ProduktyJednostkiDostep.PobierzJednostkiProduktuWgProduktu(1)).Returns(null);
            KoszykiDostep kd = new KoszykiDostep(sol);
            decimal po = kd.SprawdzIlosc(pk, idjednostki, iloscObecna, poprzednia);
      
            bool wynik = iloscOczekiwana == po;
            Assert.True(wynik,string.Format("Calkowitoliczbowa={0},limity {6},ilosc_w_opakownaiu={4}, wymaganie_oz={5},wejsciowa {3} oczekiwana {1}, otrzymana{2},podstawowa {7} poprzednia {8} "
                 , calkowitoliczbowa, iloscOczekiwana, po, iloscObecna, oz, wymaganeOZ, limit, jednostkaPodstowa, poprzednia));
        }

        //[Fact()]
        //public void StworzPozycjeTest()
        //{
        //    StworzPozycje(1,1);
        //    StworzPozycje(2, 2);
        //    StworzPozycje(4,1);
        //    StworzPozycje(null,1);
        //}

        //private void StworzPozycje(int? jednostka, int oczekiwana)
        //{
        //    int jezyk = 1;
        //    int produkt = 1;
        //    var jednostki=   A.Fake<IProduktyJednostkiDostep>();
        //    Dictionary<int,List<JednostkaProduktu>> mocJednostki=new Dictionary<int, List<JednostkaProduktu>>();
        //    mocJednostki.Add(1,new List<JednostkaProduktu>());
        //    mocJednostki[1].Add(new JednostkaProduktu() { ProduktId = produkt, Calkowitoliczowa = true, Podstawowa = true, JednostkaId = 1, Nazwa = "Podstaowa" });
        //    mocJednostki[1].Add(new JednostkaProduktu() { ProduktId = produkt, Calkowitoliczowa = true, Podstawowa = false, JednostkaId = 2, Nazwa = "druga" });
        //    mocJednostki[1].Add(new JednostkaProduktu() { ProduktId = produkt, Calkowitoliczowa = true, Podstawowa = false, JednostkaId = 3, Nazwa = "trzecia" });
        //    A.CallTo(() => jednostki.PobierzJednostkiProduktuWgProduktu(jezyk)).Returns(mocJednostki);
        //    var kd = A.Fake<KoszykiDostep>();
        //    kd.Jednostki = jednostki;
        //    Klient k=new Klient(null);
        //    k.klient_id = 1;
        //    koszyki_pozycje kp=new koszyki_pozycje();
        //    kp.jednostka = jednostka;
        //    kp.produkt_id = produkt;
        //    var pozycja=  kd.StworzPozycje(kp, jezyk,new KoszykiBLL(null));
        //    bool wynik = pozycja.jednostka == oczekiwana;

        //    Assert.True(wynik,string.Format("JEdnostka {0}, oczekiwana {1}, otrzymana {2}",jednostka,oczekiwana,pozycja.jednostka));
        //}
          [Fact()]
        public void PobierzPozycjeKoszykaPasujaceTest()
        {
           // int koszykid = 1;
            int idklienta = 1;
            //var koszykiDostep = A.Fake<KoszykiDostep>();
            var produktyklienta = A.Fake<IProduktyKlienta>();
              IKlient klient = A.Fake<IKlient>();
              A.CallTo((() => klient.Id)).Returns(idklienta);
            HashSet<long> idsdostepnych = new HashSet<long>();
            idsdostepnych.Add(1);
            idsdostepnych.Add(3);
            A.CallTo(() => produktyklienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(klient)).Returns(idsdostepnych);
            //koszykiDostep.ProduktKlienta = produktyklienta;
            //List<KoszykPozycje> pozycje = new List<KoszykPozycje>();
            //pozycje.Add(new KoszykPozycje() { KoszykId = koszykid, ProduktId = 1, Ilosc = 1, Id = 1 });
            //pozycje.Add(new KoszykPozycje() { KoszykId = koszykid, ProduktId = 2, Ilosc = 1, Id = 2 });
            //A.CallTo(() => koszykiDostep.Get(koszykid)).Returns(pozycje);
            //var wynik = koszykiDostep.PobierzPozycjeKoszykaPasujace(koszykid, klient);
            //bool ilosc = wynik.Count == 1;
            //Assert.True(ilosc);
        }

          [Fact()]
          public void ZmienStatusPozycjiInfoDostepnoscTest()
          {
              //  int koszykid = 1;
              //int klient = 1;
              //int jezyk = 1;
              //IKlient klie = A.Fake<IKlient>();

              //klie.klient_id = klient;
              //KoszykiDostep kd=new KoszykiDostep();
              //A.CallTo(() => kd.PobierzIdKoszykaDostepnosc(klie)).Returns(koszykid);
              //A.CallTo(() => kd.Pobierz(koszykid,jezyk)).Returns(koszykid);
          }

          [Fact(DisplayName="Test - zmiany statusu powzycji koszyka, dodawanue jak niema, usuwanie jak jest")]
          public void ZmienStatusPozycjiTest()
          {
              //TestPRoduktuNieMa();
              //TestPRoduktuJest();
          }

        //private void TestPRoduktuNieMa()
        //{
        //    int koszykid = 1;
        //    int klient = 1;
        //    int produkt = 1;
        //    IKlient klie = A.Fake<IKlient>();
        //    IKoszykiBLL koszyk = A.Fake<KoszykBll>();
        //    koszyk.Id = koszykid;
        //    klie.klient_id = klient;
        //    var kd = A.Fake<KoszykiDostep>();
        //    IKoszykPozycja wynik = A.Fake<IKoszykPozycja>();
        //    A.CallTo(() => kd.DodajPozycje(koszyk, klie, A<IKoszykPozycja>.Ignored)).Returns(wynik);
        // //   A.CallTo(() => kd.UaktualnijKoszyk(koszyk)).Returns(koszyk);
           
        //    List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>();

          
        //    wynik.produkt_id = produkt;
        //    wynik.ilosc = 1;
        //    wynik.koszyk_id = koszykid;

        //    A.CallTo(() => koszyk.Pozycje).Returns(pozycje);
       
        //    IKoszykPozycja kp = kd.ZmienStatusPozycji(koszyk, klie, produkt);
        //    Assert.NotNull(kp);
        //    Assert.Equal(kp.produkt_id,wynik.produkt_id);
        //}
        //private void TestPRoduktuJest()
        //{
        //    int koszykid = 1;
        //    int klient = 1;
        //    int produkt = 1;
        //    IKlient klie = A.Fake<IKlient>();
        //    IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
        //    koszyk.id = koszykid;
        //    klie.klient_id = klient;
        //    var kd =A.Fake<KoszykiDostep>();
        //    IKoszykPozycja wynik = A.Fake<IKoszykPozycja>();
        //    A.CallTo(() => kd.DodajPozycje(koszyk, klie, A<Ikoszyki_pozycje>.Ignored)).Returns(wynik);
        // //   A.CallTo(() => kd.UaktualnijKoszyk(koszyk)).Returns(koszyk);

        //    List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>();

 
        //    wynik.produkt_id = produkt;
        //    wynik.ilosc = 1;
        //    wynik.koszyk_id = koszykid;
        //    pozycje.Add(wynik);
        //    A.CallTo(() => koszyk.PobierzPozycje()).Returns(pozycje);
        //    A.CallTo(() => kd.DodajPozycje(koszyk, klie, A<IKoszykPozycja>.Ignored)).Returns(wynik);
        //    IKoszykPozycja kp = kd.ZmienStatusPozycji(koszyk, klie, produkt);
        //    Assert.Null(kp);
           
        //}

        //[Fact(DisplayName = "Test sprawdzajacy poprawność generowanych odpowiedzi - wartosci netto, brutto")]
        //public void WygenerujKomunikatyTest()
        //{
        //    List<OdpowiedzKoszyk> wynik = new List<OdpowiedzKoszyk>();

        //    decimal netto = (decimal)100878.011534535;
        //    WartoscLiczbowa zaokraglonaNetto = Decimal.Round(netto, 2);

        //    decimal brutto = (decimal)130878.011534535;
        //    WartoscLiczbowa zaokraglonaBrutto = Decimal.Round(brutto, 2);


        //    List<IKoszykPozycja> Poz = new List<IKoszykPozycja>();

        //    IKoszykPozycja kp1 = A.Fake<IKoszykPozycja>();
        //    A.CallTo(() => kp1.id).Returns(1);
        //    IKoszykPozycja kp2 = A.Fake<IKoszykPozycja>();
        //    A.CallTo(() => kp2.id).Returns(2);
        //    Poz.Add(kp1);
        //    Poz.Add(kp2);

        //    IKoszykiBLL result = A.Fake<IKoszykiBLL>();
        //    result.Typ = TypKoszyka.Koszyk;
        //    A.CallTo(() => result.Pozycje).Returns(Poz);
        //    A.CallTo(() => result.CalkowitaWartoscHurtowaNettoPoRabacie).Returns(netto);
        //    A.CallTo(() => result.CalkowitaWartoscHurtowaBruttoPoRabacie).Returns(brutto);
        //    A.CallTo(() => result.WalutaKoszyka).Returns("pl");

        //    var sesja = A.Fake<ISesjaHelper>();
        //    A.CallTo(() => sesja.JezykID).Returns(1);
        //    A.CallTo(() => sesja.PokazujCeny).Returns(true);
        //    var konf = A.Fake<IConfigBLL>();
        //    A.CallTo(() => konf.InfoPrzekroczoneStany).Returns(false);
        //    A.CallTo(() => konf.PobierzTlumaczenie(1, "Dodano do koszyka")).Returns("Dodano do koszyka");

        //    var koszykiDostep = A.Fake<KoszykiDostep>();
        //    koszykiDostep.Konfiguracja = konf;
        //    koszykiDostep.SesjaHelper = sesja;
            

        //    wynik = koszykiDostep.WygenerujKomunikaty(result, new List<IKoszykPozycja>(), new List<IKoszykPozycja>(),new List<IProduktKlienta>(), new List<IKoszykPozycja>(), Poz);
        //    Assert.Equal(wynik[0].Netto, zaokraglonaNetto);
        //    Assert.Equal(wynik[0].Brutto, zaokraglonaBrutto);
        //}
    }
}
