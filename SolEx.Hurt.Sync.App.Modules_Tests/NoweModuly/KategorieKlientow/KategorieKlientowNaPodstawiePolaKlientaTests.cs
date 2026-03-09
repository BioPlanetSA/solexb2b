using System;
using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Model;
using Xunit;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow.Tests
{
    public class KategorieKlientowNaPodstawiePolaKlientaTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
            string nazwakat = "test";
            int klientid = 1;
            var modul = A.Fake<KategorieKlientowNaPodstawiePolaKlienta>();
            modul.NazwaGrupy = "test";
      
            modul.PrzypisacPuste = true;
            modul.PoleZrodlowe = "pole_tekst1";
            List<Klient> dane = new List<Klient>();
            dane.Add(new Klient() { Id = klientid, PoleTekst1 = nazwakat });

            A.CallTo(() => modul.WszyscyKlienci()).Returns(dane);
            List<KategoriaKlienta> kk=new List<KategoriaKlienta>();
            List<KlientKategoriaKlienta> laczniki=new List<KlientKategoriaKlienta>();
            modul.Przetworz(ref kk,ref laczniki);
            bool ilosc = kk.Count == 1;
            Assert.True(ilosc, "Ilosc kategorii otrzymana "+kk.Count);
            bool wynik = kk[0].Nazwa == nazwakat;

            int id = kk[0].Id;
            Assert.True(wynik,"Nazwa kategorii");

            bool lacznik = laczniki[0].KategoriaKlientaId == id && laczniki[0].KlientId == klientid;
            Assert.True(lacznik, "Łącznik kategorii");

        }
    }
}
