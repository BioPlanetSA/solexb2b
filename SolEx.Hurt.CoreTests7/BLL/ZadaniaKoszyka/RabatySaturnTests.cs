using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL;
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
            
            KategoriaKlienta k1 = A.Fake<KategoriaKlienta>();
            k1.Grupa = "DOSTAWY";
            k1.Id = 1;
            k1.Nazwa = "dostawa_dostawa:28";

            KategoriaKlienta k2 = A.Fake<KategoriaKlienta>();
            k2.Grupa = "OPOCZNO";
            k2.Id = 2;
            k2.Nazwa = "rabat_paradyż_odbior opoczno:28";

            KategoriaKlienta k3 = A.Fake<KategoriaKlienta>();
            k3.Grupa = "OPOCZNO";
            k3.Id = 3;
            k3.Nazwa = "rabat_paradyż_platny kurier:28";

            KategoriaKlienta k4 = A.Fake<KategoriaKlienta>();
            k4.Grupa = "OPOCZNO";
            k4.Id = 4;
            k4.Nazwa = "rabat_paradyż_odbior świecie:28";
            
            IList<KategoriaKlienta> kategorie = new List<KategoriaKlienta>(){k1,k2,k3,k4};
            
            IKlient klient = A.Fake<IKlient>();
          //  A.CallTo(() => klient.Kategorie).Returns(kategorie);
            
            var solexBllCalosc = A.Fake<ISolexBllCalosc>();
      //      A.CallTo(() => solexBllCalosc.Klienci.Pobierz(A<int>.Ignored)).Returns(klient);

            A.CallTo(() => koszyk.Klient).Returns(klient);

            //Cechy dla produktu
            Dictionary<int, ICechyBll> slownikCechyBll = new Dictionary<int, ICechyBll>();
            CechyBll c2 = A.Fake<CechyBll>();
            A.CallTo(() => c2.NazwaAtrybutu).Returns("id 13 symbol rabat_cersanit nazwa Cersanit");
            slownikCechyBll.Add(13, c2);

            //Pozycje w koszyku
            KoszykPozycje p1 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p1.CenaNetto).Returns(24.06m);
            A.CallTo(() => p1.IloscWJednostcePodstawowej).Returns(1.4m);
            A.CallTo(() => p1.CalkowityRabat()).Returns(new WartoscLiczbowa(26));
            //A.CallTo(() => p1.Produkt.Cechy).Returns(slownikCechyBll);

            List<KoszykPozycje> listaPozycji = new List<KoszykPozycje>(){p1};
            A.CallTo(() => koszyk.PobierzPozycje).Returns(listaPozycji);
            
            RabatySaturn rabatySaturn = new RabatySaturn();
            rabatySaturn.NazwaAtrybutuRabatowego = "rabat";
            rabatySaturn.DomyslnaDostawa = "dostawa_";
            rabatySaturn.ISolexBllCalosc = solexBllCalosc;
            var rezultat  = rabatySaturn.Wykonaj(koszyk);

            Assert.True(p1.CalkowityRabat() == 28m);
            Assert.True(p1.WartoscNetto == 32.78m);
        }
    }
}
