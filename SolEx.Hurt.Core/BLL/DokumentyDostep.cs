using log4net;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public class DokumentyDostep : LogikaBiznesBaza, IDokumentyDostep
    {
        public DokumentyDostep(ISolexBllCalosc calosc)
            : base(calosc)
        {
        }

        protected ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        public string KatalogDokumentow
        {
            get
            {
                string sciezka = Path.Combine(KatalogSfery, "dokumenty");
                if (!Directory.Exists(sciezka))
                {
                    Directory.CreateDirectory(sciezka);
                }
                return sciezka;
            }
        }

        /// <summary>
        /// Sprawdza czy klient ma przeterminowane faktury czyli takie w których propertis Zaplacono jest na False i DokumentDniSpoznienia jest większe od 0
        /// </summary>
        /// <param name="klient">ID klienta dla którego są sprawdzane przeterminowane faktury</param>
        /// <returns>True jeśli klient ma przeterminowane faktury</returns>
        public bool CzyKlientPosiadaJakiesPrzeterminowaneFaktury(long klient)
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Count<HistoriaDokumentu>(x => x.KlientId == klient && x.Rodzaj == RodzajDokumentu.Faktura && x.Zaplacono == false
                        && x.WartoscNetto > 0 && x.TerminPlatnosci < DateTime.Now.AddDays(-1)) > 0;
        }

        private string KatalogSfery
        {
            get
            {
                string sciezka = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sfera");
                if (!Directory.Exists(sciezka))
                {
                    Directory.CreateDirectory(sciezka);
                }
                return sciezka;
            }
        }

        private string SprawdzRozszerzenie(String rozszerzenie)
        {
            if (string.IsNullOrEmpty(rozszerzenie)) throw new InvalidOperationException("Brak roszerzenia pliku");
            if (rozszerzenie.Contains("."))
                return Path.GetExtension(rozszerzenie);
            return "." + rozszerzenie;
        }



        public virtual bool IstniejeZalacznik(int dokument, string rozszerzenie)
        {
            string sciekza = PobierzSciezkePliku(dokument, rozszerzenie);
            return File.Exists(sciekza);
        }

        public List<DokumentyStawkiVAT> DokumentyStawkiVat(DokumentyBll dokument)
        {
                List<DokumentyStawkiVAT> stawki = new List<DokumentyStawkiVAT>();                
                foreach (var p in dokument.PobierzPozycjeDokumentu())
                {
                    var s = stawki.FirstOrDefault(x => x.Stawka == p.Vat);
                    if (s == null)
                    {
                        s = new DokumentyStawkiVAT(dokument.walutaB2b) { Stawka = p.Vat };
                        stawki.Add(s);
                    }
                    s.WartoscBrutto.Wartosc += p.WartoscBrutto;
                    s.WartoscNetto.Wartosc += p.WartoscNetto;
                    s.WartoscVAT.Wartosc += p.WartoscVat;
                }
                return stawki.OrderBy(p => p.Stawka).ToList();
        }

        public string PobierzSciezkePliku(int dokumentId, string rozszerzenie)
        {
            rozszerzenie = SprawdzRozszerzenie(rozszerzenie);
            string nazwaPliku = string.Format("{0}{1}", Tools.PobierzInstancje.GetMd5Hash(dokumentId.ToString(CultureInfo.InvariantCulture) ), rozszerzenie);

            return string.Format("{0}\\{1}", KatalogDokumentow, nazwaPliku);
        }

        public Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> PobierzSlownikDokumentowIPozycji(Klient klient, Expression<Func<HistoriaDokumentu, bool>> warunek)
        {
            Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> wynik = new Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();

            var dokumenty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentu>(klient, warunek);
            foreach (var historiaDokumentu in dokumenty)
            {
                int idDokumentu = historiaDokumentu.Id;
                var produkty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentuProdukt>(null, x => x.DokumentId == idDokumentu).ToList();
                wynik.Add(historiaDokumentu, produkty);
            }
            return wynik;
        }

        public void Aktualizuj(List<KlasaOpakowujacaDokumentyDoWyslania> paczka)
        {
            // List<HistoriaDokumentu> listaDokumentowPaczka = paczka.Select(x=>x.Dokument).ToList();
            HashSet<int> idDokumentow = new HashSet<int>( paczka.Select(x => x.Dokument.Id) );
            if (paczka.Count == 0) return;
            DateTime start = DateTime.Now;
            DateTime dataOdKtorejWysyalacMailaOZmienieStatusu = Calosc.Konfiguracja.PobierzDateZmianaStatusu;

            Dictionary<int, DokumentyBll> istniejaceDokumenty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DokumentyBll>(null, x => Sql.In(x.Id, idDokumentow)).ToDictionary(x => x.Id, x => x);

            HashSet<int> dokumentyDlaKtorychSkasowacPozycje = new HashSet<int>();

            foreach (KlasaOpakowujacaDokumentyDoWyslania d in paczka)
            {
                if (string.IsNullOrEmpty(d.Dokument.NazwaDokumentu))
                {
                    throw new InvalidOperationException(string.Format("Nazwa dokumentu o ID {0} jest pusta", d.Dokument.Id));
                }

                DokumentyBll dokumentObecnieWBazieB2B;
                istniejaceDokumenty.TryGetValue(d.Dokument.Id, out dokumentObecnieWBazieB2B);

                if (dokumentObecnieWBazieB2B != null)
                {
                    //usuwanie PDF jesli sa - tylko dla FAKTUR mamy PDFy
                    if (d.Dokument.Rodzaj == RodzajDokumentu.Faktura)
                    {
                        string sciezka = PobierzSciezkePliku(dokumentObecnieWBazieB2B.Id, "pdf");
                        if (File.Exists(sciezka))
                        {
                            File.Delete(sciezka);
                        }
                    }

                    if (d.Dokument.Rodzaj == RodzajDokumentu.Zamowienie && !d.Dokument.Zaplacono && dokumentObecnieWBazieB2B.TerminPlatnosci.HasValue && dokumentObecnieWBazieB2B.TerminPlatnosci != d.Dokument.TerminPlatnosci && dokumentObecnieWBazieB2B.TerminPlatnosci.Value >= DateTime.Now.Date)
                    {
                        var dokumentBll = new DokumentyBll(d.Dokument) {Klient = dokumentObecnieWBazieB2B.Klient};
                        Calosc.Statystyki.ZdarzenieZmianaTerminuRealizacjiZamowienia(dokumentBll); //wyslemy do odbiorcy
                    }

                    if (dokumentObecnieWBazieB2B.DataUtworzenia.Date >= dataOdKtorejWysyalacMailaOZmienieStatusu) //nie ma sensu wchdzoic, jak data jest zla. w mailu o zmienie statusu uwzględniamy tylko dokumenty z ostatnich x dni, jeśli jest starszy to nie ma co sprawdzać
                    {
                        if (d.Dokument.StatusId.HasValue && dokumentObecnieWBazieB2B.StatusId != d.Dokument.StatusId) //zmiana statusu
                        {
                            StatusZamowienia status = SolexBllCalosc.PobierzInstancje.Konfiguracja.StatusyZamowien[d.Dokument.StatusId.Value];
                            if (status != null && status.PowiadomienieZmianaStatusu)
                            {
                                Calosc.Statystyki.ZdarzenieZmianaStatusuDokumentu(new DokumentyBll(d.Dokument));   //wyslemy do odbiorcy
                            }
                        }
                    }

                    if (dokumentObecnieWBazieB2B.DataWyslaniaDokumentu != null)
                    {
                        d.Dokument.DataWyslaniaDokumentu = dokumentObecnieWBazieB2B.DataWyslaniaDokumentu;
                    }
                }
                d.Dokument.DataDodania = DateTime.Now;


                if (d.Produkty != null && d.Produkty.Any())
                {
                    dokumentyDlaKtorychSkasowacPozycje.Add(d.Dokument.Id);
                }
            }

            //kasujemy wszystkie pozycje z ISTNIEJACYCH DOKUMENTOW - to jest ŹLE bo powiny pozycje oddzielnie isc innym synchro, ale poki co jest tak tandetnie
            if (dokumentyDlaKtorychSkasowacPozycje.Any())
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.UsunWybrane<HistoriaDokumentuProdukt, int>(x => Sql.In(x.DokumentId, dokumentyDlaKtorychSkasowacPozycje));
            }


            Calosc.DostepDane.AktualizujListe<HistoriaDokumentu>(paczka.Select(x => x.Dokument).ToList());
            try
            {
                Dictionary<long, List<HistoriaDokumentuProdukt>> pozycjeNowe = new Dictionary<long, List<HistoriaDokumentuProdukt>>();
                foreach (KlasaOpakowujacaDokumentyDoWyslania nowe in paczka)
                {
                    long idKlienta = nowe.Dokument.KlientId;
                    if (!pozycjeNowe.ContainsKey(idKlienta))
                    {
                        pozycjeNowe.Add(idKlienta, new List<HistoriaDokumentuProdukt>());
                    }
                    foreach (var pozycja in nowe.Produkty)
                    {
                        pozycjeNowe[idKlienta].Add(pozycja);
                    }
                }
                List<HistoriaDokumentuProdukt>produktyDoWyslania = new List<HistoriaDokumentuProdukt>();
                //poprawienie na wirtualne produkty
                ProduktyWirtualneProvider prov = SolexBllCalosc.PobierzInstancje.Konfiguracja.WirtualneProduktyProvider;
                if (prov != null)
                {
                    var klienci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null,x=>pozycjeNowe.Keys.Contains(x.Id)).ToDictionary(x=>x.Id,x=>x);
                    foreach (var pozycjeKlienta in pozycjeNowe)
                    {
                        var produktyPoprawione = pozycjeKlienta.Value;
                        prov.PoprawProduktyNaDokumentach(klienci[pozycjeKlienta.Key], ref produktyPoprawione);
                        produktyDoWyslania.AddRange(produktyPoprawione);
                    }
                }
                else
                {
                    foreach (var pozycjeKlienta in pozycjeNowe)
                    {
                        produktyDoWyslania.AddRange(pozycjeKlienta.Value);
                    }

                }
                
                Calosc.DostepDane.AktualizujListe<HistoriaDokumentuProdukt>(produktyDoWyslania);
            }
            catch (Exception e)
            {
                SolexBllCalosc.PobierzInstancje.Log.ErrorFormat("Błąd aktualizacji pozycji dokumentów dla dokumentów id:" + paczka.Select(x=>x.Dokument.Id).ToCsv() );
                SolexBllCalosc.PobierzInstancje.Log.Error(e);
            }

            //zapis dokumentow glownych

            Log.DebugFormat("Paczka dokumentów {1}, czas {0} ms", (DateTime.Now - start).TotalMilliseconds, paczka.Count);
        }

        public Dictionary<int, long> PobierzSumyKontrolneDokumentow()
        {
            Dictionary<int, long> wynik = new Dictionary<int, long>();
            var dokumenty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentu>(null);
            var dokumentyBezPozycji =new HashSet<int>( SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentu>(null, x => Sql.NotInSubquery(x.Id, new SqlServerExpressionVisitor<HistoriaDokumentuProdukt>().SelectDistinct(y => y.DokumentId))).Select(x => x.Id) );

            //ilosc pozycji na dokumencie
            foreach (var d in dokumenty)
            {
                long hash = 0;
                if (!dokumentyBezPozycji.Contains(d.Id))
                {
                    hash = Tools.PobierzInstancje.PoliczHashDokumentu(d, out _);
                }
               
                wynik.Add(d.Id, hash);
            }
            return wynik;
        }   

        //dla faktur klient widzi tylko te których jest płatnikiem
        //dla zamówień widzi te w których jest płatnikiem albo odbiorcą
        public bool CzyKlientMaDostep(DokumentyBll dokument, IKlient k, HashSet<long> subkonta = null)
        {
            if (subkonta == null)
            {
                subkonta = new HashSet<long>();
                foreach (var idkli in k.Subkonta())
                {
                    subkonta.Add(idkli.Id);
                }
            }

            if (k.CzyAdministrator)
            {
                return true;//administrator może pobrać każdy dokument bo ma godmode
            }
            if (Calosc.Klienci.JestOpiekunem(dokument.DokumentPlatnikId, k))
            {
                return true;
            }

            long odbiorca = dokument.DokumentOdbiorcaId;
            if (dokument.Rodzaj == RodzajDokumentu.Zamowienie && Calosc.Klienci.JestOpiekunem(odbiorca, k))
            {
                return true;
            }

            return CzyDokumentNalezyDoKlienta(dokument, k, subkonta);
        }

        public bool CzyDokumentNalezyDoKlienta(DokumentyBll dokument, IKlient k, HashSet<long> subkonta)
        {
            if (CzyPlatnikSieZgadza(dokument, k, subkonta))
            {
                return true;
            }

            if (dokument.Rodzaj == RodzajDokumentu.Zamowienie)
            {
                long odbiorca = dokument.DokumentOdbiorcaId;
                if (k.Id == odbiorca || subkonta.Contains(odbiorca))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CzyPlatnikSieZgadza(DokumentyBll dokument, IKlient k, HashSet<long> subkonta)
        {
            return k.Id == dokument.DokumentPlatnikId || subkonta.Contains(dokument.DokumentPlatnikId);
        }

        private List<GenerowanieDokumentu> _dostepneFormaty;

        public IList<GenerowanieDokumentu> DostepneFormaty()
        {
            if (_dostepneFormaty == null)
            {
                _dostepneFormaty = new List<GenerowanieDokumentu>();
                var typy = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(GenerowanieDokumentu), "SolEx.Hurt.Core.Importy.Eksporty");

                if (typy == null || typy.Count == 0)
                {
                    throw new Exception("Brak formatów eksportu importu");
                }

                foreach (var t in typy)
                {
                    if (t.IsAbstract)
                    {
                        continue;
                    }
                    GenerowanieDokumentu modul = (GenerowanieDokumentu)Activator.CreateInstance(t);
                    if (modul.WymaganaLicencja == null || Calosc.Konfiguracja.GetLicense(modul.WymaganaLicencja.Value))
                    {
                        _dostepneFormaty.Add(modul);
                    }
                }
            }
            return _dostepneFormaty;
        }

        //todo: pawel TEST wydajnosciowy
        public List<ParametryPobieraniaDokumentu> PobierzDostepneFormatyDoPobrania(DokumentyBll dokument)
        {
            List<ParametryPobieraniaDokumentu> wynik = new List<ParametryPobieraniaDokumentu>();
            foreach (var f in DostepneFormaty())
            {
                if (f.MoznaGenerowac(dokument))
                {
                    wynik.Add(new ParametryPobieraniaDokumentu(f.Nazwa, f.PobierzNazwePliku(dokument), f.GetType().FullName, dokument.Id));
                }
            }
            return wynik.OrderBy(x => x.Nazwa).ToList();
        }

        public byte[] PobierzPlik(DokumentyBll dokument, string modul, IKlient zadajacy, out Encoding kodowanie, out string nazwa, string parametry = null)
        {
            if (string.IsNullOrEmpty(modul)) throw new InvalidOperationException("Nie wiadomo w jakim formacie pobrac dokument");
            var mod = DostepneFormaty().FirstOrDefault(x => x.GetType().FullName == modul);
            if (mod == null)
            {
                throw new InvalidOperationException("Nieznany moduł do pobierania dokumentów. Moduł: " + modul);
            }
            nazwa = mod.PobierzNazwePliku(dokument);
            kodowanie = mod.Kodowanie;
            return mod.PobierzDokumentDlaKlienta(dokument, zadajacy);
        }

        public virtual IList<HistoriaDokumentu> PobierzNiezaplaconeFakturyZTerminemPlatnosci()
        {
            return Calosc.DostepDane.Pobierz<HistoriaDokumentu>(null, x => !x.Zaplacono && x.Rodzaj == RodzajDokumentu.Faktura && x.TerminPlatnosci != null);
        }
         
        public Dictionary<HistoriaDokumentu, bool> PobierzFakturyNiezaplaconeWzgledemDaty(int? ileDniPonowneWyslanie, int dokumentPrzeterminujesieWCiagu)
        {
            Dictionary<HistoriaDokumentu, bool> wynik = new Dictionary<HistoriaDokumentu, bool>();
            //Nie wysyłamy jeżeli w module ustawiona wartość 0
            if (dokumentPrzeterminujesieWCiagu == 0)
            {
                return wynik;
            }
            DateTime datDoKiedyPrzeterminujeSieWysylac = DateTime.Now.Date.AddDays(dokumentPrzeterminujesieWCiagu);
            DateTime dataOdKiedy = ileDniPonowneWyslanie.HasValue ? DateTime.Now.AddDays(-ileDniPonowneWyslanie.Value) :DateTime.Now.AddYears(-10);

            Dictionary<long, DateTime> listaDzialan = Calosc.Statystyki.PobierzDzialaniaUzytkownikow(dataOdKiedy, ZdarzenieGlowne.PrzypomnienieNiezaplaconejFakturze);
            
            IList<HistoriaDokumentu> docs = PobierzNiezaplaconeFakturyZTerminemPlatnosci();

            foreach (HistoriaDokumentu t in docs)
            {
                try
                {
                    if (t.WartoscBrutto == 0)//Zmienione ze względu ma fakt że korekty są nam potrzebne do sprawdzenia czy wartosc korekt nie jest wieksza od wartosci naleznej
                    {
                        continue; //pomijamy kiedy to my wisimy kasę
                    }
                    if (t.WartoscNetto == 0) //Zmienione ze względu ma fakt że korekty są nam potrzebne do sprawdzenia czy wartosc korekt nie jest wieksza od wartosci naleznej
                    {
                        continue;
                    }
                    if (!t.TerminPlatnosci.HasValue)
                    {
                        continue;
                    }
                    //nie interesuja nas dokumenty juz przeterminowane
                    if (t.TerminPlatnosci < DateTime.Now || t.TerminPlatnosci > datDoKiedyPrzeterminujeSieWysylac)
                    {
                        continue;
                    }
                    bool wyslany = listaDzialan.ContainsKey(t.KlientId) && t.TerminPlatnosci.Value.AddDays(-dokumentPrzeterminujesieWCiagu) < listaDzialan[t.KlientId];
                    wynik.Add(t, wyslany);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            return wynik;
        }

        public Dictionary<HistoriaDokumentu, bool> PobierzDokumentyPrzeterminowaneWzgledemDaty(int? ileDniPonowneWyslanie, int mineloConajmniejOdTerminuPlatnosi)
        {
            DateTime data = DateTime.Now.Date.AddDays(-mineloConajmniejOdTerminuPlatnosi);
            Dictionary<HistoriaDokumentu, bool> wynik = new Dictionary<HistoriaDokumentu, bool>();
            IList<HistoriaDokumentu> docs = PobierzNiezaplaconeFakturyZTerminemPlatnosci();
            DateTime dataOdKiedy = ileDniPonowneWyslanie.HasValue ? DateTime.Now.AddDays(-ileDniPonowneWyslanie.Value) : DateTime.Now.AddYears(-10);
            Dictionary<long,DateTime> listaDzialan = Calosc.Statystyki.PobierzDzialaniaUzytkownikow(dataOdKiedy, ZdarzenieGlowne.PrzypomnienieNiezaplaconejFakturze);
            foreach (HistoriaDokumentu t in docs)
            {
                try
                {
                    if (t.WartoscBrutto == 0)
                    {
                        continue; //pomijamy kiedy to my wisimy kasę
                    }

                    if (t.WartoscNalezna ==0 && !Calosc.Konfiguracja.StatusyZamowien[t.StatusId.Value].TraktujJakoFaktoring || (t.WartoscNalezna != 0 && t.WartoscNalezna == 0)) //Zmienione ze względu ma fakt że korekty są nam potrzebne do sprawdzenia czy wartosc korekt nie jest wieksza od wartosci naleznej
                    {
                        continue;
                    }
                    if (!t.TerminPlatnosci.HasValue || t.TerminPlatnosci.Value >= DateTime.Now.Date)
                    {
                        continue;//dokument jest niezapłancny, ale jeszcze nie jest przeteminowany
                    }

                    if ( t.TerminPlatnosci.Value > data)
                    {
                        continue;//mineło już conajmniej x dni
                    }

                    bool wyslany = listaDzialan.ContainsKey(t.KlientId) && t.TerminPlatnosci.Value.AddDays(mineloConajmniejOdTerminuPlatnosi) < listaDzialan[t.KlientId];//listaDzialan.Contains(t.Id);
                    wynik.Add(t, wyslany);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            return wynik;
        }

        private DateTime DataOdkiedyWysylacMailaNoweDokumenty()
        {
            int dni = Calosc.Konfiguracja.DokumentyMailOnNowymIleDniWstecz;
            DateTime odKiedy = DateTime.Now.Date.AddDays(-dni);
            return odKiedy;
        }

        public DokumentyBll PobierzDokumentZZamowieniemPowiazanym(long id, int jezyk, IKlient klient)
        {
            var dokument = Calosc.DostepDane.PobierzPojedynczy<DokumentyBll>(id, jezyk, klient);
            if (dokument != null)
            {
                return PobierzZamowieniePowiazane(new List<DokumentyBll>() { dokument }, klient).First();
            }
            return null;
        }

        private List<DokumentyBll> PobierzZamowieniePowiazane(List<DokumentyBll> dok, IKlient klient)
        {
            Dictionary<int, ZamowienieDokumenty> zd = Calosc.DostepDane.Pobierz<ZamowienieDokumenty>(klient, x => Sql.In(x.IdDokumentu, dok.Select(y=>y.Id))).GroupBy(x=>x.IdDokumentu).ToDictionary(x=>x.Key, x=>x.FirstOrDefault());
            if (!zd.Any())
            {
                return dok;
            }
            foreach (var dokum in dok.Where(x=>zd.Keys.Contains(x.Id)))
            {
                ZamowieniaBLL zam = Calosc.DostepDane.PobierzPojedynczy<ZamowieniaBLL>(zd[dokum.Id].IdZamowienia, klient);
                dokum.UstawPowiazaneZamowienieB2B(zam);
            }
            return dok;
        }


        public DokumentyBll PobierzDokumentIDUwzgledniajacSztuczneZamowienia(long id, int jezyk, IKlient klient)
        {
            if (id > 0)
            {
                return PobierzDokumentZZamowieniemPowiazanym(id,jezyk,klient);
            }
            else
            {
                //konwersaj z zamowienia do dokumentu - jeszcze nie zdazyl sie zaciangnac pewnie dokument z ERPa
                return new DokumentyBll(SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ZamowieniaBLL>(id, jezyk, klient));
            }
        }

        /// <summary>
        /// Pobieranie dokuemntów na widoki
        /// </summary>
        /// <param name="zalogowany"></param>
        /// <param name="klient"></param>
        /// <param name="rodzaj"></param>
        /// <param name="odKiedy"></param>
        /// <param name="doKiedy"></param>
        /// <param name="szukanieFraza"></param>
        /// <param name="sortuj"></param>
        /// <param name="czyTylkoNiezrealizowane"></param>
        /// <param name="UzupelnijODokumentyPowiazane"></param>
        /// <returns></returns>
        public List<DokumentyBll> PobierzWyfiltrowaneDokumenty(IKlient zalogowany, IKlient klient, RodzajDokumentu rodzaj, DateTime odKiedy, DateTime doKiedy, 
            string szukanieFraza = null, bool sortuj = false, bool UzupelnijODokumentyPowiazane = false)
        {
            //jesli pobieraja subkonta to sprawdzmy czyto naprade subkonta
            if (zalogowany.Id != klient.Id && !zalogowany.WszystkieKontaPowiazane().Select(x => x.Id).Contains(klient.Id))
            {
                throw new UnauthorizedAccessException("Nie masz uprawnień do oglądania tych dokumentów");
            }
          //  bool czyZProfilu = czyTylkoNiezrealizowane == null || czyTylkoNiezrealizowane == false;
            bool tylkoNiezrealizowane = Calosc.ProfilKlienta.PobierzWartosc<bool>(zalogowany, TypUstawieniaKlienta.DokumentyTylkoNiezrealizowane);
            bool tylkoNiezaplacone = Calosc.ProfilKlienta.PobierzWartosc<bool>(zalogowany, TypUstawieniaKlienta.DokumentyTylkoNiezaplacone);
            bool tylkoPrzeterminowane = Calosc.ProfilKlienta.PobierzWartosc<bool>(zalogowany, TypUstawieniaKlienta.DokumentyTylkoPrzeterminowane);

            //tylko statusy ktore NIE moze klient zobaczyc
            List<int> statusyDoNIEPokazywania = SolexBllCalosc.PobierzInstancje.Konfiguracja.StatusyZamowien.Where(x => !x.Value.Widoczny).Select(x => x.Key).ToList();

            //pokazujemy tylko dla klienta wybranego i jego wszystkich subkont
           HashSet<long> idKLientowDoPokazania = new HashSet<long>( klient.WszystkieKontaPodrzedne().Select(x => x.Id) );
           idKLientowDoPokazania.Add(klient.Id);


            Expression<Func<DokumentyBll, bool>> warunek;
            if (rodzaj == RodzajDokumentu.Faktura)
            {
                warunek = x => ((HistoriaDokumentu) x).Rodzaj == rodzaj && (Sql.In(x.KlientId, idKLientowDoPokazania) || x.OdbiorcaId == klient.Id) && (x.KlientId == x.OdbiorcaId  || (x.KlientId != x.OdbiorcaId && x.OdbiorcaId != klient.Id));
            }
            else
            {
                warunek = x => ((HistoriaDokumentu)x).Rodzaj == rodzaj && (Sql.In(x.KlientId, idKLientowDoPokazania) || x.OdbiorcaId == klient.Id);
            }
           
           if (statusyDoNIEPokazywania.Any())
           {
                //nie wiem czy dobre ale dodałem warunek że statusId ma być różne od nulla SG
               Expression<Func<DokumentyBll, bool>> temp = (x => x.StatusId!= null && Sql.In(x.StatusId, statusyDoNIEPokazywania));
               warunek = warunek.And( temp.Not() );
           }

           if (odKiedy != DateTime.MinValue)
           {
                warunek= warunek.And(x => ((HistoriaDokumentu) x).DataUtworzenia >= odKiedy);
           }

            if (doKiedy != DateTime.MaxValue)
            {
                warunek= warunek.And(x => ((HistoriaDokumentu) x).DataUtworzenia <= doKiedy);
            }

            if (rodzaj == RodzajDokumentu.Faktura && tylkoNiezaplacone)
            {
                warunek= warunek.And(x => ((HistoriaDokumentu) x).Zaplacono == false);
            }

            if (rodzaj == RodzajDokumentu.Zamowienie && tylkoNiezrealizowane)
            {
                warunek = warunek.And(x => ((HistoriaDokumentu)x).Zaplacono == false);
            }

            if (rodzaj == RodzajDokumentu.Faktura && tylkoPrzeterminowane)
            {
                warunek=  warunek.And(x => ((HistoriaDokumentu) x).TerminPlatnosci < DateTime.Now && !x.Zaplacono && x.WartoscNetto != 0);
            }

            //List<DokumentyBll> docs = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DokumentyBll>(klient, warunek).Select(x => new DokumentyBll(x) { Klient = klient }).ToList();
            List<DokumentyBll> docs = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DokumentyBll>(klient, warunek).Select(x => new DokumentyBll(x) { Klient = klient }).ToList();
           

            //.Select(x=>PobierzZamowieniePowiazane(x,klient)).ToList();

            //pobranie zamowien b2b korespondujacych z zamowieniami z ERP - zeby miec np. osobe skladajaca, co wpisal klient,

            //jeśli pobieramy zamówienia to musimy dodać zamówienia które jeszcze się nie pobrały do ERPa
            //var zamowieniaDoDodania = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowieniaBLL>(klient, )
            if (rodzaj == RodzajDokumentu.Zamowienie)
            {
                string sql = string.Format("SELECT DISTINCT z.* from zamowienie z LEFT JOIN [ZamowienieDokumentyERP] zd on zd.IdZamowienia = z.Id " +
                                           " WHERE ( (zd.IdDokumentu is null) OR (zd.IdDokumentu not in (select hd.Id from HistoriaDokumentu hd)) ) " +
                                           " AND (z.KlientId in ({0}) )", idKLientowDoPokazania.ToCsv());

                if (statusyDoNIEPokazywania.Any())
                {
                    sql += string.Format(" AND (z.StatusId not in ({0}) )", statusyDoNIEPokazywania.ToCsv());
                }
                if (odKiedy != DateTime.MinValue)
                {
                    sql += string.Format(" AND (z.DataUtworzenia >= '{0}' )", odKiedy.ToString("yyyy-MM-dd 0:0:0") );
                }

                if (doKiedy != DateTime.MaxValue)
                {
                    sql += string.Format(" AND (z.DataUtworzenia < '{0}' )", doKiedy.AddDays(1).ToString("yyyy-MM-dd 0:0:0") );
                }

                //zamowienie dotychczas NIE zaimportowane

                IList<ZamowieniaBLL> zamowieniaB2B = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.SqlList<ZamowieniaBLL>(sql);
                //przez to ze pobieramy po swojemy SQL musimy uruchomić filtrowanie po selectcie recznie - normalnie ORM by to odpalil
                zamowieniaB2B = this.PobierzElementyPoSelect(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny, klient, zamowieniaB2B, null);

                if (zamowieniaB2B.Any())
                {
                    docs.AddRange(zamowieniaB2B.Select(x => new DokumentyBll(x)));
                }

            }

           if (!string.IsNullOrEmpty(szukanieFraza))
           {
                //todo: zmienić na filtrowanie SQL
               docs = SolexBllCalosc.PobierzInstancje.Szukanie.WyszukajObiekty(docs, szukanieFraza, Calosc.Konfiguracja.DokumentyWyszukiwanie).ToList();
           }
           if (sortuj)
            {  
                //todo: zmienić na sortowanie SQL           
                Sortowanie sortowanie = Calosc.ProfilKlienta.PobierzSortowanie(zalogowany, TypUstawieniaKlienta.KolumnaSortowaniaDokumentow);

               if (sortowanie == null || sortowanie.Pola.IsEmpty())
               {
                   return docs.ToList();
               }

                docs = SolexBllCalosc.PobierzInstancje.Szukanie.SortujObiekty(docs, sortowanie).ToList();
            }

            //obojetnie czy mamy do czyenienia z faktramia czy zamowieniami - musimy dodac im dokumenty bazowe - zamowienia b2b, i zwykle bazowe
            if (UzupelnijODokumentyPowiazane)
            {
               docs = PobierzZamowieniePowiazane(docs,klient);
            }
            return docs;
        }


        public DocumentSummary WygenerujeDaneDoWykresuFaktur(List<DokumentyBll> dokumenty)
        {
            DocumentSummary wynik = new DocumentSummary();
            var dok = dokumenty.Where(x => x.Rodzaj == RodzajDokumentu.Faktura).ToList();
            if (!dok.Any())
            {
                return wynik;
            }
            string waluta = dok.First().walutaB2b;
            decimal niezaplacone = decimal.Round(dok.Where(x => !x.Zaplacono && x.DokumentWartoscNalezna != null).Sum(x => x.DokumentWartoscNalezna), 2);
            int iloscniezaplacone = dok.Count(x => x.DokumentDniSpoznienia == 0 && !x.Zaplacono);
            int iloscprzetermiowane = dok.Count(x => x.DokumentDniSpoznienia > 0);
            int iloscZaplacone = dok.Count(x => x.Zaplacono);
            decimal zaplacone = decimal.Round(dok.Where(x => x.Zaplacono).Sum(x => x.DokumentWartoscBrutto), 2);
            decimal przeterminowane = decimal.Round(dok.Where(x => x.DokumentDniSpoznienia > 0 && x.DokumentWartoscNalezna != null).Sum(x => x.DokumentWartoscNalezna), 2);
            wynik.Niezaplacone = new DaneDoWykresu(new WartoscLiczbowa(niezaplacone, waluta), iloscniezaplacone);
            wynik.Zaplacone = new DaneDoWykresu(new WartoscLiczbowa(zaplacone, waluta), iloscZaplacone);
            wynik.Przeterminowane = new DaneDoWykresu(new WartoscLiczbowa(przeterminowane, waluta), iloscprzetermiowane);
            return wynik;
        }

        public DocumentSummary WygenerujeDaneDoWykresuZamowien(List<DokumentyBll> dokumenty)
        {
            DocumentSummary wynik = new DocumentSummary();
            var a = dokumenty.Where(x => x.Rodzaj != RodzajDokumentu.Zamowienie);
            var dok = dokumenty.Where(x => x.Rodzaj == RodzajDokumentu.Zamowienie).ToList();
            if (!dok.Any())
            {
                return wynik;
            }
            string waluta = dok.First().walutaB2b;
            decimal niezealizowane = decimal.Round(dok.Where(x => !x.DokumentZrealizowany).Sum(x => x.DokumentWartoscBrutto), 2);
            decimal zrealizowane = decimal.Round(dok.Where(x => x.DokumentZrealizowany).Sum(x => x.DokumentWartoscBrutto), 2);
            int zrealizowaneilosc = dok.Count(x => x.DokumentZrealizowany);
            int niezrealizowaneilosc = dok.Count(x => !x.DokumentZrealizowany);
            wynik.Zrealizowane = new DaneDoWykresu(new WartoscLiczbowa(zrealizowane, waluta), zrealizowaneilosc);
            wynik.Niezrealizowane = new DaneDoWykresu(new WartoscLiczbowa(niezealizowane, waluta), niezrealizowaneilosc);
            return wynik;
        }

        public DocumentSummary PobierzPodsumowanieFakturKlient(IKlient klient)
        {
            List<DokumentyBll> dok = PobierzWyfiltrowaneDokumenty(klient, klient, RodzajDokumentu.Faktura, DateTime.MinValue, DateTime.MaxValue, null, false);
            if (dok == null)
            {
                return null;
            }
            DocumentSummary suma = WygenerujeDaneDoWykresuFaktur(dok);
            return suma;
        }

        public void UsunNiepotrzebne()
        {
            List<int> idDokumentowDoUsuniecia = Calosc.DostepDane.Pobierz<HistoriaDokumentu>(null, x => x.DataUtworzenia <= Calosc.Konfiguracja.DokumentyOdKiedyPobierane).Select(x => x.Id).ToList();

            Debug.WriteLine("Do usunięcia {0} dokumentów", idDokumentowDoUsuniecia.Count);

            int max = 100;
            while (idDokumentowDoUsuniecia.Any())
            {
                int dopobrania = idDokumentowDoUsuniecia.Count > max ? max : idDokumentowDoUsuniecia.Count;
                List<int> listaiddok = idDokumentowDoUsuniecia.Take(dopobrania).ToList();
                if (listaiddok.Any())
                {
                    Calosc.DostepDane.Usun<HistoriaDokumentu, int>(listaiddok);
                }
                idDokumentowDoUsuniecia.RemoveRange(0, dopobrania);
            }
        }


        /// <summary>
        /// Zapytanie które wyciągaklientów ktorzy nie mają konta nadrzednego, jest aktywne, nie posiada kategorii wykluczonej z wysyłania wiadomości oraz takich którzy maja nie zapłacone faktury (z terminem płatności).
        /// </summary>
        private string sqlKlienciNiezaplaconeDokumenety = "select distinct {1} from Klient k join HistoriaDokumentu hd on (hd.KlientId=k.Id) " +
                                                                                  "where k.Aktywny= 1 and k.KlientNadrzednyId is null {0} and hd.Zaplacono=0 and TerminPlatnosci is not null and Rodzaj = 'Faktura' and WartoscBrutto>0";

        /// <summary>
        /// Metoda pobieraąca aktywnych klientów którym można wysyłać wiadomośc o nadchodzących/przeterminowanych płatnościach
        /// </summary>
        /// <returns></returns>
        public Dictionary<long, Klient> PobierzKlientowDoKtorychWysylacPowiadomieniaONiezaplaceniu(string zapytanie)
        {
            //Jezeli nie ma kategorii wykluczanej odpalamy odpowiedznie zapytanie

            string caleZapytanie = string.IsNullOrEmpty(zapytanie)?"": $"and k.Id in ({zapytanie})";

            return Calosc.DostepDane.DbORM.Select<Klient>(string.Format(sqlKlienciNiezaplaconeDokumenety, caleZapytanie, "k.*")).ToDictionary(x => x.Id, x => x);
            //if (string.IsNullOrEmpty(zapytanie))
            //{

            //}
            //return Calosc.DostepDane.DbORM.Select<Klient>(string.Format(sqlKlienciNiezaplaconeDokumenetyZKategoriamiWykluczonymi, string.Join(",", idKategoriiKlienta), "k.*")).ToDictionary(x => x.Id, x => x);
        }

        public string ZbudujWarunekDlaKategorii(int[] kategoriaKlientaNieWysylaj, int[] kategoriaKlientaWysylaj)
        {
            string warunek = string.Empty;
            if ((kategoriaKlientaNieWysylaj != null && kategoriaKlientaNieWysylaj.Any()) || (kategoriaKlientaWysylaj != null && kategoriaKlientaWysylaj.Any()))
            {
                warunek = "select distinct KlientId from KlientKategoriaKlienta where {0})";
                string w = string.Empty;
                if (kategoriaKlientaNieWysylaj != null && kategoriaKlientaNieWysylaj.Any())
                {
                    w += $"(KategoriaKlientaId in ({ string.Join(",", kategoriaKlientaNieWysylaj)}) or";
                }
                if ((kategoriaKlientaWysylaj != null && kategoriaKlientaWysylaj.Any()))
                {
                    w += $" KategoriaKlientaId not in ({ string.Join(",", kategoriaKlientaWysylaj)})";
                }
                if (!string.IsNullOrEmpty(w))
                {
                    if (w.EndsWith("or"))
                    {
                        w = w.TrimEnd(" or");
                    }
                    else if (!w.StartsWith("("))
                    {
                        w = $"({w}";
                    }
                }
                warunek = string.Format(warunek, w);
            }
            return warunek;
        }
        /// <summary>
        /// zapytanie wyciagająca dokumenty które były wysłane
        /// </summary>
        private string sqlDokumentyWyslane = "select Wartosc from DzialaniaUzytkownikow dz join DzialaniaUzytkwonikowParametry dup on (dz.Id= dup.IdDzialania) where Data >= '{0}' " +
                             "and dz.ZdarzenieGlowne = 'PrzypomnienieNiezaplaconejFakturze' and dup.NazwaParametru = 'Dokumenty id'";

        /// <summary>
        /// sql pobierajacy obiekty dokumentów
        /// </summary>
        private string sqlObiektyDokumentow = "select * from HistoriaDokumentu where Rodzaj = 'Faktura' and Zaplacono=0 and WartoscBrutto>0 and WartoscNalezna!=0 and " +
                                              "TerminPlatnosci is not null and(TerminPlatnosci< '{0}' {1}) {2}";
        //or TerminPlatnosci>'{1}'

        public HashSet<long> PobierzKlientowKtorymWyslacMaileOPrzeterminowaniu(int dokumentPrzeterminujesieWCiagu, int mineloConajmniejOdTerminuPlatnosi, int? ileDniPonowneWyslanie, string warunek, string odJakiejDatySprawdzacPonowneWysylanie, out Dictionary<long, List<HistoriaDokumentu>> slownikDokumentowPogrupowanychPoKliencie)
        {
            //Pobieramy z działań użytkowników wszystkie dokumenty dla których wyslanebyły emaile
            HashSet<int> idDokumentow = new HashSet<int>();
            List<string> dokumenty = Calosc.DostepDane.DbORM.SqlList<string>(string.Format(sqlDokumentyWyslane, odJakiejDatySprawdzacPonowneWysylanie));
            foreach (var d in dokumenty)
            {
                idDokumentow.UnionWith(d.Split(',').Select(int.Parse));
            }

            //pobieramy daty dla których pobieramy dokumenty przeterminowane i nadchodzace
            string datDoKiedyPrzeterminujeSieWysylac = dokumentPrzeterminujesieWCiagu==0?null:DateTime.Now.Date.AddDays(dokumentPrzeterminujesieWCiagu).ToString("yyyy-MM-dd");
            string poIluDniachOdTerminuPlatnosci = DateTime.Now.Date.AddDays(-mineloConajmniejOdTerminuPlatnosi).ToString("yyyy-MM-dd");

            //pobieramy wszystkie faktury które maja odpowiednie daty oraz klientami są wyfiltrowani klienci
            string calyWarunek = string.IsNullOrEmpty(warunek) ? "" : $"and KlientId in ({warunek})";

            Dictionary<int, HistoriaDokumentu> listaDokumentow = Calosc.DostepDane.DbORM.SqlList<HistoriaDokumentu>(string.Format(sqlObiektyDokumentow, poIluDniachOdTerminuPlatnosci, datDoKiedyPrzeterminujeSieWysylac==null?"":$"or (TerminPlatnosci<'{datDoKiedyPrzeterminujeSieWysylac}' and TerminPlatnosci>='{DateTime.Now.Date.ToString("yyyy-MM-dd")}')", calyWarunek)).ToDictionary(x => x.Id, x => x);


            //slowniek pogrupowanych względem klienta wysłanych dokumentów
            Dictionary<long, List<int>> dokumetnyWyslane = listaDokumentow.WhereKeyIsIn(idDokumentow).GroupBy(x => x.KlientId).ToDictionary(x => x.Key, x => x.Select(y => y.Id).ToList());

            //slownik wszystkich dokumentów spelniających warunki o których mail powinien być wysłany (jeszcze nie sa przefiltrowane pod względem wcześniejszego wysłąnia)
            slownikDokumentowPogrupowanychPoKliencie = listaDokumentow.Values.GroupBy(x => x.KlientId).ToDictionary(x => x.Key, x => x.ToList());

            //wyciagamy pogrupowane dokumenty względem klieta które będą wysłane w mailu o przeterminowanych płatnościach
            return new HashSet<long>( slownikDokumentowPogrupowanychPoKliencie.Where(x => !dokumetnyWyslane.ContainsKey(x.Key) || x.Value.Any(y => !dokumetnyWyslane[x.Key].Contains(y.Id))).Select(x => x.Key) );
        }

        /// <summary>
        /// Główna metoda odpowiedzialna za pobranie przeterminowanych i nadchodzących platności oraz wysłanie powiadomień mailowych. Zgodnie z zaleceniami pobieramy tylko dokumenty gdzie klient kupujący zalega z płatnościami, nie pobieramy taich które my zalegamy klientowi
        /// </summary>
        /// <param name="dokumentPrzeterminujesieWCiagu"></param>
        /// <param name="mineloConajmniejOdTerminuPlatnosi"></param>
        /// <param name="ileDniPonowneWyslanie"></param>
        /// <param name="kategoriaKlientaNieWysylaj"></param>
        /// <param name="kategoriaKlientaWysylaj"></param>
        public void WyslijMailaOPrzeterminowanychFakturach(int dokumentPrzeterminujesieWCiagu, int mineloConajmniejOdTerminuPlatnosi, int? ileDniPonowneWyslanie, int[] kategoriaKlientaNieWysylaj, int[] kategoriaKlientaWysylaj)
        {
            //pobieramy klientów którym trzeba wysłąć maila i przeterminowanych platnosciach
            string warunek = ZbudujWarunekDlaKategorii(kategoriaKlientaNieWysylaj, kategoriaKlientaWysylaj);
            Dictionary<long, Klient> klienci = PobierzKlientowDoKtorychWysylacPowiadomieniaONiezaplaceniu(warunek);
            //Brak klientów do wysłania pomijamy reszte
            if (klienci == null || !klienci.Any())
            {
                return;
            }
            
            //Wyciagamu idDOkumentów wysłanych
            string odJakiejDatySprawdzacPonowneWysylanie = "1900-01-01";
            if (ileDniPonowneWyslanie.HasValue)
            {
                odJakiejDatySprawdzacPonowneWysylanie = DateTime.Now.Date.AddDays(-ileDniPonowneWyslanie.Value).ToString("yyyy-MM-dd");
            }
            Dictionary<long, List<HistoriaDokumentu>> slownikDokumentowPogrupowanychPoKliencie;
            HashSet<long> dlaKtorychKlientowWyslac = PobierzKlientowKtorymWyslacMaileOPrzeterminowaniu(dokumentPrzeterminujesieWCiagu, mineloConajmniejOdTerminuPlatnosi, ileDniPonowneWyslanie, warunek, odJakiejDatySprawdzacPonowneWysylanie, out slownikDokumentowPogrupowanychPoKliencie);


            if (dlaKtorychKlientowWyslac.Any())
            {
                //Przechodzimy po kliencie oraz wysyłamy wiadomości
                foreach (var klient in dlaKtorychKlientowWyslac)
                {
                    Klient k = klienci[klient];
                    Calosc.Statystyki.ZdarzeniePowiadomieniePrzeterminowanejNadchodzacejPlatnosc(slownikDokumentowPogrupowanychPoKliencie[klient].Select(x => new DokumentyBll(x) { Klient = k }).ToList(), k);
                }
            }
        }

        public bool MozliwaPlatnoscZamowienia(ZamowieniaBLL zamowienie)
        {
         
            bool czyZaplacony = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentuPlatnosciOnline>(null, x => x.IdDokumentu == zamowienie.Id).Any();
            return SolexBllCalosc.PobierzInstancje.Konfiguracja.StatusyZamowien[(int)zamowienie.StatusId].PlatnoscOnline && !czyZaplacony;
        }

        public void WysylaniePowiadomienONowychFakturach(HashSet<int> kategorie)
        {
            IList<Klient> listaKlientow;
            if (kategorie != null && kategorie.Any())
            {
                IEnumerable<long> klienciids = Calosc.DostepDane.Pobierz<KlientKategoriaKlienta>(null, x => Sql.In(x.KategoriaKlientaId, kategorie)).Select(x => x.KlientId);
                listaKlientow = Calosc.DostepDane.Pobierz<Klient>(null, x => Sql.In(x.Id, klienciids));// klienciids.Contains(x.Id));
            }
            else
            {
                listaKlientow = Calosc.DostepDane.Pobierz<Klient>(null, x => x.Aktywny);
            }

            //todo: filtracja ktorzy klienci chca powiadomienie w ogole o nowych fakturach i czy jest aktywne powiadoeminie

            DateTime data = DataOdkiedyWysylacMailaNoweDokumenty();
            IList<DokumentyBll> faktury = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DokumentyBll>(null, x => x.Rodzaj == RodzajDokumentu.Faktura
                                                                                                                           && x.DataWyslaniaDokumentu == null
                                                                                                                           && Sql.In(x.KlientId, listaKlientow.Select(z => z.Id)) && (x.DataUtworzenia >= data));

            foreach (Klient k in listaKlientow)
            {
               
                IKlient iKlient = k;
                HashSet<long> subkontaids = new HashSet<long>(iKlient.Subkonta().Select(x => x.Id));

                IEnumerable<DokumentyBll> nowe = faktury.Where(x => CzyPlatnikSieZgadza(x, iKlient, subkontaids));
                List<DokumentyBll> doWyslania = new List<DokumentyBll>();
                foreach (DokumentyBll dokument in nowe)
                {
                    if (Calosc.Konfiguracja.WysylajPowiadomienieFakturaGdyBrakPdf && !IstniejeZalacznik(dokument.Id, "pdf"))
                    {
                        continue;   //nie wysylamy bo nie ma PDFa, a ma być
                    }

                    doWyslania.Add(dokument);
                }

                if (doWyslania.Any())
                {
                    try
                    {
                        List<DokumentyBll> wynik = new List<DokumentyBll>();
                        Calosc.Statystyki.ZdarzenieNoweDokumenty(doWyslania, iKlient);
                        foreach (DokumentyBll dok in doWyslania)
                        {
                            dok.DataWyslaniaDokumentu = DateTime.Now;
                            wynik.Add(dok);
                        }
                        SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<DokumentyBll>(wynik);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Błąd wysyłania nowych dokumentów do klienta id: {0}. Dokumenty: {1}", iKlient.Id, doWyslania.Select(x => x.NazwaDokumentu).ToCsv());
                        Log.Error(e);
                        throw;
                    }
                }
            }
        }

        public Dictionary<int, long> PobierzSumyKontrolnePozycjiDokumentow(HashSet<int> idDokumentow)
        {
            Dictionary<int,long>wynik=new Dictionary<int, long>();
            Dictionary<int, List<HistoriaDokumentuProdukt>> pozycje;
            if (idDokumentow != null && idDokumentow.Any())
            {
                pozycje = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentuProdukt>(null, x => Sql.In(x.DokumentId, idDokumentow)).GroupBy(x => x.DokumentId).ToDictionary(x => x.Key, x => x.ToList());
            }
            else
            {
                var tmp = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentuProdukt>(null);
                pozycje = tmp.GroupBy(x => x.DokumentId).ToDictionary(x => x.Key, x => x.ToList());
            }
            //var 
            foreach (var p in pozycje)
            {
                long hash = Tools.PobierzInstancje.PoliczHashPozycjiDokumentu(p.Value);
                wynik.Add(p.Key, hash);
            }
            return wynik;
        }


        public bool GetMozliwaPlatnosc(DokumentyBll dokument)
        {
            if (dokument.Rodzaj == RodzajDokumentu.Zamowienie)
            {
                if (!dokument.StatusId.HasValue)
                {
                    return false;
                }
                var status = SolexBllCalosc.PobierzInstancje.Konfiguracja.StatusyZamowien[dokument.StatusId.Value];

                return status != null && status.PlatnoscOnline;
            }
            if (dokument.Rodzaj == RodzajDokumentu.Faktura)
            {
                return !((HistoriaDokumentu) dokument).Zaplacono;
            }
            return false;
        }

        public IList<T> PobierzNumeryZamowienERPPoSelectZamowieniaB2B<T>(int jezykId, IKlient zadajacy, IList<T> obj, object parametrDoMetodyPoSelect = null)
        {
            Dictionary<long, Zamowienie> ids = new Dictionary<long, Zamowienie>(obj.Count);
            foreach (var zam in obj)
            {
                var z = zam as Zamowienie;
                if (z == null)
                {
                    throw new Exception("Metoda uzupełnienia numerów zamówień może działać tylko dla obietku typu Zamowienie");
                }
                ids.Add(z.Id, z);
            }

            var numeryDokumentowERP = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowienieDokumenty>(zadajacy, x => Sql.In(x.IdZamowienia, ids.Keys));

            foreach (var zamowienieDokumenty in numeryDokumentowERP)
            {
                Zamowienie zamowienie = ids[zamowienieDokumenty.IdZamowienia];
                if (zamowienie.dokumentyERPStworzoneZZamowienia == null)
                {
                    zamowienie.dokumentyERPStworzoneZZamowienia = new Dictionary<long, string>();
                }
                zamowienie.dokumentyERPStworzoneZZamowienia.Add(zamowienieDokumenty.IdDokumentu, zamowienieDokumenty.NazwaERP);
            }
            return obj;
        }

        /// <summary>
        /// Metoda przetwarzjaca pobrane dokumenty z bazy danych - po selekcie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jezykId"></param>
        /// <param name="zadajacy"></param>
        /// <param name="obj"></param>
        /// <param name="parametrDoMetodyPoSelect"></param>
        /// <returns></returns>
        public IList<T> PobierzElementyPoSelect<T>(int jezykId, IKlient zadajacy, IList<T> obj, object parametrDoMetodyPoSelect=null)
        {
            if (obj.IsEmpty())
            {
                return obj;
            }

            if (!( typeof(T) == typeof(DokumentBazowy)  || (typeof(T).IsDerivativeOf(typeof(DokumentBazowy))) ) )
            {
                throw new Exception("Błąd wywołania metody pobierania klienta oraz waluty dla dokumentu");
            }

            Dictionary<int, long>slownkDokumentowIKlientow = new Dictionary<int, long>();

            //sprawdzamy czy wyciagalismy DokumentyBll, w dokumentachbll może być inny odbiora a inny płatnik
            if (typeof(T) == typeof(DokumentyBll))
            {
                //sprawdzamy czy pobierac chce klient czy odbiorca
                foreach (var dok in obj)
                {
                    var dokum = dok as DokumentyBll;
                    //Gdy zadający jest odbiorca musimy pobierać względem obiorcy nie płatnika. Odbiorca może nie widzieć płatnika
                    //idsKlientowDoPobranychDokumentow.Add(zadajacy.Id == dokum.OdbiorcaId ? dokum.OdbiorcaId.Value : dokum.KlientId);
                    slownkDokumentowIKlientow.Add(dokum.Id, zadajacy!=null && zadajacy.Id == dokum.OdbiorcaId ? dokum.OdbiorcaId.Value : dokum.KlientId);
                }
            }
            else
            {
                slownkDokumentowIKlientow = obj.ToDictionary(x=> (x as DokumentBazowy).Id, x => (x as DokumentBazowy).KlientId);
            }

            

            //List<long> idsKlientowDoPobranychDokumentow = obj.Select(x => (x as DokumentBazowy).KlientId).Distinct().ToList();

            Dictionary<long, Klient> klienci;

            if (slownkDokumentowIKlientow.Count > 1)
            {
                klienci = Calosc.DostepDane.Pobierz<Klient>(zadajacy, x => Sql.In(x.Id, slownkDokumentowIKlientow.Values)).ToDictionary(x => x.Id, x => x);
            }
            else
            {
                klienci = new Dictionary<long, Klient> {{ slownkDokumentowIKlientow.Values.First(), Calosc.DostepDane.PobierzPojedynczy<Klient>(slownkDokumentowIKlientow.Values.First(), zadajacy)}};
            }

            List<int> idsDOkumentowPobranych = obj.Select(x => (x as IHasIntId).Id).ToList();
            Dictionary<int, HashSet<HistoriaDokumentuListPrzewozowy>> listyPrzewozowe = Calosc.DostepDane.Pobierz<HistoriaDokumentuListPrzewozowy>(zadajacy, x => Sql.In(x.DokumentId, idsDOkumentowPobranych)).GroupBy(x => x.DokumentId).ToDictionary(x => x.Key, x =>  new HashSet<HistoriaDokumentuListPrzewozowy>( x ));

            foreach (T dok in obj)
            {
                var dokument = dok as DokumentBazowy;

                //albo platnik albo odbiorca
                long idKlientaZdokumentu = slownkDokumentowIKlientow[dokument.Id];

                if (klienci.ContainsKey(idKlientaZdokumentu))
                {
                    dokument.Klient = klienci[idKlientaZdokumentu];
                }
                if (dokument.WalutaId.HasValue &&  Calosc.Konfiguracja.SlownikWalut.ContainsKey(dokument.WalutaId.Value))
                {
                    dokument.walutaB2b = Calosc.Konfiguracja.SlownikWalut[dokument.WalutaId.Value].WalutaB2b;
                }

                if (listyPrzewozowe.Any())
                {
                    HashSet<HistoriaDokumentuListPrzewozowy> listy;
                    if (listyPrzewozowe.TryGetValue(dokument.Id, out listy))
                    {
                        dokument.ListyPrzewozowe = listy;
                    }
                }
            }

            //defakto obuetku Zamowienie niegdy nie pobieramy tak, ale na wszelki wypadke niech taki if bedzie juz 
            if (typeof(T) == typeof(ZamowieniaBLL) || typeof(T) == typeof(Zamowienie))
            {
                return PobierzNumeryZamowienERPPoSelectZamowieniaB2B(jezykId, zadajacy, obj, parametrDoMetodyPoSelect);
            }

            return obj;
        }

       

        public IList<T> UzupelnijPozycjePoSelect<T>(int jezykID, IKlient zadajacy, IList<T> objekty, object parametrDoMetodyPoSelect)
        {
            var jednostki = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Jednostka>(zadajacy).ToDictionary(x => x.Id, x => x.Nazwa);
            var obiektDokumentu = parametrDoMetodyPoSelect as DokumentBazowy;

            foreach (var pozycja in objekty)
            {
                //jesli wyciagmy obiekty BLLowskie
                var pozycjaBll = pozycja as IPozycjaDokumentuBll;
                if (pozycjaBll != null)
                {
                    if (obiektDokumentu == null)
                    {
                        throw new Exception("Ne można pobierać pozycji BLL, bez podania obiektu dokumentu dla którego jest pobierana pozycja (typ DokumentBazowy). Pobierz obiekt nie BLL, lub dodaj parametr dokumentu");
                    }
                    pozycjaBll.walutaB2b = obiektDokumentu.walutaB2b;
                    //nie mozemy mieć tutaj produktu klienta dlatego że klient moze tego produktu nie miec w ofercie, ale ktos mogl mu sprzedać go i ma go na dokumencie
                    pozycjaBll.ProduktBazowy = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(pozycjaBll.ProduktId, zadajacy.JezykId, obiektDokumentu.Klient);

                    if (pozycjaBll.ProduktBazowy == null)
                    {
                        if(pozycjaBll is ZamowieniaProduktyBLL)
                        {
                            pozycjaBll.ProduktBazowy = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>((pozycjaBll as ZamowieniaProduktyBLL).ProduktIdBazowy, zadajacy.JezykId, obiektDokumentu.Klient);
                        }
                        else
                        {
                            pozycjaBll.ProduktBazowy = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(pozycjaBll.ProduktId, zadajacy.JezykId, obiektDokumentu.Klient);
                        }
                    }

                    pozycjaBll.UstawJednostke(jednostki[pozycjaBll.JednostkaMiary]);
                    continue;
                }

                //jesli wyciagamy obiekty zwykle
                var pozycjaZwykla = pozycja as IDokumentuPozycjaBazowa;
                if (pozycjaZwykla != null)
                {
                    if (!jednostki.ContainsKey(pozycjaZwykla.JednostkaMiary))
                    {
                        throw new Exception(String.Format("Pozycja:{0} w dokumencie: {1} ma jednostkę o id {2} która nie jest obecna w bazie.",pozycjaZwykla.Id, pozycjaZwykla.DokumentId, pozycjaZwykla.JednostkaMiary));
                    }
                    pozycjaZwykla.UstawJednostke(jednostki[pozycjaZwykla.JednostkaMiary]);
                    continue;
                }
            }

            //ProduktyWirtualneProvider prov = SolexBllCalosc.PobierzInstancje.Konfiguracja.WirtualneProduktyProvider;
            //if (prov != null && obiektDokumentu!=null)
            //{
            //    prov.PoprawProduktyNaDokumentach(obiektDokumentu.Klient,ref (HistoriaDokumentuProdukt)objekty);
            //}


            return objekty;
        }

        public bool CzySaDokumentyDlaKlienta (RodzajDokumentu rodzaj, IKlient klient )
        {
            //pokazujemy tylko dla klienta wybranego i jego wszystkich subkont
            HashSet<long> idKLientowDoPokazania = new HashSet<long>( klient.WszystkieKontaPodrzedne().Select(x => x.Id) );
            idKLientowDoPokazania.Add(klient.Id);

            HashSet<int> statusy = new HashSet<int>( Calosc.DostepDane.Pobierz<StatusZamowienia>(null, x => x.Widoczny).Select(x => x.Id) );
            int ilosc = (int) Calosc.DostepDane.DbORM.Count<HistoriaDokumentu>(x => (Sql.In(x.KlientId, idKLientowDoPokazania) || x.OdbiorcaId == klient.Id) && x.Rodzaj == rodzaj && (x.StatusId==null || Sql.In(x.StatusId,statusy)));
            if (ilosc == 0 && rodzaj == RodzajDokumentu.Zamowienie)
            {
                ilosc = (int)Calosc.DostepDane.DbORM.Count<Zamowienie>(x => x.KlientId == klient.Id && Sql.In(x.StatusId, statusy));
            }
            return ilosc > 0;
        }
        public void UsunZamowienieDokument(IList<int> obj)
        {
            var zamowieniaDokument = Calosc.DostepDane.Pobierz<ZamowienieDokumenty>(null, x => Sql.In(x.IdDokumentu, obj));
            if (zamowieniaDokument == null || !zamowieniaDokument.Any())
            {
                return;
            }

            var idZamowien = new HashSet<int>( zamowieniaDokument.Select(x => x.IdZamowienia) ) ;

            //Pobieramy zamowienie i sprawdzamy ile jest dokumentów powiazanych z tym zamowieniem
            HashSet<int> idZamowienDoUsuniecia = new HashSet<int>( Calosc.DostepDane.Pobierz<ZamowienieDokumenty>(null, z => Sql.In(z.IdZamowienia, idZamowien)).GroupBy(x => x.IdZamowienia).Where(x => x.Count() == 1).Select(x => x.Key) );
            Log.InfoFormat("Usuwamy dokumenty o id: {0}",string.Join(";",obj));
            //Ustawiamy status usuniety tylko zamówieniom które nie maja juz wiecej połączeń z dokumentami
            if (idZamowienDoUsuniecia.Any())
            {
                Log.InfoFormat("Usuwamy zamówienie dla dokumentów z wcześniejszego logu ich id to: {0}", string.Join(";", idZamowienDoUsuniecia));
                var zamowienia = Calosc.DostepDane.Pobierz<Zamowienie>(null, x => Sql.In(x.Id, idZamowienDoUsuniecia));
                foreach (var zamowieniaBll in zamowienia)
                {
                    zamowieniaBll.StatusId = StatusImportuZamowieniaDoErp.Usunięte;
                }
                Calosc.DostepDane.AktualizujListe(zamowienia);
            }
            Calosc.DostepDane.Usun<ZamowienieDokumenty, long>(zamowieniaDokument.Select(x=>x.Id).ToList());
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<WiadomoscEmail>());
        }
        /// <summary>
        /// Przed aktualizacja zamowienia jeżeli status zamówienia będzie usuniety to sprawdzamy czy jest powiazanie z dokumentem i je usuwamy
        /// </summary>
        /// <param name="obj"></param>
        public void UsunDokumentDlaUsunietegoZamowienia(IList<ZamowieniaBLL> obj)
        {
            var zamowienieUsuniete = new HashSet<int>( obj.Where(x => x.StatusId == StatusImportuZamowieniaDoErp.Usunięte).Select(x => x.Id) );
            if (!zamowienieUsuniete.Any())
            {
                return;
            }
            var zamowieniaDoumenty = Calosc.DostepDane.Pobierz<ZamowienieDokumenty>(null, x => Sql.In(x.IdZamowienia, zamowienieUsuniete)).Select(x=>x.Id).ToList();
            if (zamowieniaDoumenty.Any())
            {
                Calosc.DostepDane.Usun<ZamowienieDokumenty,long>(zamowieniaDoumenty.ToList());
            }
        }
    }
}