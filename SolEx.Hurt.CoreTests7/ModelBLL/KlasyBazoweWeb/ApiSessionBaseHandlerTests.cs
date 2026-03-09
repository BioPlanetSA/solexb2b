using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.WebSockets;
using FakeItEasy;
using ServiceStack.Common.Utils;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Modules.Api;
using Xunit;
namespace SolEx.Hurt.Web.Modules.Api.Tests
{
    public class ApiSessionBaseHandlerTests
    {
        [Fact()]
        public void HandleRequestTest()
        {
            var api = A.Fake<ApiSessionBaseHandler>();
            HttpRequest request = new HttpRequest("localhost","http://localhost","");
            TextWriter tw = new StringWriter();
            HttpResponse response = new HttpResponse(tw);
            
            HttpContext context = new HttpContext(request, response);//  A.Fake<HttpContext>(a => a.WithArgumentsForConstructor(new object[]{ request, response }));
            klienci klient = new klienci();
            klient.klient_id = 0;
            A.CallTo(() => api.Customer).Returns(klient);
            A.CallTo(() => api.WymagajAktywnejSesji).Returns(true);
            api.Wykonaj(context);
        }
    }
}
