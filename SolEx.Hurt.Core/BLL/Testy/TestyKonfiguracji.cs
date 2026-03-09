using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Configuration;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class TestyKonfiguracji : LogikaBiznesBaza, ITestyKonfiguracji
    {
        public TestyKonfiguracji(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        /// <summary>
        /// Generowanie odpowiedzi z testu
        /// </summary>
        /// <param name="test">Nazwa testu</param>
        /// <param name="blad">Treść błędu</param>
        /// <returns></returns>
        protected KlasaOpakowanieTesty WygenerujOdpowiedz(string test, List<string> blad)
        {
            KlasaOpakowanieTesty k;
            if (!blad.Any())
            {
                return k = new KlasaOpakowanieTesty() { NazwaTestu = test, BladTestu = false, ListaBledow = blad };
            }
            return k = new KlasaOpakowanieTesty() { NazwaTestu = test, BladTestu = true, ListaBledow = blad };
        }

        protected KlasaOpakowanieTesty WygenerujOdpowiedzIloscElementowWTabeli(string test, long ilosc, bool historia)
        {
            //string tresc = string.Format("W tabeli: {0} znajduje się: {1} rekordów.", test, ilosc);
            KlasaOpakowanieTesty k;
            if (ilosc > 10000)
            {
                return k = new KlasaOpakowanieTesty() { NazwaTestu = test, BladTestu = true, ListaBledow = new List<string>() { ilosc.ToString() }, CzyMaHistorieZmian = historia };
            }
            return k = new KlasaOpakowanieTesty() { NazwaTestu = test, BladTestu = false, ListaBledow = new List<string>() { ilosc.ToString() }, CzyMaHistorieZmian = historia };
        }

        protected KlasaOpakowanieTesty WygenerujOdpowiedzModulyKoszykowe(string modul, string nazwaZadania, List<string> blad)
        {
            KlasaOpakowanieTesty k;
            if (blad.Any())
            {
                return k = new KlasaOpakowanieTesty() { NazwaTestu = modul, NazwaZadania = nazwaZadania, BladTestu = true, ListaBledow = blad };
                //return k = new KlasaOpakowanieTesty() { NazwaTestu = test, BladTestu = false, ListaBledow = blad };
            }
            return new KlasaOpakowanieTesty();
        }

        public List<KlasaOpakowanieTesty> WykonajTesty()
        {
            List<KlasaOpakowanieTesty> wynik = new List<KlasaOpakowanieTesty>();
            var testy = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(TestKonfiguracjiBaza));
            foreach (var t in testy)
            {
                if (t.IsAbstract)
                {
                    continue;
                }
                TestKonfiguracjiBaza test = (TestKonfiguracjiBaza)Activator.CreateInstance(t);

                wynik.Add(WygenerujOdpowiedz(test.Opis, test.Test()));
            }
            return wynik;
        }

        public List<KlasaOpakowanieTesty> WykonajTestyBazy()
        {
            List<KlasaOpakowanieTesty> wynik = new List<KlasaOpakowanieTesty>();
            var tabele = SolexBllCalosc.PobierzInstancje.DostepDane.TabelaOrazJejRozmiar();
            foreach (var tabela in tabele)
            {
                bool czyMaCdc = Calosc.DostepDane.SprawdzAktywnoscZmian(tabela.Key);
                if (tabela.Value > 100 || (tabela.Key == "MaileBledneDoPonownejWysylki" && tabela.Value > 0) || !czyMaCdc)
                {
                    wynik.Add(WygenerujOdpowiedzIloscElementowWTabeli(tabela.Key, tabela.Value, czyMaCdc));
                }
            }

            return wynik;
        }

        public List<KlasaOpakowanieTesty> WykonajTestySkrzynekPocztowych()
        {
            List<KlasaOpakowanieTesty> wynik = new List<KlasaOpakowanieTesty>();
            var wyniki = SettingTester.Test();
            foreach (var w in wyniki)
            {
                if (w.Result == false)
                {
                    if (w.Module == "Wysyłanie wiadomości testowej ze skrzynki: Ogolne")
                    {
                        List<string> listaBledow = new List<string>();
                        listaBledow.Add(w.Error);
                        if (!w.Error.StartsWith("Brak konfiguracji skrzynki pocztowej dla danego typu wysyłki."))
                        {
                            listaBledow.Add(string.Format("Uzytkownik: {0}",
                            SolexBllCalosc.PobierzInstancje.Konfiguracja.EmailNazwaUzytkownika));
                            listaBledow.Add(string.Format("Haslo: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.EmailHaslo));
                            listaBledow.Add(string.Format("Port: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.EmailPort));
                            listaBledow.Add(string.Format("Timeout: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.EmailTimeout));
                            listaBledow.Add(string.Format("Uzywaj SSL: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.EmailSzyfrowanie));
                            listaBledow.Add(string.Format("Host: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.EmailHost));
                            listaBledow.Add(string.Format("Nadawca: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.EmailFromPrzyjaznaNazwa));
                            wynik.Add(WygenerujOdpowiedz(w.Module, listaBledow));
                        }
                    }
                    else
                    {
                        List<string> listaBledow = new List<string>();
                        listaBledow.Add(w.Error);
                        if (!w.Error.StartsWith("Brak konfiguracji skrzynki pocztowej dla danego typu wysyłki."))
                        {
                            listaBledow.Add(string.Format("Uzytkownik: {0}",
                            SolexBllCalosc.PobierzInstancje.Konfiguracja.MailingEmailNazwaUzytkownika));
                            listaBledow.Add(string.Format("Haslo: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.MailingEmailHaslo));
                            listaBledow.Add(string.Format("Port: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.MailingEmailPort));
                            listaBledow.Add(string.Format("Timeout: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.MailingEmailTimeout));
                            listaBledow.Add(string.Format("Uzywaj SSL: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.MailingEmailSzyfrowanie));
                            listaBledow.Add(string.Format("Host: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.MailingEmailHost));
                            listaBledow.Add(string.Format("Nadawca: {0}",
                                SolexBllCalosc.PobierzInstancje.Konfiguracja.MailingEmailFrom));
                            wynik.Add(WygenerujOdpowiedz(w.Module, listaBledow));
                        }
                        wynik.Add(WygenerujOdpowiedz(w.Module, listaBledow));
                    }
                }
                else
                {
                    wynik.Add(WygenerujOdpowiedz(w.Module, new List<string>() { "OK" }));
                }
            }

            return wynik;
        }

        public List<KlasaOpakowanieTesty> WykonajTestyKoszykowe()
        {
            Dictionary<string, KlasaOpakowanieTesty> test = new Dictionary<string, KlasaOpakowanieTesty>();
            var testy2 = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Zadanie>(null);
            foreach (var t in testy2)
            {
                if (!t.ModulFullTypeName.IsNullOrEmpty())
                {                    
                    ModulStowrzonyNaPodstawieZadania mod = t.StworzKontrolke<ModulStowrzonyNaPodstawieZadania>();
                    mod.ZadanieBazowe = t;
                    var akcesor = mod.GetType().PobierzRefleksja();
                    foreach (PropertyInfo p in mod.GetType().GetProperties())
                    {
                        ObsoleteAttribute ob = p.GetCustomAttribute<ObsoleteAttribute>();
                        if (ob == null)
                        {
                            continue;
                        }
                        object wart = akcesor[mod, p.Name];
                        if (wart.RownyWartosciDomyslnej())
                        {
                            continue;
                        }
                        if (mod.ZadanieBazowe!=null && mod.ZadanieBazowe.ZadanieNadrzedne != null)
                        {
                            int id = mod.ZadanieBazowe.ZadanieNadrzedne.Value;
                            var zad = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZadanieBll>(null).First(x => x.Id == id);
                            string nazwaNad = string.Empty;
                            nazwaNad = !zad.ModulFullTypeName.IsNullOrEmpty() ? string.Format("{0}, [{1}]", zad.ModulFullTypeName, id) : string.Format("Moduł synchronizacji - {0}, [{1}]", mod.Nazwa, mod.Id);
                            string nazwa = string.Empty;

                            if (!nazwaNad.Contains("Moduł synchronizacji"))
                            {
                                nazwa = string.Format("{0}, [{1}]", mod.Nazwa, mod.Id);
                            }
                            string klucz = string.Format("{0},{1}", nazwaNad, nazwa);
                            string blad = string.Format("{0} - {1}", p.Name, ob.Message);
                            Sprawdzenie(klucz, ref test, blad, nazwaNad, nazwa);
                        }
                        else
                        {
                            string nazwa = string.Format("{0}, [{1}]", mod.Nazwa, mod.Id);
                            string klucz = nazwa;
                            string blad = string.Format("{0} - {1}", p.Name, ob.Message);
                            Sprawdzenie(klucz, ref test, blad, nazwa, "");
                        }
                    }

                    //Sprawdzanie wymaganych pol
                    List<string> listaBledow = SprawdzWymaganePolaModulow(mod);


                    var testowalna = mod as ITestowalna;
                    if (testowalna != null)
                    {
                        listaBledow.AddRange(testowalna.TestPoprawnosci());
                    }
                    //var oba = mod.GetType().GetCustomAttribute<ObsoleteAttribute>();

                    if (mod.ZadanieBazowe != null && mod.ZadanieBazowe.ZadanieNadrzedne != null)
                    {
                        int id = mod.ZadanieBazowe.ZadanieNadrzedne.Value;
                        var zad = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZadanieBll>(null).First(x => x.Id == id);
                        string nazwaNad = string.Empty;
                        nazwaNad = !zad.ModulFullTypeName.IsNullOrEmpty() ? string.Format("{0}, [{1}]", zad.ModulFullTypeName, id) : string.Format("Moduł synchronizacji - {0}, [{1}]", mod.Nazwa, mod.Id);
                        string nazwa = string.Empty;
                        if (!nazwaNad.Contains("Moduł synchronizacji"))
                        {
                            nazwa = string.Format("{0}, [{1}]", mod.Nazwa, mod.Id);
                        }
                        string klucz = string.Format("{0},{1}", nazwaNad, nazwa);
                        foreach (var blad in listaBledow)
                        {
                            Sprawdzenie(klucz, ref test, blad, nazwaNad, nazwa);
                        }
                    }
                    else
                    {
                        string nazwa = string.Format("{0}, [{1}]", mod.Nazwa, mod.Id);
                        string klucz = nazwa;
                        foreach (var blad in listaBledow)
                        {
                            Sprawdzenie(klucz, ref test, blad, nazwa, "");
                        }
                    }
                }
            }
            return test.Values.OrderBy(p => p.NazwaTestu).ToList();
            //return wynik.OrderBy(p=>p.NazwaTestu).ToList();
        }

        public List<string> SprawdzWymaganePolaModulow(ModulStowrzonyNaPodstawieZadania modul)
        {
            List<string> listaBledow = new List<string>();
            var propertisy = modul.GetType().GetProperties().Where(p => !Attribute.IsDefined(p, typeof(Niewymagane)) && Attribute.IsDefined(p, typeof(WidoczneListaAdminAttribute)));
            foreach (var propertyInfo in propertisy)
            {
                var wartosc = propertyInfo.GetValue(modul);
                if (wartosc == null)
                {
                    listaBledow.Add(string.Format("Brak uzupełnionej wartość: {0}", propertyInfo.Name));
                }
            }
            return listaBledow;
        }


        public void Sprawdzenie(string klucz, ref Dictionary<string, KlasaOpakowanieTesty> slownikBledow, string blad, string nazwaNadrzedna, string nazwa)
        {
            if (slownikBledow.ContainsKey(klucz))
            {
                slownikBledow[klucz].ListaBledow.Add(blad);
            }
            else
            {
                slownikBledow.Add(klucz, WygenerujOdpowiedzModulyKoszykowe(nazwaNadrzedna, nazwa, new List<string>() { blad }));
            }
        }
    }
}