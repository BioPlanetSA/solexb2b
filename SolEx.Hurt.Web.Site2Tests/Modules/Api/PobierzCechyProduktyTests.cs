using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Web.Site2.Modules.Api;
using Xunit;
using FakeItEasy;
namespace SolEx.Hurt.Web.Site2.Modules.Api.Tests
{
    public class PobierzCechyProduktyTests
    {
        [Fact(DisplayName = "API - pobieranie cechy_produkty z filtrowaniem")]
        public void PobierzLacznikiCechyProduktyTest()
        {
            //generuje liste 10 obiektów z czego 5 ma id takie jak niżej. api musi zwrócić dokładnie 5 cech z takim id cechhy
            //int idCechyDoFiltrowania = 666;

            //Dictionary<int,cechy_produkty> listacechyprodukty = WygenerujListeCechyProdukty(idCechyDoFiltrowania);

            //HashSet<int> ids=new HashSet<int>{1,3,5,7,9};
            //ICechyAtrybuty cechyatrybuty = A.Fake<ICechyAtrybuty>();

        

            //PobierzCechyProdukty pcp = new PobierzCechyProdukty();
            //pcp.SearchCriteriaObject = cpsc;
            //pcp.CechyAtrybutyLaczniki = cechyatrybuty;
            //A.CallTo(() => cechyatrybuty.PobierzIdCechProduktow(cpsc)).Returns(ids);
            //A.CallTo(() => cechyatrybuty.WszystkieLaczniki()).Returns(listacechyprodukty);
            //Dictionary<int, cechy_produkty> slownik = pcp.PobierzLacznikiCechyProdukty();

            //Assert.Equal(5, slownik.Count);
            //Assert.Equal(0, slownik.Values.Count(a=> a.cecha_id != idCechyDoFiltrowania));

        }

        private Dictionary<int, ProduktCecha> WygenerujListeCechyProdukty(int idcechydofiltrowania)
        {
            Dictionary<int, ProduktCecha> lista = new Dictionary<int, ProduktCecha>(10);
            for (int i = 1; i < 11; i++)
            {
                ProduktCecha cp = new ProduktCecha();
          
                cp.ProduktId = i;
                cp.CechaId = i%2 == 0 ? i : idcechydofiltrowania;

                lista.Add(i,cp);
            }

            return lista;
        }
    }
}
