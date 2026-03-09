using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Bindowania;
using Xunit;

namespace SolEx.Hurt.Web.Site2Tests.Bindowania
{
    public class UniwersalnyBinderTests
    {
        [Fact(DisplayName = "Test uniwersalnego bindera")]
        public void BindModelTest()
        {
            //klient
            NameValueCollection polaformy = new NameValueCollection();
            polaformy.Add("Nazwa", "test1");
            polaformy.Add("Id", "1");


            Dictionary<string, object> wynik = new Dictionary<string, object>();
            wynik.Add("Nazwa", "test1");
            wynik.Add("Id", 1);
            Tests(typeof(Klient),polaformy,wynik);

            //produkt 
            polaformy = new NameValueCollection();

            polaformy.Add("Nazwa", "test1 f");
            polaformy.Add("Id", "99");

            wynik = new Dictionary<string, object>();

            wynik.Add("Nazwa", "test1 f");
            wynik.Add("Id", 99);
            Tests(typeof(Produkt), polaformy, wynik);
        }

        private void Tests(Type t, NameValueCollection forma, Dictionary<string,object> oczerkiwane)
        {
            var propsc = t.GetProperties();
            UniwersalnyBinder ub = new UniwersalnyBinder();
            var cc = A.Fake<ControllerContext>();

            var context = A.Fake<HttpContextBase>();
            var req = A.Fake<HttpRequestBase>();
 
            A.CallTo(() => req.Form).Returns(forma);
            A.CallTo(() => context.Request).Returns(req);
            cc.HttpContext = context;
            var mb = new ModelBindingContext();
            var mmb = new ModelMetadata(new CachedDataAnnotationsModelMetadataProvider(), null,null,t,"");

            mb.ModelMetadata = mmb;
            object o = ub.BindModel(cc, mb);
            Assert.True(o != null, "Jest nulleem");
            Assert.Equal(t, o.GetType());
            foreach (var ocz in oczerkiwane)
            {
                var prop = propsc.First(x => x.Name == ocz.Key);
                object val = prop.GetValue(o);

                Assert.True(ocz.Value.ToString() == val.ToString());
            }

        }
    }
}
