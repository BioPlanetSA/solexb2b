using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki.Tests
{
    public class KopiowanieZdjecNaDzieciWRodzinieTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
            ProduktPlik pp1 = new ProduktPlik(){ProduktId = 1, PlikId = 1, Glowny = true};
            ProduktPlik pp2 = new ProduktPlik() { ProduktId = 1, PlikId = 2, Glowny = false};
            ProduktPlik pp3 = new ProduktPlik() { ProduktId = 1, PlikId = 3,Glowny=false };
            ProduktPlik pp4 = new ProduktPlik() { ProduktId = 2, PlikId = 1 };
            ProduktPlik pp5 = new ProduktPlik() { ProduktId = 3, PlikId = 3 };
            ProduktPlik pp6 = new ProduktPlik() { ProduktId = 3, PlikId = 4 };
            ProduktPlik pp7 = new ProduktPlik() { ProduktId = 5, PlikId = 8, Glowny = true};
            ProduktPlik pp8 = new ProduktPlik() { ProduktId = 5, PlikId = 9, Glowny = false };
            ProduktPlik pp9 = new ProduktPlik() { ProduktId = 5, PlikId = 10, Glowny = false };
            List<ProduktPlik> plikiLokalnePowiazania = new List<ProduktPlik>(){pp1,pp2,pp3,pp4,pp5,pp6,pp7,pp8,pp9};
            KopiowanieZdjecNaDzieciWRodzinie kop = new KopiowanieZdjecNaDzieciWRodzinie();
            Produkt p1 = new Produkt() { Id = 1, Rodzina = "aaa" };
            Produkt p2 = new Produkt() { Id = 2, Rodzina = "aaa" };
            Produkt p3 = new Produkt() { Id = 3, Rodzina = "aaa" };
            Produkt p4 = new Produkt() { Id = 4, Rodzina = "aaa" };
            Produkt p5 = new Produkt() { Id = 5, Rodzina = "bbb" };
            Produkt p6 = new Produkt() { Id = 6, Rodzina = "bbb" };

            //brak 4 i 6

            IDictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            produktyNaB2B.Add(p1.Id, p1);
            produktyNaB2B.Add(p2.Id, p2);
            produktyNaB2B.Add(p3.Id, p3);
            produktyNaB2B.Add(p4.Id, p4);
            produktyNaB2B.Add(p5.Id, p5);
            produktyNaB2B.Add(p6.Id, p6);
            List<Plik> plikiLokalne = new List<Plik>();
            List<Cecha> cechyB2B = new List<Cecha>();
            List<KategoriaProduktu> kategorieB2B = new List<KategoriaProduktu>();
            List<Klient> klienciB2B = new List<Klient>();
            kop.Przetworz(produktyNaB2B, ref plikiLokalnePowiazania, ref plikiLokalne, null, ref cechyB2B, ref kategorieB2B,ref klienciB2B);

            Assert.True(plikiLokalnePowiazania.Count==15);
            Assert.True(plikiLokalnePowiazania.FirstOrDefault(x => x.ProduktId == 4 && x.PlikId == 1 && x.Glowny) != null);
            Assert.True(plikiLokalnePowiazania.FirstOrDefault(x=>x.ProduktId==4 && x.PlikId==2 && !x.Glowny)!=null);
            Assert.True(plikiLokalnePowiazania.FirstOrDefault(x => x.ProduktId == 4 && x.PlikId == 3 && !x.Glowny) != null);

            Assert.True(plikiLokalnePowiazania.FirstOrDefault(x => x.ProduktId == 6 && x.PlikId == 8 && x.Glowny) != null);
            Assert.True(plikiLokalnePowiazania.FirstOrDefault(x => x.ProduktId == 6 && x.PlikId == 9 && !x.Glowny) != null);
            Assert.True(plikiLokalnePowiazania.FirstOrDefault(x => x.ProduktId == 6 && x.PlikId == 10 && !x.Glowny) != null);
        }
    }
}
