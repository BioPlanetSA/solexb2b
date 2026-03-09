using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili.Tests
{
    public class PrzeterminowanePlatnosciTests
    {
        [Fact()]
        public void PrzeterminowanePlatnosciTest()
        {
            int idj = 1;
            IKlient klient = A.Fake<IKlient>();
            klient.nazwa = "nazwa";
            klient.email = "test@test.pl";
            A.CallTo(() => klient.JezykId).Returns(idj);
            IConfigBLL config = A.Fake<IConfigBLL>();

            //nadchodzace platnosci
            IDokument ndokument1 = A.Fake<IDokument>();
            A.CallTo(() => ndokument1.StatusIdHash).Returns(1);
            A.CallTo(() => ndokument1.DokumentWartoscNalezna).Returns(10);
            A.CallTo(() => ndokument1.DokumentWartoscBrutto).Returns(100);
            A.CallTo(() => ndokument1.CzyPrzeterminowany()).Returns(false);
            A.CallTo(() => ndokument1.PobierzStatus(A<int>.Ignored).TraktujJakoFaktoring).Returns(false);

            IDokument ndokument2 = A.Fake<IDokument>();
            A.CallTo(() => ndokument2.StatusIdHash).Returns(2);
            A.CallTo(() => ndokument2.DokumentWartoscNalezna).Returns(20);
            A.CallTo(() => ndokument2.DokumentWartoscBrutto).Returns(200);
            A.CallTo(() => ndokument2.CzyPrzeterminowany()).Returns(false);
            A.CallTo(() => ndokument2.PobierzStatus(A<int>.Ignored).TraktujJakoFaktoring).Returns(false);

            IDokument ndokument3 = A.Fake<IDokument>();
            A.CallTo(() => ndokument3.StatusIdHash).Returns(3);
            A.CallTo(() => ndokument3.DokumentWartoscNalezna).Returns(30);
            A.CallTo(() => ndokument3.DokumentWartoscBrutto).Returns(300);
            A.CallTo(() => ndokument3.CzyPrzeterminowany()).Returns(false);
            A.CallTo(() => ndokument3.PobierzStatus(A<int>.Ignored).TraktujJakoFaktoring).Returns(false);

            //przeterminowane platnosci
            IDokument pdokument1 = A.Fake<IDokument>();
            A.CallTo(() => pdokument1.StatusIdHash).Returns(4);
            A.CallTo(() => pdokument1.DokumentWartoscNalezna).Returns(40);
            A.CallTo(() => pdokument1.DokumentWartoscBrutto).Returns(400);
            A.CallTo(() => pdokument1.CzyPrzeterminowany()).Returns(true);
            A.CallTo(() => pdokument1.PobierzStatus(A<int>.Ignored).TraktujJakoFaktoring).Returns(false);

            IDokument pdokument2 = A.Fake<IDokument>();
            A.CallTo(() => pdokument2.StatusIdHash).Returns(5);
            A.CallTo(() => pdokument2.DokumentWartoscNalezna).Returns(50);
            A.CallTo(() => pdokument2.DokumentWartoscBrutto).Returns(500);
            A.CallTo(() => pdokument2.CzyPrzeterminowany()).Returns(true);
            A.CallTo(() => pdokument2.PobierzStatus(A<int>.Ignored).TraktujJakoFaktoring).Returns(false);

            IDokument pdokument3 = A.Fake<IDokument>();
            A.CallTo(() => pdokument3.StatusIdHash).Returns(6);
            A.CallTo(() => pdokument3.DokumentWartoscNalezna).Returns(60);
            A.CallTo(() => pdokument3.DokumentWartoscBrutto).Returns(600);
            A.CallTo(() => pdokument3.CzyPrzeterminowany()).Returns(true);
            A.CallTo(() => pdokument3.PobierzStatus(A<int>.Ignored).TraktujJakoFaktoring).Returns(false);

            //faktoringowe
            IDokument fDokument1 = A.Fake<IDokument>();
            A.CallTo(() => fDokument1.StatusIdHash).Returns(4);
            A.CallTo(() => fDokument1.DokumentWartoscNalezna).Returns(70);
            A.CallTo(() => fDokument1.DokumentWartoscBrutto).Returns(700);
            A.CallTo(() => fDokument1.CzyPrzeterminowany()).Returns(true);
            A.CallTo(() => fDokument1.StatusIdHash).Returns(1);
            A.CallTo(() => fDokument1.PobierzStatus(A<int>.Ignored).TraktujJakoFaktoring).Returns(true);

            IDokument fDokument2 = A.Fake<IDokument>();
            A.CallTo(() => fDokument2.StatusIdHash).Returns(5);
            A.CallTo(() => fDokument2.DokumentWartoscNalezna).Returns(80);
            A.CallTo(() => fDokument2.DokumentWartoscBrutto).Returns(800);
            A.CallTo(() => fDokument2.CzyPrzeterminowany()).Returns(true);
            A.CallTo(() => fDokument2.StatusIdHash).Returns(2);
            A.CallTo(() => fDokument2.PobierzStatus(A<int>.Ignored).TraktujJakoFaktoring).Returns(true);


            IDokument fDokument3 = A.Fake<IDokument>();
            A.CallTo(() => fDokument3.StatusIdHash).Returns(6);
            A.CallTo(() => fDokument3.DokumentWartoscNalezna).Returns(90);
            A.CallTo(() => fDokument3.DokumentWartoscBrutto).Returns(900);
            A.CallTo(() => fDokument3.CzyPrzeterminowany()).Returns(false);
            A.CallTo(() => fDokument3.StatusIdHash).Returns(3);
            A.CallTo(() => fDokument3.PobierzStatus(A<int>.Ignored).TraktujJakoFaktoring).Returns(true);

            IDokument fDokument4 = A.Fake<IDokument>();
            A.CallTo(() => fDokument4.StatusIdHash).Returns(7);
            A.CallTo(() => fDokument4.DokumentWartoscNalezna).Returns(90);
            A.CallTo(() => fDokument4.DokumentWartoscBrutto).Returns(900);
            A.CallTo(() => fDokument4.CzyPrzeterminowany()).Returns(false);
            A.CallTo(() => fDokument4.StatusIdHash).Returns(3);
            A.CallTo(() => fDokument4.PobierzStatus(A<int>.Ignored).TraktujJakoFaktoring).Returns(true);

            List<IDokument> wszystkie = new List<IDokument>() { ndokument1, ndokument2, ndokument3, pdokument1, pdokument2, pdokument3 };
            var pp = new PrzeterminowanePlatnosci(wszystkie);
            Assert.True(pp.WlasneNadchodzaceDoZaplaty == 60);
            Assert.True(pp.WlasnePrzeterminowaneDoZaplaty == 150);
            Assert.True(pp.WlasneWszystkieDoZaplatyBrutto == 210);

            Assert.True(pp.WlasneNadchodzaceWartoscBrutto == 600);
            Assert.True(pp.WlasnePrzeterminowaneWartoscBrutto == 1500);

            Assert.True(pp.WlasnePrzeterminowaneWartoscZaplacona == 1350);
            Assert.True(pp.WlasneNadchodzaceWartoscZaplacona == 540);

            List<IDokument> faktoringowe = new List<IDokument>() { fDokument1, fDokument2, fDokument3, fDokument4 };
            pp = new PrzeterminowanePlatnosci(faktoringowe);

            //Assert.True(pp.FaktoringowePrzeterminowaneWartoscBrutto == 1);
            //Assert.True(pp.FaktoringoweNadchodzaceWartoscBrutto == 1);
            //Assert.True(pp.WlasnePrzeterminowaneWartoscZaplacona == 1);
        }
    }
}
