using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow;
using SolEx.Hurt.Sync.App.Modules_.Rozne;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.Rozne.Tests
{
    public class KtorePolaSynchonizowacBazaTests
    {
        [Fact()]
        public void UstawPolaTest()
        {
            Klient docelowy = new Klient();
            docelowy.Id = 1;
            docelowy.HasloKlienta = "";
            docelowy.JezykId = 0;

            Klient wzorcowy = new Klient();
            wzorcowy.Id = 10;
            wzorcowy.HasloKlienta = "kkkkkkkkkkkkk";
            wzorcowy.JezykId = 1;

            PropertyInfo[] propertisy = typeof(Klient).GetProperties();
            List<string> Pola = new List<string>();
            Pola.Add("HasloKlienta");
            Pola.Add("JezykId");
            KtorePolaSynchonizowacBaza proc = new KtorePolaSynchronizowac();
            proc.UstawPola(docelowy, wzorcowy, propertisy, Pola);
            Assert.True(docelowy.HasloKlienta == wzorcowy.HasloKlienta);
        }
    }
}
