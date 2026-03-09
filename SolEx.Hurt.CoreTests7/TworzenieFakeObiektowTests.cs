using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL;
using Xunit;

namespace SolEx.Hurt.CoreTests
{
    public class TworzenieFakeObiektowTests
    {


        [Fact(DisplayName = "Tworzenie sztucznych obiektów - pobieranie kolekcji")]
        public void PobierzObiektOTypieTest()
        {
           TworzenieFakeObiektow tworzenie = new TworzenieFakeObiektow();
           var obiektLosowy = (tworzenie.PobierzObiektOTypie(typeof(List<DokumentyBll>))) as List<DokumentyBll>;
        }

        //[Fact(DisplayName = "Test poprawności nazw")]
        //public void TestPoprawnosciNazw()
        //{
        //    SolEx.Hurt.Core.TworzenieFakeObiektow tworzenie = new TworzenieFakeObiektow();
        //    var metody = tworzenie.GetType().GetMethods( BindingFlags.Public | BindingFlags.Instance).Where(x => x.ReturnType != typeof(object) && x.Name.StartsWith("Fake"));

        //    foreach (var m in metody)
        //    {
        //        Assert.Equal("Fake" + m.ReturnType.Name, m.Name);
        //    }
        //}


        [Fact(DisplayName = "Test pobierania wszystkich fakeow")]
        public void TestPobieraniaWszystkichFake()
        {
            TworzenieFakeObiektow tworzenie = new TworzenieFakeObiektow();
            var metody = tworzenie.GetType().GetMethods().Where(x => x.ReturnType != typeof(object) && x.Name.StartsWith("Fake"));

            foreach (var m in metody)
            {
                m.Invoke(tworzenie, null);
            }
        }

    }
}