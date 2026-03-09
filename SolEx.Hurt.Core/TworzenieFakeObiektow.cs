using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using Adres = SolEx.Hurt.Model.Adres;
using Rejestracja = SolEx.Hurt.Model.Rejestracja;

namespace SolEx.Hurt.Core
{
    public class TworzenieFakeObiektow
    {
        /// <summary>
        /// przelatuje po proertisach obiektu i uzupelnia o dane testowe - na podstawie faków
        /// </summary>
        /// <param name="obiekt"></param>
        public static void UstawDaneTestowe(object obiekt)
        {
            PropertyInfo[] props = obiekt.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in props)
            {
                    if (pi.GetSetMethod() == null)
                    {
                        continue;
                    }

                    object sztcznyObiekt = null;
                    try
                    {
                        sztcznyObiekt = new TworzenieFakeObiektow(SolexBllCalosc.PobierzInstancje.Konfiguracja).PobierzObiektOTypie(pi.PropertyType);
                    }
                    catch (Exception)
                    {
                        sztcznyObiekt = null;
                    }

                    pi.SetValue(obiekt, sztcznyObiekt);
            }
        }

        private int losowaLiczba = 0;
        private string losowyCiag = "";

        private IConfigBLL _konfiguracja = null;

        public TworzenieFakeObiektow(IConfigBLL konfiguracja = null)
        {
            WylosujNoweDane();
            if (konfiguracja != null)
            {
                _konfiguracja = SolexBllCalosc.PobierzInstancje.Konfiguracja;
            }
            else
            {
                Dictionary<int, StatusZamowienia> slownik = new Dictionary<int, StatusZamowienia>(300);
                for (int i = 0; i < 300; ++i)
                {
                    var status = FakeStatusZamowienia();
                    status.Id = i;
                    slownik.Add(status.Id, status);
                    WylosujNoweDane();
                }

                var config = A.Fake<IConfigBLL>();
                A.CallTo(() => config.StatusyZamowien).Returns(slownik);


                Dictionary<long, Waluta> slownikWalut = new Dictionary<long, Waluta>(5);
                for (int i = 0; i < 5; ++i)
                {
                    var waluta = FakeWaluta();
                    if (!slownikWalut.ContainsKey(waluta.Id))
                    {
                        slownikWalut.Add(waluta.Id, waluta);
                    }
                    WylosujNoweDane();
                }

                A.CallTo(() => config.SlownikWalut).Returns(slownikWalut);

                _konfiguracja = config;
            }



        }

        public Waluta FakeWaluta()
        {
            return new Waluta() {Id =  losowaLiczba, NrKonta = "34534534534 534534 53453", WalutaB2b = "PLN", WalutaErp = "PLN"};
        }


        public StatusZamowienia FakeStatusZamowienia()
        {
            return new StatusZamowienia
            {
                Id = losowaLiczba * 4,
                Importowac = losowaLiczba % 2 == 0,
                Nazwa = "StatusTestowy" + losowaLiczba,
                PobranoErp = losowaLiczba % 3 == 0,
                Widoczny = losowaLiczba % 2 == 0,
                TraktujJakoFaktoring = losowaLiczba % 7 == 0,
                Symbol = "symbolTestowy" + losowaLiczba,
                PowiadomienieZmianaStatusu = losowaLiczba % 6 == 0,
                TraktujJakoOferte = losowaLiczba % 4 == 0,
                PlatnoscOnline = losowaLiczba % 7 == 0
            };
        }

        private void WylosujNoweDane()
        {
            losowaLiczba = new Random().Next(-100000, 10000) + DateTime.Now.Millisecond + new Random().Next(-10000, 1000);
            losowyCiag = Tools.PobierzInstancje.GetMd5Hash((losowaLiczba * 34).ToString());
        }

        public object PobierzObiektOTypie(Type typ)
        {
            if (typ.IsGenericType)
            {
                if (typ.GetGenericArguments().Length > 1)
                {
                    throw new Exception("Nie obsługujemy list z dwoma zmiennymi T");
                }

                Type typWartosci = typ.GetGenericArguments()[0];
                ArrayList list = new ArrayList(10);
                for (int i=0; i<10; ++i)
                {
                    var temp = PobierzPojedycznyObiekt(typWartosci);
                    if (temp == null)
                    {
                        throw new Exception("Pusty obiekt dla typu: "  + typWartosci.Name);
                    }
                    list.Add(temp);
                    WylosujNoweDane();
                }

                object typedArray = list.ToArray(typWartosci);
                // It really helps that we don't need to work through overloads...
                MethodInfo openMethod = typeof(Enumerable).GetMethod("ToList");
                MethodInfo genericMethod = openMethod.MakeGenericMethod(typWartosci);
                object result = genericMethod.Invoke(null, new object[] { typedArray });

                return result;

            }
          return PobierzPojedycznyObiekt(typ);
        }

        public PlikIntegracjiSzablon FakePlikIntegracjiSzablon()
        {
            PlikIntegracjiSzablon szablon = new PlikIntegracjiSzablon();
            szablon.IdSzablonu = this.losowaLiczba;
            szablon.Format = "XML";
            szablon.typDanych = TypDanychIntegracja.ProduktyKatalogDrukowanie;
            szablon.Wersja = new List<int> {1, 34, 5, 7, 866, 66};
            return szablon;
        }

        private object PobierzPojedycznyObiekt(Type typ)
        {
            var theMethod = this.GetType().GetMethods().Where(x => x.ReturnType == typ && x.Name.StartsWith("Fake")).ToList();

            if (theMethod == null || theMethod.IsEmpty())
            {
                throw new Exception("Brak metody do tworzenia fake dla obiektu: " + typ.Name);
            }

            if (theMethod.Count > 1)
            {
                throw new Exception("Za dużo metod z zwracanym typem: " + typ.Name);
            }
            var metoda = theMethod.First();
            object[] parametry = null;
            if (metoda.GetParameters() != null && metoda.GetParameters().Any())
            {
                parametry = new object[metoda.GetParameters().Count()];
                for (int i = 0; i < metoda.GetParameters().Count(); i++)
                {
                    Type typParametru = metoda.GetParameters()[i].ParameterType;
                    parametry[i] = typParametru.IsValueType? Activator.CreateInstance(typParametru) : null;
                }
            }
            return metoda.Invoke(this, parametry);
        }

        public NewsletterZapisani FakeNewsletterZapisani()
        {
            var newsletter = A.Fake<NewsletterZapisani>();
            A.CallTo(() => newsletter.AdersIp).Returns("1111111111");
            A.CallTo(() => newsletter.DataZapisania).Returns(DateTime.Now);
            return newsletter;
        }

        public IKoszykiBLL FakeIKoszykiBLL()
        {
            var kosz = A.Fake<IKoszykiBLL>();
            A.CallTo(() => kosz.CalkowitaWartoscHurtowaNettoPoRabacie()).Returns(new WartoscLiczbowa(500, "PLN"));
            A.CallTo(() => kosz.CalkowitaWartoscHurtowaBruttoPoRabacie()).Returns(new WartoscLiczbowa(1000, "PLN"));
            A.CallTo(() => kosz.Uwagi).Returns("Uwagi w koszyku");
            A.CallTo(() => kosz.WagaCalokowita()).Returns(2);
            A.CallTo(() => kosz.CalkowitaObjetoscKoszyka()).Returns(23);

            var adres = A.Fake<Adres>();
            adres.KrajSymbol = "PL";
            adres.UlicaNr = "Ulica 43";
            adres.Miasto = "Częstochowa";
            adres.KodPocztowy = "42-200";
            adres.Miasto = "Częstochowa";
            adres.Kraj = "Polska";
            A.CallTo(() => kosz.Adres).Returns(adres);


            var pozycja = A.Fake<KoszykPozycje>();
            A.CallTo(() => pozycja.Ilosc).Returns(2);
            A.CallTo(() => pozycja.CenaNetto).Returns(new WartoscLiczbowa(500, "PLN"));
            A.CallTo(() => pozycja.CenaBrutto()).Returns(new WartoscLiczbowa(540, "PLN"));
            A.CallTo(() => kosz.LacznaWartoscNetto()).Returns(new WartoscLiczbowa(1000, "PLN"));
            A.CallTo(() => kosz.WartoscVat()).Returns(new WartoscLiczbowa(80, "PLN"));
             A.CallTo(() => kosz.LacznaWartoscBrutto()).Returns(new WartoscLiczbowa(1080, "PLN"));
            A.CallTo(() => pozycja.WartoscNetto).Returns(new WartoscLiczbowa(1000, "PLN"));
            A.CallTo(() => pozycja.WartoscBrutto).Returns(new WartoscLiczbowa(1080, "PLN"));

            A.CallTo(() => pozycja.Produkt).Returns( this.FakeIProduktKlienta() );
            A.CallTo(() => kosz.PobierzPozycje).Returns(new List<KoszykPozycje> { pozycja });
            return kosz;
        }

        public List<IKlient> FakeKlienciAkceptujacy()
        {
            var klient = FakeIKlient();
            klient.Email = "example1@solex.net.pl";
            var klient2 = FakeIKlient();
            klient2.Email = "example2@solex.net.pl";
            return new List<IKlient>() {klient,klient2};
        }

        public DokumentyBll FakeDokumentyBll()
        {
            var dok = A.Fake<DokumentyBll>();
            A.CallTo(() => dok.DataUtworzenia).Returns(new DateTime(2015, 11, 01).AddDays(this.losowaLiczba));
            A.CallTo(() => dok.NazwaDokumentu).Returns("Nazwa dokumentu");
            A.CallTo(() => dok.DokumentWartoscNetto).Returns(new WartoscLiczbowa(500 + this.losowaLiczba/5, "PLN"));
            A.CallTo(() => dok.DokumentWartoscBrutto).Returns(new WartoscLiczbowa(1000 + this.losowaLiczba/3, "PLN"));
            A.CallTo(() => dok.NazwaPlatnosci).Returns("Nazwa płatnosci");
            A.CallTo(() => dok.DokumentTerminRealizacji).Returns(DateTime.Now.AddDays(6 * this.losowaLiczba));
            A.CallTo(() => dok.TerminPlatnosci).Returns(DateTime.Now.AddDays(2 * this.losowaLiczba));
            A.CallTo(() => dok.walutaB2b).Returns("PLN");

            A.CallTo(() => dok.DokumentWartoscNalezna).Returns(new WartoscLiczbowa(1000 * losowaLiczba, "PLN"));

            int losowyID = new Random().Next(0, _konfiguracja.StatusyZamowien.Count - 1);

            A.CallTo(() => dok.StatusId).Returns( _konfiguracja.StatusyZamowien.ElementAt(losowyID).Value.Id );
            A.CallTo(() => dok.CzyPrzeterminowany()).Returns(true);
            A.CallTo(() => dok.DokumentDniSpoznienia).Returns(5 * this.losowaLiczba);
            A.CallTo(() => dok.DokumentWartoscNalezna).Returns(new WartoscLiczbowa(500 * this.losowaLiczba, "PLN"));
            A.CallTo(() => dok.TerminPlatnosci).Returns(DateTime.Now.AddDays(this.losowaLiczba));

            A.CallTo(() => dok.walutaB2b).Returns("PLN");
            A.CallTo(() => dok.DniDoTerminuZaplaty()).Returns(5 * this.losowaLiczba);

            return dok;
        }

        public bool FakeBoolean()
        {
            return true;
        }

        public string FakeString()
        {
            return losowyCiag;
        }

        public AtrybutBll FakeAtrybut()
        {
            WylosujNoweDane();
            AtrybutBll a = A.Fake<AtrybutBll>();
            a.Nazwa = "Atrybut przykładowy nr " + this.losowaLiczba;
            a.Id = this.losowaLiczba;
            a.CechyPokazujKatalog = true;
          
            var ListaCech = new List<CechyBll>();

            for (int i = 0; i < 15; ++i)
            {
                WylosujNoweDane();
                var c = this.FakeCecha();
                c.AtrybutId = a.Id;

                A.CallTo(() => c.PobierzAtrybut()).Returns(a);

                ListaCech.Add(c);
            }

            A.CallTo(() => a.ListaCech).Returns(ListaCech);

            return a;
        }

        public CechyBll FakeCecha()
        {
            CechyBll c = A.Fake<CechyBll>();
            c.Id = this.losowaLiczba;
            c.Nazwa = "Cecha testowa " + this.losowyCiag;
            c.Widoczna = true;
            A.CallTo(() => c.NazwaAtrybutu).Returns("Atrybut przykładowy nr " + this.losowaLiczba);
            return c;
        }

        //Fakowe obiekty produktu do podgladu
        public IProduktKlienta FakeIProduktKlienta(long id = 0, List<AtrybutBll> listaAtrybutow = null, List<KategorieBLL> listaKategorieBlls = null )
        {
            IProduktKlienta pk = A.Fake<IProduktKlienta>();

            int losowyNumer = new Random().Next();

            pk.Nazwa = "Produkt " + losowyNumer;
            pk.Kod = "Symbol " + losowyNumer;
            pk.KodKreskowy = "Kod kreskowy " + losowyNumer;
            pk.ZdjecieGlowne = null;
            pk.FriendlyLinkURL = "produkt-1-symbol3" + losowyNumer;

            if (id == 0)
            {
                id = losowaLiczba;
            }
            A.CallTo(() => pk.Id).Returns(id);

            pk.Vat = 8;
            pk.OpZbiorczeIloscWOpakowaniu = losowaLiczba;
            pk.IloscMinimalna = losowyNumer%3;

            IFlatCenyBLL flat = A.Fake<IFlatCenyBLL>();
            flat.ProduktId = pk.Id;
            flat.CenaNetto = losowaLiczba*34;
            flat.CenaHurtowaNetto = losowaLiczba * 39;

            losowyNumer = new Random().Next(0, this._konfiguracja.SlownikWalut.Count - 1);
            flat.WalutaId = this._konfiguracja.SlownikWalut.ElementAt(losowyNumer).Key;
            A.CallTo(() => pk.FlatCeny).Returns(flat);

            foreach (var p in _konfiguracja.SlownikPoziomowCenowych)
            {
                pk.CenyPoziomy.Add(p.Key, new CenaPoziomu(p.Key, Math.Abs(p.Key + losowaLiczba + losowaLiczba * p.Key % 3), pk.Id, this._konfiguracja.SlownikWalut.ElementAt(losowyNumer).Key) );
            }

            if (listaAtrybutow == null)
            {
                listaAtrybutow = new List<AtrybutBll>();
                for (int i = 0; i < 15; ++i)
                {
                    WylosujNoweDane();
                    listaAtrybutow.Add(this.FakeAtrybut());
                }
            }

            //nie mozna tu nic losowac bo te atrybuty musza sie pojawic w pliku dla szabllnow bo inaczej nie da sie zrobic szablonu
            List<CechyBll> cechyB2B = listaAtrybutow.Where(x => x.ListaCech != null).SelectMany(x => x.ListaCech.Count > 20 ? x.ListaCech.Take(20) : x.ListaCech).DistinctBy(x => x.Id).ToList();

            Dictionary<long, CechyBll> slownikCech = cechyB2B.ToDictionary(x => x.Id, x => x);

            foreach (AtrybutBll atrybut in listaAtrybutow)
            {
                List<CechyBll> listaCech = slownikCech.Values.Where(x => x.AtrybutId == atrybut.Id).ToList();
                A.CallTo(() => pk.CechyDlaAtrybutu(atrybut.Id, true)).Returns(listaCech);
                A.CallTo(() => pk.CechyDlaAtrybutu(atrybut.Id, false)).Returns(listaCech);
            }

            A.CallTo(() => pk.Kategorie).Returns(listaKategorieBlls);
          
            //slownik cech dla produktu
            A.CallTo(() => pk.Cechy).Returns(slownikCech);

            pk.Klient = this.FakeIKlient();

            pk.JezykId = this._konfiguracja.JezykIDDomyslny;

            return pk;
        }

        //Fakowe obiekty do podgladu
        public IKlient FakeIKlient()
        {
            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Id).Returns(1);
            klient.Nazwa = "Testowy";
            klient.Symbol = "Symbol testowy";
            klient.Telefon = "111111";
            klient.Email = "email@exaple.com";
            klient.Login = "email@exaple.com";
            klient.Kategorie = new[] {1, 2, 3, 10};
            Waluta waluta = new Waluta() {NrKonta ="42342423", WalutaB2b = "PLN", WalutaErp = "PLN"};
            klient.WalutaId = waluta.Id;
            A.CallTo(() => klient.JezykId).Returns(1);

            A.CallTo(() => klient.KluczSesji).Returns("adsasdbb###61281kdjswldks");

            IKlient opiekun = A.Fake<IKlient>();
            A.CallTo(() => opiekun.Id).Returns(2);
            opiekun.Nazwa = "Opiekun nazwa";
            opiekun.Symbol = "Opiekun symbol";
            opiekun.Email = "opiekun@exaple.com";
            opiekun.Telefon = "222";
            A.CallTo(() => opiekun.JezykId).Returns(1);
            var obrazek = A.Fake<IObrazek>();
            A.CallTo(() => obrazek.LinkWWersji(A<string>.Ignored)).Returns("/zasoby/Obrazki/ikonaFacet.png?preset=ico82x82wp");
            A.CallTo(() => opiekun.Obrazek).Returns(obrazek);

            A.CallTo(() => klient.Opiekun).Returns(opiekun);
            A.CallTo(() => klient.OpiekunId).Returns(opiekun.Id);

            A.CallTo(() => klient.DrugiOpiekun).Returns(opiekun);
            A.CallTo(() => klient.DrugiOpiekunId).Returns(opiekun.Id);

            A.CallTo(() => klient.Przedstawiciel).Returns(opiekun);
            A.CallTo(() => klient.PrzedstawicielId).Returns(opiekun.Id);

            klient.IndywidualnaStawaVat = null;

            return klient;
        }

        public ZamowieniaBLL FakeZamowieniaBLL()
        {
            ZamowieniaBLL zamowienie = A.Fake<ZamowieniaBLL>();
            A.CallTo(() => zamowienie.Klient).Returns(FakeIKlient());
            A.CallTo(() => zamowienie.DokumentNazwa).Returns("Nazwa Dokumentu");
            A.CallTo(() => zamowienie.NumerWlasnyZamowieniaKlienta).Returns("000/000/000");
            A.CallTo(() => zamowienie.NazwaPlatnosci).Returns("Przelew");
            A.CallTo(() => zamowienie.DokumentWartoscBrutto).Returns(new WartoscLiczbowa { Wartosc = 500, Waluta = "PLN" });
            A.CallTo(() => zamowienie.DokumentWartoscNetto).Returns(new WartoscLiczbowa { Wartosc = 1000, Waluta = "PLN" });
            A.CallTo(() => zamowienie.GetUwagiHTML()).Returns("Jakies uwagi klienta");
            A.CallTo(() => zamowienie.DataUtworzenia).Returns(DateTime.Now.AddDays(-5));
            A.CallTo(() => zamowienie.BladKomunikat).Returns("Komunikat błędu");
          //  A.CallTo(() => zamowienie.DokumentWartoscVat).Returns(new WartoscLiczbowa(230, "PLN"));

            var listaPozycj = this.PobierzObiektOTypie(typeof(List<ZamowieniaProduktyBLL>)) as IEnumerable<ZamowieniaProduktyBLL>;

            A.CallTo(() => zamowienie.PobierzPozycjeDokumentu()).Returns(listaPozycj);
            A.CallTo(() => zamowienie.WagaZamowienia()).Returns(3.03m);
            A.CallTo(() => zamowienie.ObjetoscZamowienia()).Returns(2.13m);

            var adres = new Adres
            {
                Id = 1,
                UlicaNr = "Borowej Góry 5/71",
                Miasto = "Warszawa - Bemowo",
                KodPocztowy = "01-354",
                Telefon = "22 664 24 24"
            };
            A.CallTo(() => zamowienie.Adres).Returns(adres);

            return zamowienie;
        }

        public Rejestracja FakeRejestracja()
        {
            var rej = A.Fake<Rejestracja>();
            A.CallTo(() => rej.Nazwa).Returns("Rejestracja nazwa");
            A.CallTo(() => rej.Nip).Returns("Rejestracja nip");
            A.CallTo(() => rej.Ulica).Returns("Rejestracja ulica");
            A.CallTo(() => rej.Miasto).Returns("Rejestracja miasto");
            A.CallTo(() => rej.KodPocztowy).Returns("42-200");
            A.CallTo(() => rej.Panstwo).Returns("Rejestracja panstwo");
            A.CallTo(() => rej.ImieNazwisko).Returns("Rejestracja imie i nazwisko");
            A.CallTo(() => rej.Telefon).Returns("789789789");
            A.CallTo(() => rej.Email).Returns("Rejestracja@email.com");
            A.CallTo(() => rej.RodzajDzialalnosci).Returns("Rejestracja rodzaj dzialalnosci");
            A.CallTo(() => rej.WysylkaUlica).Returns("Rejestracja wysylka ulica");
            A.CallTo(() => rej.WysylkaMiasto).Returns("Rejestracja wysylka miasto");
            A.CallTo(() => rej.WysylkaKodPocztowy).Returns("42-200");
            A.CallTo(() => rej.WysylkaPanstwo).Returns("Rejestracja panstwo");
            A.CallTo(() => rej.Zalacznik1).Returns("17092015280904_2016273179\\2015-09-08_13h29_09.png");
            A.CallTo(() => rej.HasloJednorazowe).Returns("haslo maslo");
            A.CallTo(() => rej.AdresIp).Returns("192.168.1.1");
            A.CallTo(() => rej.DataRejestracji).Returns(DateTime.Now);

            return rej;
        }

        public ProduktBazowy FakeProduktBazowy()
        {
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>() { new JednostkaProduktu(true, 1, losowaLiczba, "szt.", 1) };
            var pb = new ProduktBazowy(1)
            {
                Nazwa = "Nazwa losowy produkt nr. " + losowaLiczba,
                Kod = "symbol pozycja1",
                KodKreskowy = losowaLiczba.ToString(),
                Id = losowaLiczba,
                Jednostki = jednostki
            };
            return pb;
        }

        public ZamowieniaProduktyBLL FakeZamowieniaProduktyBLL()
        {
            ZamowieniaProduktyBLL poz1 = A.Fake<ZamowieniaProduktyBLL>();
            A.CallTo(() => poz1.Ilosc).Returns(3*Math.Abs(losowaLiczba));
            A.CallTo(() => poz1.PozycjaDokumentuCenaNetto).Returns(new WartoscLiczbowa { Wartosc = Math.Abs(5*this.losowaLiczba), Waluta = "PLN" });
            A.CallTo(() => poz1.PozycjaDokumentuCenaBrutto).Returns(new WartoscLiczbowa { Wartosc = 10, Waluta = "PLN" });
            A.CallTo(() => poz1.PozycjaDokumentuWartoscNetto).Returns(new WartoscLiczbowa { Wartosc = 25, Waluta = "PLN" });
            A.CallTo(() => poz1.PozycjaDokumentuWartoscBrutto).Returns(new WartoscLiczbowa { Wartosc = 50, Waluta = "PLN" });
            A.CallTo(() => poz1.Jednostka).Returns("szt.");
            poz1.UstawJednostke("szt.");

            var pb = FakeProduktBazowy();
       
            IKlient klient = new SolEx.Hurt.Core.Klient();

            A.CallTo(() => poz1.ProduktBazowy).Returns(pb);
            ProduktBazowy p = A.Fake<ProduktBazowy>();
            A.CallTo(() => p.JednostkaPodstawowa).Returns(new JednostkaProduktu() {Nazwa = "szt.", Id = 1});
            A.CallTo(() => poz1.ProduktBazowy).Returns(p);
            A.CallTo(() => poz1.JednostkaPrzelicznik).Returns(1.8m);
            A.CallTo(() => poz1.Opis).Returns("Testowy opis");

            return poz1;
        }

        public HistoriaDokumentuListPrzewozowy FakeListyPrzewozowe()
        {
            var lista1 = A.Fake<HistoriaDokumentuListPrzewozowy>();
            A.CallTo(() => lista1.NumerListu).Returns("numer listu");
            A.CallTo(() => lista1.Link).Returns("link listu");
            return lista1;
        }

        public IKatalogSzablonModelBLL FakeIKatalogSzablonModelBLL()
        {
            IKatalogSzablonModelBLL katalog = A.Fake<IKatalogSzablonModelBLL>();
            katalog.Id = losowaLiczba;
            katalog.Nazwa = "szablon katalog 4";
            katalog.ParametrTekstowy1 = "param tekst1" + losowyCiag;
            katalog.ParametrTekstowy2 = "param tekst2" + losowyCiag;
            katalog.ParametrTekstowy3 = "param tekst3" + losowyCiag; ;
            katalog.ParametrTekstowy4 = "param tekst4" + losowyCiag; ;
            katalog.Aktywny = true;
            return katalog;
        }

        public ProduktPrzyjetyNaMagazyn FakeProduktPrzyjetyNaMagazyn()
        {
            ProduktPrzyjetyNaMagazyn p = A.Fake<ProduktPrzyjetyNaMagazyn>();
            p.KodKreskowy = "kod kreskowy";
            p.KodProduktu = "kod produktu";
            p.NazwaProduktu = "nazwa produktu";
            p.ProduktId = 1;
            p.Stan = losowaLiczba / 10;

            return p;
        }
    }
}
