//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Caching;
//using FakeItEasy;
//using Fasterflect;
//using ServiceStack.Common;
//using SolEx.Hurt.Core.BLL;
//using SolEx.Hurt.Core.BLL.Interfejsy;
//using SolEx.Hurt.Core.ModelBLL;
//using SolEx.Hurt.Core.ModelBLL.Interfejsy;
//using SolEx.Hurt.Helpers;
//using SolEx.Hurt.Model;
//using SolEx.Hurt.Model.Core;
//using SolEx.Hurt.Model.CustomSearchCriteria;
//using SolEx.Hurt.Model.Enums;
//using SolEx.Hurt.Model.Helpers;
//using SolEx.Hurt.Model.Interfaces;
//using SolEx.Hurt.Model.Web;
//using Xunit;
//using System.ServiceModel.Security;
//namespace SolEx.Hurt.Core.BLL.Tests
//{
//    public class DokumentyTests
//    {
//        [Fact()]
//        public void WyslijMailaONowymDokumencieTest()
//        {
//            TestWyslijMailaONowymDokumencieTest(DateTime.Now, null, 3, true,true,true);
//            TestWyslijMailaONowymDokumencieTest(DateTime.Now, null, 3, false, false, false);
//            TestWyslijMailaONowymDokumencieTest(DateTime.Now, DateTime.Now, 3, false, true, true);
//            TestWyslijMailaONowymDokumencieTest(DateTime.Now.AddDays(-5), null, 3, false, true, true);
//        }
//        public void TestWyslijMailaONowymDokumencieTest(DateTime datautworzenia, DateTime? dataWyslania, int ilednimax, bool wynik,bool jestzalacznik,bool wysylajgdybrak)
//        {
//            int jezytk = 1;
//            var dokument = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty()}));//, new List<DokumentyPozycje>(),  new List<ListyPrzewozowe>() }));
//            dokument.id = 1;
//            dokument.data_utworzenia = datautworzenia;
//            dokument.DataWyslaniaDokumentu = dataWyslania;
         
//            var dokumenty = StworzFakeDokumentow(ilednimax,jezytk);
//            A.CallTo(() => dokumenty.ConfigBll.WysylajPowiadomienieFakturaGdyBrakPdf).Returns(wysylajgdybrak);
//            A.CallTo(() => dokumenty.IstniejeZalacznik(dokument, A<string>.Ignored)).Returns(jestzalacznik);
//            bool result = dokumenty.CzyWyslacMailaONowymDokumencie(dokument);
//            bool tmp = result == wynik;
//            Assert.True(tmp, string.Format("data utrzonie {0}, data wyslania {1} ile dni max {2} wynik {3} oczekiwany {4}", datautworzenia, dataWyslania
//                ,ilednimax,wynik,result));
//        }
//        private Dokumenty StworzFakeDokumentow(int iledni,int jezyk)
//        {
//            var dokumenty = A.Fake<Dokumenty>();
//            var config = A.Fake<IConfigBLL>();
//            A.CallTo(() => config.DokumentyMailOnNowymIleDniWstecz).Returns(iledni);
//            A.CallTo(() => config.JezykIDPolski).Returns(jezyk);
//            dokumenty.ConfigBll = config;
//            return dokumenty;
//        }

//        [Fact(DisplayName = "Test sprawdzajacy poprawność pobierania niezaplaconych dokumentem wzgledem daty")]
//        public void PobierzDokumentyNiezaplaconeWzgledemDatyTest()
//        {
//            klienci klient = new klienci(null) { klient_id = 1 };
//            var statystyki = A.Fake<IStatystyki>();

//            int jezyk = 1;
//            var dokumenty = StworzFakeDokumentow(0, jezyk);
//            List<IDokument> niezaplacone = new List<IDokument>();

//            var dokument = A.Fake<IDokument>();//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
//            A.CallTo(() => dokument.DokumentId).Returns(1);
//            A.CallTo(() => dokument.DokumentNazwa).Returns("nazwa 1");
//            A.CallTo(() => dokument.DokumentTerminPlatnosci).Returns(DateTime.Now.Date.AddDays(2));
//            A.CallTo(() => dokument.DokumentWartoscBrutto).Returns(2000);
//            A.CallTo(() => dokument.DokumentWartoscNalezna).Returns(3000);
//            niezaplacone.Add(dokument);

//            var dokument2 = A.Fake<IDokument>();
//            A.CallTo(() => dokument2.DokumentId).Returns(2);
//            A.CallTo(() => dokument2.DokumentNazwa).Returns("nazwa 2");
//            A.CallTo(() => dokument2.DokumentTerminPlatnosci).Returns(DateTime.Now.Date.AddDays(2));
//            A.CallTo(() => dokument2.DokumentWartoscBrutto).Returns(-2000);
//            A.CallTo(() => dokument2.DokumentWartoscNalezna).Returns(3000);

//            niezaplacone.Add(dokument2);
//            var dokument3 = A.Fake<IDokument>();
//            A.CallTo(() => dokument3.DokumentId).Returns(3);
//            A.CallTo(() => dokument3.DokumentNazwa).Returns("nazwa 3");
//            A.CallTo(() => dokument3.DokumentTerminPlatnosci).Returns(DateTime.Now.Date.AddDays(2));
//            A.CallTo(() => dokument3.DokumentWartoscBrutto).Returns(2000);
//            A.CallTo(() => dokument3.DokumentWartoscNalezna).Returns(-3000);
//            niezaplacone.Add(dokument3);

//            var dokument4 = A.Fake<IDokument>();
//            A.CallTo(() => dokument4.DokumentId).Returns(4);
//            A.CallTo(() => dokument4.DokumentNazwa).Returns("nazwa 4");
//            A.CallTo(() => dokument4.DokumentTerminPlatnosci).Returns(DateTime.Now.Date.AddDays(-3));
//            A.CallTo(() => dokument4.DokumentWartoscBrutto).Returns(2000);
//            A.CallTo(() => dokument4.DokumentWartoscNalezna).Returns(3000);
//            niezaplacone.Add(dokument4);

//            var dokument5 = A.Fake<IDokument>();//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
//            A.CallTo(() => dokument5.DokumentId).Returns(5);
//            A.CallTo(() => dokument5.DokumentNazwa).Returns("nazwa 5");
//            A.CallTo(() => dokument5.DokumentTerminPlatnosci).Returns(DateTime.Now.Date.AddDays(7));
//            A.CallTo(() => dokument5.DokumentWartoscBrutto).Returns(2000);
//            A.CallTo(() => dokument5.DokumentWartoscNalezna).Returns(3000);
//            niezaplacone.Add(dokument5);

//            AkcjaKlienta a1 = new AkcjaKlienta() { IdWpisu = 1 };
//            List<AkcjaKlienta> listaAkcji = new List<AkcjaKlienta>() { a1 };

//            A.CallTo(() => statystyki.ZnajdzZdarzenie(ZdarzenieGlowne.WyslanieMaila, RodzajZdarzenia.PrzypomnienieNiezaplaconeFaktury, "Dokument", dokument.DokumentNazwa)).Returns(listaAkcji);
//            A.CallTo(() => statystyki.ZnajdzZdarzenie(ZdarzenieGlowne.WyslanieMaila, RodzajZdarzenia.PrzypomnienieNiezaplaconeFaktury, "Dokument", dokument2.DokumentNazwa)).Returns(new List<AkcjaKlienta>());
//            A.CallTo(() => statystyki.ZnajdzZdarzenie(ZdarzenieGlowne.WyslanieMaila, RodzajZdarzenia.PrzypomnienieNiezaplaconeFaktury, "Dokument", dokument3.DokumentNazwa)).Returns(listaAkcji);

//            A.CallTo(() => statystyki.DodajZdarzenie(ZdarzenieGlowne.WyslanieMaila, RodzajZdarzenia.PrzypomnienieNiezaplaconeFaktury, "Dokument", dokument3.DokumentNazwa, klient)).DoesNothing();

            A.CallTo(() => dokumenty.PobierzNiezaplaconweFaktury()).Returns(niezaplacone);
            dokumenty.Statystyki = statystyki;
            var wynik = dokumenty.PobierzDokumentyNiezaplaconeWzgledemDaty(6);
            bool result = wynik.Count == 1 && wynik.Any(x => x.Key == dokument);
            Assert.True(result);
        }

//        [Fact(DisplayName = "Test sprawdzający poprawność pobierania przeterminowanych dokumentow wzgledem daty")]
//        public void PobierzDokumentyPrzeterminowaneWzgledemDatyTest()
//        {
//            klienci klient = new klienci(null) {klient_id = 1};
//            var statystyki = A.Fake<IStatystyki>();
            
//            int jezyk = 1;
//            var dokumenty = StworzFakeDokumentow(0, jezyk);
//            List<IDokument> niezaplacone = new List<IDokument>();
           
//            //dokument niezapłacowny
//            var dokument = A.Fake<IDokument>();//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
//            A.CallTo(() => dokument.DokumentId).Returns(1);
//            A.CallTo(() => dokument.DokumentNazwa).Returns("nazwa 1");
//            A.CallTo(() => dokument.DokumentDataWystawienia).Returns(DateTime.Now.Date.AddDays(-15));
//            A.CallTo(() => dokument.DokumentTerminPlatnosci).Returns(DateTime.Now.Date.AddDays(-5));
//            A.CallTo(() => dokument.DokumentWartoscBrutto).Returns(2000);
//            A.CallTo(() => dokument.DokumentWartoscNalezna).Returns(3000);
//            niezaplacone.Add(dokument);
            
//            //dokument przeterminowany
//            var dokument2 = A.Fake<IDokument>();
//            A.CallTo(() => dokument2.DokumentId).Returns(2);
//            A.CallTo(() => dokument2.DokumentNazwa).Returns("nazwa 2");
//            A.CallTo(() => dokument2.DokumentDataWystawienia).Returns(DateTime.Now.Date.AddDays(-15));
//            A.CallTo(() => dokument2.DokumentTerminPlatnosci).Returns(DateTime.Now.Date.AddDays(-5));
//            A.CallTo(() => dokument2.DokumentWartoscBrutto).Returns(-2000);
//            A.CallTo(() => dokument2.DokumentWartoscNalezna).Returns(3000);
//            niezaplacone.Add(dokument2);

//            var dokument3 = A.Fake<IDokument>();
//            A.CallTo(() => dokument3.DokumentId).Returns(3);
//            A.CallTo(() => dokument3.DokumentNazwa).Returns("nazwa 3");
//            A.CallTo(() => dokument3.DokumentDataWystawienia).Returns(DateTime.Now.Date.AddDays(-15));
//            A.CallTo(() => dokument3.DokumentTerminPlatnosci).Returns(DateTime.Now.Date.AddDays(-5));
//            A.CallTo(() => dokument3.DokumentWartoscBrutto).Returns(2000);
//            A.CallTo(() => dokument3.DokumentWartoscNalezna).Returns(-3000);
//            niezaplacone.Add(dokument3);

//            var dokument4 = A.Fake<IDokument>();
//            A.CallTo(() => dokument4.DokumentId).Returns(4);
//            A.CallTo(() => dokument4.DokumentNazwa).Returns("nazwa 4");
//            A.CallTo(() => dokument4.DokumentDataWystawienia).Returns(DateTime.Now.Date.AddDays(-15));
//            A.CallTo(() => dokument4.DokumentTerminPlatnosci).Returns(DateTime.Now.Date.AddDays(11));
//            A.CallTo(() => dokument4.DokumentWartoscBrutto).Returns(2000);
//            A.CallTo(() => dokument4.DokumentWartoscNalezna).Returns(3000);
//            niezaplacone.Add(dokument4);

//            var dokument5 = A.Fake<IDokument>();//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
//            A.CallTo(() => dokument5.DokumentId).Returns(5);
//            A.CallTo(() => dokument5.DokumentNazwa).Returns("nazwa 5");
//            A.CallTo(() => dokument5.DokumentDataWystawienia).Returns(DateTime.Now.Date.AddDays(-15));
//            A.CallTo(() => dokument5.DokumentTerminPlatnosci).Returns(DateTime.Now.Date.AddDays(-1));
//            A.CallTo(() => dokument5.DokumentWartoscBrutto).Returns(2000);
//            A.CallTo(() => dokument5.DokumentWartoscNalezna).Returns(3000);
//            niezaplacone.Add(dokument5);

//            AkcjaKlienta a1 = new AkcjaKlienta(){IdWpisu = 1};
//            List<AkcjaKlienta> listaAkcji = new List<AkcjaKlienta>(){a1};

//            A.CallTo(() =>statystyki.ZnajdzZdarzenie(ZdarzenieGlowne.WyslanieMaila,RodzajZdarzenia.PrzypomnieniePrzeterminowaneFaktury, "Dokument", dokument.DokumentNazwa)).Returns(listaAkcji);
//            A.CallTo(() => statystyki.ZnajdzZdarzenie(ZdarzenieGlowne.WyslanieMaila, RodzajZdarzenia.PrzypomnieniePrzeterminowaneFaktury, "Dokument", dokument2.DokumentNazwa)).Returns(new List<AkcjaKlienta>());
//            A.CallTo(() => statystyki.ZnajdzZdarzenie(ZdarzenieGlowne.WyslanieMaila, RodzajZdarzenia.PrzypomnieniePrzeterminowaneFaktury, "Dokument", dokument3.DokumentNazwa)).Returns(listaAkcji);

//            A.CallTo(() =>statystyki.DodajZdarzenie(ZdarzenieGlowne.WyslanieMaila,RodzajZdarzenia.PrzypomnieniePrzeterminowaneFaktury, "Dokument", dokument3.DokumentNazwa, klient)).DoesNothing();

            A.CallTo(() => dokumenty.PobierzNiezaplaconweFaktury()).Returns(niezaplacone);
            dokumenty.Statystyki = statystyki;
            var wynik = dokumenty.PobierzDokumentyPrzeterminowaneWzgledemDaty(3);//które przeterminowały się conajmniej 3 dni temu
            bool result = wynik.Count == 1 && wynik.Any(x => x.Key  == dokument);
            Assert.True(result);
        }

//        [Fact()]
//        public void PobierzDokumentyDoWyslaniaInfoTest()
//        {
//            PobierzDokumentyDoWyslaniaInfoTest1();
//            PobierzDokumentyDoWyslaniaInfoTest2();
//            PobierzDokumentyDoWyslaniaInfoTest3();
//        }

//        private void PobierzDokumentyDoWyslaniaInfoTest1()
//        {
//            int jezyk = 1;
//            var dokumenty = StworzFakeDokumentow(0, jezyk);
//            Dictionary<IDokument, bool> niezaplacone = new Dictionary<IDokument, bool>();
//            //dokument niezapłacony
//            var dokument = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty()}));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
//            dokument.id = 1;
//            dokument.nazwa_dokumentu = "nazwa 1";
//            dokument.data_utworzenia = DateTime.Now.Date.AddDays(-5);
//            dokument.termin_platnosci = DateTime.Now.Date.AddDays(5);
//            dokument.klient_id = 1;
//            dokument.zaplacono = false;
//            dokument.wartosc_nalezna = 5;
//            dokument.wartosc_brutto = 5;
//            niezaplacone.Add(dokument, false);
//            var dokument2 = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
//            dokument2.id = 2;
//            dokument2.nazwa_dokumentu = "nazwa 2";
//            dokument2.data_utworzenia = DateTime.Now.Date.AddDays(-5);
//            dokument2.termin_platnosci = DateTime.Now.Date.AddDays(10);
//            dokument2.klient_id = 1;
//            dokument2.zaplacono = false;
//            dokument2.wartosc_nalezna = 5;
//            dokument2.wartosc_brutto = 5;
//            niezaplacone.Add(dokument2, false);

//            Dictionary<IDokument, bool> przeterminowane = new Dictionary<IDokument, bool>();
//            //przeterminowane
//            var dokument3 = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
//            dokument3.id = 3;
//            dokument3.nazwa_dokumentu = "nazwa 3";
//            dokument3.data_utworzenia = DateTime.Now.Date.AddDays(-5);
//            dokument3.termin_platnosci = DateTime.Now.Date.AddDays(5);
//            dokument3.klient_id = 1;
//            dokument3.zaplacono = false;
//            dokument3.wartosc_nalezna = 5;
//            dokument3.wartosc_brutto = 5;
//            przeterminowane.Add(dokument3, false);
//            //dokument przeterminowany
//            var dokument4 = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
//            dokument4.id = 4;
//            dokument4.data_utworzenia = DateTime.Now.Date.AddDays(-5);
//            dokument4.termin_platnosci = DateTime.Now.Date.AddDays(-4);
//            dokument4.klient_id = 1;
//            dokument4.zaplacono = false;
//            dokument4.wartosc_nalezna = 5;
//            dokument4.wartosc_brutto = 5;
//            dokument4.nazwa_dokumentu = "nazwa 4";
//            przeterminowane.Add(dokument4, false);

            var wynik = dokumenty.PobierzDokumentyDoWyslaniaInfo(niezaplacone, przeterminowane,new klienci());
            bool result = wynik[1].Count == 4;
            Assert.True(result, "ilosc do wyslania");
        }

//        private void PobierzDokumentyDoWyslaniaInfoTest2()
//        {
//            int jezyk = 1;
//            var dokumenty = StworzFakeDokumentow(0, jezyk);
//            Dictionary<IDokument, bool> niezaplacone = new Dictionary<IDokument, bool>();
//            //dokument niezapłacony
//            var dokument = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
//            dokument.id = 1;
//            dokument.nazwa_dokumentu = "nazwa 1";
//            dokument.data_utworzenia = DateTime.Now.Date.AddDays(-5);
//            dokument.termin_platnosci = DateTime.Now.Date.AddDays(5);
//            dokument.klient_id = 1;
//            dokument.zaplacono = false;
//            dokument.wartosc_nalezna = 5;
//            dokument.wartosc_brutto = 5;
//            niezaplacone.Add(dokument, true);
//            var dokument2 = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
//            dokument2.id = 2;
//            dokument2.nazwa_dokumentu = "nazwa 2";
//            dokument2.data_utworzenia = DateTime.Now.Date.AddDays(-5);
//            dokument2.termin_platnosci = DateTime.Now.Date.AddDays(10);
//            dokument2.klient_id = 1;
//            dokument2.zaplacono = false;
//            dokument2.wartosc_nalezna = 5;
//            dokument2.wartosc_brutto = 5;
//            niezaplacone.Add(dokument2, true);

            Dictionary<IDokument, bool> przeterminowane = new Dictionary<IDokument, bool>();
            //przeterminowane
            var dokument3 = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
            dokument3.id = 3;
            dokument3.nazwa_dokumentu = "nazwa 3";
            dokument3.data_utworzenia = DateTime.Now.Date.AddDays(-5);
            dokument3.termin_platnosci = DateTime.Now.Date.AddDays(5);
            dokument3.klient_id = 1;
            dokument3.zaplacono = false;
            dokument3.wartosc_nalezna = 5;
            dokument3.wartosc_brutto = 5;
            przeterminowane.Add(dokument3, true);
            //dokument przeterminowany
            var dokument4 = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
            dokument4.id = 4;
            dokument4.data_utworzenia = DateTime.Now.Date.AddDays(-5);
            dokument4.termin_platnosci = DateTime.Now.Date.AddDays(-4);
            dokument4.klient_id = 1;
            dokument4.zaplacono = false;
            dokument4.wartosc_nalezna = 5;
            dokument4.wartosc_brutto = 5;
            dokument4.nazwa_dokumentu = "nazwa 4";
            przeterminowane.Add(dokument4, true);
            var wynik = dokumenty.PobierzDokumentyDoWyslaniaInfo(niezaplacone, przeterminowane, new klienci());
            bool result = !wynik.ContainsKey(1);
            Assert.True(result, "ilosc do wyslania");
        }
        private void PobierzDokumentyDoWyslaniaInfoTest3()
        {
            int jezyk = 1;
            var dokumenty = StworzFakeDokumentow(0, jezyk);
            Dictionary<IDokument, bool> niezaplacone = new Dictionary<IDokument, bool>();
            //dokument niezapłacony
            var dokument = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
            dokument.id = 1;
            dokument.nazwa_dokumentu = "nazwa 1";
            dokument.data_utworzenia = DateTime.Now.Date.AddDays(-5);
            dokument.termin_platnosci = DateTime.Now.Date.AddDays(5);
            dokument.klient_id = 1;
            dokument.zaplacono = false;
            dokument.wartosc_nalezna = 5;
            dokument.wartosc_brutto = 5;
            niezaplacone.Add(dokument, false);
            var dokument2 = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
            dokument2.id = 2;
            dokument2.nazwa_dokumentu = "nazwa 2";
            dokument2.data_utworzenia = DateTime.Now.Date.AddDays(-5);
            dokument2.termin_platnosci = DateTime.Now.Date.AddDays(10);
            dokument2.klient_id = 1;
            dokument2.zaplacono = false;
            dokument2.wartosc_nalezna = 5;
            dokument2.wartosc_brutto = 5;
            niezaplacone.Add(dokument2, true);

            Dictionary<IDokument, bool> przeterminowane = new Dictionary<IDokument, bool>();
            //przeterminowane
            var dokument3 = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
            dokument3.id = 3;
            dokument3.nazwa_dokumentu = "nazwa 3";
            dokument3.data_utworzenia = DateTime.Now.Date.AddDays(-5);
            dokument3.termin_platnosci = DateTime.Now.Date.AddDays(5);
            dokument3.klient_id = 1;
            dokument3.zaplacono = false;
            dokument3.wartosc_nalezna = 5;
            dokument3.wartosc_brutto = 5;
            przeterminowane.Add(dokument3, true);
            //dokument przeterminowany
            var dokument4 = A.Fake<DokumentyBll>(a => a.WithArgumentsForConstructor(new Object[] { new historia_dokumenty() }));//, new List<DokumentyPozycje>(), new List<ListyPrzewozowe>() }));
            dokument4.id = 4;
            dokument4.data_utworzenia = DateTime.Now.Date.AddDays(-5);
            dokument4.termin_platnosci = DateTime.Now.Date.AddDays(-4);
            dokument4.klient_id = 1;
            dokument4.zaplacono = false;
            dokument4.wartosc_nalezna = 5;
            dokument4.wartosc_brutto = 5;
            dokument4.nazwa_dokumentu = "nazwa 4";
            przeterminowane.Add(dokument4, true);
            var wynik = dokumenty.PobierzDokumentyDoWyslaniaInfo(niezaplacone, przeterminowane, new klienci());
            bool result = wynik[1].Count ==4;
            Assert.True(result, "ilosc do wyslania");
        }

//        [Fact(DisplayName = "Czy klient ma dostęp do wybranego dokumentu")]
//        public void CzyKlientMaDostepTest()
//        {
//            TestDostepu(1, 1, 2, RodzajDokumentu.Faktura, true);
//            TestDostepu(1, 2, 1, RodzajDokumentu.Faktura, false);
//            TestDostepu(1, 1, 2, RodzajDokumentu.Zamowienie, true);
//            TestDostepu(1, 2, 1, RodzajDokumentu.Zamowienie, true);
//        }

//        private void TestDostepu(int idklient, int dokklient,int odbiorca,RodzajDokumentu rodzaj, bool wynik)
//        {
//            var klient = A.Fake<IKlient>();
//            klient.klient_id = idklient;
//            A.CallTo(() => klient.WszystkieKontaPodrzedne()).Returns(new List<IKlient>());
            
//            var dd = A.Fake<Dokumenty>();
//            IDokument tmp = A.Fake<IDokument>();

//            A.CallTo(() => tmp.DokumentOdbiorcaId).Returns(odbiorca);
//            A.CallTo(() => tmp.DokummentRodzaj).Returns(rodzaj);
//            A.CallTo(() => tmp.DokumentPlatnikId).Returns(dokklient);

//            var klienciDostep = A.Fake<IKlienciDostep>();
//            A.CallTo(() => klienciDostep.JestOpiekunem(tmp.DokumentPlatnikId, klient)).Returns(false);

//            dd.Kliencidostep = klienciDostep;
//            var czyjest = dd.CzyKlientMaDostep(tmp, klient);
//            Assert.True(czyjest == wynik, string.Format("klient id {0} dok {1} odb {2} rodzaj {3} wnik {4} oczekiwane {5}", idklient, dokklient, odbiorca, rodzaj, czyjest, wynik));
//        }


//        [Fact(DisplayName = "Czy klient ma dostęp do wybranego dokumentu druga metoda")]
//        public void CzyKlientMaDostepTest2()
//        {
//            TestDostepu2(1, 1, 2, RodzajDokumentu.Faktura, true);
//            TestDostepu2(1, 2, 1, RodzajDokumentu.Faktura, false);
//            TestDostepu2(1, 1, 2, RodzajDokumentu.Zamowienie, true);
//            TestDostepu2(1, 2, 1, RodzajDokumentu.Zamowienie, true);
//        }

//        private void TestDostepu2(int idklient, int dokklient, int? odbiorca, RodzajDokumentu rodzaj, bool wynik)
//        {
//            var klient = A.Fake<IKlient>();
//            klient.klient_id = idklient;
//            A.CallTo(() => klient.WszystkieKontaPodrzedne()).Returns(new List<IKlient>());

//            var dd = A.Fake<IDokumenty>();
//            IKlienciDostep kliencidostep = A.Fake<IKlienciDostep>();
//            A.CallTo(() => kliencidostep.Pobierz(idklient)).Returns(klient);

//            dd.Kliencidostep = kliencidostep;

//            IDokument tmp = A.Fake<IDokument>();

//            A.CallTo(() => tmp.DokumentOdbiorcaId).Returns(odbiorca.Value);
//            A.CallTo(() => tmp.DokummentRodzaj).Returns(rodzaj);
//            A.CallTo(() => tmp.DokumentPlatnikId).Returns(dokklient);

//            var ddd = new Dokumenty();
//            ddd.Kliencidostep = kliencidostep;

//            bool czyjest = false;

//            A.CallTo(() => dd.CzyKlientMaDostep(tmp, klient,null)).Invokes((IDokument dok, int idklienta) => czyjest = ddd.CzyKlientMaDostep(dok, idklienta));

//            dd.CzyKlientMaDostep(tmp, klient);
//            Assert.True(czyjest == wynik, string.Format("klient id {0} dok {1} odb {2} rodzaj {3} wnik {4} oczekiwane {5}", idklient, dokklient, odbiorca, rodzaj, czyjest, wynik));
//        }

//        [Fact()]
//        public void WszystkieDokumentyTest()
//        {
//            ConfigBLL config = A.Fake<ConfigBLL>();
//            A.CallTo(() => config.JezykIDPolski).Returns(1);


//            IDokument dok1 = A.Fake<IDokument>();
//            A.CallTo(() => dok1.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok1.DokumentPlatnikId).Returns(1);
//            A.CallTo(() => dok1.DokumentId).Returns(1);
            
//            IDokument dok2 = A.Fake<IDokument>();
//            A.CallTo(() => dok2.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok2.DokumentPlatnikId).Returns(1);
//            A.CallTo(() => dok2.DokumentId).Returns(2);
            
//            IDokument dok3 = A.Fake<IDokument>();
//            A.CallTo(() => dok3.DokummentRodzaj).Returns(RodzajDokumentu.Zamowienie);
//            A.CallTo(() => dok3.DokumentOdbiorcaId).Returns(3);
//            A.CallTo(() => dok3.DokumentId).Returns(3);
//            A.CallTo(() => dok3.DokumentDataWystawienia).Returns(new DateTime(2012, 6, 10));

//            IDokument dok4 = A.Fake<IDokument>();
//            A.CallTo(() => dok4.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok4.DokumentPlatnikId).Returns(2);
//            A.CallTo(() => dok4.DokumentId).Returns(4);
//            A.CallTo(() => dok4.DokumentDataWystawienia).Returns(new DateTime(2013, 6, 10));

            
//            IDokument dok5 = A.Fake<IDokument>();
//            A.CallTo(() => dok5.DokummentRodzaj).Returns(RodzajDokumentu.Zamowienie);
//            A.CallTo(() => dok5.DokumentOdbiorcaId).Returns(2);
//            A.CallTo(() => dok5.DokumentId).Returns(5);
            
//            IDokument dok6 = A.Fake<IDokument>();
//            A.CallTo(() => dok6.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok6.DokumentPlatnikId).Returns(2);
//            A.CallTo(() => dok6.DokumentId).Returns(6);

//            Dictionary<int, IDokument> slownikdokumenty = new Dictionary<int, IDokument>();
//            slownikdokumenty.Add(1, dok1);
//            slownikdokumenty.Add(2, dok2);
//            slownikdokumenty.Add(3, dok3);
//            slownikdokumenty.Add(4, dok4);
//            slownikdokumenty.Add(5, dok5);
//            slownikdokumenty.Add(6, dok6);

//            IKlient klient1 = A.Fake<IKlient>();
//            A.CallTo(() => klient1.klient_id).Returns(1);
//            A.CallTo(() => klient1.WszystkieKontaPodrzedne()).Returns(new List<IKlient>());
//            IKlient klient2 = A.Fake<IKlient>();
//            A.CallTo(() => klient2.klient_id).Returns(2);
//            A.CallTo(() => klient2.WszystkieKontaPodrzedne()).Returns(new List<IKlient>());

//            KlienciDostep klienciDostep = A.Fake<KlienciDostep>();

//            A.CallTo(() => klienciDostep.Pobierz(1)).Returns(klient1);
//            A.CallTo(() => klienciDostep.Pobierz(2)).Returns(klient2);

//            Dokumenty dokumenty = A.Fake<Dokumenty>();
//            dokumenty.Kliencidostep = klienciDostep;
//            A.CallTo(() => dokumenty.WszystkieDokumenty()).Returns(slownikdokumenty);

//            ICacheBll cache = A.Fake<ICacheBll>();
//            //A.CallTo(() => cache.DodajObiekt(A<string>.Ignored, A<HashSet<int>>.Ignored));
//            dokumenty.Cache = cache;
            
//            string cache_Klucz1 = dokumenty.PobierzKluczDokumentyKlient(klient1.klient_id, RodzajDokumentu.Faktura);
//            string cache_Klucz2 = dokumenty.PobierzKluczDokumentyKlient(klient2.klient_id, RodzajDokumentu.Faktura);
//            string cache_Klucz3 = dokumenty.PobierzKluczDokumentyKlient(klient1.klient_id, RodzajDokumentu.Zamowienie);
//            string cache_Klucz4 = dokumenty.PobierzKluczDokumentyKlient(klient2.klient_id, RodzajDokumentu.Zamowienie);

         
//            A.CallTo(() => cache.PobierzObiekt<HashSet<int>>(cache_Klucz1)).Returns(null);
//            A.CallTo(() => cache.PobierzObiekt<HashSet<int>>(cache_Klucz2)).Returns(null);
//            A.CallTo(() => cache.PobierzObiekt<HashSet<int>>(cache_Klucz3)).Returns(null);
//            A.CallTo(() => cache.PobierzObiekt<HashSet<int>>(cache_Klucz4)).Returns(null);

//            A.CallTo(dokumenty)
//                .Where(
//                    a =>
//                       // a.Arguments.Get<RodzajDokumentu>(0) == RodzajDokumentu.Faktura && a.Arguments.Get<int>(1) == 1 &&
//                        a.Method.Name == "PobierzDokumenty")
//                .WithReturnType<Dictionary<int, IDokument>>().CallsBaseMethod();

//            var fakturyKlient1 = dokumenty.PobierzDokumenty(RodzajDokumentu.Faktura, 1);
//            Assert.Equal(2, fakturyKlient1.Count);

//            var fakturyKlient2 = dokumenty.PobierzDokumenty(RodzajDokumentu.Faktura, 2);
//            Assert.Equal(2, fakturyKlient2.Count);

//            var zamowieniaKlient1 = dokumenty.PobierzDokumenty(RodzajDokumentu.Zamowienie, 1);
//            Assert.Equal(0, zamowieniaKlient1.Count);

//            var zamowieniaKlient2 = dokumenty.PobierzDokumenty(RodzajDokumentu.Zamowienie, 2);
//            Assert.Equal(1, zamowieniaKlient2.Count);
//        }

//        [Fact(DisplayName = "Sprawdza poprawność danych z pól pozycji w fakturze")]
//        public void DokumentyPoprawneWartosciFaktur()
//        {
//            ConfigBLL config = A.Fake<ConfigBLL>();
//            A.CallTo(() => config.JezykIDPolski).Returns(1);


//            DokumentyBll dok = new DokumentyBll(null);
            
//            dok.waluta = null;
//            dok.wartosc_netto = 30;
//            dok.wartosc_brutto = 55;
//            Assert.Equal(30, dok.DokumentWartoscNetto);
//            Assert.Equal(55, dok.DokumentWartoscBrutto);

//            dok.waluta = "PLN";

//            Assert.Equal(30, dok.DokumentWartoscNetto);
//            Assert.Equal(55, dok.DokumentWartoscBrutto);

//            Assert.Equal("30,00 PLN", dok.DokumentWartoscNetto.ToString());
//            Assert.Equal("55,00 PLN", dok.DokumentWartoscBrutto.ToString());

//            dok.waluta = "EUR";

//            Assert.Equal(30, dok.DokumentWartoscNetto);
//            Assert.Equal(55, dok.DokumentWartoscBrutto);

//            Assert.Equal("30,00 EUR", dok.DokumentWartoscNetto.ToString());
//            Assert.Equal("55,00 EUR", dok.DokumentWartoscBrutto.ToString());
//        }

//        [Fact(DisplayName = "Sprawdza poprawność danych z pól pozycji w zamówieniu")]
//        public void DokumentyPoprawneWartosciZamowien()
//        {
//            IConfigBLL config = A.Fake<IConfigBLL>();
//            A.CallTo(() => config.JezykIDPolski).Returns(1);

//            ZamowieniaBLL zam1 = new ZamowieniaBLL(null);
//            zam1.zamowienie_id = 1;
//            zam1.klient_id = 20;

//            IRabatyBll rabaty = A.Fake<IRabatyBll>();
//            ZamowieniaDostep zamowieniadostep = A.Fake<ZamowieniaDostep>();
//            Dictionary<int, ProduktBazowy> produkty = new Dictionary<int, ProduktBazowy>();
//            IProduktyBazowe produktyBazowe = A.Fake<IProduktyBazowe>();


//#region pozycja 1
//            FlatCenyBLL flatcenypozycja1 = new FlatCenyBLL();
//            flatcenypozycja1.rabat = 10;
//            flatcenypozycja1.cena_hurtowa_netto = 8;
//            flatcenypozycja1.configBLL = config;
//            flatcenypozycja1.produktyBazowe = produktyBazowe;
//               ProduktBazowy dok1Pozycja1Bazowy = new ProduktBazowy(config.JezykIDPolski) {vat = 23};

//            ZamowieniaProduktyBLL dok1pozycja1 = A.Fake<ZamowieniaProduktyBLL>(a=> a.WithArgumentsForConstructor(new Object[] {null, zam1}));
//            dok1pozycja1.rabatyBLL = rabaty;
//            dok1pozycja1.configBLL = config;
//            dok1pozycja1.cena_netto = 10;
//            dok1pozycja1.cena_brutto = 12;
//            dok1pozycja1.ilosc = 10;
//            dok1pozycja1.produkt_id = 666;
//            dok1pozycja1.jednostka_przelicznik = 1;
//            dok1pozycja1.produktyBazowe = produktyBazowe;
//            dok1pozycja1.jednostka = "twarda sztuka";
//            A.CallTo(() => dok1pozycja1.PobierzProdukt(config.JezykIDPolski)).Returns(dok1Pozycja1Bazowy);
//            A.CallTo(() => rabaty.PobierzCeneProduktuDlaKlienta(zam1.klient_id, dok1Pozycja1Bazowy))
//                .Returns(flatcenypozycja1);
            
         
//            produkty.Add(dok1pozycja1.produkt_id, dok1Pozycja1Bazowy);
//#endregion

//            ZamowieniaProduktyBLL dok1pozycja2 = A.Fake<ZamowieniaProduktyBLL>(a => a.WithArgumentsForConstructor(new Object[] {null, zam1}));
//            dok1pozycja2.rabatyBLL = rabaty;
//            dok1pozycja2.configBLL = config;
//            dok1pozycja2.cena_netto = 300;
//            dok1pozycja2.cena_brutto = 360;
//            dok1pozycja2.ilosc = 5;
//            dok1pozycja2.produkt_id = 777;
//            dok1pozycja2.jednostka_przelicznik = 1;
//            dok1pozycja2.produktyBazowe = produktyBazowe;
//            dok1pozycja2.jednostka = "miękka sztuka";
            
//            produkty.Add(dok1pozycja2.produkt_id, new ProduktBazowy(config.JezykIDPolski));

//            A.CallTo(() => produktyBazowe.WszystkieSlownik(config.JezykIDPolski)).Returns(produkty);
//            A.CallTo(() => produktyBazowe.Pobierz(dok1pozycja1.produkt_id, config.JezykIDPolski))
//                .Returns(dok1Pozycja1Bazowy);

//            IEnumerable<IDokumentPozycja> dok1pozycje = new List<IDokumentPozycja>() { dok1pozycja1, dok1pozycja2 };

            

//            A.CallTo(() => zamowieniadostep.PobierzPozycje(zam1.zamowienie_id)).Returns(dok1pozycje);

//            zam1.Zamowieniadostep = zamowieniadostep;

//            var pobranePozycje = zamowieniadostep.PobierzPozycje(zam1.zamowienie_id).ToList();

//            Assert.Equal(dok1pozycja1.produkt_id, pobranePozycje[0].PozycjaDokumentuIdProduktu);
//            Assert.Equal(dok1pozycja1.jednostka, pobranePozycje[0].PozycjaDokumentuJednostka);
//            //Assert.Equal(dok1pozycja1.jednostka, pobranePozycje[0].PozycjaDokumentuWaluta);
//            Assert.Equal(new WartoscLiczbowaZaokraglana(dok1pozycja1.ilosc), pobranePozycje[0].PozycjaDokumentuIlosc);
//            Assert.Equal(new WartoscLiczbowa(dok1pozycja1.cena_netto), pobranePozycje[0].PozycjaDokumentuCenaNetto);
//            Assert.Equal(new WartoscLiczbowa(dok1pozycja1.cena_brutto), pobranePozycje[0].PozycjaDokumentuCenaBrutto);
//            Assert.Equal(new WartoscLiczbowa(flatcenypozycja1.cena_hurtowa_netto), pobranePozycje[0].PozycjaDokumentuCenaNettoBezRabatu);
//            Assert.Equal(new WartoscLiczbowa(flatcenypozycja1.cena_hurtowa_brutto), pobranePozycje[0].PozycjaDokumentuCenaBruttoBezRabatu);
//            Assert.Equal(new WartoscLiczbowa(dok1pozycja1.cena_netto * dok1pozycja1.ilosc), pobranePozycje[0].PozycjaDokumentuWartoscNetto);
//            Assert.Equal(new WartoscLiczbowa(dok1pozycja1.cena_brutto * dok1pozycja1.ilosc), pobranePozycje[0].PozycjaDokumentuWartoscBrutto);
//            Assert.Equal(new WartoscLiczbowaZaokraglana(flatcenypozycja1.rabat), pobranePozycje[0].PozycjaDokumentuRabat);
//            Assert.Equal(new WartoscLiczbowa((dok1pozycja1.cena_brutto * dok1pozycja1.ilosc) - (dok1pozycja1.cena_netto * dok1pozycja1.ilosc)), pobranePozycje[0].PozycjaDokumentuWartoscVat);
//            Assert.Equal(new WartoscLiczbowa(dok1Pozycja1Bazowy.vat), pobranePozycje[0].PozycjaDokumentuStawkaVat);
//            Assert.Equal(dok1pozycja1.cena_netto, pobranePozycje[0].PozycjaDokumentuCenaNettoBezWaluty);
//            Assert.Equal(dok1pozycja1.cena_brutto, pobranePozycje[0].PozycjaDokumentuCenaBruttoBezWaluty);

//            //w zamówieniach opisy pozycji mają zwrócić null. stringa zwracaja tylko dla faktur
//            Assert.Null(pobranePozycje[0].OpisPozycji1);
//            Assert.Null(pobranePozycje[0].OpisPozycji2);

            

//            Assert.Equal(1600, zam1.DokumentWartoscNetto);
//            Assert.Equal(1920, zam1.DokumentWartoscBrutto);

//            zam1.waluta = "PLN";

//            Assert.Equal(1600, zam1.DokumentWartoscNetto);
//            Assert.Equal(1920, zam1.DokumentWartoscBrutto);

//            Assert.Equal("1 600,00 PLN", zam1.DokumentWartoscNetto.ToString());
//            Assert.Equal("1 920,00 PLN", zam1.DokumentWartoscBrutto.ToString());

//            zam1.waluta = "EUR";

//            Assert.Equal(1600, zam1.DokumentWartoscNetto);
//            Assert.Equal(1920, zam1.DokumentWartoscBrutto);

//            Assert.Equal("1 600,00 EUR", zam1.DokumentWartoscNetto.ToString());
//            Assert.Equal("1 920,00 EUR", zam1.DokumentWartoscBrutto.ToString());
//        }

//        [Fact(DisplayName = "Pobieranie wszystkich dokumentów klienta test wydajnościowy")]
//        public void PobieranieDokumentowKlientaTestWydajnosciowy()
//        {
//            ConfigBLL config = A.Fake<ConfigBLL>();
//            A.CallTo(() => config.JezykIDPolski).Returns(1);

//            IKlient klient1 = A.Fake<IKlient>();
//            A.CallTo(() => klient1.klient_id).Returns(10);
//            A.CallTo(() => klient1.WszystkieKontaPodrzedne()).Returns(new List<IKlient>());
//            IKlient klient2 = A.Fake<IKlient>();
//            A.CallTo(() => klient2.klient_id).Returns(2);
//            A.CallTo(() => klient2.WszystkieKontaPodrzedne()).Returns(new List<IKlient>());

//            KlienciDostep klienciDostep = A.Fake<KlienciDostep>();

//            A.CallTo(() => klienciDostep.Pobierz(10)).Returns(klient1);
//            A.CallTo(() => klienciDostep.Pobierz(2)).Returns(klient2);

//            var slownikdokumenty = WygenerujDokumentyTylkoInterfejs(40000);

//            Dokumenty dokumenty = A.Fake<Dokumenty>();
//            dokumenty.Kliencidostep = klienciDostep;
//            ICacheBll cache = A.Fake<ICacheBll>();
//            dokumenty.Cache = cache;
//            A.CallTo(() => cache.PobierzObiekt<HashSet<int>>(A<string>.Ignored)).Returns(null);
//            A.CallTo(() => dokumenty.WszystkieDokumenty()).Returns(slownikdokumenty);

//            Stopwatch stoper = Stopwatch.StartNew();
//            var fakturyKlient1 = dokumenty.PobierzDokumentyGlowna(RodzajDokumentu.Faktura, 10);
//            stoper.Stop();

//            Assert.True(stoper.Elapsed.TotalSeconds < 1, string.Format("za długi czas sprawdzania dokumentów klienta: {0} sekund zamiast poniżej 1 sekundy", stoper.Elapsed.TotalSeconds));
//        }

//        private Dictionary<int, IDokument> WygenerujDokumenty(int ile)
//        {
//             Dictionary<int,IDokument> dokumenty = new  Dictionary<int,IDokument>();

//            for (int i = 0; i < ile; i++)
//            {
//                DokumentyBll dokument = new DokumentyBll(null);
//                dokument.id = i;
//                dokument.klient_id = i % 2 == 0 ? 10 : i + 1;
//                dokument.Rodzaj = i%10 == 0 ? RodzajDokumentu.Zamowienie : RodzajDokumentu.Faktura;
//                dokument.OdbiorcaId = i%2 == 0 ? i : i + 1;
//                dokumenty.Add(i, dokument);
//            }

//            return dokumenty;
//        }

//        private Dictionary<int, IDokument> WygenerujDokumentyTylkoInterfejs(int ile)
//        {
//            Dictionary<int, IDokument> dokumenty = new Dictionary<int, IDokument>();

//            for (int i = 0; i < ile; i++)
//            {
//                IDokument dokument = A.Fake<IDokument>();
//                A.CallTo(() => dokument.DokumentId).Returns(i);
//                A.CallTo(() => dokument.DokumentOdbiorcaId).Returns(i % 2 == 0 ? 10 : i + 1);
//                A.CallTo(() => dokument.DokummentRodzaj).Returns(i % 10 == 0 ? RodzajDokumentu.Zamowienie : RodzajDokumentu.Faktura);
//                dokumenty.Add(i, dokument);
//            }

//            return dokumenty;
//        }

//        [Fact(DisplayName = "Łączenie faktur i zamówień w słownik dokumentów")]
//        public void WszystkieDokumentyLaczenieDokumentow()
//        {
//            Dokumenty dokumenty = A.Fake<Dokumenty>();

//            //czasem nie zmieniać tego na interfejs
//            ZamowieniaDostep zamowienia = A.Fake<ZamowieniaDostep>();
//            dokumenty.Zamowieniadostep = zamowienia;
//            ICacheBll cache = A.Fake<ICacheBll>();
//            dokumenty.Cache = cache;
//            A.CallTo(() => cache.PobierzObiekt<Dictionary<int, IDokument>>(A<string>.Ignored)).Returns(null);

//            var wygenerowane = WygenerujDokumenty(10);

//            ZamowieniaBLL zam1 = new ZamowieniaBLL(null);
//            zam1.IdDokumentu = "2";
//            zam1.zamowienie_id = 10;

//            ZamowieniaBLL zam2 = new ZamowieniaBLL(null);
//            zam2.IdDokumentu = "-5000";
//            zam2.zamowienie_id = 1000;

//            ZamowieniaBLL zam3 = new ZamowieniaBLL(null);
//            zam3.IdDokumentu = "600";
//            zam3.zamowienie_id = 2000;

//            Dictionary<int, ZamowieniaBLL> wszystkiezamowienia = new Dictionary<int, ZamowieniaBLL>();
//            wszystkiezamowienia.Add(zam1.zamowienie_id, zam1);
//            wszystkiezamowienia.Add(zam2.zamowienie_id, zam2);
//            wszystkiezamowienia.Add(zam3.zamowienie_id, zam3);

//            A.CallTo(() => zamowienia.PobierzZamowienia(null)).Returns(wszystkiezamowienia);
//            A.CallTo(() => dokumenty.PobierzBazowe(A<DokumentySearchCriteria>.Ignored)).Returns(wygenerowane.Values.Select(a=> a as historia_dokumenty).ToList());

//            Assert.Equal(10, wygenerowane.Count);

//            var wszystkie = dokumenty.WszystkieDokumenty();

//            Assert.Equal(12, wszystkie.Count); 

//            Assert.True(wszystkie.Any(a=> a.Key.ToString() == zam2.IdDokumentu));
//            Assert.True(wszystkie.Any(a => a.Key.ToString() == zam3.IdDokumentu));
//        }

//        [Fact(DisplayName = "Pobieranie zamówienia o określonym ID")]
//        public void PobierzTest()
//        {
//            Dokumenty dokumenty = A.Fake<Dokumenty>();
//            Dictionary<int,IDokument> dokumentykolekcha=new Dictionary<int, IDokument>();


//            var zam1 = A.Fake<IDokument>();
//            A.CallTo(() => zam1.DokumentId).Returns(2);
//            A.CallTo(() => zam1.DokummentRodzaj).Returns(RodzajDokumentu.Zamowienie);
//            dokumentykolekcha.Add(zam1.DokumentId, zam1);


//            var zam2 = A.Fake<ZamowieniaBLL>();
//            A.CallTo(() => zam2.DokummentRodzaj).Returns(RodzajDokumentu.Zamowienie);
//            zam2.IdDokumentu = "-1";
//            zam2.zamowienie_id = -10;
//            dokumentykolekcha.Add(zam2.DokumentId, zam2);


//            var dok = A.Fake<IDokument>();
//            A.CallTo(() => dok.DokumentId).Returns(-8);
//            A.CallTo(() => dok.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            dokumentykolekcha.Add(dok.DokumentId, dok);

//            A.CallTo(() => dokumenty.WszystkieDokumenty()).Returns(dokumentykolekcha);

//            //var w1 = dokumenty.PobierzDokument(2);
//            //Assert.True(w1!=null);
//            //Assert.True(w1.DokumentId == zam1.DokumentId);

//            //var w2 = dokumenty.PobierzDokument(-10);
//            //Assert.True(w2 != null);
//            //Assert.True(w2.DokumentId.ToString() == zam2.IdDokumentu);
//            //var w3 = dokumenty.PobierzDokument(-5);
//            //Assert.True(w3== null);

//        }

//        [Fact(DisplayName = "Dokumenty - test przeterminowanych faktur klienta")]
//        public void CzyKlientPosiadaJakiesPrzeterminowaneFakturyTest()
//        {
//            Dokumenty dok = A.Fake<Dokumenty>();

//            ConfigBLL config = A.Fake<ConfigBLL>();
//            A.CallTo(() => config.JezykIDPolski).Returns(1);


//            IDokument dok1 = A.Fake<IDokument>();
//            A.CallTo(() => dok1.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok1.DokumentPlatnikId).Returns(1);
//            A.CallTo(() => dok1.DokumentId).Returns(1);
//            A.CallTo(() => dok1.DokumentZaplacony).Returns(false);
//            A.CallTo(() => dok1.DokumentDniSpoznienia).Returns(0);

//            IDokument dok2 = A.Fake<IDokument>();
//            A.CallTo(() => dok2.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok2.DokumentPlatnikId).Returns(1);
//            A.CallTo(() => dok2.DokumentZaplacony).Returns(false);
//            A.CallTo(() => dok2.DokumentDniSpoznienia).Returns(1);
//            A.CallTo(() => dok2.DokumentId).Returns(2);
//            A.CallTo(() => dok2.DokumentWartoscNetto).Returns(2);

//            IDokument dok3 = A.Fake<IDokument>();
//            A.CallTo(() => dok3.DokummentRodzaj).Returns(RodzajDokumentu.Zamowienie);
//            A.CallTo(() => dok3.DokumentOdbiorcaId).Returns(3);
//            A.CallTo(() => dok3.DokumentId).Returns(3);
//            A.CallTo(() => dok3.DokumentDataWystawienia).Returns(new DateTime(2012, 6, 10));

//            IDokument dok4 = A.Fake<IDokument>();
//            A.CallTo(() => dok4.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok4.DokumentPlatnikId).Returns(2);
//            A.CallTo(() => dok4.DokumentId).Returns(4);
//            A.CallTo(() => dok4.DokumentDataWystawienia).Returns(new DateTime(2013, 6, 10));


//            IDokument dok5 = A.Fake<IDokument>();
//            A.CallTo(() => dok5.DokummentRodzaj).Returns(RodzajDokumentu.Zamowienie);
//            A.CallTo(() => dok5.DokumentOdbiorcaId).Returns(2);
//            A.CallTo(() => dok5.DokumentId).Returns(5);

//            IDokument dok6 = A.Fake<IDokument>();
//            A.CallTo(() => dok6.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok6.DokumentPlatnikId).Returns(2);
//            A.CallTo(() => dok6.DokumentId).Returns(6);

//            Dictionary<int, IDokument> slownikdokumenty = new Dictionary<int, IDokument>();
//            slownikdokumenty.Add(1, dok1);
//            slownikdokumenty.Add(2, dok2);
//            slownikdokumenty.Add(3, dok3);
//            slownikdokumenty.Add(4, dok4);
//            slownikdokumenty.Add(5, dok5);
//            slownikdokumenty.Add(6, dok6);

//            IKlient klient1 = A.Fake<IKlient>();
//            A.CallTo(() => klient1.klient_id).Returns(1);
//            A.CallTo(() => klient1.WszystkieKontaPodrzedne()).Returns(new List<IKlient>());
//            IKlient klient2 = A.Fake<IKlient>();
//            A.CallTo(() => klient2.klient_id).Returns(2);
//            A.CallTo(() => klient2.WszystkieKontaPodrzedne()).Returns(new List<IKlient>());

//            KlienciDostep klienciDostep = A.Fake<KlienciDostep>();

//            A.CallTo(() => klienciDostep.Pobierz(1)).Returns(klient1);
//            A.CallTo(() => klienciDostep.Pobierz(2)).Returns(klient2);

//            Dokumenty dokumenty = A.Fake<Dokumenty>();
//            dokumenty.Kliencidostep = klienciDostep;
//            A.CallTo(() => dokumenty.WszystkieDokumenty()).Returns(slownikdokumenty);


//            string cache_Klucz1 = dokumenty.PobierzKluczDokumentyKlient(klient1.klient_id, RodzajDokumentu.Faktura);
//            string cache_Klucz2 = dokumenty.PobierzKluczDokumentyKlient(klient2.klient_id, RodzajDokumentu.Faktura);
//            string cache_Klucz3 = dokumenty.PobierzKluczDokumentyKlient(klient1.klient_id, RodzajDokumentu.Zamowienie);
//            string cache_Klucz4 = dokumenty.PobierzKluczDokumentyKlient(klient2.klient_id, RodzajDokumentu.Zamowienie);

//            ICacheBll cache = A.Fake<ICacheBll>();
//            //A.CallTo(() => cache.DodajObiekt(A<string>.Ignored, A<HashSet<int>>.Ignored));
//            dokumenty.Cache = cache;

//            A.CallTo(() => cache.PobierzObiekt<HashSet<int>>(cache_Klucz1)).Returns(null);
//            A.CallTo(() => cache.PobierzObiekt<HashSet<int>>(cache_Klucz2)).Returns(null);
//            A.CallTo(() => cache.PobierzObiekt<HashSet<int>>(cache_Klucz3)).Returns(null);
//            A.CallTo(() => cache.PobierzObiekt<HashSet<int>>(cache_Klucz4)).Returns(null);

//            var pobraneDokumenty =slownikdokumenty.WhereKeyIsIn( dokumenty.PobierzDokumentyGlowna(RodzajDokumentu.Faktura, 1)).ToDictionary(x=>x.DokumentId,x=>x);

//            A.CallTo(() => dokumenty.PobierzDokumenty(RodzajDokumentu.Faktura, 1)).Returns(pobraneDokumenty);

//            var fakturyKlient1 = dokumenty.CzyKlientPosiadaJakiesPrzeterminowaneFaktury(1);
//            Assert.True(fakturyKlient1);
//        }

//        [Fact(DisplayName = "Podsumowanie dokumentów dla wybranego klienta")]
//        public void PobierzPodsumowanieKlientaTest()
//        {
//            ConfigBLL config = A.Fake<ConfigBLL>();
//            A.CallTo(() => config.JezykIDPolski).Returns(1);


//            var slownikdokumenty = WygenerujSlownikDokumentow();
//            IKlient klient1 = A.Fake<IKlient>();
//            A.CallTo(() => klient1.klient_id).Returns(1);
//            A.CallTo(() => klient1.WszystkieKontaPodrzedne()).Returns(new List<IKlient>());
//            IKlient klient2 = A.Fake<IKlient>();
//            A.CallTo(() => klient2.klient_id).Returns(2);
//            A.CallTo(() => klient2.WszystkieKontaPodrzedne()).Returns(new List<IKlient>());

//            KlienciDostep klienciDostep = A.Fake<KlienciDostep>();

//            A.CallTo(() => klienciDostep.Pobierz(1)).Returns(klient1);
//            A.CallTo(() => klienciDostep.Pobierz(2)).Returns(klient2);

//            Dokumenty dokumenty = A.Fake<Dokumenty>();
//            dokumenty.Kliencidostep = klienciDostep;
//            A.CallTo(() => dokumenty.WszystkieDokumenty()).Returns(slownikdokumenty);
//            A.CallTo(() => dokumenty.PobierzDokumenty(RodzajDokumentu.Faktura, klient1.klient_id)).Returns(slownikdokumenty);

//            DocumentSummary podsumowanieKlienta = dokumenty.WygenerujeDaneDoWykresuFaktur(slownikdokumenty.Values.Where(x=>x.DokumentPlatnikId==klient1.klient_id).ToList());

//            Assert.Equal(2, podsumowanieKlienta.Niezaplacone.IloscPozycji);
//            Assert.Equal(1, podsumowanieKlienta.Przeterminowane.IloscPozycji);
//            Assert.Equal(69, podsumowanieKlienta.Przeterminowane.Cena.Wartosc);
//            Assert.Equal(0, podsumowanieKlienta.Niezaplacone.Cena.Wartosc);
//        }

//        private static Dictionary<int, IDokument> WygenerujSlownikDokumentow()
//        {
//            IDokument dok1 = A.Fake<IDokument>();
//            A.CallTo(() => dok1.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok1.DokumentPlatnikId).Returns(1);
//            A.CallTo(() => dok1.DokumentId).Returns(1);
//            A.CallTo(() => dok1.DokumentZaplacony).Returns(false);
//            A.CallTo(() => dok1.DokumentDniSpoznienia).Returns(0);
//            A.CallTo(() => dok1.DokumentWartoscBrutto).Returns(30);
//            A.CallTo(() => dok1.DokumentWartoscNalezna).Returns(10);
//            IDokument dok2 = A.Fake<IDokument>();
//            A.CallTo(() => dok2.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok2.DokumentPlatnikId).Returns(1);
//            A.CallTo(() => dok2.DokumentZaplacony).Returns(false);
//            A.CallTo(() => dok2.DokumentDniSpoznienia).Returns(1);
//            A.CallTo(() => dok2.DokumentWartoscNalezna).Returns(69);
//            A.CallTo(() => dok2.DokumentWartoscBrutto).Returns(20);
//            A.CallTo(() => dok2.DokumentId).Returns(2);
//            IDokument dok7 = A.Fake<IDokument>();
//            A.CallTo(() => dok7.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok7.DokumentPlatnikId).Returns(1);
//            A.CallTo(() => dok7.DokumentZaplacony).Returns(false);
//            A.CallTo(() => dok7.DokumentId).Returns(7);
//            A.CallTo(() => dok7.DokumentWartoscBrutto).Returns(-10);
//            A.CallTo(() => dok7.DokumentWartoscNalezna).Returns(-10);
//            A.CallTo(() => dok7.DokumentDniSpoznienia).Returns(0);
//            IDokument dok3 = A.Fake<IDokument>();
//            A.CallTo(() => dok3.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok3.DokumentOdbiorcaId).Returns(3);
//            A.CallTo(() => dok3.DokumentZaplacony).Returns(true);
//            A.CallTo(() => dok3.DokumentId).Returns(3);
//            A.CallTo(() => dok3.DokumentDataWystawienia).Returns(new DateTime(2012, 6, 10));
//            A.CallTo(() => dok3.DokumentWartoscBrutto).Returns(30);
//            IDokument dok4 = A.Fake<IDokument>();
//            A.CallTo(() => dok4.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok4.DokumentPlatnikId).Returns(2);
//            A.CallTo(() => dok4.DokumentZaplacony).Returns(true);
//            A.CallTo(() => dok4.DokumentId).Returns(4);
//            A.CallTo(() => dok4.DokumentDataWystawienia).Returns(new DateTime(2013, 6, 10));
//            A.CallTo(() => dok4.DokumentWartoscBrutto).Returns(40);
//            IDokument dok5 = A.Fake<IDokument>();
//            A.CallTo(() => dok5.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok5.DokumentOdbiorcaId).Returns(2);
//            A.CallTo(() => dok5.DokumentZaplacony).Returns(true);
//            A.CallTo(() => dok5.DokumentId).Returns(5);
//            A.CallTo(() => dok5.DokumentWartoscBrutto).Returns(50);
//            IDokument dok6 = A.Fake<IDokument>();
//            A.CallTo(() => dok6.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
//            A.CallTo(() => dok6.DokumentPlatnikId).Returns(2);
//            A.CallTo(() => dok6.DokumentZaplacony).Returns(true);
//            A.CallTo(() => dok6.DokumentId).Returns(6);
//            A.CallTo(() => dok6.DokumentWartoscBrutto).Returns(60);

//            Dictionary<int, IDokument> slownikdokumenty = new Dictionary<int, IDokument>();

//            slownikdokumenty.Add(1, dok1);
//            slownikdokumenty.Add(2, dok2);
//            slownikdokumenty.Add(3, dok3);
//            slownikdokumenty.Add(4, dok4);
//            slownikdokumenty.Add(5, dok5);
//            slownikdokumenty.Add(6, dok6);
//            slownikdokumenty.Add(7, dok7);
//            return slownikdokumenty;
//        }

//        [Fact(DisplayName = "Pobieranie dokumentu o wybranym ID dla klienta o wybranym ID")]
//        public void PobierzTestPobieranieDokumentuKlienta()
//        {
//            var slownikdokumentow = WygenerujSlownikDokumentow();

//            Dokumenty dokumenty = A.Fake<Dokumenty>();

//            A.CallTo(() => dokumenty.Pobierz(slownikdokumentow[1].DokumentId)).Returns(slownikdokumentow[1]);

//            A.CallTo(() => dokumenty.Pobierz(slownikdokumentow[2].DokumentId)).Returns(null);

//            A.CallTo(() => dokumenty.Pobierz(slownikdokumentow[3].DokumentId)).Returns(slownikdokumentow[3]);

     
//            A.CallTo(() => dokumenty.CzyKlientMaDostep(slownikdokumentow[1], slownikdokumentow[1].DokumentPlatnikId)).Returns(true);
//            A.CallTo(() => dokumenty.CzyKlientMaDostep(slownikdokumentow[3], slownikdokumentow[3].DokumentPlatnikId)).Returns(false);

//            IDokument dokument = dokumenty.Pobierz(slownikdokumentow[1].DokumentId,
//                slownikdokumentow[1].DokumentPlatnikId);

//            Assert.Equal(slownikdokumentow[1], dokument);

//            IDokument dokument2 = dokumenty.Pobierz(slownikdokumentow[2].DokumentId,
//                slownikdokumentow[2].DokumentPlatnikId);

//            Assert.Equal(null, dokument2);


//            try
//            {
//                IDokument dokument3 = dokumenty.Pobierz(slownikdokumentow[3].DokumentId,
//                    slownikdokumentow[3].DokumentPlatnikId);

//                Assert.True(false, "Powinno wyrzucić wyjątek z brakiem dostępu do dokumentu a tego nie zrobiło");
//            }
//            catch(Exception ex)
//            {
//                Assert.IsType(typeof(SecurityAccessDeniedException), ex);
//            }
            
//        }

      
//        [Fact(DisplayName = "Sprawdza czy bierzący klient posiada dokument faktura ")]
//        public void PosiadaDokumentyTest()
//        {
//            Klient k1 = new Klient(null){klient_id = 1};
//            Klient k2 = new Klient(null) { klient_id = 2 };
//            Klient k3 = new Klient(null) { klient_id = 3 };
             

//            IDokument dok1 = A.Fake<IDokument>();
//            A.CallTo(() => dok1.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);

//            IDokument dok2 = A.Fake<IDokument>();
//            A.CallTo(() => dok2.DokummentRodzaj).Returns(RodzajDokumentu.Zamowienie);


//            Dictionary<int, IDokument> slownikdokumenty = new Dictionary<int, IDokument>();
//            slownikdokumenty.Add(1,dok1);

//            Dictionary<int, IDokument> slownikdokumenty2 = new Dictionary<int, IDokument>();
//            slownikdokumenty2.Add(1, dok2);

//            Dokumenty dokumenty = A.Fake<Dokumenty>();
//            A.CallTo(()=>dokumenty.PobierzDokumenty(RodzajDokumentu.Faktura,k1.klient_id)).Returns(slownikdokumenty);
//            A.CallTo(() => dokumenty.PobierzDokumenty(RodzajDokumentu.Faktura, k2.klient_id)).Returns(new Dictionary<int, IDokument>());
//            A.CallTo(() => dokumenty.PobierzDokumenty(RodzajDokumentu.Zamowienie, k3.klient_id)).Returns(slownikdokumenty2);

//            bool wynik = dokumenty.PosiadaDokumenty(k1);
//            Assert.True(wynik);
//            wynik = dokumenty.PosiadaDokumenty(k2);
//            Assert.False(wynik);
//            wynik = dokumenty.PosiadaDokumenty(k3);
//            Assert.True(wynik, string.Format("Oczekiwano: true, Otrzymano:{0}",wynik));
//        }

//        [Fact(DisplayName = "Zwraca id dokumentu dla powiązanej nazwy")]
//        public void PobierzIdPowiazanegoPoNazwieTest()
//        {
//            Dictionary<int, IDokument> slownik = new Dictionary<int, IDokument>();
//            IDokument dok = A.Fake<IDokument>();

//            A.CallTo(() => dok.DokumentPowiazanyNazwa).Returns("test");
//            A.CallTo(() => dok.DokumentId).Returns(1);
//            slownik.Add(1, dok);


//            Dokumenty dokumenty = A.Fake<Dokumenty>();
//            A.CallTo(() => dokumenty.WszystkieDokumenty()).Returns(slownik);
            
//            int? wynik = dokumenty.PobierzIdPowiazanegoPoNazwie(slownik[1].DokumentPowiazanyNazwa);
//            Assert.True(wynik == slownik[1].DokumentId, string.Format("Oczekiwano: 0, trzymano {0}", wynik));

//            wynik = dokumenty.PobierzIdPowiazanegoPoNazwie("aaa");
//            Assert.Null(wynik);

//        }

//        [Fact(DisplayName = "Test funkcji zwracającej link do dokumentu dla oreslonego jezyka")]
//        public void PobierzLinkTest()
//        {
//            string nazwa = "aaa";

//            IDokument dok = A.Fake<IDokument>();
//            A.CallTo(() => dok.DokumentNazwa).Returns(nazwa);
//            A.CallTo(() => dok.DokumentId).Returns(1);

//            var texthelper = A.Fake<TextHelper>();
//            A.CallTo(() => texthelper.OczyscNazwePliku(nazwa)).Returns(nazwa);


//            SesjaHelper sh = A.Fake<SesjaHelper>();
//            A.CallTo(() => sh.JezykID).Returns(3);

//            jezyki jezykpl = new jezyki();
//            jezykpl.id = 3;
//            jezykpl.symbol = "pl";
//            Dictionary<int, jezyki> JezykiWSystemie = new Dictionary<int, jezyki>(1);
//            JezykiWSystemie.Add(jezykpl.id, jezykpl);
            
//            var config = A.Fake<IConfigBLL>();
//            A.CallTo(() => config.JezykiWSystemie).Returns(JezykiWSystemie);

//            Dokumenty dokumenty = new Dokumenty();
//            dokumenty.ConfigBll = config;
//            dokumenty.TextHelper = texthelper;
//            dokumenty.SesjaHelper = sh;
//            string oczekiwana = string.Format("/{0},d{1},{2}", nazwa, dok.DokumentId,"pl" );
//            string wynik = dokumenty.PobierzLink(dok);
//            Assert.Equal(oczekiwana, wynik);

//        }




//        [Fact(DisplayName = "Obliczania sumy dla dokumentow ")]

//        public void PobierzSumyDokumentowTest()
//        {
//            IDokument dokument = A.Fake<IDokument>();
//            A.CallTo(() => dokument.LiczycHash).Returns(true);
//            A.CallTo(() => dokument.DokumentId).Returns(1);
//            IDokument dokument2 = A.Fake<IDokument>();
//            A.CallTo(() => dokument2.LiczycHash).Returns(false);
//            A.CallTo(() => dokument2.DokumentId).Returns(2);
//            IDokument dokument3 = A.Fake<IDokument>();
//            A.CallTo(() => dokument3.LiczycHash).Returns(true);
//            A.CallTo(() => dokument3.DokumentId).Returns(3);
//            IDokument dokument4 = A.Fake<IDokument>();
//            A.CallTo(() => dokument4.LiczycHash).Returns(false);
//            A.CallTo(() => dokument4.DokumentId).Returns(4);

//            Dictionary<int, IDokument> slownik = new Dictionary<int, IDokument>();
//            slownik.Add(1, dokument);
//            slownik.Add(2, dokument2);
//            slownik.Add(3, dokument3);
//            slownik.Add(4, dokument4);

//            Dokumenty doc = A.Fake<Dokumenty>();
//            A.CallTo(() => doc.WszystkieDokumenty()).Returns(slownik);

            
//            Dictionary<int, StatusDokumentu> slowStat = doc.PobierzSumyDokumentow();
//            Assert.Equal(slownik[1].DokumentId, slowStat[1].Id);
//            Assert.Equal(slownik[3].DokumentId, slowStat[3].Id);
//            Assert.DoesNotContain(2, slowStat.Keys);
//            Assert.DoesNotContain(4, slowStat.Keys);
            

//        }

//        //[Fact(DisplayName = "")]
//        //public void PobierzPozycjeDokumentuTest()
//        //{
//        //    int idd = 1;
//        //    List<DokumentyPozycje> listaDokumentyPozycja = new List<DokumentyPozycje>();

//        //    ICacheBll cache = A.Fake<ICacheBll>();
//        //    A.CallTo(() => cache.PobierzObiekt<HashSet<int>>(A<string>.Ignored)).Returns(null);

//        //    DokumentyPozycjeSearchCriteria criteria = new DokumentyPozycjeSearchCriteria();
//        //    criteria.dokument_id.Add(idd);

//        //    List<historia_dokumenty_produkty> listaHistoriaDokumentyProdukty = new List<historia_dokumenty_produkty>();

//        //    historia_dokumenty_produkty hdp = new historia_dokumenty_produkty() { dokument_id = idd };
//        //    historia_dokumenty_produkty hdp2 = new historia_dokumenty_produkty() { dokument_id = idd };

//        //    listaHistoriaDokumentyProdukty.Add(hdp);
//        //    listaHistoriaDokumentyProdukty.Add(hdp2);



//        //    Dokumenty doc = A.Fake<Dokumenty>();
//        //    doc.Cache = cache;
            
//        //}

//        [Fact(DisplayName = "Test sprawdzajacy poprawność pobierania filtrowanych dokumentów")]
//        public void PobierzWyfiltrowaneDokumentyTest()
//        {
//            int id = 1;
//            DateTime odKiedy = DateTime.Now.AddHours(-3);
//            DateTime doKiedy = DateTime.Now.AddHours(3);
//            RodzajDokumentu rodzaj = RodzajDokumentu.Zamowienie;
//            KolejnoscSortowania kierunek = KolejnoscSortowania.desc;
//            string szukanaFraza = string.Empty;

//            IKlient klient = A.Fake<IKlient>();
//            A.CallTo(() => klient.klient_id).Returns(id);

//            HashSet<IKlient> kontaPowiazane = new HashSet<IKlient>();
//            kontaPowiazane.Add(klient);
//            A.CallTo(() => klient.WszystkieKontaPowiazane()).Returns(kontaPowiazane);

//            Dictionary<int,IDokument> slownikDokumentow = new Dictionary<int, IDokument>();
//            IDokument dokument = A.Fake<IDokument>();
//            A.CallTo(() => dokument.DokummentRodzaj).Returns(rodzaj);
//            A.CallTo(() => dokument.DokumentDataWystawienia).Returns(DateTime.Now);
//            A.CallTo(() => dokument.DokumentId).Returns(1);
//            A.CallTo(() => dokument.DokumentZaplacony).Returns(false);
//            A.CallTo(() => dokument.DokumentDniSpoznienia).Returns(8);
//            slownikDokumentow.Add(dokument.DokumentId,dokument);

//            IDokument dokument2 = A.Fake<IDokument>();
//            A.CallTo(() => dokument2.DokummentRodzaj).Returns(rodzaj);
//            A.CallTo(() => dokument2.DokumentDataWystawienia).Returns(DateTime.Now);
//            A.CallTo(() => dokument2.DokumentId).Returns(2);
//            A.CallTo(() => dokument2.DokumentZaplacony).Returns(true);
//            A.CallTo(() => dokument2.DokumentDniSpoznienia).Returns(8);
//            slownikDokumentow.Add(dokument2.DokumentId, dokument2);

//            IDokument dokument3 = A.Fake<IDokument>();
//            A.CallTo(() => dokument3.DokummentRodzaj).Returns(rodzaj);
//            A.CallTo(() => dokument3.DokumentDataWystawienia).Returns(DateTime.Now);
//            A.CallTo(() => dokument3.DokumentId).Returns(3);
//            A.CallTo(() => dokument3.DokumentZaplacony).Returns(false);
//            A.CallTo(() => dokument3.DokumentDniSpoznienia).Returns(0);
//            slownikDokumentow.Add(dokument3.DokumentId, dokument3);

//            Dokumenty dok = A.Fake<Dokumenty>();
//            A.CallTo(() => dok.PobierzDokumenty(rodzaj, id)).Returns(slownikDokumentow);

//            List<string> listaPol = new List<string>();
//            listaPol.Add("DokumentNazwa");

//            var config = A.Fake<IConfigBLL>();
//            A.CallTo(() => config.DokumentyWyszukiwanie).Returns(listaPol);
//            dok.ConfigBll = config;

//            List<IDokument> wynik = dok.PobierzWyfiltrowaneDokumenty(klient, klient.klient_id, rodzaj, odKiedy, doKiedy, true, true, null, kierunek, szukanaFraza);
//            Assert.True(wynik.Count()==1);
//            List<IDokument> wynik2 = dok.PobierzWyfiltrowaneDokumenty(klient, klient.klient_id, rodzaj, odKiedy, doKiedy, false, true, null, kierunek, szukanaFraza);
//            Assert.True(wynik2.Count==2);
//            List<IDokument> wynik3 = dok.PobierzWyfiltrowaneDokumenty(klient, klient.klient_id, rodzaj, odKiedy, doKiedy, false, false, null, kierunek, szukanaFraza);
//            Assert.True(wynik3.Count() == 3);


//        } 
//    }
//}
