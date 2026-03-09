using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class KlienciZCechaTests
    {
        [Fact(DisplayName = "Test sprawdzający czy metoda zwraca klientow z okreslona cecha")]
        public void PrzetworzTest()
        {
            string poczatekCechy = "poczatekcechy";
            Dictionary<long, Klient> SlownikKlientow = new Dictionary<long, Klient>();
            Klient k1 = new Klient(null) { Id = 1, Aktywny = true };
            Klient k2 = new Klient(null) { Id = 2, Aktywny = true };
            Klient k3 = new Klient(null) { Id = 3, Aktywny = true };
            Klient k4 = new Klient(null) { Id = 4, Aktywny = true };
            Klient k5 = new Klient(null) { Id = 5, Aktywny = true };
            SlownikKlientow.Add(k1.Id, k1);
            SlownikKlientow.Add(k2.Id, k2);
            SlownikKlientow.Add(k3.Id, k3);
            SlownikKlientow.Add(k4.Id, k4);
            SlownikKlientow.Add(k5.Id, k5);


            var KlienciDostep = A.Fake<KategorieKlientowWyszukiwanie>();
            //A.CallTo(() => KlienciDostep.CzyKlientMaCeche(k1, A<List<KategoriaKlienta>>.Ignored, A<List<KlientKategoriaKlienta>>.Ignored, poczatekCechy)).Returns(true);
            //A.CallTo(() => KlienciDostep.CzyKlientMaCeche(k2, A<List<KategoriaKlienta>>.Ignored, A<List<KlientKategoriaKlienta>>.Ignored, poczatekCechy)).Returns(false);
            //A.CallTo(() => KlienciDostep.CzyKlientMaCeche(k3, A<List<KategoriaKlienta>>.Ignored, A<List<KlientKategoriaKlienta>>.Ignored, poczatekCechy)).Returns(true);
            //A.CallTo(() => KlienciDostep.CzyKlientMaCeche(k4, A<List<KategoriaKlienta>>.Ignored, A<List<KlientKategoriaKlienta>>.Ignored, poczatekCechy)).Returns(false);
            //A.CallTo(() => KlienciDostep.CzyKlientMaCeche(k5, A<List<KategoriaKlienta>>.Ignored, A<List<KlientKategoriaKlienta>>.Ignored, poczatekCechy)).Returns(true);
         
            Dictionary<Adres, KlientAdres> adresyWErp = new Dictionary<Adres, KlientAdres>();
            List<Sklep> sklepy=new List<Sklep>();
            List<SklepKategoriaSklepu> sklpeylaczniki=new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepyKategorie=new List<KategoriaSklepu>();
            List<Kraje> kraje= new List<Kraje>();
            List<Region> regiony=new List<Region>();
            KlienciZCecha kzc = new KlienciZCecha();
            kzc.PoczatekCechy = poczatekCechy;
            kzc.KategorieKlientowWyszukiwanie = KlienciDostep;

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            kzc.Przetworz(ref SlownikKlientow, new Dictionary<long, Produkt>(), ref adresyWErp, new List<KategoriaKlienta>(), new List<KlientKategoriaKlienta>(), ref sklepy, ref sklpeylaczniki, ref sklepyKategorie, ref kraje, ref regiony, ref magazyny, provider);
            List<Klient> aktywni = SlownikKlientow.Values.Where(x => x.Aktywny).ToList();
            List<Klient> nieAktywni = SlownikKlientow.Values.Where(x => !x.Aktywny).ToList();
            
            Assert.True(nieAktywni.Count==2);
            Assert.True(nieAktywni.Contains(k2));
            Assert.True(nieAktywni.Contains(k4));

            Assert.True(aktywni.Count == 3);
            Assert.True(aktywni.Contains(k1));
            Assert.True(aktywni.Contains(k3));
            Assert.True(aktywni.Contains(k5));

        }
    }
}
