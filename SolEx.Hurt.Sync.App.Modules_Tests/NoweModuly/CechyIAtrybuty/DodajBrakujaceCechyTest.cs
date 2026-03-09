using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.CechyIAtrybuty
{
    public class DodajBrakujaceCechyTest
    {
        [Fact(DisplayName = "Test sprawdzajacy poprawne działanie modulu")]
        public void PrzetworzTest()
        {
            Atrybut a1 = new Atrybut();
            a1.Id = 1696198502;
            a1.Nazwa = "kategoria";

            Cecha c1 = new CechyBll();
            c1.Id = 100013;
            c1.Nazwa = "WYROBY\\KARNISZE RUROWE 28mm";
            c1.Symbol = "kategoria:wyroby\\karnisze rurowe 28mm";
            c1.AtrybutId = a1.Id;

            DodajBrakujaceCechy mod = new DodajBrakujaceCechy();
            IConfigSynchro konfiguracja = A.Fake<IConfigSynchro>();
            A.CallTo(() => konfiguracja.SeparatorAtrybutowWCechach).Returns(":".ToCharArray());

            mod.Konfiguracja = konfiguracja;

            mod.Atrybut = a1.Id.ToString();
            mod.Separator = "\\";
            List<Cecha>listaCech = new List<Cecha>() { c1 };
            mod.Przetworz(new List<Atrybut>() {a1}, listaCech,null, null);


            Assert.True(listaCech.Count==2);
        }
    }
}
