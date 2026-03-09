using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.BLL
{
    public class KlienciDostep : LogikaBiznesBaza, IKlienciDostep
    {
        public KlienciDostep(ISolexBllCalosc calosc) : base(calosc){}

        public Klient KlientNiezalogowany()
        {
            var tmo = new Klient(null) {Nazwa = "", JezykId = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny, Id = 0, Symbol = "NIEZALOGOWANY", WalutaId = Calosc.Konfiguracja.SlownikWalut.ElementAt(0).Key};
            return tmo;
        }

        /// <summary>
        ///     Zwraca obiekt klienta wg podanego ID, lub jesli pusty to zalogowanego aktualnie klienta
        /// </summary>
        /// <param name="klientID"></param>
        /// <returns></returns>
        public virtual IKlient PobierzWgIdLubZalogowanyAktualnie(long klientID)
        {
            IKlient klient = Calosc.DostepDane.PobierzPojedynczy<Klient>(klientID);
            return klient;
        }

        public virtual Klient PobierzPologinie(string login, string hasloOdkryte = null, string hasloMd5 = null)
        {
            if (string.IsNullOrEmpty(login))
            {
                return null;
            }
            var klient = Calosc.DostepDane.Pobierz<Klient>(null, x => (x.Login != null) && (x.Login == login)).FirstOrDefault();
            if (klient == null)
            {
                return null;
            }
            return WalidujHaslo(klient, hasloOdkryte, hasloMd5);
        }

        private Klient WalidujHaslo(Klient klient, string hasloOdkryte, string hasloMd5)
        {
            if (hasloOdkryte == null)
            {
                return klient;
            }
            if (string.IsNullOrEmpty(hasloMd5))
            {
                if (!string.IsNullOrEmpty(hasloOdkryte))
                {
                    hasloMd5 = Tools.PobierzInstancje.GetMd5Hash(hasloOdkryte);
                }
            }
            if (klient.HasloKlienta == hasloMd5)
            {
                return klient;
            }

            //druga opcja - sprawdzanie czy haslo wpisane odkryte == klucz api - jesli tak to zaloguje sie. Bartek to robi dlatego zeby mozna bylo robic testy obciazenia itp
            if (klient.KluczSesji == hasloOdkryte)
            {
                return klient;
            }
            return null;
        }

        public Klient Pobierz(long id, IKlient zadajacy)
        {
            return Calosc.DostepDane.PobierzPojedynczy<Klient>(id, zadajacy);
        }

        private void ZweryfikujKlienta(Model.Klient klient)
        {
            if (string.IsNullOrEmpty(klient.Symbol))
            {
                klient.Symbol = "auto-" + Guid.NewGuid();
            }
            if (!klient.Role.Any())
            {
                klient.Role = new HashSet<RoleType> {RoleType.Klient};
            }
            if (klient.Id == klient.OpiekunId)
            {
                klient.OpiekunId = null;
            }
            if (klient.Id == klient.PrzedstawicielId)
            {
                klient.PrzedstawicielId = null;
            }
            if (klient.Id == klient.DrugiOpiekunId)
            {
                klient.DrugiOpiekunId = null;
            }
            if (klient.Id == klient.KlientNadrzednyId)
            {
                klient.KlientNadrzednyId = null;
            }
            if (string.IsNullOrEmpty(klient.Login))
            {
                klient.Login = klient.Email;
            }
            if (!klient.Aktywny)
            {
                klient.Login = null;
                klient.Email = null;
            }
        }

        public bool SprawdzenieMaila(IKlienci klient, IEnumerable<IKlienci> wszyscy)
        {
            if (string.IsNullOrEmpty(klient.Email) && !klient.Aktywny) //nieakywni mają mieć pusty mail
            {
                return true;
            }
            if (string.IsNullOrEmpty(klient.Email) && klient.Aktywny) //tylko aktywni nie mogą mieć pustego maila
            {
                return false;
            }
            if (!klient.Email.PoprawnyAdresEmail())
            {
                return false; //walidacja czy mail jest w poprawnym formacie
            }
            var sprMaila = wszyscy.FirstOrDefault(x => (x.Email == klient.Email) && x.Aktywny); //może  istnieć niektywny z powielonym mailem, ale
            if ((sprMaila != null) && (sprMaila.Id != klient.Id))
            {
                return false;
            }
            return true;
        }

        public bool ZmienHaslo(IKlient customer, string nowehasloodkryte)
        {
            var zmieniane = Tools.PobierzInstancje.GetMd5Hash(nowehasloodkryte);
            if ((zmieniane != customer.HasloKlienta) || customer.Role.Contains(RoleType.Administrator))
            {
                customer.HasloKlienta = zmieniane;
                customer.Gid = null;
                var k = new Klient(customer as Klient);
                k.DataZmianyHasla = DateTime.Now;
                Calosc.DostepDane.AktualizujPojedynczy(k);
                return true;
            }
            return false;
        }

        private void UstawGid(IKlienci customer)
        {
            if (string.IsNullOrEmpty(customer.Gid))
            {
                customer.Gid = Guid.NewGuid().ToString();
            }
        }

        public void ResetHasla(IKlient klient, string nowehasloodkryte = null)
        {
            if (!string.IsNullOrEmpty(nowehasloodkryte))
            {
                var haslo = nowehasloodkryte;
                klient.HasloKlienta = haslo;
                PoprawHasloKlienta(klient, haslo);
            }
            UstawGid(klient);
            Calosc.DostepDane.AktualizujPojedynczy(klient as Klient);
            SolexBllCalosc.PobierzInstancje.Statystyki.ZdarzenieResetHasla(klient);
        }

        private void PoprawHasloKlienta(IKlienci klient, string haslo)
        {
            if (!string.IsNullOrEmpty(haslo))
            {
                var zmieniane = Tools.PobierzInstancje.GetMd5Hash(haslo);
                klient.HasloKlienta = zmieniane;
                klient.DataZmianyHasla = DateTime.Now;
            }
        }

        public IList<Klient> PobierzKlientowRoli(RoleType rola, long? oddzialKlientNadrzedny = null)
        {
            Klient k = null;
            if (oddzialKlientNadrzedny == 0)
            {
                oddzialKlientNadrzedny = null;
            }
            if (oddzialKlientNadrzedny.HasValue)
            {
                var x = Calosc.DostepDane.PobierzPojedynczy<Klient>(oddzialKlientNadrzedny.Value);
                if ((x != null) && x.Role.Contains(rola))
                {
                    k = x;
                }
            }
            var wynik = Calosc.DostepDane.Pobierz<Klient>(null, x => (x.KlientNadrzednyId == oddzialKlientNadrzedny) && x.Role.Contains(rola));
            if ((k != null) && wynik.All(x => x.Id != k.Id))
            {
                wynik.Insert(0, k);
            }
            return wynik.ToList();
        }

        public List<Podpowiedz> ZnajdzDoZarzadania(string searchString, int count, int lang, IKlient zadajacy)
        {
            var lista =
                Calosc.DostepDane.Pobierz<Model.Klient>(null, q => q.Nazwa.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase) || q.Symbol.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase)).ToList();
            IList<Klient> pasujace = new List<Klient>();
            if (lista.Any())
            {
                pasujace = Calosc.DostepDane.Pobierz<Klient>(zadajacy, x => lista.Select(y => y.Id).Contains(x.Id));
            }
            return pasujace.Select(s => new Podpowiedz {Klucz = s.Id.ToString(CultureInfo.InvariantCulture), Wartosc = s.Nazwa}).ToList();
        }

        public string KluczDoKlientaWypisanieZapisaniaZNewsletera(IKlient klient)
        {
            var zmienna1 = klient.Email.WygenerujIDObiektuSHAWersjaLong() + (klient.Email + "SOLEX-EVER").WygenerujIDObiektuSHAWersjaLong()/4 - (klient.Email + "złam to wacku").WygenerujIDObiektuSHAWersjaLong()%6;
            if (klient != null)
            {
                zmienna1 += klient.Id;
                zmienna1 -= klient.Id*8;
                zmienna1 += klient.Id.ToString().WygenerujIDObiektuSHAWersjaLong();
            }
            return zmienna1.ToString();
        }

        public virtual IKlient SztucznyAdministrator()
        {
            //muismy miec jakeigos klienta juz zrobionego zeby walidac na nim
            var k = new Klient {Nazwa = "admin", Symbol = "admin", Email = "admin@solex.net.pl", Aktywny = true, HasloKlienta = "202cb962ac59075b964b07152d234b70", HasloZrodlowe = "202cb962ac59075b964b07152d234b70"};
            if (Calosc.Konfiguracja.SlownikWalut.Any())
            {
                k.WalutaId = Calosc.Konfiguracja.SlownikWalut.ElementAt(0).Key;
            }
            k.Role.Add(RoleType.Administrator);
            k.Role.Add(RoleType.Pracownik);
            return k;
        }

        public void WygenerujKluczeAdministratorom()
        {
            var admini = Calosc.DostepDane.Pobierz<Klient>(SztucznyAdministrator(), x => x.Role.Contains(RoleType.Administrator)).ToList();
            if (!admini.Any())
            {
                var admin = new Klient(SztucznyAdministrator() as Model.Klient);
                admin.Id = -1;
                admin.JezykId = Calosc.Konfiguracja.JezykIDPolski;
                using (var trans = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.OpenTransaction())
                {
                    Calosc.DostepDane.DbORM.Insert<Model.Klient>(admin);
                    trans.Commit();
                }
                admini.Add(admin);
            }
            var adminu = new List<Model.Klient>();
            foreach (var klienci in admini)
            {
                if (string.IsNullOrEmpty(klienci.KluczSesji))
                {
                    WygenerujKlucz(klienci);
                }
            }
        }

        public void WygenerujKlucz(Klient customer)
        {
            try
            {
                customer.KluczSesji = Guid.NewGuid().ToString();
                customer.DataZmianyKlucza = DateTime.Now;
                Calosc.DostepDane.AktualizujPojedynczy(customer);
                Calosc.Statystyki.ZdarzenieGenerowanieKluczaApi(customer);
            } catch (Exception e)
            {
                Calosc.Log.ErrorFormat("Nie udało się stworzyć klucza API dla klienta id: {0}", customer.Id);
                Calosc.Log.Error(e);
                throw;
            }
        }

        public void WygenerujBrakujaceLoginy()
        {
            var admini = Calosc.DostepDane.Pobierz<Klient>(SztucznyAdministrator(), x => (x.Login == null) || (x.Login == "") || (x.Email == "") || (x.Email == null) || (!x.Aktywny && (x.Login != null) && (x.Login != "")));
            var adminu = new List<Klient>();
            foreach (var klienci in admini)
            {
                if (klienci.Aktywny)
                {
                    if (string.IsNullOrEmpty(klienci.Login) && !string.IsNullOrEmpty(klienci.Email))
                    {
                        klienci.Login = klienci.Email;
                        adminu.Add(new Klient(klienci));
                    }
                }
                else if (!klienci.Aktywny && !string.IsNullOrEmpty(klienci.Login))
                {
                    klienci.Login = null;
                    adminu.Add(new Klient(klienci));
                }
            }
            if (adminu.Any())
            {
                Calosc.DostepDane.AktualizujListe(adminu);
            }
        }

        /// <summary>
        ///     Wysyla maile powitalne od szefa
        /// </summary>
        /// <param name="wywolujacy"></param>
        public void WyslijMailePowitalneOdSzefa(IKlienci wywolujacy)
        {
            var maxdniWstecz = 4;
            var najwszesniejszadata = DateTime.Now.AddDays(-maxdniWstecz).Date;

            //Musi upłynąć conajmniej 1 dzień od dodania
            IEnumerable<IKlient> klienci = Calosc.DostepDane.Pobierz<Klient>(null, x => x.Aktywny && (x.DataDodatnia != null) && (x.DataDodatnia > najwszesniejszadata) && (x.DataDodatnia < DateTime.Now.AddDays(-1)));
            foreach (var k in klienci)
            {
                var zdarzenie = Calosc.Statystyki.ZnajdzZdarzenie(ZdarzenieGlowne.PowitanieOdSzefa, "Klient", k.Nazwa);
                var wyslany = (zdarzenie != null) && (zdarzenie.Count > 0);
                if (!wyslany)
                {
                    Calosc.Statystyki.ZdarzeniePowitanieSzef(k);
                }
            }
        }
        
        public virtual void SprawdzAdresIpKlienta(Klient c)
        {
            var zmianaAdresuPowiadomienie = new ZmianaAdresuIP();
            var powiadomienieEmail = Calosc.DostepDane.PobierzPojedynczy<UstawieniePowiadomienia>(zmianaAdresuPowiadomienie.Id);
            if (!Calosc.Konfiguracja.GetLicense(Licencje.ZmianaIp))
            {
                return;
            }
            if (c.Kategorie.Contains(Calosc.Konfiguracja.KategoriaKlientaNieBlokujZmianaIp))
            {
                return;
            }
            if ((powiadomienieEmail != null) && !powiadomienieEmail.ParametryWysylania.First(x => x.DoKogo == TypyPowiadomienia.Klient).Aktywny)
            {
                return;
            }
            if (!((c.Role.Count == 1) && (c.Role.First() == RoleType.Klient)))
            {
                return;
            }
            var logowania = Calosc.Statystyki.ZnajdzZdarzenie(ZdarzenieGlowne.LogowanieDoSystemu_Udane, "Klient", c.Login);
            if (logowania != null)
            {
                var zdarzenie = logowania.OrderByDescending(x => x.Id).First();
                if (!zdarzenie.IpKlienta.Equals(SesjaHelper.PobierzInstancje.IpKlienta))
                {
                    c.BlokadaZamowien = true;
                    c.PowodBlokady = BlokadaPowod.ZmianaAdresuIp;
                    c.GidIp = Tools.PobierzInstancje.GetMd5Hash(SesjaHelper.PobierzInstancje.IpKlienta);
                    try
                    {
                        Calosc.Statystyki.ZdarzenieZmianaAdresuIP(c, SesjaHelper.PobierzInstancje.IpKlienta, zdarzenie.IpKlienta);
                        Calosc.DostepDane.AktualizujPojedynczy(c);
                    } catch (Exception e)
                    {
                        Calosc.Log.ErrorFormat("Błąd zapisu inforamcji o zmianie adresu IP dla klienta: {0}", c.Id);
                        Calosc.Log.Error(e);
                    }
                }
            }
        }

        public bool ResetGid(string gid)
        {
            var hashzaktualnegoip = Tools.PobierzInstancje.GetMd5Hash(SesjaHelper.PobierzInstancje.IpKlienta);
            var c = Calosc.DostepDane.Pobierz<Klient>(null, x => x.GidIp != null && x.GidIp == gid && x.GidIp == hashzaktualnegoip).FirstOrDefault();
            if (c != null)
            {
                c.PowodBlokady = BlokadaPowod.Brak;
                c.GidIp = null;
                c.BlokadaZamowien = false;
                Calosc.DostepDane.AktualizujPojedynczy(c);
                //UpdateCustomers(new List<Model.Klient> {new Model.Klient(c)});
                DodajZdarzenieLogowannie(c.Login, true);
                return true;
            }
            return false;
        }

        public bool JestOpiekunem(long odbiorca, IKlient opiekun)
        {
            IKlient k;
            try
            {
                k = Calosc.DostepDane.PobierzPojedynczy<Klient>(odbiorca);
            } catch (Exception)
            {
                return false;
            }
            return (k.OpiekunId == opiekun.Id) || (k.PrzedstawicielId == opiekun.Id) || (k.DrugiOpiekunId == opiekun.Id);
        }

        public virtual void DodajZdarzenieLogowannie(string login, bool ok)
        {
            Calosc.Statystyki.DodajZdarzenie(ok ? ZdarzenieGlowne.LogowanieDoSystemu_Udane : ZdarzenieGlowne.LogowanieDoSystemu_Nieudane, "Klient", login, new Model.Klient {Email = login});
        }

        public List<IKlient> PobierzKontaDoZarzadania(IKlient klient)
        {
            var ids = klient.WszystkieKontaPodrzedne().Select(x => x.Id).ToList();
            ids.Add(klient.Id);
            var zarzadane = new List<IKlient>();
            if ((klient.KlientNadrzednyId != null) && (klient.SubkontaAdministrator != SubkontaRodzajAdministratora.Brak))
            {
                var wynik = new List<IKlient>();
                if (klient.SubkontaAdministrator == SubkontaRodzajAdministratora.Glowny) //glowny widzi wszystkich
                {
                    wynik = klient.KlientNadrzedny.WszystkieKontaPodrzedne();
                }
                else if (klient.SubkontoGrupaId != null)
                {
                    wynik = Calosc.DostepDane.Pobierz<Klient>(null, x => x.SubkontoGrupaId == klient.SubkontoGrupaId).Select(x => x as IKlient).ToList();
                }
                zarzadane = wynik.Where(x => !ids.Contains(x.Id)).OrderBy(x => x.Nazwa).ToList();
            }
            return zarzadane;
        }

        /// <summary>
        ///     Lista uprawnień do adminia dla określonego klienta
        /// </summary>
        /// <param name="klient">Klient</param>
        /// <returns></returns>
        public HashSet<string> UprawniniaKlienta(IKlient klient)
        {
            HashSet<string> uprawnia;
            if (string.IsNullOrEmpty(klient.DostepneModulyAdmina))
            {
                //if (klient.Role.Contains(RoleType.Oddzial))
                //{
                //    uprawnia = new HashSet<string>(Calosc.Konfiguracja.DomyslneUprawnieniaOddzial);
                //}
                if (klient.Role.Contains(RoleType.Przedstawiciel))
                {
                    uprawnia = new HashSet<string>(Calosc.Konfiguracja.DomyslneUprawnieniaPrzedstawiciel);
                }
                else if (klient.Role.Contains(RoleType.Pracownik))
                {
                    uprawnia = new HashSet<string>(Calosc.Konfiguracja.DomyslneUprawnieniaPracownik);
                }
                else
                {
                    throw new HttpException(403, "Brak uprawnień");
                    // throw new Exception("Coś poszło nie tak");
                }
            }
            else
            {
                uprawnia = Serializacje.PobierzInstancje.DeSerializeList<string>(klient.DostepneModulyAdmina, ";");
            }
            return uprawnia;
        }
        
        /// <summary>
        ///     Czy klient ma dostęp do wybranej strony
        /// </summary>
        /// <param name="klient">Klient</param>
        /// <param name="modul">Moduł</param>
        /// <returns></returns>
        public bool MaDostepDoModuluAdmina(IKlient klient, NavigationItem modul)
        {
            if (klient.Role.Contains(RoleType.Administrator))
            {
                return true;
            }
            if (klient.Role.Contains(RoleType.Oddzial) && !modul.WidoczneOddzial)
            {
                return false;
            }
            var uprawnienia = UprawniniaKlienta(klient);
            if (uprawnienia.Contains(modul.NazwaModuluWykrywanie))
            {
                return true;
            }
            return false;
        }

        public bool CzyKlientIstnieje(int id)
        {
            IKlient k = Calosc.DostepDane.PobierzPojedynczy<Klient>(id);
            return k != null;
        }

        private bool _czySaJakiesSzablonyKatalogi = true;
        private ConcurrentDictionary<long, List<KatalogSzablonModelBLL>> widocznosciKatalogowDlaKlientow;

        public virtual List<KatalogSzablonModelBLL> PobierzSzablonyWidoczneDlaKlienta(IKlient zadajacy)
        {
            if (_czySaJakiesSzablonyKatalogi == false)
            {
                return null;
            }
         
            if (widocznosciKatalogowDlaKlientow == null)
            {
                _czySaJakiesSzablonyKatalogi = Calosc.DostepDane.DbORM.Count<KatalogSzablonModelBLL>(x => x.Aktywny) > 0;
                widocznosciKatalogowDlaKlientow = new ConcurrentDictionary<long, List<KatalogSzablonModelBLL>>();

                if (!_czySaJakiesSzablonyKatalogi)
                {
                    return null;
                }
            }

            List<KatalogSzablonModelBLL> szablonyKlienta;
            if (!widocznosciKatalogowDlaKlientow.TryGetValue(zadajacy.Id, out szablonyKlienta))
            {
                szablonyKlienta = Calosc.DostepDane.Pobierz<KatalogSzablonModelBLL>(zadajacy, x => x.Aktywny).ToList();
                //spradzenie dodatkowe roli poki kontrolka widzonsoci tego nie robi
              
                szablonyKlienta = szablonyKlienta.Where(x => x.DostepnyDla == null || x.DostepnyDla.Overlaps(zadajacy.Role)).ToList();
                widocznosciKatalogowDlaKlientow.TryAdd(zadajacy.Id, szablonyKlienta);
            }
            return szablonyKlienta;
        }

        //walidatro dla klientów - warunki dla ktorych jak sa spelnione to klient jest widoczny
        public Expression<Func<Klient, IKlient, bool>> WalidatorKlientow
        {
            get
            {
                return (klient, zadajacy) => zadajacy.Role.Contains(RoleType.Administrator) || (zadajacy.Id == klient.Id) || ((klient.Id != 0) && //id != 0
                                                                                                                              (zadajacy.WidziWszystkich ||
                                                                                                                               (zadajacy.Role.Contains(RoleType.Przedstawiciel) &&
                                                                                                                                ((klient.OpiekunId == zadajacy.Id) || (klient.PrzedstawicielId == zadajacy.Id) ||
                                                                                                                                 (klient.DrugiOpiekunId == zadajacy.Id))) || (klient.KlientNadrzednyId == zadajacy.Id)));
            }
        }

        //todo: testy tego - nie wiadomo co sie tu dzieje
        public IList<Klient> BindPoSelectKlieta(int jezykId, IKlient zadajacy, IList<Klient> obj, object parametrDoMetodyPoSelect = null)
        {
            for (var i = 0; i < obj.Count; ++i)
            {
                var klient = obj[i];
                klient.Komunikaty = Calosc.KomunikatyBll.PobierzKomunikatyKlienta(klient);
                if (klient.Id != 0)
                {
                    //waluta jest obowiazkowa dla klientow
                    klient.WalutaKlienta = Calosc.Konfiguracja.SlownikWalut[klient.WalutaId];
                }
                if (klient.DostepneMagazyny != null)
                {
                    klient.DostepneMagazynyDlaKlienta = Calosc.Konfiguracja.SlownikMagazynowPoSymbolu.WhereKeyIsIn(klient.DostepneMagazyny);
                }

                // sprawdzanie czy klient ma inywidualizowaną ofertę
                if (((Calosc.Konfiguracja.WirtualneProduktyProvider != null) && Calosc.Konfiguracja.WirtualneProduktyProvider.WplywaNaWidocznoscProduktowDlaKlienta) ||
                    !klient.PelnaOferta || ((Calosc.Konfiguracja.GetLicense(Licencje.moj_katalog) || Calosc.Konfiguracja.GetLicense(Licencje.katalog_klienta)) && CzyKlienMaOferte(klient.Id)) )
                {
                    klient.OfertaIndywidualizowana = true;
                }

                //------------------------
                //PONIEJ JUZ WALIDACJA - jesli jest podany klient zadajacy
                //------------------------
                if (zadajacy == null)
                {
                    //todo: przywroci walidacje - ale duzo poprawek w kodzie :(
                    continue;
                }
                if (zadajacy.CzyAdministrator || (klient.Id == zadajacy.Id))
                {
                    continue;
                }

                //bartek zmiana - bylo wyzej dotychczas
                if (klient.Dostep == AccesLevel.Niezalogowani)
                {
                    obj.RemoveAt(i);
                    --i;
                    continue;
                }
                if (zadajacy.Role.Contains(RoleType.Przedstawiciel) || (zadajacy.OddzialDoJakiegoNalezyKlient != 0))
                {
                    //jak widzi wszystkich to nie sprawdzamy czy jest opiekunem / przedstawiciel
                    if (zadajacy.WidziWszystkich && (zadajacy.OddzialDoJakiegoNalezyKlient == klient.OddzialDoJakiegoNalezyKlient))
                    {
                        continue;
                    }
                    if (((klient.Opiekun != null) && (klient.Opiekun.Id == zadajacy.Id)) || ((klient.Przedstawiciel != null) && (klient.Przedstawiciel.Id == zadajacy.Id)) ||
                        ((klient.DrugiOpiekun != null) && (klient.DrugiOpiekun.Id == zadajacy.Id)))
                    {
                        continue;
                    }
                }
                if ((zadajacy.Role.Count == 1) && zadajacy.Role.Contains(RoleType.Klient))
                {
                    if (klient.KlientNadrzednyId != zadajacy.Id)
                    {
                        obj.RemoveAt(i);
                        --i;
                    }
                }
            }
            return obj;
        }

        private HashSet<long> _klienciZOferta;

        /// <summary>
        ///     sprawdzamy czy klient ma oferte indywidualizowaną
        /// </summary>
        /// <param name="idKlienta"></param>
        /// <returns></returns>
        private bool CzyKlienMaOferte(long idKlienta)
        {
            if (_klienciZOferta == null)
            {
                //pobieramy id klientów którzy mają produkty ukryte lub czy są w kategorii która ma produkty ukryte
                var sql = @"SELECT KlientZrodloId FROM ProduktUkryty pu WHERE KlientZrodloId is not null AND (Tryb like 'Wykluczenia' OR Tryb like 'Dostepne')
                                UNION 
                                SELECT KlientId FROM KlientKategoriaKlienta WHERE KategoriaKlientaId IN (SELECT KategoriaKlientowId  FROM ProduktUkryty  WHERE KategoriaKlientowId is NOT NULL AND Tryb like 'Wykluczenia' OR Tryb like 'Dostepne')";
                _klienciZOferta = new HashSet<long>( Calosc.DostepDane.DbORM.SqlList<long>(sql) );
            }
            return _klienciZOferta.Contains(idKlienta);
        }

        public void UsunCacheKlienci(IList<object> obj)
        {
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<Model.Klient>());
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<Klient>());
        }

        public IList<Pracownik> PobierzPracownikow(int jezyk, IKlient zadajacy)
        {
            return Calosc.DostepDane.Pobierz<Klient>(jezyk, zadajacy, x => x.Role.Contains(RoleType.Pracownik)).Select(x => new Pracownik(x)).ToList();
        }

        public IList<AdministratorBLL> PobierzAdministratorow(int jezyk, IKlient zadajacy)
        {
            return Calosc.DostepDane.Pobierz<Klient>(jezyk, zadajacy, x => x.Role.Contains(RoleType.Administrator)).Select(x => new AdministratorBLL(x)).ToList();
        }

        public void PrzedAktualizacjaPracownikow(IList<Pracownik> obj)
        {
            foreach (var pracownik in obj)
            {
                if (pracownik.DomyslnyDrugiOpiekun)
                {
                    Calosc.Konfiguracja.IdDrugiDomyslnyOpiekun = pracownik.Id;
                }
                else if (pracownik.Id == Calosc.Konfiguracja.IdDrugiDomyslnyOpiekun)
                {
                    Calosc.Konfiguracja.IdDrugiDomyslnyOpiekun = 0;
                }
                if (pracownik.DomyslnyOpiekun)
                {
                    Calosc.Konfiguracja.IdDomyslnyOpiekun = pracownik.Id;
                }
                else if (pracownik.Id == Calosc.Konfiguracja.IdDomyslnyOpiekun)
                {
                    Calosc.Konfiguracja.IdDomyslnyOpiekun = 0;
                }
                if (pracownik.DomyslnyPrzedstawiciel)
                {
                    Calosc.Konfiguracja.IdDomyslnyPrzedstawiciel = pracownik.Id;
                }
                else if (pracownik.Id == Calosc.Konfiguracja.IdDomyslnyPrzedstawiciel)
                {
                    Calosc.Konfiguracja.IdDomyslnyPrzedstawiciel = 0;
                }
                if (!pracownik.Role.Any())
                {
                    pracownik.Role = new HashSet<RoleType> {RoleType.Klient, RoleType.Pracownik};
                }
                else
                {
                    pracownik.Role.Add(RoleType.Pracownik);
                }
                var wartosc = string.Empty;
                if ((pracownik.JakieElementyMenu != null) && pracownik.JakieElementyMenu.Any())
                {
                    foreach (var el in pracownik.JakieElementyMenu)
                    {
                        wartosc += string.Format("{0};", el);
                    }
                }
                pracownik.DostepneModulyAdmina = wartosc;
                PoprawHasloKlienta(pracownik, pracownik.HasloOdkryte);
            }
        }

        public void PrzedAktualizacjaKlientow(IList<Model.Klient> obj)
        {
            AkcejDlaKlientaPrzedAktualizacja(obj);
        }

        public void PrzedAktualizacjaKlientow(IList<Klient> obj)
        {
            AkcejDlaKlientaPrzedAktualizacja(obj.Select(x => (Model.Klient) x).ToList());
        }

        private void AkcejDlaKlientaPrzedAktualizacja(IList<Model.Klient> obj)
        {
            //przed aktualizacją wrzucone jest to co kiedyś robione było w UpdateCustomers
            //spradzamy tylko po emailu i id dublowanych klientow
            var listaEmaili = obj.Select(x => x.Email).Distinct().ToList();
            var ids = obj.Select(x => x.Id).Distinct().ToList();
            var loginy = obj.Select(x => x.Login).Distinct().ToList();
            obj.OperacjeNaPolachTekstowych();
            HashSet<long> klienciNieAktywni = new HashSet<long>();
            var existing = Calosc.DostepDane.Pobierz<Klient>(SztucznyAdministrator(), x => Sql.In(x.Id, ids) || Sql.In(x.Login, loginy) || Sql.In(x.Email, listaEmaili));
            foreach (var k in obj)
            {
                ZweryfikujKlienta(k);
                if (!SprawdzenieMaila(k, existing))
                {
                    Calosc.Log.ErrorFormat("Próba zdublowania lub wpisania pustego maila. Mail: {0} id klient {1}, symbol {2}", k.Email, k.Id, k.Symbol);
                    continue;
                }
                var gid = Guid.NewGuid().ToString();
                IKlient c = existing.FirstOrDefault(p => p.Id == k.Id);
                if (c == null)
                {
                    k.HasloKlienta = k.HasloZrodlowe;
                    k.Gid = gid;
                    k.DataDodatnia = DateTime.Now;
                    k.ZgodaNaNewsletter = Calosc.Konfiguracja.DomyslnaZgodaNaNewsletter;
                    k.JezykId = Calosc.DostepDane.Pobierz<Jezyk>(null).Select(x => x.Id).FirstOrDefault();
                }
                else
                {
                    k.DataDodatnia = c.DataDodatnia ?? DateTime.Now;
                    //k.JezykId = c.JezykId;
                    if (k.Aktywny && ((k.HasloZrodlowe != c.HasloZrodlowe) || !string.IsNullOrEmpty(k.HasloOdkryte)))
                    {
                        k.HasloKlienta = k.HasloZrodlowe;
                        k.DataZmianyHasla = DateTime.Now;
                        k.Login = k.Email;
                        if (string.IsNullOrEmpty(k.Gid)) //nowy gid generujemy jeśli nie był już wpisany w klienta
                        {
                            k.Gid = gid;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(k.HasloOdkryte))
                {
                    PoprawHasloKlienta(k, k.HasloOdkryte);
                }
                if (!k.Aktywny)
                {
                    k.Email = k.Login = null;
                    klienciNieAktywni.Add(k.Id);
                }
                if (string.IsNullOrEmpty(k.Email) && string.IsNullOrEmpty(k.Login))
                {
                    k.Aktywny = false;
                    klienciNieAktywni.Add(k.Id);
                }
            }
            if (klienciNieAktywni.Any())
            {
                var klienci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(SztucznyAdministrator(), x => x.KlientNadrzednyId != null && Sql.In(x.KlientNadrzednyId, klienciNieAktywni)).ToList();
                if (!klienci.Any()) return;

                {
                    foreach (Klient klient in klienci)
                    {
                        klient.Aktywny = false;
                        klient.Email = klient.Login = null;
                    }
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(klienci);
                }
            }
        }

        public SzablonLimitow PobierzCalkowityLimitWartosciZamowien(IKlient klient)
        {
            SzablonLimitow grupa;
            var limitklienta = LimityKlient(klient, out grupa);
            SzablonLimitow wynik = null;
            if (grupa != null)
            {
                wynik = grupa;
            }
            if ((limitklienta != null) && limitklienta.WartoscZamowien.HasValue)
            {
                wynik = limitklienta;
            }
            return wynik;
        }

        private readonly string _sqlLimityWartosci = " select COALESCE(SUM(a.wartosc),0) from (select sum(WartoscNetto) as wartosc from HistoriaDokumentu where KlientId in ({0}) and DataUtworzenia>='{1}' " +
                                                     "union all Select sum(WartoscNetto) as wartosc from Zamowienie z " +
                                                     "left join ZamowienieDokumentyERP zd on z.Id=zd.IdZamowienia join StatusZamowienia sz on sz.Id=z.StatusId where KlientId in ({0}) and DataUtworzenia>='{1}' and (z.StatusId= {2} or (zd.IdDokumentu is null and sz.Widoczny=1) or zd.IdDokumentu not in (select Id from HistoriaDokumentu)))a";

        private readonly string _sqlLimityIlosci = "select COALESCE(SUM(a.wartosc),0) from (select count(*) as wartosc  from HistoriaDokumentu where KlientId in ({0}) and DataUtworzenia>='{1}' " +
                                                   "union all Select count(*) as wartosc from Zamowienie z " +
                                                   "left join ZamowienieDokumentyERP zd on z.Id=zd.IdZamowienia join StatusZamowienia sz on sz.Id=z.StatusId where KlientId in ({0}) and DataUtworzenia>='{1}' and (z.StatusId= {2} or (zd.IdDokumentu is null and sz.Widoczny=1) or zd.IdDokumentu not in (select Id from HistoriaDokumentu)))a";

        private T PobierzLimitWykorzystany<T>(IKlient klient, SzablonLimitow szablon, RodzajLimitu rodzajLimitu)
        {
            var wynik = default(T);
            var idKlientow = new HashSet<long> {klient.Id};
            if (!klient.SzablonLimitowId.HasValue)
            {
                idKlientow.UnionWith(PowiazanieKlienciLimity(klient));
            }
            var odKiedyPobierac = WyliczOdKiedy(szablon);
            switch (rodzajLimitu)
            {
                case RodzajLimitu.LimitIlosciZamowien:
                    wynik = Calosc.DostepDane.DbORM.Scalar<T>(string.Format(_sqlLimityIlosci, string.Join(", ", idKlientow), odKiedyPobierac.ToString("yyyy-MM-dd"), (int) StatusImportuZamowieniaDoErp.Złożone));
                    break;
                case RodzajLimitu.LimitWartosciZamowien:
                    wynik = Calosc.DostepDane.DbORM.Scalar<T>(string.Format(_sqlLimityWartosci, string.Join(", ", idKlientow), odKiedyPobierac.ToString("yyyy-MM-dd"), (int) StatusImportuZamowieniaDoErp.Złożone));
                    break;
            }
            return wynik;
        }

        private readonly string kluczLimity = "_SzablonyLimitow_";

        public void UsunCacheLimitow(long idKlienta)
        {
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(idKlienta + kluczLimity);
        }

        public static object lok = new object();

        public T PobierzWykorzystanyLimit<T>(IKlient klient, SzablonLimitow szablon, RodzajLimitu rodzajLimitu)
        {
            var kluczCache = klient.Id + kluczLimity + rodzajLimitu;
            return Calosc.Cache.PobierzObiekt(() => PobierzLimitWykorzystany<T>(klient, szablon, rodzajLimitu), lok, kluczCache);
        }

        public IList<DokumentyBll> DokumentyDoLiczeniaLimitow(IKlient klient, DateTime odKiedy, HashSet<long> klienci)
        {
            var wynik = new List<DokumentyBll>();
            foreach (var k in klienci)
            {
                wynik.AddRange(Calosc.DokumentyDostep.PobierzWyfiltrowaneDokumenty(klient, PobierzWgIdLubZalogowanyAktualnie(k), RodzajDokumentu.Zamowienie, odKiedy, DateTime.Now));
            }
            return wynik;
        }

        private HashSet<long> PowiazanieKlienciLimity(IKlient klient)
        {
            var klienci = new HashSet<long>();
            klienci.Add(klient.Id);
            if (klient.SubkontoGrupaId.HasValue)
            {
                var kliecnigrupa = Calosc.DostepDane.Pobierz<Klient>(null, x => x.SubkontoGrupaId == klient.SubkontoGrupaId);
                foreach (var k in kliecnigrupa)
                {
                    klienci.Add(k.Id);
                }
            }
            return klienci;
        }

        public DateTime WyliczOdKiedy(SzablonLimitow szablon)
        {
            var aktualna = DateTime.Now;
            var poczatek = szablon.OdKiedy.GetValueOrDefault(new DateTime(aktualna.Year, 1, 1));
            while (poczatek.AddMonths(szablon.IloscMiesiecy) <= aktualna)
            {
                poczatek = poczatek.AddMonths(szablon.IloscMiesiecy);
            }
            return poczatek;
        }

        public SzablonLimitow PobierzCalkowityLimitIloscZamowien(IKlient klient)
        {
            SzablonLimitow grupa;
            SzablonLimitow wynik = null;
            var limitklienta = LimityKlient(klient, out grupa);
            if (grupa != null)
            {
                wynik = grupa;
            }
            if ((limitklienta != null) && limitklienta.IloscZamowien.HasValue)
            {
                wynik = limitklienta;
            }
            return wynik;
        }

        public SzablonAkceptacjiBll PobierzSzablonAkceptacji(IKlient klient, bool limityprzekoczone)
        {
            var id = limityprzekoczone ? klient.SzablonAkceptacjiPrzekrocznyLimitId : klient.SzablonAkceptacjiId;
            if ((id == null) && klient.SubkontoGrupaId.HasValue)
            {
                var grupa = Calosc.DostepDane.PobierzPojedynczy<SubkontoGrupa>(klient.SubkontoGrupaId, null);
                if (grupa != null)
                {
                    id = limityprzekoczone ? grupa.SzablonAkceptacjiPrzekrocznyLimitId : grupa.SzablonAkceptacjiId;
                }
            }
            if (id.HasValue)
            {
                return Calosc.DostepDane.PobierzPojedynczy<SzablonAkceptacjiBll>(id);
            }
            return null;
        }

        public bool CzyKlientNieMaMinimumLogistyczne(IKlient klient)
        {
            var kategorieBezMinimum = Calosc.Konfiguracja.DeaktywujMinimumLogistyczneDlaWybranychKategoriiKlientow;
            if (kategorieBezMinimum.IsEmpty())
            {
                return false;
            }
            var wynik = kategorieBezMinimum.Overlaps(klient.Kategorie);
            return wynik;
        }

    public bool CzyWymaganaZmianaHasla(IKlient klientDoZalogowania)
    {
            bool result = false;
            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.CoIleDniZmieniacHaslo > 0 && !klientDoZalogowania.Role.Contains(RoleType.Administrator))
            {
                DateTime lastChange = klientDoZalogowania.DataZmianyHasla;
                result = lastChange.AddDays(SolexBllCalosc.PobierzInstancje.Konfiguracja.CoIleDniZmieniacHaslo) <= DateTime.Now;
            }
            return result;
        }

        private SzablonLimitow LimityKlient(IKlient klient, out SzablonLimitow limitygrupy)
        {
            SzablonLimitow wynik = null;
            limitygrupy = null;
            if (klient.SzablonLimitowId.HasValue)
            {
                wynik = Calosc.DostepDane.PobierzPojedynczy<SzablonLimitow>(klient.SzablonLimitowId, null);
            }
            else if (klient.SubkontoGrupaId.HasValue)
            {
                var grupa = Calosc.DostepDane.PobierzPojedynczy<SubkontoGrupa>(klient.SubkontoGrupaId, null);
                if ((grupa != null) && grupa.SzablonLimitowId.HasValue)
                {
                    limitygrupy = Calosc.DostepDane.PobierzPojedynczy<SzablonLimitow>(grupa.SzablonLimitowId, null);
                }
            }
            return wynik;
        }

        public bool CzyMoznaZalogowacKlienta(string login, string haslo, string ipKlienta, out Klient klientDoZalogowania)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(haslo))
            {
                throw new InvalidDataException("Login lub hasło nie zostały uzupełnione");
            }
            klientDoZalogowania = PobierzPologinie(login, haslo);
            return CzyKlientMozeSieZalogowac(klientDoZalogowania);
        }

        public bool CzyKlientMozeSieZalogowac(Klient klientDoZalogowania)
        {
            if (klientDoZalogowania == null)
            {
                throw new Exception("Niepoprawne dane logowania");
            }
            if (!klientDoZalogowania.Aktywny)
            {
                throw new Exception("Twoje konto zostało zdeaktywowane - prosimy o kontakt z opiekun handlowym lub przedstawicielem");
            }
            if (klientDoZalogowania.BlokadaZamowien && (klientDoZalogowania.PowodBlokady == BlokadaPowod.BrakFaktur))
            {
                throw new Exception("Blokada logowania z powodu żadkich zakupów. Prosimy o kontakt z opiekunem handlowym.");
            }
            SprawdzAdresIpKlienta(klientDoZalogowania);
            if (klientDoZalogowania.BlokadaZamowien && (klientDoZalogowania.PowodBlokady == BlokadaPowod.ZmianaAdresuIp) && Calosc.Konfiguracja.GetLicense(Licencje.ZmianaIp) &&
                ((klientDoZalogowania.Kategorie == null) || !klientDoZalogowania.Kategorie.Contains(Calosc.Konfiguracja.KategoriaKlientaNieBlokujZmianaIp)))
            {
                throw new Exception("Nasz system bezpieczeństwa blokuje twoje konto z powodu zmiany w adresie IP." + "Wysłaliśmy do Ciebie maila z instrukcją odblokowania konta - prosimy sprawdź swoją skrzynkę pocztową.");
            }
            if (klientDoZalogowania.BlokadaZamowien)
            {
                throw new Exception("Blokada logowania z nieznanych powodów");
            }
            return true;
        }
    }
}