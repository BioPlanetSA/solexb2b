//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FakeItEasy;
//using SolEx.Hurt.Core.ModelBLL;
//using SolEx.Hurt.Helpers;
//using SolEx.Hurt.Model;
//using SolEx.Hurt.Model.CustomSearchCriteria;
//using SolEx.Hurt.Model.Enums;
//using SolEx.Hurt.Model.Interfaces;
//using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
//using Xunit;
//namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
//{
//    public class CenyNaPodstawieOfertTests
//    {

//        Dictionary<int, produkty> produktyNaB2B = new Dictionary<int, produkty>();

//        [Fact(Name = "Generowanie stałych cen na podstawie dokumnentów oferty")]
//        public void PrzetworzTest()
//        {
//            //Test(new List<historia_dokumenty>(), 0, new List<rabaty>());
//            //Test(new List<historia_dokumenty>(), 1, new List<rabaty>());

//            var docs = new List<historia_dokumenty>();

//            var rabaty = new List<rabaty>();

//            historia_dokumenty hd = new historia_dokumenty();
//            docs.Add(hd);
//            hd.data_utworzenia = DateTime.Now.Date.AddDays(-1);
//            hd.historia_dokumenty_produkty = new List<historia_dokumenty_produkty>();
//            hd.klient_id = 1;

//            produkty prod1 = new produkty();
//            produkty prod2 = new produkty();
//            produkty prod3 = new produkty();
//            produkty prod4 = new produkty();
            
//            prod1.produkt_id = 1;
//            prod2.produkt_id = 2;
//            prod3.produkt_id = 3;
//            prod4.produkt_id = 4;

//            prod1.rodzina = "rodzina1";
//            prod2.rodzina = "rodzina1";
//            prod3.rodzina = "rodzina2";
//            prod4.rodzina = "rodzina2";

//            produktyNaB2B.Add(prod1.produkt_id, prod1);
//            produktyNaB2B.Add(prod2.produkt_id, prod2);
//            produktyNaB2B.Add(prod3.produkt_id, prod3);
//            produktyNaB2B.Add(prod4.produkt_id, prod4);

//            historia_dokumenty_produkty hdp = new historia_dokumenty_produkty();
//            Model.rabaty r = new rabaty();
//            hd.historia_dokumenty_produkty.Add(hdp);
//            rabaty.Add(r);
//            hdp.produkt_id = r.produkt_id = 1;

//            r.wartosc1 = r.wartosc2 = r.wartosc3 = r.wartosc4 = r.wartosc5 = hdp.cena_netto = 0;
//            r.TypRabatu=RabatTyp.Zaawansowany;
//            r.TypWartosci=RabatSposob.StalaCena;
//            r.klient_id = 1;

//            Test(docs,0,rabaty);

//            rabaty = new List<rabaty>();
//            rabaty.Add(r);
//            Test(docs, 1, rabaty);
//        }

//        void Test(List<historia_dokumenty> dokumenty, int waznosc, List<rabaty> oczekiwane)
//        {
//            IAPIWywolania api = A.Fake<IAPIWywolania>();
//            CenyNaPodstawieOfert modul=new CenyNaPodstawieOfert();
//            IConfigBLL config = A.Fake<IConfigBLL>();
//            Hurt.Core.BLL.Dokumenty dok=new Hurt.Core.BLL.Dokumenty();
//            dok.ConfigBll = config;
//            modul.Statusy=new List<string>{"1","2"};

//            modul.ApiWywolanie = api; 
//            modul.dokumenty = dok;
            
//            A.CallTo(() => api.PobierzDokumenty(A<DokumentySearchCriteria>.Ignored)).Returns(dokumenty);
//            List<rabaty> wynik=new List<rabaty>();
//            List<Konfekcje> konfekcje = new List<Konfekcje>();
//             IDictionary<int,kategorie_klientow> kk=new Dictionary<int, kategorie_klientow>();
//             IDictionary<string, klienci_kategorie> kklacz = new Dictionary<string, klienci_kategorie>();
//             List<produkty_ukryte> pu = new List<produkty_ukryte>();


//            //sprawdzam czy działa jeśli ceny nie sją jednakowe dla dzieci
//            modul.JednakoweCenyDlaWszystkichDzieci = false;
//            modul.Przetworz(ref wynik,ref pu,ref konfekcje,new Dictionary<int, klienci>(),produktyNaB2B,
//                new List<poziomy_cen>(),new List<cechy>(),new List<cechy_produkty>(),new Dictionary<int, kategorie>(),
//                new List<produkty_kategorie>(), ref kk, ref kklacz);

//            Assert.Equal(oczekiwane.Count,wynik.Count);
//            foreach (var r in oczekiwane)
//            {
//                bool jest = wynik.Any(x => x.Porownaj(r));
//                Assert.True(jest, "W wynikowych nie znaleziono oczekiwanego rabatu");
//            }
//            foreach (var r in wynik)
//            {
//                bool jest = oczekiwane.Any(x => x.Porownaj(r));
//                Assert.True(jest, "W oczekiwanych nie znaleziono oczekiwanego rabatu");
//            }

//            //sprawdzam czy działa jeśli ceny są jednakowe dla wszystkich dzieci
//            modul.JednakoweCenyDlaWszystkichDzieci = true;

//            Model.rabaty tmpRabaty = new rabaty();
//            tmpRabaty.produkt_id = 2;

//            tmpRabaty.wartosc1 = tmpRabaty.wartosc2 = tmpRabaty.wartosc3 = tmpRabaty.wartosc4 = tmpRabaty.wartosc5 = 0;
//            tmpRabaty.TypRabatu = RabatTyp.Zaawansowany;
//            tmpRabaty.TypWartosci = RabatSposob.StalaCena;
//            tmpRabaty.klient_id = 1;
//            oczekiwane.Add(tmpRabaty);

//            modul.Przetworz(ref wynik, ref pu, ref konfekcje, new Dictionary<int, klienci>(), produktyNaB2B,
//                new List<poziomy_cen>(), new List<cechy>(), new List<cechy_produkty>(), new Dictionary<int, kategorie>(),
//                new List<produkty_kategorie>(), ref kk, ref kklacz);


//            Assert.Equal(oczekiwane.Count, wynik.Count);

//            foreach (var r in oczekiwane)
//            {
//              bool jest=  wynik.Any(x => x.Porownaj(r));
//                Assert.True(jest,"W wynikowych nie znaleziono oczekiwanego rabatu");
//            }
//            foreach (var r in wynik)
//            {
//                bool jest = oczekiwane.Any(x => x.Porownaj(r));
//                Assert.True(jest, "W oczekiwanych nie znaleziono oczekiwanego rabatu");
//            }


//        }
//    }
//}
