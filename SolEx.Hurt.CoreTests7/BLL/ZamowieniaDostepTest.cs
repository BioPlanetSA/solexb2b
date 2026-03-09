using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL
{
    public class ZamowieniaDostepTest
    {
        private string baza = ":memory:";
        private OrmLiteConnectionFactory polaczenie;
        public ZamowieniaDostepTest()
        {
            polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider) { DialectProvider = { UseUnicode = true }, AutoDisposeConnection = false };
        }


        [Fact(DisplayName = "Test sprawdzający poprawne tworzenie oraz zapis statusów zamówień w oparciu o Enuma")]
        public void SprawdzStatusy_UtworzStatusySystemoweTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            IConfigBLL konfiguracja = A.Fake<IConfigBLL>();
            A.CallTo(() => konfiguracja.StatusyZamowien).Returns(new Dictionary<int, StatusZamowienia>());
            A.CallTo(() => solexBllCalosc.Konfiguracja).Returns(konfiguracja);
            string symbol = "test";
            using (var db = polaczenie.OpenDbConnection())
            {
                StatusZamowienia sz = new StatusZamowienia();
                sz.Id = 1;
                sz.Symbol = symbol;
                db.CreateTable<StatusZamowienia>();
                db.Insert(sz);
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostep);

            ZamowieniaDostep zd = new ZamowieniaDostep(solexBllCalosc);
            zd.SprawdzStatusy_UtworzStatusySystemowe();

            var statusyZamowien = dostep.Pobierz<StatusZamowienia>(null);
            Assert.True(statusyZamowien.Count()==6);
            Assert.True(statusyZamowien.FirstOrDefault(x=>x.Symbol == symbol)==null);
        }

        [Fact(DisplayName = "Test sprawdzający poprawne zapisywanie i pobieranie zamówień oraz czy status przypisuje się prawidłowo z new StatusId")]
        public void TestPobieranieZamowien()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            IConfigBLL konfiguracja = A.Fake<IConfigBLL>();
            A.CallTo(() => konfiguracja.StatusyZamowien).Returns(new Dictionary<int, StatusZamowienia>());
            A.CallTo(() => solexBllCalosc.Konfiguracja).Returns(konfiguracja);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Zamowienie>();
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostep);

            Zamowienie z = new Zamowienie();
            z.Id = 1;
            z.StatusId = StatusImportuZamowieniaDoErp.Złożone;
            //Dodajemy zamowienie
            dostep.AktualizujPojedynczy(z);

            var zemowienie = dostep.Pobierz<ZamowieniaBLL>(null);

            Assert.True(zemowienie.First().StatusId==StatusImportuZamowieniaDoErp.Złożone);
        }


    }
}
