using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Model;
using Xunit;

namespace SolEx.Hurt.CoreTests.DostepDane
{
    public class OrmLiteWriteConnectionExtensionsExtensionsTests
    {

        private string baza = ":memory:";
        private OrmLiteConnectionFactory polaczenie;

        public OrmLiteWriteConnectionExtensionsExtensionsTests()
        {
            polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider) { DialectProvider = { UseUnicode = true }, AutoDisposeConnection = false };
        }

        [Fact(DisplayName = "Test serialializacji slownika")]
        public void TestDeserializator()
        {
            List<Dictionary<object, object>> testowySLownik = new List<Dictionary<object, object>>();


            Dictionary<string, HashSet<int>> slownikOdpowiedni = new Dictionary<string, HashSet<int>>();

            for (int i = 0; i < 100; i++)
            {
                string iString = i.ToString();
                slownikOdpowiedni.Add(iString, new HashSet<int>());
                for (int j = 0; j < 100; j++)
                {
                    slownikOdpowiedni[iString].Add(j);
                }
            }
            for (int i = 0; i < 50; i++)
            {
                testowySLownik.Add(slownikOdpowiedni.ToDictionary(x=>(object)x.Key, x=>(object)x.Value));
            }

            var test = TypeSerializer.SerializeToString(testowySLownik);

            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            DostepDoDanych dd = new DostepDoDanych(solexBllCalosc);
            dd.DbFactory = polaczenie;
            Produkt p =new Produkt(1);
            p.Kod = "test";
            p.Nazwa = "test";
            p.Widoczny = true;
            p.StanMin = 0;
            p.IloscWOpakowaniu = 1;
            p.IloscMinimalna = 1;
            p.WymaganeOz = false;
            p.DataDodania=DateTime.Now;
            p.WyslanoMailNowyProdukt = true;
            p.Abstrakcyjny = true;
            p.DaneDlaProduktowWirtualnych = testowySLownik;
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Produkt>();
            }

            dd.AktualizujPojedynczy(p);

            var prod = dd.PobierzPojedynczy<Produkt>(1);

        }




    }
}
