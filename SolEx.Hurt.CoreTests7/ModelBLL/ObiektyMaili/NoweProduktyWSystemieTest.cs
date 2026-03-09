using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using Xunit;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.CoreTests.ModelBLL.ObiektyMaili
{
    public class NoweProduktyWSystemieTest
    {
        
    }


    public class MailNoweProduktTest : MailNoweProdukty
    {

        private string baza = ":memory:";
        private OrmLiteConnectionFactory polaczenie;


        [Fact(DisplayName = "Test sprawdzający poprawne filtrowanie produktów dla maila o nowych produtach")]
        public void PobierzWyfiltrowaneProdukty2()
        {
            List<ProduktBazowy>listaProduktow = WygenerujProduktyBazowe();
            A.CallTo(Calosc.DostepDane).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<ProduktBazowy>>().WithAnyArguments().Returns(listaProduktow);

            Dictionary<string, List<ProduktBazowy>> wyfiltrowaneProdukty = PobierzWyfiltrowaneProdukty(new long[] {1}, new long[] {2});
            //Filtrowanie spełnione jest przez 4 produkty jednak dwa są w jednej rodzinie przez co jeden jest pomijany
            Assert.True(wyfiltrowaneProdukty.Count==2);
            Assert.True(wyfiltrowaneProdukty[""].Count == 2);
            Assert.True(wyfiltrowaneProdukty["rodzina"].Count == 2);

        }
        //todo Do dopracowania
        //[Fact(DisplayName = "Test sprawdzający poprawne pobieranie produktów które były juz wysłane klientowi")]
        //public void PobierzWyslaneProduktyDlaKlientowTest()
        //{
        //    ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            
        //    DostepDoDanych dostep = new DostepDoDanych(Calosc);
        //    A.CallTo(() => Calosc.Statystyki).Returns(new Statystyki(Calosc));

        //    //Prawidłow działanie - powinno być pobrane
        //    DzialaniaUzytkownikow du = new DzialaniaUzytkownikow();
        //    du.Zdarzenie=ZdarzenieGrupa.Produkty;
        //    du.EmailKlienta = "test@solex.net.pl";
        //    du.Data = DateTime.Now.AddDays(-10);
        //    du.ZdarzenieGlowne = ZdarzenieGlowne.NoweProduktyWSystemie;


        //    DzialaniaUzytkwonikowParametry dup = new DzialaniaUzytkwonikowParametry();
        //    dup.NazwaParametru = "Produkty id";
        //    dup.IdDzialania = 1;
        //    dup.Wartosc = "1,2,3";

        //    //Nie pobrane ze względu na date
        //    DzialaniaUzytkownikow du2 = new DzialaniaUzytkownikow();
        //    du2.Zdarzenie = ZdarzenieGrupa.Produkty;
        //    du2.EmailKlienta = "test@solex.net.pl";
        //    du2.Data = DateTime.Now.AddDays(-20);
        //    du2.ZdarzenieGlowne = ZdarzenieGlowne.NoweProduktyWSystemie;


        //    DzialaniaUzytkwonikowParametry dup2 = new DzialaniaUzytkwonikowParametry();
        //    dup2.NazwaParametru = "Produkty id";
        //    dup2.IdDzialania = 2;
        //    dup2.Wartosc = "4,5";


        //    //Poprawne z innym klientem
        //    DzialaniaUzytkownikow du3 = new DzialaniaUzytkownikow();
        //    du3.Zdarzenie = ZdarzenieGrupa.Produkty;
        //    du3.EmailKlienta = "test2@solex.net.pl";
        //    du3.Data = DateTime.Now.AddDays(-10);
        //    du3.ZdarzenieGlowne = ZdarzenieGlowne.NoweProduktyWSystemie;


        //    DzialaniaUzytkwonikowParametry dup3 = new DzialaniaUzytkwonikowParametry();
        //    dup3.NazwaParametru = "Produkty id";
        //    dup3.IdDzialania = 3;
        //    dup3.Wartosc = "11,12,13";

        //    //Poprawne z pierwszym klintem
        //    DzialaniaUzytkownikow du4 = new DzialaniaUzytkownikow();
        //    du4.Zdarzenie = ZdarzenieGrupa.Produkty;
        //    du4.EmailKlienta = "test@solex.net.pl";
        //    du4.Data = DateTime.Now.AddDays(-10);
        //    du4.ZdarzenieGlowne = ZdarzenieGlowne.NoweProduktyWSystemie;


        //    DzialaniaUzytkwonikowParametry dup4 = new DzialaniaUzytkwonikowParametry();
        //    dup4.NazwaParametru = "Produkty id";
        //    dup4.IdDzialania = 4;
        //    dup4.Wartosc = "7,8,9";

        //    dostep.DbFactory = polaczenie;
        //    dostep.BindSelect<DzialaniaUzytkownikow>(SposobCachowania.Brak, null, null, null, SolexBllCalosc.PobierzInstancje.Statystyki.PobierzParametry);
        //    A.CallTo(() => Calosc.DostepDane).Returns(dostep);
        //    using (var db = polaczenie.OpenDbConnection())
        //    {
        //        db.CreateTable<DzialaniaUzytkownikow>();
        //        db.CreateTable<DzialaniaUzytkwonikowParametry>();

        //        db.Insert(du);
        //        db.Insert(du2);
        //        db.Insert(du3);
        //        db.Insert(du4);

        //        db.Insert(dup);
        //        db.Insert(dup2);
        //        db.Insert(dup3);
        //        db.Insert(dup4);
        //    }

        //    //A.CallTo(()=>calosc.Statystyki).Returns(new )

        //    var wynik = PobierzWyslaneProduktyDlaKlientow(DateTime.Now.AddDays(-15));
        //    Assert.True(wynik.Count==2);
        //    Assert.True(wynik["test@solex.net.pl"].Count == 6);
        //    Assert.True(wynik["test2@solex.net.pl"].Count == 3);


            
        //}


        public MailNoweProduktTest():base(null)
        {
            base.Calosc = A.Fake<ISolexBllCalosc>();
            polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider)
            {
                DialectProvider = { UseUnicode = true },
                AutoDisposeConnection = false
            };
        }



        private List<ProduktBazowy> WygenerujProduktyBazowe()
        {
            //Produkt nie spełnia filtrowania ma wysłanego maila o nowych produktach
            ProduktBazowy pb1 =new ProduktBazowy() {Id=1, Widoczny = true, WyslanoMailNowyProdukt = true, Zdjecia = new List<IObrazek>() {new Obrazek() {Id = 1} }, ZdjecieGlowne = new Obrazek(), IdCechPRoduktu = new HashSet<long>(){ 1}, Kategorie = new List<KategorieBLL>() {new KategorieBLL() {Id = 1, Widoczna = true} } };

            //Produkt nie spełnia filtrowania jest niewidoczny
            ProduktBazowy pb2 = new ProduktBazowy() { Id = 2, Widoczny = false, Zdjecia = new List<IObrazek>() { new Obrazek() { Id = 1 } }, WyslanoMailNowyProdukt = false, ZdjecieGlowne = new Obrazek(), IdCechPRoduktu = new HashSet<long>() { 1 }, Kategorie = new List<KategorieBLL>() { new KategorieBLL() { Id = 1, Widoczna = true } } };

            //Produkt nie spełnia filtrowania brak zdjęcia głównego
            ProduktBazowy pb3 = new ProduktBazowy() {Id = 3, Widoczny = true, Zdjecia = new List<IObrazek>() { new Obrazek() { Id = 1 } }, WyslanoMailNowyProdukt = false, ZdjecieGlowne = null, IdCechPRoduktu = new HashSet<long>() { 1 }, Kategorie = new List<KategorieBLL>() {new KategorieBLL() {Id = 1, Widoczna = true}}};

            //Produkt nie spełnia filtrowania brak wymaganej cechy
            ProduktBazowy pb4 = new ProduktBazowy() { Id = 4, Widoczny = true, Zdjecia = new List<IObrazek>() { new Obrazek() { Id = 1 } }, WyslanoMailNowyProdukt = false, ZdjecieGlowne = new Obrazek(), Kategorie = new List<KategorieBLL>() { new KategorieBLL() { Id = 1, Widoczna = true } }, IdCechPRoduktu = new HashSet<long>() { 3 } };

            //Produkt nie spełnia filtrowania posiada wymagana ceche ale posiada tez zabroniona
            ProduktBazowy pb5 = new ProduktBazowy() { Id = 5, Widoczny = true, Zdjecia = new List<IObrazek>() { new Obrazek() { Id = 1 } }, WyslanoMailNowyProdukt = false, ZdjecieGlowne = new Obrazek(), Kategorie = new List<KategorieBLL>() { new KategorieBLL() { Id = 1, Widoczna = true } }, IdCechPRoduktu = new HashSet<long>() { 1,2 }};

            //Produkt spełnia filtrowanie nie posiada rodziny
            ProduktBazowy pb6 = new ProduktBazowy() { Id = 6, Widoczny = true, Zdjecia = new List<IObrazek>() { new Obrazek() { Id = 1 } }, WyslanoMailNowyProdukt = false, ZdjecieGlowne = new Obrazek(), Kategorie = new List<KategorieBLL>() { new KategorieBLL() { Id = 1, Widoczna = true } }, IdCechPRoduktu = new HashSet<long>() { 1 } };

            //Produkt spełnia filtrowanie nie posiada rodziny
            ProduktBazowy pb7 = new ProduktBazowy() { Id = 7, Widoczny = true, Zdjecia = new List<IObrazek>() { new Obrazek() { Id = 1 } }, WyslanoMailNowyProdukt = false, ZdjecieGlowne = new Obrazek(), Kategorie = new List<KategorieBLL>() { new KategorieBLL() { Id = 1, Widoczna = true } }, IdCechPRoduktu = new HashSet<long>() { 1 } };
            //Produkt spełnia filtrowanie posiada rodzine
            ProduktBazowy pb8 = new ProduktBazowy() { Id = 8, Widoczny = true, Zdjecia = new List<IObrazek>() { new Obrazek() { Id = 1 } }, Rodzina = "rodzina", WyslanoMailNowyProdukt = false, ZdjecieGlowne = new Obrazek(), Kategorie = new List<KategorieBLL>() { new KategorieBLL() { Id = 1, Widoczna = true } }, IdCechPRoduktu = new HashSet<long>() { 1 } };
            //Produkt spełnia filtrowanie posiada rodzine
            ProduktBazowy pb9 = new ProduktBazowy() { Id = 9, Widoczny = true, Zdjecia = new List<IObrazek>() { new Obrazek() { Id = 1 } }, Rodzina = "rodzina", WyslanoMailNowyProdukt = false, ZdjecieGlowne = new Obrazek(), Kategorie = new List<KategorieBLL>() { new KategorieBLL() { Id = 1, Widoczna = true } }, IdCechPRoduktu = new HashSet<long>() { 1 } };


            return new List<ProduktBazowy>() {pb1,pb2,pb3,pb4,pb5,pb6,pb7,pb8,pb9};
        }
    }

}
