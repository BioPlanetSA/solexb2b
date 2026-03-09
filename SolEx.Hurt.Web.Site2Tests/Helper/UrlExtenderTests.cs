using System.Web.Mvc;
using System.Web.Routing;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Koszyk;
using Xunit;

namespace SolEx.Hurt.Web.Site2Tests.Helper
{
    public class UrlExtenderTests
    {
        [Fact()]
        public void ZbudujLinkTest()
        {
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() =>config.WieleJezykowWSystemie).Returns(true);
            UrlExtender.Config = config;
            UrlHelper helper = A.Fake<UrlHelper>();
            IKlient klient = A.Fake<IKlient>();
            Jezyk jezyk = A.Fake<Jezyk>();
            jezyk.Symbol = "pl";

            CechyBll cecha = A.Fake<CechyBll>();
            UrlExtender.ZbudujLink(new UrlHelper(), cecha, jezyk);

            ProduktKlienta produkt = A.Fake<ProduktKlienta>();
            UrlExtender.ZbudujLink(helper, produkt, jezyk);

            KategorieBLL kategoria = A.Fake<KategorieBLL>();
            kategoria.Nazwa = "kat1";
            kategoria.Id = 1;
            UrlExtender.ZbudujLink(new UrlHelper(), kategoria, jezyk);


            BlogWpisBll blogWpis = A.Fake<BlogWpisBll>();
            UrlExtender.ZbudujLink(helper, blogWpis,"b", jezyk);

            GrupaBLL grupa = A.Fake<GrupaBLL>();
            UrlExtender.ZbudujLink(helper, grupa, jezyk, "grupa");

            UrlExtender.ZbudujLink(helper, jezyk,klient);

            OpisImportera import = A.Fake<OpisImportera>();
            UrlExtender.ZbudujLink(helper, import);

            DokumentyBll dokument = A.Fake<DokumentyBll>();
            UrlExtender.ZbudujLink(helper, dokument,jezyk);

            //ZbudujLink(this UrlHelper helper, DokumentyBll dokument, Jezyk jezyk)
        }
    }
}
