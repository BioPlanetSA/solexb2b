//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FakeItEasy;
//using SolEx.Hurt.Core.BLL;
//using SolEx.Hurt.Model;
//using SolEx.Hurt.Model.Interfaces;
//using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;
//using Xunit;
//namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
//{
//    public class TrecProduktyTests
//    {
//        [Fact()]
//        public void ZrobOpisTest()
//        {
//            string json = @"{""informacje_ogolne"" :{""radio_1"" : ""2"",""naglowek_tabeli"" : ""Nutrition facts"",""naglowki_tabeli"" : [""100 g"",""90 g"",""30 g""],""ilosc_kolumn"" : ""3""},""wartosci_energetyczne"" :{""kj"":[""1474"",""1327"",""442""],""kcal"":[""352"",""317"",""106""]},""wartosci_odzywcze"" :{""radio_2"" : ""1"",""bialko"":[""g"",""67,96"",""61,17"",""20,39""],""tluszcz"":[""g"",""4,93"",""4,43"",""1,48""],""weglowodany"":[""g"",""9,03"",""8,13"",""2,17""]},""skladniki"" : [[""[Koncentrat Białka Serwatkowego, Koncentrat Białek Mleka, Kazeinian Wapnia]"",""g"",""82,84"",""74,56"",""24,85""]],""zalecane"" : [["""","""",""""]],""witaminy"" :{""wielkoscporcji"" : """",""wielkoscporcji_sel"" : ""mg"",""nazwa"" :[],""wartosc"" :[]}}";
//            TrecProdukty tp=new TrecProdukty();
//            /*
//             {"informacje_ogolne" :{"radio_1" : "2","naglowek_tabeli" : "Nutrition facts","naglowki_tabeli" : ["100 g","90 g","30 g"],"ilosc_kolumn" : "3"},"wartosci_energetyczne" 
//             * :{"kj":["1474","1327","442"],"kcal":["352","317","106"]},"wartosci_odzywcze" :{"radio_2" : "1","bialko":["g","67,96","61,17","20,39"],"tluszcz":["g","4,93","4,43","1,48"],
//             * "weglowodany":["g","9,03","8,13","2,17"]},"skladniki" : [["[Koncentrat Białka Serwatkowego, Koncentrat Białek Mleka, Kazeinian Wapnia]","g"
//             * ,"82,84","74,56","24,85"]],"zalecane" : [["","",""]],"witaminy" :{"wielkoscporcji" : "","wielkoscporcji_sel" : "mg","nazwa" :[],"wartosc" :[]}}
//             */
//            ConfigBLL config = A.Fake<ConfigBLL>();
//            IConfigDataProvider provider = A.Fake<IConfigDataProvider>();

//            A.CallTo(() => provider.PobierzSystemPola()).Returns(new List<system_pola>());

//            config.Provider = provider;
//            Dictionary<int, jezyki> slownikJezykow = new Dictionary<int, jezyki>();
//            slownikJezykow.Add(1, new jezyki(){ id = 1, nazwa="Polski", symbol = "pl"} );
//            A.CallTo(() => config.JezykiWSystemie).Returns(slownikJezykow);
//            A.CallTo(() => config.PobierzTlumaczenie(1, "100 g")).Returns("100 g");
//            A.CallTo(() => config.PobierzTlumaczenie(1, "90 g")).Returns("90 g");
//            A.CallTo(() => config.PobierzTlumaczenie(1, "30 g")).Returns("30 g");
//             A.CallTo(() => config.PobierzTlumaczenie(1, "30 g")).Returns("30 g");
//             A.CallTo(() => config.PobierzTlumaczenie(1, "Wartość energetyczna")).Returns("Wartość energetyczna");
//             A.CallTo(() => config.PobierzTlumaczenie(1, "Wartość odżywcza")).Returns("Wartość odżywcza");
//             A.CallTo(() => config.PobierzTlumaczenie(1, "bialko")).Returns("bialko");
//             A.CallTo(() => config.PobierzTlumaczenie(1, "tluszcz")).Returns("tluszcz");
//             A.CallTo(() => config.PobierzTlumaczenie(1, "weglowodany")).Returns("weglowodany");
//            tp._ConfigBLL = config;

//            string wynik = tp.ZrobOpis(json, "pl");
//        }
//    }
//}
