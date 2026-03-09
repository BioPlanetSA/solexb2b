using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Pliki
{
    public class ZdjeciaDoCechTests
    {
        [Fact(DisplayName = "Test sprawdzający poprawność dopasowywania zdjęc do cech")]
        public void PrzetworzTest()
        {
            string sciezka = "c:\\pliki\\";
            string pole = "nazwa";
            IDictionary<long, Produkt> produkty = new Dictionary<long, Produkt>();
            List<ProduktPlik> lista = new List<ProduktPlik>();
            List<Plik> listaPlikow = new List<Plik>();
            ISyncProvider provider = null;
            List<KategoriaProduktu> kategorieB2b = new List<KategoriaProduktu>();
            List<Klient> klienci = new List<Klient>();


            Cecha cecha1 = new Cecha() { Id = 1, Nazwa = "BANAWULTRA250_2_1" };
            Cecha cecha2 = new Cecha() { Id = 2, Nazwa = "BAREG200_3_1" };
            Cecha cecha3 = new Cecha(){Id = 3, Nazwa = "Testowa nazwa 3"};

            List<Cecha> listaCech = new List<Cecha>(){cecha1, cecha2, cecha3};

            ZdjeciaDoCech zdc = new ZdjeciaDoCech();
            zdc.Sciezka = sciezka;
            zdc.Pole = pole;
            zdc.KomuPrzypisacZdjecie=KomuPrzypisywac.PierwzemuZnalezionemu;
            //zdc.SposobDopasowania = SposobDopasowania.JedenDoJednego;
            //zdc.Przetworz(produkty, ref lista, ref listaPlikow, provider, ref listaCech, ref kategorieB2b, ref klienci);
            //Assert.True(listaPlikow.Count==2);

            zdc.SposobDopasowania = SposobDopasowania.ZawieraWSobie;
            zdc.Przetworz(produkty, ref lista, ref listaPlikow, provider, ref listaCech, ref kategorieB2b, ref klienci);
            Assert.True(listaPlikow.Count==1);
        }
    }
}
