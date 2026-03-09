using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.CechyIAtrybuty
{
    public class BudujFiltryKonfiguratoraTests
    {

        [Fact]
        public void RozbijCecheTest()
        {
            List<Atrybut> listaAtr = new List<Atrybut>();
            List<Cecha> listaCech = new List<Cecha>();
            Dictionary<long,ProduktCecha> listaProduktCecha = new Dictionary<long, ProduktCecha>();
            string sep = "|";
            Atrybut testAtr = new Atrybut("test");
            testAtr.Id = 1;
            listaAtr.Add(testAtr);
            Atrybut testAtr2 = new Atrybut("test");
            testAtr2.Id = 2;
            listaAtr.Add(testAtr2);

            Cecha testCecha = new Cecha("test1" + sep + "test2" + sep + "test3", "test1" + sep + "test2" + sep + "test3");
            testCecha.AtrybutId = testAtr.Id;
            testCecha.Id = 1;
            Cecha testCecha3 = new Cecha("test21" + sep + "test22" + sep + "test3", "test21" + sep + "test22" + sep + "test3");
            testCecha3.AtrybutId = testAtr.Id;
            testCecha3.Id = 3;
            Cecha testCecha2 = new Cecha("testCecha2", "testCecha2");
            testCecha2.AtrybutId = testAtr2.Id;
            testCecha2.Id = 2;

            listaCech.Add(testCecha);
            listaCech.Add(testCecha2);
            listaCech.Add(testCecha3);


            var cp1 = new ProduktCecha(1, testCecha.Id);
            var cp2 = new ProduktCecha(2, testCecha2.Id);
            var cp3 = new ProduktCecha(3, testCecha3.Id);

            listaProduktCecha.Add(cp1.Id,cp1);
            listaProduktCecha.Add(cp2.Id,cp2);
            listaProduktCecha.Add(cp3.Id,cp3);

            BudujFiltryKonfiguratora buduj = new BudujFiltryKonfiguratora();
            buduj.Atrybut = testAtr.Id;
            buduj.Separator = sep;
            buduj.NazwyAtrybutow = "AtrTest1" + sep + "AtrTest2" + sep + "AtrTest3";

            buduj.RozbijCeche(ref listaAtr, ref listaCech, ref listaProduktCecha);

            Assert.True(listaCech.Any(x => x.Nazwa == "test1") );
            Assert.True(listaAtr.Any(x => x.Nazwa == "test1") );


             Cecha testCecha5 = new Cecha("test1" + sep + "test2" + sep + "test53", "test1" + sep + "test2" + sep + "test53");
            testCecha5.AtrybutId = testAtr.Id;
            testCecha5.Id = 5;

            listaCech.Add(testCecha5);

            buduj.RozbijCeche(ref listaAtr, ref listaCech, ref listaProduktCecha);
            Assert.True(listaCech.Any(x => x.Nazwa == "test53"));


            Cecha testCecha4 = new Cecha("test31" + sep + "test32", "test31" + sep + "test32");
            testCecha4.AtrybutId = testAtr.Id;
            testCecha4.Id = 4;

            listaCech.Add(testCecha4);

            buduj.RozbijCeche(ref listaAtr, ref listaCech, ref listaProduktCecha);

            Assert.False(listaCech.Any(x => x.Nazwa == "test31"));

            buduj.RozbijCeche(ref listaAtr,ref listaCech, ref listaProduktCecha);
            long idCechy = listaCech.Where(y => y.Nazwa == "test1").Select(z => z.Id).FirstOrDefault();
            Assert.True(listaProduktCecha.Any(x=>x.Value.CechaId == idCechy));


          

         }
    }
}
