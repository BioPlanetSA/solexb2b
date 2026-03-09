using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Modele;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class DodawaniePracownikowTests
    {
        [Fact(DisplayName = "Test sprawdzajacy porawne dodawnie roli klientom na podstawie domeny")]
        public void PrzetworzTest()
        {
            string domena = "gmail.com";
            RoleType rola = RoleType.Przedstawiciel;


            Dictionary<long, Klient> listaWejsciowa = new Dictionary<long, Klient>();
            var k1 = A.Fake<Klient>();
            k1.Id = 1;
            k1.Email = "klient1@gmail.com";
            A.CallTo(() => k1.Role).Returns(new HashSet<RoleType> { RoleType.Klient});
            var k2 = A.Fake<Klient>();
            k2.Id = 2;
            k2.Email = "klient2@o2.com";
            A.CallTo(() => k2.Role).Returns(new HashSet<RoleType> { RoleType.Klient });
            var k3 = A.Fake<Klient>();
            k3.Id = 3;
            k3.Email = "klient3@gmail.com";
            A.CallTo(() => k3.Role).Returns(new HashSet<RoleType> { RoleType.Przedstawiciel });

            listaWejsciowa.Add(k1.Id, k1);
            listaWejsciowa.Add(k2.Id, k2);
            listaWejsciowa.Add(k3.Id, k3);

            List<KupowaneIlosci> iloscii = new List<KupowaneIlosci>();
            Dictionary<Adres, KlientAdres> adresyWErp = new Dictionary<Adres, KlientAdres>();
            List<Sklep> sklepy= new List<Sklep>();
            List<SklepKategoriaSklepu> sklpeylaczniki=new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepyKategorie = new List<KategoriaSklepu>();
            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony = new List<Region>();

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            DodawaniePracownikow dp = new DodawaniePracownikow();
            dp.Domena = domena;
            dp.RolaKlienta = rola;
            dp.Przetworz(ref listaWejsciowa, new Dictionary<long, Produkt>(),ref adresyWErp,new List<KategoriaKlienta>(),new List<KlientKategoriaKlienta>(),ref sklepy, ref sklpeylaczniki, ref sklepyKategorie, ref kraje, ref regiony , ref magazyny, provider);
            Assert.True(listaWejsciowa[1].Role.Count==2);
            Assert.True(listaWejsciowa[2].Role.Count == 1);
            Assert.True(listaWejsciowa[3].Role.Count == 1);


        }
    }
}
