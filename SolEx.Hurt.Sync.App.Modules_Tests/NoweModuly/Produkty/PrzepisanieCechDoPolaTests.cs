using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class PrzepisanieCechDoPolaTests
    {
        [Fact()]
        public void PrzepisanieCechDoPolaTest()
        {
           Produkt produkt = new Produkt();
           produkt.Id = 1;
            produkt.Kod = "S0784450";

            List<Produkt> listaproduktow = new List<Produkt>(){produkt};

            Atrybut atrybutKB = new Atrybut();
            atrybutKB.Nazwa = "Symbol KB";
            atrybutKB.Id = 1;

            Atrybut atrybutGB = new Atrybut();
            atrybutGB.Nazwa = "symbolGB";
            atrybutGB.Id = 2;

            List<Atrybut> atrybutyGB = new List<Atrybut>() {atrybutGB};
            List<Atrybut> atrybutyKB = new List<Atrybut>() { atrybutKB };

            Cecha cechaKB = new Cecha();
            cechaKB.Nazwa = "40K028X";
            cechaKB.Symbol = "symbol kb:40K028X";
            cechaKB.AtrybutId = atrybutKB.Id;
            cechaKB.Id = 100;
            Dictionary<long, Cecha> listaCech = new Dictionary<long, Cecha>() {{ cechaKB.Id,cechaKB }};

            ProduktCecha cechyProdukty = new ProduktCecha();
            cechyProdukty.ProduktId = produkt.Id;
            cechyProdukty.CechaId = cechaKB.Id;

            List<ProduktCecha> lacznikicech = new List<ProduktCecha>() { cechyProdukty };


            List<Tlumaczenie> slowniki = new List<Tlumaczenie>(0);
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>(0);
            Dictionary<long, ProduktCecha> laczniki = new Dictionary<long, ProduktCecha>(0);
           List< ProduktKategoria> lacznikikategorii = new List< ProduktKategoria>(0);
            List<ProduktUkryty> produktyukryte = new List<ProduktUkryty>(0);

            //pierwszy test powinien skopiować do pola produktu cechę
            PrzepisanieCechDoPola pcdp = new PrzepisanieCechDoPola();
            pcdp.Pola = new List<string>() { "pole_tekst1" };
            pcdp.PrzepisujJesliWartoscNieJestPusta = true;
            pcdp.Atrybuty = "Symbol KB";
            pcdp.Przetworz(ref listaproduktow, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki, ref laczniki, ref lacznikikategorii, null, ref produktyukryte, listaCech);

            Assert.Equal(produkt.PoleTekst1, cechaKB.Nazwa);

            //drugi test nie znajdzie odpowiedniej cechy i nie nadpisze poprzedniej wartości pola w produkcie
            PrzepisanieCechDoPola pcdp2 = new PrzepisanieCechDoPola();
            pcdp2.Pola = new List<string>() { "pole_tekst1" };
            pcdp2.PrzepisujJesliWartoscNieJestPusta = true;
            pcdp2.Atrybuty = "symbolGB";
            pcdp2.Przetworz(ref listaproduktow, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki, ref laczniki, ref lacznikikategorii, null, ref produktyukryte, new Dictionary<long, Cecha>());

            Assert.Equal(produkt.PoleTekst1, cechaKB.Nazwa);

            //tutaj zmieniamy zasady gry i pole w produkcie będzie nadpisane przez drugi moduł
            pcdp2.PrzepisujJesliWartoscNieJestPusta = false;
            pcdp2.Przetworz(ref listaproduktow, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki, ref laczniki, ref lacznikikategorii, null, ref produktyukryte, new Dictionary<long, Cecha>());
            Assert.Equal(produkt.PoleTekst1, string.Empty);
        }

        [Fact()]
        public void PrzetworzTest()
        {
            Produkt produkt = new Produkt();
            produkt.Id = 1;
            produkt.Kod = "S0784450";

            List<Produkt> listaproduktow = new List<Produkt>() { produkt };

            Atrybut atrybutBoolowski1 = new Atrybut();
            atrybutBoolowski1.Nazwa = "atrybutBoolowski1";
            atrybutBoolowski1.Id = 1;

            Atrybut atrybutBoolowski2 = new Atrybut();
            atrybutBoolowski2.Nazwa = "atrybutBoolowski2";
            atrybutBoolowski2.Id = 2;

            List<Atrybut> atrybutyGB = new List<Atrybut>() { atrybutBoolowski1 };
            List<Atrybut> atrybutyKB = new List<Atrybut>() { atrybutBoolowski2 };

            Cecha cechaTak = new Cecha();
            cechaTak.Nazwa = "TAK";
            cechaTak.Symbol = "atrybutBoolowski1:tak";
            cechaTak.AtrybutId = atrybutBoolowski1.Id;
            cechaTak.Id = 100;

            Cecha cechaNie = new Cecha();
            cechaNie.Nazwa = "NIE";
            cechaNie.Symbol = "atrybutBoolowski2:nie";
            cechaNie.AtrybutId = atrybutBoolowski2.Id;
            cechaNie.Id = 101;
            Dictionary<long, Cecha> listaCech = new Dictionary<long, Cecha>() {{cechaTak.Id, cechaTak}, {cechaNie.Id,cechaNie }};

            ProduktCecha cechyProdukty = new ProduktCecha();
            cechyProdukty.ProduktId = produkt.Id;
            cechyProdukty.CechaId = cechaTak.Id;


            ProduktCecha cechyProdukty2 = new ProduktCecha();
            cechyProdukty2.ProduktId = produkt.Id;
            cechyProdukty2.CechaId = cechaNie.Id;

            List<ProduktCecha> lacznikicech = new List<ProduktCecha>() { cechyProdukty, cechyProdukty2 };


            List<Tlumaczenie> slowniki = new List<Tlumaczenie>(0);
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>(0);
            Dictionary<long, ProduktCecha> laczniki = new Dictionary<long, ProduktCecha>(0);
           List< ProduktKategoria> lacznikikategorii = new List< ProduktKategoria>(0);
            List<ProduktUkryty> produktyukryte = new List<ProduktUkryty>(0);

            //pierwszy test powinien skopiować do pola produktu cechę
            PrzepisanieCechDoPola pcdp = new PrzepisanieCechDoPola();
            pcdp.Pola = new List<string>() { "NiePodlegaRabatowaniu" };
            pcdp.PrzepisujJesliWartoscNieJestPusta = true;
            pcdp.Atrybuty = "atrybutBoolowski1";
            pcdp.Przetworz(ref listaproduktow, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki, ref laczniki, ref lacznikikategorii, null,
                ref produktyukryte, listaCech);

            Assert.True(produkt.NiePodlegaRabatowaniu);

            //drugi test nie znajdzie odpowiedniej cechy i nie nadpisze poprzedniej wartości pola w produkcie
            PrzepisanieCechDoPola pcdp2 = new PrzepisanieCechDoPola();
            pcdp2.Pola = new List<string>() { "NiePodlegaRabatowaniu" };
            pcdp2.PrzepisujJesliWartoscNieJestPusta = true;
            pcdp2.Atrybuty = "atrybutBoolowski2";
            pcdp2.Przetworz(ref listaproduktow, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki, ref laczniki, ref lacznikikategorii, null,
                ref produktyukryte, listaCech);

            Assert.False(produkt.NiePodlegaRabatowaniu);
        }
    }
}
