using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Modele;
using SolEx.Hurt.Model.Web;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class KonfigurowalneUwagiTests
    {
        [Fact()]
        public void KonfigurowalneUwagiZwrocKlientaTest()
        {
            var koszyk = A.Fake<IKoszykiBLL>();
            var pozycja1 = A.Fake<IKoszykPozycja>();
            IKlient klient = koszyk.PierwotnyKlient();
            A.CallTo(() => koszyk.PierwotnyKlient()).Returns(klient);

            pozycja1.przedstawiciel_nazwa = "Nazwa przedstawiciela1";
            A.CallTo(() => pozycja1.CenaNetto).Returns(5);
            
            var pozycja2 = A.Fake<IKoszykPozycja>();
            A.CallTo(() => pozycja2.CenaNetto).Returns(1000);
            
            koszyk.Pozycje.Add(pozycja1);
            koszyk.Pozycje.Add(pozycja2);

            A.CallTo(() => koszyk.SlownikParametrow.Uwagi).Returns("Słownik parametrów i jego uwagi");
            A.CallTo(() => koszyk.WagaCalokowita.ToString()).Returns("11");
            A.CallTo(() => koszyk.PierwotnyKlient().email).Returns("pierwotny@mailklienta.pl");
            A.CallTo(() => koszyk.PierwotnyKlient().nazwa).Returns("pierwotnaNazwaKlienta");
            A.CallTo(() => koszyk.Platnosc).Returns("Przelew");
            //A.CallTo(() => koszyk.SlownikParametrow.NumerZamowienia).Returns("1234");
            //A.CallTo(() => koszyk.SlownikParametrow.TerminDostawy.Value).Returns(new DateTime(2020, 6, 1));
            
            var modul = new KonfigurowalneUwagi();
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            modul.ZwrocKlienta(koszyk, tmp);

            Assert.Equal(tmp["{przedstawiciel}"], "Nazwa przedstawiciela1" );
            Assert.Equal(tmp["{uwagi_klienta}"], "Słownik parametrów i jego uwagi");
            Assert.Equal(tmp["{laczna_waga}"], "11");
            Assert.Equal(tmp["{klient_email}"], "pierwotny@mailklienta.pl");
            Assert.Equal(tmp["{klient_nazwa}"], "pierwotnaNazwaKlienta");
            Assert.Equal(tmp["{liczba_produktow}"], "2");
            Assert.Equal(tmp["{najtanszy_produkt}"], "5,00");
            Assert.Equal(tmp["{najdrozszy_produkt}"], "1000,00");
            Assert.Equal(tmp["{sposob_platnosci}"], "Przelew");
            //Assert.Equal(tmp["{numer_zamowienia_klienta}"], "1234");
            //Assert.Equal(tmp["{termin_dostawy}"], "2015-09-09");
        }
        
        [Fact()]
        public void KonfigurowalneUwagiZwrocDostaweTest()
        {
            var koszyk = A.Fake<IKoszykiBLL>();
            ISposobDostawy dostawa = koszyk.KosztDostawy();
            ProduktBazowy produkt = koszyk.KosztDostawy().ProduktDostawy;
            produkt.kod = "kodkodkod";
            
            A.CallTo(() => koszyk.KosztDostawy()).Returns(dostawa);
            A.CallTo(() => koszyk.KosztDostawy().ProduktDostawy).Returns(produkt);
            A.CallTo(() => koszyk.KosztDostawy().OpisDostawy).Returns("Opis dostawy");

            var modul = new KonfigurowalneUwagi();
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            modul.ZwrocDostawe(koszyk, tmp);

            Assert.Equal(tmp["{dostawa_nazwa}"], "kodkodkod");
            Assert.Equal(tmp["{dostawa_nazwa_opis}"], "Opis dostawy");
        }

        [Fact()]
        public void KonfigurowalneUwagiZwrocAdresTest()
        {
            var koszyk = A.Fake<IKoszykiBLL>();
            var adres = A.Fake<Adres>();
            adres.Nazwa = "Nazwa";
            adres.Telefon = "700700700";
            adres.KodPocztowy = "00-999";
            adres.Miasto = "Warszawa";
            adres.UlicaNr = "Sezamkowa 18";

            A.CallTo(() => koszyk.Adres).Returns(adres);

            var modul = new KonfigurowalneUwagi();
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            modul.ZwrocAdres(koszyk, tmp);
            //Assert.Equal(tmp["{adres_dostawy}"], "Nazwa, Sezamkowa 18, 00-999 Warszawa, ");
            Assert.Equal(tmp["{adres_dostawy_nazwa}"], "Nazwa");
            Assert.Equal(tmp["{adres_telefon}"], "700700700");
            Assert.Equal(tmp["{adres_dostawy_kod_pocztowy}"], "00-999");
            Assert.Equal(tmp["{adres_dostawy_miasto}"], "Warszawa");
            Assert.Equal(tmp["{adres_dostawy_ulica}"], "Sezamkowa 18");
        }

        [Fact()]
        public void KonfigurowalneUwagiZwrocDokumentyTest()
        {
            var koszyk = A.Fake<IKoszykiBLL>();
            var dokumenty = A.Fake<IDokumenty>();
            Dictionary<string, string> tmp = new Dictionary<string, string>();

            DocumentSummary ds = new DocumentSummary();
            var danedowykresuNiezaplacone = new DaneDoWykresu();
            danedowykresuNiezaplacone.IloscPozycji = 5;
            danedowykresuNiezaplacone.Cena = 998;

            var danedowykresuZaplacone = new DaneDoWykresu();
            danedowykresuZaplacone.IloscPozycji = 15;
            danedowykresuZaplacone.Cena = 1998;

            ds.Niezaplacone = danedowykresuNiezaplacone;
            ds.Zaplacone = danedowykresuZaplacone;
            
            A.CallTo(() => dokumenty.PobierzPodsumowanieFakturKlient(null)).Returns(ds);
            
            var modul = new KonfigurowalneUwagi();
            modul.dokumenty = dokumenty;
            modul.ZwrocDokumenty(koszyk, tmp);
            Assert.Equal(tmp["{dokumenty_niezaplacone_liczba}"], "");
            Assert.Equal(tmp["{dokumenty_niezaplacone_wartosc}"], "");
            Assert.Equal(tmp["{dokumenty_przeterminowane_liczba}"], "");
            Assert.Equal(tmp["{dokumenty_przeterminowane_wartosc}"], "");
        }

        [Fact()]
        public void KonfigurowalneUwagiZwrocDanePlatnikaTest()
        {
            var koszyk = A.Fake<IKoszykiBLL>();
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            var slownik = A.Fake<SlownikParametrowKoszyka>();
            slownik.PlatnikNazwa = "Jakas nazwa";
            slownik.PlatnikAdres = "Fajna 1";
            slownik.PlatnikKodPocztowy = "42-200";
            slownik.PlatnikKraj = "Polska";
            slownik.PlatnikMiasto = "Kraków";
            slownik.PlatnikNip = "987123321";
            slownik.PlatnikTelefon = "999-000-333";
            A.CallTo(() => koszyk.SlownikParametrow).Returns(slownik);
            var modul = new KonfigurowalneUwagi();
            modul.ZwrocDanePlatnika(koszyk,tmp);
            Assert.Equal(tmp["{platnik_dane}"], " Jakas nazwa nip: 987123321 telefon 999-000-333 adres Fajna 1 42-200 Kraków Polska"); 
        }

        [Fact()]
        public void KonfigurowalneUwagiZwrocProduktyAutomatyczneTest()
        {
            var koszyk = A.Fake<IKoszykiBLL>();
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            
            IKoszykPozycja pozycja1 = A.Fake<IKoszykPozycja>();
            pozycja1.TypPozycji = TypPozycjiKoszyka.Automatyczny;
            pozycja1.Produkt.nazwa = "Buty kalkuty";
            pozycja1.ilosc = 10;
            pozycja1.Jednostka.Nazwa = "par";
            
            IKoszykPozycja pozycja2 = A.Fake<IKoszykPozycja>();
            pozycja2.TypPozycji = TypPozycjiKoszyka.Automatyczny;
            pozycja2.Produkt.nazwa = "Długopis";
            pozycja2.ilosc = 100;
            pozycja2.Jednostka.Nazwa = "szt.";
            
            IKoszykPozycja pozycja3 = A.Fake<IKoszykPozycja>();
            pozycja3.TypPozycji = TypPozycjiKoszyka.Gratis;
            pozycja3.Produkt.nazwa = "Temperówka";
            pozycja3.ilosc = 20;
            pozycja3.Jednostka.Nazwa = "szt.";

            List<IKoszykPozycja> lista = new List<IKoszykPozycja>();
            lista.Add(pozycja1);
            lista.Add(pozycja2);
            lista.Add(pozycja3);

            A.CallTo(() => koszyk.Pozycje).Returns(lista);

            var modul = new KonfigurowalneUwagi();
            modul.ZwrocProduktyAutomatyczne(koszyk, tmp);

            Assert.Equal(tmp["{produkty_automatycznie_dodane}"], "Buty kalkuty:10 parDługopis:100 szt."); 
        }

        [Fact()]
        public void KonfigurowalneUwagiZwrocOsobeZamawiajacaWImieniuKlientaTest()
        {
            var koszyk = A.Fake<IKoszykiBLL>();
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            
            var sesja = A.Fake<ISesjaHelper>();
            A.CallTo(() => sesja.KlientID).Returns(1);
            
            var klienciDostep = A.Fake<IKlienciDostep>();
            var klient = A.Fake<IKlient>();
            klient.nazwa = "Marian Kowalski";
            A.CallTo(() => sesja.PrzedstawicielID).Returns(1);
            A.CallTo(() => klienciDostep.Pobierz(A<int>.Ignored)).Returns(klient);
            
            var modul = new KonfigurowalneUwagi();
            modul.SHelper = sesja;
            modul.Klienci = klienciDostep;
            modul.ZwrocOsobeZamawiajacaWImieniuKlienta(koszyk, tmp);
            
            Assert.Equal(tmp["{Nazwa_osoby_zamawiajacej_w_imieniu_klienta}"], "Marian Kowalski");
            Assert.Equal(tmp["{Inicjaly_osoby_zamawiajacej_w_imieniu_klienta}"], "MK");
        }

        [Fact()]
        public void ZwrocDaneOfertyTest()
        {
            var koszyk = A.Fake<IKoszykiBLL>();

            var pozycjae = new List<IKoszykPozycja>();
            IKoszykPozycja pozycjaKoszyka = A.Fake<IKoszykPozycja>();
            pozycjaKoszyka.produkt_id = 1;
            IProduktKlienta pk = A.Fake<IProduktKlienta>();
            pk.kod = "k1";
            pk.produkt_id = 1;
            A.CallTo(() => pozycjaKoszyka.Jednostka).Returns(new JednostkaProduktu(true, 1, 1, "szt", 1));
            A.CallTo(() => pozycjaKoszyka.Produkt).Returns(pk);

            pozycjae.Add(pozycjaKoszyka);
            A.CallTo(() => koszyk.Pozycje).Returns(pozycjae);
            var dokumenty = A.Fake<IDokumenty>();
            Dictionary<string, string> tmp = new Dictionary<string, string>();

           Dictionary<int,IDokument> wynik=new Dictionary<int, IDokument>();


            A.CallTo(() => dokumenty.PobierzDokumenty(A<RodzajDokumentu>.Ignored, A<int>.Ignored)).Returns(wynik);

            var modul = new KonfigurowalneUwagi();
            modul.dokumenty = dokumenty;
            modul.ZwrocDaneOferty(koszyk, tmp);
            Assert.Equal(tmp["{ProduktyOferta}"], "");


            IDokument dok = A.Fake<IDokument>();
            A.CallTo(() => dok.DokumentId).Returns(1);
            List<IDokumentPozycja> pozycje=new List<IDokumentPozycja>();
            IDokumentPozycja pozycja = A.Fake<IDokumentPozycja>();
            A.CallTo(() => pozycja.PozycjaDokumentuIdProduktu).Returns(1);
            pozycje.Add(pozycja);
            IDokumentPozycja pozycja2 = A.Fake<IDokumentPozycja>();
            A.CallTo(() => pozycja2.PozycjaDokumentuIdProduktu).Returns(2);
            pozycje.Add(pozycja2);
            A.CallTo(() => dok.PobierzPozycjeDokumentu()).Returns(pozycje);
            wynik.Add(dok.DokumentId,dok);
            tmp=new Dictionary<string, string>();
            modul.ZwrocDaneOferty(koszyk, tmp);
            Assert.NotEqual(tmp["{ProduktyOferta}"], "");
        }
    }
}
