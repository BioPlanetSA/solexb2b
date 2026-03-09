using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty.Tests
{
    public class AktywacjaKlientowNaPodstawieWartosciFakturTests
    {
        [Fact(DisplayName = "AktywacjaKlientowNaPodstawieWartosciFakturTests - dzialanie modulu")]
        public void PrzetworzTest()
        {
            List<StatusDokumentu> status = new List<StatusDokumentu>();
            status.Add( new StatusDokumentu(1, 1, "", 5,DateTime.Now.AddDays(-5), RodzajDokumentu.Faktura));
            status.Add(new StatusDokumentu(2, 1, "", 20, DateTime.Now.AddDays(-5), RodzajDokumentu.Zamowienie));
            status.Add(new StatusDokumentu(3, 2, "", 30, DateTime.Now.AddDays(-40), RodzajDokumentu.Faktura));
            status.Add(new StatusDokumentu(4, 1, "", 60, DateTime.Now.AddDays(-60), RodzajDokumentu.Faktura));
            status.Add(new StatusDokumentu(5, 1, "", 30, DateTime.Now.AddDays(-14), RodzajDokumentu.Faktura));
            //TestDzialania(false, 500, 30, new Klient() { Id = 1 }, status.ToDictionary(x => x.Id, x => x), BlokadaPowod.Brak);
            //TestDzialania(true, 500, 30, new Klient() { Id = 1 }, status.ToDictionary(x => x.Id, x => x), BlokadaPowod.BrakFaktur);
            //TestDzialania(true, 20, 30, new Klient() { Id = 1 }, status.ToDictionary(x => x.Id, x => x), BlokadaPowod.Brak);
            //TestDzialania(true, 70, 90, new Klient() { Id = 1 }, status.ToDictionary(x => x.Id, x => x), BlokadaPowod.Brak);
        }

        private void TestDzialania(bool uruchamiac, decimal wartosc, int iledni, Klient kient, Dictionary<int, long> status, BlokadaPowod wynik)
        {
            List<Klient> klienci = new List<Klient>();
            klienci.Add(kient);

            ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> dok =new ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();
            List<StatusZamowienia> pozycje = new List<StatusZamowienia>();
            AktywacjaKlientowNaPodstawieWartosciFaktur modul = A.Fake<AktywacjaKlientowNaPodstawieWartosciFaktur>();
            modul.IleDniPrzed = iledni;
            modul.MinimalnaWartosc = wartosc;
            A.CallTo(() => modul.UruchamiacModul).Returns(uruchamiac);
            modul.Przetworz(ref dok, ref pozycje, status,ref klienci);
            
            string sciezka = modul.SciezkaDoPliku;
            if (File.Exists(sciezka))
            {
                File.Delete(sciezka);
            }
            Assert.True(kient.PowodBlokady==wynik,string.Format("Wynik {0}, oczekiwany {1}, uruchamiac {2}, ilosc dok {3} iledni {4} wartosc {5}"
                ,kient.PowodBlokady,wynik,uruchamiac,status.Count,iledni,wartosc));


        }

        [Fact(DisplayName = "AktywacjaKlientowNaPodstawieWartosciFakturTests - czy moduł ma być uruchamiany")]
        public void PrzetworzTest1()
        {

            Testuruchomienia(null, 30,true);
            Testuruchomienia(DateTime.Now.AddDays(-2), 30, false);
            Testuruchomienia(DateTime.Now.AddDays(-35), 30, true);
        }

        private void Testuruchomienia(DateTime? datapliku,int iledni,bool oczekiwanyWynik)
        {
            AktywacjaKlientowNaPodstawieWartosciFaktur modul = new AktywacjaKlientowNaPodstawieWartosciFaktur();
            modul.CoIleDni = iledni;
            string sciezka = modul.SciezkaDoPliku;
            if (File.Exists(sciezka))
            {
                File.Delete(sciezka);
            }
            if (datapliku != null)
            {
               File.WriteAllText(sciezka,"");
                File.SetLastWriteTime(sciezka,datapliku.Value);
            }
            Assert.True(oczekiwanyWynik==modul.UruchamiacModul,string.Format("Wynik {0} oczekiwany {1}, data pliku {2} ",modul.UruchamiacModul,oczekiwanyWynik,datapliku));
            if (File.Exists(sciezka))
            {
                File.Delete(sciezka);
            }
        }
    }
}
