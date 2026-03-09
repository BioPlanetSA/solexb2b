using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class WczytajKoncesjeTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
           
            Cecha c1 = new Cecha() { Id = 1, Nazwa = "cecha 1", AtrybutId = 1 };
            Cecha c2 = new Cecha() { Id = 2, Nazwa = "cecha 1", AtrybutId = 1 };
            Cecha c3 = new Cecha() { Id = 3, Nazwa = "cecha 1", AtrybutId = 1 };
            Cecha c21 = new Cecha() { Id = 21, Nazwa = "cecha 21", AtrybutId = 2 };
            Cecha c22 = new Cecha() { Id = 22, Nazwa = "cecha 21", AtrybutId = 2 };
            Cecha c23 = new Cecha() { Id = 23, Nazwa = "cecha 21", AtrybutId = 2 };

            Atrybut atr1 = new Atrybut("atr1",1);
            Atrybut atr2 = new Atrybut("atr2",2);

            List<Cecha> listaCech = new List<Cecha>() { c1, c2, c3,c21,c22, c23 };

            List<Atrybut> atrybuty = new List<Atrybut>() {atr1,atr2};

            ProduktCecha cp1 = new ProduktCecha() {  ProduktId = 1,CechaId = 1};
            ProduktCecha cp2 = new ProduktCecha() { ProduktId = 2, CechaId = 2 };
            ProduktCecha cp3 = new ProduktCecha() { ProduktId = 3, CechaId = 3 };
            ProduktCecha cp31 = new ProduktCecha() { ProduktId = 1, CechaId = 3 };
            ProduktCecha cp32 = new ProduktCecha() { ProduktId = 2, CechaId = 1 };
            ProduktCecha cp33 = new ProduktCecha() { ProduktId = 3, CechaId = 2 };
            ProduktCecha cp21 = new ProduktCecha() { ProduktId = 1, CechaId = 21 };
            ProduktCecha cp22 = new ProduktCecha() { ProduktId = 2, CechaId = 22 };
            ProduktCecha cp23 = new ProduktCecha() { ProduktId = 3, CechaId = 23 };

            WczytajKoncesje wk = new WczytajKoncesje();
            wk.Atrybut = 1;
            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha> { { cp1.Id, cp1 }, { cp2.Id, cp2 }, { cp3.Id, cp3 }, { cp21.Id, cp21 }, { cp22.Id, cp22 }, { cp23.Id, cp23 }, { cp31.Id, cp31 }, { cp32.Id, cp32 }, { cp33.Id, cp33 } };

            Produkt p1 = new Produkt() { Id = 1, Kod = "00052_L", Rodzina = "KAMIZELKA OSTRZEGAWCZA W KOLORZE ŻÓŁTYM Z 2 PASKAMI" };
            Produkt p2 = new Produkt() { Id = 2, Kod = "00065" };
            Produkt p3 = new Produkt() { Id = 3, Kod = "00364_43", Rodzina = "TRZEWIKI ROBOCZE URG 101 KATEGORIA S3 EN20345" };

            Produkt p4 = new Produkt() { Id = 4, Kod = "00172", Rodzina = "TRZEWIKI ROBOCZE URG 101 KATEGORIA S3 EN20345" };
            Produkt p5 = new Produkt() { Id = 5, Kod = "00350_46", Rodzina = "GUMOWCE WYSOKIE WYKONANE Z PCV W KOLORZE CZARNYM EN20347" };
            Produkt p6 = new Produkt() { Id = 6, Kod = "09931_1", Rodzina = "SPODNIE OGRODNICZKI HARPOON AVACORE W KOLORZE SZARYM" };

            List<Produkt> listaWejsciowa = new List<Produkt>(){p1,p2,p3,p4,p5,p6};
            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            List<ProduktKategoria> lacznikiKategorii =new  List<ProduktKategoria>();
            Model.Interfaces.Sync.ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
            
            wk.Przetworz(ref listaWejsciowa, ref produktyTlumaczenia, produktyNaB2B, ref  jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp,
                ref zamienniki, new Dictionary<long, KategoriaProduktu>(),ref listaCech, ref atrybuty);
        
            Assert.True(listaWejsciowa.Any(x=>x.WymaganaKoncesja != null));
        }

        [Fact(DisplayName = "Test w sytuacji gdy pole do szukania jest nullem")]
        public void PrzetworzTest3()
        {

            Cecha c1 = new Cecha() { Id = 1, Nazwa = "08616;08618;08615", AtrybutId = 1 };
            Cecha c2 = new Cecha() { Id = 2, Nazwa = "01630", AtrybutId = 1 };
            Cecha c3 = new Cecha() { Id = 3, Nazwa = "09931_1;09951;09970", AtrybutId = 1 };

            List<Cecha> ListaCech = new List<Cecha>() { c1, c2, c3 };


            ProduktCecha cp1 = new ProduktCecha() {ProduktId = 1, CechaId = 1 };
            ProduktCecha cp2 = new ProduktCecha() {ProduktId = 2, CechaId = 2 };
            ProduktCecha cp3 = new ProduktCecha() { ProduktId = 3, CechaId = 3 };


            AkcesoriaZPolWlasnych akcesoriaZPolWlasnych = A.Fake<AkcesoriaZPolWlasnych>();
            akcesoriaZPolWlasnych.CzyKopiowacAkcesoriaWRodzinie = false;
            A.CallTo(() => akcesoriaZPolWlasnych.CechyNaB2B).Returns(ListaCech);
            akcesoriaZPolWlasnych.Pole = "pole_tekst1";
            akcesoriaZPolWlasnych.Separator = ';';
            akcesoriaZPolWlasnych.Atrybuty = new List<string>() { "1" };
            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha> { { cp1.Id, cp1 }, { cp2.Id, cp2 }, { cp3.Id, cp3 } };

            Produkt p1 = new Produkt() { Id = 1, PoleTekst1 = "00052_L", Rodzina = "KAMIZELKA OSTRZEGAWCZA W KOLORZE ŻÓŁTYM Z 2 PASKAMI" };
            Produkt p2 = new Produkt() { Id = 2 };
            Produkt p3 = new Produkt() { Id = 3, PoleTekst1 = "00364_43", Rodzina = "TRZEWIKI ROBOCZE URG 101 KATEGORIA S3 EN20345" };

            Produkt p4 = new Produkt() { Id = 4, PoleTekst1 = "00172", Rodzina = "TRZEWIKI ROBOCZE URG 101 KATEGORIA S3 EN20345" };
            Produkt p5 = new Produkt() { Id = 5, PoleTekst1 = "00350_46", Rodzina = "GUMOWCE WYSOKIE WYKONANE Z PCV W KOLORZE CZARNYM EN20347" };
            Produkt p6 = new Produkt() { Id = 6, PoleTekst1 = "09931_1", Rodzina = "SPODNIE OGRODNICZKI HARPOON AVACORE W KOLORZE SZARYM" };

            List<Produkt> listaWejsciowa = new List<Produkt>() { p1, p2, p3, p4, p5, p6 };
            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>();
            Model.Interfaces.Sync.ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
            
            akcesoriaZPolWlasnych.Przetworz(ref listaWejsciowa, ref produktyTlumaczenia, produktyNaB2B, ref  jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp, ref zamienniki, new Dictionary<long, KategoriaProduktu>(),ref cechy,ref atrybuty);
            Assert.Equal(1, zamienniki.Count);
            Assert.True(zamienniki.Any(x => x.ProduktId == 3 && x.ZamiennikId == 6));
        }
    }
}
