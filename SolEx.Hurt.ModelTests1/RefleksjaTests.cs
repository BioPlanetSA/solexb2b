using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FastMember;
using ServiceStack.ServiceInterface.ServiceModel;
using SolEx.Hurt.Core;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using Xunit;
using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.ModelTests1
{
    public class RefleksjaTests
    {
        [Fact(DisplayName = "Test wydajności refleksji")]
        public void SzybkoscRefleksjiTest()
        {
            List<Klient> klienci = new List<Klient>(1000000);
            for (int i = 0; i < 1000000; ++i)
            {
                klienci.Add( new Klient() );
            }

            PropertyInfo proeprtisDoUstawienia = typeof(Klient).Properties().Values.ToList().FirstOrDefault(x => x.Name == "Nazwa");

            Stopwatch sw = new Stopwatch();

            sw.Start();
            foreach (Klient klient in klienci)
            {
                proeprtisDoUstawienia.SetValue(klient, "testowa nazwa seter");
            }
            sw.Stop();

            var czasRefleksji = sw.Elapsed.Milliseconds;

            var akcesor  = TypeAccessor.Create(typeof(Klient));

            sw.Reset();
            sw.Start();
            foreach (Klient klient in klienci)
            {
                akcesor[klient, proeprtisDoUstawienia.Name] = "testowa nazwa seter";
            }
            sw.Stop();
            var czasFastMembera = sw.Elapsed.Milliseconds;

            Assert.True(czasRefleksji < czasFastMembera, $"Czas refleksji MS: {czasRefleksji}ms, czas fastmembera: {czasFastMembera}ms.");
        }

        [Fact(DisplayName = "Pobieranie propertisów obiektu Zamowienie - problem z propertisem StatusId")]
        public void PobieraniePropertisowZamowieniaTest()
        {
            var proeprtisy = typeof(Zamowienie).Properties();
            Assert.True(proeprtisy.Count > 0, "Brak propertisow");
            Assert.True(proeprtisy.ContainsKey("StatusId"), "Nie ma StatusId");
            Assert.True(proeprtisy["StatusId"].PropertyType == typeof(StatusImportuZamowieniaDoErp), "zły typ");
            Assert.True(proeprtisy.ContainsKey("NazwaKlienta"), "Propertis z bazowej klasy");

            proeprtisy = typeof(ProduktKlienta).Properties();
            Assert.True(proeprtisy.ContainsKey("Nazwa"));

            //proeprtisy = typeof(SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.ListaProduktow).Properties();
            //Assert.True(proeprtisy.ContainsKey("Nazwa"));
        }

        [Fact(DisplayName = "Spradzanie refleksji dla typow anonimowych")]
        public void PobieraniePropertisowTypuAnonimowegoTest()
        {
            Zamowienie zam = new ZamowieniaImport();
            object o = new {zam.Adres, zam.Id, zam.BladKomunikat};
            var proeprtisy = o.Properties();
            Assert.True(proeprtisy.ContainsKey("Adres"));
            Assert.True(proeprtisy.ContainsKey("Id"));
            Assert.True(proeprtisy.ContainsKey("BladKomunikat"));
        }


        [Fact(DisplayName = "Czy ustawia poprawnie propertisy Fastreflect")]
        public void PobieranieWszystkichPropertisow()
        {
            var proeprtisy = typeof(Klient).Properties();
            Klient k = new Klient();
            var akcesor = TypeAccessor.Create(typeof(Klient));

            foreach (var p in proeprtisy)
            {
                object pobranie = null;
                if (p.Value.CanRead)
                {
                    pobranie = akcesor[k, p.Key];
                }

                if (p.Value.CanWrite)
                {
                    akcesor[k, p.Key] = pobranie;
                }
            }
        }
    }
}
