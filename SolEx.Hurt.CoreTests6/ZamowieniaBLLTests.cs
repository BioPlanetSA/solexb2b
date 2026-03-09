using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.Common;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Core.Tests
{
    public class ZamowieniaBLLTests
    {
     
        [Fact(DisplayName = "Genrowanie nazwy dokumentu")]
        public void GenrowanieNazwy()
        {
           TestNazwy(1,"1/1/2014","",0,2014,1,1);
           TestNazwy(1,"1/2/2014", "", 0, 2014, 2, 1);
           TestNazwy(1,"1/test/2014", "test", 1, 2014, 1, 1);
           TestNazwy(1,"1/test/2014", "test", 1, 2014, 2, 1);
        }

        void TestNazwy(int zamowienieid,string oczekiwana, string odzial, int odzialid, int rok, int miesiac,int dzien)
        {
            IKlient kliet = A.Fake<IKlient>();
            A.CallTo(() => kliet.OddzialDoJakiegoNalezyKlient).Returns(odzialid);
            A.CallTo(() => kliet.OddzialDoJakiegoNalezyKlientNazwa).Returns(odzial);
            ZamowieniaBLL z = A.Fake<ZamowieniaBLL>();
            z.zamowienie_id = zamowienieid;
            z.data_utworzenia = new DateTime(rok, miesiac, dzien);
            A.CallTo(() => z.Klient).Returns(kliet);

      
            Assert.True(z.DokumentNazwa == oczekiwana, string.Format("Oczekiwana {0}, otrzymana {1} odzial {2} {3} data {4} {5} {6} zamowienie {7}"
                , oczekiwana, z.DokumentNazwa,odzialid,odzial,rok,miesiac,dzien,zamowienieid));
        }

        //[Fact(DisplayName = "tescik")]
        //public void JakisTest()
        //{
        //    ZamowienieSynchronizacja zs1 = new ZamowienieSynchronizacja() { DokumentNazwaSynchronizacja = "ZK 805/04/2015", zamowienie_id = 794, numerZRozbicia = "B2B  794/1/Odziez z 2", IdDokumentu = 140237 };
        //    ZamowienieSynchronizacja zs2 = new ZamowienieSynchronizacja() { DokumentNazwaSynchronizacja = "ZK 805/04/2015", zamowienie_id = 794, numerZRozbicia = "B2B  794/2/SpozAkce z 2", IdDokumentu = 140238 };
        //    ZamowienieSynchronizacja zs3 = new ZamowienieSynchronizacja() { DokumentNazwaSynchronizacja = "ZK 815/04/2015", zamowienie_id = 795, numerZRozbicia = "B2B  795/2/SpozAkce z 2", IdDokumentu = 140239 };
        //    List<ZamowienieSynchronizacja> listaZamowienSynchronizacji = new List<ZamowienieSynchronizacja>() { zs1, zs2,zs3 };
        //    foreach (var x in listaZamowienSynchronizacji)
        //    {
        //        string idDokumentu = string.Empty;
        //        string nazwaSynchro = string.Empty;
        //        HashSet<ZamowienieSynchronizacja> hashset = listaZamowienSynchronizacji.Where(a => a.numerZRozbicia.Contains(x.zamowienie_id.ToString())).ToHashSet();
        //        foreach (var zamowienieSynchronizacja in hashset)
        //        {
        //            idDokumentu += string.Format("{0};",zamowienieSynchronizacja.IdDokumentu);
        //            nazwaSynchro += string.Format("{0};", zamowienieSynchronizacja.DokumentNazwaSynchronizacja);
        //        }
        //        string oczekiwane = string.Format("{0};{1};", zs1.IdDokumentu, zs2.IdDokumentu);
        //        Assert.True(idDokumentu == oczekiwane);

        //    }

        //}
    }
}
