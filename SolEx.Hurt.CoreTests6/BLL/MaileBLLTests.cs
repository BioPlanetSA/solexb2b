//using System;
//using System.Collections.Generic;
//using FakeItEasy;
//using SolEx.Hurt.Core.BLL.Interfejsy;
//using SolEx.Hurt.Core.ModelBLL.Interfejsy;
//using SolEx.Hurt.Model;
//using SolEx.Hurt.Model.Interfaces;
//using Xunit;
//namespace SolEx.Hurt.Core.BLL.Tests
//{
//    public class MaileBLLTests
//    {
//        [Fact(DisplayName = "Pobieranie klientów do wysyłania newslettera, nic nie ma w wyslanych, dwóch do wysłania")]
//        public void PobierzListeKlientowDoWysylkiTest()
//        {
//            IKlient k1 = A.Fake<IKlient>();
//            k1.klient_id = 1;
//            k1.email = "mail1";
//            IKlient k2 = A.Fake<IKlient>();
//            k2.klient_id = 2;
//            k2.email = "mail2";

//            k1.ZgodaNaNewsletter = true;
//            k2.ZgodaNaNewsletter = true;
//            kategorie_klientow kat1=new kategorie_klientow(){Id = 1,nazwa = "kat1"};
//            kategorie_klientow kat2 = new kategorie_klientow() { Id = 2, nazwa = "kat2" };
//            IKategorieKlientowBll kategorieKlientow = A.Fake<IKategorieKlientowBll>();
//            A.CallTo(() => kategorieKlientow.Pobierz(A<HashSet<int>>.Ignored)).Returns(new List<kategorie_klientow> { kat1, kat2 });
//            IKlienciDostep klientDostep = A.Fake<IKlienciDostep>();
//            A.CallTo(() => klientDostep.KlienciWKategorii(1)).Returns(new HashSet<IKlient> { k1 });
//            A.CallTo(() => klientDostep.KlienciWKategorii(2)).Returns(new HashSet<IKlient> { k2 });
//            IConfigBLL config = A.Fake<IConfigBLL>();
//            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//            A.CallTo(() => calosc.Konfiguracja).Returns(config);
//            A.CallTo(() => calosc.Klienci).Returns(klientDostep);
//            A.CallTo(() => calosc.KategorieKlientow).Returns(kategorieKlientow);
//            var maile = new MaileBLL(calosc);

//              mailing_kampanie kampania=new mailing_kampanie();
//            kampania.ListaKategoriiKlientowId = new HashSet<int> {1, 2};
        
//            var wyslane = new Dictionary<string, Tuple<bool, string>>();
       
//            A.CallTo(() => config.JezykIDPolski).Returns(1);
//            A.CallTo(() => maile.DoKogoPoszlyJuzMaileKampanii(kampania)).Returns(wyslane);

     
//            Dictionary<string, Tuple<bool,string>> wyslaneWynik;
          

//            var wynik = maile.PobierzListeKlientowDoWysylki(kampania, out wyslaneWynik);
//            Assert.True(wyslaneWynik.Count==0,"Nikt nie powienn być w wysłanych");
//            Assert.Equal(wynik.Count,2);
//        }
//        [Fact(DisplayName = "Pobieranie klientów do wysyłania newslettera, nic nie ma w wyslanych,jeden brak zgody, jeden do wysłania")]
//        public void PobierzListeKlientowDoWysylkiTest6()
//        {
//            kategorie_klientow kat1 = new kategorie_klientow() { Id = 1, nazwa = "kat1" };
//            kategorie_klientow kat2 = new kategorie_klientow() { Id = 2, nazwa = "kat2" };

//            IKlient k1 = A.Fake<IKlient>();
//            k1.klient_id = 1;
//            k1.email = "mail1";
//            IKlient k2 = A.Fake<IKlient>();
//            k2.klient_id = 2;
//            k2.email = "mail2";

//            k1.ZgodaNaNewsletter = true;
//            k2.ZgodaNaNewsletter = false;
//            mailing_kampanie kampania = new mailing_kampanie();
//            kampania.ListaKategoriiKlientowId = new HashSet<int> { 1, 2 };
//            IKlienciDostep klientDostep = A.Fake<IKlienciDostep>();
//            A.CallTo(() => klientDostep.KlienciWKategorii(1)).Returns(new HashSet<IKlient> { k1 });
//            A.CallTo(() => klientDostep.KlienciWKategorii(2)).Returns(new HashSet<IKlient> { k2 });
//            var wyslane = new Dictionary<string, Tuple<bool, string>>();

//            IConfigBLL config = A.Fake<IConfigBLL>();
//            A.CallTo(() => config.JezykIDPolski).Returns(1);
          
//            IKategorieKlientowBll kategorieKlientow = A.Fake<IKategorieKlientowBll>();
//            A.CallTo(() => kategorieKlientow.Pobierz(A<HashSet<int>>.Ignored)).Returns(new List<kategorie_klientow> { kat1, kat2 });


       
//            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//            A.CallTo(() => calosc.Konfiguracja).Returns(config);
//            A.CallTo(() => calosc.Klienci).Returns(klientDostep);
//            A.CallTo(() => calosc.KategorieKlientow).Returns(kategorieKlientow);
//            var maile = A.Fake<MaileBLL>(x => x.WithArgumentsForConstructor(new[] { calosc }));
//            A.CallTo(() => maile.DoKogoPoszlyJuzMaileKampanii(kampania)).Returns(wyslane);
//            Dictionary<string, Tuple<bool, string>> wyslaneWynik;


//            var wynik = maile.PobierzListeKlientowDoWysylki(kampania, out wyslaneWynik);
//            Assert.True(wyslaneWynik.Count == 0, "Nikt nie powienn być w wysłanych");
//            Assert.Equal(wynik.Count, 1);
//        }
//        [Fact(DisplayName = "Pobieranie klientów do wysyłania newslettera, jeden w wysłanych, 1 do wysłania")]
//        public void PobierzListeKlientowDoWysylkiTest2()
//        {
//            kategorie_klientow kat1 = new kategorie_klientow() { Id = 1, nazwa = "kat1" };
//            kategorie_klientow kat2 = new kategorie_klientow() { Id = 2, nazwa = "kat2" };
//            IKategorieKlientowBll kategorieKlientow = A.Fake<IKategorieKlientowBll>();
//            A.CallTo(() => kategorieKlientow.Pobierz(A<HashSet<int>>.Ignored)).Returns(new List<kategorie_klientow> { kat1, kat2 });
//            IKlient k1 = A.Fake<IKlient>();
//            k1.klient_id = 1;
//            k1.email = "mail1";
//            IKlient k2 = A.Fake<IKlient>();
//            k2.klient_id = 2;
//            k2.email = "mail2";
//            k1.ZgodaNaNewsletter = true;
//            k2.ZgodaNaNewsletter = true;
//            mailing_kampanie kampania = new mailing_kampanie();
//            kampania.ListaKategoriiKlientowId = new HashSet<int> { 1, 2 };
//            IKlienciDostep klientDostep = A.Fake<IKlienciDostep>();
//            A.CallTo(() => klientDostep.KlienciWKategorii(1)).Returns(new HashSet<IKlient>{k1});
//            A.CallTo(() => klientDostep.KlienciWKategorii(2)).Returns(new HashSet<IKlient> { k2 });
//            var wyslane = new Dictionary<string, Tuple<bool, string>>();
//            wyslane.Add("mail1",new Tuple<bool, string>(true,""));
//            IConfigBLL config = A.Fake<IConfigBLL>();
//            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//            A.CallTo(() => calosc.Konfiguracja).Returns(config);
//            A.CallTo(() => calosc.Klienci).Returns(klientDostep);
//            A.CallTo(() => calosc.KategorieKlientow).Returns(kategorieKlientow);
//            var maile = A.Fake<MaileBLL>(x => x.WithArgumentsForConstructor(new[] { calosc }));
//            A.CallTo(() => config.JezykIDPolski).Returns(1);
//            A.CallTo(() => maile.DoKogoPoszlyJuzMaileKampanii(kampania)).Returns(wyslane);



//            Dictionary<string, Tuple<bool, string>> wyslaneWynik;


//            var wynik = maile.PobierzListeKlientowDoWysylki(kampania, out wyslaneWynik);
//            Assert.True(wyslaneWynik.Count == 1, "1 wysłany");
//            Assert.Equal(wynik.Count, 1);
//        }

//        [Fact(DisplayName = "Pobieranie klientów do wysyłania newslettera, jeden w wysłanych, 1 do wysłania inna wersja")]
//        public void PobierzListeKlientowDoWysylkiTest3()
//        {
//            kategorie_klientow kat1 = new kategorie_klientow() { Id = 1, nazwa = "kat1" };
//            kategorie_klientow kat2 = new kategorie_klientow() { Id = 2, nazwa = "kat2" };
//            IKategorieKlientowBll kategorieKlientow = A.Fake<IKategorieKlientowBll>();
//            A.CallTo(() => kategorieKlientow.Pobierz(A<HashSet<int>>.Ignored)).Returns(new List<kategorie_klientow> { kat1, kat2 });
//            IKlient k1 = A.Fake<IKlient>();
//            k1.klient_id = 1;
//            k1.email = "mail1";
//            IKlient k2 = A.Fake<IKlient>();
//            k2.klient_id = 2;
//            k2.email = "mail2";
//            k1.ZgodaNaNewsletter = true;
//            k2.ZgodaNaNewsletter = true;
//            mailing_kampanie kampania = new mailing_kampanie();
//            kampania.ListaKategoriiKlientowId = new HashSet<int> { 1, 2 };
//            IKlienciDostep klientDostep = A.Fake<IKlienciDostep>();
//            A.CallTo(() => klientDostep.KlienciWKategorii(1)).Returns(new HashSet<IKlient>());
//            A.CallTo(() => klientDostep.KlienciWKategorii(2)).Returns(new HashSet<IKlient> { k2,k1 });
//            var wyslane = new Dictionary<string, Tuple<bool, string>>();
//            wyslane.Add("mail1", new Tuple<bool, string>(true,""));
//            IConfigBLL config = A.Fake<IConfigBLL>();
//            A.CallTo(() => config.JezykIDPolski).Returns(1);


//            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//            A.CallTo(() => calosc.Konfiguracja).Returns(config);
//            A.CallTo(() => calosc.Klienci).Returns(klientDostep);
//            A.CallTo(() => calosc.KategorieKlientow).Returns(kategorieKlientow);
//            var maile = A.Fake<MaileBLL>(x => x.WithArgumentsForConstructor(new[] { calosc }));
//            A.CallTo(() => maile.DoKogoPoszlyJuzMaileKampanii(kampania)).Returns(wyslane);



//            Dictionary<string, Tuple<bool, string>> wyslaneWynik;


//            var wynik = maile.PobierzListeKlientowDoWysylki(kampania, out wyslaneWynik);
//            Assert.True(wyslaneWynik.Count == 1, "1 wysłany");
//            Assert.Equal(wynik.Count, 1);
//        }
//        [Fact(DisplayName = "Pobieranie klientów do wysyłania newslettera, jeden w wysłanych, 1 do wysłania inna wersja, klienci w 2 kategoriach")]
//        public void PobierzListeKlientowDoWysylkiTest4()
//        {
//            kategorie_klientow kat1 = new kategorie_klientow() { Id = 1, nazwa = "kat1" };
//            kategorie_klientow kat2 = new kategorie_klientow() { Id = 2, nazwa = "kat2" };
//            IKategorieKlientowBll kategorieKlientow = A.Fake<IKategorieKlientowBll>();
//            A.CallTo(() => kategorieKlientow.Pobierz(A<HashSet<int>>.Ignored)).Returns(new List<kategorie_klientow> { kat1, kat2 });
//            IKlient k1 = A.Fake<IKlient>();
//            k1.klient_id = 1;
//            k1.email = "mail1";
//            IKlient k2 = A.Fake<IKlient>();
//            k2.klient_id = 2;
//            k2.email = "mail2";
//            k1.ZgodaNaNewsletter = true;
//            k2.ZgodaNaNewsletter = true;
//            mailing_kampanie kampania = new mailing_kampanie();
//            kampania.ListaKategoriiKlientowId = new HashSet<int> { 1, 2 };
//            IKlienciDostep klientDostep = A.Fake<IKlienciDostep>();
//            A.CallTo(() => klientDostep.KlienciWKategorii(1)).Returns(new HashSet<IKlient>{k2, k1});
//            A.CallTo(() => klientDostep.KlienciWKategorii(2)).Returns(new HashSet<IKlient> { k2, k1 });
//            var wyslane = new Dictionary<string, Tuple<bool, string>>();
//            wyslane.Add("mail1", new Tuple<bool, string>(true,""));
//            IConfigBLL config = A.Fake<IConfigBLL>();
//            A.CallTo(() => config.JezykIDPolski).Returns(1);
//            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//            A.CallTo(() => calosc.Konfiguracja).Returns(config);
//            A.CallTo(() => calosc.Klienci).Returns(klientDostep);
//            A.CallTo(() => calosc.KategorieKlientow).Returns(kategorieKlientow);
//            var maile = A.Fake<MaileBLL>(x => x.WithArgumentsForConstructor(new[] { calosc }));
//            A.CallTo(() => maile.DoKogoPoszlyJuzMaileKampanii(kampania)).Returns(wyslane);


//            Dictionary<string, Tuple<bool, string>> wyslaneWynik;


//            var wynik = maile.PobierzListeKlientowDoWysylki(kampania, out wyslaneWynik);
//            Assert.True(wyslaneWynik.Count == 1, "1 wysłany");
//            Assert.Equal(wynik.Count, 1);
//        }
//        [Fact(DisplayName = "Pobieranie klientów do wysyłania newslettera, nic nie ma w wyslanych, dwóch do wysłania,klienci w 2 kategoriach")]
//        public void PobierzListeKlientowDoWysylkiTest5()
//        {
//            kategorie_klientow kat1 = new kategorie_klientow() { Id = 1, nazwa = "kat1" };
//            kategorie_klientow kat2 = new kategorie_klientow() { Id = 2, nazwa = "kat2" };
//            IKategorieKlientowBll kategorieKlientow = A.Fake<IKategorieKlientowBll>();
//            A.CallTo(() => kategorieKlientow.Pobierz(A<HashSet<int>>.Ignored)).Returns(new List<kategorie_klientow> { kat1, kat2 });
//            IKlient k1 = A.Fake<IKlient>();
//            k1.klient_id = 1;
//            k1.email = "mail1";
//            IKlient k2 = A.Fake<IKlient>();
//            k2.klient_id = 2;
//            k2.email = "mail2";
//            k1.ZgodaNaNewsletter = true;
//            k2.ZgodaNaNewsletter = true;
//            mailing_kampanie kampania = new mailing_kampanie();
//            kampania.ListaKategoriiKlientowId = new HashSet<int> { 1, 2 };
//            IKlienciDostep klientDostep = A.Fake<IKlienciDostep>();
//            A.CallTo(() => klientDostep.KlienciWKategorii(1)).Returns(new HashSet<IKlient> { k1, k2 });
//            A.CallTo(() => klientDostep.KlienciWKategorii(2)).Returns(new HashSet<IKlient> { k2, k1 });
//            var wyslane = new Dictionary<string, Tuple<bool, string>>();
//            IConfigBLL config = A.Fake<IConfigBLL>();
//            A.CallTo(() => config.JezykIDPolski).Returns(1);
//            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
//            A.CallTo(() => calosc.Konfiguracja).Returns(config);
//            A.CallTo(() => calosc.Klienci).Returns(klientDostep);
//            A.CallTo(() => calosc.KategorieKlientow).Returns(kategorieKlientow);
//            var maile = A.Fake<MaileBLL>(x => x.WithArgumentsForConstructor(new[] { calosc }));

//            A.CallTo(() => maile.DoKogoPoszlyJuzMaileKampanii(kampania)).Returns(wyslane);

//            Dictionary<string, Tuple<bool, string>> wyslaneWynik;


//            var wynik = maile.PobierzListeKlientowDoWysylki(kampania, out wyslaneWynik);
//            Assert.True(wyslaneWynik.Count == 0, "Nikt nie powienn być w wysłanych");
//            Assert.Equal(wynik.Count, 2);
//        }
//    }
//}
