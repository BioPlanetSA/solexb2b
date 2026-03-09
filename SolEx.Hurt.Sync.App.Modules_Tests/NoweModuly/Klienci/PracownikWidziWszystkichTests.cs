using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Klienci
{
    public class PracownikWidziWszystkichTests
    {
        [Fact(DisplayName = "Ustawianie pola widzi wszyskich")]
        public void PrzetworzTets( )
        {
            Klient klient1 = new Klient(123);
            Klient klient2 = new Klient(124);
            Klient klient3 = new Klient(125);

            KategorieKlientowWyszukiwanie klienciHelper = A.Fake<KategorieKlientowWyszukiwanie>();

            PracownikWidziWszystkich modul = new PracownikWidziWszystkich();
            modul.KategorieKlientowWyszukiwanie = klienciHelper;

            //A.CallTo(() =>klienciHelper.CzyKlientMaCeche(klient1, A<List<KategoriaKlienta>>.Ignored,A<List<KlientKategoriaKlienta>>.Ignored, "TESTOWA1")).Returns(true);
            //A.CallTo(() =>klienciHelper.CzyKlientMaCeche(klient1, A<List<KategoriaKlienta>>.Ignored,A<List<KlientKategoriaKlienta>>.Ignored, "TESTOWA2")).Returns(false);
            //A.CallTo(() =>klienciHelper.CzyKlientMaCeche(klient2, A<List<KategoriaKlienta>>.Ignored,A<List<KlientKategoriaKlienta>>.Ignored, "TESTOWA2")).Returns(true);
            //A.CallTo(() =>klienciHelper.CzyKlientMaCeche(klient3, A<List<KategoriaKlienta>>.Ignored,A<List<KlientKategoriaKlienta>>.Ignored, "TESTOWA2")).Returns(false);

            Dictionary<Adres, KlientAdres> adresy = new Dictionary<Adres, KlientAdres>();
            List<KupowaneIlosci> ilosci = new List<KupowaneIlosci>();
            List<Sklep> sklepy = new List<Sklep>();
            List<SklepKategoriaSklepu> sklepylaczniki = new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepykategorie = new List<KategoriaSklepu>();

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();

            Dictionary<long, Klient> slownikKlienci = new Dictionary<long, Klient>();
            slownikKlienci.Add(klient1.Id, klient1);
            slownikKlienci.Add(klient2.Id, klient2);
            slownikKlienci.Add(klient3.Id, klient3);

            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony = new List<Region>();
            modul.GrupaKlienta = "TESTOWA1";
            modul.Widocznosc = Widocznosc.WidziWszyskich;

            modul.Przetworz(ref slownikKlienci, new Dictionary<long, Produkt>(), ref adresy, new List<KategoriaKlienta>(), new List<KlientKategoriaKlienta>(), ref sklepy, ref sklepylaczniki, ref sklepykategorie, ref kraje, ref regiony, ref magazyny, provider);

            Assert.True(slownikKlienci[klient1.Id].WidziWszystkich);
            Assert.True(!slownikKlienci[klient2.Id].WidziWszystkich);
            Assert.True(!slownikKlienci[klient3.Id].WidziWszystkich);

        }
    }
}
