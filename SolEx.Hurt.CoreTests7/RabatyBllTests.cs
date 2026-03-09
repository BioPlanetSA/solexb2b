using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.Tests
{
    public class RabatyBllTests
    {
        private RabatyBll rabaty;
        private IProduktKlienta produkt;
        private IFlatCeny flatCena;
        private IKlient klient;
        private List<Konfekcje> listaKonfekcji;

        private void ustawDaneDlaGradacji(decimal ileJuzKupione, decimal OZWymaganyKrok = 0.2m)
        {
            produkt = A.Fake<IProduktKlienta>();
            klient = A.Fake<IKlient>();
            flatCena = A.Fake<IFlatCeny>();

            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();

            rabaty = new RabatyBll(calosc);

            Waluta waluta = A.Fake<Waluta>();
            waluta.Id = 6;
            waluta.WalutaB2b = "PLN";

            flatCena.WalutaId = waluta.Id;
            flatCena.CenaNettoBezGradacji = 666;

            A.CallTo(() => calosc.KupowaneIlosciBLL.SumaKupowanychIlosci(0, null, null, DateTime.MinValue)).WithAnyArguments().Returns(ileJuzKupione);
          //  A.CallTo(() => calosc.Konfiguracja.GradacjeOdKiedyLiczyc).Returns(DateTime.MinValue);
            A.CallTo(() => calosc.Konfiguracja.SlownikWalut).Returns(new Dictionary<long, Waluta> { { waluta.Id, waluta } });
            A.CallTo(() => calosc.Konfiguracja.ZCzegoLiczycGradacje).Returns(new HashSet<ZCzegoLiczycGradacje> { ZCzegoLiczycGradacje.Faktura });

            listaKonfekcji = new List<Konfekcje>
            {
                new Konfekcje {Ilosc = 0, ProduktId = produkt.Id}, 
                new Konfekcje {Ilosc = 10, ProduktId = produkt.Id, RabatKwota = 100},
                new Konfekcje {Ilosc = 20, ProduktId = produkt.Id, RabatKwota = 800},   //ta konfeckaj jest za duza cena wiec mamy ja przeskoczyc
                new Konfekcje {Ilosc = 30, ProduktId = produkt.Id, RabatKwota = 70},
                new Konfekcje {Ilosc = 100, ProduktId = produkt.Id, RabatKwota = 50},
            };

            A.CallTo(() => produkt.GradacjePosortowane).Returns(listaKonfekcji);
            A.CallTo(() => produkt.JednostkaPodstawowa.PoprawanaIloscZakupu(null)).Returns(OZWymaganyKrok);   //minimalna ilosc sprzedazy po to zeby gradacje sprwadzic czy dobrze policzy sie
        }

        [Fact(DisplayName = "Test jesli nic nie kupione - aktywna powinna byc pierwsza gradacja")]
        public void WyliczoneGradacjeTest_JakNieKupioneWOgole()
        {
            ustawDaneDlaGradacji(0, 0.2m); //maksymalnie dobra cena - kupujemy wiecej niz trzeba

            decimal zakupionaDotychczasIlosc;
            List<GradacjaWidok> wynikoweGradacje = rabaty.WyliczoneGradacje(produkt, klient, flatCena, out zakupionaDotychczasIlosc);

            Assert.True(wynikoweGradacje != null);
            Assert.True(wynikoweGradacje.First().AktualnaCena);
            Assert.False(wynikoweGradacje.First().Spelniny);
            Assert.True(wynikoweGradacje.First().CenaNetto == flatCena.CenaNettoBezGradacji);
            Assert.False(wynikoweGradacje.First().PrzedzialOd == 0);
        }

        [Fact(DisplayName = "Test jesli poziomy gradacji maja te same ceny")]
        public void WyliczoneGradacjeTest_TeSameIlosciGradacji()
        {
            ustawDaneDlaGradacji(0, 0.2m); //maksymalnie dobra cena - kupujemy wiecej niz trzeba

            //nowa lista konfekcji
            listaKonfekcji = new List<Konfekcje>
            {
                new Konfekcje {Ilosc = 0, ProduktId = produkt.Id},
                new Konfekcje {Ilosc = 10, ProduktId = produkt.Id, RabatKwota = 100},
                new Konfekcje {Ilosc = 20, ProduktId = produkt.Id, RabatKwota = 100},   //ta konfeckaj jest za duza cena wiec mamy ja przeskoczyc
                new Konfekcje {Ilosc = 30, ProduktId = produkt.Id, RabatKwota = 70},
                new Konfekcje {Ilosc = 100, ProduktId = produkt.Id, RabatKwota = 50},
            };

            A.CallTo(() => produkt.GradacjePosortowane).Returns(listaKonfekcji);

            decimal zakupionaDotychczasIlosc;
            List<GradacjaWidok> wynikoweGradacje = rabaty.WyliczoneGradacje(produkt, klient, flatCena, out zakupionaDotychczasIlosc);

            Assert.True(wynikoweGradacje != null);
            Assert.True(wynikoweGradacje.First().AktualnaCena);
            Assert.False(wynikoweGradacje.First().Spelniny);
            Assert.True(wynikoweGradacje.First().CenaNetto == flatCena.CenaNettoBezGradacji);
            Assert.False(wynikoweGradacje.First().PrzedzialOd == 0);
            Assert.False(wynikoweGradacje.First().PrzedzialDo == 20);

            Assert.False(wynikoweGradacje[1].PrzedzialOd == 20);
            Assert.False(wynikoweGradacje[1].PrzedzialDo == 30);

        }

        [Fact()]
        public void WyliczoneGradacjeTest_MaksymalnaGradacja()
        {
            ustawDaneDlaGradacji(150); //maksymalnie dobra cena - kupujemy wiecej niz trzeba

            decimal zakupionaDotychczasIlosc;
            List<GradacjaWidok> wynikoweGradacje = rabaty.WyliczoneGradacje(produkt, klient, flatCena, out zakupionaDotychczasIlosc);

            Assert.True(wynikoweGradacje != null);
            Assert.True(listaKonfekcji.Last().Ilosc < rabaty.Calosc.KupowaneIlosciBLL.SumaKupowanychIlosci(0, null, null, DateTime.MinValue));

            Assert.True(wynikoweGradacje.Last().AktualnaCena);  
            Assert.False(wynikoweGradacje.Last().Spelniny);   
            Assert.True(wynikoweGradacje.Last().CenaNetto == listaKonfekcji.Last().RabatKwota.Value);  
        }

        [Fact()]
        public void WyliczoneGradacjeTest()
        {
            ustawDaneDlaGradacji(4);

            decimal zakupionaDotychczasIlosc;
            List<GradacjaWidok> wynikoweGradacje = rabaty.WyliczoneGradacje(produkt, klient, flatCena, out zakupionaDotychczasIlosc);

            Assert.True(wynikoweGradacje != null);

            Assert.True(rabaty.Calosc.KupowaneIlosciBLL.SumaKupowanychIlosci(0, null, null, DateTime.MinValue) == 4, "żeby kolejne testy działały to na sztywno ilość już kupiona ma być 4");
            Assert.True(produkt.JednostkaPodstawowa.PoprawanaIloscZakupu(null) == 0.2m, "żeby kolejne testy działały to na sztywno oz krok 0.2 wymagany");
            Assert.True(wynikoweGradacje.First().IleBrakujeDoSpelnieniaPoziomu ==   0 , "Powinno być 0 dlatego że mamy już kupione 4 sztuki, a mimnimalnie mozna kupic produktu 0.2 wiec - wiec poziom jest spelniony już");
            Assert.True(wynikoweGradacje.First().AktualnaCena == true );
            Assert.True(wynikoweGradacje.First().Spelniny == false);
            Assert.True(wynikoweGradacje.First().CenaNetto == 666);
            Assert.True(wynikoweGradacje.First().PrzedzialOd == null);
            Assert.True(wynikoweGradacje.First().PrzedzialOdRzeczywisty == 0);
            Assert.True(wynikoweGradacje.First().PrzedzialDo == null);
            Assert.True(wynikoweGradacje.First().PrzedzialDoRzeczywisty == 10);

            //druga gradacja  - przedzial od 10 do 30
            Assert.True(wynikoweGradacje[1].IleBrakujeDoSpelnieniaPoziomu == 6, "Powinno być 6 - bo juz mamy 4 kupione, a przedzial zaczyna sie od 10 - wiec wystarczy ze kupuje jeszcze 6 i bedziemy miec lepsza cene");
            Assert.True(wynikoweGradacje[1].AktualnaCena == false);
            Assert.True(wynikoweGradacje[1].Spelniny == false);
            Assert.True(wynikoweGradacje[1].CenaNetto == 100);
            Assert.True(wynikoweGradacje[1].PrzedzialOd == 6, "powinno byc 6 bo juz mamy 4 kupione, a przedzial zaczyna sie od 10 - wiec wystarczy ze kupuje jeszcze 6 i bedziemy miec lepsza cene");
            Assert.True(wynikoweGradacje[1].PrzedzialOdRzeczywisty == 10);
            Assert.True(wynikoweGradacje[1].PrzedzialDo == 26, "ma być 26 bo jak kupuje 26 do moich już 4 to wskocze w wyższy lewel");
            Assert.True(wynikoweGradacje[1].PrzedzialDoRzeczywisty == 30);

            Assert.True(wynikoweGradacje.Count == 4);   //od 0 - 30 (jedna konfekcje przeskakujemy bo jest zła cena!), 1 = 30 - 100, 2 = 100 +
        }
    }
}
