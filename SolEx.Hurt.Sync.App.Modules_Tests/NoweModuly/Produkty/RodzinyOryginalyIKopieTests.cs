using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Sync.App.Modules_Tests.Rozne;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class RodzinyOryginalyIKopieTests
    {
        [Fact(DisplayName = "Sprawdzenie poprawności działania modułu RodzinyOryginalyIKopie - dodawanie brakujących atrybutów i cech")]
        public void DodawanieBrakujacychAtrybutowICech()
        {
            RodzinyOryginalyIKopie modul = new RodzinyOryginalyIKopie();
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.TypDomyslnyFiltru).Returns("_DropDownWielokrotnegoWyboru");
            modul.Config = config;

            Atrybut atrybut = modul.WygenerujAtrybut(modul.NazwaAtrybutu);

            Cecha cechaOryginal = modul.WygenerujCeche(modul.NazwaDlaOryginalu, atrybut);
            Cecha cechaKopia = modul.WygenerujCeche(modul.NazwaDlaKopii, atrybut);

            List<Atrybut> listaAtrybutowFakowych = GenerowanieObiektow.WygenerujLosoweAtrybuty(modul, 100);
            List<Cecha> listaCechFakowych = GenerowanieObiektow.WygenerujLosoweCechy(modul, listaAtrybutowFakowych, 15);

            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            modul.Przetworz(ref listaAtrybutowFakowych, ref listaCechFakowych,produktyNaB2B);

            //czy utworzony atrybut jest na liście i ma takie samo wygenerowane ID
            var dodanyAtrybut = listaAtrybutowFakowych.FirstOrDefault(a=> a.Nazwa == atrybut.Nazwa);
            Assert.NotNull(dodanyAtrybut);
            Assert.Equal(atrybut.Id, dodanyAtrybut.Id);

            //czy utworzona cecha dla oryginału ma takie samo wygenerowane ID
            var dodanaCechaOryginal = listaCechFakowych.FirstOrDefault(a => a.Symbol == cechaOryginal.Symbol);
            Assert.NotNull(dodanaCechaOryginal);
            Assert.Equal(cechaOryginal.Id, dodanaCechaOryginal.Id);

            //czy utworzona cecha dla kopii ma takie samo wygenerowane ID
            var dodanaCechaKopia = listaCechFakowych.FirstOrDefault(a => a.Symbol == cechaKopia.Symbol);
            Assert.NotNull(dodanaCechaKopia);
            Assert.Equal(cechaKopia.Id, dodanaCechaKopia.Id);

            //czy po zmianie danych dla atrybutu będącego już na liście moduł poprawnie go odszuka i nadpisze
            dodanyAtrybut.Id = 1231231;
            dodanyAtrybut.Widoczny = false;
            dodanyAtrybut.ProviderWyswietlania = "Asdasdasd";
            modul.Przetworz(ref listaAtrybutowFakowych, ref listaCechFakowych,produktyNaB2B);
            
            var znalezioneAtrybuty = listaAtrybutowFakowych.Where(a => a.Nazwa == atrybut.Nazwa).ToList();
            Assert.Equal(1, znalezioneAtrybuty.Count);
            Assert.Equal(atrybut.Id, znalezioneAtrybuty.First().Id);

            //takie same zmiany ale dla cechy kopii
            dodanaCechaKopia.Nazwa = "aertsejrtert";
            dodanaCechaKopia.AtrybutId = 34234234;
            dodanaCechaKopia.Widoczna = false;
            dodanaCechaKopia.Id = 344444444;
            modul.Przetworz(ref listaAtrybutowFakowych, ref listaCechFakowych,produktyNaB2B);
            var znalezineCechyKopia = listaCechFakowych.Where(a => a.Symbol == cechaKopia.Symbol);
            Assert.Equal(1, znalezineCechyKopia.Count());
            Assert.Equal(cechaKopia.Id, znalezineCechyKopia.First().Id);

            //takie same zmiany ale dla cechy kopii
            dodanaCechaOryginal.Nazwa = "aertsasdfasrtejrtert";
            dodanaCechaOryginal.AtrybutId = 868678678;
            dodanaCechaOryginal.Widoczna = false;
            dodanaCechaOryginal.Id = 345357;
            modul.Przetworz(ref listaAtrybutowFakowych, ref listaCechFakowych,produktyNaB2B);
            var znalezineCechyOryginal = listaCechFakowych.Where(a => a.Symbol == cechaOryginal.Symbol);
            Assert.Equal(1, znalezineCechyOryginal.Count());
            Assert.Equal(cechaOryginal.Id, znalezineCechyOryginal.First().Id);
        }

        [Fact(DisplayName = "Sprawdzenie poprawności działania modułu RodzinyOryginalyIKopie - dodawanie brakujących łączników cech")]
        public void DodawanieLacznikowCechDlaProduktowWRodzinie()
        {
            RodzinyOryginalyIKopie modul = new RodzinyOryginalyIKopie();
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.TypDomyslnyFiltru).Returns("_DropDownWielokrotnegoWyboru");
            modul.Config = config;
            Atrybut atrybut = modul.WygenerujAtrybut(modul.NazwaAtrybutu);
            Cecha cechaOryginal = modul.WygenerujCeche(modul.NazwaDlaOryginalu, atrybut);
            Cecha cechaKopia = modul.WygenerujCeche(modul.NazwaDlaKopii, atrybut);

            List<Atrybut> listaAtrybutowFakowych = GenerowanieObiektow.WygenerujLosoweAtrybuty(modul, 100);
            List<Cecha> listaCechFakowych = GenerowanieObiektow.WygenerujLosoweCechy(modul, listaAtrybutowFakowych, 15);
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            modul.Przetworz(ref listaAtrybutowFakowych, ref listaCechFakowych,produktyNaB2B);

            List<Produkt> listaProduktowFakowych = GenerowanieObiektow.WygenerujProdukty(100);

            Dictionary<long, ProduktCecha> laczniki = new Dictionary<long, ProduktCecha>();
            foreach (Cecha cecha in listaCechFakowych)
            {
                if (cecha.Symbol != cechaOryginal.Symbol && cecha.Symbol != cechaKopia.Symbol)
                {
                    List<ProduktCecha> listaLacznikow = GenerowanieObiektow.wygenerujListeLacznikowDlaCechy(listaProduktowFakowych, cecha, modul);

                    foreach (var cp in listaLacznikow)
                    {
                        laczniki.Add(cp.Id, cp);
                    }
                }
            }
            List<ProduktyZamienniki> TODO = new List<ProduktyZamienniki>();
            List<Tlumaczenie> slownikitmp = new List<Tlumaczenie>(0);
            List<JednostkaProduktu> jednostkitmp = new List<JednostkaProduktu>(0);
           List< ProduktKategoria> lacznikikategoriitmp = new List< ProduktKategoria>(0);
            List<ProduktUkryty> produktyukrytetmp = new List<ProduktUkryty>(0);   List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           
            modul.Przetworz(ref listaProduktowFakowych, ref slownikitmp, new Dictionary<long, Produkt>(0), ref jednostkitmp, ref laczniki, ref lacznikikategoriitmp,
                null, ref produktyukrytetmp, ref TODO, new Dictionary<long, KategoriaProduktu>() ,ref cechy,ref atrybuty);


            foreach (Produkt produkt in listaProduktowFakowych)
            {
                if (!string.IsNullOrEmpty(produkt.Rodzina))
                {
                    if (produkt.Nazwa == produkt.Rodzina)
                    {
                        var lacznik = modul.WygenerujLacznikCech(cechaOryginal, produkt);
                    
                        Assert.True(laczniki.ContainsKey(lacznik.Id));
                        Assert.Equal(lacznik.Id, laczniki[lacznik.Id].Id);
                        Assert.Equal(cechaOryginal.Id, laczniki[lacznik.Id].CechaId);    
                    }
                    else
                    {
                        var lacznik = modul.WygenerujLacznikCech(cechaKopia, produkt);

                        Assert.True(laczniki.ContainsKey(lacznik.Id));
                        Assert.Equal(lacznik.Id, laczniki[lacznik.Id].Id);
                        Assert.Equal(cechaKopia.Id, laczniki[lacznik.Id].CechaId);    
                    }
                }
            }
        }
    }
}
