using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class UsuwanieSlowNadmiarowychTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
           
            UsuwanieSlowNadmiarowych usuwanie = new UsuwanieSlowNadmiarowych();

            

            Produkt p1 = new Produkt() { Id = 1, Kod = "00052_L", Rodzina = "Super bluzka czerwona L" };
            Produkt p2 = new Produkt() { Id = 2, Kod = "00065", Rodzina = "Super bluzka czerwona XL" };
            Produkt p3 = new Produkt() { Id = 3, Kod = "00364_43", Rodzina = "Super bluzka czerwonaX" };
            Produkt p4 = new Produkt() { Id = 4, Kod = "00172", Rodzina = "Super bluzka XL czerwona" };
            Produkt p5 = new Produkt() { Id = 5, Kod = "00350_46", Rodzina = "Super bluzka XL S czerwona L" };
            Produkt p6 = new Produkt() { Id = 6, Kod = "09931_1", Rodzina = "XSuper bluzka czerwona XL" };
            Produkt p7 = new Produkt() { Id = 7, Kod = "00350_461", Rodzina = "Super bluzkaM XL S czerwona XL" };

            List<Produkt> listaWejsciowa = new List<Produkt>(){p1,p2,p3,p4,p5,p6,p7};
            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha>();
            List<Cecha> listaCech = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            List<ProduktKategoria> lacznikiKategorii =new  List<ProduktKategoria>();
            Model.Interfaces.Sync.ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
            
            usuwanie.Przetworz(ref listaWejsciowa, ref produktyTlumaczenia, produktyNaB2B, ref  jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp,
                ref zamienniki, new Dictionary<long, KategoriaProduktu>(),ref listaCech, ref atrybuty);

            Assert.True(listaWejsciowa.First(x => x.Id == 1).Rodzina.Equals("Super bluzka czerwona"));
            Assert.True(listaWejsciowa.First(x => x.Id == 2).Rodzina.Equals("Super bluzka czerwona"));
            Assert.True(listaWejsciowa.First(x => x.Id == 3).Rodzina.Equals("Super bluzka czerwonaX"));
            Assert.True(listaWejsciowa.First(x => x.Id == 4).Rodzina.Equals("Super bluzka czerwona"));
            Assert.True(listaWejsciowa.First(x => x.Id == 5).Rodzina.Equals("Super bluzka czerwona"));
            Assert.True(listaWejsciowa.First(x => x.Id == 6).Rodzina.Equals("XSuper bluzka czerwona"));
            Assert.True(listaWejsciowa.First(x => x.Id == 7).Rodzina.Equals("Super bluzkaM czerwona"));
        }
    }
}
