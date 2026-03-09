using ServiceStack.Common.Extensions;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Modele;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using SolEx.Hurt.Core.BLL.ZadaniaDlaZamowienia;
using SolEx.Hurt.Model.Interfaces;
using ServiceStack.DataAnnotations;
using System.Reflection;
using Newtonsoft.Json;
using SolEx.Hurt.Core.Helper;

namespace SolEx.Hurt.Core.BLL
{
    public class KoszykiDostep : LogikaBiznesBaza, IKoszykiDostep
    {
        public KoszykiDostep(ISolexBllCalosc calosc) : base(calosc)
        {
        }
        private ConcurrentDictionary<long, int?> _slownikKlientowZAkceptacja;
        /// <summary>
        /// kasuje zmiany zrobione przez moduly koszykowe - kasuje produkty automatyczne i czysci rabaty dodatkowe nadane przez moduly koszykowe
        /// </summary>
        /// <param name="koszyk"></param>
        /// <returns></returns>
        public KoszykBll ZresetujKoszyk(KoszykBll koszyk)
        {
            foreach (ModelBLL.Interfejsy.IKoszykPozycja pozycja in koszyk.PobierzPozycje)
            {
                pozycja.RabatDodatkowy = 0;
                if (pozycja.TypPozycji == TypPozycjiKoszyka.Automatyczny)
                {
                    pozycja.Ilosc = 0;
                }
            }
            return koszyk;
        }



        public virtual void UaktualnijKoszyk(KoszykBll koszyk, bool aktualizujPozycje = true)
        {
            try
            {
                if (aktualizujPozycje)
                {
                    List<KoszykPozycje> pozycje = new List<KoszykPozycje>();

                    //Bartek - hujowe rozwiazanie, ale i tak lepsze niz zapis kazdej pozycji osobno - niestety na BIo nie chcemy już zmieniąc kosmicznei kodu - taka hujnia Bartek
                    var pozycjeSQL = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Select<KoszykPozycje>(x => x.KoszykId == koszyk.Id).ToDictionary(x => x.Id, x => x);

                    foreach (KoszykPozycje koszyki_Pozycje in koszyk.PobierzPozycje)
                    {
                        //najpierw sprawdzamy czy ilosc 0 - jak tak to kasowanie
                        if (koszyki_Pozycje.Ilosc == 0)
                        {
                            Calosc.DostepDane.UsunPojedynczy<KoszykPozycje>(koszyki_Pozycje.Id);
                            continue;
                        }

                        koszyki_Pozycje.Hash = WygenerujTyp(koszyki_Pozycje);
                        koszyki_Pozycje.KoszykId = koszyk.Id;
                        koszyki_Pozycje.Ilosc = SprawdzIlosc(koszyki_Pozycje);

                        //bartek robie taka hujnie bo w bio nie ma inaczej jak tego zrobic bez mocnego refaktora :(

                        if (pozycjeSQL.TryGetValue(koszyki_Pozycje.Id, out var pozycjaIstniejacaJuzWSQL))
                        {
                            //jest pozycja juz w sql
                            //czy sa roznice
                            if (
                                koszyki_Pozycje.ProduktId != pozycjaIstniejacaJuzWSQL.ProduktId ||
                                koszyki_Pozycje.Ilosc != pozycjaIstniejacaJuzWSQL.Ilosc ||
                                koszyki_Pozycje.JednostkaId != pozycjaIstniejacaJuzWSQL.JednostkaId ||
                                koszyki_Pozycje.PowodDodatkowegoRabatu != pozycjaIstniejacaJuzWSQL.PowodDodatkowegoRabatu ||
                                koszyki_Pozycje.PrzedstawicielId != pozycjaIstniejacaJuzWSQL.PrzedstawicielId ||
                                koszyki_Pozycje.WymuszonaCenaNettoPrzedstawiciel != pozycjaIstniejacaJuzWSQL.WymuszonaCenaNettoPrzedstawiciel ||
                                koszyki_Pozycje.Hash != pozycjaIstniejacaJuzWSQL.Hash ||
                                koszyki_Pozycje.DodajaceZadanie != pozycjaIstniejacaJuzWSQL.DodajaceZadanie ||
                                koszyki_Pozycje.Opis != pozycjaIstniejacaJuzWSQL.Opis ||
                                koszyki_Pozycje.WymuszonaCenaNettoModul != pozycjaIstniejacaJuzWSQL.WymuszonaCenaNettoModul ||
                                koszyki_Pozycje.ProduktBazowyId != pozycjaIstniejacaJuzWSQL.ProduktBazowyId ||
                                koszyki_Pozycje.TypPozycji != pozycjaIstniejacaJuzWSQL.TypPozycji
                                )
                            {
                                pozycje.Add(koszyki_Pozycje);
                            }
                            else
                            {
                                continue;
                            }
                            //jak nie to idziemy dalej
                        }
                        else
                        {
                            //nie ma produktu dotychczas
                            pozycje.Add(koszyki_Pozycje);
                        }
                    }

                    if (pozycje.Count > 0)
                    {
                        Calosc.DostepDane.AktualizujListe<KoszykPozycje>(pozycje);
                    }

                    //TODO: beznadzijene rozwianie - gdzies trzeba kasowac gradacje policzone - na szybko tutaj ale to nie powinno byc tu. Problem jest taki ze w DAL nie mamy klienta / jezyka
                    //Calosc.Rabaty.WyczyscGradacjePoZmianieKoszyka(koszyk.Klient);
                }

                Calosc.DostepDane.AktualizujPojedynczy(koszyk);
            }
            catch (Exception ex)
            {
                Calosc.Log.Debug(ex);
                throw;
            }
        }

        private void KoszykWEdycji(KoszykBll koszyk)
        {
            string kluczDlaEdycjiLock = $"DodajProduktDoKoszyka_edycja_{koszyk.Id}";

            //Sprawdzamy czy koszyk jest w edycji. Jeżeli tak wywalamy wyjątek, w przeciwnym wypadku blokujemy koszyk ponieważ będziemy wykonywać na nim operacje dodawania pozycji
            LockHelper.PobierzInstancje.UruchomKodWLocku_BezUzywaniaCache(kluczDlaEdycjiLock, () =>
            {
                if (koszyk.WEdycji)
                {
                    throw new Exception($"Koszyk id: {koszyk.Id} podczas modyfikacji");
                }

                koszyk.WEdycji = true;
            });
        }

        public KoszykBll ZmienPozycjeKoszyka(List<KoszykPozycje> pozycje, IKlient k, out List<ModelBLL.Interfejsy.IKoszykPozycja> przekroczoneStany, out List<ModelBLL.Interfejsy.IKoszykPozycja> przekroczoneLimity, 
            out List<IProduktKlienta> nowe, out List<ModelBLL.Interfejsy.IKoszykPozycja> zmienioneilosci, out List<ModelBLL.Interfejsy.IKoszykPozycja> dodane, KoszykBll item)
        {
            nowe = new List<IProduktKlienta>();
            przekroczoneLimity = new List<IKoszykPozycja>();
            przekroczoneStany = new List<IKoszykPozycja>();
            zmienioneilosci = new List<IKoszykPozycja>();
            dodane = new List<IKoszykPozycja>();
            if (pozycje.Count > 0)
            {
                try
                {
                    this.KoszykWEdycji(item);

                    try
                    {
                        foreach (KoszykPozycje t in pozycje)
                        {
                            if (!CzyMoznaDodacDoKoszyka(item, t, k))
                            {
                                continue;
                            }

                            IKoszykPozycja pozycja = DodajPozycje(item, k, t);
                            SprawdzPozycje(item, pozycja, dodane, zmienioneilosci, przekroczoneStany, przekroczoneLimity, nowe);
                        }
                    }
                    finally
                    {
                        item.DataModyfikacji = DateTime.Now;
                        UaktualnijKoszyk(item);
                        item.WEdycji = false;
                    }
                }
                catch (Exception)
                {
                    dodane.Clear();
                    przekroczoneLimity.Clear();
                    przekroczoneStany.Clear();
                    zmienioneilosci.Clear();
                    return null;
                }

                return this.Calosc.DostepDane.PobierzPojedynczy<KoszykBll>(item.Id, k);
            }

            return null;
        }

        private int WygenerujTyp(IKoszykPozycja pozycja)
        {
            if (pozycja.Indywidualizacja != null && pozycja.Indywidualizacja.Any())
            {
                return 1 + Math.Abs(pozycja.Indywidualizacja.GetHashCode());
            }
            return 1;
        }

        /// <summary>
        /// metoda spradza pozycje i zwraca listy np. przekrocznych, zmienionych ilosci itp
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pozycja"></param>
        /// <param name="dodane"></param>
        /// <param name="zmienioneilosci"></param>
        /// <param name="przekroczoneStany"></param>
        /// <param name="przekroczoneLimity"></param>
        /// <param name="nowe"></param>
        private void SprawdzPozycje(KoszykBll item, ModelBLL.Interfejsy.IKoszykPozycja pozycja, List<ModelBLL.Interfejsy.IKoszykPozycja> dodane, List<ModelBLL.Interfejsy.IKoszykPozycja> zmienioneilosci, List<ModelBLL.Interfejsy.IKoszykPozycja> przekroczoneStany, List<ModelBLL.Interfejsy.IKoszykPozycja> przekroczoneLimity, List<IProduktKlienta> nowe)
        {
            ModelBLL.Interfejsy.IKoszykPozycja ist = item.PobierzPozycje.FirstOrDefault(p => p.ProduktId == pozycja.ProduktId && p.TypPozycji == pozycja.TypPozycji && p.Hash == pozycja.Hash);
            if (ist == null)
            {
                nowe.Add(pozycja.Produkt);
            }
            decimal tmpIloscPrzedSprawdzeniem = pozycja.Ilosc;
            pozycja.Ilosc = SprawdzIlosc(pozycja);

            dodane.Add(pozycja);

            if (tmpIloscPrzedSprawdzeniem != pozycja.Ilosc)
            {
                zmienioneilosci.Add(pozycja);
                dodane.Remove(pozycja);
            }
            if (CzyJestPrzekroczonyStan(item, pozycja))
            {
                przekroczoneStany.Add(pozycja);
                dodane.Remove(pozycja);
            }
            if (tmpIloscPrzedSprawdzeniem > pozycja.Ilosc)
            {
                przekroczoneLimity.Add(pozycja);
                dodane.Remove(pozycja);
            }
        }

        /// <summary>
        /// Dodaje, zwiększa stan pozycji w koszyku
        /// </summary>
        /// <param name="item">Koszyk do którego dodajemy</param>
        /// <param name="klient"></param>
        /// <param name="wzorPozycja"></param>
        /// <returns>Dodana/Zmodyfikowana pozycja</returns>
        public virtual IKoszykPozycja DodajPozycje(IKoszykiBLL item, IKlient klient, IKoszykPozycja wzorPozycja)
        {
            wzorPozycja.Hash = WygenerujTyp(wzorPozycja);
            IProduktBazowy pk = Calosc.DostepDane.PobierzPojedynczy<ProduktBazowy>(wzorPozycja.ProduktBazowyId);
            KoszykPozycje pozycja = item.PobierzPozycje.FirstOrDefault(p => p.ProduktId == wzorPozycja.ProduktId && p.Hash == wzorPozycja.Hash && p.TypPozycji == wzorPozycja.TypPozycji);
            if (pozycja == null)
            {
                pozycja = new KoszykPozycje(wzorPozycja);
                pozycja.Klient = item.Klient;
                pozycja.Ilosc = 0;
                item.DodajPozycjeDoKoszyka(pozycja);
            }

            long idjednostki = wzorPozycja.JednostkaId.GetValueOrDefault();
            if (idjednostki == 0)
            {
                idjednostki = pk.JednostkaPodstawowa.Id;
            }
            ((IKoszykPozycja)pozycja).JednostkaId = idjednostki;
            pozycja.WymuszonaCenaNettoModul = wzorPozycja.WymuszonaCenaNettoModul;
            pozycja.PrzedstawicielId = wzorPozycja.PrzedstawicielId;
            pozycja.DataZmiany = wzorPozycja.DataZmiany;
            pozycja.Ilosc += wzorPozycja.Ilosc ;
            pozycja.Indywidualizacja = wzorPozycja.Indywidualizacja;
            //   pozycje.dodano_z_kategorii = wzor.dodano_z_kategorii;
            pozycja.DataDodania = DateTime.Now;
            pozycja.RabatDodatkowy = wzorPozycja.RabatDodatkowy;
            return pozycja;
        }

        public bool CzyMoznaDodacDoKoszyka(KoszykBll item, KoszykPozycje kp, IKlient klient)
        {
            if (kp.TypPozycji == TypPozycjiKoszyka.Gratis || kp.TypPozycji == TypPozycjiKoszyka.ZaPunkty)
            {
                return true;
            }

            if (!Calosc.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(klient).Contains(kp.ProduktId))
            {
                return false;
            }

            IProduktKlienta pk = Calosc.DostepDane.PobierzPojedynczy<ProduktKlienta>(kp.ProduktId,item.Klient.JezykId,klient);
            string info;
            bool pokaz;
            if ((item.Typ == TypKoszyka.Koszyk || item.Typ == TypKoszyka.KoszykApi) && !Calosc.ProduktyBazowe.MoznaDodacDoKoszyka(pk, out info, out pokaz))
                return false;
            return true;
        }

        /// <summary>
        /// Spradza czy produkt ma przekroczony stan - NIE bierze pod uwagę modułu przekroczone stany w koszyku
        /// </summary>
        /// <param name="koszyk"></param>
        /// <param name="pozycja"></param>
        /// <returns></returns>
        public bool CzyJestPrzekroczonyStan(KoszykBll koszyk, IKoszykPozycja pozycja)
        {
            if (pozycja == null)
            {
                throw new InvalidOperationException("Brak pozycji");
            }

            if (pozycja.IloscWJednostcePodstawowej <= pozycja.Produkt.IloscLaczna)
            {
                return false;
            }
            var sposoby = SolexBllCalosc.PobierzInstancje.SposobyPokazywaniaStanowBll.WszystkieSposobyKlienta(koszyk.Klient,Calosc.Konfiguracja.JezykIDPolski, null);
            if (sposoby!=null && sposoby.Any() && CzyJestWlaczonyModulPrzekroczonychStanow)
            {
                return true;
            }
            return false;
        }

        private bool? _przekroczoneStanyModulCzyJestWlaczony;

        public virtual bool CzyJestWlaczonyModulPrzekroczonychStanow
        {
            get
            {
                if (!_przekroczoneStanyModulCzyJestWlaczony.HasValue)
                {
                    _przekroczoneStanyModulCzyJestWlaczony = Calosc.ZadaniaBLL.JestAktywneZadanie<PrzekroczoneStany>();
                }
                return _przekroczoneStanyModulCzyJestWlaczony.Value;
            }
        }

        public IList<KoszykBll> MetodaPrzetwarzajacaPoSelect(int i, IKlient klient, IList<KoszykBll> koszyki, object arg4)
        {
            foreach (KoszykBll k in koszyki)
            {
                k.Klient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(k.KlientId);

                var dostawy = SolexBllCalosc.PobierzInstancje.Koszyk.DostepneSposobyDostawy(k);
                var platnosci = SolexBllCalosc.PobierzInstancje.Koszyk.DostepneSposobyPlantosci(k);

                if (dostawy != null && dostawy.Count == 1)
                {
                    k.KosztDostawyId = dostawy.First().Id;
                }
                if (platnosci != null && platnosci.Count == 1)
                {
                    k.PlatnoscId = platnosci.First().Id;
                }

            }
            return koszyki;
        }

        public void KupZaPunkty(IKoszykiBLL koszyk, ZamowieniaBLL zamowienie)
        {
            var klient = koszyk.Klient.KlientPodstawowy();
            if (klient == null) klient = zamowienie.Klient;
            var pozycjeZaPunkty = koszyk.PobierzPozycje.Where(x => x.TypPozycji == TypPozycjiKoszyka.ZaPunkty);
            foreach (var poz in pozycjeZaPunkty)
            {
                decimal iloscPkt = poz.Produkt.CenaWPunktach*poz.Ilosc;
                PunktyWpisy zdejmij = new PunktyWpisy(klient.Id, iloscPkt, DateTime.Now, "", string.Format("Zamówienie nr: {0}", zamowienie.DokumentNazwa));
                zdejmij.ZamowienieId = zamowienie.Id;
                Calosc.PunktyDostep.ZdejmijPunkty(zdejmij);
            }
        }
     
        public bool CzyKlientMaKoszykiDoAkceptacji(long idKlienta, out int? ilosc)
        {
            bool saKoszykiDoAkceptacji = true;

            //jezeli prywatny cache jest nullem tworzymy slowniek gdzie kluczem to id klienta oraz ilosc koszykow do akceptacji
            if (_slownikKlientowZAkceptacja == null)
            {
                _slownikKlientowZAkceptacja = new ConcurrentDictionary<long, int?>();
            }
            if (!_slownikKlientowZAkceptacja.TryGetValue(idKlienta, out ilosc))
            {
                //pobieramy ilosc koszyków do akceptacji
                ilosc = Convert.ToInt32(this.Calosc.DostepDane.DbORM.Count<KoszykBll>(x => Sql.InSubquery(x.Id, new SqlServerExpressionVisitor<AkceptacjaKoszykow>().Select(z => z.KoszykId).Where(z => z.KlientId == idKlienta))));
                if (ilosc == 0)
                {
                    ilosc = null;
                    saKoszykiDoAkceptacji = false;
                }
                _slownikKlientowZAkceptacja.TryAdd(idKlienta, ilosc);
            }
            else if (_slownikKlientowZAkceptacja[idKlienta] == null)
            {
                return false;
            }

            return saKoszykiDoAkceptacji;
        }

        public void UsunAkceptacjeKoszyka(long koszykId)
        {
            //usuwamy akceptacje koszyka 
            this.Calosc.DostepDane.UsunWybrane<AkceptacjaKoszykow, long>(x => x.KoszykId == koszykId);

            //Czyscimy koszyki do akcpetacji klientom 
            WyczyscKoszykiDoAkceptacji(koszykId);
        }
        public void WyczyscKoszykiDoAkceptacji(long idKoszyka) { _slownikKlientowZAkceptacja = null; }
        /// <summary>
        /// główna metoda do finalizacji koszyka
        /// </summary>
        /// <param name="koszyk"></param>
        /// <param name="aktualnyKlient"></param>
        /// <param name="przedstawiciel"></param>
        public void FinalizujKoszyk(IKoszykiBLL koszyk, IKlient aktualnyKlient, IKlient przedstawiciel)
        {
            WykonajZadaniaNaKoszyku<IZadaniePoFinalizacji>(koszyk);

            List<ZamowieniaProduktyBLL> pozycje = new List<ZamowieniaProduktyBLL>();
            ZamowieniaBLL zamowienie = ZapiszJakoZamowienie(koszyk, przedstawiciel, out pozycje);
            if (zamowienie.Id == 0)
            {
                throw new Exception("Zamówienie nie zostało zapisano - błędny identyfikator zamówienia");
            }

            var klient = koszyk.Klient;
            
            if (koszyk.HistoriaZmianStatusow != null && koszyk.HistoriaZmianStatusow.Any(x => x.Staus == StatusKoszyka.Zakceptowany))
            {
                Calosc.Statystyki.ZdarzenieSubkonta_ZamowienieZaakceptowane(zamowienie, klient, aktualnyKlient, null);
            }
            else
            {
                Calosc.Statystyki.ZdarzenieNoweZamowienie_Finalizacja(zamowienie, klient, null);
            }

            if (koszyk.PobierzPozycje.Any(x => x.TypPozycji == TypPozycjiKoszyka.ZaPunkty))
            {
                KupZaPunkty(koszyk, zamowienie);
            }

            if (Calosc.Konfiguracja.ZamowieniaTworzRezerwacje)
            {
                AktualizujStany(zamowienie, pozycje);
            }
            
            Calosc.DostepDane.UsunPojedynczy<KoszykBll>(koszyk.Id);
            Calosc.Klienci.UsunCacheLimitow(koszyk.KlientId);
            Calosc.ProfilKlienta.UsunWartosc(klient,TypUstawieniaKlienta.WybranyKoszyk, TypKoszyka.Koszyk.ToString());
        }

        private ZamowieniaBLL ZapiszJakoZamowienie(IKoszykiBLL koszyk, IKlient przedstawiciel, out List<ZamowieniaProduktyBLL> pozycje)
        {
            ZamowieniaBLL nowe = new ZamowieniaBLL(null);

            IKlient k = koszyk.Klient;

            if (koszyk.KosztDostawy() != null && koszyk.NieDodawajDostawyDoKoszyka != true)
            {
                if (koszyk.KosztDostawy().ProduktDostawy != null)
                {
                    decimal cena = koszyk.KosztDostawy().WyliczCene(koszyk);
                    if (cena > 0 || Calosc.Konfiguracja.DodawajSposobDostawyJakoPozycje)
                    {
                        KoszykPozycje kp = new KoszykPozycje { ProduktId = koszyk.KosztDostawy().ProduktDostawy.Id, JednostkaId = koszyk.KosztDostawy().ProduktDostawy.JednostkaPodstawowa.Id,ProduktBazowyId = koszyk.KosztDostawy().ProduktDostawy.Id };
                        KoszykPozycje pozycjeDostawa = new KoszykPozycje(kp);
                        pozycjeDostawa.Klient = koszyk.Klient;
                        pozycjeDostawa.ProduktId = koszyk.KosztDostawy().ProduktDostawy.Id;
                        pozycjeDostawa.Ilosc = 1;
                        ((IKoszykPozycja)pozycjeDostawa).JednostkaId = koszyk.KosztDostawy().ProduktDostawy.JednostkaPodstawowa.Id;
                        pozycjeDostawa.DataDodania = DateTime.Now;
                        pozycjeDostawa.KoszykId = koszyk.Id;
                        pozycjeDostawa.WymuszonaCenaNettoModul = cena;
                        koszyk.DodajPozycjeDoKoszyka(pozycjeDostawa);
                    }
                    if (cena == 0 && Calosc.Konfiguracja.DodawajSposobDostawyDoUwag)
                    {
                        koszyk.Uwagi += string.Format(SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(koszyk.Klient.JezykId,"Wybrano darmowy sposób dostawy: {0}"), koszyk.KosztDostawy().ProduktDostawy.Kod);
                    }
                }
                else
                {
                    throw new Exception("Błąd kosztu dostawy, próba dodania niepoprawnego produktu " + koszyk.KosztDostawyId);
                }
            }

            if (koszyk.Adres != null)
            {
                nowe.AdresId = koszyk.Adres.Id;
                nowe.Adres = new Model.Adres(koszyk.Adres);
            }

            nowe.KlientId = koszyk.KlientId;
            nowe.Klient = koszyk.Klient;
            nowe.walutaB2b = koszyk.WalutaKoszyka().WalutaB2b;
            nowe.NumerWlasnyZamowieniaKlienta = string.IsNullOrEmpty(koszyk.NumerZamowienia) ? null: koszyk.NumerZamowienia;
            nowe.PoziomCenyId = k.PoziomCenowyId;
            nowe.Uwagi = (koszyk.Uwagi ?? "").ReplaceHexadecimalSymbols();
            nowe.DataUtworzenia = DateTime.Now;
            nowe.StatusId = StatusImportuZamowieniaDoErp.Złożone;//Calosc.Konfiguracja.StatusyZamowien.Values.First(p => p.Symbol == "Złożone").Id;
            nowe.WalutaId = koszyk.WalutaKoszyka().Id;
            nowe.TerminDostawy = koszyk.TerminDostawy;

            if (!string.IsNullOrEmpty(koszyk.MagazynRealizujacy))
            {
                if (Calosc.Konfiguracja.SlownikMagazynowPoSymbolu.ContainsKey(koszyk.MagazynRealizujacy))
                {
                    nowe.MagazynRealizujacy = koszyk.MagazynRealizujacy;
                }
                else
                {
                    throw new Exception("Wybrany magazyn nie istniej na platformie!!!");
                }
                
            }

            if (string.IsNullOrEmpty(nowe.MagazynRealizujacy))
            {
                var podstawowy = Calosc.Konfiguracja.SlownikMagazynowPoSymbolu.Values.FirstOrDefault(x => x.MagazynRealizujacy);
                if (podstawowy != null)
                {
                    nowe.MagazynRealizujacy = podstawowy.Symbol;
                }
            }

            if (!string.IsNullOrEmpty(koszyk.MagazynDlaMm))
            {
                if (Calosc.Konfiguracja.SlownikMagazynowPoSymbolu.ContainsKey(koszyk.MagazynDlaMm))
                {
                    nowe.MagazynDlaMm = koszyk.MagazynDlaMm;
                }
                else
                {
                    throw new Exception("Wybrany magazyn dla mm-ek nie istniej na platformie!!!");
                }

            }

            nowe.KategoriaZamowienia = koszyk.KategoriaZamowienia;
            nowe.DodatkowePola = koszyk.DodatkowePolaErp;
            if (przedstawiciel != null)
            {
                nowe.PracownikSkladajacyId = przedstawiciel.Id;
            }

            nowe.DefinicjaDokumentuErp = koszyk.DefinicjaDokumentuERP;
            pozycje = new List<ZamowieniaProduktyBLL>();
            var sort = Calosc.Konfiguracja.SortowaniePozycjiPrzedZapisemZamowienia;
            var pr = Calosc.Szukanie.SortujObiekty(koszyk.PobierzPozycje, sort);
            List<ZadaniaPozycjiZamowienia> pozycjeZadania = Calosc.ZadaniaBLL.PobierzZadania<IZadaniePoStworzeniuZamowienia, ZadaniaPozycjiZamowienia>(k.JezykId, k).ToList();

            foreach (var pozycja in pr)
            {
                if (pozycja.Ilosc <= 0)
                {
                    Calosc.Log.InfoFormat($"Pomijamy pozycje, ilość <=0, klient {koszyk.KlientId}");
                    continue;
                }
                ZamowieniaProduktyBLL poz = new ZamowieniaProduktyBLL(null, pozycja)
                {
                    Opis = (pozycja.Opis + " " + pozycja.OpisIndywidualizacji() + (pozycja.WymuszonaCenaNettoPrzedstawiciel.HasValue ? " Zmieniona przez przedstawiciela. " : "")).Trim()
                };
                //Dodawanie pozycji zzadania dodającego produkt do zamówienia
                if (pozycja.DodajaceZadanie.HasValue)
                {
                    poz.Opis += " " + Calosc.DostepDane.PobierzPojedynczy<ZadanieBll>(pozycja.DodajaceZadanie.Value).Modul().Komentarz;
                }
                poz.TypPozycji = PobierzTypPozycjiZamowienia(pozycja, koszyk);
                var p = pozycje.FirstOrDefault(x => x.ProduktIdBazowy == poz.ProduktIdBazowy);
                if (p != null && p.ProduktId==poz.ProduktId&& (pozycja.Indywidualizacja == null || !pozycja.Indywidualizacja.Any() )) //|| poz.Opis.IndexOf(pozycja.OpisIndywidualizacji(), StringComparison.CurrentCultureIgnoreCase)!=-1)
                {
                    SolexBllCalosc.PobierzInstancje.Log.ErrorFormat($"Próba zdublowania pozycji zamówienia dla produktu o id: {pozycja.ProduktBazowyId}");
                    continue;
                }
                if (pozycjeZadania.Any())
                {
                    WykonajZadaniaNaPozycjiZamowienia<IZadaniePoStworzeniuZamowienia>(poz, pozycja, pozycjeZadania);
                }

                pozycje.Add(poz);
            }
           

            nowe.NazwaPlatnosci = koszyk.Platnosc;
            if (koszyk.DodatkoweParametry != null)
            {
                int[] pliki = koszyk.DodatkoweParametry.Values.Where(x => x.Symbol.Contains("PlikiDodaneDoZamowienia")
                               && !string.IsNullOrEmpty(x.WybraneWartosciString)).SelectMany(x => x.WybraneWartosciString.FromJson<int[]>()).ToArray();
                nowe.Pliki = pliki.Any() ? pliki : null;
            }
            nowe.WartoscBrutto = pozycje.Sum(x => x.PozycjaDokumentuWartoscBrutto.Wartosc);
            nowe.WartoscNetto = pozycje.Sum(x => x.PozycjaDokumentuWartoscNetto.Wartosc);
            nowe.IdOddzialu = k.OddzialDoJakiegoNalezyKlient; //jesli brak oddzialu to bedzie oddzial 0

            nowe.NumerTymczasowyZamowienia = SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.GenerujNumerZamowieniaDlaOddzialu(k, DateTime.Now.Year);

            lock (lokDlaZamowien)
            {
                SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.AktualizujZamowienia(nowe, pozycje);
            }

            //nowe.DokumentNazwaSynchronizacja = null;
            //nowe.Id = SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.AktualizujZamowienia(nowe, pozycje);
           
            
          
            //if (!string.IsNullOrEmpty(nowe.NumerTymczasowyZamowienia))
            //{
            //    SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.AktualizujZamowienia(nowe, pozycje);
            //}
            return nowe;
        }


        private static object lokDlaZamowien = new object();

        public TypPozycjiZamowienia PobierzTypPozycjiZamowienia(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            if (pozycja.TypPozycji == TypPozycjiKoszyka.Automatyczny)
            {
                return TypPozycjiZamowienia.DodanyAutomatycznie;
            }
            if (koszyk.KosztDostawy() != null && koszyk.KosztDostawy().ProduktDostawy.Id != 0 && koszyk.KosztDostawy().ProduktDostawy.Id == pozycja.ProduktId)
            {
                return TypPozycjiZamowienia.Dostawa;
            }
            return TypPozycjiZamowienia.DodanyPrzezKlienta;

        }

        /// <summary>
        /// aktualizuje stany - odejmuje ze stanu ilosci zamowione
        /// </summary>
        /// <param name="zamowienie"></param>
        /// <param name="pozycje"></param>
        public void AktualizujStany(ZamowieniaBLL zamowienie, List<ZamowieniaProduktyBLL> pozycje )
        {
            if (string.IsNullOrEmpty(zamowienie.MagazynRealizujacy))
            {
                SolexBllCalosc.PobierzInstancje.Log.ErrorFormat("Nie zdefiniowano magazynu podstawowego z którego maja być zdejmowane stany!!!!");
            }
            foreach (var pozycja in pozycje)
            {
                if (pozycja.TypPozycji == TypPozycjiZamowienia.Dostawa)
                {
                    continue;
                }
                try
                {
                    SolexBllCalosc.PobierzInstancje.ProduktyStanBll.ZmniejszStany(pozycja.ProduktIdBazowy, pozycja.PozycjaDokumentuIlosc, zamowienie.MagazynRealizujacy);
                }
                catch (Exception ex)
                {
                    SolexBllCalosc.PobierzInstancje.Log.ErrorFormat($"Problem przy aktualizacji stanów po złożeniu zamówienia o id: {zamowienie.Id}, dla produktu: {pozycja.Id}, błąd: {ex.Message}");
                }
                
            }
        }

        /// <summary>
        /// Generuje komunikaty dla klienta do pokazania
        /// </summary>
        /// <param name="jezyk"></param>
        /// <param name="result"></param>
        /// <param name="przekroczoneStany"></param>
        /// <param name="przekroczoneLimity"></param>
        /// <param name="nowe"></param>
        /// <param name="zmienioneIlosci"></param>
        /// <param name="pozycjeDodawane"></param>
        /// <param name="aktualnyKlient"></param>
        /// <returns></returns>
        public OdpowiedzKoszyk WygenerujKomunikaty(int jezyk, IKoszykiBLL result, List<IKoszykPozycja> przekroczoneStany,
            List<IKoszykPozycja> przekroczoneLimity, List<IProduktKlienta> nowe, List<IKoszykPozycja> zmienioneIlosci, List<IKoszykPozycja> pozycjeDodawane, IKlient aktualnyKlient)
        {
            HashSet<long> idProduktowZmienianych = new HashSet<long>();
            List<OdpowiedzKoszykaDlaPozycji> wynik = new List<OdpowiedzKoszykaDlaPozycji>();
            if (result == null)
            {
                wynik.Add(StworzOdpowiedz(Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, "Nie udało się dodać do koszyka produktów. Proszę odświeżyć stronę."), aktualnyKlient));
                return new OdpowiedzKoszyk()
                {
                    Odpowiedzi = wynik
                };
            }
            else
            {
                if (pozycjeDodawane.Any())
                {
                    wynik.Add(
                        StworzOdpowiedz(Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, "Dodano do koszyka"),
                        aktualnyKlient,
                        "success"
                        ));
                    idProduktowZmienianych.UnionWith(pozycjeDodawane.Select(x => x.ProduktId));
                }
                if (przekroczoneStany.Any())
                {
                    wynik.AddRange(KomunikatyOPrzekroczonychStanach(jezyk, result, przekroczoneStany, aktualnyKlient));
                    idProduktowZmienianych.UnionWith(przekroczoneStany.Select(x => x.ProduktId));
                }
                if (przekroczoneLimity.Any())
                {
                    wynik.AddRange(KomunikatyOPrzekroczonychLimitach(jezyk, result, przekroczoneLimity, aktualnyKlient));
                    idProduktowZmienianych.UnionWith(przekroczoneLimity.Select(x=>x.ProduktId));
                }
                if(zmienioneIlosci.Any())
                {
                    wynik.AddRange(KomunikatyOZmienionychIlosciach(jezyk, result, zmienioneIlosci, aktualnyKlient));
                    idProduktowZmienianych.UnionWith(zmienioneIlosci.Select(x=>x.ProduktId));
                }
            }
            List<AktualneIlosci> ilosci = new List<AktualneIlosci>();
            //dodanie aktualnych ilości wszystkich pozycji
            if (idProduktowZmienianych.Any() && result!=null)
            {
                foreach (var pozycja in idProduktowZmienianych)
                {
                    if (result.PobierzIlosciProduktowWKoszyku().ContainsKey(pozycja))
                    {
                        ilosci.Add(new AktualneIlosci {ProduktId = pozycja, Ilosc = result.PobierzIlosciProduktowWKoszyku()[pozycja]});
                    }
                }
            }

            return new OdpowiedzKoszyk() {
                Odpowiedzi = wynik,
                Pozycje = ilosci,
                Netto = Math.Round(result.CalkowitaWartoscHurtowaNettoPoRabacie(),2),
                Brutto = Math.Round(result.CalkowitaWartoscHurtowaBruttoPoRabacie(),2),
                Waluta = result.WalutaKoszyka().WalutaB2b,
                IloscPozycji = result.PobierzPozycje.Count};
        }
        
        private IEnumerable<OdpowiedzKoszykaDlaPozycji> KomunikatyOZmienionychIlosciach(int jezyk, IKoszykiBLL result, IEnumerable<ModelBLL.Interfejsy.IKoszykPozycja> zmienioneIlosci, IKlient aktualnyKlient)
        {
            return zmienioneIlosci.Select(poz => StworzOdpowiedz(string.Format(Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, "Zmieniono ilość pozycji {0}"), poz.Produkt.Nazwa), aktualnyKlient, "info")).ToList();
        }

        private IEnumerable<OdpowiedzKoszykaDlaPozycji> KomunikatyOPrzekroczonychLimitach(int jezyk, IKoszykiBLL result, IEnumerable<ModelBLL.Interfejsy.IKoszykPozycja> przekroczoneLimity, IKlient aktualnyKlient)
        {
            return przekroczoneLimity.Select(poz => StworzOdpowiedz(string.Format(Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, "Przekroczono dostępny limit zakupowy produktu {0}"), poz.Produkt.Nazwa), aktualnyKlient, "info")).ToList();
        }

        private IEnumerable<OdpowiedzKoszykaDlaPozycji> KomunikatyOPrzekroczonychStanach(int jezyk, IKoszykiBLL result, IEnumerable<ModelBLL.Interfejsy.IKoszykPozycja> przekroczoneStany, IKlient aktualnyKlient)
        {
            List<OdpowiedzKoszykaDlaPozycji> wynik = new List<OdpowiedzKoszykaDlaPozycji>();
            if (Calosc.Konfiguracja.InfoPrzekroczoneStany)
            {
                wynik.AddRange(przekroczoneStany.Select(poz => StworzOdpowiedz(string.Format(Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, "Przekroczono stan produktu {0}"), poz.Produkt.Nazwa), aktualnyKlient, "info")));
            }
            return wynik;
        }

        private OdpowiedzKoszykaDlaPozycji StworzOdpowiedz(string tekst, IKlient aktualnyKlient, string typ = "error")
        {
            if (!SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(aktualnyKlient, TypUstawieniaKlienta.UkryjCenyHurtowe))
            {
                return new OdpowiedzKoszykaDlaPozycji() {  CzasWyswietlania = Calosc.Konfiguracja.CzasWyswietlaniaKoszyka, Tekst = tekst, Typ = typ };
            }
            return new OdpowiedzKoszykaDlaPozycji { CzasWyswietlania = Calosc.Konfiguracja.CzasWyswietlaniaKoszyka, Tekst = tekst, Typ = typ };
        }

        public List<Platnosc> DostepneSposobyPlantosci(IKoszykiBLL koszyk)
        {
            //Calosc.ZadaniaBLL.PobierzZadania<ISposobPlatnosci, ZadanieCalegoKoszyka>(koszyk.Klient.JezykId, koszyk.Klient);
            var tmp = Calosc.ZadaniaBLL.PobierzZadaniaCalegoKoszykaKtorePasuja<ISposobPlatnosci>(koszyk);
            return tmp.Where(s => s.Wykonaj(koszyk)).Cast<Platnosc>().ToList();
        }

        public List<ISposobDostawy> DostepneSposobyDostawy(IKoszykiBLL koszyk)
        {
            var tmp = Calosc.ZadaniaBLL.PobierzZadaniaCalegoKoszykaKtorePasuja<ISposobDostawy>(koszyk);
            return tmp.Where(s => s.Wykonaj(koszyk)).Cast<ISposobDostawy>().ToList();
        }

        public List<ISposobDostawy> WszystkjeSposobyDostawy(IKoszykiBLL koszyk)
        {
            var tmp = Calosc.ZadaniaBLL.PobierzZadania<ISposobDostawy, ZadanieCalegoKoszyka>(koszyk.Klient.JezykId, koszyk.Klient);
            return tmp.Where(s => s.Wykonaj(koszyk)).Cast<ISposobDostawy>().ToList();
        }

        /// <summary>
        /// Tworzy nowy koszyk - jako typ mozna podac np. koszyk API
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="nazwa"></param>
        /// <param name="typ"></param>
        /// <returns></returns>
        public long StworzNowyKoszyk(IKlient klient, string nazwa, TypKoszyka typ = TypKoszyka.Koszyk)
        {
            KoszykBll tmp = new KoszykBll
            {
                Klient = (Klient)klient,
                Nazwa = nazwa,
                DataModyfikacji = DateTime.Now,
                Typ = typ,
                Aktywny = true,
                KlientId = klient.Id,
            };
            UaktualnijKoszyk(tmp);
            return tmp.Id;
        }

        //TODO: do przeniesinia do produktu bazweog - nigdy nie jest wykorzystane, wiec moze byc wspolne dla produktow bazowych dla wszystkich klientow naraz
        public JednostkaProduktu[] UzupelnijJednostkiOKrokDodania(List<JednostkaProduktu> jednostki, IProduktKlienta produkt)
        {
            foreach (JednostkaProduktu t in jednostki)
            {
                if (produkt.WymaganeOz)
                {
                    if (produkt.JednostkaPodstawowa.Id == t.Id)
                    {
                        t.Krok = produkt.IloscWOpakowaniu;
                    }
                    else
                    {
                        if (t.Calkowitoliczowa)
                        {
                            t.Krok = Math.Ceiling(produkt.IloscWOpakowaniu / t.Przelicznik);
                        }
                        else
                        {
                            t.Krok = Math.Round(produkt.IloscWOpakowaniu / t.Przelicznik, t.Zaokraglenie);
                        }
                    }
                }
                else
                {
                    if (t.Przelicznik < produkt.IloscWOpakowaniu)
                    {
                        //jesli nie ma wymaganegoOz a przelicznik jest mniejszy od ilości to wzracamy tylko przelicznik  
                        if (t.Calkowitoliczowa)
                        {
                            t.Krok = Math.Ceiling(t.Przelicznik);
                        }
                        else
                        {
                            t.Krok = Math.Round(t.Przelicznik, t.Zaokraglenie);
                        }
                    }
                    else
                    {
                        // int zaokraglenie = t.Calkowitoliczowa ? 0 : t.Zaokraglenie;
                        if (t.Calkowitoliczowa)
                        {
                            t.Krok = Math.Ceiling(produkt.IloscWOpakowaniu / t.Przelicznik);
                        }
                        else
                        {
                            t.Krok = Math.Round(produkt.IloscWOpakowaniu / t.Przelicznik, t.Zaokraglenie);
                        }
                    }
                }
            }
            return jednostki.ToArray();
        }

        public decimal SprawdzIlosc(IProduktKlienta produkt, long jednostka, decimal ilosc, decimal poprzedniailosc, decimal iloscWKoszyku = 0)
        {
            //zaokrąglenie ma na celu usunięcie błędu przy wyliczeniu np.: 12.9999999999999999999999994 gdzie powino być 13 lub 13,0000000000000000000000000001
            ilosc = Math.Round(ilosc, 7);

            JednostkaProduktu j = produkt.Jednostki.FirstOrDefault(x => x.Id == jednostka) ?? produkt.Jednostki.First();
            
            //zaokrąglamy tylko w górę  #9247 2016-08-26 12.16 - w dodatku zokraglenie do kilku miejsc po przecinku
            decimal moznik = (decimal)Math.Pow(10, j.Zaokraglenie);
            ilosc = decimal.Ceiling(ilosc * moznik) / moznik;

            //poprzedniailosc = Math.Round(poprzedniailosc, j.Zaokraglenie);

            // bool minimumwymagane = Calosc.Konfiguracja.MinimumLogistyczneWymagane;
            bool odejmowanie = poprzedniailosc > ilosc;// sprzwdzam czy odejmujemy
            bool dodawanie = poprzedniailosc < ilosc; // sprawdzamy czy dodajemy
           
            decimal il = j.PrzeliczIlosc(ilosc);
            
            decimal poprzedniaIloscJednostkaPodstawowa = Math.Round(il * j.Przelicznik, 4);

            //minimum logistyczne jest zawsze wymagane task 9607 pkt 2
            if (jednostka == produkt.JednostkaPodstawowa.Id)// && minimumwymagane)
            {
                if (il != 0 && (il+ iloscWKoszyku) < produkt.IloscMinimalna && !odejmowanie)    //jesli jest 0 to nie ruszamy jednostki minimalnej
                {
                    il = produkt.IloscMinimalna;
                    poprzedniaIloscJednostkaPodstawowa = produkt.IloscMinimalna;
                }
                else if ((il + iloscWKoszyku) < produkt.IloscMinimalna && odejmowanie)
                {
                    poprzedniaIloscJednostkaPodstawowa = 0;
                    if (il >= 1)
                    {
                        il = 0;
                        poprzedniaIloscJednostkaPodstawowa = produkt.IloscMinimalna;
                    }
                }
            }

            // ile sztuk w opakowaniu zbiporczym jest w wybranej jednostce.
            decimal przelicznikDlaWybranejJednostki = produkt.IloscWOpakowaniu / j.Przelicznik;

            if (jednostka == produkt.JednostkaPodstawowa.Id && produkt.WymaganeOz && produkt.IloscWOpakowaniu != 0 && poprzedniaIloscJednostkaPodstawowa % produkt.IloscWOpakowaniu != 0)
            {
                //  decimal iloscop = decimal.Round(poprzedniaIloscJednostkaPodstawowa / produkt.IloscWOpakowaniu);
                decimal iloscop = decimal.Ceiling(poprzedniaIloscJednostkaPodstawowa / produkt.IloscWOpakowaniu);
                il = iloscop * przelicznikDlaWybranejJednostki;
            }
            else if (jednostka != produkt.JednostkaPodstawowa.Id && produkt.WymaganeOz && produkt.IloscWOpakowaniu != 0 && poprzedniaIloscJednostkaPodstawowa % produkt.IloscWOpakowaniu != 0)
            {
                if (odejmowanie)
                {
                    decimal iloscop = decimal.Floor(ilosc / przelicznikDlaWybranejJednostki);
                    il = iloscop * przelicznikDlaWybranejJednostki;
                }
                else if (dodawanie)
                {
                    decimal iloscop = decimal.Ceiling(ilosc / przelicznikDlaWybranejJednostki);
                    il = iloscop * przelicznikDlaWybranejJednostki;
                }
            }
            if (produkt.DostepnyLimit != null && il > produkt.DostepnyLimit)
            {
                il = produkt.DostepnyLimit.GetValueOrDefault();
            }

            if (il > 9999) //jak ktoś przegnie z ilością dodawaną
            {
                il = 9999;
            }
            return il;
        }

        public decimal SprawdzIlosc(IKoszykPozycja pozycjaBLL)
        {
            return SprawdzIlosc(pozycjaBLL.Produkt, pozycjaBLL.JednostkaId.GetValueOrDefault(), pozycjaBLL.Ilosc, pozycjaBLL.Ilosc);
        }

        public void Odrzuc(KoszykBll koszyk, IKlient klient)
        {
            koszyk.KlienciMogacyAkceptowacKoszyk = null;
            this.Calosc.Koszyk.UsunAkceptacjeKoszyka(koszyk.Id);
            koszyk.UstawStatus(klient, StatusKoszyka.Odrzucony);
            Calosc.Statystyki.ZdarzenieSubkonta_ZamowienieOdrzucone(koszyk, koszyk.Klient, klient);
            UaktualnijKoszyk(koszyk);
        }

        public bool WyslijKoszykDoAkceptacji(KoszykBll koszyk, IKlient aktualnyKlient)
        {
            SzablonAkceptacjiPoziomy poziom;
            bool koszykdoAkceptacji = koszyk.CzyKoszykDoAkceptacji(aktualnyKlient, out poziom);

            //czyscimy stare koszyki akceptacje
            UsunAkceptacjeKoszyka(koszyk.Id);
            if (!koszykdoAkceptacji)
            {
                koszyk.UstawStatus(aktualnyKlient, StatusKoszyka.Zakceptowany);
                koszyk.KlienciMogacyAkceptowacKoszyk = null;
                return false;
            }
            koszyk.UstawStatus(aktualnyKlient, StatusKoszyka.DoAkceptacji);
            List<AkceptacjaKoszykow> listaAkceptacji = new List<AkceptacjaKoszykow>();
            foreach (var klient in poziom.Klienci)
            {
                listaAkceptacji.Add(new AkceptacjaKoszykow(koszyk.Id,klient));
            }
            koszyk.KlienciMogacyAkceptowacKoszyk = poziom.Klienci;
            Calosc.DostepDane.AktualizujListe<AkceptacjaKoszykow>(listaAkceptacji);
            Calosc.Statystyki.ZdarzenieSubkonta_ZamowienieDoAkceptacji(koszyk, aktualnyKlient);
            UaktualnijKoszyk(koszyk);
            return true;
        }

        public OdpowiedzKoszykaDlaPozycji ZmienStatusPozycjiInfoDostepnosc(IKlient klient, long produkt, int jezyk, out bool ist)
        {
            ist = true;
            OdpowiedzKoszykaDlaPozycji odp;
            if (klient.IdInfoODostepnosci !=null && klient.IdInfoODostepnosci.Contains(produkt))
            {
                klient.IdInfoODostepnosci.Remove(produkt);
                ist = false;
                odp = StworzOdpowiedz(Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, "Nie będziemy informawać Cię już o dostępności tego produktu"), klient, "notice");
            }
            else
            {
                if (klient.IdInfoODostepnosci == null)
                {
                    klient.IdInfoODostepnosci=new HashSet<long>();
                }
                klient.IdInfoODostepnosci.Add(produkt);
                IProduktKlienta produktKlienta = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produkt, klient);
                Calosc.Statystyki.ZdarzenieProsbaOInformacjeODostepnosci(produktKlienta, klient);
                odp = StworzOdpowiedz(Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, "Poinformujemy Cię, kiedy produkt będzie dostępny"), klient, "success");
            }
            Calosc.DostepDane.AktualizujPojedynczy(klient as Klient);
            return odp;
        }

        public OdpowiedzKoszykaDlaPozycji ZmienStatusPozycjiUlubione(IKlient klient, long produkt, int jezyk, out bool ist)
        {
            ist = true;
            OdpowiedzKoszykaDlaPozycji odp = null;
            if (klient.IdUlubionych!=null && klient.IdUlubionych.Contains(produkt))
            {
                klient.IdUlubionych.Remove(produkt);
                ist = false;
                odp = StworzOdpowiedz(Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, "Usunięto produkt z ulubionych"), klient, "notice");
            }
            else
            {
                if (klient.IdUlubionych == null)
                {
                    klient.IdUlubionych=new HashSet<long>();
                }
                klient.IdUlubionych.Add(produkt);
                odp = StworzOdpowiedz(Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, "Dodano produkt do ulubionych"), klient, "success");
            }

            var prod = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produkt, klient);

            if(prod == null)
            {
                //brak produktu - nic nie robimy
                return null;
            }

            if (ist)
            {
                prod.IdCechPRoduktu.Add(SolexBllCalosc.PobierzInstancje.Konfiguracja.CechaUlubione.Id);
            }
            else
            {
                prod.IdCechPRoduktu.Remove(SolexBllCalosc.PobierzInstancje.Konfiguracja.CechaUlubione.Id);
            }

            Calosc.DostepDane.AktualizujPojedynczy(klient as Klient);
            //Calosc.ProduktyKlienta.WyczyscCacheProduktyKlienta(klient);
            return odp;
        }

        public Komunikat[] WykonajModulyKoszykowe(KoszykBll koszyk)
        {
            IKlient aktualnyKlient = koszyk.Klient; 

            koszyk = ZresetujKoszyk(koszyk);
            //BARRTEK  - duża zmiana. Nie musimy zpaisywac zmian po modulach i przed - bo przeciez i tak jak wycaigniemy z bazy zeby finalziowac to policzymy i wyjdzie tak samo. Nie ma znaczenia czy zapiszemy zmiany do bazy
            //UaktualnijKoszyk(koszyk);
            List<Komunikat> komunikaty = new List<Komunikat>();
            if (!koszyk.PobierzPozycje.Any())
            {
                IEnumerable<ZadanieCalegoKoszyka> zadania = Calosc.ZadaniaBLL.PobierzZadaniaCalegoKoszykaKtorePasuja<IModulKoszykPusty>(koszyk);
                foreach (var zadanieKoszyka in zadania)
                {
                    zadanieKoszyka.DodajWiadomosc += (sender, args) => komunikaty.Add(args.Komunikat);
                    if (!zadanieKoszyka.Wykonaj(koszyk))
                    {
                        koszyk.MoznaFinalizowacKoszyk = false;
                    }
                }
            }

            koszyk.MoznaFinalizowacKoszyk = true;

            if (koszyk.PobierzPozycje.Any())
            {
                Dictionary<IKoszykPozycja, List<ZadaniePozycjiKoszyka>> pozycjeZadania = Calosc.ZadaniaBLL.PobierzZadaniaPozycjiKtorePasuja<IModulStartowy>(koszyk);
                foreach (var kp in pozycjeZadania)
                {
                    foreach (var zadanieKoszyka in kp.Value)
                    {
                        zadanieKoszyka.DodajWiadomosc += (sender, args) => komunikaty.Add(args.Komunikat);
                        if (!zadanieKoszyka.Wykonaj(kp.Key, koszyk))
                        {
                            koszyk.MoznaFinalizowacKoszyk = false;
                        }
                    }
                }
                IEnumerable<ZadanieCalegoKoszyka> zadania = Calosc.ZadaniaBLL.PobierzZadaniaCalegoKoszykaKtorePasuja<IModulStartowy>(koszyk).ToList();
                foreach (var zadanieKoszyka in zadania)
                {
                    bool czySaGratisyDoWyboru = true;
                    if (zadanieKoszyka is IModulKoszykGratisy)
                    {
                        czySaGratisyDoWyboru = PobierzDostepneGratisy(koszyk, aktualnyKlient.JezykId, aktualnyKlient).Any();
                    }
                    if (!czySaGratisyDoWyboru)
                    {
                        koszyk.MoznaFinalizowacKoszyk = false;
                        continue;
                    }
                    zadanieKoszyka.DodajWiadomosc += (sender, args) => komunikaty.Add(args.Komunikat);
                    if (!zadanieKoszyka.Wykonaj(koszyk))
                    {
                        koszyk.MoznaFinalizowacKoszyk = false;
                    }
                }
            }
            return komunikaty.OrderBy(x => x.Priorytet).ToArray();
        }
        
        public List<string> PobierzDostepneMagazyny(KoszykBll koszyk, IKlient aktualnyKlient)
        {
            List<string> wynik = new List<string>();
            var tmp = Calosc.ZadaniaBLL.PobierzZadaniaCalegoKoszykaKtorePasuja<IModulWyboruMagazynuRealizujacego>(koszyk).ToArray();
            foreach (var t in tmp)
            {
                IModulWyboruMagazynuRealizujacego m = (IModulWyboruMagazynuRealizujacego) t;
                wynik.AddRange(m.PobierzDostepneMagazyny(aktualnyKlient)); 
            }
            return wynik;
        }

        public IList<Tuple<IProduktKlienta, ParametryIlosciProduktu>> PobierzDostepneGratisy(IKoszykiBLL koszyk, int aktualnyJezyk, IKlient aktualnyKlient)
        {
            var tmp = Calosc.ZadaniaBLL.PobierzZadaniaCalegoKoszykaKtorePasuja<IModulWyboruGratisow>(koszyk).ToArray();
            Dictionary<long, OpisProduktuGratisowego> wynik = new Dictionary<long, OpisProduktuGratisowego>();
            foreach (var t in tmp)
            {
                IModulWyboruGratisow m = (IModulWyboruGratisow)t;
                var produkty = m.PobierzProdukty(aktualnyKlient);
                foreach (var p in produkty)
                {
                    if (!wynik.ContainsKey(p.IdProduktu))
                    {
                        wynik.Add(p.IdProduktu, p);
                    }
                    else if (wynik[p.IdProduktu].Cena.CenaNetto > p.Cena.CenaNetto)
                    {
                        wynik[p.IdProduktu] = p;
                    }
                }
            }
            bool odwrotnaKolejnoscJednostek = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(aktualnyKlient, TypUstawieniaKlienta.SposobSortowaniaJednostek);
            IList<Tuple<IProduktKlienta, ParametryIlosciProduktu>> lista = new List<Tuple<IProduktKlienta, ParametryIlosciProduktu>>();
            HashSet<long> ids = new HashSet<long>(wynik.Keys);
            var bazowe = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(aktualnyJezyk, null, x => ids.Contains(x.Id));

            foreach (var p in bazowe)
            {
                var cena = wynik[p.Id].Cena;
                ProduktKlientZaPunktyLubGratis pk = new ProduktKlientZaPunktyLubGratis(p, cena, aktualnyKlient);
                var paril = new ParametryIlosciProduktu(pk, odwrotnaKolejnoscJednostek, TypPozycjiKoszyka.Gratis, PobierzParametryPrzyciskowDodawania(koszyk, pk.Id, TypPozycjiKoszyka.Gratis, aktualnyKlient, aktualnyJezyk, null));
                Tuple<IProduktKlienta, ParametryIlosciProduktu> ob = new Tuple<IProduktKlienta, ParametryIlosciProduktu>(pk, paril);
                lista.Add(ob);
            }
            return lista;
        }

        public DodawanieProduktuPrzyciski PobierzParametryPrzyciskowDodawania(IKoszykiBLL koszyk, long produkt, TypPozycjiKoszyka typ, IKlient klient, int jezyk, bool? dodawanieTekstowe, string tekstprzyciskbrak = "Dodaj do koszyka",
            string tekstprzyciskjest = "W koszyku")
        {
            decimal ileJestWKoszyku = 0;
            koszyk.PobierzIlosciProduktowWKoszyku().TryGetValue(produkt, out ileJestWKoszyku);
            bool jestwkoszyku = ileJestWKoszyku > 0;
            bool dodawanietxt;
            if (dodawanieTekstowe.HasValue)
            {
                dodawanietxt = dodawanieTekstowe.Value;
            }
            else
            {
                dodawanietxt = Calosc.Konfiguracja.TekstowySposobPokazywaniaPrzyciskuDoKoszyka;
            }
            return new DodawanieProduktuPrzyciski(dodawanietxt, tekstprzyciskbrak, tekstprzyciskjest, jestwkoszyku, ileJestWKoszyku);
        }


        public void WykonajZadaniaNaPozycjiZamowienia<T>(ZamowieniaProduktyBLL pozycja, KoszykPozycje koszykPozycja, IList<ZadaniaPozycjiZamowienia>zadania) where T : IGrupaZadania
        {
            foreach (var zadanie in zadania)
            {
                if (zadanie.CzySpelniaKryteria(koszykPozycja, pozycja))
                {
                    zadanie.Wykonaj(koszykPozycja, pozycja);
                }
            }
        }


        public bool WykonajZadaniaNaKoszyku<T>(IKoszykiBLL koszyk) where T: IGrupaZadania 
        {
            bool czyMoznaFinalizowac = true;
            Dictionary<ModelBLL.Interfejsy.IKoszykPozycja, List<ZadaniePozycjiKoszyka>> pozycjeZadania = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.PobierzZadaniaPozycjiKtorePasuja<T>(koszyk);
            foreach (var kp in pozycjeZadania)
            {
                foreach (var zadanieKoszyka in kp.Value)
                {
                    if (!zadanieKoszyka.Wykonaj(kp.Key, koszyk))
                    {
                        czyMoznaFinalizowac = false;
                    }
                }
            }
            var zadania = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.PobierzZadaniaCalegoKoszykaKtorePasuja<T>(koszyk);
            foreach (var zadanieKoszyka in zadania)
            {
                if (!zadanieKoszyka.Wykonaj(koszyk))
                {
                    czyMoznaFinalizowac = false;
                }
            }
            UaktualnijKoszyk(koszyk as KoszykBll);
            return czyMoznaFinalizowac;
        }
        
        public void FinalizacjaKoszyka(KoszykBll koszyk, IKlient aktualny, out bool akcepacja, IKlient klient, IKlient przedstawiciel)
        {
            akcepacja = false;
            //todo: czemu tu jest ZresetujKoszyk(koszyk); ?? - bartek komentuje
           // koszyk = ZresetujKoszyk(koszyk);
            if (WyslijKoszykDoAkceptacji(koszyk, aktualny))
            {
                akcepacja = true;
            }
            else
            {
                FinalizujKoszyk(koszyk, klient,przedstawiciel);
            }
        }

        public IList<KoszykPozycje> UzupelnijPozycjePoSelect(int jezykId, IKlient zadajacy, IList<KoszykPozycje> objekty, object parametrDoMetodyPoSelect = null)
        { 
            var obiektKoszyka = parametrDoMetodyPoSelect as KoszykBll;
            if (objekty == null || !objekty.Any())
            {
                return objekty;
            }

            //Nie mozna przypisać waluty z koszyka gdyż jest ona pobierana z pierwszej pozycji koszyka.
            foreach (KoszykPozycje pozycja in objekty)
            {
                if (obiektKoszyka == null)
                {
                    throw new Exception("Ne można pobierać pozycji BLL, bez podania obiektu koszyka dla którego jest pobierana pozycja.");
                }
                pozycja.Klient = obiektKoszyka.Klient;
            }
            
            return objekty;
        }

        public decimal? PozostalyLimitWartosciZamowien(IKlient klient, IKoszykiBLL koszyk, out decimal przekroczono)
        {
            przekroczono = 0;
            SzablonLimitow limitCalkowity = Calosc.Klienci.PobierzCalkowityLimitWartosciZamowien(klient);
            if (limitCalkowity == null || limitCalkowity.WartoscZamowien==null)
            {
                return null;
            }
            decimal? limitWykorzystany = SolexBllCalosc.PobierzInstancje.Klienci.PobierzWykorzystanyLimit<decimal?>(klient, limitCalkowity, RodzajLimitu.LimitWartosciZamowien);
            decimal pozostalylimit = limitCalkowity.WartoscZamowien.Value - (limitWykorzystany!=null? limitWykorzystany.Value:0m);
            przekroczono = pozostalylimit - koszyk.CalkowitaWartoscHurtowaNettoPoRabacie();
            if (przekroczono > 0)   //nie wykorzystliśmy limitu, czyli przekroczone o 0
            {
                przekroczono = 0;
            }
            przekroczono = Math.Abs(przekroczono);
            return pozostalylimit;
        }

        public int? PozostalyLimitIloscZamowien(IKlient k)
        {
            //int? limitCalkowity = Calosc.Klienci.PobierzCalkowityLimitIloscZamowien(k);
            SzablonLimitow limit = Calosc.Klienci.PobierzCalkowityLimitIloscZamowien(k);
            if (limit == null || limit.IloscZamowien == null)
            {
                return null;
            }
            int? limitWykorzystany = Calosc.Klienci.PobierzWykorzystanyLimit<int?>(k, limit, RodzajLimitu.LimitIlosciZamowien);
            if (limitWykorzystany.HasValue)
            {
                int pozostalylimit = limit.IloscZamowien.Value - limitWykorzystany.Value;
                return pozostalylimit;
            }
            return limit.IloscZamowien.Value;
        }

        public bool PrzekroczoneLimityKoszyka(IKoszykiBLL koszyk)
        {
            int? limitilosc = PozostalyLimitIloscZamowien(koszyk.Klient);
            decimal przekoczony;
            decimal? limitwartosc = PozostalyLimitWartosciZamowien(koszyk.Klient, koszyk, out przekoczony);
            if ((limitilosc == null || limitilosc.Value > 0) && (limitwartosc == null || przekoczony == 0))
            {
                return false;
            }
            return true;   //znaczy że limity przekroczone
        }

        public bool MoznaFinalizowacKoszykPrzezLimity(IKoszykiBLL koszyki)
        {
            bool limityprzekoczone = PrzekroczoneLimityKoszyka(koszyki);
            SzablonAkceptacjiBll szablon = Calosc.Klienci.PobierzSzablonAkceptacji(koszyki.Klient, limityprzekoczone);
            return !limityprzekoczone || szablon != null;
        }
    }
}