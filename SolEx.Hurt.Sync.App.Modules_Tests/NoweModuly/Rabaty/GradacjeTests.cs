using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty.Tests
{
    public class GradacjeTests
    {
        [Fact()]
        public void GradacjeTest()
        {
            Gradacje g = new Gradacje();
            IDictionary<int, KategoriaKlienta> katkl = new Dictionary<int, KategoriaKlienta>();
            
            Dictionary<int,List<ProduktCecha>> cechyprodukty = new   Dictionary<int,List<ProduktCecha>>();
            cechyprodukty.Add(2, new List<ProduktCecha>(){ new ProduktCecha(){ CechaId = 1,  ProduktId = 1}, new ProduktCecha(){ CechaId = 2,  ProduktId = 2}});
            Dictionary<long, Produkt> produkty = new Dictionary<long, Produkt>();
            produkty.Add(2, new Produkt(2));
            IDictionary<long, Klient> klienci = new Dictionary<long, Klient>();
            g.SeparatorGrupy = "";
            g.SeparatorKolejnychGradacji = "/";
            g.SeparatorWartosci = ":";
           var konfekcje = g.PrzetworzCeche(new Cecha { Id = 2,Symbol = "gradacja_cenowa_euh:4:39 / 9:32 / 29:25 / 49:22 / 1000:19"}, ":",
                ref katkl,"gradacja_cenowa_euh", klienci);
            
           Assert.Equal(5, konfekcje.Count);
           Assert.Equal(4, konfekcje[0].Ilosc);
           Assert.Equal(39, konfekcje[0].RabatKwota);
           Assert.Equal(2, konfekcje[0].CechaId);

           Assert.Equal(9, konfekcje[1].Ilosc);
           Assert.Equal(32, konfekcje[1].RabatKwota);
           Assert.Equal(2, konfekcje[1].CechaId);

           Assert.Equal(29, konfekcje[2].Ilosc);
           Assert.Equal(25, konfekcje[2].RabatKwota);
           Assert.Equal(2, konfekcje[2].CechaId);

           Assert.Equal(49, konfekcje[3].Ilosc);
           Assert.Equal(22, konfekcje[3].RabatKwota);
           Assert.Equal(2, konfekcje[3].CechaId);

           Assert.Equal(1000, konfekcje[4].Ilosc);
           Assert.Equal(19, konfekcje[4].RabatKwota);
           Assert.Equal(2, konfekcje[4].CechaId);
        }

        [Fact()]
        public void GradacjeSzukanieWybranejGradacji()
        {
//            AtrybutGradacji = GRADACJA_CENOWA_PLH
//SeparatorKolejnychGradacji = /
//SeparatorGrupy = 
//SeparatorWartosci = :
//TrybDzialania = Automatyczny
//TypDlaGradacji = GrupyKlientow
//SymbolWaluty = PLN
//GrupaKlientow = WALUTA:PLH
//Komentarz = poprawka walut

            string gradacja =
                "gradacja_cenowa_plh:5:99,00 / 10:89,00 / 30:79,00 / 50:69,00 / 100:65,00 / 200:59,00 / 500:59,00 / 1000:59,00";

            Gradacje g = new Gradacje();
            g.AtrybutGradacji = new List<int>(){1};
            g.SeparatorKolejnychGradacji = "/";
            g.SeparatorWartosci = ":";
            g.TrybDzialania = Gradacje.Tryb.Automatyczny;
            g.ListaRozwijanaWalut = "PLN";
            //g.GrupaKlientow = "WALUTA:PLH".ToLower();
            IDictionary<int, KategoriaKlienta> katkl = new Dictionary<int, KategoriaKlienta>();
            List<Cecha> listacech = new List<Cecha>();
            Dictionary<int, List<ProduktCecha>> cechyprodukty = new Dictionary<int, List<ProduktCecha>>();
            List<ProduktCecha> listacechyprodukty = new List<ProduktCecha>();
            listacechyprodukty.Add(new ProduktCecha(){ CechaId = 1, ProduktId = 100});
            cechyprodukty.Add(1, listacechyprodukty);

            Dictionary<long, Produkt> produkty = new Dictionary<long, Produkt>();
            produkty.Add(100, new Produkt(100){ Kod="fajny produkt" });
            IDictionary<long, Klient> klienci = new Dictionary<long, Klient>();
            g.PrzetworzCeche(new Cecha {Symbol = gradacja, Id = 1}, g.SeparatorWartosci, ref katkl,
                "gradacja_cenowa_plh", klienci);
        }
    }
}
