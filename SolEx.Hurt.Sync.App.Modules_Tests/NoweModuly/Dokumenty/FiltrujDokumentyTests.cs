using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Dokumenty
{
    public class FiltrujDokumentyTests
    {
        [Fact(DisplayName = "Test sprawdzajacy poprawność działania metody filtrującej dokumenty")]
        public void PrzetworzTest()
        {
            //HistoriaDokumentu hd1 = new HistoriaDokumentu(){NazwaDokumentu = "NIE nazwa test 1"};
            //HistoriaDokumentu hd2 = new HistoriaDokumentu(){NazwaDokumentu = "Nazwa test 2"};
            //HistoriaDokumentu hd3 = new HistoriaDokumentu(){NazwaDokumentu = "nazwa test3"};
            //HistoriaDokumentu hd4 = new HistoriaDokumentu(){NazwaDokumentu = "nazwa test 4"};
            //HistoriaDokumentu hd5 = new HistoriaDokumentu(){NazwaDokumentu = "nazwa test 5"};
            //HistoriaDokumentu hd6 = new HistoriaDokumentu(){NazwaDokumentu = "nazwa test 6"};

            //Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa = new Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>(){ {hd1,null}, {hd2,null}, { hd3, null}, { hd4, null}, { hd5, null}, { hd6,null}};

            //FiltrujDokumenty fd = new FiltrujDokumenty();
            //List<StatusZamowienia> st = new List<StatusZamowienia>();
            //List<Klient> k = new List<Klient>();
            //string slowoZakazane = "NIE";
            //fd.SlowaZakazane = slowoZakazane;
            //fd.Przetworz(ref listaWejsciowa, ref st,new Dictionary<int, long>(),ref k);
            //Assert.True(!listaWejsciowa.Any(x=>x.Key.NazwaDokumentu.Contains(slowoZakazane)));

            //fd.SlowaZakazane = "";
            //fd.SlowaWymagane = "test3";

            //fd.Przetworz(ref listaWejsciowa, ref st, new Dictionary<int, long>(), ref k);
            //Assert.True(listaWejsciowa.Count==1);
        }
    }
}
