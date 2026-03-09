using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty.Tests
{
    public class KopiowaniePolaDoAtrybutuTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
            Dictionary<long, Produkt> produkty=new Dictionary<long, Produkt>();
         KopiowaniePolaDoAtrybutu modul=new KopiowaniePolaDoAtrybutu();
            IAPIWywolania api = A.Fake<IAPIWywolania>();
            //A.CallTo(() => api.PobierzProdukty(A<ProduktySearchCriteria>.Ignored)).Returns(produkty);
            modul.ApiWywolanie = api;
            modul.Atrybut = "Test";
            modul.Pole = "pole_tekst1";
            List<Atrybut> atr=new List<Atrybut>();
            List<Cecha> cechy=new List<Cecha>();
            modul.Przetworz(ref atr,ref cechy,produkty);
            Assert.Equal(1,atr.Count);

        }
    }
}
