using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty.Tests
{
    public class RabatyImpelTests
    {
        [Fact(DisplayName = "Impel - test sprawdzający poprawność danych z csv z modułu  do rabatów")]
        public void PrzetworzPobranaWartoscTest()
        {
            RabatyImpel modul = new RabatyImpel();

            ProduktUkryty pu = new ProduktUkryty();
            object pola = new { klient_zrodlo_id = pu.KlientZrodloId, produkt_zrodlo_id = pu.ProduktZrodloId, pu.Tryb };

            Dictionary<long, Rabat> rabaty = new Dictionary<long, Rabat>();
            Dictionary<string, ProduktUkryty> produktyukryte = new Dictionary<string, ProduktUkryty>();
            Dictionary<long, Konfekcje> konfekcje = new Dictionary<long, Konfekcje>();

            string przypadek1 = "2,26;12^3%";
            string przypadek2 = "6,99;120^5,49 zł";
            string przypadek3 = "20%";
            string przypadek4 = "6,99";
            string przypadek5 = "20%;24^ 25%";
            string przypadek6 = "70,00 zł";


            Klient k1 = new Klient(1);
            Produkt p1 = new Produkt(11);

            Klient k2 = new Klient(2);
            Produkt p2 = new Produkt(22);

            Klient k3 = new Klient(3);
            Produkt p3 = new Produkt(33);

            Klient k4 = new Klient(4);
            Produkt p4 = new Produkt(44);

            Klient k5 = new Klient(5);
            Produkt p5 = new Produkt(55);

            Klient k6 = new Klient(6);
            Produkt p6 = new Produkt(6);

            modul.PrzetworzPobranaWartosc(przypadek1, k1, p1, rabaty, produktyukryte, konfekcje, 10M, pola);

            var rabatyk1 = rabaty.Where(a => a.Value.KlientId == k1.Id).ToList();
            Assert.Equal(1, rabatyk1.Count);
            Assert.Equal(2.26M, rabatyk1.First().Value.Wartosc1);
            Assert.Equal(RabatTyp.Zaawansowany, rabatyk1.First().Value.TypRabatu);
            Assert.Equal(RabatSposob.StalaCena, rabatyk1.First().Value.TypWartosci);

            var konfekcjek1 = konfekcje.Where(a => a.Value.KlientId == k1.Id).ToList();
            Assert.Equal(1, konfekcjek1.Count);
            Assert.Equal(12, konfekcjek1.First().Value.Ilosc);
            Assert.Equal(3, konfekcjek1.First().Value.Rabat);

            var produktyukrytek1 = produktyukryte.Where(a => a.Value.KlientZrodloId == k1.Id).ToList();
            Assert.Equal(1, produktyukrytek1.Count);
            Assert.Equal(p1.Id, produktyukrytek1.First().Value.ProduktZrodloId);
            Assert.Equal(KatalogKlientaTypy.Dostepne, produktyukrytek1.First().Value.Tryb);

            modul.PrzetworzPobranaWartosc(przypadek2, k2, p2, rabaty, produktyukryte, konfekcje, 10M, pola);

            var rabatyk2 = rabaty.Where(a => a.Value.KlientId == k2.Id).ToList();
            Assert.Equal(1, rabatyk2.Count);
            Assert.Equal(6.99M, rabatyk2.First().Value.Wartosc1);
            Assert.Equal(RabatTyp.Zaawansowany, rabatyk2.First().Value.TypRabatu);
            Assert.Equal(RabatSposob.StalaCena, rabatyk2.First().Value.TypWartosci);

            var konfekcjek2 = konfekcje.Where(a => a.Value.KlientId == k2.Id).ToList();
            Assert.Equal(1, konfekcjek2.Count);
            Assert.Equal(120, konfekcjek2.First().Value.Ilosc);
            Assert.Equal(5.49M, konfekcjek2.First().Value.RabatKwota);

            var produktyukrytek2 = produktyukryte.Where(a => a.Value.KlientZrodloId == k2.Id).ToList();
            Assert.Equal(1, produktyukrytek2.Count);
            Assert.Equal(p2.Id, produktyukrytek2.First().Value.ProduktZrodloId);
            Assert.Equal(KatalogKlientaTypy.Dostepne, produktyukrytek2.First().Value.Tryb);

            modul.PrzetworzPobranaWartosc(przypadek3, k3, p3, rabaty, produktyukryte, konfekcje, 10M, pola);

            var rabatyk3 = rabaty.Where(a => a.Value.KlientId == k3.Id).ToList();
            Assert.Equal(1, rabatyk3.Count);
            Assert.Equal(20, rabatyk3.First().Value.Wartosc1);
            Assert.Equal(RabatTyp.Zaawansowany, rabatyk3.First().Value.TypRabatu);
            Assert.Equal(RabatSposob.Procentowy, rabatyk3.First().Value.TypWartosci);

            var konfekcjek3 = konfekcje.Where(a => a.Value.KlientId == k3.Id).ToList();
            Assert.Equal(0, konfekcjek3.Count);

            var produktyukrytek3 = produktyukryte.Where(a => a.Value.KlientZrodloId == k3.Id).ToList();
            Assert.Equal(1, produktyukrytek3.Count);
            Assert.Equal(p3.Id, produktyukrytek3.First().Value.ProduktZrodloId);
            Assert.Equal(KatalogKlientaTypy.Dostepne, produktyukrytek3.First().Value.Tryb);

            modul.PrzetworzPobranaWartosc(przypadek4, k4, p4, rabaty, produktyukryte, konfekcje, 10M, pola);

            var rabatyk4 = rabaty.Where(a => a.Value.KlientId == k4.Id).ToList();
            Assert.Equal(1, rabatyk4.Count);
            Assert.Equal(6.99M, rabatyk4.First().Value.Wartosc1);
            Assert.Equal(RabatTyp.Zaawansowany, rabatyk4.First().Value.TypRabatu);
            Assert.Equal(RabatSposob.StalaCena, rabatyk4.First().Value.TypWartosci);

            var konfekcjek4 = konfekcje.Where(a => a.Value.KlientId == k4.Id).ToList();
            Assert.Equal(0, konfekcjek4.Count);

            var produktyukrytek4 = produktyukryte.Where(a => a.Value.KlientZrodloId == k4.Id).ToList();
            Assert.Equal(1, produktyukrytek4.Count);
            Assert.Equal(p4.Id, produktyukrytek4.First().Value.ProduktZrodloId);
            Assert.Equal(KatalogKlientaTypy.Dostepne, produktyukrytek4.First().Value.Tryb);

            modul.PrzetworzPobranaWartosc(przypadek5, k5, p5, rabaty, produktyukryte, konfekcje, 10M, pola);

            var rabatyk5 = rabaty.Where(a => a.Value.KlientId == k5.Id).ToList();
            Assert.Equal(1, rabatyk5.Count);
            Assert.Equal(20M, rabatyk5.First().Value.Wartosc1);
            Assert.Equal(RabatTyp.Zaawansowany, rabatyk5.First().Value.TypRabatu);
            Assert.Equal(RabatSposob.Procentowy, rabatyk5.First().Value.TypWartosci);

            var konfekcjek5 = konfekcje.Where(a => a.Value.KlientId == k5.Id).ToList();
            Assert.Equal(1, konfekcjek5.Count);
            Assert.Equal(24, konfekcjek5.First().Value.Ilosc);
            Assert.Equal(7.5M, konfekcjek5.First().Value.RabatKwota);

            var produktyukrytek5 = produktyukryte.Where(a => a.Value.KlientZrodloId == k5.Id).ToList();
            Assert.Equal(1, produktyukrytek5.Count);
            Assert.Equal(p5.Id, produktyukrytek5.First().Value.ProduktZrodloId);
            Assert.Equal(KatalogKlientaTypy.Dostepne, produktyukrytek5.First().Value.Tryb);

            modul.PrzetworzPobranaWartosc(przypadek6, k6, p6, rabaty, produktyukryte, konfekcje, 10M, pola);

            var rabatyk6 = rabaty.Where(a => a.Value.KlientId == k6.Id).ToList();
            Assert.Equal(1, rabatyk6.Count);
            Assert.Equal(70M, rabatyk6.First().Value.Wartosc1);
            Assert.Equal(RabatTyp.Zaawansowany, rabatyk6.First().Value.TypRabatu);
            Assert.Equal(RabatSposob.StalaCena, rabatyk6.First().Value.TypWartosci);

        }
    }
}
