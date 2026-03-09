using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
using FakeItEasy;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Core.BLL.Tests
{

    public class ProduktyKlientaTest
    {
        [Fact(DisplayName="Testowanie czy id i inne propertisy zwracają się poprawnie dla produktów wirtualnych")]
        public void WirtualneProduktyTest()
        {
            var fake = new TworzenieFakeObiektow();
            IKlient klient = fake.FakeIKlient();

            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => solexBllCalosc.Konfiguracja.JezykiWSystemie).Returns(new Dictionary<int, Jezyk> { { 1, new Jezyk() { Id = 1 } } });
            A.CallTo(() => solexBllCalosc.CenyPoziomy.PobierzCenyProduktu(0)).Returns(new Dictionary<int, CenaPoziomu> { { 1, new CenaPoziomu() } });

            //Produkt produktModel = new Produkt() {Id = 10, Nazwa = "produkt prawdziwy"};
            ProduktBazowy produktBazowy = (ProduktBazowy)fake.PobierzObiektOTypie(typeof(ProduktBazowy));
            produktBazowy.Calosc = solexBllCalosc;

            produktBazowy.Zamienniki = new Dictionary<long, bool> { { 1, true }, { 2, false } };

            ProduktKlienta produktKlienta = new ProduktKlienta(produktBazowy, klient);
            produktKlienta.Zamienniki = produktBazowy.Zamienniki;

            ProduktKlientaWirtualny produktWirtualny = new ProduktKlientaWirtualny(produktBazowy, klient, 777);

            //jesli patrzymy z perpektywy produktu REALNEGO klienta to mamy mieć klucz REALNY
            Assert.Equal(produktKlienta.Id, produktBazowy.Id);

            //jesli patrzymy z perpektywy produktu WIRTUALENGO klienta to mamy mieć klucz WIRTUALNY - czyli NIE ten z bazowego
            Assert.NotEqual(produktWirtualny.Id, produktBazowy.Id);

            //jesli wyciagnimy produkt bazowy z produktu WIRTUALNEGO to mamy mieć id BAZOWE (REALNE)
            Assert.Equal( (produktWirtualny as ProduktBazowy).Id, produktBazowy.Id);

            //proeprtis zamieniki
            // produktBazowy.Zamienniki = new Dictionary<long, bool> {{1, true}, {2, false}};
            
            Assert.True(produktKlienta.Zamienniki.Count == 2, "z perspektywy produktu klienta powinno byc 2 element");

            produktKlienta.Zamienniki = new Dictionary<long, bool> {{3, true}};

            Assert.True( produktKlienta.Zamienniki.Count == 1 , "z perspektywy produktu klienta powinno byc 1 element");
            Assert.True( (produktKlienta as ProduktBazowy).Zamienniki.Count == 2, "z perspektywy produktu bazowego powinno byc 2 elementy");
        }


        public void WirtualneProdukty_Stany()
        {
            var fake = new TworzenieFakeObiektow();
            IKlient klient = fake.FakeIKlient();
            //Produkt produktModel = new Produkt() {Id = 10, Nazwa = "produkt prawdziwy"};
            ProduktBazowy produktBazowy = (ProduktBazowy)fake.PobierzObiektOTypie(typeof(ProduktBazowy));
            produktBazowy.Zamienniki = new Dictionary<long, bool> { { 1, true }, { 2, false } };

            ProduktKlienta produktKlienta = new ProduktKlienta(produktBazowy, klient);
            ProduktKlientaWirtualny produktWirtualny = new ProduktKlientaWirtualny(produktBazowy, klient, 777);

            //produktWirtualny.IloscLaczna
        }

        protected IList<IProduktKlienta> ListaProduktowTestowych()
        {
            IList<IProduktKlienta> lista = new List<IProduktKlienta>();

            string[] nazwyProdutktow = 
            {
                "makaron ameba jakas tam",
                "makaron jajeczny ameba jakas tam2",
                "makaron jajeczny",
                "test nazwa",
                "nazwa naro mak",
                "aronia duża pulchna ąś"
            };

            List<HashSet<long>> kategorieIDs = new List<HashSet<long>>
            {
                new HashSet<long>() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10},
                new HashSet<long>() {10},
                new HashSet<long>() {10},
                new HashSet<long>() {10},
                new HashSet<long>() {5},
                new HashSet<long>() {5, 10}
            };

            for (int i = 0; i < nazwyProdutktow.Length - 1; ++i)
            {
                TworzenieFakeObiektow generator = new TworzenieFakeObiektow();
                IProduktKlienta produkt = generator.FakeIProduktKlienta();
                produkt.Nazwa = nazwyProdutktow[i];
                A.CallTo(() => produkt.KategorieId).Returns( kategorieIDs[i] );
                lista.Add(produkt);
            }

            return lista;
        }


        [Fact(DisplayName = "Test sprawdzający poprawnośc sprawdzania czy wszystkie dzieci w rodzinie maja ta samą cene oraz ustalenie najniższej ceny jeżeli ceny są różne")]
        public void CzyWszystkieDzieciMajaTaSamaCeneTest()
        {

            IFlatCenyBLL fc = A.Fake<IFlatCenyBLL>();
            A.CallTo(() => fc.CenaNetto).Returns(new WartoscLiczbowa(18m));

            IFlatCenyBLL fc1 = A.Fake<IFlatCenyBLL>();
            A.CallTo(() => fc1.CenaNetto).Returns(new WartoscLiczbowa(20m));

            IFlatCenyBLL fc2 = A.Fake<IFlatCenyBLL>();
            A.CallTo(() => fc2.CenaNetto).Returns(new WartoscLiczbowa(10m));

            IFlatCenyBLL fc3 = A.Fake<IFlatCenyBLL>();
            A.CallTo(() => fc3.CenaNetto).Returns(new WartoscLiczbowa(0));

            IProduktKlienta pk1 = A.Fake<IProduktKlienta>();

            A.CallTo(() => pk1.FlatCeny).Returns(fc1);
            IProduktKlienta pk2 = A.Fake<IProduktKlienta>();
            A.CallTo(() => pk2.FlatCeny).Returns(fc2);
            IProduktKlienta pk3 = A.Fake<IProduktKlienta>();
            A.CallTo(() => pk3.FlatCeny).Returns(fc3);

            
            ProduktKlienta pk = A.Fake<ProduktKlienta>();
            A.CallTo(() => pk.FlatCeny).Returns(fc);
            A.CallTo(() => pk.ProduktyWRodzinie(A<HashSet<long>>.Ignored)).Returns(new List<IProduktKlienta>() { pk1, pk2, pk3 });

            bool wynik = pk.CzyWszystkieDzieciMajaTaSamaCene(ref fc);
            Assert.True(fc.CenaNetto.Wartosc==10m);
            Assert.False(wynik);

            A.CallTo(() => fc.CenaNetto).Returns(new WartoscLiczbowa(20m));
            A.CallTo(() => fc1.CenaNetto).Returns(new WartoscLiczbowa(20m));
            A.CallTo(() => fc2.CenaNetto).Returns(new WartoscLiczbowa(20m));
            A.CallTo(() => fc3.CenaNetto).Returns(new WartoscLiczbowa(20));

            A.CallTo(() => pk.FlatCeny).Returns(fc);
            A.CallTo(() => pk.ProduktyWRodzinie(A<HashSet<long>>.Ignored)).Returns(new List<IProduktKlienta>() { pk1, pk2, pk3 });
            wynik = pk.CzyWszystkieDzieciMajaTaSamaCene(ref fc);
            Assert.True(wynik);
            Assert.True(fc.CenaNetto.Wartosc == 20m);
        }

        [Fact(DisplayName = "Test dziedziczenia")]
        public void testDziedziczenia()
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo( () => calosc.Pliki.PobierzObrazkiProduktu(1)).Returns(new List<IObrazek>());

            ProduktBazowy pb = new ProduktBazowy(1);
            pb.Id = 1;
            pb.Calosc = calosc;
            IKlient klient = new Klient();

            ProduktKlienta pk = new ProduktKlienta(pb, klient);
            ProduktKlienta pk1 = new ProduktKlienta(pb, klient);
            ProduktKlienta pk2 = new ProduktKlienta(pb, klient);

            //w bazowym wybieramy zdjecie - czy przejdzie do klienta?
            Assert.True(pk.Zdjecia.Count == 0);
            Assert.True(pk1.Zdjecia.Count == 0);
            Assert.True(pk2.Zdjecia.Count == 0);

            //powinien sie wykonac RAZ - a nie dla kazdego produktu klienta
            A.CallTo(() => calosc.Pliki.PobierzObrazkiProduktu(1)).MustHaveHappened(Repeated.Exactly.Once);
        }


        [Fact(DisplayName = "Test dziedziczenia - id cech")]
        public void testDziedziczenia_idCech()
        {
            ProduktBazowy pb = new ProduktBazowy(1);
            pb.Id = 1;
            pb.IdCechPRoduktu = new HashSet<long> { 1,2,3,4,5 };
            IKlient klient = new Klient();

            ProduktKlienta pk = new ProduktKlienta(pb, klient);
            pk.IdCechPRoduktu = new HashSet<long> { 666 };

            //w bazowym wybieramy zdjecie - czy przejdzie do klienta?
            Assert.Equal(1, pk.IdCechPRoduktu.Count);
            Assert.Equal(5, pb.IdCechPRoduktu.Count);
        }



    }

}