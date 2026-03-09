using Xunit;
using SolEx.Hurt.Web.Site2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;

namespace SolEx.Hurt.Web.Site2.Controllers.Tests
{
    public class BlogControllerTests: BlogController
    {
        [Fact()]
        public void WpisyBlogaTest()
        {

            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            this.Calosc = calosc;

            this.SolexHelper = A.Fake<SolexHelper>();
            A.CallTo(() => this.SolexHelper.AktualnyJezyk.Id).Returns(1);
            A.CallTo(() => this.SolexHelper.AktualnyKlient).Returns( A.Fake<IKlient>());

            //6 elementów
            List<BlogWpisBll> lista = new List<BlogWpisBll>() {new BlogWpisBll(), new BlogWpisBll(), new BlogWpisBll(), new BlogWpisBll(), new BlogWpisBll(), new BlogWpisBll()};
            A.CallTo(calosc.DostepDane).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<BlogWpisBll>>().WithAnyArguments().Returns(lista);

            //test jak dotykamy maxa to nie powinno nic zwracac ponad liste mozliwa
            BlogModel model = new BlogModel() {BlogIlePokazywacMaxWpisow = lista.Count + 10, BlogIleDynamicznieZaladowacAktualnosci = 2, BlogKolejnosc =  BlogiKolejnosc.Losowa};

            int iloscWszystkich;
            var wynik = this.PosortujWpisyBloga(BlogiKolejnosc.Losowa, new long[] {1,2}, lista.Count + 10, 0, out iloscWszystkich);

            Assert.True( iloscWszystkich == lista.Count );
            Assert.True( wynik.Count == iloscWszystkich);

            iloscWszystkich = 0;
            wynik = this.PosortujWpisyBloga(BlogiKolejnosc.Losowa, null, 10, lista.Count -1, out iloscWszystkich);

            Assert.True(iloscWszystkich == lista.Count);
            Assert.True(wynik.Count == 1, wynik.Count.ToString());


            iloscWszystkich = 0;
            wynik = this.PosortujWpisyBloga(BlogiKolejnosc.Losowa, null, 10, lista.Count +10, out iloscWszystkich);

            Assert.True(iloscWszystkich == lista.Count);
            Assert.True(wynik == null);
        }
    }
}