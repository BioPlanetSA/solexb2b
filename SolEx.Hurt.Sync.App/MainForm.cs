using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Controls;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System.IO;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Sync.App.Modules_.Helpers;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.PoziomyCenowe;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;

using System.Reflection;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using SolEx.Hurt.Model.Core;
using Adres = SolEx.Hurt.Model.Adres;
using HistoriaDokumentuListPrzewozowy = SolEx.Hurt.Model.HistoriaDokumentuListPrzewozowy;
using Klient = SolEx.Hurt.Model.Klient;
using System.Collections.Concurrent;
using SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Sync.App
{
    public partial class MainForm : Form, IWyswietlanieKomunikatu
    {
        private IAPIWywolania ApiWywolanie = APIWywolania.PobierzInstancje;
        public IConfigSynchro Konfiguracja = Core.Sync.SyncManager.PobierzInstancje.Konfiguracja;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private DateTime _dataRozpoczeciaOdliczania;

        public ISyncManager SyncManager { get; set; }

        public void InicjalizacjaFormatki()
        {
            this.SyncManager = Hurt.Core.Sync.SyncManager.PobierzInstancje;
            InitializeComponent();
            ApiWywolanie.ZalogujKlienta();
            PrzeladujListeZadan();
            LogiFormatki.PobierzInstancje.InicjalizujLogi(ref logText);
            if (Program.DzialajWTle)
            {
                Hide();
                ShowInTaskbar = false;
                //uruchom wszystkie zdaania do uruchomienia
                btnUruchom_Click(this, new EventArgs());
                WylaczAplikacje();
            }
            else
            {               
                UstawTimer();
            }
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                if (true)
                {
                    if ((releaseKey <= 378389))
                    {
                        throw new Exception("Twoja wersja frameworka nie spe³nia wymagañ. Potrzebujesz nowej wersji .NET Framework (min. 4.5.1)");
                    }
                }
            }
            //logowanie ustawione na ALL to pokazac komunikkat
            if (log.IsDebugEnabled)
            {
                this.BackColor = Color.PaleVioletRed;
                MessageBox.Show("W³¹czony tryb debugowania kodu. W takm trybie nie bêdzie dzia³aæ automatyczna synchronizacja.");
            }


            labelLicencja.Text = $"Licencja dla aplikacji: {ConfigurationManager.AppSettings["url"]}";
            string plik = Application.ExecutablePath;
            try
            {
                string wersja = Tools.PobierzInstancje.PobierzWersjeDLL(plik);
                if (wersja.Length > 18)
                {
                    wersja = wersja.Substring(0, 18);
                }
                lblWersja.Text = $"Wersja synchro.: {wersja}";
            }
            catch
            {
                throw new Exception("Nie mo¿na sprawdziæ numeru wersji dla aplikacji: " + plik  + ".");
            }

           
           // labelLicencjaNumer.Text = string.Format("Licencja numer: {0}", Tools.PobierzInstancje.GetMd5Hash(ConfigurationManager.AppSettings["url"]));


        }
        private void WylaczAplikacje()
        {
            log.Info("############### Koniec dzia³ania aplikacji #########################");
            if (Application.MessageLoop)
            {
                Application.Exit();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void PrzeladujListeZadan()
        {
            panelZadan.Controls.Clear();
            int i = 0;
            if (Program.ZadaniaDoUruchomienia != null)
            {
                foreach (var z in Program.ZadaniaDoUruchomienia)
                {
                    if (tylkoPlanowe.Checked && z.CzyPowinnoBycUruchomioneTeraz == false)
                    {
                        continue;
                    }
                    ZadanieKontrolka kon = new ZadanieKontrolka(z)
                    {
                        Zaznaczone = z.CzyPowinnoBycUruchomioneTeraz,
                        Anchor = AnchorStyles.Left
                    };

                    if (++i % 2 == 0)
                    {
                        kon.BackColor = Color.WhiteSmoke;
                    }

                    kon.OnClick += kon_OnClick;
         
                    kon.Dock = DockStyle.Fill;
                    panelZadan.Controls.Add(kon);
                }
            }

            panelZadan.AutoScroll = true;
            panelZadan.HorizontalScroll.Enabled = false;
            panelZadan.HorizontalScroll.Visible = false;
        }

        void kon_OnClick(object sender, Zadanie zadanieDoUruchomienia)
        {
            ZadanieKontrolka k = (sender as ZadanieKontrolka);

            foreach (object kontrolka in panelZadan.Controls)
            {
                if (kontrolka is ZadanieKontrolka)
                {
                    var kk = (kontrolka as ZadanieKontrolka);
                    if (kk.Zadanie.NazwaZadania != k.Zadanie.NazwaZadania)
                        kk.Zaznaczone = false;
                }
            }

            k.Zaznaczone = true;
        }


        protected string FolderZapisuPlikowTymczasowych = AppDomain.CurrentDomain.BaseDirectory + "paczka_temp\\";

        private ISyncProvider _aktualnyProv;
        protected ISyncProvider AktualnyProvider
        {
            get
            {
                if (_aktualnyProv == null)
                {
                    _aktualnyProv = SyncManager.GetProvider( SyncManager.Konfiguracja.ProviderERP, SyncManager.Konfiguracja);
                }
                return _aktualnyProv;
            }
        }


        public void AktualizujProduktyUkryte(List<ProduktUkryty> produktyUkryteERP)
        {
            if (produktyUkryteERP == null) return;

            Dictionary<long, ProduktUkryty> produktyukryteB2B = ApiWywolanie.PobierzProduktyUkryte();
            HashSet<long> doAktualizacji = new HashSet<long>();
            HashSet<long> doUsuniecia = new HashSet<long>();
            HashSet<long> doDodania = new HashSet<long>();

            var cechyB2B = ApiWywolanie.PobierzCechy();
            if (!cechyB2B.Any())
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"Na B2B nie ma ¿adnych cech? Produkty ukryte nie moga dzia³aæ w takich warunkach.");
                return;
            }

            HashSet<long> idCechWidocznych = new HashSet<long>( cechyB2B.Where(x=>x.Value.Widoczna).Select(x=>x.Key) );
            Dictionary<long, ProduktUkryty> produktyUkryteErpNowe = new Dictionary<long, ProduktUkryty>(produktyUkryteERP.Count);


            foreach (var pu in produktyUkryteERP)
            {
                pu.Synchronizowane = true;
                pu.Id = pu.WygenerujIDObiektuSHAWersjaLong();
                if (!produktyUkryteErpNowe.ContainsKey(pu.Id) && (!pu.CechaProduktuId.HasValue || (pu.CechaProduktuId.HasValue && idCechWidocznych.Contains(pu.CechaProduktuId.Value))))
                {
                    produktyUkryteErpNowe.Add(pu.Id, pu);
                }
            }

            LogiFormatki.PobierzInstancje.LogujInfo($"Pobrano cech widocznych z B2B: {idCechWidocznych.Count}, produktów ukrytych ERP przed filtracj¹ cech: {produktyUkryteERP.Count}. Do porównania: {produktyUkryteErpNowe.Count}");

            produktyukryteB2B.Porownaj(produktyUkryteErpNowe, ref doAktualizacji, ref doDodania, ref doUsuniecia, null, this);

            if (doAktualizacji.Any() || doDodania.Any())
            {
                if (!SyncManager.Konfiguracja.GetLicense(Licencje.moj_katalog) || !SyncManager.Konfiguracja.GetLicense(Licencje.katalog_klienta))
                {
                    log.DebugFormat($"Spis licencji: {SyncManager.Konfiguracja.Licenses.ToCsv()}");
                    throw new Exception("Brak licencji Moj katalog oraz Katalog klienta");
                }
            }

            if (doUsuniecia.Count > 0)
            {
                ApiWywolanie.UsunProduktyUkryte(doUsuniecia);
            }

            if (doDodania.Count > 0)
            {
                List<ProduktUkryty> ukryteDoDodania = produktyUkryteErpNowe.WhereKeyIsIn(doDodania);
                ApiWywolanie.AktualizujProduktyUkryte(ukryteDoDodania);
            }

            if (doAktualizacji.Count > 0)
            {
                List<ProduktUkryty> ukryteDoAktualizacji = produktyUkryteErpNowe.WhereKeyIsIn(doAktualizacji);
                ApiWywolanie.AktualizujProduktyUkryte(ukryteDoAktualizacji);
            }
        }

        public void aktualizujKonfekcje(Dictionary<long,Konfekcje> erp)
        {
            if (erp == null) return;

            Dictionary<long, Konfekcje> konfekcjeB2B = ApiWywolanie.PobierzKonfekcje().ToDictionary(a => a.Id, a => a);
            HashSet<long> doAktualizacji = new HashSet<long>();
            HashSet<long> doUsuniecia = new HashSet<long>();
            HashSet<long> doDodania = new HashSet<long>();
            //int id = -1;
            //foreach (var jp in konfekcjeerpprop)
            //{

            //    jp.Value.Id = konfekcjaB2Bprop.ContainsKey(jp.Key) ? konfekcjaB2Bprop[jp.Key].Id : id--;
            //}

            konfekcjeB2B.Porownaj(erp, ref doAktualizacji, ref doDodania, ref doUsuniecia, null, this);
            if (doUsuniecia.Count > 0)
            {
                ApiWywolanie.UsunKonfekcje(doUsuniecia.ToList());
            }
            if (doDodania.Count > 0)
            {
                List<Konfekcje> ukryteDoDodania = erp.WhereKeyIsIn(doDodania);
                ApiWywolanie.AktualizujKonfekcje(ukryteDoDodania);
            }
            if (doAktualizacji.Count > 0)
            {
                List<Konfekcje> ukryteDoAktualizacji = erp.WhereKeyIsIn(doAktualizacji);
                ApiWywolanie.AktualizujKonfekcje(ukryteDoAktualizacji);
            }
        }

        private void SprawdzKonfiguracje()
        {
            if (Directory.Exists(FolderZapisuPlikowTymczasowych))
            {
                Directory.Delete(FolderZapisuPlikowTymczasowych, true);
            }
            if (!string.IsNullOrEmpty( SyncManager.Konfiguracja.ERPcs))
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"Inicjalizacja po³¹czenia do systemu ksiêgowego: {SyncManager.Konfiguracja.ERPcs.Substring(0, 20)}");
                DBHelper.Baza.ConnectionString =  SyncManager.Konfiguracja.ERPcs;
            }

            if ((SyncManager.Konfiguracja.ProviderERP != ERPProviderzy.SymplexSB
                 && SyncManager.Konfiguracja.ProviderERP != ERPProviderzy.Tema && SyncManager.Konfiguracja.ProviderERP != ERPProviderzy.Vendo
                 && string.IsNullOrEmpty(SyncManager.Konfiguracja.ERPcs))
                || (SyncManager.Konfiguracja.ProviderERP == ERPProviderzy.SymplexSB && string.IsNullOrEmpty(SyncManager.Konfiguracja.KatalogPlikowWymianySymplex))
                || (SyncManager.Konfiguracja.ProviderERP == ERPProviderzy.Tema && string.IsNullOrEmpty(SyncManager.Konfiguracja.KatalogPlikowWymianyTema))
                || (SyncManager.Konfiguracja.ProviderERP == ERPProviderzy.Vendo && string.IsNullOrEmpty(SyncManager.Konfiguracja.ApiAdresVendo))
                )
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak danych do po³¹czenia do systemu ERP");
            }
        }
        private void WlaczZadanie(Zadanie z, List<SyncModul> moduly)
        {
            try
            {
                ZatrzymajTimer();

                LogiFormatki.PobierzInstancje.LogujInfo(" ----------- Rozpoczynanie zadania: " + z.NazwaZadania + " ----------- ");
                SprawdzKonfiguracje();
                Application.DoEvents();
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.StanyProduktów)
                {
                    AktualizujStany(moduly.Where(x => x is IModulStany).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.InformacjaODostepnosci)
                {
                    InformacjeODostepnosci(moduly.Where(x => x is IInformacjaODostepnosci).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.CechyIAtrybuty)
                {
                    AktualizujCechy(moduly.Where(x => x is IModulCechyIAtrybuty).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.Dokumenty)
                {
                    AktualizujDokumenty(moduly.Where(x => x is IModulDokumenty).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.ListyPrzewozowe)
                {
                    AktualizujListyPrzewozowe(moduly.Where(x => x is IModulListyPrzewozowe).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.WysylanieFakturPDF)
                {
                    AktualizujWysylaniePDF(moduly.Where(x => x is IFakturyPdf).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.KategorieProduktów)
                {
                    AktualizujKategorieProduktow(moduly.Where(x => x is IModulKategorieProduktow).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.Produkty)
                {
                    AktualizujProdukty(moduly.Where(x => x is IModulProdukty).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.Klienci)
                {
                    AktualizujKlientow(moduly.Where(x => x is IModulKlienci).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.KategorieKlientów)
                {
                    AktualizujKategorieKlientow(moduly.Where(x => x is IModulKategorieKlientow).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.Rabaty)
                {
                    AktualizujRabaty(moduly.Where(x => x is IModulRabaty).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.PoziomyCenowe)
                {
                    AktualizujPoziomyCenowe(moduly.Where(x => x is IModulPoziomyCen).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.ImportZamówieñ)
                {
                    ImportZamowien(moduly.OfType<IModulZamowienia>(), moduly.OfType<IPrzedZapisemZamowien>(), moduly.OfType<IPoZapisieZamowien>());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.ImportZdjec)
                {
                    AktualizujZdjecia(moduly.Where(x => x is IModulPliki).ToList());
                }

                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.ImportEksportXMLCSV)
                {
                    ImportEksport(moduly.Where(x => x is IModulEksportImportXMLCSV).ToList());
                }

                //if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.AktualizacjaProduktow)
                //{
                //    AktualizujProduktyMale(moduly.Where(x => x is IAktualizacjaProduktow).ToList());
                //}
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.ImportRejestracji)
                {
                    ImportRejestracji();
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.WyliczenieGotowychCen)
                {
                    AktualizujWyliczoneCeny(moduly.Where(x => x is IModulWyliczanieGotowychCen).ToList());
                }
                if (z.TypZadaniaSynchronizacji == ElementySynchronizacji.ZadaniaOgolne)
                {
                    WykonajZadaniaOgolne(moduly.Where(x => x is IZadaniaOglone).ToList());
                }
                Application.DoEvents();
            } catch (AggregateException e)
            {
                foreach (var exception in e.InnerExceptions)
                {
                    LogiFormatki.PobierzInstancje.LogujError(exception);
                }
            }
            catch (Exception e)
            {
                LogiFormatki.PobierzInstancje.LogujError(e);
            }
            finally
            {
                LogiFormatki.PobierzInstancje.LogujInfo($" ----------- Koniec zadania: {z.NazwaZadania} ----------- ");
                if (AktualnyProvider != null)
                {
                    AktualnyProvider.CleanUp();
                }
            }
            UstawTimer();
            string email = SyncManager.Konfiguracja.EmailCustomerError;
            if (!string.IsNullOrEmpty(email))
            {
                LogiFormatki.PobierzInstancje.WyslijMailaZBledami(email);
            }
           
        }

        private void InformacjeODostepnosci(List<SyncModul> moduly)
        {
            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IInformacjaODostepnosci).Przetworz();
                ZakonczModul(m);
            }
        }

        private void WykonajZadaniaOgolne(List<SyncModul> moduly)
        {
            AktualizujWaluty();
            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IZadaniaOglone).Przetworz(AktualnyProvider);
                ZakonczModul(m);
            }
        }

        private void AktualizujWyliczoneCeny(List<SyncModul> moduly)
        {
            //tylko licza moduly - jak ich nie ma to nie moze byc zadania
            if (!moduly.Any())
            {
                throw new Exception("Brak ustawionych modu³ów do liczenia gotowych cen! Wy³¹cz zadanie z synchronziacji!");
            }

            List<long> idproduktow = ApiWywolanie.PobierzProduktyId();
            Dictionary<long, Klient> dlaKogoLiczyc = ApiWywolanie.PobierzKlientow().Values.Where(x => x.Id > 0).ToDictionary(x => x.Id, x => x);
            List<FlatCeny> wynik = new List<FlatCeny>();
            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IModulWyliczanieGotowychCen).Przetworz(ref wynik, dlaKogoLiczyc, AktualnyProvider);
                ZakonczModul(m);
            }

            LogiFormatki.PobierzInstancje.LogujDebug($"Pobranych cen z erp: {wynik.Count}");
            wynik.RemoveAll(x => !idproduktow.Contains(x.ProduktId));//jakby by³y produkty których nie na na b2b

            LogiFormatki.PobierzInstancje.LogujDebug($"Cen z erp po przefiltrowaniu : {wynik.Count}");

            List<FlatCeny> cenyb2B = ApiWywolanie.PobierzCenyWyliczoneErp();
            LogiFormatki.PobierzInstancje.LogujDebug($"Pobranych cen z b2b: {cenyb2B.Count}");
            HashSet<long> doAktualizacji = new HashSet<long>();
            HashSet<long> doUsuniecia = new HashSet<long>();
            HashSet<long> doDodania = new HashSet<long>();
            foreach (var jp in wynik)
            {
                jp.CenaNetto = decimal.Parse(jp.CenaNetto.DoLadnejCyfry());
                jp.CenaHurtowaNetto = decimal.Parse(jp.CenaHurtowaNetto.DoLadnejCyfry());
            }

            //poprawieni ceny dokladnej - czaem jest tu problem
            foreach (var cena in wynik)
            {
                if (cena.CenaNettoDokladna == 0 && cena.CenaNetto != 0)
                {
                    cena.CenaNettoDokladna = cena.CenaNetto;
                }
            }

            Dictionary<long, FlatCeny> cenyErp = wynik.ToDictionary(a => a.Id, a => a);
            Dictionary<long, FlatCeny> cenyb2Bdic = cenyb2B.ToDictionary(a => a.Id, a => a);
            cenyb2Bdic.Porownaj(cenyErp, ref doAktualizacji, ref doDodania, ref doUsuniecia, null, this);
            if (doUsuniecia.Count > 0)
            {
                ApiWywolanie.UsunCenyWyliczoneErp(doUsuniecia.ToList());
            }
            if (doDodania.Count > 0)
            {
                List<FlatCeny> ukryteDoDodania = cenyErp.WhereKeyIsIn(doDodania);
                ApiWywolanie.AktualizujCenyWyliczoneErp(ukryteDoDodania);
            }
            if (doAktualizacji.Count > 0)
            {
                List<FlatCeny> ukryteDoAktualizacji = cenyErp.WhereKeyIsIn(doAktualizacji);
                ApiWywolanie.AktualizujCenyWyliczoneErp(ukryteDoAktualizacji);
            }
        }

        private void AktualizujRabaty(IEnumerable<SyncModul> moduly)
        {
            Dictionary<long, Rabat> rabatyNaB2B = ApiWywolanie.PobierzRabaty();
            LogiFormatki.PobierzInstancje.LogujInfo("Pobrano {0} rabatów z B2B", rabatyNaB2B.Count);
            Dictionary<long, Produkt> produkty = ApiWywolanie.PobierzProdukty().Values.Where(x => x.Widoczny).ToDictionary(x => x.Id, x => x);
            Dictionary<long, ProduktCecha> cechyProduktyB2B_Org = ApiWywolanie.PobierzCechyProdukty();

            var cechy_Produkty_DoZmian = cechyProduktyB2B_Org.ToDictionary(x => x.Key, x => new ProduktCecha(x.Value));

            var cechy = ApiWywolanie.PobierzCechy().Values.ToList();
            var cenyPoziomy = ApiWywolanie.PobierzPoziomyCen().Values.ToList();
            var kategorie = ApiWywolanie.PobierzKategorie();
            var produkty_Kategorie = ApiWywolanie.PobierzProduktyKategoriePolaczenia().Values.ToList();
            Dictionary<long, Klient> klienci = ApiWywolanie.PobierzKlientow();
            log.DebugFormat($"Klientów na B2B {klienci.Count}. Produktów na B2B: {produkty.Count}");
            IDictionary<int, KategoriaKlienta> kategorieKlientow = ApiWywolanie.PobierzKategorieKlientow();
            IDictionary<long, KlientKategoriaKlienta> klienciKategorie = ApiWywolanie.PobierzKlienciKategorie(new Dictionary<string, object>(0));
            IDictionary<long, KlientKategoriaKlienta> klienciKategorieB2B = klienciKategorie.ToDictionary(a => a.Key, a => a.Value);
            IDictionary<int, KategoriaKlienta> kategorieKlientowB2B = kategorieKlientow.ToDictionary(a => a.Key, a => a.Value);
            Dictionary<long, CenaPoziomu> cenybazowe = ApiWywolanie.PobierzPoziomyCenProduktow();
            //to musi tak byæ zrobione bo w impelu nie maj¹ ¿adnego providera ksiêgowego ale modu³ do rabatów musi byæ odpalony
            List<Rabat> rabaty = new List<Rabat>();
            if ( SyncManager.Konfiguracja.ProviderERP != ERPProviderzy.Brak)
            {
                log.Debug("Pobieranie rabatów z providera ksiegowego");
                rabaty = AktualnyProvider.PobierzRabaty(klienci, produkty, cenyPoziomy, cechy, cechy_Produkty_DoZmian.Values.ToList(), kategorie, produkty_Kategorie, kategorieKlientow, klienciKategorie);
                log.DebugFormat($"Koniec pobierania rabatów z providera ksiegowego, pobrana iloœæ: {rabaty.Count}");
            }
  
            List<ProduktUkryty> produktyUkryte = null;      //musi byc null
            Dictionary<long,Konfekcje> konfekcje = new Dictionary<long, Konfekcje>();
            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IModulRabaty).Przetworz(ref rabaty, ref produktyUkryte, ref konfekcje, klienci, produkty, cenyPoziomy, cechy, cechy_Produkty_DoZmian,
                    kategorie, produkty_Kategorie, ref kategorieKlientow, ref klienciKategorie);
                ZakonczModul(m);
            }

            SynchronizujKolekcje(kategorieKlientow.Values.ToList(), kategorieKlientowB2B, (a) => a.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujKategorieKlientow, ApiWywolanie.UsunKategorieKlientow);
            SynchronizujKolekcje(klienciKategorie.Values.ToList(), klienciKategorieB2B, SprawdzLacznKlienciKategorie, items => ApiWywolanie.AktualizujKlienciKategorie(items), ApiWywolanie.UsunKlienciKategorie);
            AktualizujProduktyKategorie(produkty_Kategorie, produkty);

            SynchronizacjaCech(cechy);
            AktualizujCechyProdukty(cechy_Produkty_DoZmian.Values.ToList(), produkty, cechyProduktyB2B_Org);
            if (produktyUkryte != null && produktyUkryte.Any())
            {
                ProduktyWyszukiwanie.PobierzInstancje.SprawdzProduktyUkryte(produktyUkryte, klienci, produkty, cechy.ToDictionary(x => x.Id, x => x), kategorie, kategorieKlientow);
            }
            AktualizujProduktyUkryte(produktyUkryte);
            aktualizujKonfekcje(konfekcje);
            log.DebugFormat($"Klientów na b2b przed czyszczeniem {klienci.Count}");
            var rpas = SprawdzIstnienieElementowRabatow(rabaty, produkty, kategorie, kategorieKlientowB2B, klienci, cenyPoziomy, cechy);
            log.DebugFormat($"Klientów na b2b po czyszczeniu {klienci.Count}");

            Dictionary<long, Rabat> rabatyErp = PoprawRabaty(rpas, cenybazowe);

            //czy sa nulle rabatów - nie moze byc ale czasem sa
            var rabatyNull = rabatyErp.Where(x => x.Value == null || x.Key == 0).Select(x => x.Key).ToList();
            if (rabatyNull.Any()){
                throw new Exception($"Obiekt rabatu jest NULL dla kliczy rabatów id: {rabatyNull.Join(",")} ");
            }

            //nullowanie wartosci rabatow w prograch jak sa takie same
            foreach (var r in rabatyErp)
            {
                //od tyl sprawdzanie
                if (r.Value.Wartosc5 == r.Value.Wartosc4)
                {
                    r.Value.Wartosc5 = null;
                }

                if (r.Value.Wartosc4 == r.Value.Wartosc3)
                {
                    r.Value.Wartosc4 = null;
                }

                if (r.Value.Wartosc3 == r.Value.Wartosc2)
                {
                    r.Value.Wartosc3 = null;
                }

                if (r.Value.Wartosc2 == r.Value.Wartosc1)
                {
                    r.Value.Wartosc2 = null;
                }
            }


            //walidator rabatów i walut klientów - bartek dodaje dlatego ze klienci czasem mieszaja klient ma walute EURO, ale rabat daja mu od ceny PLN
            //walidator nie dziala w wiekszosci przypadko dlatego ze waluta nie jest ustawiona na rabacie - trzeba dodac sprawdzenie wg. poziomu cenogowego i waluty
            var wynik = this.WalidujRabatyWgWalutyKlientow(rabatyErp.Values.ToList(), klienci.Values.ToList(), klienciKategorie.Values.ToList());
            if (wynik != null && wynik.Any())
            {
                foreach (var w in wynik)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo($"Dla klienta id: {w.Key.Id} [{w.Key.Email}] wykryto {w.Value.Count} rabatów w innej walucie ni¿ klienta. " +
                                                            $"System bêdzie próbowa³ przeliczyæ cene po kursie! {w.Value.Join(",")}");
                }
            }
            AktualizujRabaty(rabatyErp, rabatyNaB2B);
        }

        public Dictionary<Klient, List<long>> WalidujRabatyWgWalutyKlientow(List<Rabat> rabaty, List<Klient> klienci, List<KlientKategoriaKlienta> kategorieKlientowLaczniki)
        {
            if (rabaty == null || !rabaty.Any())
            {
                return null;
            }
            Dictionary<Klient, List<long> > wynik = new Dictionary<Klient, List<long>>();
            List<Rabat> rabatyZWalutami = rabaty.Where(x => x.WalutaId.HasValue).ToList();
            log.Debug($"Walidator rabatów - pomieszane waluty - start. Rabatów z walutami do sprawdzania: {rabatyZWalutami.Count}. Klientów do weryfikacji: {klienci.Count}.");
            if (!rabatyZWalutami.Any())
            {
                return null;
            }

            Parallel.ForEach(klienci, k =>
            {
                HashSet<int> kategorieKlienta = new HashSet<int>( kategorieKlientowLaczniki.Where(x => x.KlientId == k.Id).Select(x => x.KategoriaKlientaId) );
                List<long> wszystkieRabatyKlientaSkopane = rabatyZWalutami.Where(x => x.WalutaId.Value != k.WalutaId &&
                           ((x.KlientId == null && x.KategoriaKlientowId == null) || (x.KlientId.HasValue && x.KlientId.Value == k.Id) ||
                            (x.KategoriaKlientowId.HasValue && kategorieKlienta.Contains(x.KategoriaKlientowId.Value)))).Select(x => x.Id).ToList();

                if (wszystkieRabatyKlientaSkopane.Any())
                {
                    wynik.Add(k, wszystkieRabatyKlientaSkopane);
                }
            });
            return wynik;
        }

        private void AktualizujRabaty(IDictionary<long, Rabat> rabatyErp, Dictionary<long, Rabat> rabatyNaB2B)
        {
            HashSet<long> doAktualizacji = new HashSet<long>();
            HashSet<long> doUsuniecia = new HashSet<long>();
            HashSet<long> doDodania = new HashSet<long>();

            rabatyNaB2B.Porownaj(rabatyErp, ref doAktualizacji, ref doDodania, ref doUsuniecia,null, this);
            if (doUsuniecia.Count > 0)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Rabaty do usuniecia: {0}",doUsuniecia.Count);
                ApiWywolanie.UsunRabaty(doUsuniecia);
            }         
             
            if (doAktualizacji.Count > 0)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Rabaty aktualizacja: {0}", doAktualizacji.Count);
                List<Rabat> poziomyDoAktualizacji = rabatyErp.WhereKeyIsIn(doAktualizacji);
                ApiWywolanie.AktualizujRabaty(poziomyDoAktualizacji);
            }

            if (doDodania.Count > 0)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Rabaty dodawanie: {0}", doDodania.Count);
                List<Rabat> poziomyDoDodania = rabatyErp.WhereKeyIsIn(doDodania);
                ApiWywolanie.AktualizujRabaty(poziomyDoDodania);
            }
        }

        public static List<Rabat> SprawdzIstnienieElementowRabatow(List<Rabat> wejscie, IDictionary<long, Produkt> produkty,
            IDictionary<long, KategoriaProduktu> kategorie, IDictionary<int, KategoriaKlienta> katKlientow
            , Dictionary<long, Klient> klienci,  List< PoziomCenowy> poziomycen, List< Cecha> cechy)
        {
            List<Rabat> rabaty = new List<Rabat>(wejscie);

            HashSet<long> produktyB2B = new HashSet<long>( produkty.Keys );
            HashSet<long> klienciB2B = new HashSet<long>( klienci.Values.Where(x=>x.Aktywny).Select(x=>x.Id));//wysy³amy rabaty tylko dla aktywnych klientów 
            HashSet<int> kategorieklientow = new HashSet<int>( katKlientow.Keys );
            HashSet<long> kategorieproduktow = new HashSet<long>(kategorie.Keys);
            HashSet<int> poziomy = new HashSet<int>(poziomycen.Select(x => x.Id));
            HashSet<long> cechid = new HashSet<long>(cechy.Select(x => x.Id));
            int przed = rabaty.Count;

            LogiFormatki.PobierzInstancje.LogujInfo("Przetwarzanie {0} rabatów", przed);

            rabaty.RemoveAll(x => x.ProduktId.HasValue && !produktyB2B.Contains(x.ProduktId.Value));
            int po = rabaty.Count;
            if (przed != po)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Pominiêto {0} rabatów, przypisane im produkty nie istniej¹ na b2b", przed - po);
            }
            przed = rabaty.Count;
            rabaty.RemoveAll(x => x.KlientId.HasValue && !klienciB2B.Contains(x.KlientId.Value));
            po = rabaty.Count;
            if (przed != po)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Pominiêto {0} rabatów, przypisani im klienci nie istniej¹ na b2b", przed - po);
            }
            przed = rabaty.Count;
            rabaty.RemoveAll(x => x.KategoriaKlientowId.HasValue && !kategorieklientow.Contains(x.KategoriaKlientowId.Value));
            po = rabaty.Count;
            if (przed != po)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Pominiêto {0} rabatów, przypisane im katergorie klientów nie istniej¹ na b2b", przed - po);
            }
            przed = rabaty.Count;
            rabaty.RemoveAll(x => x.KategoriaProduktowId.HasValue && !kategorieproduktow.Contains(x.KategoriaProduktowId.Value));
            po = rabaty.Count;
            if (przed != po)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Pominiêto {0} rabatów, przypisane im kategorie produktów nie istniej¹ na b2b. UPEWNIJ SIÊ ¯E ISTNIEJE GRUPA KATEGORII ZBUDOWANA NA PODSTAWIE KATEGORI Z ERP", przed - po);
            }
            przed = rabaty.Count;
            rabaty.RemoveAll(x => !x.ProduktId.HasValue && x.TypWartosci == RabatSposob.StalaCena);
            po = rabaty.Count;
            if (przed != po)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Pominiêto {0} rabatów, rabaty typu stala cena, ale bez ustawionego produktu", przed - po);
            }
            przed = rabaty.Count;
            rabaty.RemoveAll(x => x.DoKiedy.HasValue && x.DoKiedy < DateTime.Now);
            po = rabaty.Count;
            if (przed != po)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Pominiêto {0} rabatów, rabaty maj¹ datê zakoñczenie wczeœniejsz¹ ni¿ obena chwila", przed - po);
            }
            przed = rabaty.Count;
            rabaty.RemoveAll(x => x.PoziomCenyId.HasValue && !poziomy.Contains(x.PoziomCenyId.Value));
            po = rabaty.Count;
            if (przed != po)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Pominiêto {0} rabatów, rabaty maj¹  poziom cen nie istnij¹cy na b2b", przed - po);
            }
            przed = rabaty.Count;
            rabaty.RemoveAll(x => x.CechaId.HasValue && !cechid.Contains(x.CechaId.Value));
            po = rabaty.Count;
            if (przed != po)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Pominiêto {0} rabatów, rabaty przypisabe do cech nie istnij¹cy na b2b", przed - po);
            }

            LogiFormatki.PobierzInstancje.LogujInfo("Koniec wyliczania rabatów - iloœæ do dalszego przetwarzania {0}", rabaty.Count);

            return rabaty;
        }

        /// <summary>
        /// metoda wyszukuje rabaty dzialajce na te same produkty / klientów - i wybiera ten lepszy
        /// </summary>
        /// <param name="rabaty"></param>
        /// <param name="cenybazowe"></param>
        /// <returns></returns>
        public Dictionary<long, Rabat> PoprawRabaty(List<Rabat> rabaty, Dictionary<long, CenaPoziomu> cenybazowe)
        {
            var unikalneRabaty = rabaty.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.ToList());
            Dictionary<long, Rabat> wynik = new Dictionary<long, Rabat>(unikalneRabaty.Count);

            foreach(var r in unikalneRabaty)
            {
                Rabat najlepszy = r.Value.First();

                if (r.Value.Count > 1)
                {
                    //szukamy najlepszego rabatu - albo po cenach - jak sa poziomy cenowe, albo po rabatach
                    //wystarczy ze sprawdzimy dla jednego rabatu, bo i tak mamy tu te same rabaty po ID - wiec maja takie same warunki, tylko inne wartoœci
                    CenaPoziomu cenaNajlepszego = null;
                    if (najlepszy.ProduktId.HasValue && najlepszy.PoziomCenyId.HasValue && cenybazowe.TryGetValue(CenaPoziomu.wyliczKlucz(najlepszy.ProduktId.Value, najlepszy.PoziomCenyId.Value), out cenaNajlepszego))
                    {

                        decimal? cenaPoRabacieNajlepszego = najlepszy.Wartosc1.GetValueOrDefault(1)*cenaNajlepszego.Netto;
                        foreach (Rabat rabat in r.Value)
                        {
                            CenaPoziomu cena;
                            //czy jest poziom cenowy i jest INNY ni¿ ten z najlepszego rabatu
                            if (cenaPoRabacieNajlepszego.HasValue && rabat.PoziomCenyId.HasValue && najlepszy.PoziomCenyId.HasValue && rabat.PoziomCenyId.Value != najlepszy.PoziomCenyId.Value &&
                                cenybazowe.TryGetValue(CenaPoziomu.wyliczKlucz(najlepszy.ProduktId.Value, rabat.PoziomCenyId.Value), out cena))
                            {
                                decimal porabacie = cena.Netto*rabat.Wartosc1.GetValueOrDefault(1);
                                //jest poziom cenowy, ale inny niz najlepszego

                                if (cenaPoRabacieNajlepszego > porabacie)
                                {
                                    //zmiana
                                    najlepszy = rabat;
                                    cenaPoRabacieNajlepszego = porabacie;
                                    continue;
                                }
                            }
                            else
                            {
                                //nie ma poziomu w ogole, albo sa takie same - sprawdzenie po rabacie tylko
                                if (najlepszy.Wartosc1.GetValueOrDefault(1) < rabat.Wartosc1.GetValueOrDefault(1))
                                {
                                    //zmiana
                                    najlepszy = rabat;
                                    cenaPoRabacieNajlepszego = null;
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Rabat rabat in r.Value)
                        {
                            if (najlepszy.Wartosc1.GetValueOrDefault(1) < rabat.Wartosc1.GetValueOrDefault(1))
                            {
                                //zmiana
                                najlepszy = rabat;
                            }
                        }
                    }
                }

                if (najlepszy == null || najlepszy.Id == 0)
                {
                    throw new Exception("Pusty rabat!");
                }

                //tylko najlepsze dodajemy do kolekcji
                wynik.Add(najlepszy.Id, najlepszy);
            }

            LogiFormatki.PobierzInstancje.LogujInfo($"Koniec poprawiania rabatów - zwrócono ostatecznie: {wynik.Count} rabatów.");
            return wynik;
        }

        private void ImportEksport(IEnumerable<SyncModul> moduly)
        {
            foreach (SyncModul m in moduly)
            {
              UruchomModul(m);
                (m as IModulEksportImportXMLCSV).Przetworz();
                ZakonczModul(m);
            }
        }

        // task 9965
        //private void AktualizujProduktyMale(IEnumerable<SyncModul> moduly)
        //{
        //    Dictionary<long, Produkt> produktyNaB2B = ApiWywolanie.PobierzProdukty().Where( x=> x.Value.Widoczny).ToDictionary(x=>x.Key,x=>x.Value);

        //    List<Produkt> produktyDoZmiany = produktyNaB2B.Values.ToList().CloneList().ToList();
        //    //SlownikiSearchCriteria slownikiCriteria = new SlownikiSearchCriteria();
        //    //slownikiCriteria.typ_id.Add( SyncManager.Konfiguracja.GetSystemTypeId(typeof(produkty)));
        //    //slownikiCriteria.AddtionalSQL = " obiekt_id>0 ";
        //    //List<slowniki> slownikilista = ApiWywolanie.GetSlowniki(slownikiCriteria).Values.ToList();
        //    List<Tlumaczenie> slownikilista = ApiWywolanie.GetSlowniki().Values.Where(x => x.Typ == typeof (Produkt).PobierzOpisTypu()).ToList();
        //    foreach (SyncModul m in moduly)
        //    {
        //        UruchomModul(m);
        //        (m as IAktualizacjaProduktow).Przetworz(ref produktyDoZmiany, ref slownikilista);
        //        ZakonczModul(m);
        //    }      

        //    Dictionary<long, Produkt> produktyErp = produktyDoZmiany.ToDictionary(x => x.Id, x => x);
        //    HashSet<long> doAktualizacji = new HashSet<long>();
        //    HashSet<long> doUsuniecia = new HashSet<long>();
        //    HashSet<long> doDodania = new HashSet<long>();
        //    Produkt pomijanie = new Produkt();
        //    produktyNaB2B.Porownaj(produktyErp, ref doAktualizacji, ref doDodania, ref doUsuniecia, new { pomijanie.DataDodania, pomijanie.WyslanoMailNowyProdukt }, this);
        //    if (doAktualizacji.Count > 0)
        //    {
        //        List<Produkt> poziomyDoAktualizacji = produktyErp.WhereKeyIsIn(doAktualizacji);
        //        ApiWywolanie.AktualizujProdukty(poziomyDoAktualizacji);
        //    }

        //    AktualizujSlowniki(typeof(Produkt), slownikilista);
        //}


        private void AktualizujZdjecia(IEnumerable<SyncModul> moduly)
        {
            Dictionary<long, Produkt> produktyNaB2B = ApiWywolanie.PobierzProdukty().Where(x => x.Value.Widoczny).ToDictionary(x => x.Key, x => x.Value); 
            List<Cecha> cechyB2B = ApiWywolanie.PobierzCechy().Values.ToList();
            List<KategoriaProduktu> kategorieB2B = ApiWywolanie.PobierzKategorie().Values.ToList();
            List<Plik> plikiNaB2B = ApiWywolanie.PlikNaB2BPobierz();
            LogiFormatki.PobierzInstancje.LogujInfo("ilosc plikow: {0}", plikiNaB2B.Count);
            List<Plik> lokalnePLiki = new List<Plik>(plikiNaB2B.Count);
            Dictionary<long, Klient> klienciB2Bdict = ApiWywolanie.PobierzKlientow().Values.Where(x=>x.Aktywny).ToDictionary(x=>x.Id,x=>x);
            List<Klient> klienciB2B = klienciB2Bdict.Values.Where(a => a.Id > 0).Select(a => new Klient(a)).ToList();
            List<ProduktPlik> lokalnePowiazania = new List<ProduktPlik>(plikiNaB2B.Count);
            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IModulPliki).Przetworz(produktyNaB2B, ref lokalnePowiazania, ref lokalnePLiki, AktualnyProvider, ref cechyB2B, ref kategorieB2B, ref klienciB2B);
                ZakonczModul(m);
            }

            List<Plik> tempPlikiDoWyslania = new List<Plik>();
            //dopasowywanie plikow
            foreach (Plik p in lokalnePLiki)
            {
                Plik plikB2B = plikiNaB2B.FirstOrDefault(a => a.CzyTeSamePliki(p));
                if (plikB2B == null)
                {
                    tempPlikiDoWyslania.Add(p);
                }
                else
                {
                    foreach (var po in lokalnePowiazania)
                    {
                        if (po.PlikId == p.Id)
                        {
                            po.PlikId = plikB2B.Id;
                        }
                    }
                    foreach (var po in cechyB2B)
                    {
                        if (po.ObrazekId == p.Id)
                        {
                            log.DebugFormat("Podmiana pliku dla cechy:{2} z:{0}, na {1}", po.Id, plikB2B.Id, po.Id);
                            po.ObrazekId = plikB2B.Id;
                        }
                    }
                    foreach (var po in kategorieB2B)
                    {
                        if (po.ObrazekId == p.Id)
                        {
                            po.ObrazekId = plikB2B.Id;
                        }
                    }
                    foreach (var po in klienciB2B)
                    {
                        if (po.ZdjecieId == p.Id)
                        {
                            po.ZdjecieId = plikB2B.Id;
                        }
                    }
                    p.Id = plikB2B.Id;
                    plikiNaB2B.Remove(plikB2B);
                }
            }

            if (plikiNaB2B.Count > 0)
            {
                List<int> temnp = plikiNaB2B.Where(a => a.Id > 0 && a.RodzajPliku!=RodzajPliku.PlikDoPrzeniesieniaJedenNaJedenOdKlienta).Select(a => a.Id).ToList(); //tu musi wywalaæ tylko te których ID > 0 bo te z ujemnymi s¹ np do flag przez co wczeœniej flagi znika³y
                LogiFormatki.PobierzInstancje.LogujInfo("plików do usunêcia " + temnp.Count);
                ApiWywolanie.PlikNaB2BUsun(temnp);
            }

            LogiFormatki.PobierzInstancje.LogujInfo("Plików do wys³ania ca³kowita iloœæ: " + tempPlikiDoWyslania.Count);

            if (tempPlikiDoWyslania.Count > 0)
            {
                List<Plik> nowaPaczkaZSerwera = ApiWywolanie.PlikiNaB2BDodajPaczkowanie(tempPlikiDoWyslania, s =>  LogiFormatki.PobierzInstancje.LogujInfo(s) ) ;

                foreach (Plik p in nowaPaczkaZSerwera)
                {
                    //log.DebugFormat("szukanie lacznika dla plik id: {0}, œcie¿ka lokalna: {1} ",p.Id, p.PelnaSciezkaLokalna);
                    var lokalny = lokalnePLiki.First(a => a.CzyTeSamePliki(p));
                    foreach (var po in lokalnePowiazania)
                    {
                        if (po.PlikId == lokalny.Id)
                        {
                            po.PlikId = p.Id;
                        }
                    }
                    foreach (var po in cechyB2B)
                    {
                        if (po.ObrazekId == lokalny.Id)
                        {
                            po.ObrazekId = p.Id;
                        }
                    }
                    foreach (var po in kategorieB2B)
                    {
                        if (po.ObrazekId == lokalny.Id)
                        {
                            po.ObrazekId = p.Id;
                        }
                    }
                    foreach (var po in klienciB2B)
                    {
                        if (po.ZdjecieId == lokalny.Id)
                        {
                            po.ZdjecieId = p.Id;
                        }
                    }
                    lokalny.Id = p.Id;
                    p.DanePlikBase64 = null;
                }
            }

        
            AktualizujPlikiProdukty(lokalnePowiazania);
            SynchronizacjaCech(cechyB2B);
            SynchronizacjaKategorii(kategorieB2B);
            SynchronizujKlientow(klienciB2B.ToDictionary(x => x.Id, x => x), klienciB2Bdict);
        }

        private void AktualizujPlikiProdukty(List<ProduktPlik> lokalnePowiazania)
        {
            List<long> produkty = ApiWywolanie.PobierzProduktyId();
            List<Plik> plikiNaB2B = ApiWywolanie.PlikNaB2BPobierz();
            lokalnePowiazania.RemoveAll(x => !produkty.Contains(x.ProduktId));
            lokalnePowiazania.RemoveAll(x => !plikiNaB2B.Select(z => z.Id).Contains(x.PlikId));
            var valspp = ApiWywolanie.PlikiProduktowPobierz().Values.ToList();
            valspp.RemoveAll(x => x.ProduktId < 0);
            Dictionary<long, ProduktPlik> plikiNaB2BPowiazania = valspp.ToDictionary(x => x.Id, x => x);
            //int id = -1;
            //foreach (ProduktPlik lokal in lokalnePowiazania)
            //{
            //    ProduktPlik powiazanieB2B = plikiNaB2BPowiazania.Values.FirstOrDefault(a => a.ProduktId == lokal.ProduktId && a.PlikId == lokal.PlikId && a.Glowny == lokal.Glowny);
            //    //lokal.Id = powiazanieB2B != null ? powiazanieB2B.Id : id--;
            //}

            Dictionary<long, ProduktPlik> produktyPlikierp = lokalnePowiazania.ToDictionary(x => x.Id, x => x);
            HashSet<long> doAktualizacji = new HashSet<long>();
            HashSet<long> doUsuniecia = new HashSet<long>();
            HashSet<long> doDodania = new HashSet<long>();
            plikiNaB2BPowiazania.Porownaj(produktyPlikierp, ref doAktualizacji, ref doDodania, ref doUsuniecia, null, this);
            if (doUsuniecia.Count > 0)
            {
                ApiWywolanie.PlikProduktowUsun(doUsuniecia);
            }
            if (doDodania.Count > 0)
            {
                List<ProduktPlik> poziomyDoDodania = produktyPlikierp.WhereKeyIsIn(doDodania);
                ApiWywolanie.PlikProduktowDodaj(poziomyDoDodania);
            }
            if (doAktualizacji.Count > 0)
            {
                List<ProduktPlik> poziomyDoAktualizacji = produktyPlikierp.WhereKeyIsIn(doAktualizacji);
                ApiWywolanie.PlikProduktowDodaj(poziomyDoAktualizacji);
            }
        }

        private void AktualizujKlientow(List<SyncModul> moduly)
        {
            List<Kraje> kraje = AktualnyProvider.PobierzKraje();
            List<Magazyn>magazyny = null;
            List<Model.Region> regiony = AktualnyProvider.PobierzRegiony();
            LogiFormatki.PobierzInstancje.LogujDebug($"Iloœæ krajów {kraje.Count} iloœæ regionów {regiony.Count}");
            Dictionary<Adres, KlientAdres> adresyWErp;
            LogiFormatki.PobierzInstancje.LogujDebug("Pobieranie klienci  pocz¹tek");
            Dictionary<long, Klient> klienciNaPlatfomie = ApiWywolanie.PobierzKlientow();
            LogiFormatki.PobierzInstancje.LogujInfo("Ilosc klientow na platformie: {0}",klienciNaPlatfomie.Count);
            LogiFormatki.PobierzInstancje.LogujDebug("Pobieranie klienci  koniec");
            LogiFormatki.PobierzInstancje.LogujDebug("Klienci erp pocz¹tek");
            Dictionary<long, Klient> klienci = AktualnyProvider.PobierzKlientow(klienciNaPlatfomie.Values.ToList(), out adresyWErp);
            LogiFormatki.PobierzInstancje.LogujDebug("Klienci erp koniec");
            Dictionary<long, Produkt> produkty = ApiWywolanie.PobierzProdukty();
            List<KategoriaKlienta> kategorieklientow = ApiWywolanie.PobierzKategorieKlientow().Values.ToList();
            List<KlientKategoriaKlienta> kliencikategorielaczniki = ApiWywolanie.PobierzKlienciKategorie(new Dictionary<string, object>()).Values.ToList();//AktualnyProvider.PobierzKategorieKlientowPolaczenia();
            List<Sklep> sklepy = new List<Sklep>();
            List<SklepKategoriaSklepu> sklepylaczniki = new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepykategorie = new List<KategoriaSklepu>();

            List<Klient> pracownicy = klienciNaPlatfomie.Values.ToList(); //lista klientów na platformie oznaczona jako pracownicy

            //Dezaktywacja klientów je¿eli nie maj¹ wpisanego poziomu cenowego a poziom ceny hurtowej jest nie wybrany
            var poziomHurtowy = SyncManager.Konfiguracja.GetPriceLevelHurt;
            if (poziomHurtowy == 0)
            {
                HashSet<long> listaKlientowBezPoziomuCenowego = new HashSet<long>( klienci.Values.Where(x => x.PoziomCenowyId == 0 && x.Aktywny).Select(x=>x.Id) );
                foreach (var klient in listaKlientowBezPoziomuCenowego)
                {
                    klienci[klient].Aktywny = false;
                    LogiFormatki.PobierzInstancje.LogujError(new Exception($"Klient o emailu: {klienci[klient].Email} zosta³ zdezaktywowany poniewa¿ nie posiada³ wpisanego poziomu cenowego"));
                }
            }

            SprawdzenieKredytuKlientow(klienci);

            //BARTEK - przygotowanie do wielopracownikowatosci
            //to nizej sluzy do przypisania opiekunow, przedstawicieli itp w kliencie. Providery ERP umieszczaja informacje o pracownikach w slowniku PrzedstawicieleOpiekunowie w kliencie
            //providery NIE MOGA uzupelniac pracownika bezposrednio w sobie (opiekuna, przedstawicial, itp.) - tu sie ma to dziac.
            
            //test czy z providera dostalismy poprawnych klientów - NIE uzupelnionych o pracownikow
            Parallel.ForEach(klienci.Values, k =>
            {
                if (k.OpiekunId.HasValue || k.PrzedstawicielId.HasValue || k.DrugiOpiekunId.HasValue || k.KlientNadrzednyId.HasValue)
                {
                    throw new Exception($"Provider wykona³ niedozowolon¹ operacjê - przypisa³ klientowi id: {k.Id} pracownika. ERP nie mo¿e tego zrobiæ na tym etapie.");
                }
            });

            foreach (var klienterp in klienci)
            {
                foreach (var przedstawcieleopiekunowie in klienterp.Value.PrzedstawicieleOpiekunowie)
                {
                    if (!string.IsNullOrEmpty(przedstawcieleopiekunowie.Value))
                    {
                        int id;
                        Klient pracownikNaB2B;
                        if (int.TryParse(przedstawcieleopiekunowie.Value, out id))
                        {
                            pracownikNaB2B = pracownicy.FirstOrDefault(p => (p.Symbol == przedstawcieleopiekunowie.Value) || (p.Id == id));
                        }
                        else
                        {
                            pracownikNaB2B = pracownicy.FirstOrDefault(p => p.Symbol == przedstawcieleopiekunowie.Value);
                        }
                        if (pracownikNaB2B != null)
                        {
                            switch (przedstawcieleopiekunowie.Key)
                            {
                                case OpiekunowiePrzedstawiciele.Opiekun:
                                {
                                    klienterp.Value.OpiekunId = pracownikNaB2B.Id;
                                }
                                    break;
                                case OpiekunowiePrzedstawiciele.DrugiOpiekun:
                                {
                                    klienterp.Value.DrugiOpiekunId = pracownikNaB2B.Id;
                                }
                                    break;
                                case OpiekunowiePrzedstawiciele.Przedstawiciel:
                                {
                                    klienterp.Value.PrzedstawicielId = pracownikNaB2B.Id;
                                }
                                    break;
                                case OpiekunowiePrzedstawiciele.KlientNadrzedny:
                                {
                                    klienterp.Value.KlientNadrzednyId = pracownikNaB2B.Id;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            PrzesunModulNaKoniec(moduly, typeof(Modules_.NoweModuly.Klienci.KtorePolaSynchronizowac));
            PrzesunModulNaKoniec(moduly, typeof(KtorePolaSynchronizowacRegiony));
            PrzesunModulNaKoniec(moduly, typeof(KtorePolaSynchronizowacKraje));
            PrzesunModulNaKoniec(moduly, typeof(KtorePolaSynchronizowacMagazyny));

            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IModulKlienci).Przetworz(ref klienci, produkty, ref adresyWErp, kategorieklientow, kliencikategorielaczniki, ref sklepy, ref sklepylaczniki, ref sklepykategorie, ref kraje, ref regiony, ref magazyny, AktualnyProvider);
                ZakonczModul(m);
            }

            List<Klient> erpkl = klienci.Values.ToList();
            Dictionary<long, Klient> klienciErp = SprawdzKlientow(erpkl, klienciNaPlatfomie);

            if (klienciErp?.Values == null || !klienciErp.Values.Any())
            {
                throw new Exception("Brak klientów z ERP");
            }

            klienciErp.Values.OperacjeNaPolachTekstowych();
            Parallel.ForEach(klienciErp.Values, klient =>
            {
                List<string> zaDlugieWartosci = Tools.SprawdzIloscZnakow(klient);
                if (zaDlugieWartosci.Any())
                {
                    LogiFormatki.PobierzInstancje.LogujInfo("Ucieto ilosc znakow dla pol: {0} dla klienta o id: {1}, nazwa: {2}", zaDlugieWartosci.Join(","), klient.Id, klient.Nazwa);
                }
            });

            Dictionary<long, Adres> adresy = ApiWywolanie.PobierzAdresy();
            
            SynchronizujKlientow(klienciErp, klienciNaPlatfomie);
            AktualizujKraje(kraje, adresy);
            AktualizujRegiony(regiony);

            var wszyscyKlienci= ApiWywolanie.PobierzKlientow();

            AktualizujAdresy(adresyWErp, wszyscyKlienci, adresy);
            AktualizujLacznikiAdresow(adresyWErp, wszyscyKlienci);
            AktualizujSklepy(sklepy);
            AktualizujKategorieSklepow(sklepykategorie);
            AktualizujSklepyKategorie(sklepylaczniki);
            AktualizujMagazyny(magazyny);
        }

        private void SprawdzenieKredytuKlientow(Dictionary<long, Klient> klienci)
        {
            foreach (Klient klient in klienci.Values)
            {
                if (klient.LimitKredytu == 0)
                {
                    continue;
                }
                if ((klient.IloscPozostalegoKredytu == 0 && klient.IloscWykorzystanegoKredytu == 0) ||
                    (klient.IloscPozostalegoKredytu != 0 && klient.IloscWykorzystanegoKredytu != 0))
                {
                    continue;
                }
                if (klient.IloscPozostalegoKredytu == 0)
                {
                    klient.IloscPozostalegoKredytu = klient.LimitKredytu - klient.IloscWykorzystanegoKredytu;
                }
                else
                {
                    klient.IloscWykorzystanegoKredytu = klient.LimitKredytu - klient.IloscPozostalegoKredytu;
                }
            }
        }
        public static void PrzesunModulNaKoniec<T>(List<T> moduly, Type type)
        {
           
            List<T> pasujace = moduly.Where(x => x.GetType() == type).ToList();
            if(!pasujace.Any())
            {
                throw new InvalidOperationException("Brak modu³u " + type.FullName);
            }
            moduly.RemoveAll(x => x.GetType() == type);
            foreach (T m in pasujace)
            {
                moduly.Add(m);
            }
        }
        private void AktualizujMagazyny(List<Magazyn> magazyny)
        {
            if (magazyny == null)
            {
                return;
            }
            var istniejace = ApiWywolanie.PobierzMagazyny().Where(x=>x.Id>0).ToDictionary(x=>x.Id,x=>x);
            SynchronizujKolekcje(magazyny, istniejace, (data) => data.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujMagazyny, elementyDoUsunieciaMagazyny => ApiWywolanie.UsunMagazyny(elementyDoUsunieciaMagazyny));

        }

        private void AktualizujSklepyKategorie(List<SklepKategoriaSklepu> sklepylaczniki)
        {
            Dictionary<long, SklepKategoriaSklepu> istniejace =
                ApiWywolanie.PobierzSklepyKategorieLaczniki()
                    .Where(x => !x.RecznieDodany())
                    .ToDictionary(x => x.Id, x => x);

            SynchronizujKolekcje(sklepylaczniki, istniejace, (data) => data.ToDictionary(x => x.Id, x => x),
                ApiWywolanie.AktualizujSklepyKategorieLaczniki,
                elementyDoUsunieciaAdresy => ApiWywolanie.UsunSklepyKategorieLaczniki(elementyDoUsunieciaAdresy));

        }

        private void AktualizujKategorieSklepow(List<KategoriaSklepu> sklepykategorie)
        {
           Dictionary<long, KategoriaSklepu> istniejace = ApiWywolanie.PobierzSklepyKategorie().Values.Where(x=>x.Id>0).ToDictionary(x=>x.Id,x=>x);
            SynchronizujKolekcje(sklepykategorie, istniejace, (data) => data.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujSklepyKategorie, ApiWywolanie.UsunSklepyKategorie);
        }

        private void AktualizujSklepy(List<Sklep> sklepy)
        {
            Dictionary<long, Adres> adresy = ApiWywolanie.PobierzAdresy();
            Dictionary<long, Sklep> istniejace = ApiWywolanie.PobierzSklepy().Values.Where(x=>x.Id>0).ToDictionary(x=>x.Id,x=>x);
            Dictionary<long, Adres> adresyDoAktualizacji = new Dictionary<long, Adres>();
            foreach (Sklep s in sklepy)
            {
                if (istniejace.ContainsKey(s.Id) && s.AdresId != null && istniejace[s.Id].AdresId != null)
                {
                    //log.DebugFormat("nazwa sklepu:{1}, adres sklepu:{0}",s.AdresId,s.Nazwa);
                    Adres adres = adresy[(int) s.AdresId];
                    Adres adresIs = adresy[(int) istniejace[s.Id].AdresId];
                    if (!s.KoordynatyZERP)
                    {
                        s.AutomatyczneKoordynaty = istniejace[s.Id].AutomatyczneKoordynaty;
                        s.DataUtworzenia = istniejace[s.Id].DataUtworzenia;
                        adres.Lat = adresIs.Lat;
                        adres.Lon = adresIs.Lon;
                    }

                    if (s.AutomatyczneKoordynaty && !adres.CzyPoprawneKoordynaty)
                        //mamy pobieraæ koordynaty automatycznie, a w danych na serwerze nie ma ich poprawnych 
                    {
                        adres.Lat = adres.Lat == 0 ? -1 : 0;
                        adres.Lon = adres.Lon == 0 ? -1 : 0;
                        LogiFormatki.PobierzInstancje.LogujDebug($"sklep {s.Id} lat {adres.Lat} lon {adres.Lon}");
                    }
                    if (adresyDoAktualizacji.ContainsKey(adres.Id))
                    {
                        adresyDoAktualizacji.Add(adres.Id, adres);
                    }
                }
                else
                {
                    if(!adresy.ContainsKey(s.AdresId.Value)) s.AdresId = null;
                    log.DebugFormat("{2} nazwa sklepu:{1}, adres sklepu:{0}", s.AdresId, s.Nazwa, s.Id);
                }
            }
            ApiWywolanie.AktualizujAdresy(adresyDoAktualizacji.Values.ToList());
            log.Info($"Ilosc sklepów do wys³ania {sklepy.Count}, ilosc adresów do aktualizacji {adresyDoAktualizacji.Count}");
            SynchronizujKolekcje(sklepy, istniejace, (data) => data.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujSklepy, ApiWywolanie.UsunSklepy);
        }

        private Dictionary<long, Klient> SprawdzKlientow(List<Klient> klienci, Dictionary<long, Klient> klienciNaPlatfomie)
        {
            Dictionary<int, PoziomCenowy> poziomyCenowe = ApiWywolanie.PobierzPoziomyCen();
            ConcurrentDictionary<long, Klient> klienciErp = new ConcurrentDictionary<long, Klient>();
            Dictionary<long, Waluta> walutyNaB2B = PobierzWaluty();

            ConcurrentDictionary<string,List<Klient>> pomijaniKlienci = new ConcurrentDictionary<string, List<Klient>>();

            Parallel.ForEach(klienci, c =>
            {
                if (string.IsNullOrEmpty(c.Email))
                {
                    log.Error($"Klient o symbolu : {c.Symbol} nie posiada uzupe³nionego adresu email - zostaje pominiêty");
                    return;
                }
                if (klienciErp.ContainsKey(c.Id))
                {
                    log.InfoFormat($"Klient: {c.Nazwa}[{c.Id}], jest ju¿ w s³owniku.");
                    return;
                }

                if (!c.Email.PoprawnyAdresEmail())
                {
                    pomijaniKlienci.AddOrUpdate("niepoprawny adres email", new List<Klient> { c }, (s, list) => { list.Add(c); return list; });
                    c.Opis = $"deaktywowany automatycznie solex b2b - niepoprawny adres email: {c.Email}";
                    c.Aktywny = false;  //deaktywacja autoamtyczna
                }

                c.Email = c.Email.Trim().ToLower();
                c.Nazwa = c.Nazwa.Trim();
                c.Symbol = c.Symbol.Trim();

                if (string.IsNullOrEmpty(c.Login))
                {
                    c.Login = c.Email;
                }

                if (string.IsNullOrEmpty(c.Login))
                {
                    pomijaniKlienci.AddOrUpdate("brak loginu", new List<Klient> { c}, (s, list) => { list.Add(c); return list; });
                    c.Opis = "deaktywowany automatycznie solex b2b - brak loginu";
                    c.Aktywny = false;  //deaktywacja autoamtyczna
                }

                if (!c.Aktywny)
                {
                    c.Email = c.Login = null;   //kasowanie maila dla niekatywnych
                    if (string.IsNullOrEmpty(c.Opis))
                    {
                        c.Opis = $"deaktywowany automatycznie solex b2b - nieznane powody. Email pierwotny klienta: {c.Email}";
                    }
                }

                if (c.Aktywny)
                {
                    int klientowZtakimSamymMailem = klienci.Count(a => !string.IsNullOrEmpty(a.Email) && a.Email.Equals(c.Email, StringComparison.InvariantCultureIgnoreCase));
                    if (klientowZtakimSamymMailem > 1)
                    {
                        pomijaniKlienci.AddOrUpdate("zdublowane maile w ERP", new List<Klient> { c }, (s, list) => { list.Add(c); return list; });
                        c.Opis = $"deaktywowany automatycznie solex b2b - zdublowane maile ERP: {c.Email}";
                        c.Aktywny = false;  //deaktywacja autoamtyczna
                    }
                    int klientowZtakimSamymMailemb2B = klienciNaPlatfomie.Count(a => !string.IsNullOrEmpty(a.Value.Email) && a.Value.Email.Equals(c.Email, StringComparison.InvariantCultureIgnoreCase) && a.Value.Id != c.Id);
                    if (klientowZtakimSamymMailemb2B > 0)
                    {
                        pomijaniKlienci.AddOrUpdate("zdublowane maile w B2B (za³o¿eni rêcznie? Subkonta?)", new List<Klient> { c }, (s, list) => { list.Add(c); return list; });
                        c.Opis = $"deaktywowany automatycznie solex b2b - zdublowane maile B2B: {c.Email}";
                        c.Aktywny = false;  //deaktywacja autoamtyczna
                    }
                }

                if(!string.IsNullOrEmpty(c.HasloZrodlowe) && c.HasloZrodlowe.Length!=32)
                {
                    c.HasloZrodlowe = Tools.PobierzInstancje.GetMd5Hash(c.HasloZrodlowe);
                }
                ResetKlientaNadrzednego(c, klienciNaPlatfomie.Values.ToList());

                if (c.Aktywny && c.JezykId == 0)
                {
                    if (SyncManager.Konfiguracja.JezykiWSystemie.Count > 1)
                    {
                        string rozszerzenieMaila = c.Email.Substring(c.Email.LastIndexOf('.') + 1).ToLower().Trim();
                        //jesli email nie jest PL a jezyk jest EN to autoamtycznie domyslnie dajemy EN
                        if (rozszerzenieMaila != "pl" &&
                            SyncManager.Konfiguracja.JezykiWSystemieSlownikPoSymbolu.ContainsKey("en"))
                        {
                            c.JezykId = SyncManager.Konfiguracja.JezykiWSystemieSlownikPoSymbolu["en"].Id;
                        }
                        Jezyk temp;
                        if (SyncManager.Konfiguracja.JezykiWSystemieSlownikPoSymbolu.TryGetValue(rozszerzenieMaila,
                            out temp))
                        {
                            c.JezykId = temp.Id;
                        }
                    }
                    else
                    {
                        c.JezykId = SyncManager.Konfiguracja.JezykIDDomyslny;
                    }
                }

                if (c.Aktywny && c.WalutaId != 0 && !walutyNaB2B.ContainsKey(c.WalutaId))
                {
                    LogiFormatki.PobierzInstancje.LogujInfo(
                        $"Deaktywowanie klienta id: {c.Id}, email: {c.Email}, symbol: {c.Symbol} dlatego ¿e posiada walute której nie ma w B2B (klucz brakuj¹cej waluty: {c.WalutaId})");
                    c.Aktywny = false;
                    c.WalutaId = 0;
                    c.Email = c.Login = null;
                    c.Opis = $"deaktywowany automatycznie solex b2b - z³a waluta.";
                }

                //notorycznie wszyzscy wpisywali tu spacje w magazyny symboel - dlatego to sprawdzenie
                if (c.DostepneMagazyny != null)
                {
                    c.DostepneMagazyny = new HashSet<string>( c.DostepneMagazyny.Select(x => x.Trim()) );
                }

                if (c.Aktywny && c.WalutaId==0 && c.PoziomCenowyId.HasValue)
                {
                    PoziomCenowy poziom;
                    if (poziomyCenowe.TryGetValue(c.PoziomCenowyId.Value, out poziom))
                    {
                        if (!poziom.WalutaId.HasValue)
                        {
                            throw new Exception($"Poziom powinien mieæ walute ID wpisan¹, ale nie ma. Poziom id: {poziom.Id}");
                        }
                        c.WalutaId = poziom.WalutaId.Value;
                    }
                    else
                    {
                        throw new Exception($"Brak poziomu cenowego o Id:{c.PoziomCenowyId.Value}");
                    }
                }
                             
                klienciErp.TryAdd(c.Id, c);
            });

            if (pomijaniKlienci.Any())
            {
                StringBuilder komunikat = new StringBuilder( "B³êdy w klientach:\r\n");
                foreach (var pole in pomijaniKlienci)
                {
                    komunikat.AppendLine($"{pole.Key}:\r\n {pole.Value.Select(x => $"{x.Id} [EMAIL:{x.Email}, SYMBOL: {x.Symbol}]\r\n").Join(", ")}\r\n\r\n");
                }
                LogiFormatki.PobierzInstancje.LogujInfo(komunikat.ToString());
            }

            return klienciErp.ToDictionary(x=> x.Key, x=> x.Value);
        }
        private void SynchronizujKlientow(Dictionary<long, Klient> klienciErp, Dictionary<long, Klient> klienciNaPlatfomie)
        {
            LogiFormatki.PobierzInstancje.LogujInfo("Start synchronizacji klientów.");
            Klient kw=new Klient();
            HashSet<long> doAktualizacji = new HashSet<long>();
            HashSet<long> doUsuniecia = new HashSet<long>();
            HashSet<long> doDodania = new HashSet<long>();
            klienciErp.KopiujPolaIstniejaceObiekty(klienciNaPlatfomie,
                new { gid = kw.Gid, kw.GidIp, data_ostatniego_logowania = kw.DataOstatniegoLogowania, kw.DataDodatnia, BlokadaPowod = kw.PowodBlokady, kw.ZgodaNaNewsletter });
            
            klienciErp = klienciErp.Values.Where(x => x.Aktywny).ToDictionary(x => x.Id, x => x);//tylko aktrywny mo¿emy dodawaæ
            klienciNaPlatfomie = klienciNaPlatfomie.Where(x => x.Key > 0 && x.Value.Aktywny).ToDictionary(x => x.Key, x => x.Value);
            klienciNaPlatfomie.Porownaj(klienciErp, ref doAktualizacji, ref doDodania, ref doUsuniecia, null, this);

            if (doUsuniecia.Count > 0)
            {
                IEnumerable<Klient> dowylaczenia = klienciNaPlatfomie.WhereKeyIsIn(doUsuniecia);
                foreach (Klient k in dowylaczenia)
                {
                    k.Aktywny = false;
                    k.Email = null;
                    k.Login = null;
                }
                ApiWywolanie.AktualizujKlientow(dowylaczenia);
            }
            if (doDodania.Count > 0)
            {
                List<Klient> poziomyDoDodania = klienciErp.WhereKeyIsIn(doDodania);
                ApiWywolanie.AktualizujKlientow(poziomyDoDodania);
            }

            if (doAktualizacji.Count > 0)
            {
                List<Klient> poziomyDoAktualizacji = klienciErp.WhereKeyIsIn(doAktualizacji);
                ApiWywolanie.AktualizujKlientow(poziomyDoAktualizacji);
            }
            LogiFormatki.PobierzInstancje.LogujInfo("Koniec synchronizacji klientów.");
        }

        private void AktualizujAdresy(Dictionary<Adres, KlientAdres> adresy, Dictionary<long, Klient> klienciB2B, Dictionary<long,Adres> adresyNaPlatfomie)
        {
            IEnumerable<long> adrDel = adresy.Values.Where(x => !klienciB2B.Keys.Contains(x.KlientId)).Select(x=>x.AdresId);
            List<Adres> adresList = adresy.Keys.ToList();
            adresList.RemoveAll(x => adrDel.Contains(x.Id));
            adresList.ForEach(x => x.Miasto = x.Miasto.Trim());
            adresList.ForEach(x => x.KrajId = x.KrajId == 0 ? null : x.KrajId);

            //Dictionary<long, Adres> adresyNaPlatfomie = ApiWywolanie.PobierzAdresy().Values.Where(x=>!x.RecznieDodany()).ToDictionary(x=>x.Id,x=>x);

            SynchronizujKolekcje(adresList, adresyNaPlatfomie.Values.Where(x => !x.RecznieDodany()).ToDictionary(x => x.Id, x => x), (data) => data.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujAdresy, ApiWywolanie.UsunAdresy, new {new Adres().DataDodania });
        }

        private void AktualizujLacznikiAdresow(Dictionary<Adres, KlientAdres> adresy, Dictionary<long, Klient> klienciNaPlatformie)
        {
            //Pobieramy adresy recznie dodane, ¿eby nie usuwaæ ich ³¹czników
            HashSet<long> adresyRecznieDodane = new HashSet<long>( ApiWywolanie.PobierzAdresy().Where(x => x.Value.RecznieDodany()).Select(x => x.Key) );
            Dictionary<long, KlientAdres> lacznikiAdresow = ApiWywolanie.PobierzLacznikiAdresow().Values.Where(x=>!adresyRecznieDodane.Contains(x.AdresId)).ToDictionary(x => x.Id, x => x);

            //laczniki do usuniecia
            List<long> lacznikiDel = adresy.Values.Where(x => !klienciNaPlatformie.ContainsKey(x.KlientId)).Select(x => x.Id).ToList();
            List<KlientAdres> laczniki = adresy.Values.ToList();
            laczniki.RemoveAll(x => lacznikiDel.Contains(x.Id));
            laczniki.ForEach(x=>x.TypAdresu = (x.TypAdresu==null)?TypAdresu.Glowny:x.TypAdresu);
            SynchronizujKolekcje(laczniki, lacznikiAdresow, (data) => data.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujLacznikiAdresow, ApiWywolanie.UsunLacznikiAdresow);
        }
        /// <summary>
        /// Metoda która dodaje kraje których nie ma w adresach w erp ale s¹ w recznie dodanych oraz dezaktywuje te które nie sa wykorzystywane
        /// </summary>
        /// <param name="kraje"></param>
        /// <param name="adresy"></param>
        /// <param name="krajeNaPlatfomie"></param>
        public void OperacjeNaKrajach(List<Kraje> kraje, Dictionary<long, Adres> adresy, Dictionary<int, Kraje> krajeNaPlatfomie)
        {
            //Kraje wykorzystane w recznie dodanych adresach
            HashSet<int> idRecznieDodane_Kraje = new HashSet<int>( adresy.Values.Where(x => x.RecznieDodany() && x.KrajId.HasValue).Select(x => x.KrajId.Value) );

            //sprawdzamy których krajow z recznie dodanych adresów brakuje w kolekcji je¿eli s¹ takie to je dodajemy
            HashSet<int> krajeBrakujace = new HashSet<int>( idRecznieDodane_Kraje.Except(kraje.Select(x => x.Id)) );
            if (krajeBrakujace != null && krajeBrakujace.Any())
            {
                kraje.AddRange(krajeNaPlatfomie.WhereKeyIsIn(krajeBrakujace));
            }

            //pobieramy id krajów które powinny byæ dezaktywowane
            HashSet<int> idKrajowDoDezaktywacji = new HashSet<int>( krajeNaPlatfomie.Keys.Except(kraje.Select(x => x.Id)) );
            if (idKrajowDoDezaktywacji != null && idKrajowDoDezaktywacji.Any())
            {
                //przy dezaktywacji kraju musimy wy³¹czaæ jego widoczonoœæ oraz czyœcic symbol
                kraje.AddRange(krajeNaPlatfomie.WhereKeyIsIn(idKrajowDoDezaktywacji).Select(x => new Kraje(x.Id, x.Nazwa, "", false)));
            }

        }


        private void AktualizujKraje(List<Kraje> kraje, Dictionary<long, Adres> adresy)
        {
            kraje.ForEach(x=>x.Synchronizowane=true);
            Dictionary<int, Kraje> krajeNaPlatfomie = ApiWywolanie.PobierzKraje().Values.Where(x => x.Synchronizowane && x.Widoczny).ToDictionary(x => x.Id, x => x);

            OperacjeNaKrajach(kraje, adresy,krajeNaPlatfomie);
            kraje.ForEach(x => x.JezykId = SyncManager.Konfiguracja.JezykIDPolski);
            log.DebugFormat($"Kraje: {kraje.ToCsv()}");
            SynchronizujKolekcje(kraje, krajeNaPlatfomie, (data) => data.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujKraje, null);
        }
        private void AktualizujRegiony(List<Model.Region> regiony)
        {
            regiony.ForEach(x => x.Synchronizowane = true);
            Dictionary<int, Model.Region> regionyNaPlatfomie = ApiWywolanie.PobierzRegiony().Values.Where(x => x.Synchronizowane).ToDictionary(x => x.Id, x => x);
            SynchronizujKolekcje(regiony, regionyNaPlatfomie, (data)=>data.ToDictionary(x=>x.Id,x=>x), ApiWywolanie.AktualizujRegiony, ApiWywolanie.UsunRegiony);
        }

        public void CzyszczeniePolNiewidocznychProduktow(ref List<Produkt> listaProduktow, Dictionary<long, KategoriaProduktu> kategorie, List<Grupa> grupyNaPlatformie, List<ProduktKategoria> pkzERP)
        {
            HashSet<long> idWidocznychGrup = new HashSet<long>( grupyNaPlatformie.Where(x => x.Widoczna).Select(y => y.Id) );
            HashSet<long> idWidocznychKategorii = new HashSet<long>( kategorie.Values.Where(x => x.Widoczna && idWidocznychGrup.Contains(x.GrupaId)).Select(y => y.Id) );

            HashSet<long> idProduktowZKategoriiWidocznych = new HashSet<long>( pkzERP.Where(x => idWidocznychKategorii.Contains(x.KategoriaId)).Select(y=>y.ProduktId) );
           // HashSet<int> test = idProduktowZKategoriiNiewidocznych.RemoveWhere(x=>pkzERP.FirstOrDefault(x=>x))

            List<string> listaPolDoWyzerowania = SyncManager.Konfiguracja.PolaDoWyzerowania;
            foreach (var produkt in listaProduktow)
            {
                if (!idProduktowZKategoriiWidocznych.Contains(produkt.Id) || !produkt.Widoczny)
                {
                    foreach (var pole in listaPolDoWyzerowania)
                    {
                        PropertyInfo pi = produkt.GetType().GetProperties().FirstOrDefault(x => x.Name == pole);
                        if (pi == null)
                        {
                            throw new InvalidOperationException("Brak pola o nazwie " + pole);
                        }
                        object wartosc = pi.GetValue(produkt);
                        
                        if (!wartosc.RownyWartosciDomyslnej())
                        {
                            Type t = wartosc.GetType();
                            object dom = t.IsValueType?Activator.CreateInstance(t):null;
                            pi.SetValue(produkt,dom);
                        }
                    }
                }
            }
        }

        private void AktualizujProdukty(List<SyncModul> moduly)
        {
            List<Tlumaczenie> produktyTlumaczenia;
            List<JednostkaProduktu> jednostki;
            Dictionary<long, Produkt> wszystkieProduktyNaB2B = ApiWywolanie.PobierzProdukty();
            Dictionary<long, Cecha> cechyNaB2B = ApiWywolanie.PobierzCechy();
            Dictionary<int, Atrybut> atrybutyNaPlatformie = ApiWywolanie.PobierzAtrybuty();
            List<Grupa> grupyNaPlatformie = ApiWywolanie.PobierzGrupy();
            Dictionary<long, KategoriaProduktu> kategorie = ApiWywolanie.PobierzKategorie();
            HashSet<string> magazyny = ApiWywolanie.PobierzMagazynySymbole();
            var idsatrybutow = PobierzAtrybutyKtorymNiePobieramyCechCech(atrybutyNaPlatformie.Values);
            LogiFormatki.PobierzInstancje.LogujInfo("Pobieram ³¹czniki z ERP");
            Dictionary<long, ProduktCecha> wszystkiecp = ApiWywolanie.PobierzCechyProdukty().Values.Where(x => !x.RecznieDodany()).ToDictionary(x => x.Id, x => x);
            LogiFormatki.PobierzInstancje.LogujInfo($"Pobrano {wszystkiecp.Count} ³¹czników cechyProdukty" );
            List<Produkt> produktyERP = AktualnyProvider.PobierzProdukty(out produktyTlumaczenia, out jednostki, magazyny);
            LogiFormatki.PobierzInstancje.LogujInfo($"Pobrano {produktyERP.Count} produktów z ERP, jednostki: {jednostki.Count} i magazyny: {magazyny.Count}.");

            foreach (var p in produktyERP)
            {
                if (p.Dostawa == "0")
                {
                    p.Dostawa = null;
                }
            }

            HashSet<long> idProduktERP = new HashSet<long>( produktyERP.Select(x => x.Id) );

            //Z wszystkich produktów które s¹ na b2b wyci¹gamy tylko te które s¹ w erpie lub nie ma ich ale sa aktywne (aby je zdezaktywowac)
            Dictionary<long, Produkt> produktyNaB2B = wszystkieProduktyNaB2B.Where(x => idProduktERP.Contains(x.Key) || (!idProduktERP.Contains(x.Key) && x.Value.Widoczny)).ToDictionary(x => x.Key, x => x.Value);

            LogiFormatki.PobierzInstancje.LogujInfo("Pobrano: {0} produktów z ERP, produktów na B2B: {1}", produktyERP.Count, produktyNaB2B.Count);
            LogiFormatki.PobierzInstancje.LogujInfo("Widocznych produktow z ERP: {0}", produktyERP.Count(x => x.Widoczny));
            Dictionary<long, ProduktCecha> produktyCechyErp = AktualnyProvider.PobierzCechyProduktow_Polaczenia(idsatrybutow).ToDictionary(x => x.Id, x => x);
            
            LogiFormatki.PobierzInstancje.LogujInfo("Wygenerowano {0} ³¹czników cechyProdukty. Przed wys³aniem ³¹czniki bêda przefiltrowane wg. cech i produktów na B2B.", produktyCechyErp.Count);

            List<ProduktKategoria> pkzErp = new List<ProduktKategoria>();
            List<ProduktyZamienniki> zamiennikierp = new List<ProduktyZamienniki>();

            if (grupyNaPlatformie.Count > 0)
            {
                List<ProduktCecha> cp = wszystkiecp.Values.ToList();

                foreach (Grupa grupa in grupyNaPlatformie)
                {
                    KategorieProduktowSyncHelper.PobierzInstancje.KategorieProduktyZCechy(grupa, ref pkzErp, cp);
                }
            }

            //po co to tak w³aœciwie jest? pokazuje to samo co logi ciut wy¿ej
            LogiFormatki.PobierzInstancje.LogujInfo("Wygenerowano {0} ³¹czników do kategorii", pkzErp.Count);

            //Zakomentowane ze wzglêdu na task #8510. Nie zerujemy produktów bez widocznej kategorii grupy
            //CzyszczeniePolNiewidocznychProduktow(ref produkty, kategorie, grupyNaPlatformie, pkzERP);             //Czyszczenie pol produktow jezeli grupa, kategoria albo widocznosc produktu ustawiona jest na false
            produktyERP.OperacjeNaPolachTekstowych();

            Produkt prod = new Produkt();
            var slownikProduktow = produktyERP.ToDictionary(x => x.Id, x => x);

            produktyERP = slownikProduktow.Values.ToList();
            slownikProduktow.KopiujPolaIstniejaceObiekty(produktyNaB2B, new { prod.WyslanoMailNowyProdukt, prod.DataDodania }); //Kopiowanie pól WyslanoMailNowyProdukt oraz DataDodania z istniejacych obiektow
            List<ProduktUkryty> produktuUkryteErp = null;
            PrzesunModulNaKoniec(moduly, typeof(Modules_.NoweModuly.Produkty.KtorePolaSynchronizowac));
            PrzesunModulNaKoniec(moduly, typeof(KtorePolaSynchronizowacJednostki));

            List<Cecha> cechytmp = new List<Cecha>(cechyNaB2B.Values.ToList());
            List<Atrybut> atrybuty = new List<Atrybut>(atrybutyNaPlatformie.Values.ToList());

            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IModulProdukty).Przetworz(ref produktyERP, ref produktyTlumaczenia, produktyNaB2B, ref jednostki, ref produktyCechyErp, ref pkzErp, AktualnyProvider, ref produktuUkryteErp, ref zamiennikierp, kategorie, ref cechytmp, ref atrybuty);
                ZakonczModul(m);
            }
            Dictionary<long, Klient> klienci = ApiWywolanie.PobierzKlientow().Where(x => x.Key > 0).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            //Produkty wirtualne o ile s¹
            if (SyncManager.Konfiguracja.WirtualneProduktyProvider != null)
            {
                ProduktyWirtualneProvider providerWirtualnych = SyncManager.Konfiguracja.WirtualneProduktyProvider;
                LogiFormatki.PobierzInstancje.LogujInfo("------Rozpoczynanie tworzenia wirtualnych produktów, wg. providera: {0}", providerWirtualnych.GetType().Name);
                
                Dictionary<long, Klient> klienciB2B = klienci.ToDictionary(x => x.Key, x => new Klient(x.Value));

                providerWirtualnych.SynchronizatorPrzetwarzajWirtualneProdukty(ref produktyERP, ref produktyTlumaczenia, produktyNaB2B, ref jednostki, ref produktyCechyErp, ref pkzErp, AktualnyProvider, ref produktuUkryteErp, ref zamiennikierp, kategorie, ref cechytmp, ref atrybuty, ref klienci);
                LogiFormatki.PobierzInstancje.LogujInfo("Koniec tworzenia wirtualnych produktów, wg. providera: {0}.", providerWirtualnych.GetType().Name);

                LogiFormatki.PobierzInstancje.LogujInfo($"Liczba produktow abstrakryjnych: {produktyERP.Count(x => x.Abstrakcyjny)}, liczba wszytkich produktów: {produktyERP.Count}");

                //Jeœli provider wymaga to aktualizujemy klientów.
                if (providerWirtualnych.AktualizujKlientow)
                {
                    SynchronizujKlientow(klienci, klienciB2B);
                }               
            }
            
            slownikProduktow.KopiujPolaIstniejaceObiekty(produktyNaB2B, new { prod.WyslanoMailNowyProdukt, prod.DataDodania }); //Kopiowanie pól WyslanoMailNowyProdukt oraz DataDodania z istniejacych obiektow

            LogiFormatki.PobierzInstancje.LogujDebug($"Liczba produktow abstrakryjnych po kopiowaniu: {produktyERP.Count(x => x.Abstrakcyjny)}, liczba wszytkich produktów: {produktyERP.Count}");
            //Wyciagamy produkty które s¹ pojedyncze w rodzinie
            var atrybutyRodzinowe = SyncManager.Konfiguracja.AtrybutyRodzin;
            bool czySaRodziny = produktyERP.Any(x => !string.IsNullOrEmpty(x.Rodzina));

            if (czySaRodziny && (atrybutyRodzinowe == null || !atrybutyRodzinowe.Any()))
            {
                throw new Exception("B³¹d. Posiadasz produkty rodzinowe jednak nie masz uzupe³nionego ustawienia: id_Atrybutu_rodziny. Uzupe³nienie tego ustawienia jest wymagane.");
            }
            
            //rêczne usuwanie rodzin
            if (czySaRodziny)
            {
                HashSet<long> zPojedynczymDzieckiem = new HashSet<long>( produktyERP.Where(x => !string.IsNullOrEmpty(x.Rodzina)).GroupBy(x => x.Rodzina).Where(x => x.Count() == 1).Select(x => x.First().Id) );
                Parallel.ForEach(produktyERP, p =>
                {
                    if (!zPojedynczymDzieckiem.Contains(p.Id))
                    {
                        return;
                    }
                    LogiFormatki.PobierzInstancje.LogujInfo("Usuwanie rodziny o nazwie: {0} - brak dzieci dla rodziny", p.Rodzina);
                    p.Rodzina = null;
                });
            }

            Produkt pTemp = new Produkt();

            //usuwanie dubli kodów kreskowych
            produktyERP.UsunDuble(new {kod_kreskowy = pTemp.KodKreskowy});
            produktyERP.SprawdzDuble(new { pTemp.Kod, pTemp.Id });
            

            Dictionary<long, Produkt> produktyErp = produktyERP.ToDictionary(x => x.Id, x => x);

            Parallel.ForEach(produktyERP, produkt =>
            {
                List<string> zaDlugieWartosci = Tools.SprawdzIloscZnakow(produkt);
                if (zaDlugieWartosci.Any())
                {
                    LogiFormatki.PobierzInstancje.LogujInfo("Ucieto ilosc znakow dla pol: {0} dla produktu id: {1}, nazwa: {2}", zaDlugieWartosci.Join(","), produkt.Id, produkt.Nazwa);
                }
                if (produkt.IloscWOpakowaniu == 0)
                {
                    produkt.IloscWOpakowaniu = 1;
                }
            });

            HashSet<long> doAktualizacji = new HashSet<long>();
            HashSet<long> doUsuniecia = new HashSet<long>();
            HashSet<long> doDodania = new HashSet<long>();
            produktyNaB2B.Porownaj(produktyErp, ref doAktualizacji, ref doDodania, ref doUsuniecia, new { pTemp.DataDodania, pTemp.WyslanoMailNowyProdukt }, this);
            LogiFormatki.PobierzInstancje.LogujDebug("Aktualizacja produktów");
            if (doUsuniecia.Count > 0)
            {
                List<Produkt> poziomyDoDeaktywacji = produktyNaB2B.WhereKeyIsIn(doUsuniecia);
                poziomyDoDeaktywacji.ForEach(x => x.UstawWidocznoscProduktu(false));
                ApiWywolanie.AktualizujProdukty(poziomyDoDeaktywacji);
            }
            if (doDodania.Count > 0)
            {
                List<Produkt> poziomyDoDodania = produktyErp.WhereKeyIsIn(doDodania);
                ApiWywolanie.AktualizujProdukty(poziomyDoDodania);
            }
            if (doAktualizacji.Count > 0)
            {
                List<Produkt> poziomyDoAktualizacji = produktyErp.WhereKeyIsIn(doAktualizacji);
                ApiWywolanie.AktualizujProdukty(poziomyDoAktualizacji);
            }

            produktyErp = produktyErp.Where(x => x.Value.Widoczny).ToDictionary(x => x.Key, x => x.Value);

            HashSet<long> idProduktow = new HashSet<long>( produktyErp.Select(x => x.Key) );
            jednostki.RemoveAll(x => !idProduktow.Contains(x.ProduktId));
            LogiFormatki.PobierzInstancje.LogujDebug("Aktualizacja s³owników");
            AktualizujSlowniki(typeof(ProduktBazowy), produktyTlumaczenia);
            LogiFormatki.PobierzInstancje.LogujDebug("Aktualizacja jednostek");
            AktualizujJednostki(jednostki);

            AktualizujCechyProdukty(produktyCechyErp.Values.ToList(), produktyErp, wszystkiecp);
            LogiFormatki.PobierzInstancje.LogujDebug("Aktualizacja kategorii produktów");
            AktualizujProduktyKategorie(pkzErp, produktyErp);
            LogiFormatki.PobierzInstancje.LogujDebug("Aktualizacja produktów ukrytych");


            if (produktuUkryteErp != null && produktuUkryteErp.Any())
            {
                var kat = ApiWywolanie.PobierzKategorieKlientow();
                ProduktyWyszukiwanie.PobierzInstancje.SprawdzProduktyUkryte(produktuUkryteErp, klienci, produktyErp, cechyNaB2B, kategorie, kat);
            }
            

            AktualizujProduktyUkryte(produktuUkryteErp);
            LogiFormatki.PobierzInstancje.LogujDebug("Aktualizacja zamienników");
            AktualizujZamienniki(zamiennikierp);
        }


        private IDictionary<long, ProduktyZamienniki> SprawdzZamieniki(IEnumerable<ProduktyZamienniki> data)
        {
            List<ProduktyZamienniki> wynik=new List<ProduktyZamienniki>();
            List<long> idproduktow = ApiWywolanie.PobierzProduktyId();
            foreach (var x in data)
            {
                if (!idproduktow.Contains(x.ProduktId) || !idproduktow.Contains(x.ZamiennikId) )
                {
                    continue;
                }
                wynik.Add(x);
            }
           
            return wynik.ToDictionary(x => x.Id, x => x);
        }
        private void AktualizujZamienniki(List<ProduktyZamienniki> zamiennikierp)
        {
            Dictionary<long, ProduktyZamienniki> zamiennikb2B = ApiWywolanie.PobierzZamienniki();
            SynchronizujKolekcje(zamiennikierp, zamiennikb2B, SprawdzZamieniki, ApiWywolanie.AktalizujZamienniki, ApiWywolanie.UsunZamienniki);
        }

        private void AktualizujProduktyKategorie(List<ProduktKategoria> lacznikislownik, Dictionary<long, Produkt> produktyB2B)
        {
            Dictionary<long, KategoriaProduktu> kategorieB2B = ApiWywolanie.PobierzKategorie();
            int liczbaLacznikow = lacznikislownik.Count;

            Dictionary<long, ProduktKategoria> pkzNaPlatfromie = ApiWywolanie.PobierzProduktyKategoriePolaczenia();
            log.DebugFormat($"Produkty na b2b {produktyB2B.Count} kategorie na b2b {kategorieB2B.Count}");
            lacznikislownik.RemoveAll(x =>x.ProduktId>0 && !produktyB2B.Keys.Contains(x.ProduktId));
            lacznikislownik.RemoveAll(x =>x.KategoriaId>0 && !kategorieB2B.Keys.Contains(x.KategoriaId));
            if (lacznikislownik.Count != liczbaLacznikow)
            {
                log.WarnFormat($"£¹czniki zosta³y skasowane z powodu braku produktów lub braku kategorii. Liczba skasowanych ³¹czników: {liczbaLacznikow - lacznikislownik.Count}");
            }
            SynchronizujKolekcje(lacznikislownik, pkzNaPlatfromie, (a)=>a.ToDictionary(x=>x.Id,x=>x), ApiWywolanie.AktualizujProduktyKategoriePolaczenia, ApiWywolanie.UsunProduktyKategoriePolaczenia);
        }
        private void AktualizujCechyProdukty(List<ProduktCecha> laczniki, Dictionary<long, Produkt> produktyB2B, Dictionary<long, ProduktCecha> cechyProduktyB2B)
        {
            LogiFormatki.PobierzInstancje.LogujDebug($"Aktualizacja ³¹czników cech produktów. Iloœc z ERP: {laczniki.Count}, iloœæ na B2B: {cechyProduktyB2B.Count}");

            Dictionary<long, Cecha> cechyB2B = ApiWywolanie.PobierzCechy();
            log.DebugFormat($"£¹czników na wejœciu: {laczniki.Count}");

            int iloscLacznikow = laczniki.Count;
            laczniki.RemoveAll(x => !produktyB2B.Keys.Contains(x.ProduktId));

            if (iloscLacznikow != laczniki.Count)
            {
                log.InfoFormat($"Czêœæ ³¹czników zosta³a skasowana z powodu braku produktów. Skasowano: {iloscLacznikow - laczniki.Count} ³¹czników.");
                iloscLacznikow = laczniki.Count;
            }

            laczniki.RemoveAll(x => !cechyB2B.Keys.Contains(x.CechaId));
            if (iloscLacznikow != laczniki.Count)
            {
                log.InfoFormat($"Czêœæ ³¹czników zosta³a skasowana z powodu braku cech. Skasowano: {iloscLacznikow - laczniki.Count} ³¹czników.");
            }

            SynchronizujKolekcje(laczniki, cechyProduktyB2B, (data) => data.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujCechyProdukty, ApiWywolanie.UsunCechyProdukty);
        }

        public void SynchronizujKolekcje<T, TT>(IEnumerable<TT> daneErp, IDictionary<T, TT> daneNaPlatformie, Func<IEnumerable<TT>, IDictionary<T, TT>> metodaFiltrujacoPrzeksztalacajco, Action<IList<TT>> metodaDodajacaAktualizujaca, Action<HashSet<T>> metodaUsuwajaca, object pomijane = null)
        {
            SynchronizujKolekcje(daneErp, daneNaPlatformie, metodaFiltrujacoPrzeksztalacajco, metodaDodajacaAktualizujaca, metodaDodajacaAktualizujaca, metodaUsuwajaca, pomijane);
        }
        private void SynchronizujKolekcje<T, TT>(IEnumerable<TT> daneErp, IDictionary<T, TT> daneNaPlatformie, Func<IEnumerable<TT>, IDictionary<T, TT>> metodaFiltrujacoPrzeksztalacajco, 
            Action<IList<TT>> metodaDodajaca, Action<IList<TT>> metodaAktualizujaca, Action<HashSet<T>> metodaUsuwajaca, object pomijane = null)
        {
            HashSet<T> doAktualizacji = new HashSet<T>();
            HashSet<T> doUsuniecia = new HashSet<T>();
            HashSet<T> doDodania = new HashSet<T>();
            IDictionary<T, TT> daneErpSlownik = metodaFiltrujacoPrzeksztalacajco(daneErp);
            daneNaPlatformie.Porownaj(daneErpSlownik, ref doAktualizacji, ref doDodania, ref doUsuniecia, pomijane, this);
            if (doUsuniecia.Count > 0)
            {
                if (metodaUsuwajaca == null)
                {
                    throw new Exception("S¹ elementy do usuniecia, ale nie ma metody usuwaj¹cej!");
                }
                metodaUsuwajaca.Invoke(doUsuniecia);
            }

            //Aktualizacja musi byæ przed dodawnia bo w sytuacji gdy obiekt zostanie usuniety a na jego miejsce zostanie dodany inny to id siê pokryj¹ wiêc trzeba zaktualizowaæ ceche o tym id - dokladnie komentrz w tasku #10021
            if (doAktualizacji.Count > 0)
            {
                IList<TT> poziomyDoAktualizacji = daneErpSlownik.WhereKeyIsIn(doAktualizacji);
                metodaAktualizujaca(poziomyDoAktualizacji);
            }

            if (doDodania.Count > 0)
            {
                IList<TT> poziomyDoDodania = daneErpSlownik.WhereKeyIsIn(doDodania);

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Dodowanie obiektów (10 pierwszych):");
                    foreach (var obiekt in (poziomyDoDodania.Count > 10 ?  poziomyDoDodania.Take(10) : poziomyDoDodania) )
                    {
                        log.Debug(obiekt.ToJson());
                    }
                }

                metodaDodajaca(poziomyDoDodania);
            }
        }

        private void AktualizujJednostki(List<JednostkaProduktu> jednostki)
        {
            Dictionary<long, Jednostka> jednostkiB2B = ApiWywolanie.PobierzJednostki();  
            var jednostkiWErpie = JednostkiWErpie(jednostki, jednostkiB2B);
            Jednostka pomijanie = new Jednostka();
            SynchronizujKolekcje(jednostkiWErpie, jednostkiB2B, (data) => data.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujJednostki, ApiWywolanie.UsunJednostki, new {pomijanie.Calkowitoliczowa, pomijanie.Komunikat});
            List<ProduktJednostka> lacznikErp = jednostki.Select(x=>x.Lacznik) .ToList();
            Dictionary<long, ProduktJednostka> produktyJednoskiB2B = ApiWywolanie.PobierzProduktyJednostki();
            SynchronizujKolekcje(lacznikErp, produktyJednoskiB2B, (a) => a.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujProduktyJednostkiJednostki, ApiWywolanie.UsunJednostkiLaczniki);
        }

        public List<Jednostka> JednostkiWErpie(List<JednostkaProduktu> jednostki, Dictionary<long, Jednostka> jednostkiB2B)
        {
            Dictionary<long, Jednostka> jedostkiErp = jednostki.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => new Jednostka(jednostki.First(y => y.Id == x.Key)));
            LogiFormatki.PobierzInstancje.LogujInfo($"Iloœæ jednostek na B2B: {jednostkiB2B.Count}, w ERP: {jedostkiErp.Count}.");

            //Robimy tak poniewa¿ dodajemy jednostki podczas synchronizacji dokumentów przez co nie mo¿emy usuwaæ jednostek z b2b
            HashSet<long> tylkoNaB2B = new HashSet<long>( jednostkiB2B.Keys.Except(jedostkiErp.Keys) );
            foreach (var i in tylkoNaB2B)
            {
                jedostkiErp.Add(i, new Jednostka(jednostkiB2B[i]));
            }

            //sortowanie zeby do walidacji zawsze taka sama kolejnosc wchodzila - sortowanie po czestosci wystepowania
            List<Jednostka> jednostkiDoWalidacji = jedostkiErp.Values.Where(x => !string.IsNullOrEmpty(x.Nazwa)).ToList();  //.OrderByDescending(x => jednostki.Count(z => z.Id == x.Id))
            List<Jednostka> jednostkiWErpie = new List<Jednostka>(jednostkiDoWalidacji.Count);
            foreach (Jednostka jednostka in jednostkiDoWalidacji)
            {
                // walidacja kodów z kropkami i bez
                //czy juz jest jednostka o takiej samej nazwie (z kropka i bez na koncu)
                //var jednstokaDubel = jednostkiWErpie.FirstOrDefault(x => x.Nazwa == jednostka.Nazwa || x.Nazwa.TrimEnd('.') == jednostka.Nazwa.TrimEnd('.'));
                //if (jednstokaDubel != null)
                //{
                //    LogiFormatki.PobierzInstancje.LogujInfo($"Zdublowana jednostka o nazwie: '{jednostka.Nazwa}' (id: {jednostka.Id}). Na platforme zostanie wys³ana wersja: '{jednstokaDubel.Nazwa}' (id: {jednstokaDubel.Id}), pominiêta bêdzie wersja: '{jednostka.Nazwa}'  (id: {jednostka.Id})");
                //    //kasowanie polaczen do tej omijanej jednostki
                //    //jednostki.RemoveAll(x => x.Id == jednostka.Id);
                //    Parallel.ForEach(jednostki.Where(x => x.Id == jednostka.Id), j =>
                //    {
                //        j.Id = jednstokaDubel.Id; //przepisanie jednostki
                //    });
                //    //nie mozemy pomijac tych skopanych jednostek bo sie usuwa kleintom i cuda sie dzieja - klient mogl sobie cos ustawiæ na tym ju¿
                //    //jednostka.Aktywna = false;
                //}

                //poprawienie jezyka
                jednostka.JezykId = Konfiguracja.JezykIDDomyslny;
                //dodanie bo jest OK
                jednostkiWErpie.Add(jednostka);
            }
            log.Debug($"Iloœæ jednostek produktów po filtracji wg. dubli - do wys³ania do B2B: {jednostkiWErpie.Count}.");
            return jednostkiWErpie;
        }

        private void AktualizujSlowniki(Type typ, List<Tlumaczenie> tlumacznia)
        {

            if (tlumacznia.Count == 0) return; //tlumaczen nie usuwamy, wiêcz jak jest 0 to nie ma co tej metody wywo³ywaæ
            Dictionary<long, Tlumaczenie> tlumaczeniaNaB2B = ApiWywolanie.GetSlowniki().Where(x => x.Value.Typ == typ.PobierzOpisTypu()).ToDictionary(x => x.Key, x => x.Value);
            LogiFormatki.PobierzInstancje.LogujInfo($"tlumaczenie na b2b: {tlumaczeniaNaB2B.Count}, typ: {typ.PobierzOpisTypu()}");
            SynchronizujKolekcje(tlumacznia, tlumaczeniaNaB2B, (a) => a.ToDictionary(x => x.Id, x => x), ApiWywolanie.DodajTlumaczenia, ApiWywolanie.UsunTlumaczenia);
        }

        private void AktualizujKategorieProduktow(List<SyncModul> moduly)
        {
            List<Grupa> grupyPRoduktow = ApiWywolanie.PobierzGrupy();
            Dictionary<long, KategoriaProduktu> kategorieNaPlatformie = ApiWywolanie.PobierzKategorie();
            Dictionary<long, KategoriaProduktu> kategorieWerp = new Dictionary<long, KategoriaProduktu>();

            LogiFormatki.PobierzInstancje.LogujInfo("Pobranych {0} grup i {1} kategorii z B2B", grupyPRoduktow.Count, kategorieNaPlatformie.Count);

            foreach (Grupa grupa in grupyPRoduktow)
            {
                Dictionary<long, KategoriaProduktu> kategorie = new Dictionary<long, KategoriaProduktu>();
                KategorieProduktowSyncHelper.PobierzInstancje.KategorieZCechy(grupa, kategorie);
                foreach (KeyValuePair<long, KategoriaProduktu> keyValuePair in kategorie)
                {
                    if (!kategorieWerp.ContainsKey(keyValuePair.Key))
                    {
                        kategorieWerp.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                    else
                    {
                        var istniejacaKategoria = kategorieWerp[keyValuePair.Key];
                        if (istniejacaKategoria.GrupaId != keyValuePair.Value.GrupaId)
                        {
                            LogiFormatki.PobierzInstancje.LogujInfo("B³¹d - zdublowane kategorie. Prawdopodobnie kilka grup wykorzystuje te same cechy do budowania drzewa kategorii. " +
                                                                    "Cecha ID która jest wykorzystana kilka razy: {0}, dla grup: {1} i {2}",
                                                                    keyValuePair.Value.Id, keyValuePair.Value.GrupaId, istniejacaKategoria.GrupaId);
                        }
                    }
                }
                LogiFormatki.PobierzInstancje.LogujInfo("Dla grupy: {0} zbudowano {1} kategorii", grupa.Nazwa, kategorie.Count);
                LogiFormatki.PobierzInstancje.LogujDebug("Kategorie: {0}", grupa.Nazwa, kategorie.Select(x=>x.Value.Nazwa).ToCsv());
            }

            PrzesunModulNaKoniec(moduly, typeof(Modules_.NoweModuly.KategorieProduktow.KtorePolaSynchronizowac));
            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IModulKategorieProduktow).Przetworz(ref kategorieWerp, kategorieNaPlatformie, AktualnyProvider, grupyPRoduktow);
                ZakonczModul(m);
            }

            LogiFormatki.PobierzInstancje.LogujDebug("Iloœæ kategorii ERP: {0}", kategorieWerp.Count);
            foreach (var kat in kategorieWerp)
            {
                kat.Value.JezykId = SyncManager.Konfiguracja.JezykIDPolski;
            }
            SynchronizacjaKategorii(kategorieWerp.Values.ToList());
        }

        private void SynchronizacjaKategorii(List<KategoriaProduktu> kategorie)
        {
            Dictionary<long, KategoriaProduktu> kategorieNaPlatformie = ApiWywolanie.PobierzKategorie();
            kategorie.ForEach(x=>x.JezykId = SyncManager.Konfiguracja.JezykIDPolski);

            HashSet<long> kategorieKtorychParentowNieMaNaB2B = new HashSet<long>( kategorie.Where(x => x.ParentId != null && !kategorieNaPlatformie.ContainsKey(x.ParentId.Value)).Select(x=>x.ParentId.Value) );
            if (kategorieKtorychParentowNieMaNaB2B.Any())
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"Aktualizowanie: {kategorieKtorychParentowNieMaNaB2B.Count} kategorii parentów któych nie ma na b2b");
                var kategorieDoDodania = kategorie.Where(x => kategorieKtorychParentowNieMaNaB2B.Contains(x.Id)).ToList();
                ApiWywolanie.AktualizujKategorieProduktow(kategorieDoDodania);
                foreach (var kat in kategorieDoDodania)
                {
                    kategorieNaPlatformie.Add(kat.Id,kat);
                }
            }
            
            SynchronizujKolekcje(kategorie, kategorieNaPlatformie, (a) => a.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujKategorieProduktow, ApiWywolanie.UsunKategorieProduktow);
        }

        private void AktualizujWysylaniePDF(IEnumerable<SyncModul> moduly)
        {
            List<StatusDokumentuPDF> statusDokumentuPdf = ApiWywolanie.PobierzDokumentyDlaKtorychTrzebaDrukowacFaktureElektroniczna().OrderByDescending(x=>x.IdDokumentu).ToList();
            
            LogiFormatki.PobierzInstancje.LogujInfo($"Pobrano {statusDokumentuPdf.Count} dokumentów dla któych powinny byæ drukowane PDF.");
            int paczkaRozmiar = 5;
            for (int i = 0; i < statusDokumentuPdf.Count; )
            {
                List<StatusDokumentuPDF> paczka = statusDokumentuPdf.Skip(i).Take(paczkaRozmiar).ToList();
                foreach (SyncModul m in moduly)
                {
                   UruchomModul(m);
                    (m as IFakturyPdf).Przetworz(ref paczka, AktualnyProvider);
                    ZakonczModul(m);
                }
                LogiFormatki.PobierzInstancje.LogujInfo($"Wysy³anie PDFów do B2B: {paczka.Count}.");
                ApiWywolanie.AktualizujDokumentElektroniczne(paczka);
                i += paczkaRozmiar;
            }
        }

        private void AktualizujListyPrzewozowe(List<SyncModul> moduly)
        {
            if (!moduly.Any())
            {
                return;
            }
            DokumentySearchCriteria docCriteia = new DokumentySearchCriteria();
            docCriteia.Rodzaj.Add(RodzajDokumentu.Faktura);
            LogiFormatki.PobierzInstancje.LogujDebug("Pobieram dokumenty z b2b");
            var dokumentyNab2B = ApiWywolanie.PobierzHashDokumentow();
            LogiFormatki.PobierzInstancje.LogujDebug("Dokumentów z b2b: {0}", dokumentyNab2B.Count);
            List<HistoriaDokumentuListPrzewozowy> listy = new List<HistoriaDokumentuListPrzewozowy>();
            foreach (SyncModul m in moduly)
            {
               UruchomModul(m);
                (m as IModulListyPrzewozowe).Przetworz(ref listy, dokumentyNab2B, AktualnyProvider);
                ZakonczModul(m);
            }

            LogiFormatki.PobierzInstancje.LogujDebug("Pobieram Listy przewozowe z b2b");
            Dictionary<int, HistoriaDokumentuListPrzewozowy> listynab2B = ApiWywolanie.PobierzListyPrzewozowe();
            //LogiFormatki.PobierzInstancje.LogujDebug(string.Format("Iloœæ Listów przewozowe z b2b : {0}",listynab2B.Count));
            //Dictionary<string, HistoriaDokumentuListPrzewozowy> listyb2Bprop = (listynab2B!=null && listynab2B.Any()) ? listynab2B.Values.ZbudojSlownikZKluczemPropertisowym(): new Dictionary<string, HistoriaDokumentuListPrzewozowy>();

            //LogiFormatki.PobierzInstancje.LogujDebug(string.Format("Iloœæ Listów przewozowe z erp : {0}", listy.Count));
            //Dictionary<string, HistoriaDokumentuListPrzewozowy> listyerp = listy.ZbudojSlownikZKluczemPropertisowym();
            //int id = -1;
            //foreach (var jp in listyerp)
            //{
            //    jp.Value.Id = listyb2Bprop.ContainsKey(jp.Key) ? listyb2Bprop[jp.Key].Id : id--;
            //}
            //var listyWerp = listyerp.Values;

            SynchronizujKolekcje(listy, listynab2B, (data)=>data.ToDictionary(x=>x.Id,x=>x), listyDoAktualizacji => ApiWywolanie.AktalizujListyPrzewozowe(listyDoAktualizacji), ApiWywolanie.UsunListyPrzewozowe);
        }
        /// <summary>
        /// wyliczamy wartoœc vat-u dla listy dokumnetów na podstawie pozycji 
        /// </summary>
        /// <param name="dokumenty"></param>
        public void WyliczWartoscVatDlaDokumentow(Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> dokumenty)
        {
            Parallel.ForEach(dokumenty, dokument =>
            {
                dokument.Key.WartoscVat = WyliczWartoscVatDlaPozycji(dokument.Value);
            });
        }

        /// <summary>
        /// Wyliczamy wartoœæ VAT-u dla listy pozycji
        /// </summary>
        /// <param name="pozycje">Lista pozycji dla której mamy wyliczyæ wartoœæ vatu</param>
        /// <returns>Wartoœæ VAT</returns>
        public decimal WyliczWartoscVatDlaPozycji(List<HistoriaDokumentuProdukt> pozycje)
        {
            decimal wartoscVat = 0;
            Dictionary<decimal, decimal> wartosciNettoDlaVat = new Dictionary<decimal, decimal>();
            //wyliczmy kwote netto dla kazdej stawki vat-u
            foreach (var produkt in pozycje)
            {
                if (wartosciNettoDlaVat.ContainsKey(produkt.Vat))
                {
                    wartosciNettoDlaVat[produkt.Vat] += produkt.WartoscNetto;
                }
                else
                {
                    wartosciNettoDlaVat.Add(produkt.Vat, produkt.WartoscNetto);
                }
            }
            wartoscVat = wartosciNettoDlaVat.Sum(x => Math.Round((x.Key / 100) * x.Value, 2, MidpointRounding.AwayFromZero));
            return wartoscVat;
        }

        /// <summary>
        /// MEGA metoda Bartka do testu dokumentów - NIE WOLNO nic zmieniaæ bo ³apy pourywam - i wszystko po takim opisie wiadomo co ona robi 
        /// </summary>
        /// <param name="dokument"></param>
        /// <param name="pozycje"></param>
        /// <param name="blad"></param>
        /// <returns></returns>
        public bool SprawdzDokumentCzyPoprawny(HistoriaDokumentu dokument, List<HistoriaDokumentuProdukt> pozycje, out string blad)
        {
            blad = null;
            decimal dokumentSumaNetto = 0;
            Dictionary<decimal, decimal> dokumentTabelaVatu = new Dictionary<decimal, decimal>();

            //sprwadzanie kwot pozycji czy sie zgadzaj netto brutto
            foreach (var poz in pozycje)
            {
               
                if (poz.Vat < 1 && poz.Vat > 0)
                {
                    blad = $"Vat jest w z³ym formacie. Powinien byæ wiêkszy ni¿ 0 i nie byæ liczb¹ u³amkow¹, a jest: {poz.Vat}";
                    return false;
                }

                if (string.IsNullOrEmpty(poz.NazwaProduktu) || string.IsNullOrEmpty(poz.KodProduktu))
                {
                    blad = $"Brak nazwy lub kody dla pozycji id: {poz.Id}";
                    return false;
                }

                //zakladamy ze cena netto jest stala i reszte liczymy sami
                
                //cena brutto przed rabatem czy sie zgadzaja
                decimal cenaBruttoJakaPowinnaByc = Math.Round((poz.CenaNetto*(poz.Vat/100m) + poz.CenaNetto), 2, MidpointRounding.AwayFromZero);
                if (Math.Round(poz.CenaBrutto, 2) != cenaBruttoJakaPowinnaByc)
                {
                    log.Debug($"cena netto: {poz.CenaNetto}, vat: {poz.Vat}");
                    blad = $"Dla pozycji o ID: {poz.Id} cena brutto przed rabatem powinna wynosiæ: {cenaBruttoJakaPowinnaByc}, ale wynosi: {poz.CenaBrutto}";
                    return false;
                }

                //netto po rabacie
                decimal cenaNettoPoRabacieJakaPowinnaByc = 0;
                if (poz.Rabat != 0)
                {
                    cenaNettoPoRabacieJakaPowinnaByc = Math.Round(poz.CenaNetto - (poz.CenaNetto*(poz.Rabat/100m)),2, MidpointRounding.AwayFromZero);                    
                }
                else
                {
                    cenaNettoPoRabacieJakaPowinnaByc = poz.CenaNetto;
                }

                if (poz.CenaNettoPoRabacie != cenaNettoPoRabacieJakaPowinnaByc)
                {
                    blad = $"Dla pozycji o ID: {poz.Id} cena netto po rabacie powinna wynosiæ: {cenaNettoPoRabacieJakaPowinnaByc}, ale wynosi: {poz.CenaNettoPoRabacie}. Cena netto przed rabatem: {poz.CenaNetto}, rabat: {poz.Rabat}";
                    return false;
                }

                //cena brutto po rabacie
               
                    decimal cenaBruttoPoRabaciePowinnaByc = Math.Round(poz.CenaNettoPoRabacie*(poz.Vat/100m) + poz.CenaNettoPoRabacie, 2,MidpointRounding.AwayFromZero);
                    if (poz.CenaBruttoPoRabacie != cenaBruttoPoRabaciePowinnaByc)
                    {
                        blad = $"Dla pozycji o ID: {poz.Id} cena brutto po rabacie powinna wynosiæ: {cenaBruttoPoRabaciePowinnaByc}, ale wynosi: {poz.CenaBruttoPoRabacie}. Cena netto po rabacie: {poz.CenaNettoPoRabacie}, vat: {poz.Vat}";
                        return false;
                    }

                //ceny PO RABACIE
                
                    //wartosc NETto po rabacie
                    decimal wartoscNettoPoRabacieowinnaByc = Math.Round(poz.CenaNettoPoRabacie*poz.Ilosc, 2, MidpointRounding.AwayFromZero);
                    if (Math.Round(poz.WartoscNetto, 2) != wartoscNettoPoRabacieowinnaByc)
                    {
                        blad =$"Dla pozycji o ID: {poz.Id} wartoœæ netto po rabacie powinna wynosiæ: {wartoscNettoPoRabacieowinnaByc}, ale wynosi: {poz.WartoscNetto}. Cena netto po rabacie: {poz.CenaNettoPoRabacie}, iloœæ: {poz.Ilosc}.";
                        return false;
                    }
                

                //wartosc VAt po rabacie
                decimal wartoscVatPoRabaciePowinnaByc = Math.Round(poz.WartoscNetto*poz.Vat/100, 2, MidpointRounding.AwayFromZero);
                if (Math.Round(poz.WartoscVat, 2) != wartoscVatPoRabaciePowinnaByc)
                {
                    blad = $"Dla pozycji o ID: {poz.Id} wartoœæ VAT po rabacie powinna wynosiæ: {wartoscVatPoRabaciePowinnaByc}, ale wynosi: {poz.WartoscVat}. Wartosc netto: {poz.WartoscNetto}, vat: {poz.Vat}.";
                    return false;
                }

                //wartosc brutto po rabacie
                decimal wartoscBruttoPoRabaciePowinnaByc = Math.Round(poz.WartoscNetto + poz.WartoscVat,2,MidpointRounding.AwayFromZero);
                if (Math.Round(poz.WartoscBrutto, 2) != wartoscBruttoPoRabaciePowinnaByc)
                {
                    //jesli dokument jest liczony od czeny brutto w tym wyliczeniu mo¿e wyst¹piæ ró¿nica 1 gr ale to niekoniecznie jest b³¹d
                    blad =
                        $"Dla pozycji o ID: {poz.Id} wartoœæ brutto po rabacie powinna wynosiæ: {wartoscBruttoPoRabaciePowinnaByc}, ale wynosi: {poz.WartoscBrutto}. Wartosc netto: {poz.WartoscNetto}, vat: {poz.Vat}.";
                    return false;
                }

                dokumentSumaNetto += poz.WartoscNetto;

                if (dokumentTabelaVatu.ContainsKey(poz.Vat))
                {
                    dokumentTabelaVatu[poz.Vat] += poz.WartoscNetto;
                }
                else
                {
                    dokumentTabelaVatu.Add(poz.Vat, poz.WartoscNetto);
                }
                
                //ZAOKRAGLENIA robimy to dlatego ze czasem dostajemy liczbe do 4 miejc z ERPa - w sumie to w ERPi powinno byc porawione, ale tu te¿ to robimy
                poz.CenaNettoPoRabacie = Math.Round(poz.CenaNettoPoRabacie, 2, MidpointRounding.AwayFromZero);
                poz.CenaBruttoPoRabacie = Math.Round(poz.CenaBruttoPoRabacie, 2, MidpointRounding.AwayFromZero);
                poz.WartoscNetto = Math.Round(poz.WartoscNetto, 2, MidpointRounding.AwayFromZero);
                poz.WartoscBrutto = Math.Round(poz.WartoscBrutto, 2, MidpointRounding.AwayFromZero);
                poz.WartoscVat = Math.Round(poz.WartoscVat, 2, MidpointRounding.AwayFromZero);
                poz.CenaNetto = Math.Round(poz.CenaNetto, 2, MidpointRounding.AwayFromZero);
                poz.CenaBrutto = Math.Round(poz.CenaBrutto, 2, MidpointRounding.AwayFromZero);

            }

            if (dokument.WartoscNetto != dokumentSumaNetto)
            {
                blad = $"Wartosæ dokumentu netto powinna wynosiæ: {dokumentSumaNetto} a wynosi: {dokument.WartoscNetto}";
                return false;
            }

            decimal sumaVatDokumentu = dokumentTabelaVatu.Sum(x => Math.Round( (x.Key/100)*x.Value, 2, MidpointRounding.AwayFromZero) );
            if (dokument.WartoscVat != sumaVatDokumentu)
            {
                blad = $"Wartosæ VAT dokumentu powinna wynosiæ: {sumaVatDokumentu} a wynosi: {dokument.WartoscVat}. ";
                return false;
            }

            decimal sumaBruttoDokumentu = sumaVatDokumentu + dokumentSumaNetto;
            if (dokument.WartoscBrutto != sumaBruttoDokumentu)
            {
                if (!dokument.CenyLiczoneOdBrutto)
                {
                    blad = $"Wartosæ brutto dokumentu powinna wynosiæ: {sumaBruttoDokumentu} a wynosi: {dokument.WartoscBrutto}";
                    return false;
                }

                log.Error($"Wartosæ brutto dokumentu powinna wynosiæ: {sumaBruttoDokumentu} a wynosi: {dokument.WartoscBrutto}");
            }

            return true;
        }

        private void AktualizujDokumenty(List<SyncModul> moduly)
        {
            DateTime date =  SyncManager.Konfiguracja.DokumentyOdKiedyPobierane;
            LogiFormatki.PobierzInstancje.LogujInfo("Dokumenty bêda pobierane od dnia {0} - wg. ustawienia B2B 'DokumentyIleMiesiecyPobieranie'", date);

            Dictionary<int, long> dokumenty = ApiWywolanie.PobierzHashDokumentow();
            LogiFormatki.PobierzInstancje.LogujInfo("Pobrano z B2B {0} dokumentów", dokumenty.Count);

            IDokumentyRoznicowe roznicowyProvider = AktualnyProvider as IDokumentyRoznicowe;
            if (roznicowyProvider == null) throw new InvalidOperationException("Aktualny provider  ksiêgowy nie obs³uguje tej funkcji");

            Dictionary<long, Klient> kliencib2B = ApiWywolanie.PobierzKlientow().Values.Where(x=>x.Id>0).ToDictionary(x=>x.Id,x=>x);

            //tylko dla aktywnych klientow dokumenty bierzemy
            List<Klient> klienciNaB2B = kliencib2B.Values.Where(x=>x.Aktywny).ToList();

            List<int> todel = new List<int>();
            if (dokumenty.Count > 0)
            {
                todel = roznicowyProvider.DokumentyDoUsuniecia(dokumenty, new HashSet<long>( kliencib2B.Keys )).Where(x => x > 0).ToList();
                LogiFormatki.PobierzInstancje.LogujDebug("Do usuniêcia dokumentów iloœæ: " + todel.Count);
            }

            Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> docsToSendERP = roznicowyProvider.DokumentyDoWyslania(dokumenty, date, klienciNaB2B);
            ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> docsToSend = new ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();

            LogiFormatki.PobierzInstancje.LogujInfo($"Z ERP pobrano: {docsToSendERP.Count} dokumentów");

            WyliczWartoscVatDlaDokumentow(docsToSendERP);

            Parallel.ForEach(docsToSendERP, dokument =>
            {
                string blad = null;
                try
                {
                    if (!this.SprawdzDokumentCzyPoprawny(dokument.Key, dokument.Value, out blad))
                    {
                        log.Error($"W dokumencie id: {dokument.Key.Id} o numerze: {dokument.Key.NazwaDokumentu} jest b³¹d i nie mo¿e byæ wys³any - dokument zostanie pominiêty w wysy³ce. B³¹d:\r\nCena liczona od brutto: {dokument.Key.CenyLiczoneOdBrutto}\t{blad}");
                    }
                    docsToSend.TryAdd(dokument.Key, dokument.Value);
                } catch (Exception e)
                {
                    LogiFormatki.PobierzInstancje.LogujError(e);
                    throw;
                }
            });

            if (docsToSend.Count != docsToSendERP.Count)
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"By³o:  {docsToSendERP.Count - docsToSend.Count} b³êdnych dokumentów - nie zostan¹ wys³ana do B2B! Szczegó³y b³êdów s¹ w logu.");
            }
            
            HistoriaDokumentu najstarszyDokument = docsToSend.OrderBy(x=> x.Key.DataUtworzenia).Select(x=> x.Key).FirstOrDefault();

            LogiFormatki.PobierzInstancje.LogujInfo($"Po filtracji porpawnoœci dokumentów pozosta³o: {docsToSend.Count} dokumentów - tyle te dokumenty bed¹ wysy³ane do B2B Najstarszy dokument: {najstarszyDokument?.NazwaDokumentu}, z daty: {najstarszyDokument?.DataUtworzenia.ToShortDateString()}.");
            HashSet<int> klucze = new HashSet<int>(docsToSend.Keys.Select(x => x.Id));
            
            Dictionary<int, long> hashePozycji = ApiWywolanie.PobierzHashPozycjiDokumentow(klucze);

            StringBuilder komunikat = new StringBuilder($"Do wys³ania dokumentów z ERP iloœæ: {docsToSend.Count}. Ograniczenie maksymalnie iloœæ dokumentów synchronizacji: {SyncManager.Konfiguracja.MaksimumDokumentowWPaczce} (ustawienie 'dokumentow_paczka').");

            if (SyncManager.Konfiguracja.SlowaWymaganeWDokumencie!=null && SyncManager.Konfiguracja.SlowaWymaganeWDokumencie.Any())
            {
                komunikat.AppendLine($"Dokumenty zosta³y przefiltrowne - w nazwach dokumentu musz¹ wyst¹piæ s³owa: {SyncManager.Konfiguracja.SlowaWymaganeWDokumencie.ToCsv()}");
            }

            if (SyncManager.Konfiguracja.SlowaZakazaneWDokumencie != null && SyncManager.Konfiguracja.SlowaZakazaneWDokumencie.Any())
            {
                komunikat.AppendLine($"Dokumenty zosta³y przefiltrowne - w nazwach dokumentu nie mog¹ wyst¹piæ s³owa: {SyncManager.Konfiguracja.SlowaZakazaneWDokumencie.ToCsv()} ");
            }

            LogiFormatki.PobierzInstancje.LogujInfo(komunikat.ToString());

            //Sprawdzamy i tworzymy nowe statusy zamowieñ
            List<StatusZamowienia> statusy = AktualnyProvider.PobierzStatusyDokumentow();
            foreach (StatusZamowienia s in statusy)
            {
                s.JezykId = SyncManager.Konfiguracja.JezykIDDomyslny;
                s.Nazwa = s.Symbol;
                s.PobranoErp = true;
                s.Widoczny = false;
            }

            PrzesunModulNaKoniec(moduly, typeof(KtorePolaSynchronizowacStatusy));
            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IModulDokumenty).Przetworz(ref docsToSend, ref statusy, dokumenty, ref klienciNaB2B);
                ZakonczModul(m);
            }
            AktualizujStatusyDokumentow(statusy);

            //tylko aktynwych. Sycnhro tu klientow dlatego ze sa limity kredytowe i inne dane ktore mogle byc zmienione
            SynchronizujKlientow(klienciNaB2B.Where(x=>x.Aktywny).ToDictionary(x=>x.Id,x=>x),kliencib2B);

            List<HistoriaDokumentu> dowywalenia = new List<HistoriaDokumentu>();
            Dictionary<int, StatusZamowienia> statusynaplatformie = ApiWywolanie.PobierzStatusyZamowien().ToDictionary(x => x.Id, x => x);

            HashSet<int> statusOferty = new HashSet<int>( statusynaplatformie.Values.Where(x => x.TraktujJakoOferte).Select(x => x.Id) );

            foreach (var dok in docsToSend)
            {
                if (dok.Value.Count == 0)
                {
                    dowywalenia.Add(dok.Key);
                    LogiFormatki.PobierzInstancje.LogujInfo(string.Format("Dokument {0} ma 0 pozycji i nie zostanie wys³any", dok.Key.NazwaDokumentu));
                    continue;
                }
                if (dok.Key.StatusId != null && !statusynaplatformie.ContainsKey(dok.Key.StatusId.Value))
                {
                    dowywalenia.Add(dok.Key);
                    LogiFormatki.PobierzInstancje.LogujInfo(string.Format("Dokument {0} ma status który nie istniej na b2b, status {1} dokument bêdzie pomijany", dok.Key.NazwaDokumentu, dok.Key.StatusId));
                    continue;
                }

                //spradzanie czy to typ OFERTA -> tylko dla zamowien
                if (statusOferty.Any() && dok.Key.Rodzaj == RodzajDokumentu.Zamowienie && dok.Key.StatusId.HasValue && statusOferty.Contains(dok.Key.StatusId.Value) )
                {
                    //jesli juz jest zrealizowane to PO OFERCIE
                    if (!dok.Key.Zaplacono)
                    {
                        dok.Key.Rodzaj = RodzajDokumentu.Oferta;
                    }
                }
            }
            
            //filtrowanie wszystkich dokumentów wg hashsetów dokumentów które maj¹ 0 pozycji i wysy³anie tylko tych które maj¹ jakie kolwiek pozycje
            foreach (var emelentdousuniecia in dowywalenia)
            {
                ((IDictionary)docsToSend).Remove(emelentdousuniecia);
            }
            
            LogiFormatki.PobierzInstancje.LogujDebug("Kasowanie dokumentów iloœæ: " + todel.Count);
            if (todel.Count > 0)
            {
                while (todel.Count > 0)
                {
                    int max = todel.Count > 1000 ? 1000 : todel.Count;

                    HashSet<int> todellist = new HashSet<int>( todel.Take(max) );

                    ApiWywolanie.DeleteDocuments(todellist);
                    todel = todel.Skip(max).ToList();
                }
            }

            //foreach (var v in docsToSend)
            //{
            //    LogiFormatki.PobierzInstancje.LogujDebug(string.Format("dokument {0} id {1} klient {2} data {3} hash{4}", v.Key.NazwaDokumentu, v.Key.Id, v.Key.KlientId, v.Key.DataUtworzenia, Tools.PobierzInstancje.PoliczHashDokumentu(v.Key)));
            //}

            //czyscimy liste pozycji jezeli sa takie same jak na platformie
            foreach (var dokument in docsToSend)
            {
                int idDokumentu = dokument.Key.Id;
                var hash = Tools.PobierzInstancje.PoliczHashPozycjiDokumentu(dokument.Value);
                if (hashePozycji.ContainsKey(idDokumentu) && hashePozycji[idDokumentu] == hash)
                {
                    dokument.Value.Clear();
                }
            }
            
            LogiFormatki.PobierzInstancje.LogujInfo("Wysy³anie na B2B {0} dokumentów", docsToSend.Count);
            ApiWywolanie.DodajDokumenty(docsToSend.Select(x=>new KlasaOpakowujacaDokumentyDoWyslania(x.Key,x.Value)).ToList());
            LogiFormatki.PobierzInstancje.LogujInfo("Koniec wys³ania dokumentów");
        }
        public virtual Dictionary<int, PoziomCenowy> PobierzPoziomCen()
        {
            return ApiWywolanie.PobierzPoziomyCen();
        }

        public virtual Dictionary<long, Waluta> PobierzWaluty()
        {
            return ApiWywolanie.PobierzWaluty();
        }

        private void AktualizujWaluty()
        {
            var walutyErp = AktualnyProvider.PobierzDostepneWaluty().Values.ToList();
            var walutyB2B = PobierzWaluty();
            walutyErp.ForEach(x => x.JezykId = SyncManager.Konfiguracja.JezykIDPolski);

            //numer konta przepisanie z API - bo recznie uzupelniane jest - dla Bioplanet zmiana, nie mieli modlu do ktore pola synchronizaowac

            foreach(Waluta walutaERP in walutyErp)
            {
                if (walutyB2B.TryGetValue(walutaERP.Id, out Waluta walutaERPa)) {
                    walutaERP.NrKonta = walutaERPa.NrKonta;
                }
            }

            SynchronizujKolekcje(walutyErp, walutyB2B, (a) => a.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujWaluty, ApiWywolanie.UsunWaluty);
        }
       
        private void AktualizujStatusyDokumentow(List<StatusZamowienia> statusyerp)
        {
            var statusynaplatformie = ApiWywolanie.PobierzStatusyZamowien().Where(x => x.PobranoErp).ToDictionary(x => x.Id, x => x);
            SynchronizujKolekcje(statusyerp, statusynaplatformie, (a) => a.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujStatusyZamowien, ApiWywolanie.UsunStatusyZamowien);
        }

        private void AktualizujStany(List<SyncModul> moduly)
        {
            if (AktualnyProvider is IStanyRoznicowe)
            {
                LogiFormatki.PobierzInstancje.LogujInfo(
                    "Provider obs³uguje stany ró¿nicowe, ale obecna wersja jeszcze tego nie wspiera");
            }

            List<Magazyn> listaMagazynowPlatformy = ApiWywolanie.PobierzMagazyny().Where(x => x.ImportowacZErp).ToList();
            List<Produkt> produktyB2B = ApiWywolanie.PobierzProdukty().Values.ToList();

            //todo Trzeba pobierac cale obiekty produktów w kazdym module z osobna pobieramy przez api co jest do dupy. Dodatkowo dzieki temu ze bedziemy mieli produkty mozemy wyciagnac tylko aktywne produkty i bedziemy mogli wywaliæ stany dla produktów zdezaktywowanych

            //HashSet<long> produktyB2B = new HashSet<long>(ApiWywolanie.PobierzProduktyId());
            if (!listaMagazynowPlatformy.Any())
            {
                LogiFormatki.PobierzInstancje.LogujError(new Exception("Na platformie nie zdefiniowano ¿adnego magazynu z opcj¹ IMPORTOWAC Z ERP!"));
                return;
            }

            Dictionary<int, List<ProduktStan>> listStanowDoWyslania = new Dictionary<int, List<ProduktStan>>();
            Dictionary<long, ProduktStan> stanyAktualneNieZerowePlatformy = new Dictionary<long, ProduktStan>();
            foreach (Magazyn m in listaMagazynowPlatformy)
            {
                var stanyb2b = ApiWywolanie.PobierzStanyProduktow(m);
                stanyAktualneNieZerowePlatformy.AddRange(stanyb2b.ToDictionary(x => x.Id, x => x));

                if (m.ImportowacZErp)
                {
                    Dictionary<long, decimal> stanyDoWyslania = StanyHelpers.PobierzStanyDlaMagazynow(AktualnyProvider, m);

                    LogiFormatki.PobierzInstancje.LogujInfo($"Stanów z ERP dla magazynu {m.Symbol} : {stanyDoWyslania.Count}");
                    LogiFormatki.PobierzInstancje.LogujInfo($"Stanów z B2B dla magazynu {m.Symbol} : {stanyb2b.Count}");
                    listStanowDoWyslania.Add(m.Id, new List<ProduktStan>());
                    //do modu³u trzeba wys³aæ wszystkie stany bo on dzia³a przecie¿ na ich podstawie, wczeœniej by³y przekazywane stany PO filtrowaniu co by³o idiotyczne
                    foreach (var p in stanyDoWyslania)
                    {
                        listStanowDoWyslania[m.Id].Add(new ProduktStan
                        {
                            MagazynId = m.Id,
                            ProduktId = p.Key,
                            Stan = p.Value < 0 ? 0 : decimal.Round(p.Value, 2)
                        });
                    }
                }
            }

            foreach (SyncModul mod in moduly)
            {
                UruchomModul(mod);
                (mod as IModulStany).Przetworz(ref listStanowDoWyslania, listaMagazynowPlatformy, produktyB2B);
                ZakonczModul(mod);
            }
            //porownanie z tym co jest - co wyslac
            //zmieni³em to tak ¿eby operowaæ na takiej samej liœcie i w razie czego usuwaæ z niej stany które siê nie zmieni³y
            //dziêki temu modu³y do stanów bêd¹ dzia³a³y poprawnie a potem filtrowanie zadzia³a tak samo jak wczeœniej - Andrew

            foreach (Magazyn m in listaMagazynowPlatformy)
            {
                List<ProduktStan> magazynstany = listStanowDoWyslania[m.Id];
                //listStanowDoWyslania.Where(a => a.MagazynId == m.Id).ToList();

                //stany do usuniecia - te które maja stan 0 oraz te które s¹ dla produktów których nie ma na platformie b¹dŸ s¹ zdezaktywowane
                HashSet<long> stanyDousuniecia = PobierzStanyDoUsuniecia(ref magazynstany, stanyAktualneNieZerowePlatformy, new HashSet<long>( produktyB2B.Where(x => x.Widoczny).Select(x => x.Id) ));
                if (stanyDousuniecia.Any())
                {
                    LogiFormatki.PobierzInstancje.LogujDebug($"Do usuniecia {stanyDousuniecia.Count} stanów");
                    ApiWywolanie.UsunStanyProduktow(stanyDousuniecia);
                }

                LogiFormatki.PobierzInstancje.LogujInfo("Stanów po przefiltrowaniu (stany które siê zmieni³y) do wys³ania : " + magazynstany.Count);

                if (magazynstany.Count > 0)
                {
                    ApiWywolanie.AktualizujStanyProduktow(magazynstany);
                }
            }
        }


        public HashSet<long> PobierzStanyDoUsuniecia(ref List<ProduktStan> listaWejsciowa, Dictionary<long, ProduktStan> stanyAktualneNieZerowePlatformy, HashSet<long>idProduktowNaPlatformie)
        {
            HashSet<long> stanyDoUsuniecia = new HashSet<long>();
            int max = listaWejsciowa.Count;
            for (int i = 0; i < max; i++)
            {
                //zmienna okreslajaca czy stan dla magazynu i produktu by³ na platformie
                ProduktStan stanPlatformy = null;
                bool jestNaPlatformie = true;
                stanyAktualneNieZerowePlatformy.TryGetValue(listaWejsciowa[i].Id, out stanPlatformy);
                if (stanPlatformy == null)
                {
                    //przyjmujemy ze jesli NULL, to znaczy ze 0 na platformie jest
                    jestNaPlatformie = false;
                    stanPlatformy = new ProduktStan { Stan = 0 };
                }

                //Sprawdzamy czy stan != 0 (nie chcemy w bazie stanów zerowych) oraz czy jest on wogole na platformie
                if (listaWejsciowa[i].Stan == 0 && jestNaPlatformie)
                {
                    stanyDoUsuniecia.Add(stanPlatformy.Id);
                }
                else
                {
                    //Jezli stan != 0 (nie chcemy w bazie stanów zerowych), stan ró¿ny od stanu na platformie to aktualizujemy stan w przeciwnym razie wywalamy ten stan i go nie wysylamy oraz produkt jest na platformie
                    if (listaWejsciowa[i].Stan != 0 && stanPlatformy.Stan != listaWejsciowa[i].Stan && idProduktowNaPlatformie.Contains(listaWejsciowa[i].ProduktId))
                    {
                        decimal aktualizowanyStan = listaWejsciowa[i].Stan < 0 ? 0 : listaWejsciowa[i].Stan;
                        listaWejsciowa[i].Stan = aktualizowanyStan;
                        continue;
                    }
                }
                listaWejsciowa.RemoveAt(i--);
                max--;
            }
            return stanyDoUsuniecia;
        }


        private IDictionary<long, KlientKategoriaKlienta> SprawdzLacznKlienciKategorie(IEnumerable<KlientKategoriaKlienta> data)
        {
            List<KlientKategoriaKlienta> tml=new List<KlientKategoriaKlienta>();
            HashSet<int> kategorieId = new HashSet<int>( ApiWywolanie.PobierzKategorieKlientow().Keys );
            HashSet<long> klietid = new HashSet<long>( ApiWywolanie.PobierzKlientow().Keys );
            foreach ( var x in data)
            {
                if (!kategorieId.Contains(x.KategoriaKlientaId))
                {
                    continue;
                    
                }
                if (!klietid.Contains(x.KlientId))
                {
                    continue;

                }
                tml.Add(x);
            }
            return tml.ToDictionary(x => x.Id, x => x);
        }
        private void AktualizujKategorieKlientow(IEnumerable<SyncModul> moduly)
        {
            IDictionary<long, KlientKategoriaKlienta> kategorieNaPlatformiekk = ApiWywolanie.PobierzKlienciKategorie(new Dictionary<string, object>(0));
            List<KlientKategoriaKlienta> kategorieWerPkk = AktualnyProvider.PobierzKategorieKlientowPolaczenia();
            Dictionary<int, KategoriaKlienta> kategorieNaPlatformie = ApiWywolanie.PobierzKategorieKlientow();
            List<KategoriaKlienta> kategorie = AktualnyProvider.PobierzKategorieKlientow();

            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IModulKategorieKlientow).Przetworz(ref kategorie, ref kategorieWerPkk);
                ZakonczModul(m);
            }
            kategorie.ForEach(x=>x.JezykId = SyncManager.Konfiguracja.JezykIDPolski);
            SynchronizujKolekcje(kategorie, kategorieNaPlatformie,PrzetworzKategoriePrzedWyslaniem, ApiWywolanie.AktualizujKategorieKlientow, ApiWywolanie.UsunKategorieKlientow);
            SynchronizujKolekcje(kategorieWerPkk, kategorieNaPlatformiekk, SprawdzLacznKlienciKategorie, items => ApiWywolanie.AktualizujKlienciKategorie(items), ApiWywolanie.UsunKlienciKategorie);
        }

        private IDictionary<int,KategoriaKlienta>  PrzetworzKategoriePrzedWyslaniem(IEnumerable<KategoriaKlienta> data)
        {
            List<KategoriaKlienta> tmp=new List<KategoriaKlienta>(data);
            tmp.ForEach(a => a.Grupa = string.IsNullOrEmpty(a.Grupa) ?  SyncManager.Konfiguracja.CechaAuto : a.Grupa);
            tmp.ForEach(a => a.Grupa = a.Grupa.ToUpper());
            tmp.ForEach(a => a.Nazwa = a.Nazwa.ToUpper().Replace("/", "").Replace("\\", ""));
            return tmp.ToDictionary(x => x.Id, x => x);
        }

        private int[] PobierzAtrybutyKtorymNiePobieramyCechCech(IEnumerable<Atrybut> atrybuty)
        {
            return atrybuty.Where(x => !x.PobierajCechy).Select(x => x.Id).ToArray();
        }
        
        private void AktualizujCechy(List<SyncModul> moduly)
        {
            Dictionary<int, Atrybut> atrybutyNaPlatformie = ApiWywolanie.PobierzAtrybuty();
            Dictionary<long, Produkt> produktyNaB2B = ApiWywolanie.PobierzProdukty();
            var atrybutydlaktorychniepobieramycechy = PobierzAtrybutyKtorymNiePobieramyCechCech(atrybutyNaPlatformie.Values);
            List<Atrybut> atrybuty = new List<Atrybut>();
            List<Cecha> cechy = AktualnyProvider.PobierzCechyIAtrybuty(out atrybuty, atrybutydlaktorychniepobieramycechy);

            LogiFormatki.PobierzInstancje.LogujInfo("Pobrano z ERPa: {0} atrybutów i {1} cech", atrybuty.Count, cechy.Count);

            Application.DoEvents();
            cechy.ForEach(x => x.Symbol = x.Symbol.ToLower().Trim());
            LogiFormatki.PobierzInstancje.LogujInfo("Modu³ów do uruchomienia {0}",moduly.Count());
            
            PrzesunModulNaKoniec(moduly, typeof(KtorePolaSynchronizowacAtrybuty));
            PrzesunModulNaKoniec(moduly, typeof(KtorePolaSynchronizowacCechy));
            
            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IModulCechyIAtrybuty).Przetworz(ref atrybuty, ref cechy, produktyNaB2B);
                ZakonczModul(m);
            }

            //duplikaty symboli
            Cecha cc = new Cecha();
            cechy.SprawdzDuble(new { cc.Symbol });
            atrybuty.ForEach(x=>x.JezykId = SyncManager.Konfiguracja.JezykIDPolski);

            LogiFormatki.PobierzInstancje.LogujInfo($"Atrybutów w ERP do wys³ania na B2B: {atrybuty.Count}");
            SynchronizujKolekcje(atrybuty, atrybutyNaPlatformie, (data) => data.ToDictionary(x => x.Id, x => x), atrybutyDoAktualizacji => ApiWywolanie.AktualizujAtrybuty(atrybutyDoAktualizacji), ApiWywolanie.UsunAtrybuty);
            
            LogiFormatki.PobierzInstancje.LogujInfo($"Cechy w ERP do wys³ania na B2B: {cechy.Count}");
            SynchronizacjaCech(cechy);
        }

        private void SynchronizacjaCech(List<Cecha> cechy)
        {
            Parallel.ForEach(cechy, c =>
            {
                List<string> zaDlugieWartosci = Tools.SprawdzIloscZnakow(c);
                if (zaDlugieWartosci.Any())
                {
                    LogiFormatki.PobierzInstancje.LogujError(new Exception($"Ucieto ilosc znakow dla pol: {zaDlugieWartosci.Join(",")} dla cechy o id: {c.Id}, nazwa: {c.Nazwa}"));
                }

                if (c.ObrazekId.HasValue && c.ObrazekId.Value < 0)
                {
                    c.ObrazekId = null;
                }
            });

            Dictionary<long, Cecha> cechyNaPlatformie = ApiWywolanie.PobierzCechy();
           

            //duplikaty symboli
            Cecha cc = new Cecha();
            cechy.SprawdzDuble(new { cc.Symbol });
            foreach (Cecha cecha in cechy)
            {
                //Oczyszczanie znaków zakazanych
                cecha.Nazwa = cecha.Nazwa.Replace("|", "");
            }
            SynchronizujKolekcje(cechy, cechyNaPlatformie, (data) => data.ToDictionary(x=>x.Id,x=>x), item => ApiWywolanie.AktualizujCechy(item), ApiWywolanie.UsunCechy); //bez usuwania CECH ApiWywolanie.UsunCechy - z powodu zmiany klucza
        }
       
        private void ImportRejestracji()
        {
            ISciaganieNowychRejestracji prov = AktualnyProvider as ISciaganieNowychRejestracji;
            if (prov == null)
            {
                throw new Exception("Aktualny provider ksiêgowy nie obs³uguje importu klientów");
            }
            IEnumerable<Rejestracja> nowe = ApiWywolanie.PobierzRejestracje();
            List<Rejestracja> zmienione = new List<Rejestracja>();
            foreach (var rejestracje in nowe)
            {
                Rejestracja zm = prov.ImportKlientowDoERP(rejestracje);
                zmienione.Add(zm);
            }
            ApiWywolanie.AktualizujRejestracje(zmienione);
        }

        //Jako ze z B2B przychodz¹ pozycje bez poka Jednostka to do poprawnego importu zamówieñ do WF-Maga musia³em 
        //dopisaæ metodê która uzupe³ni pole Jednosta na podstawie JednostkaMiary SG 
        private void UzupelnijNazwyJednostek(List<ZamowienieSynchronizacja> zamowienia, Dictionary<long, Jednostka> jednostka)
        {
            log.DebugFormat($"Uzupe³niam jednostki. Iloœæ jednostek: {jednostka.Count}");
            foreach (ZamowienieSynchronizacja zamowienieSynchronizacja in zamowienia)
            {
                foreach (var pozycja in zamowienieSynchronizacja.pozycje  )
                {
                    log.DebugFormat($"Pozycja:{pozycja.Id} [{pozycja.ProduktIdBazowy}] - {pozycja.JednostkaMiary}");
                    Jednostka jednostkaPozy = null;
                    if (string.IsNullOrEmpty(pozycja.Jednostka) && jednostka.TryGetValue(pozycja.JednostkaMiary,out jednostkaPozy))
                    {
                        log.DebugFormat($"Pozycja:{pozycja.Id}  - {jednostkaPozy.Nazwa}");
                        pozycja.UstawJednostke(jednostkaPozy.Nazwa);
                    }
                }
            }
        }

        private void ImportZamowien(IEnumerable<IModulZamowienia> moduly, IEnumerable<IPrzedZapisemZamowien> przedZapisem, IEnumerable<IPoZapisieZamowien> poZapisie)
        {
            List<ZamowienieSynchronizacja> zamowieniaDoZapisania = ApiWywolanie.PobierzZamowienia();
            if (!zamowieniaDoZapisania.Any()&&!przedZapisem.Any())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak zamówieñ do importu. Brak modu³ów");
                return;
            }

            LogiFormatki.PobierzInstancje.LogujInfo($"Pobrano {zamowieniaDoZapisania.Count} zamówieñ z b2b.");

            Dictionary<long, Klient> wszyscy = ApiWywolanie.PobierzKlientow();
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();
            Dictionary<long, Jednostka> jednostki = new Dictionary<long, Jednostka>();
            Dictionary<long, ProduktJednostka> produktyjednostki = new Dictionary<long, ProduktJednostka>();
            if (moduly.Any() || przedZapisem.Any() || poZapisie.Any())
            {
                produktyB2B = ApiWywolanie.PobierzProdukty();
                produktyjednostki = ApiWywolanie.PobierzProduktyJednostki();
            }
            
            jednostki = ApiWywolanie.PobierzJednostki();
            List<Cecha> cechyB2B = ApiWywolanie.PobierzCechy().Values.ToList();
            cechyB2B.ForEach(x => x.Symbol = x.Symbol.ToLower());

            List<ProduktCecha> cechyProdukty = ApiWywolanie.PobierzCechyProdukty().Values.ToList();

            LogiFormatki.PobierzInstancje.LogujInfo($"Pobrano klientów, produkty, jednostki i cechy z b2b");

            LogiFormatki.PobierzInstancje.LogujInfo($"Modu³ów przed zapisem {przedZapisem.Count()}, modu³ów po zapisie zamówieñ: {poZapisie.Count()}");
            foreach (IPrzedZapisemZamowien modul in przedZapisem)
            {
                UruchomModul(modul as SyncModul);
                modul.Przetworz(zamowieniaDoZapisania, wszyscy, produktyB2B, jednostki, produktyjednostki, AktualnyProvider);
            }

            log.Debug($"Zamówieñ do zapisania: {zamowieniaDoZapisania.Count}");
            if (!zamowieniaDoZapisania.Any())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak zamówieñ do importu.");
            }else
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"Zamówieñ do importu: {zamowieniaDoZapisania.Count}, numery: {zamowieniaDoZapisania.Select(x => x.NumerZPlatformy).ToCsv()}." );

                if (zamowieniaDoZapisania.Any(x => x.pozycje.Any(y => string.IsNullOrEmpty(y.Jednostka) && y.JednostkaMiary > 0)))
                {
                    LogiFormatki.PobierzInstancje.LogujDebug("S¹ pozycje bez jednostki");
                    this.UzupelnijNazwyJednostek(zamowieniaDoZapisania, jednostki);
                }

                for (int i = 0; i < zamowieniaDoZapisania.Count; i++)
                {
                    //Tworzymy
                    List<ZamowienieSynchronizacja> zamowienia = new List<ZamowienieSynchronizacja> {zamowieniaDoZapisania[i]};

                    foreach (IModulZamowienia m in moduly)
                    {
                        UruchomModul(m as SyncModul);
                        m.Przetworz(zamowienia[0], ref zamowienia, AktualnyProvider, jednostki, produktyjednostki,produktyB2B,cechyB2B,cechyProdukty);
                        ZakonczModul(m as SyncModul);
                    }
                    long a = zamowieniaDoZapisania[i].pozycje.Select(x=>x.ProduktIdBazowy).FirstOrDefault(x => x < 0);
                    if (a != 0)
                    {
                        LogiFormatki.PobierzInstancje.LogujError(new Exception($"Pomijamy zamówienie o numerze: {a} poniewa¿ w zamówieniu znajduje siê produkt dodany w panelu administracyjnym"));
                        zamowieniaDoZapisania[i].BladKomunikat = "Pomijamy zamówienie poniewa¿ w zamówieniu znajduje siê produkt dodany w panelu administracyjnym";
                        ApiWywolanie.AktualizujZamowienie(new List<ZamowienieSynchronizacja> { zamowieniaDoZapisania[i] });
                    }
                    else
                    {
                        log.Debug($"Zamówienia: {zamowienia.ToJson()}");
                        //wysy³anie zamówienia do providera do importu
                        List<ZamowienieSynchronizacja> zapisane = AktualnyProvider.ImportZamowien(zamowienia, wszyscy);

                        foreach (IPoZapisieZamowien modul in poZapisie)
                        {
                            UruchomModul(modul as SyncModul);
                            modul.Przetworz(zapisane, zamowieniaDoZapisania, wszyscy, produktyB2B, jednostki, produktyjednostki, AktualnyProvider);
                            ZakonczModul(modul as SyncModul);
                        }

                        List<ZamowienieSynchronizacja> doAktualizacji = new List<ZamowienieSynchronizacja>();
                        Dictionary<int, List<ZamowienieSynchronizacja>> pogrupowaneZapisaneZamowienia = zapisane.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.ToList());
                        
                        foreach (var zamowienieSynchronizacja in zapisane)
                        {
                            //jesli id zamowienia bazowego == 0to omijamy, pochodzi np. z edu coenncetora
                            if (zamowienieSynchronizacja.Id == 0)
                            {
                                continue;
                            }

                            //juz bylo obrabiane
                            if (doAktualizacji.Any(x => x.Id == zamowienieSynchronizacja.Id))
                            {
                                continue;
                            }

                          if (pogrupowaneZapisaneZamowienia.ContainsKey(zamowienieSynchronizacja.Id))
                            {
                               zamowienieSynchronizacja.ListaDokumentowZamowienia = UstawIdDokumentowSynchronizacji(pogrupowaneZapisaneZamowienia[zamowienieSynchronizacja.Id]);
                                if (string.IsNullOrEmpty(zamowienieSynchronizacja.BladKomunikat))
                                {
                                    log.DebugFormat(
                                        $"Po zapisie zwrócono z ERP {pogrupowaneZapisaneZamowienia[zamowienieSynchronizacja.Id].Count} zamówieñ stworzonych na podstawie zamowienia: {zamowienieSynchronizacja.Id} - stworzone dokumenty o ID: {string.Join(", ", zamowienieSynchronizacja.ListaDokumentowZamowienia.Select(x => x.IdDokumentu))}.");
                                }
                            }
                          doAktualizacji.Add(zamowienieSynchronizacja);
                        }
                        
                        if (doAktualizacji.Count > 0)
                        {
                            ApiWywolanie.AktualizujZamowienie(doAktualizacji);
                        }
                    }
                }
            }
            AktualnyProvider.CleanUp();
        }


        private List<ZamowienieDokumenty> UstawIdDokumentowSynchronizacji(List<ZamowienieSynchronizacja> listaZamowien)
        {
            List<ZamowienieDokumenty> zamowienieDokumenty = new List<ZamowienieDokumenty>();
            if (listaZamowien.Count > 1)
            {
                foreach (var zam in listaZamowien)
                {
                    zamowienieDokumenty.AddRange(zam.ListaDokumentowZamowienia);
                }
            }
            else
            {
                zamowienieDokumenty = listaZamowien.First().ListaDokumentowZamowienia;
            }
            return zamowienieDokumenty;
        }

        public void AktualizujPoziomyCenowe(List<SyncModul> moduly)
        {
            Dictionary<int, PoziomCenowy> poziomyNaB2B = ApiWywolanie.PobierzPoziomyCen();
            Dictionary<int, PoziomCenowy> poziomyWerp = AktualnyProvider.PobierzDostepnePoziomyCen().ToDictionary(x => x.Id, x => x);
            Dictionary<long, CenaPoziomu> cenyNaB2B = ApiWywolanie.PobierzPoziomyCenProduktow();
            List<CenaPoziomu> cenyPoziomy = AktualnyProvider.PobierzPoziomyCenoweProduktow();

            LogiFormatki.PobierzInstancje.LogujInfo($"Pobrano z B2B poziomów cenowych: {poziomyNaB2B.Count} i cen produktów: {cenyNaB2B.Count}");

            PrzesunModulNaKoniec(moduly, typeof(KtorePolaSynchronizowacPoziomyCen));
            foreach (SyncModul m in moduly)
            {
                UruchomModul(m);
                (m as IModulPoziomyCen).Przetworz(ref poziomyWerp, ref cenyPoziomy, poziomyNaB2B, cenyNaB2B);
                ZakonczModul(m);
            }

            //usuwanie cen zerowych - nie chcemy ich na B2B - BARTEK EKSPERYMENT!
            int usuniete = cenyPoziomy.RemoveAll(x => x.Netto == 0);
            if (usuniete > 0)
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"Pomijanie zerowych cen ERP - usuniêto: {usuniete} cen. Ca³kowita liczba cen produktów ERP: {cenyPoziomy.Count}.");
            }

            //poprawienie zaokragalnia do 2 miejsc - gdyby ERP wyslala do 4
            cenyPoziomy.ForEach(x=> x.Netto = Math.Round(x.Netto,2, MidpointRounding.AwayFromZero) );

            SynchronizujKolekcje(poziomyWerp.Values.ToList(), poziomyNaB2B, (data) => data.ToDictionary(x => x.Id, x => x), ApiWywolanie.AktualizujPoziomyCen, ApiWywolanie.UsunPoziomyCen);
            SynchronizujKolekcje(cenyPoziomy, cenyNaB2B, SprawdzCenyPoziomy, ApiWywolanie.AktualizujPoziomyCenProduktow, ApiWywolanie.UsunCenyPoziomy);
        }

        private IDictionary<long, CenaPoziomu> SprawdzCenyPoziomy(IEnumerable<CenaPoziomu> data)
        {
            Dictionary<int, PoziomCenowy> poziomyNaB2B = ApiWywolanie.PobierzPoziomyCen();
            List<long> produkty = ApiWywolanie.PobierzProduktyId();
            var cenypasujace = data.Where(x => poziomyNaB2B.Keys.Contains(x.PoziomId) && produkty.Contains(x.ProduktId)).ToList();  //wywalamy poziomy cen i produkty których nie ma na b2b
            foreach (CenaPoziomu cp in cenypasujace)
            {
                var poziomwaluta = poziomyNaB2B[cp.PoziomId].WalutaId;
                if (poziomwaluta!=null)
                {
                    if (cp.WalutaId!=null || cp.WalutaId != poziomwaluta)
                    {
                        cp.WalutaId = poziomwaluta;
                    }
                }
            }
            return cenypasujace.ToDictionary(x => x.Id, x => x);
        }

        //takie zabezpieczenie w razie jeœli klient ma wype³nione pola z id innego klienta którego na platformie nie ma
        //dziêki temu ci klienci nie bêd¹ nonstop wysy³ani na platformê
        private static void ResetKlientaNadrzednego(Klient c, List<Klient> klienciDoAktualizacji)
        {
            if (c.KlientNadrzednyId.HasValue)
            {
                if (klienciDoAktualizacji.All(a => a.Id != c.KlientNadrzednyId.Value) || c.KlientNadrzednyId == c.Id)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo(
                        string.Format("Klient {0} o id {2} ma klienta nadrzêdnego o id {1} ale nie ma go na platformie lub próba przypisania samego siebie.",
                                      c.Nazwa, c.KlientNadrzednyId.Value, c.Id));
                    c.KlientNadrzednyId = null;
                }
            }

            if (c.PrzedstawicielId.HasValue)
            {
                if (klienciDoAktualizacji.All(a => a.Id != c.PrzedstawicielId.Value) || c.PrzedstawicielId == c.Id)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo(
                        string.Format("Klient {0} o id {2} ma przedstawiciela o id {1} ale nie ma go na platformie lub próba przypisania samego siebie.",
                                      c.Nazwa, c.PrzedstawicielId.Value, c.Id));
                    c.PrzedstawicielId = null;
                }
            }

            if (c.OpiekunId.HasValue)
            {
                if (klienciDoAktualizacji.All(a => !a.Aktywny || a.Id != c.OpiekunId.Value) || c.OpiekunId == c.Id)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo(
                        string.Format("Klient {0} o id {2} ma opiekuna o id {1} ale nie ma go na platformie  lub próba przypisania samego siebie.",
                                      c.Nazwa, c.OpiekunId.Value, c.Id));
                    c.OpiekunId = null;
                }
            }

            if (c.DrugiOpiekunId.HasValue)
            {
                if (klienciDoAktualizacji.All(a => !a.Aktywny || a.Id != c.DrugiOpiekunId.Value) || c.DrugiOpiekunId == c.Id)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo(string.Format("Klient {0} o id {2} ma drugiego opiekuna o id {1} ale nie ma go na platformie lub próba przypisania samego siebie.", c.Nazwa, c.DrugiOpiekunId.Value, c.Id));
                    c.DrugiOpiekunId = null;
                }
            }
        }

        private void tylkoPlanowe_CheckedChanged(object sender, EventArgs e)
        {
            PrzeladujListeZadan();
        }

        private void btnUruchom_Click(object sender, EventArgs e)
        {
            panelZadan.Enabled = false;
            btnUruchom.Enabled = panelZadan.Enabled;
            log.DebugFormat($"panelZadan.Controls count {panelZadan.Controls.Count}");
            foreach (var c in panelZadan.Controls)
            {
                if ((c is ZadanieKontrolka) && (c as ZadanieKontrolka).Zaznaczone)
                {
                    ElementySynchronizacji zadanie = (c as ZadanieKontrolka).Zadanie.TypZadaniaSynchronizacji.Value;
                    List<SyncModul> moduly = PobierzModulDlaTypuZadania(zadanie);

                    log.InfoFormat("Pobrane ID modu³ów do uruchomienia dla zadania {1}: {0}",moduly.Select(x=>x.Id).ToCsv(), zadanie);

                    Zadanie z = (c as ZadanieKontrolka).Zadanie;

                    Program.ZadaniaDoUruchomienia.First(a => a.Id == z.Id).OstatnieUruchomienieStart = DateTime.Now;

                    WlaczZadanie(z, moduly);

                    //aktulizujemy moduly zeby czasy wyslac - czasy ustawiaja sie przy wywolaniu
                    foreach (var m in moduly)
                    {
                        Program.AktualizujZadanie(m.ZadanieBazowe);
                    }

                    Program.ZadaniaDoUruchomienia.First(a => a.Id == z.Id).OstatnieUruchomienieKoniec = DateTime.Now;
                    Program.AktualizujZadanie(z);
                }
            }
            panelZadan.Enabled = true;
            btnUruchom.Enabled = panelZadan.Enabled;
            PrzeladujListeZadan();
        }


        private List<SyncModul> PobierzModulDlaTypuZadania(ElementySynchronizacji element)
        {
            return Program.ModulyDoUruchomienia.Where(a => a.JakiejOperacjiSynchronizacjiDotyczy.Contains(element) && a.ZadanieBazowe.CzyPowinnoBycUruchomioneTeraz).OrderBy(x=>x.ZadanieBazowe.ModulKolejnosc).ToList();
        }

        private void chOdwrocZaznaczenie_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var c in panelZadan.Controls)
            {
                if (c is ZadanieKontrolka)
                {
                    (c as ZadanieKontrolka).Zaznaczone = !(c as ZadanieKontrolka).Zaznaczone;
                }
            }
        }

        private void panelZadan_MouseEnter(object sender, EventArgs e)
        {
            panelZadan.Focus();
        }

        private void logText_MouseEnter(object sender, EventArgs e)
        {
            logText.Focus();
        }

        public void WyswietlString(string komunikat)
        {
            LogiFormatki.PobierzInstancje.LogujInfo(komunikat);
        }

        private void UruchomModul(SyncModul mod)
        {
            LogiFormatki.PobierzInstancje.LogujInfo($" ---- Uruchamianie modu³u id: {mod.Id}, {mod.GetType().Name} - w celu: {mod.Komentarz} ----");
            mod.ApiWywolanie = ApiWywolanie;
            mod.KategorieKlientowWyszukiwanie = KategorieKlientowWyszukiwanie.PobierzInstancje;
            mod.KategorieKlientowWyszukiwanie.Konfiguracja = SyncManager.Konfiguracja;
            mod.ZadanieBazowe.OstatnieUruchomienieStart = DateTime.Now;
        }

        private void ZakonczModul(SyncModul mod)
        {
            mod.ZadanieBazowe.OstatnieUruchomienieKoniec = DateTime.Now;
            LogiFormatki.PobierzInstancje.LogujInfo($"**** CZAS DZIA£ANIA: { (mod.ZadanieBazowe.OstatnieUruchomienieKoniec - mod.ZadanieBazowe.OstatnieUruchomienieStart).Value.TotalSeconds } sekund. *******");
        }

        #region TIMER
        private void PokazTimer()
        {
            CzasTimera.Visible = LabelTimera.Visible = true;
        }

        private void UkryjTimer()
        {
            CzasTimera.Visible = LabelTimera.Visible = false;
        }


        private void UstawTimer()
        {
            _dataRozpoczeciaOdliczania = DateTime.Now.AddMinutes( SyncManager.Konfiguracja.CzasDoZamknieciaSynchronizacji);
            TimerWylaczajacy.Start();
            PokazTimer();
        }

        private void ZatrzymajTimer()
        {
            TimerWylaczajacy.Stop();
            UkryjTimer();
        }

        private void TimerWylaczajacy_Tick(object sender, EventArgs e)
        {
            TimeSpan roznicaCzasu = _dataRozpoczeciaOdliczania - DateTime.Now;
            if (roznicaCzasu.TotalMinutes <= 0)
            {
                WylaczAplikacje();
            }

            else CzasTimera.Text = string.Format("{0:m\\:ss}", roznicaCzasu);
            Application.DoEvents();
        }

        #endregion
    }
}
