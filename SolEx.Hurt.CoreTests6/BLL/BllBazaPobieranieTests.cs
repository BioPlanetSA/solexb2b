//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using FakeItEasy;
//using SolEx.Hurt.Core.BLL;
//using SolEx.Hurt.Core.ModelBLL.Interfejsy;
//using SolEx.Hurt.Model.Enums;
//using Xunit;
//namespace SolEx.Hurt.Core.BLL.Tests
//{
//    public class BllBazaPobieranieTests
//    {
//        [Fact()]
//        public void PobierzPosortujTest()
//        {
//            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//            KlienciDostep kd = A.Fake<KlienciDostep>(x => x.WithArgumentsForConstructor(new[] { calosc })); ;
//            List<IKlient> klieni = new List<IKlient>();
//            IKlient k2 = A.Fake<IKlient>();
//            k2.klient_id = 2;
//            k2.nazwa = "klient drugi";
//            klieni.Add(k2);
//            IKlient k1 = A.Fake<IKlient>();
//            k1.klient_id = 1;
//            k1.nazwa = "klient pierwszy";
//            klieni.Add(k1);


//            A.CallTo(() => kd.Wszystykie(A<int>.Ignored)).Returns(klieni);

//            var dane = kd.PobierzPosortuj(1, (x) => true, (x) => x.klient_id,KolejnoscSortowania.asc);
//            Assert.Equal(1, dane[0].klient_id);
//            Assert.Equal(2, dane[1].klient_id);
//        }


//        [Fact()]
//        public void PobierzPosortujTestKolekcja()
//        {
//            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//            KlienciDostep kd = A.Fake<KlienciDostep>(x => x.WithArgumentsForConstructor(new[] { calosc })); ;
//            List<IKlient> klieni = new List<IKlient>();
//            IKlient k2 = A.Fake<IKlient>();
//            k2.klient_id = 2;
//            k2.nazwa = "klient drugi";
//            klieni.Add(k2);
//            IKlient k1 = A.Fake<IKlient>();
//            k1.klient_id = 1;
//            k1.nazwa = "klient pierwszy";
//            klieni.Add(k1);


//            A.CallTo(() => kd.Wszystykie(A<int>.Ignored)).Returns(klieni);

            
          
//            SortowanieKryteria<IKlient> warunek=  kd.StworzKryterium((x => x.nazwa), KolejnoscSortowania.desc);
//            var dane = kd.PobierzPosortuj(1, (x) => true, warunek);
//            Assert.Equal(1, dane[0].klient_id);
//            Assert.Equal(2, dane[1].klient_id);
//        }
//         [Fact()]
//        public void PobierzTest()
//        {
//            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//            KlienciDostep kd = A.Fake<KlienciDostep>(x => x.WithArgumentsForConstructor(new[] { calosc })); ;
//            List<IKlient> klieni = new List<IKlient>();
//            IKlient k2 = A.Fake<IKlient>();
//            k2.klient_id = 2;
//            k2.nazwa = "klient drugi";
//            klieni.Add(k2);
//            IKlient k1 = A.Fake<IKlient>();
//            k1.klient_id = 1;
//            k1.nazwa = "klient pierwszy";
//            klieni.Add(k1);


//            A.CallTo(() => kd.Wszystykie(A<int>.Ignored)).Returns(klieni);

//            var dane = kd.Pobierz(1, (x) => x.nazwa.Contains("pierwszy"));
//            Assert.Equal(1, dane[0].klient_id);
//        }
//         [Fact(DisplayName = "Test stronicowania")]
//         public void PobierzTestStronicowanie()
//         {
//             ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//             KlienciDostep kd = A.Fake<KlienciDostep>(x => x.WithArgumentsForConstructor(new[] { calosc })); ;

//             A.CallTo(() => kd.Wszystykie(A<int>.Ignored)).Returns(TworzDane(10000));

//             var dane = kd.PobierzSortujStronicujDane(1, x => true, x => x.klient_id, KolejnoscSortowania.desc, 1, 10);
//             Assert.Equal(10,dane.Count);
//             Assert.Equal(10000, dane[0].klient_id);
//         }
//         [Fact(DisplayName = "Test stronicowania 2")]
//         public void PobierzTestStronicowanie2()
//         {
//             ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//             KlienciDostep kd = A.Fake<KlienciDostep>(x => x.WithArgumentsForConstructor(new[] { calosc })); ;

//             A.CallTo(() => kd.Wszystykie(A<int>.Ignored)).Returns(TworzDane(10000));

//             var dane = kd.PobierzSortujStronicujDane(1, x => true, x => x.klient_id, KolejnoscSortowania.desc, 2, 10);
//             Assert.Equal(10, dane.Count);
//             Assert.Equal(9990, dane[0].klient_id);
//         }


//        [Fact(DisplayName = "Test stronicowania wydajnosc")]
//        public void PobierzTestStronicowanieWydajnosc()
//        {
//            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//            KlienciDostep kd = A.Fake<KlienciDostep>(x => x.WithArgumentsForConstructor(new[] {calosc}));
//            A.CallTo(() => kd.Wszystykie(A<int>.Ignored)).Returns(TworzDane(10000));
//            Stopwatch stoper = Stopwatch.StartNew();
//            for (int i = 0; i < 4; i++)
//            {
//                var dane = kd.PobierzSortujStronicujDane(1, x => true, x => x.klient_id, KolejnoscSortowania.desc, 2, 10);
//            }

//            stoper.Stop();
//             Assert.True(stoper.Elapsed.TotalMilliseconds<1000,"Za długo trwało "+stoper.Elapsed.TotalMilliseconds);
//         }
//        private List<IKlient> TworzDane(int ile)
//        {
//            List<IKlient> klieni = new List<IKlient>();
//            for (int i = 0; i < ile; i++)
//            {

//                IKlient k2 = A.Fake<IKlient>();
//                k2.klient_id = ile - i;
//                k2.nazwa = "klient " + i;
           
//                klieni.Add(k2);


//            }
//            return klieni;
            
//        }

      
//    }
//}
