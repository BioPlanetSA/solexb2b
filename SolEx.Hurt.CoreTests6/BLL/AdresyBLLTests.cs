using System;
using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using Xunit;

namespace SolEx.Hurt.Core.BLL.Tests
{
    public class AdresyBLLTests
    {
        [Fact(DisplayName = "Sprawdzenie czy Adres o określonym id zostal uzyty")]
        public void CzyMoznaUzycTest()
        {
            //int id = 1;
            
            //IKlient k1 = new Klient(null) {klient_id=1, adres_wysylki_id = 1};
            //IKlient k2 = new Klient(null) { klient_id = 2, adres_wysylki_id = 5 };
            //IKlient k3 = new Klient(null) { klient_id = 3 };
            //IKlient k4 = new Klient(null) { klient_id = 4, adres_wysylki_id = 1 };
            //List<IKlient> listaKlientowTrue = new List<IKlient>(){k1,k2,k3,k4};
            //List<IKlient> listaKlientowFalse = new List<IKlient>(){k2,k3};

            //var klienciDostep = A.Fake<IKlienciDostep>();
            //A.CallTo(() => klienciDostep.PobierzWszystkich(null)).Returns(listaKlientowTrue);
            
            //AdresyBLL a1 = new AdresyBLL(){KlienciDostep = klienciDostep};

            //bool wynik = a1.CzyUzyty(id);
            //Assert.True(wynik);

            //A.CallTo(() => klienciDostep.PobierzWszystkich(null)).Returns(listaKlientowFalse);
            //wynik = a1.CzyUzyty(id);
            //Assert.False(wynik);
            throw new NotImplementedException();
        }

        [Fact(DisplayName="Test sprawdzajacy poprawnosc dzialania metody zwracajacej kraj o określonym ID")]
        public void PobierzKrajByIdTest()
        {
            throw  new NotImplementedException();
            //Dictionary<int, Kraje> slownikKrajow = new Dictionary<int, Kraje>();
            //Kraje k1 = new Kraje(){Id=1, Nazwa = "Polska"};
            //Kraje k2 = new Kraje() { Id = 2, Nazwa = "Niemacy" };
            //Kraje k3 = new Kraje() { Id = 3, Nazwa = "Rosja" };
            //Kraje k4 = new Kraje() { Id = 4, Nazwa = "USA" };
            //Kraje k5 = new Kraje() { Id = 5, Nazwa = "Burkinafaso" };
            //slownikKrajow.Add(k1.Id, k1);
            //slownikKrajow.Add(k2.Id, k2);
            //slownikKrajow.Add(k3.Id, k3);
            //slownikKrajow.Add(k4.Id, k4);
            //slownikKrajow.Add(k5.Id, k5);

            //var adresy = A.Fake<AdresyBLL>();
            //A.CallTo(() => adresy.PobierzKraje(1)).Returns(slownikKrajow);

            //Kraje kNotNull = adresy.PobierzKrajById(1, 1);
            //Kraje kNull = adresy.PobierzKrajById(20, 1);

            //Assert.NotNull(kNotNull);
            //Assert.Null(kNull);

        }

    }
}
