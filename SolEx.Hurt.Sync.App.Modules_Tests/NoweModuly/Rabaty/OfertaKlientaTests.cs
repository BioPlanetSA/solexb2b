using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using SolEx.Hurt.Model;
using Xunit;
using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class OfertaKlientaTests
    {
        [Fact(DisplayName = "Test modulu pokazujacego/ukrywajacego towar dla wspolnej cechy klienta oraz produktu")]
        public void PrzetworzTest()
        {
            ShowTest("kategorie", "test", "kategorie:test", ":", ":", 2, 2, true, 2);
            ShowTest("kategorie", "test", "kategorie:test", ":", ":", 0, 2, true, null);    //wymagamy opiekuna ale klient nie ma opiekuna
            ShowTest("kategorie", "test", "kategorie:test", ":", ":", 2, 2, false, null);

            ShowTest("kategorie", "test", "kategorie:t3est", ":", ":", 0, 2, true, 2);
            ShowTest("kategorie", "test", "kategorie:t3est", ":", ":", 0, 2, true, 2);
            ShowTest("kategorie", "test", "kategorie:t3est", ":", ":", 0, 2, false, null);
        }

        void ShowTest(string kategoria_klientow_grupa, string kategoria_klientow_nazwa, string symbol, string separatorAtrybutow, string separatorGrup, int wartoscOczekiwana, int iloscKlientow, bool TylkoOpiekun, int? id_przedstawiciela )
        {
            IDictionary<long, Klient> klienci = new Dictionary<long, Klient>();
            Dictionary<long, Produkt> slownikProduktow = new Dictionary<long, Produkt>();
            Dictionary<long, ProduktCecha> cechyProdukty = new Dictionary<long,ProduktCecha>();
            IDictionary<int, KategoriaKlienta> kategorieKlientow = new Dictionary<int, KategoriaKlienta>();
            IDictionary<long, KlientKategoriaKlienta> klienciKategorie = new Dictionary<long, KlientKategoriaKlienta>();
            List<Cecha> listaCech = new List<Cecha>();

            for (int i = 0; i < iloscKlientow; i++)
            {
                KategoriaKlienta kategoriaKlienta = new KategoriaKlienta() { Id = i+1, Grupa = kategoria_klientow_grupa, Nazwa = kategoria_klientow_nazwa };
                KlientKategoriaKlienta klientKategorie = new KlientKategoriaKlienta() { KlientId = i + 1, KategoriaKlientaId = i+1 };
                ProduktCecha cProduktu = new ProduktCecha() { CechaId = i + 1, ProduktId = i + 1 };
                Produkt produkt = new Produkt() { Id = i + 1 };
                Cecha cecha = new Cecha() { Id = i + 1, Symbol = symbol };
                Klient klient = new Klient(i + 1) { Email = "email" + i + "@email.com", PrzedstawicielId = id_przedstawiciela , Aktywny = true};

                klienci.Add(i+1, klient);
                kategorieKlientow.Add(i + 1, kategoriaKlienta);
                slownikProduktow.Add(i + 1, produkt);
                cechyProdukty.Add(cProduktu.Id, cProduktu);
                klienciKategorie.Add(klientKategorie.Id, klientKategorie);
                listaCech.Add(cecha);
            }


            List<ProduktUkryty> produktyUkryte = new List<ProduktUkryty>();
            List<Rabat> rabatyNaB2B = new List<Rabat>();
            Dictionary<long,Konfekcje> konfekcjaNaB2B = new Dictionary<long, Konfekcje>();

            OfertaKlienta modul = new OfertaKlienta();

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.SeparatorAtrybutowWCechach).Returns(separatorAtrybutow.ToCharArray());
            A.CallTo(() => config.SeparatorGrupKlientow).Returns(separatorGrup.ToArray());

            modul.Konfiguracja = config;
            modul.PoczatekCechy = kategoria_klientow_grupa;
            modul.TylkoZPrzedstawicielem = TylkoOpiekun;

            KategorieKlientowWyszukiwanie.PobierzInstancje.Konfiguracja = config;

            modul.Przetworz(ref rabatyNaB2B, ref produktyUkryte, ref konfekcjaNaB2B, klienci, slownikProduktow, new List<PoziomCenowy>(), listaCech, cechyProdukty, new Dictionary<long, KategoriaProduktu>(), new List<ProduktKategoria>(), ref kategorieKlientow, ref  klienciKategorie);

            Assert.True(produktyUkryte.Count == wartoscOczekiwana, string.Format("Oczekiwano {0}, otrzymano{1}, ", wartoscOczekiwana, produktyUkryte.Count));
            Assert.True(produktyUkryte.Any(x => x.KlientZrodloId == 1 && x.CechaProduktuId == 1) || produktyUkryte.Count == 0);

        }
        
    }
}
