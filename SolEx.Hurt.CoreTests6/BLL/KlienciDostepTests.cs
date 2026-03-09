using System;
using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Tests
{
    public class KlienciDostepTests
    {
        [Fact()]
        public void WyliczOdKiedyTest()
        {
           TestWyliczOdKiedy(6,null,new DateTime(2016,1,1),new DateTime(2016,1,1));
           TestWyliczOdKiedy(6, null, new DateTime(2016, 7, 2), new DateTime(2016, 7, 1));
           TestWyliczOdKiedy(6, null, new DateTime(2016, 7, 1), new DateTime(2016, 7, 1));
           TestWyliczOdKiedy(6, new DateTime(2016, 3, 1), new DateTime(2016, 7, 1), new DateTime(2016, 3, 1));
           TestWyliczOdKiedy(6, new DateTime(2016, 3, 1), new DateTime(2016, 10, 1), new DateTime(2016, 9, 1));
        }

        private void TestWyliczOdKiedy(int iloscMiesiecy, DateTime? odkiedy,DateTime aktualna,DateTime oczekiwana)
        {
             ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            KlienciDostep kd = new KlienciDostep(calosc);
            SzablonLimitow sl=new SzablonLimitow();
            sl.IloscMiesiecy = iloscMiesiecy;
            sl.OdKiedy = odkiedy;
            DateTime data = kd.WyliczOdKiedy(sl, aktualna);
            Assert.Equal(oczekiwana,data);
        }

        [Fact()]
        public void DokumentyDoLiczeniaLimitowTest()
        {
            TestPobieraniaDokumentow1();
            TestPobieraniaDokumentow2();
            TestPobieraniaDokumentow3();
        }

        [Fact()]
        public void CzyKlientNieMaMinimumLogistyczneTest()
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => config.DeaktywujMinimumLogistyczneDlaWybranychKategoriiKlientow).Returns(new HashSet<int>() {10,11,12});
            A.CallTo(() => config.JezykIDDomyslny).Returns(1);
            KlienciDostep kd = new KlienciDostep(calosc);
            IKlient klient = A.Fake<IKlient>();

            A.CallTo(() => klient.Kategorie).Returns(new int[] {1, 2, 10, 15});
            bool wynik = kd.CzyKlientNieMaMinimumLogistyczne(klient);
            Assert.True(wynik);


            A.CallTo(() => klient.Kategorie).Returns(new int[] { 1, 2, 14, 15 });
            wynik = kd.CzyKlientNieMaMinimumLogistyczne(klient);
            Assert.False(wynik);

            A.CallTo(() => klient.Kategorie).Returns(new int[] {10});
            wynik = kd.CzyKlientNieMaMinimumLogistyczne(klient);
            Assert.True(wynik);

        }
        private void TestPobieraniaDokumentow1()
        {
            DateTime odkiedy=new DateTime(2016,1,1);
            Klient tmp=new Klient();
            tmp.Id = 1;
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDokumentyDostep dok = A.Fake<IDokumentyDostep>();
            A.CallTo(() => calosc.DokumentyDostep).Returns(dok);
            List<DokumentyBll> dokfake=new List<DokumentyBll>();
            dokfake.Add(A.Fake<DokumentyBll>());
            A.CallTo(() => dok.PobierzWyfiltrowaneDokumenty(tmp, tmp, RodzajDokumentu.Zamowienie, odkiedy, A<DateTime>.Ignored, "", true, false)).Returns(dokfake);

            HashSet<long> klienci=new HashSet<long>();
            klienci.Add(tmp.Id);
            KlienciDostep kd = new KlienciDostep(calosc);
            var wynik= kd.DokumentyDoLiczeniaLimitow(tmp, odkiedy, klienci);
            Assert.Equal(wynik.Count,1);
        }
        private void TestPobieraniaDokumentow2()
        {
            DateTime odkiedy = new DateTime(2016, 1, 1);
            Klient tmp = new Klient();
            tmp.Id = 1;
            Klient idk2 = new Klient() {Id = 2};
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDokumentyDostep dok = A.Fake<IDokumentyDostep>();
            A.CallTo(() => calosc.DokumentyDostep).Returns(dok);
            List<DokumentyBll> dokfake = new List<DokumentyBll>();
            dokfake.Add(A.Fake<DokumentyBll>());
            A.CallTo(() => dok.PobierzWyfiltrowaneDokumenty(tmp, tmp, RodzajDokumentu.Zamowienie, odkiedy, A<DateTime>.Ignored,  "", true, false)).Returns(dokfake);
            A.CallTo(() => dok.PobierzWyfiltrowaneDokumenty(tmp, idk2, RodzajDokumentu.Zamowienie, odkiedy, A<DateTime>.Ignored, "", true, false)).Returns(dokfake);
            HashSet<long> klienci = new HashSet<long>();
            klienci.Add(tmp.Id);

            KlienciDostep kd = new KlienciDostep(calosc);
            var wynik = kd.DokumentyDoLiczeniaLimitow(tmp, odkiedy, klienci);
            Assert.Equal(wynik.Count, 1);
        }
        private void TestPobieraniaDokumentow3()
        {
            DateTime odkiedy = new DateTime(2016, 1, 1);
            Klient tmp = new Klient();
            tmp.Id = 1;
            Klient idk2 = new Klient() { Id = 2 };
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDokumentyDostep dok = A.Fake<IDokumentyDostep>();
            A.CallTo(() => calosc.DokumentyDostep).Returns(dok);
            List<DokumentyBll> dokfake = new List<DokumentyBll>();
            dokfake.Add(A.Fake<DokumentyBll>());
            A.CallTo(() => dok.PobierzWyfiltrowaneDokumenty(tmp, tmp, RodzajDokumentu.Zamowienie, odkiedy, A<DateTime>.Ignored,  "", true, false)).Returns(dokfake);
            A.CallTo(() => dok.PobierzWyfiltrowaneDokumenty(tmp, idk2, RodzajDokumentu.Zamowienie, odkiedy, A<DateTime>.Ignored,  "", true, false)).Returns(dokfake);
            HashSet<long> klienci = new HashSet<long>();
            klienci.Add(tmp.Id);
            klienci.Add(idk2.Id);
            KlienciDostep kd = new KlienciDostep(calosc);
            var wynik = kd.DokumentyDoLiczeniaLimitow(tmp, odkiedy, klienci);
            Assert.Equal(wynik.Count, 2);
        }
    }
}
