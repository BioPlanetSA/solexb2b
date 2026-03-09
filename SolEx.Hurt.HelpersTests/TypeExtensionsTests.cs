using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using Xunit;
namespace SolEx.Hurt.Helpers.Tests
{
    public class TypeExtensionsTests
    {
        [Fact()]
        public void WygenerujIDObiektuSHATest()
        {
            ProduktUkryty pu3 = new ProduktUkryty();
            pu3.KlientZrodloId = 10120792;
            pu3.ProduktZrodloId = 341808;
            pu3.Tryb = KatalogKlientaTypy.Dostepne;
            pu3.Id = 0;
            pu3.Id = pu3.WygenerujIDObiektuSHAWersjaLong();

            ProduktUkryty pu4 = new ProduktUkryty();
            pu4.KlientZrodloId = 10262611;
            pu4.ProduktZrodloId = 346710;
            pu4.Tryb = KatalogKlientaTypy.Dostepne;
            pu4.Id = 0;
            pu4.Id = pu4.WygenerujIDObiektuSHAWersjaLong();

            Assert.NotEqual(pu3.Id, pu4.Id);
        }
    }
}
