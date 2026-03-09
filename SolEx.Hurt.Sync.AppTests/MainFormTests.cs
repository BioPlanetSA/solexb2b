using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using Xunit;
using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.Sync.AppTests
{
    public class MainFormTests
    {
        [Fact(DisplayName = "test walidatora dokumentów")]
        public void SprawdzDokumentCzyPoprawnyTest()
        {            
            List<HistoriaDokumentuProdukt> pozycjeDokPOPRAWNE = new List<HistoriaDokumentuProdukt>()
            {
                new HistoriaDokumentuProdukt(0, "n", "k") {CenaNetto = 50, Vat = 23, CenaBrutto = 61.5m, Ilosc = 1, Rabat = 33.33m, CenaNettoPoRabacie = 33.34m, CenaBruttoPoRabacie = 41.01m, WartoscNetto = 33.34m, WartoscBrutto = 41.01m, WartoscVat = 7.67m},
                new HistoriaDokumentuProdukt(1, "n", "k") {CenaNetto = 42.54m, Vat = 23, CenaBrutto = 52.32m, Ilosc = 789, Rabat = 23.1m, CenaNettoPoRabacie =  32.71m, CenaBruttoPoRabacie =  40.23m, WartoscNetto = 25808.19m, WartoscBrutto = 31744.07m, WartoscVat = 5935.88m},
                new HistoriaDokumentuProdukt(2,"n", "k") {CenaNetto = 56.23m, Vat = 83, CenaBrutto =  102.9m, Ilosc = 33, Rabat = 39m, CenaNettoPoRabacie =  34.30m, CenaBruttoPoRabacie =  62.77m, WartoscNetto = 1131.90m, WartoscBrutto = 2071.38m, WartoscVat = 939.48m},
                new HistoriaDokumentuProdukt(3,"n", "k") {CenaNetto = 12.34m, Vat = 31, CenaBrutto =  16.17m, Ilosc = 983.54m , Rabat = 67.45m, CenaNettoPoRabacie = 4.02m, CenaBruttoPoRabacie =  5.27m, WartoscNetto = 3953.83m, WartoscBrutto = 5179.52m, WartoscVat = 1225.69m},
                new HistoriaDokumentuProdukt(4,"n", "k") {CenaNetto = 0.89m, Vat = 0, CenaBrutto =  0.89m, Ilosc = 348.23m , Rabat = 56.7m, CenaNettoPoRabacie =   0.39m, CenaBruttoPoRabacie =   0.39m, WartoscNetto = 135.81m, WartoscBrutto = 135.81m, WartoscVat = 0},
                new HistoriaDokumentuProdukt(5,"n", "k") {CenaNetto = 323.11m, Vat = 31, CenaBrutto =  423.27m, Ilosc = 348m , Rabat = 42, CenaNettoPoRabacie =   187.40m, CenaBruttoPoRabacie =   245.49m, WartoscNetto = 65215.20m, WartoscBrutto = 85431.91m, WartoscVat = 20216.71m},
            };

            HistoriaDokumentu dokumentPoprawny = new HistoriaDokumentu {DataUtworzenia = DateTime.Now, DataWyslaniaDokumentu = null, WartoscNetto = 96278.27m, WartoscVat =  28325.43m, WartoscBrutto = 124603.70m, NazwaDokumentu = "23/43/2019" };

            MainForm m = new MainForm();
            string blad = null;
            bool wynik = m.SprawdzDokumentCzyPoprawny(dokumentPoprawny, pozycjeDokPOPRAWNE, out blad);

            Assert.True(wynik, blad);
        }

        [Fact(DisplayName = "Sprawdzenie porównywania poziomów cenowych do aktualizacji")]
        public void AktualizujPoziomyCenoweTest()
        {
            PoziomCenowy detaliczna = new PoziomCenowy();
            detaliczna.Id = 1;
            detaliczna.Nazwa = "Detaliczna";
            detaliczna.WalutaId = 2;

            PoziomCenowy hurtowa = new PoziomCenowy();
            hurtowa.Id = 2;
            hurtowa.Nazwa = "Hurtowa";
            hurtowa.WalutaId = null;

            PoziomCenowy specjalna = new PoziomCenowy();
            specjalna.Id = 3;
            specjalna.Nazwa = "Specjalna";
            specjalna.WalutaId = null;

            Dictionary<int, PoziomCenowy> poziomyNaB2B = new Dictionary<int, PoziomCenowy>(3);
            poziomyNaB2B.Add(detaliczna.Id, detaliczna);
            poziomyNaB2B.Add(hurtowa.Id, hurtowa);
            poziomyNaB2B.Add(specjalna.Id, specjalna);

            PoziomCenowy detalicznaerp = new PoziomCenowy();
            detalicznaerp.Id = 1;
            detalicznaerp.Nazwa = "Detaliczna";
            detalicznaerp.WalutaId = null;

            PoziomCenowy hurtowaerp = new PoziomCenowy();
            hurtowaerp.Id = 2;
            hurtowaerp.Nazwa = "Hurtowa";
            hurtowaerp.WalutaId = null;

            PoziomCenowy specjalnaerp = new PoziomCenowy();
            specjalnaerp.Id = 3;
            specjalnaerp.Nazwa = "Specjalna";
            specjalnaerp.WalutaId = null;

            Dictionary<int, PoziomCenowy> poziomyWERP = new Dictionary<int, PoziomCenowy>(3);
            poziomyWERP.Add(detalicznaerp.Id, detalicznaerp);
            poziomyWERP.Add(hurtowaerp.Id, hurtowaerp);
            poziomyWERP.Add(specjalnaerp.Id, specjalnaerp);

            HashSet<int> doAktualizacji = new HashSet<int>();
            HashSet<int> doUsuniecia = new HashSet<int>();
            HashSet<int> doDodania = new HashSet<int>();

            poziomyNaB2B.Porownaj(poziomyWERP, ref doAktualizacji, ref doDodania, ref doUsuniecia, null, null);

            Assert.Equal(doAktualizacji.Count, 1);
        }

        [Fact(DisplayName = "test wyliczania vatu dla dokumentów")]
        public void WyliczWartoscVatDlaPozycjiTest()
        {
            List<HistoriaDokumentuProdukt> pozycjeDok= new List<HistoriaDokumentuProdukt>()
            {
                new HistoriaDokumentuProdukt(0, "n", "k") {Vat = 23,WartoscNetto = 33.34m},
                new HistoriaDokumentuProdukt(1, "n", "k") {Vat = 23, WartoscNetto = 25808.19m},
                new HistoriaDokumentuProdukt(2,"n", "k") {Vat = 83, WartoscNetto = 1131.90m},
                new HistoriaDokumentuProdukt(3,"n", "k") {Vat = 31,WartoscNetto = 3953.83m},
                new HistoriaDokumentuProdukt(4,"n", "k") {Vat = 0, WartoscNetto = 135.81m},
                new HistoriaDokumentuProdukt(5,"n", "k") {Vat = 31,WartoscNetto = 65215.20m},
            };

            MainForm m = new MainForm();
            decimal wynik = m.WyliczWartoscVatDlaPozycji(pozycjeDok);

            Assert.True(wynik == 28325.43m, $"wynik powinien być = 28325.43m a wynosi: {wynik}");
        }




        [Fact(DisplayName = "Test sprawdzający aktualizacje walut")]
        public void AktualizujDokumentyTest()
        {
            //List<historia_dokumenty> docsToSend = new List<historia_dokumenty>();
            //historia_dokumenty d  = new historia_dokumenty() { id = 1,waluta = "PLNN"};
            //historia_dokumenty d2 = new historia_dokumenty() { id = 2,waluta = ""};
            //historia_dokumenty d3 = new historia_dokumenty() { id = 3, waluta = "USD"};
            //historia_dokumenty d4 = new historia_dokumenty() { id = 4,waluta="PLN" };
            //docsToSend.Add(d);
            //docsToSend.Add(d2);
            //docsToSend.Add(d3);
            //docsToSend.Add(d4);

            //Dictionary<int, poziomy_cen> walutyb2b = new Dictionary<int, poziomy_cen>();
            //poziomy_cen pc  = new poziomy_cen() { id = 1, waluta = "PLN", walutaERP = "PLNN" };
            //poziomy_cen pc2 = new poziomy_cen() { id = 2, waluta = "PLNi", walutaERP = "PLN" };
            //poziomy_cen pc3 = new poziomy_cen() { id = 3, waluta = "", walutaERP = "USD" };
            //walutyb2b.Add(pc.id,pc);
            //walutyb2b.Add(pc2.id, pc2);
            //walutyb2b.Add(pc3.id, pc3);

            //var a = A.Fake<SolEx.Hurt.Core.BLL.Dokumenty>();
            //A.CallTo(() => a.PobierzPoziomCen()).Returns(walutyb2b);
            //a.AktualizujWaluty(docsToSend);


            //Assert.True(docsToSend[0].waluta == pc.waluta,  string.Format("oczekiwano: {0}, otrzymano{1}", pc.waluta, docsToSend[0].waluta));
            //Assert.True(docsToSend[1].waluta == pc3.waluta, string.Format("oczekiwano: {0}, otrzymano{1}", pc3.waluta, docsToSend[1].waluta));
            //Assert.True(docsToSend[2].waluta == pc3.waluta, string.Format("oczekiwano: {0}, otrzymano{1}", pc3.waluta, docsToSend[2].waluta));
            //Assert.True(docsToSend[3].waluta == pc2.waluta, string.Format("oczekiwano: {0}, otrzymano{1}", pc2.waluta, docsToSend[3].waluta));



        }

        [Fact(DisplayName = "Test metody fltrującej rabaty, które nie mogą być wysłane na b2b")]
        public void SprawdzIstnienieElementowRabatowTest()
        {
            Rabat r = new Rabat {Wartosc1 = 1};
            Test(null, null, null, null, null, null, r, r);
            r = new Rabat { Wartosc1 = 1,ProduktId = 1};
            Test(null, null, null, null, null, null, r, null);
            Test(1, null, null, null, null, null, r, r);
            r = new Rabat { Wartosc1 = 1, KategoriaProduktowId = 1 };
            Test(null, null, null, null, null, null, r, null);
            Test(null, 1, null, null, null, null, r, r);
            r = new Rabat { Wartosc1 = 1, CechaId = 1 };
            Test(null, null, null, null, null, null, r, null);
            Test(null, null, 1, null, null, null, r, r);
            r = new Rabat { Wartosc1 = 1, PoziomCenyId = 1 };
            Test(null, null, null, null, null, null, r, null);
            Test(null, null, null, 1, null, null, r, r);
            r = new Rabat { Wartosc1 = 1, KlientId = 1 };
            Test(null, null, null, null, null, null, r, null);
            Test(null, null, null, null, 1, null, r, r);
            r = new Rabat { Wartosc1 = 1, KategoriaKlientowId = 1 };
            Test(null, null, null, null, null, null, r, null);
            Test(null, null, null, null, null, 1, r, r);

            r = new Rabat { Wartosc1 = 1, DoKiedy = DateTime.Now.Date.AddDays(-1)};
            Test(null, null, null, null, null, null, r, null);
            r = new Rabat { Wartosc1 = 1, DoKiedy = DateTime.Now.Date.AddDays(1) };
            Test(null, null, null, null, null, 1, r, r);
        }

        void Test(int? produkt, int? kategoria, int? cecha, int? poziom,long? klient,int? kk, Rabat test,Rabat wynik )
        {

            List<Rabat> wejscie = new List<Rabat> { test };
            Dictionary<long, Produkt> produkty = new Dictionary<long, Produkt>();
            Dictionary<long, KategoriaProduktu> kategorie = new Dictionary<long, KategoriaProduktu>();
            Dictionary<int, KategoriaKlienta> kategorieklientow = new Dictionary<int, KategoriaKlienta>();
            Dictionary<long, Klient> klienci = new Dictionary<long, Klient>();
            List<PoziomCenowy> poziomy = new List<PoziomCenowy>();
            List<Cecha> cechy = new List<Cecha>();
            if (produkt.HasValue)
            {
                produkty.Add(produkt.Value,new Produkt(produkt.Value));
            }
            if (klient.HasValue)
            {
                klienci.Add(klient.Value, new Klient(klient.Value));
            }
            if (kk.HasValue)
            {
                kategorieklientow.Add(kk.Value, new KategoriaKlienta());
            }
            if (kategoria.HasValue)
            {
                kategorie.Add(kategoria.Value, new KategoriaProduktu());
            }
            if (cecha.HasValue)
            {
                cechy.Add(new Cecha() { Id = cecha.Value});
            }
            if (poziom.HasValue)
            {
                poziomy.Add(new PoziomCenowy() { Id = poziom.Value });
            }
            List<Rabat> oczekiwane=new List<Rabat>();
            if (wynik != null)
            {
                oczekiwane.Add(wynik);
            }
            TestFiltowania(wejscie, produkty, kategorie, kategorieklientow, klienci, poziomy, cechy, oczekiwane);

        }
        void TestFiltowania(List<Rabat> wejsciowe, Dictionary<long, Produkt> produkty, Dictionary<long, KategoriaProduktu> kategorie,
          Dictionary<int,KategoriaKlienta> kategorieklientow ,Dictionary<long, Klient> klienci,List<PoziomCenowy> poziomy,
            List<Cecha> cechy,List<Rabat> oczekiwane  )
        {
          var  wynik=MainForm.SprawdzIstnienieElementowRabatow(wejsciowe, produkty, kategorie, kategorieklientow, klienci,poziomy,cechy);
          Assert.Equal(oczekiwane.Count, wynik.Count);
            foreach (var r in oczekiwane)
            {
                bool jest = wynik.Any(x => x.Porownaj(r, typeof(Rabat).Properties(), typeof(Rabat).PobierzRefleksja() ));
                Assert.True(jest, "W wynikowych nie znaleziono oczekiwanego rabatu");
            }
            foreach (var r in wynik)
            {
                bool jest = oczekiwane.Any(x => x.Porownaj(r, typeof(Rabat).Properties(), typeof(Rabat).PobierzRefleksja()));
                Assert.True(jest, "W oczekiwanych nie znaleziono oczekiwanego rabatu");
            }
        }

        [Fact(DisplayName = "Test dla walidatora rabatów")]
        public void WalidujRabatyWgWalutyKlientowTest()
        {
            List<Klient> klienci = new List<Klient>() {new Klient() {Id = 1, WalutaId = 3}, new Klient() {Id = 2, WalutaId = 4}, new Klient() {Id = 3, WalutaId = 5}, new Klient() {Id = 6, WalutaId = 5}};
            List<KlientKategoriaKlienta> lacznikiKategoriiKlientow = new List<KlientKategoriaKlienta>()
            {
                new KlientKategoriaKlienta(1, 1),
                new KlientKategoriaKlienta(1, 2),
                new KlientKategoriaKlienta(2, 3),
                new KlientKategoriaKlienta(3, 1)
            };

            List<Rabat> rabaty = new List<Rabat>() {
                new Rabat() {KlientId = 1, WalutaId = 1}, //bledny bo klient ma walute id 3
                new Rabat() {KlientId = 1, WalutaId = 3}, //dobry
                  new Rabat() {KlientId = 1, WalutaId = null}, //zły bez waluty
                    new Rabat() {KlientId = 1}, //zły bez waluty
                      new Rabat() {KlientId = 3}, //zły bez waluty
                new Rabat() {KategoriaKlientowId = 3, WalutaId = 4}, //dobry
                new Rabat() {KategoriaKlientowId = 3, WalutaId = 2} //zly bo klient numer 2 ma walute 4
            };

            MainForm main = new MainForm();
            var wynik = main.WalidujRabatyWgWalutyKlientow(rabaty, klienci, lacznikiKategoriiKlientow);

            Assert.True(wynik != null);
            Assert.True(wynik.Any());

            Klient klientId1 =klienci.First(x => x.Id == 1);

            Assert.True(wynik.ContainsKey(klientId1) );
            Assert.True(wynik[klientId1].Count ==  1 , "pierwszy klient ma tylko miec jeden blad");
        }

        [Fact(DisplayName = "Metoda poprawiająca wartość rabatów - wybierająca korzystniejszy z perespektywy klienta")]
        public void PoprawRabatyTest()
        {
            TestCen( new Rabat {Wartosc1 = 5}, new Rabat {Wartosc1 = 6}, null, null, new Rabat {Wartosc1 = 6});
            TestCen(new Rabat { Wartosc1 = 5, ProduktId = 1 }, new Rabat { Wartosc1 = 6 }, null, null, new Rabat { Wartosc1 = 6 }, new Rabat { Wartosc1 = 5, ProduktId = 1 });
            TestCen(new Rabat { Wartosc1 = 5, ProduktId = 1 }, new Rabat { Wartosc1 = 6 ,ProduktId = 1}, null, null, new Rabat { Wartosc1 = 6,ProduktId = 1});
            TestCen(new Rabat { Wartosc1 = 5, ProduktId = 1, PoziomCenyId = 2 }, new Rabat { Wartosc1 = 6, ProduktId = 1, PoziomCenyId = 1 }, new CenaPoziomu(1, 100, 1), new CenaPoziomu(2, 10, 1), new Rabat { Wartosc1 = 5, ProduktId = 1, PoziomCenyId = 2 });
        }

        void TestCen(Rabat r, Rabat r2, CenaPoziomu c1 = null, CenaPoziomu c2 = null, Rabat w1 = null, Rabat w2 = null)
        {

            List<Rabat> wejscie = new List<Rabat>();
            List<Rabat> oczekiwane = new List<Rabat>();
            Dictionary<long, CenaPoziomu> ceny = new Dictionary<long, CenaPoziomu>();
            if (r != null)
            {
                wejscie.Add(r);
            }

            if (r2 != null)
            {
                wejscie.Add(r2);
            }
            if (c1 != null)
            {
                ceny.Add(c1.Id,c1);
            }
            if (c2 != null)
            {
                ceny.Add(c2.Id, c2);
            }
            if (w1 != null)
            {
                oczekiwane.Add(w1);
            }
            if (w2 != null)
            {
                oczekiwane.Add(w2);
            }
            TestPoprawinaia(wejscie,oczekiwane,ceny);

        }


        void TestPoprawinaia(List<Rabat> wejscie, List<Rabat> oczekiwane, Dictionary<long, CenaPoziomu> ceny)
        {
            MainForm main = new MainForm();
           var wynik= main.PoprawRabaty(wejscie, ceny).Values;
            Assert.Equal(oczekiwane.Count, wynik.Count);
            foreach (var r in oczekiwane)
            {
                bool jest = wynik.Any(x => x.Porownaj(r,new{r.Id}.Properties(),typeof(Rabat).PobierzRefleksja()  ) );
                Assert.True(jest, "W wynikowych nie znaleziono oczekiwanego rabatu");
            }
            foreach (var r in wynik)
            {
                bool jest = oczekiwane.Any(x => x.Porownaj(r, new { r.Id }.Properties(), (typeof(Rabat).PobierzRefleksja())) );
                Assert.True(jest, "W oczekiwanych nie znaleziono oczekiwanego rabatu");
            }
        }

        [Fact(DisplayName = "Test sprawdzajacy poprawność działania metody czyszczacej okreslone pola jezeli produkt jest nie widoczny")]
        public void WidoczneProduktyTest()
        {
            Produkt p1 = new Produkt() { Id = 1, Opis = "Jakis opis 1", Widoczny = true };
            Produkt p2 = new Produkt() { Id = 2, Opis = "Jakis opis 2", Widoczny = true };
            Produkt p5 = new Produkt() { Id = 5, Opis = "Jakis opis 5", Widoczny = true };
            Produkt p3 = new Produkt() { Id = 3, Opis = "Jakis opis 3", Widoczny = false };
            Produkt p4 = new Produkt() { Id = 4, Opis = "Jakis opis 4", Widoczny = false };

            Produkt p6 = new Produkt() { Id = 6, Opis = "Jakis opis 6", Widoczny = false };
            List<Produkt> listaProduktow = new List<Produkt>(){p1,p2,p3,p4,p5,p6};

            Dictionary<long, KategoriaProduktu> kategorie = new Dictionary<long, KategoriaProduktu>();
            KategoriaProduktu k1 = new KategoriaProduktu() { Id = 1, Widoczna = true, GrupaId = 1 };
            KategoriaProduktu k3 = new KategoriaProduktu() { Id = 3, Widoczna = true, GrupaId = 1 };

            KategoriaProduktu k4 = new KategoriaProduktu() { Id = 4, Widoczna = false, GrupaId = 1 };
            KategoriaProduktu k2 = new KategoriaProduktu() { Id = 2, Widoczna = true, GrupaId = 2 };
            KategoriaProduktu k5 = new KategoriaProduktu() { Id = 5, Widoczna = false, GrupaId = 2 };

            kategorie.Add(k1.Id, k1);
            kategorie.Add(k2.Id, k2);
            kategorie.Add(k3.Id, k3);
            kategorie.Add(k4.Id, k4);
            kategorie.Add(k5.Id, k5);

            Grupa g1 = new Grupa() { Id = 1, Widoczna = true };
            Grupa g2 = new Grupa() { Id = 2, Widoczna = false };
            List<Grupa> grupyNaPlatformie = new List<Grupa>(){g1,g2};

            ProduktKategoria pk1 = new ProduktKategoria(){ProduktId = 1,KategoriaId = 1};

            ProduktKategoria pk2 = new ProduktKategoria() { ProduktId = 2, KategoriaId = 1 };
            ProduktKategoria pk4 = new ProduktKategoria() { ProduktId = 3, KategoriaId = 1 };
            ProduktKategoria pk5 = new ProduktKategoria() { ProduktId = 3, KategoriaId = 3 };
            ProduktKategoria pk6 = new ProduktKategoria() { ProduktId = 4, KategoriaId = 1 };

            ProduktKategoria pk3 = new ProduktKategoria() { ProduktId = 2, KategoriaId = 2 };
            
            
            ProduktKategoria pk7 = new ProduktKategoria() { ProduktId = 4, KategoriaId = 2 };

            ProduktKategoria pk8 = new ProduktKategoria() { ProduktId = 5, KategoriaId = 2 };
            ProduktKategoria pk9 = new ProduktKategoria() { ProduktId = 6, KategoriaId = 5 };
            ProduktKategoria pk10 = new ProduktKategoria() { ProduktId = 5, KategoriaId = 2 };
            ProduktKategoria pk11 = new ProduktKategoria() { ProduktId = 6, KategoriaId = 5 };
            List<ProduktKategoria> pkzERP = new List<ProduktKategoria>() { pk1, pk2, pk3, pk4, pk5, pk6, pk7, pk8, pk9, pk10, pk11 };

            //MainForm.config = config;
            MainForm m = new MainForm();
            m.SyncManager = A.Fake<ISyncManager>();
            A.CallTo(() => m.SyncManager.Konfiguracja.PolaDoWyzerowania).Returns(new List<string>() { "Opis" });

            m.CzyszczeniePolNiewidocznychProduktow(ref listaProduktow, kategorie, grupyNaPlatformie, pkzERP);

            Assert.True(listaProduktow[0].Opis==p1.Opis);
            Assert.True(listaProduktow[1].Opis == p2.Opis);
            Assert.True(listaProduktow[2].Opis == null);
            Assert.True(listaProduktow[3].Opis == null);
            Assert.True(listaProduktow[4].Opis == null);
            Assert.True(listaProduktow[5].Opis == null);

        }

        [Fact(DisplayName = "Test sprawdzajacy poprawność porownywania kategorii klienta")]
        public void PorownajTest1()
        {
            string grupa = "Haslo";
            string grupa2 = "cos";
            KategoriaKlienta k1 = new KategoriaKlienta() { Grupa = "JACEK LIS", Id = 1, Nazwa = "rabat_pilchb_Kurier:20" };
            KategoriaKlienta k2 = new KategoriaKlienta() { Grupa = grupa, Id = 2, Nazwa = "Nazwa2" };
            KategoriaKlienta k3 = new KategoriaKlienta() { Grupa = "OPOCZNO", Id = 3, Nazwa = "rabat_incana_Platny kurier:40" };
            KategoriaKlienta k4 = new KategoriaKlienta() { Grupa = "TESTOWA", Id = 4, Nazwa = "rabat_tubądzin_Platny kurier:32" };
            KategoriaKlienta k5 = new KategoriaKlienta() { Grupa = "WIKTOR", Id = 5, Nazwa = "rabat_kwadro_Odbiór Świecie:25" };
            KategoriaKlienta k6 = new KategoriaKlienta() { Grupa = grupa2, Id = 6, Nazwa = "Nazwa6" };
            KategoriaKlienta k7 = new KategoriaKlienta() { Grupa = grupa, Id = 7, Nazwa = "Nazwa7" };
            KategoriaKlienta k8 = new KategoriaKlienta() { Grupa = "WYSYŁKA POLSKA", Id = 8, Nazwa = "rabat_incana_Odbiór Świecie:38" };
            KategoriaKlienta k9 = new KategoriaKlienta() { Grupa = grupa, Id = 9, Nazwa = "Nazwa9" };
            //List<kategorie_klientow> kategorieNaPlatformie = new List<kategorie_klientow>() { k1, k2, k3, k4, k5, k6, k7, k8, k9 };
            Dictionary<int, KategoriaKlienta>kategorieNaPlatformie = new Dictionary<int, KategoriaKlienta>();
            kategorieNaPlatformie.Add(k1.Id,k1);
            kategorieNaPlatformie.Add(k2.Id, k2);
            kategorieNaPlatformie.Add(k3.Id, k3);
            kategorieNaPlatformie.Add(k4.Id, k4);
            kategorieNaPlatformie.Add(k5.Id, k5);
            kategorieNaPlatformie.Add(k6.Id, k6);
            kategorieNaPlatformie.Add(k7.Id, k7);
            kategorieNaPlatformie.Add(k8.Id, k8);
            kategorieNaPlatformie.Add(k9.Id, k9);
            //List<kategorie_klientow> kategorieWyfiltrowane = new List<kategorie_klientow>() { k2, k6, k7, k9 };
            
            Dictionary<int, KategoriaKlienta>kategorieWyfiltrowane = new Dictionary<int, KategoriaKlienta>();
            kategorieWyfiltrowane.Add(k2.Id,k2);
            kategorieWyfiltrowane.Add(k6.Id, k6);
            kategorieWyfiltrowane.Add(k7.Id, k7);
            kategorieWyfiltrowane.Add(k9.Id, k9);

            HashSet<int> doAktualizacji = new HashSet<int>();
            HashSet<int> doUsuniecia = new HashSet<int>();
            HashSet<int> doDodania = new HashSet<int>();

            kategorieNaPlatformie.Porownaj(kategorieWyfiltrowane, ref doAktualizacji, ref doDodania, ref doUsuniecia);
            Assert.True(doUsuniecia.Count==5);

        }

        [Fact(DisplayName = "Test sortowania modułow")]
        public void PrzesunModulNaKoniecTest()
        {
            List<IModulKlienci> modulty=new List<IModulKlienci>();
            modulty.Add(new KtorePolaSynchronizowac());
            modulty.Add(new DaneLogowaniaPoleWlasne());
            modulty.Add(new DodanieKupowanychIlosci());

            MainForm.PrzesunModulNaKoniec(modulty, typeof (KtorePolaSynchronizowac));

           Assert.Equal(typeof(KtorePolaSynchronizowac),modulty[2].GetType());
        }

        [Fact(DisplayName = "Test dla jednostek")]
        public void JednostkiWErpie()
        {
            JednostkaProduktu jpSystem = new JednostkaProduktu();
            JednostkaProduktu jpERp = new JednostkaProduktu();

            IConfigSynchro konf = A.Fake<IConfigSynchro>();
            A.CallTo(() => konf.JezykIDDomyslny).Returns(1);

            jpSystem.Id = 1739980013;
            jpSystem.Nazwa = "szt";
            jpSystem.Aktywna = true;
            jpSystem.Calkowitoliczowa = true;
            jpSystem.Komunikat = "test";

            jpERp.Id = 9153096501306629499;
            jpERp.Nazwa = "szt";
            jpERp.Aktywna = true;
            jpERp.Calkowitoliczowa = true;
            jpERp.Komunikat = "test";

            MainForm main = new MainForm();
            main.Konfiguracja = konf;
            Dictionary<long, Jednostka> slownikJednostekSystem = new Dictionary<long, Jednostka>() {{jpSystem.Id, jpSystem}};
            List<JednostkaProduktu> listaProduktow = new List<JednostkaProduktu>() { jpERp };
            var test = main.JednostkiWErpie(listaProduktow, slownikJednostekSystem);
            Assert.True(test.First(x=>x.Id== 1739980013).Aktywna);
            Assert.True(slownikJednostekSystem.Values.First(x => x.Id == 1739980013).Aktywna);
        }

        [Fact(DisplayName = "Test sprawdzający prawidłowe działanie metody która dezaktywuje/dodaje kraje")]
        public void OperacjeNaKrajachTest()
        {
            MainForm mf = new MainForm();
            //wykorzystywany tylko w kraju recznie dodanym
            Kraje k1 = new Kraje(1,"Kraj1","kraj1");
            //wykorzystywany w recznie dodanym i erp
            Kraje k3 = new Kraje(2, "Kraj3", "kraj3");
            //wykorzystywany tylko w erp
            Kraje k4 = new Kraje(3, "Kraj4", "kraj4");
            //niegdzie nie wykorzystywany (powinien zostać zdezaktywowany)
            Kraje k5 = new Kraje(4, "Kraj5", "kraj5");

            //kraje które sa z erpa (k1 i k5 pomijane bo sa tylko na platformie wykorzystywane lub nie ma ich wcale)
            List<Kraje>kraje = new List<Kraje>() {k3,k4};


            Adres a1= new Adres() {Id=-1, KrajId = 1};
            Adres a2 = new Adres() { Id = -2, KrajId = 2 };
            Adres a3 = new Adres() { Id = 1, KrajId = 2 };
            Adres a4 = new Adres() { Id = 2, KrajId = 3 };

            Dictionary<int, Kraje> krajeNaPlatfomie = new Dictionary<int, Kraje>() { {k1.Id,k1}, { k3.Id, k3 }, { k4.Id, k4 }, { k5.Id, k5 } };
            Dictionary<long, Adres> adresy = new Dictionary<long, Adres>() { { a1.Id, a1 }, { a2.Id, a2 }, { a3.Id, a3 }, { a4.Id, a4 } };


            mf.OperacjeNaKrajach(kraje, adresy, krajeNaPlatfomie);
            Assert.True(kraje.Count == 4);
            Assert.True(kraje.First(x=>x.Id==1).Widoczny && kraje.First(x => x.Id == 1).Symbol== "kraj1");
            Assert.True(!kraje.First(x => x.Id == 4).Widoczny && kraje.First(x => x.Id == 4).Symbol == "");
            Assert.True(kraje.Count(x=>x.Widoczny)==3);
        }

        [Fact(DisplayName = "Test sprawdzający poprawne działanie metody zwracające stany do usunięcia oraz aktualiująca stany które otrzymaliśmy")]
        public void PobierzStanyDoUsunieciaTest()
        {
            List<ProduktStan> listaStanowZErpaIModulow = new List<ProduktStan>();
            MainForm mf = new MainForm();

            //stan ten z listy stanow z erpa powinien być usuniety nie chcemy mieć stanów zerowych
            ProduktStan ps1 = new ProduktStan() {MagazynId = 1, Stan = 0, ProduktId = 1};
            listaStanowZErpaIModulow.Add(ps1 );

            //stan ten z listy stanow z erpa powinien być usuniety gdyż nie ma produktu o id 9999 na platformie
            ProduktStan ps2 = new ProduktStan() { MagazynId = 1, Stan = 10, ProduktId = 9999 };
            listaStanowZErpaIModulow.Add(ps2);

            //Prawidłowy stan nie ma go jeszcze na platformie
            ProduktStan ps3 = new ProduktStan() { MagazynId = 1, Stan = 10, ProduktId = 2 };
            listaStanowZErpaIModulow.Add(ps3);

            //Prawidłowy stan jest na platformie ale jego stan = 0
            ProduktStan ps4 = new ProduktStan() { MagazynId = 1, Stan = 10, ProduktId = 3 };
            listaStanowZErpaIModulow.Add(ps4);

            //Stan rowny 0 nie ma stanu na platformie - po przejsciu metody nie powinno byc go w stanach do wysłania
            ProduktStan ps5 = new ProduktStan() { MagazynId = 1, Stan = 0, ProduktId = 9998 };
            listaStanowZErpaIModulow.Add(ps5);

            //Prawidłowy stan - na platformie taki sam jak z erpa
            ProduktStan ps6 = new ProduktStan() { MagazynId = 1, Stan = 10, ProduktId = 4 };
            listaStanowZErpaIModulow.Add(ps6);


            //Na liscie stanow z erpa powinny zostać dwa stany ps3 oraz ps4
            Dictionary<long, ProduktStan> slownikStanowZPlatformy =new Dictionary<long, ProduktStan>();

            //stan zmienia sie z 10 na zero wiec z platformy powinien byc usuniety stan ps1
            slownikStanowZPlatformy.Add(ps1.Id, new ProduktStan { MagazynId = 1, Stan = 10, ProduktId = 1 });

            //Stan produktu gdzie stan = 0 sprawdzamy czy prawidłowo zostanie zaktualizowany do 10
            slownikStanowZPlatformy.Add(ps4.Id, new ProduktStan{ MagazynId = 1, Stan = 0, ProduktId = 3 });

            //Keidys produkt mial stan 10 teraz ma zero wiec stand ten powienien zostac wywalony
            slownikStanowZPlatformy.Add(ps6.Id, new ProduktStan { MagazynId = 1, Stan = 10, ProduktId = 4 });


            HashSet<long>idProduktowAktywnychNaPlatformie = new HashSet<long>() {1,2,3,4};

            //Powinno być zwrócowne tylko dla id ps1 - gdyz jako jedyny jest na platformie a jego stan jest 0 
            HashSet<long> idStanowDoUsuniecia = mf.PobierzStanyDoUsuniecia(ref listaStanowZErpaIModulow, slownikStanowZPlatformy, idProduktowAktywnychNaPlatformie);

            Assert.True(idStanowDoUsuniecia.Count==1);
            Assert.True(idStanowDoUsuniecia.First()==ps1.Id);


            Assert.True(listaStanowZErpaIModulow.Count==2);
            Assert.True(listaStanowZErpaIModulow.First(x=>x.ProduktId==2).Stan==10);
            Assert.True(listaStanowZErpaIModulow.First(x => x.ProduktId == 3).Stan == 10);


        }



    }
}
