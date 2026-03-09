using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FakeItEasy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class SztucznaDostepnoscTests
    {
        /// <summary>
        /// Test który ma 3 zestawy składające się z 2*kola,1*rama,1*siodelko - stan zestawów to 3(1 sztuka rower1 i 2 sztuki rower2). 
        /// Po rozłożeniu otrzymać powinniśmy 28 sztuk kół (10 na mag1, 12 na mag2 oraz 6 z zestawow(3 zestawy* 2kola)), 11ram (8 na stanie + 3 z zestawów (w sklad zestawu wchodzi 1 sztuka razy 3 zestawy), 6 siodeł (3+3zestaw)
        /// </summary>
        [Fact(DisplayName = "Test sprawdzający poprawne składanie/rozkładanie zestawów")]
        public void PrzetworzTest()
        {
            Cecha c1 = new Cecha() { Id=1, Nazwa="2*Kola;rama;siodelko", AtrybutId = 1};

            Produkt p1 = new Produkt() { Id = 1, Nazwa = "Kola" };
            Produkt p2 = new Produkt() { Id = 2, Nazwa = "rama" };
            Produkt p3 = new Produkt() { Id = 3, Nazwa = "siodelko" };
            Produkt p4 = new Produkt() { Id = 4, Nazwa = "rower1" };
            Produkt p5 = new Produkt() { Id = 5, Nazwa = "rower2" };

            List<Produkt> listaProduktow = new List<Produkt>() {p1, p2, p3, p4, p5};
            
            ProduktStan ps1 = new ProduktStan() {MagazynId=1, ProduktId = 1, Stan = 10};
            ProduktStan ps2 = new ProduktStan() { MagazynId = 1, ProduktId = 2, Stan = 8 };
            ProduktStan ps3 = new ProduktStan() { MagazynId = 1, ProduktId = 3, Stan = 3 };
            ProduktStan ps4 = new ProduktStan() { MagazynId = 1, ProduktId = 4, Stan = 1 };
            ProduktStan ps5 = new ProduktStan() { MagazynId = 1, ProduktId = 5, Stan = 2 };
            ProduktStan ps6 = new ProduktStan() { MagazynId = 2, ProduktId = 1, Stan = 12 };

            Dictionary<int, List<ProduktStan>> stanyProduktow = new Dictionary<int, List<ProduktStan>>();
            stanyProduktow.Add(1, new List<ProduktStan>() { ps1, ps2, ps3, ps4, ps5 });
            stanyProduktow.Add(2, new List<ProduktStan>() {ps6});

            ProduktCecha cp1 = new ProduktCecha() {ProduktId = 4, CechaId = 1};
            ProduktCecha cp2 = new ProduktCecha() { ProduktId = 8, CechaId = 2 };
            ProduktCecha cp3 = new ProduktCecha() {  ProduktId = 5, CechaId = 1 };


            Dictionary<long, ProduktCecha> slownikCechyProdukty = new Dictionary<long, ProduktCecha>();
            slownikCechyProdukty.Add(cp1.Id, cp1);
            slownikCechyProdukty.Add(cp2.Id, cp2);
            slownikCechyProdukty.Add(cp3.Id, cp3);

            //slownikCechyProdukty.Add(cp4.produkt_id, cp4);

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzCechyProdukty(A<HashSet<long>>.Ignored,A<int>.Ignored)).Returns(slownikCechyProdukty);
            
            SztucznaDostepnosc sztuczna = A.Fake<SztucznaDostepnosc>();
            sztuczna.Atrybut=new HashSet<int>(){1};
            sztuczna.CzyLiczyc = true;
            sztuczna.Separator=";";
            sztuczna.Magazyn = 1;
            sztuczna.Pole = "Nazwa";
            A.CallTo(() => sztuczna.CechyNaB2B).Returns(new Dictionary<long, Cecha>() { { c1.Id, c1 } });
            sztuczna.ApiWywolanie = api;

            sztuczna.Przetworz(ref stanyProduktow, new List<Magazyn>(), listaProduktow);
            Assert.True(stanyProduktow[1][0].Stan==28);
            Assert.True(stanyProduktow[1][1].Stan == 11);
            Assert.True(stanyProduktow[1][2].Stan == 6);
            //4 ze wzgledy na fakt ze kol jest 22 ram jest 11 ale tylko 6 siodelka sa ale jest jeden rower  + 1 sztuki tyle było zestawów
            Assert.True(stanyProduktow[1][3].Stan == 7);
            //4 ze wzgledy na fakt ze kol jest 22 ram jest 11 ale tylko 6 siodelka sa ale jest jeden rower  + 2 sztuki tyle było zestawów
            Assert.True(stanyProduktow[1][4].Stan == 8);
            //tyle samo gdyż zwiekszamy stan tylko dla magazyny o id 1
            Assert.True(stanyProduktow[2][0].Stan == 12);
        }

        private Regex myRegex = new Regex(@"^[\d]*\,*[\d]{1,4}");

        [Fact(DisplayName = "Test sprawdzająćy poprawne pobieranie danych dla składnika")]
        public void PobierzDaneSkladnikaTest()
        {
            //Test dla "1*Koło" ilosc powinna byc 1 a wartosc Koło
            string s = "1*Koło";
            decimal ilosc;
            SztucznaDostepnosc sztuczna = A.Fake<SztucznaDostepnosc>();
            string nazwa = sztuczna.PobierzDaneSkladnika(s, myRegex, out ilosc);
            Assert.True(nazwa=="Koło");
            Assert.True(ilosc == 1);

            //Test dla "Koło" ilosc powinna byc 1 (domyślnie jeżeli brak wartości to ilosc - 1) a wartosc Koło
            s = "Koło";
            nazwa = sztuczna.PobierzDaneSkladnika(s, myRegex, out ilosc);
            Assert.True(nazwa == "Koło");
            Assert.True(ilosc == 1);

            //Test dla "2*siodła" ilosc powinna byc 2 a wartosc siodła
            s = "2*siodła";
            nazwa = sztuczna.PobierzDaneSkladnika(s, myRegex, out ilosc);
            Assert.True(nazwa == "siodła");
            Assert.True(ilosc == 2);
        }

        [Fact(DisplayName = "Test sprawdzający pobranie ilości sztuk zestawu z konkretna cecha")]
        public void PobierzSumeProduktowZTakimSamymSklademTest()
        {
            HashSet<long>idProduktowZCecha = new HashSet<long>() {1,2};
            //Przypadek gdy nie ma w slowniku zestawów produktu co powinno skutkować tym ze dla brakującego produktu stan jest 0
            Dictionary<long,decimal>slownikStanowZestawow= new Dictionary<long, decimal>() { {1,10}, {3,15} };
            SztucznaDostepnosc sztuczna = A.Fake<SztucznaDostepnosc>();

            //10sz - 1 + 0szt - 2 = 10
            Assert.True(sztuczna.PobierzSumeProduktowZTakimSamymSkladem(idProduktowZCecha,slownikStanowZestawow)==10);


            //Musimy ponownie wpisać stan dla zestawu 1 bo przez referencje została wyzerowana
            slownikStanowZestawow[1] = 10;
            //dodajemy do slownika stan dla zestawu 2
            slownikStanowZestawow.Add(2,20);
            //10sz - 1 + 20szt - 2 = 30
            Assert.True(sztuczna.PobierzSumeProduktowZTakimSamymSkladem(idProduktowZCecha, slownikStanowZestawow) == 30);

        }

        [Fact(DisplayName = "Test sprawdzająćy poprawne wyliczanie możliwej do zbudowania ilości zestawów")]
        public void PoliczIloscMozliwychZestawowDoZbudowaniaTest()
        {
            int? iloscProduktu = null;

            Dictionary<long, decimal> potrzebneTowary = new Dictionary<long, decimal>() { {1,2}, {2,1}, {3,1} };
            SztucznaDostepnosc sztuczna = A.Fake<SztucznaDostepnosc>();
            sztuczna.Magazyn = 1;

            
            Dictionary<int, List<ProduktStan>> listaWejsciowa = new Dictionary<int, List<ProduktStan>>() { {1, new List<ProduktStan>()} };
            listaWejsciowa[1].Add(new ProduktStan() {ProduktId = 1, Stan = 28});
            listaWejsciowa[1].Add(new ProduktStan() { ProduktId = 2, Stan = 11 });
            listaWejsciowa[1].Add(new ProduktStan() { ProduktId = 3, Stan = 6 });

            //kola 28:2 - 14, rama - 11:1=11 siodelko - 6:1= 6    W rezultacie mozna złożyć max 6 zaestwów

            sztuczna.PoliczIloscMozliwychZestawowDoZbudowania(potrzebneTowary,listaWejsciowa,ref iloscProduktu,new HashSet<long>());
            Assert.True(iloscProduktu ==6 );

            //kola 28:2 - 14, rama - 11:2=5 siodelko - 6:1= 6    W rezultacie mozna złożyć max 5 zaestwów
            potrzebneTowary = new Dictionary<long, decimal>() { { 1, 2 }, { 2, 2 }, { 3, 1 } };
            sztuczna.PoliczIloscMozliwychZestawowDoZbudowania(potrzebneTowary, listaWejsciowa, ref iloscProduktu, new HashSet<long>());
            Assert.True(iloscProduktu == 5);

        }





        [Fact(DisplayName = "test sprawdzający poprawne pobieranie cech gdzie w zawartości jest zestaw")]
        public void PobierzIdCechKtoreMajaZestawowTest()
        {
            Dictionary<long, Cecha> cechyB2B = new Dictionary<long, Cecha>();
            HashSet< long > idZestawow = new HashSet<long>();
            List<Produkt> produkty = new List<Produkt>();

            //Cecha z zestawem
            cechyB2B.Add(5, new Cecha("1*rower1",""));

            //cecha bez zestawu
            cechyB2B.Add(1, new Cecha("2*kolo", ""));

            //zestaw
            produkty.Add(new Produkt() {Id=1, Nazwa = "rower1" });
            
            //produkt
            produkty.Add(new Produkt() { Id = 2, Nazwa = "kolo" });

            idZestawow.Add(1);

            SztucznaDostepnosc sztuczna = A.Fake<SztucznaDostepnosc>();
            sztuczna.Pole = "Nazwa";
            HashSet<long> wynik = sztuczna.PobierzIdCechKtoreMajaZestawow(cechyB2B, idZestawow, myRegex, produkty, new Dictionary<string, long>() { { "rower1",1}, { "kolo",2 } });
            Assert.True(wynik.Count==1);
            Assert.True(wynik.First() == 5);
        }
        [Fact(DisplayName = "test sprawdzający poprawne pobieranie cech gdzie w zawartości jest produkt którego składaliśmy")]
        public void PobierzZestawySkladajaceSieZProduktuTest()
        {
            List<Cecha>cechyB2B = new List<Cecha>();
            //Cecha z zestawem
            cechyB2B.Add(new Cecha("1*rower1", "") {Id=5});

            //cecha bez zestawu
            cechyB2B.Add(new Cecha("2*kolo", "") { Id = 1 });

            SztucznaDostepnosc sztuczna = A.Fake<SztucznaDostepnosc>();
            List<long>wynik = sztuczna.PobierzZestawySkladajaceSieZProduktu("kolo", cechyB2B, myRegex);

            Assert.True(wynik.Count==1);
            Assert.True(wynik[0]==1);
        }

        [Fact(DisplayName = "Test sprawdzający poprawne składanie/rozkładanie zestawów gdzie w zestawie jest produkt którego nie ma na b2b")]
        public void PrzetworzTestBrakProduktuNaB2b()
        {
            Cecha c1 = new Cecha() { Id = 1, Nazwa = "2*prod2", AtrybutId = 1 };
            Cecha c2 = new Cecha() { Id = 2, Nazwa = "2*prod3", AtrybutId = 1 };
            
            //zestaw składający się z 2 prod2
            Produkt p1 = new Produkt() { Id = 1, Nazwa = "prod1" };

            //zestaw składający się z 2 prod3
            Produkt p2 = new Produkt() { Id = 2, Nazwa = "prod2" };
            //produkt ostateczny
            Produkt p3 = new Produkt() { Id = 3, Nazwa = "prod3" };

            List<Produkt> listaProduktow = new List<Produkt> {p1,p2};

            ProduktStan ps1 = new ProduktStan() { MagazynId = 1, ProduktId = 1, Stan = 10 };
            ProduktStan ps2 = new ProduktStan() { MagazynId = 1, ProduktId = 2, Stan = 8 };
            Dictionary<int, List<ProduktStan>> stanyProduktow = new Dictionary<int, List<ProduktStan>>() ;
            stanyProduktow.Add(1, new List<ProduktStan>() { ps1, ps2 });


            ProduktCecha cp1 = new ProduktCecha() { ProduktId = 1, CechaId = 1 };
            ProduktCecha cp2 = new ProduktCecha() { ProduktId = 2, CechaId = 2 };

            Dictionary<long, ProduktCecha> slownikCechyProdukty = new Dictionary<long, ProduktCecha>() { { cp1.Id , cp1}, { cp2.Id, cp2 } };

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzCechyProdukty(A<HashSet<long>>.Ignored, A<int>.Ignored)).Returns(slownikCechyProdukty);
            SztucznaDostepnosc sztuczna = A.Fake<SztucznaDostepnosc>();
            sztuczna.Atrybut = new HashSet<int>() { 1 };
            sztuczna.CzyRozkladacZestaw = false;
            sztuczna.CzyLiczyc = true;
            sztuczna.Separator = ";";
            sztuczna.Magazyn = 1;
            sztuczna.Pole = "Nazwa";
            A.CallTo(() => sztuczna.CechyNaB2B).Returns(new Dictionary<long, Cecha>() { { c1.Id, c1 }, { c2.Id, c2 } });
            sztuczna.ApiWywolanie = api;

            sztuczna.Przetworz(ref stanyProduktow, new List<Magazyn>(), listaProduktow);

            //Stan powinien wynosić 14 bo 10 jest bazowego stanu a 8 sztuk jest prod2 do zbudowania potrzeba 2 sztuk wiec 10+8/2 = 14
            Assert.True(stanyProduktow[1][0].Stan==14);

            //ten zestaw składa się z zestawu prod3 jednak jego nie ma na platformie a wiec ilosc = 0 wiec stan wynosi tyle co stan produktu
            Assert.True(stanyProduktow[1][1].Stan == 8);

        }

    }
}
