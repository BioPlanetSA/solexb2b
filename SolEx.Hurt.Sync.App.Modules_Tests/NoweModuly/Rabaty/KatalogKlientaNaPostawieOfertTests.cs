//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FakeItEasy;
//using SolEx.Hurt.Helpers;
//using SolEx.Hurt.Model;
//using SolEx.Hurt.Model.CustomSearchCriteria;
//using SolEx.Hurt.Model.Enums;
//using SolEx.Hurt.Model.Interfaces;
//using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
//using Xunit;
//namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
//{
//    public class KatalogKlientaNaPostawieOfertTests
//    {
//        [Fact(Name = "Generowanie produktów klienta na podstawie dokumnentów oferty")]
//        public void PrzetworzTest()
//        {
//            Test(new List<historia_dokumenty>(), 0, new List<produkty_ukryte>());
//            Test(new List<historia_dokumenty>(), 1, new List<produkty_ukryte>());

//            var docs = new List<historia_dokumenty>();

//            var pu = new List<produkty_ukryte>();

//            historia_dokumenty hd = new historia_dokumenty();
//            docs.Add(hd);
//            hd.data_utworzenia = DateTime.Now.Date.AddDays(-1);
//            hd.historia_dokumenty_produkty = new List<historia_dokumenty_produkty>();
//            hd.klient_id = 1;
//            historia_dokumenty_produkty hdp = new historia_dokumenty_produkty();
//            produkty_ukryte r=new produkty_ukryte();
//            hd.historia_dokumenty_produkty.Add(hdp);
//            pu.Add(r);
//            hdp.produkt_id = r.produkt_zrodlo_id = 1;
          
//            r.Tryb = KatalogKlientaTypy.MojKatalog;
//            r.klient_zrodlo_id = 1;
//            Test(docs, 0, pu);
//            Test(docs, 1, pu);
//        }
//        void Test(List<historia_dokumenty> dokumenty, int waznosc, List<produkty_ukryte> oczekiwane)
//        {
//            IAPIWywolania api = A.Fake<IAPIWywolania>();
//            KatalogKlientaNaPostawieOfert modul = new KatalogKlientaNaPostawieOfert();
//            IConfigBLL config = A.Fake<IConfigBLL>();
//            Hurt.Core.BLL.Dokumenty dok = new Hurt.Core.BLL.Dokumenty();
//            dok.ConfigBll = config;
//            modul.Statusy = new List<string> { "1", "2" };
         
//            modul.ApiWywolanie = api;
//            modul.dokumenty = dok;

//            A.CallTo(() => api.PobierzDokumenty(A<DokumentySearchCriteria>.Ignored)).Returns(dokumenty);
//            List<rabaty> wynik = new List<rabaty>();
//            List<Konfekcje> konfekcje = new List<Konfekcje>();
//            IDictionary<int, kategorie_klientow> kk = new Dictionary<int, kategorie_klientow>();
//            IDictionary<string, klienci_kategorie> kklacz = new Dictionary<string, klienci_kategorie>();
//            List<produkty_ukryte> pu = new List<produkty_ukryte>();
//            modul.Przetworz(ref wynik, ref pu, ref konfekcje, new Dictionary<int, klienci>(), new Dictionary<int, produkty>(),
//                new List<poziomy_cen>(), new List<cechy>(), new List<cechy_produkty>(), new Dictionary<int, kategorie>(),
//                new List<produkty_kategorie>(), ref kk, ref kklacz);

//            Assert.Equal(oczekiwane.Count, pu.Count);
//            foreach (var r in oczekiwane)
//            {
//                bool jest = pu.Any(x => x.Porownaj(r));
//                Assert.True(jest, "W wynikowych nie znaleziono oczekiwanego rabatu");
//            }
//            foreach (var r in pu)
//            {
//                bool jest = oczekiwane.Any(x => x.Porownaj(r));
//                Assert.True(jest, "W oczekiwanych nie znaleziono oczekiwanego rabatu");
//            }
//        }      
//    }
//}
