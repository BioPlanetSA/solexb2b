using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class RabatySaturnTests
    {
        [Fact(DisplayName = "Test dla Rabatów Saturna")]
        public void WykonajTest()
        {
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            ISposobDostawy sposobDostawy = A.Fake<ISposobDostawy>();
            A.CallTo(() => koszyk.KosztDostawy()).Returns(sposobDostawy);
            A.CallTo(() => koszyk.KosztDostawy().SymbolProduktu).Returns("");
            
            //kategorie 
            
            kategorie_klientow k1 = A.Fake<kategorie_klientow>();
            k1.grupa = "DOSTAWY";
            k1.Id = 1;
            k1.nazwa = "dostawa_dostawa:28";
            
            kategorie_klientow k2 = A.Fake<kategorie_klientow>();
            k2.grupa = "OPOCZNO";
            k2.Id = 2;
            k2.nazwa = "rabat_paradyż_odbior opoczno:28";
            
            kategorie_klientow k3 = A.Fake<kategorie_klientow>();
            k3.grupa = "OPOCZNO";
            k3.Id = 3;
            k3.nazwa = "rabat_paradyż_platny kurier:28";
           
            kategorie_klientow k4 = A.Fake<kategorie_klientow>();
            k4.grupa = "OPOCZNO";
            k4.Id = 4;
            k4.nazwa = "rabat_paradyż_odbior świecie:28";
            
            IList<kategorie_klientow> kategorie = new List<kategorie_klientow>(){k1,k2,k3,k4};
            
            IKlient klient = A.Fake<IKlient>();
          //  A.CallTo(() => klient.Kategorie).Returns(kategorie);
            
            var solexBllCalosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => solexBllCalosc.Klienci.Pobierz(A<int>.Ignored)).Returns(klient);
            
            //Cechy dla produktu
            Dictionary<int, ICechyBll> slownikCechyBll = new Dictionary<int, ICechyBll>();
            CechyBll c2 = A.Fake<CechyBll>();
            A.CallTo(() => c2.NazwaAtrybutu).Returns("id 13 symbol rabat_cersanit nazwa Cersanit");
            slownikCechyBll.Add(13, c2);

            //Pozycje w koszyku
            IKoszykPozycja p1 = A.Fake<IKoszykPozycja>();
            A.CallTo(() => p1.CenaNetto).Returns(24.06m);
            A.CallTo(() => p1.IloscWJednostcePodstawowej).Returns(1.4m);
            A.CallTo(() => p1.CalkowityRabat).Returns(new WartoscLiczbowa(26));
            //A.CallTo(() => p1.Produkt.Cechy).Returns(slownikCechyBll);

            List<IKoszykPozycja> listaPozycji = new List<IKoszykPozycja>(){p1};
            A.CallTo(() => koszyk.Pozycje).Returns(listaPozycji);
            
            RabatySaturn rabatySaturn = new RabatySaturn();
            rabatySaturn.NazwaAtrybutuRabatowego = "rabat";
            rabatySaturn.DomyslnaDostawa = "dostawa_";
            rabatySaturn.ISolexBllCalosc = solexBllCalosc;
            var rezultat  = rabatySaturn.Wykonaj(koszyk);

            Assert.True(p1.CalkowityRabat == 28m);
            Assert.True(p1.WartoscNetto == 32.78m);
        }
    }
}
