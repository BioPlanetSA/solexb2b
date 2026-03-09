using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL
{
    public class PlikiBllTest
    {
        [Fact(DisplayName = "Test sprawdzjący poprawne działanie ")]
        public void UsunPlikiFizyczneBezPlikuWBazie()
        {
            ISolexBllCalosc Calosc = A.Fake<ISolexBllCalosc>();

            Plik pl = A.Fake<Plik>();
            A.CallTo(() => pl.SciezkaBezwzgledna).Returns("C:\\projekty\\SolexB2B\\SolEx.Hurt.Web.Site2\\Zasoby\\import\\test2\\901619660889.jpg");
            
            A.CallTo(() => Calosc.DostepDane.Pobierz<Plik>(null)).Returns(new List<Plik>() { pl });

            PlikiBLL p = A.Fake<PlikiBLL>(x => x.WithArgumentsForConstructor(new List<object>() {Calosc}));
            A.CallTo(() => p.PobierzPlikiZKataloguImportu()).Returns(new HashSet<string>()
            {
                "C:\\projekty\\SolexB2B\\SolEx.Hurt.Web.Site2\\Zasoby\\import\\test2\\901619660889.jpg",
                "C:\\projekty\\SolexB2B\\SolEx.Hurt.Web.Site2\\Zasoby\\import\\test1\\6258.jpg",
                "C:\\projekty\\SolexB2B\\SolEx.Hurt.Web.Site2\\Zasoby\\import\\test2\\6258.jpg"
            });
            p.UsunPlikiFizyczneBezPlikuWBazie();

        }

    }
}
