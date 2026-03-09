using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Web;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using log4net;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.App_Start;
using SolEx.Hurt.Web.Site2.Bindowania;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Http;
using ServiceStack.OrmLite;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using ServiceStack.OrmLite.SqlServer;
using Klient = SolEx.Hurt.Core.Klient;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Web.Site2.Factories;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Msie;
using StackExchange.Profiling;

namespace SolEx.Hurt.Web.Site2
{
    public class Global : HttpApplication
    {
        private static ILog Log
        {
            get
            {
                return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;

        /// <summary>
        /// metoda uruchamia zdarzenia raz dziennie
        /// </summary>
        protected void Application_Start_RazDziennie()
        {
            string sciezkaPlikuDoRozpoznania = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plikGlobalAsaxPierwszerUchomienie");

            //czy plik istnieje
            if (System.IO.File.Exists(sciezkaPlikuDoRozpoznania))
            {
                var fileInfo = new FileInfo(sciezkaPlikuDoRozpoznania);
                if (fileInfo.CreationTime.DayOfYear == DateTime.Now.DayOfYear)
                {
                    return; //nic nie robimy - wychodzimy bo juz dzis bylo uruchomione
                }
                File.Delete(sciezkaPlikuDoRozpoznania);
            }

            //normalne operajce zaplanowane
            SolexBllCalosc.PobierzInstancje.Konfiguracja.KasowaniePdfBezDokumentow();
            SolexBllCalosc.PobierzInstancje.ProfilKlienta.CzyszczenieUstawienWProfilu();
            SolexBllCalosc.PobierzInstancje.ProfilKlienta.InicjalizujDomyslneWartosci();
            SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjUstawienia(); //ukrywanie i tak uruchamia w srodku dodawanie ustawien nowych
            SolexBllCalosc.PobierzInstancje.Statystyki.UsunStareStatystyki();
            SolexBllCalosc.PobierzInstancje.MaileBLL.UsunStaraHistorieMaili();
            SolexBllCalosc.PobierzInstancje.Konfiguracja.UsunLogi();
            SolexBllCalosc.PobierzInstancje.DokumentyDostep.UsunNiepotrzebne();
            SolexBllCalosc.PobierzInstancje.Pliki.UsunPlikiFizyczneBezPlikuWBazie();


            //hashe szukania
            SolexBllCalosc.PobierzInstancje.Szukanie.PrzeliczHashDlaProduktow(true);

            File.AppendAllText(sciezkaPlikuDoRozpoznania, DateTime.Now.ToString("G"));
        }

        protected void Application_Start(object sender, EventArgs e)
        {
#if DEBUG
            //miniprofiler
            MiniProfiler.Configure(new MiniProfilerOptions
            {
                RouteBasePath = "~/profiler",
                ResultsAuthorize = request => request.IsLocal,
                StackMaxLength = 256
            }
            );
#endif

            //aktywacja stimulsfot licencji
            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkNsz2LZEqO9R75UfwWagpgSn18+MBfuFvutcmS4/iaHrO4lK" +
                                            "UxMyqjOnXjplps8Pp5zDEbKpBnJ/Re3WZRLXsBL3t6lPzfJaKzgfZQUGnqVASilS9Xwu+aNXI9HLCbOO9IikPB/qTr" +
                                            "ppP31uHwvA921SCI26tmcM0NQd//Q0r0PTUq/Yr9wRfGmUBq3fOospGDelETNkfExAdMq+p8faMG/IPkNvIr8/ykjQ" +
                                            "GLgMzE3ujecpsiUII2KzmbbD89YA5mhqKxTLLIqMU6XzX/j5nduRpX3CU3WQYXAK65Uf5g1Ki4PXD110q3JDMZHtam" +
                                            "pW9vLc/jyv+RS+DXdKivLsUtjALHyyKIMTSL63HCCUbufzVTcco7Yribs3a1/mEnc4xVT8u6IaVLUXSUtS03NvxmUV" +
                                            "LFttCFhbr5ulBbFdi4e0yC34sQIxwlzD8n6/z6s9JL2DTIPDjWUyp5GhU8jrQXDdU4w8AZ8HGFGZLXBIzOlgYcr8X8" +
                                            "Ugb6LeIuym32BE4+Y/yTARtqyHx28dWacyYkP8Iywip8TShNmYJmSnJiTg==";

            IJsEngineSwitcher engineSwitcher = JsEngineSwitcher.Current;
            engineSwitcher.EngineFactories.AddMsie(new MsieSettings {UseEcmaScript5Polyfill = true, UseJson2Library = true});
            engineSwitcher.DefaultEngineName = MsieJsEngine.EngineName;

            IControllerFactory factory = new CustomControllerFactory("SolEx.Hurt.Web.Site2.Controllers");
            ControllerBuilder.Current.SetControllerFactory(factory);

            GlobalConfiguration.Configure(WebApiConfig.Register);

            //sprawdzanie i aktualizacja bazy 
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujBaze(Assembly.GetExecutingAssembly());

            SolexBllCalosc.PobierzInstancje.Konfiguracja.UstalUstawieniaStartowe();        

            Calosc.Cache.PlikowyCache_wyczyscWszystkoStare( new string[] { "xmlapi" } );

            Calosc.Cache.InicjalizujPrywatnySlownik(new List<string> {"kategoriaBll_linki", "produktKlienta_linki_" + AccesLevel.Zalogowani, "produktKlienta_linki_" + AccesLevel.Niezalogowani, "lista-plikow" });

            //NIE WOLNO nic dawać zanim bindingi sie nie ustawia!!!             
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<HistoriaDokumentuPlatnosciOnline>(SposobCachowania.CalaLista, null, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<LogWpis>(SposobCachowania.Brak, null, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<HistoriaDokumentu>(SposobCachowania.Brak, null, null, null);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<HistoriaDokumentuProdukt>(SposobCachowania.Brak, null, null, null, SolexBllCalosc.PobierzInstancje.DokumentyDostep.UzupelnijPozycjePoSelect);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<DokumentyPozycje>(SposobCachowania.Brak, null, null, null, SolexBllCalosc.PobierzInstancje.DokumentyDostep.UzupelnijPozycjePoSelect);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ZamowieniaProduktyBLL>(SposobCachowania.Brak, null, null, null, SolexBllCalosc.PobierzInstancje.DokumentyDostep.UzupelnijPozycjePoSelect);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ZamowienieProdukt>(SposobCachowania.Brak, null, null, null, SolexBllCalosc.PobierzInstancje.DokumentyDostep.UzupelnijPozycjePoSelect);

            

            //TYMNCZSOWo pozwalamy na pobieranie dwoch roznych modeli DokumentyBll i HistoriaDokumentu - > jak najszybicej knwersja do JEDNEGO modelu
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<DokumentyBll>(SposobCachowania.Brak, null, null, null, SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzElementyPoSelect);
            
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<NewsletterKampania>(SposobCachowania.Brak, null, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<DzialaniaUzytkownikow>(SposobCachowania.Brak, null, null, null, SolexBllCalosc.PobierzInstancje.Statystyki.PobierzParametry);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<DzialaniaUzytkwonikowParametry>(SposobCachowania.Brak, null, null, null);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<StatusZamowienia>(SposobCachowania.CalaLista, null, null, null); //caLE STATUSY SA W KONfiguracji jako DICTIONARY

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ZamowieniaBLL>(SposobCachowania.Brak, (bll, klient) => bll.KlientId == klient.Id || klient.Role.Contains(RoleType.Administrator) || Sql.InSubquery(bll.KlientId, new SqlServerExpressionVisitor<Model.Klient>().Select(z => z.Id).Where(z => z.KlientNadrzednyId == klient.Id)), 
                 metodaPrzetwarzajacaPoSelect: SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzElementyPoSelect); // SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.SpradzCzyKlientMaPrawoDoZamowienia(bll, klient)

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<WiadomoscEmail>(SposobCachowania.Brak, null, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ProduktCecha>(SposobCachowania.Brak, null, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ProduktKategoria>(SposobCachowania.Brak, null, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<KoszykPozycje>(SposobCachowania.Brak, null, null, null, SolexBllCalosc.PobierzInstancje.Koszyk.UzupelnijPozycjePoSelect);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<RabatBLL>(SposobCachowania.Brak, null, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Rabat>(SposobCachowania.Brak, null, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Ustawienie>(SposobCachowania.CalaLista, null, null, null, Calosc.Konfiguracja.PoprawUstawieniaPoSelect );
            //SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Zamowienie>(SposobCachowania.Brak, null, null, null, true);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Zadanie>(SposobCachowania.Brak);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Magazyn>(SposobCachowania.CalaLista, null, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ProduktyZamienniki>(SposobCachowania.CalaLista, null, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ProduktUkryty>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ProfilKlienta>(SposobCachowania.Brak, null,null,null, SolexBllCalosc.PobierzInstancje.ProfilKlienta.SprawdzIPopraw);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<WidocznosciTypow>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Waluta>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Plik>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ProduktStan>(SposobCachowania.CalaLista);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<PoziomCenowy>(SposobCachowania.TylkoPojedyncze);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Obrazek>(SposobCachowania.ZakazanePobieranie);    //nalezy stosować PlikiBll.PobierzObrazek

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<SposobPokazywaniaStanowRegula>(SposobCachowania.Brak);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Jednostka>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ProduktJednostka>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<TrescWierszBll>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<TrescKolumnaBll>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<KatalogSzablonModelBLL>(SposobCachowania.CalaLista);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Sesja>(SposobCachowania.TylkoPojedyncze);

            //TODO: zabezpieczenie zeby nikt nie pobieral Grupa -> a tylko GrupaBLL + lokalizacje dla grupaBll
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Grupa>(SposobCachowania.ZakazanePobieranie);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<GrupaBLL>(SposobCachowania.CalaLista, (GrupaBLL, klient) => klient.CzyAdministrator || SolexBllCalosc.PobierzInstancje.KategorieDostep.JestWidocznaDlaKlienta(GrupaBLL, klient) );

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<FlatCeny>(SposobCachowania.Brak);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<KlientKategoriaKlienta>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<KategoriaKlienta>(SposobCachowania.Brak);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Konfekcje>(SposobCachowania.Brak);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<CenaPoziomu>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Tresc>(SposobCachowania.CalaLista);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Slajd>(SposobCachowania.CalaLista);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<UstawieniePowiadomienia>(SposobCachowania.Brak);
            

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<KategoriaProduktu>(SposobCachowania.ZakazanePobieranie);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<KategorieBLL>(SposobCachowania.CalaLista, (kategoria, klient) => klient.CzyAdministrator || SolexBllCalosc.PobierzInstancje.KategorieDostep.JestWidocznaDlaKlienta(kategoria, klient), null, null, SolexBllCalosc.PobierzInstancje.KategorieDostep.MetodaPrzetwarzajacaPoSelect );


            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<BlogWpisBll>(SposobCachowania.CalaLista, null, null, null, SolexBllCalosc.PobierzInstancje.Blog.BindingPoSelect);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Komunikaty>(SposobCachowania.CalaLista);


            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<KategoriaProduktu>(Calosc.KategorieDostep.UsunCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<KategorieBLL>(Calosc.KategorieDostep.UsunCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<TrescWierszBll>(Calosc.TresciDostep.WyczyscCacheWierszy);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUsunieciem<TrescWierszBll,object>(Calosc.TresciDostep.UsunCacheWierszy);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUsunieciem<Rejestracja, object>(Calosc.Konfiguracja.UsunZalacznikRejestracji);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Cecha>(SposobCachowania.CalaLista);
         //   SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<CechyBll>(SposobCachowania.ZakazanePobieranie);   //korzystać z PobierzWszystkieCechy

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<AtrybutBll>(SposobCachowania.CalaLista, null, null, null, Calosc.CechyAtrybuty.BindingAtrybutyPoSelect);
        //  SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Atrybut>(SposobCachowania.ZakazanePobieranie); //pobieramy zawsze atrybutyBll

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<SposobPokazywaniaStanow>(SposobCachowania.Brak, null, null, null,Calosc.SposobyPokazywaniaStanowBll.UzupelnijSposoby);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ProduktPlik>(SposobCachowania.CalaLista);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ProduktPlik>(SolexBllCalosc.PobierzInstancje.Pliki.BindPoAktualizacji);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Model.Klient>(SposobCachowania.Brak);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Klient>(SposobCachowania.TylkoPojedyncze, Calosc.Klienci.WalidatorKlientow, null, null, Calosc.Klienci.BindPoSelectKlieta);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect(SposobCachowania.Brak, null, Calosc.ZadaniaBLL.BindHarmonogram);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ModulSynchronizacji>(SposobCachowania.Brak, null, Calosc.ZadaniaBLL.BindModulySynchronizacji);
            
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<RodzinyCechyUnikalne>(SposobCachowania.Brak, null,null, null, Calosc.CechyAtrybuty.BindPoSelectCechUnikatowych);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect(SposobCachowania.Brak, null, SolexBllCalosc.PobierzInstancje.ZadaniaBLL.BindModulyPunktowe);


            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ModulKoszyka>(SposobCachowania.Brak, null, Calosc.ZadaniaBLL.BindModulyKoszyka);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ZadanieBll>(SposobCachowania.CalaLista, (Zadanie, klient) =>  Calosc.ZadaniaBLL.KlientMaDostep(Zadanie, klient),null,typeof(ModulStowrzonyNaPodstawieZadania), Calosc.ZadaniaBLL.BindingPoSelecie);

            //wylaczenie akceptacji koszykow
            //&& !Sql.InSubquery(koszyk.Id, new SqlServerExpressionVisitor<AkceptacjaKoszykow>().Select(z => z.KoszykId))


            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<KoszykBll>(SposobCachowania.TylkoPojedyncze, (koszyk, k) =>
            koszyk.KlientId == k.Id, null, null, SolexBllCalosc.PobierzInstancje.Koszyk.MetodaPrzetwarzajacaPoSelect);


            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ZamowieniaBLL>(SolexBllCalosc.PobierzInstancje.DokumentyDostep.UsunDokumentDlaUsunietegoZamowienia);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Tlumaczenie>(SposobCachowania.Brak, null, null, null);


            //SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<RodzinyCechyUnikalne>(SposobCachowania.Brak, null, null, null);


            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<Indywidualizacja>(SolexBllCalosc.PobierzInstancje.ProduktyBazowe.BindPoAktualizacjiIndywidualizacji);


            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Indywidualizacja>(SposobCachowania.Brak, null, null, null, Calosc.ProduktyBazowe.BindPoSelectIndywidualizacji);

            //typ lokalizacji zmieniony gdyz obiekt sie nie tlumaczył bo w metodzie StworzWpisDoSlownika brany był pod uwagę produkt bazowy
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ProduktBazowy>(SposobCachowania.CalaLista, null, null, typeof(ProduktBazowy), Calosc.ProduktyBazowe.MetodaPrzetwarzajacaPoSelect_UzupelnijProdutkBazowy);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<ProduktKlienta>(SposobCachowania.Brak, null, SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzProduktyKlientaZCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<SzablonLimitow>(SposobCachowania.CalaLista, (x, k) => x.Tworca == k.Id);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<SzablonAkceptacjiBll>(SposobCachowania.CalaLista, (x, k) => x.Tworca == k.Id);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<SubkontoGrupa>(SposobCachowania.CalaLista, (x, k) => x.KlientId == k.Id);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<MiejsceKosztow>(SposobCachowania.CalaLista, (x, k) => x.KlientId == k.Id);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<Adres>(SposobCachowania.CalaLista, (x, k) => x.AutorId == k.Id,null,null,Calosc.AdresyDostep.UzupelnijPozycjePoSelect);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<KlientAdres>(SposobCachowania.CalaLista, (x, k) => x.KlientId == k.Id);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ProduktBazowy>(SolexBllCalosc.PobierzInstancje.ProduktyBazowe.ZapiszZdjecieProduktu);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<HistoriaDokumentuListPrzewozowy>(SolexBllCalosc.PobierzInstancje.ListyPrzewozoweBll.WyslijPowiadomienie);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<Model.Klient>(SolexBllCalosc.PobierzInstancje.Klienci.UsunCacheKlienci);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<Klient>(SolexBllCalosc.PobierzInstancje.Klienci.UsunCacheKlienci);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<Pracownik>(SolexBllCalosc.PobierzInstancje.Klienci.UsunCacheKlienci);


            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<Waluta>(SolexBllCalosc.PobierzInstancje.Konfiguracja.UsunCacheWalut);

            //nowi kliencie do statystyk
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdateRozroznienieStareIAktualizowane<Model.Klient>((starzy, nowi) => Calosc.Statystyki.AktualizacjaKlientow_RozpoznanieNowychKlientow(starzy.Select(x =>  new Klient(x) as IKlient).ToList(), nowi.Select(x => new Klient(x) as IKlient).ToList()) );
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdateRozroznienieStareIAktualizowane<Klient>((starzy, nowi) => Calosc.Statystyki.AktualizacjaKlientow_RozpoznanieNowychKlientow(starzy.Select(x=> x as IKlient).ToList(), nowi.Select(x => x as IKlient).ToList()));


            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<BlogWpisBll>(SolexBllCalosc.PobierzInstancje.Blog.AktualizujLacznikiKategorii);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUpdate<Model.Klient>(SolexBllCalosc.PobierzInstancje.Klienci.PrzedAktualizacjaKlientow);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUpdate<Klient>(SolexBllCalosc.PobierzInstancje.Klienci.PrzedAktualizacjaKlientow);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUpdate<Pracownik>(SolexBllCalosc.PobierzInstancje.Klienci.PrzedAktualizacjaPracownikow);


            //Dzialania Uzytkownikow
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<DzialaniaUzytkownikow>(SolexBllCalosc.PobierzInstancje.Statystyki.DodajParametry);

            //Koszyk
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<KoszykBll>(SolexHelper.WyczyscObiektKoszykaZSolexHelpera );


            //sklepy
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUpdate<SklepyBll>(SolexBllCalosc.PobierzInstancje.Sklepy.DodajAdresDoSklepu);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUsunieciem<SklepyBll,long>(SolexBllCalosc.PobierzInstancje.Sklepy.UsunAdresSklepu);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<SklepyBll>(SolexBllCalosc.PobierzInstancje.Sklepy.ZarzadzajLacznikamiKategoriiSklepu);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<SklepyBll>(SposobCachowania.CalaLista, null, null);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<KategoriaSklepu>(SposobCachowania.CalaLista, null, null);
          
            //cechy
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUpdate<AtrybutBll>(SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PrzedAktualizacjaAtrybutow);
            //tresci
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUpdate<TrescBll>(SolexBllCalosc.PobierzInstancje.TresciDostep.Sprawdz);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect(SposobCachowania.CalaLista, null, SolexBllCalosc.PobierzInstancje.Klienci.PobierzPracownikow); //pobiera listę pracowników
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect(SposobCachowania.CalaLista, null, SolexBllCalosc.PobierzInstancje.Klienci.PobierzAdministratorow); //pobiera listę administratorów
            SolexBllCalosc.PobierzInstancje.DostepDane.BindSelect<TrescBll>(SposobCachowania.CalaLista, (bll, klient) => SolexBllCalosc.PobierzInstancje.TresciDostep.SprawdzDostep(bll, klient));

            //Magazyny
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUpdate<Magazyn>(SolexBllCalosc.PobierzInstancje.ProduktyStanBll.UstawMagazynRealizujacy);

            //Newsleter
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUpdate<NewsletterKampania>(SolexBllCalosc.PobierzInstancje.MaileBLL.PrzedAktualizacjaNewslettera);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUsunieciu<HistoriaDokumentu,int>(obj=>SolexBllCalosc.PobierzInstancje.DokumentyDostep.UsunZamowienieDokument(obj));

            //SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUsunieciem<HistoriaDokumentu>(SolexBllCalosc.PobierzInstancje.DokumentyDostep.ZmienStatusZamowien);

            //***********************************************
            //***********************************************
            //--------------  USUWANIE CACHE --------------
            //***********************************************

            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ProduktKategoria>(SolexBllCalosc.PobierzInstancje.ProduktyKategorieDostep.WyczyscCacheKategorii);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ModulSynchronizacji>(SolexBllCalosc.PobierzInstancje.ZadaniaBLL.UsunCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<CenaPoziomu>(SolexBllCalosc.PobierzInstancje.CenyPoziomy.UsunCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<Adres>(SolexBllCalosc.PobierzInstancje.AdresyDostep.AktualizujLacznikiKlientow);
           
            //atrybuty i cechy
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<AtrybutBll>(SolexBllCalosc.PobierzInstancje.CechyAtrybuty.UsunCacheAtrybutyICechy);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<CechyBll>(SolexBllCalosc.PobierzInstancje.CechyAtrybuty.UsunCacheAtrybutyICechy);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<CechyBll>(SolexBllCalosc.PobierzInstancje.ProduktyUkryteBll.WyczyszCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<CechyBll>(SolexBllCalosc.PobierzInstancje.CechyProduktyDostep.CzyscCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<CechyBll>(SolexBllCalosc.PobierzInstancje.Rabaty.UsunCache);

            //ProduktUkryty
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ProduktUkryty>(SolexBllCalosc.PobierzInstancje.ProduktyUkryteBll.WyczyszCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<KategoriaKlienta>(SolexBllCalosc.PobierzInstancje.ProduktyUkryteBll.WyczyszCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<KategoriaKlienta>(SolexBllCalosc.PobierzInstancje.Rabaty.UsunCache);

            //Stany
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ProduktStan>(SolexBllCalosc.PobierzInstancje.ProduktyStanBll.UsunCache);

            //ProduktCecha
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ProduktCecha>(SolexBllCalosc.PobierzInstancje.CechyProduktyDostep.CzyscCache);

            //maile
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUsunieciu<WiadomoscEmail,long>(obj => SolexBllCalosc.PobierzInstancje.MaileBLL.UsunCache(obj));

            //rabaty
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<RabatBLL>(SolexBllCalosc.PobierzInstancje.Rabaty.UsunCache);

            //koszyk
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ModulKoszyka>(SolexBllCalosc.PobierzInstancje.ZadaniaBLL.UsunCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ModulPunktowy>(SolexBllCalosc.PobierzInstancje.ZadaniaBLL.UsunCache);
            //jednostki
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPrzedUpdate<Jednostka>(SolexBllCalosc.PobierzInstancje.JednostkaProduktuBll.PoprawJednostkiPrzedAktualizacja);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<Jednostka>(SolexBllCalosc.PobierzInstancje.JednostkaProduktuBll.UsunCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<ProduktJednostka>(SolexBllCalosc.PobierzInstancje.JednostkaProduktuBll.UsunCache);
            //pliki
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<Plik>(SolexBllCalosc.PobierzInstancje.Pliki.UsunCache);
            //Harmonogram 
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<HarmonogramBll>(SolexBllCalosc.PobierzInstancje.ZadaniaBLL.UsunCache);
            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<Zadanie>(SolexBllCalosc.PobierzInstancje.ZadaniaBLL.UsunCache);

            SolexBllCalosc.PobierzInstancje.DostepDane.BindPoUpdate<StatusZamowienia>(SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.UsunStatusyCache);

            // ----------------------- RESZTA --------------------------
         
            SolexBllCalosc.PobierzInstancje.Konfiguracja.SprawdzJezyki();

            //todo:klient niezalogowany w synchronziacji trzeba dodać bo wylatuje pewnie non stop
            //brakujacy klient niezalogowany
            if (SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Model.Klient>(0, SolexBllCalosc.PobierzInstancje.Klienci.SztucznyAdministrator() ) == null)
            {
                Waluta waluta = new Waluta("pln".WygenerujIDObiektuSHAWersjaLong(1), "PLN", "PLN") { JezykId = Calosc.Konfiguracja.JezykIDDomyslny };
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(waluta);

                Model.Klient klient = SolexBllCalosc.PobierzInstancje.Klienci.KlientNiezalogowany();
                SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Insert<Model.Klient>(klient);  //nie mozemy nazego ORM bo klucz 0 musi wejść
            }
            

            log4net.Config.XmlConfigurator.Configure();
            Log.Warn("Start aplikacji");

            SolexBllCalosc.PobierzInstancje.CechyAtrybuty.UstawSlownikiMetek();
            Refleksja.ZaladujAssembly();
          
            SolexBllCalosc.PobierzInstancje.Klienci.WygenerujKluczeAdministratorom();
            SolexBllCalosc.PobierzInstancje.Klienci.WygenerujBrakujaceLoginy();           

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ViewEngines.Engines.Clear();
            var ve = new WidokiWeWlasnymKataloguKlienta();
            ViewEngines.Engines.Add(ve);

            var typy = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(IBindowalny));
            foreach (var type in typy)
            {
                ModelBinders.Binders[type] = new UniwersalnyBinder();
            }

            //ModelBinders.Binders.Add(typeof(object[]), new ObjectArrayBinder());
            ModelBinders.Binders.Add(typeof(OpisPolaObiektu), new OpisPolaObiektuBinder());

            ModelBinders.Binders.Add(typeof(decimal), new DecimalBinder());
            ModelBinders.Binders.Add(typeof(Type), new TypeBinder());
            ModelBinders.Binders[typeof(MapowanePola)] = new AktualizowanePolaBinder();
           // ModelBinders.Binders[typeof(TlumaczenieDoEdycji)] = new TlumaczenieDoEdycjiBinder();

           // ModelBinders.Binders[typeof(FiltrListaProduktow[])] = new FiltrListaProduktowKolekcjaBinder();
            ModelBinders.Binders[typeof(ParametryPrzekazywaneDoListyProduktow)] = new ParametryPrzekazywaneDoListyProduktowBinder();
            ModelBinders.Binders[typeof(Sortowanie)] = new SortowanieBinder();
            ModelBinders.Binders[typeof(DaneEdycjaAdmin)] = new DaneEdycjaAdminBinder();

            //poprawki do bazy danych
            SolexBllCalosc.PobierzInstancje.ZadaniaBLL.SprawdzZadaniaSystemowe();
            SolexBllCalosc.PobierzInstancje.Sklepy.AktualizujSklepMapaSklepow();
            //SolexBllCalosc.PobierzInstancje.ProduktyStanBll.UzupelnijStany();
            
            SolexBllCalosc.PobierzInstancje.CechyAtrybuty.GenerujStandardoweCechy();
           
            SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.SprawdzStatusy_UtworzStatusySystemowe();
           
            SolexBllCalosc.PobierzInstancje.ZadaniaBLL.UsunZdublowaneModulySystemowe();
            ImageResizerConfigure.UstawWatermark();
            TemplateBLL.WczytajDomyslneSzablony();
            SolexBllCalosc.PobierzInstancje.SposobyPokazywaniaStanowBll.UtworzaSzablonyRegul();

            SolexBllCalosc.PobierzInstancje.MaileBLL.InicjalizacjaPowiadomien();
            AreaRegistration.RegisterAllAreas();

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SolexBllCalosc.PobierzInstancje.TresciDostep.DodajDomyslneTresci();

            StworzMenuAdmina();
            
            //podlaczanie pod zdarzenia emaili
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_GenerowanieKluczaApi += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieOZmienieKluczaApi;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_NowyKlient += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieNowyKlient;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_Rejestracj += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieNowaRejestracja;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_PobranieFaktury += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomieniePobranieFaktury;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_ResetHasla += WyslijPowiadomienia.PobierzInstancje.WyslijMailaResetHaslaKlienta;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_ProsbaOInformacjeODostepnosci += WyslijPowiadomienia.PobierzInstancje.WyslijProsbeOInformacjeODostepnosci;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_NoweDokumenty += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieNoweDokumenty;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_PojawienieSieProduktow += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomieniePojawienieSieProduktow;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_PrzeterminowanePlatnosc += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomieniePrzeterminowanePlatnosc;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_ZapisDoNewslettera += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieOZapisieDoNewslettera;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_NoweListyPrzewozowe += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieOListachPrzewozowych;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_ZmianaTerminuRealizacjiZamowienia += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieOZmianieTerminuRealizacjiZamowienia;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_ZmianaStatusDokumentu += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieZmianieStatusuDokumentu;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_PowitanieSzef += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomieniePowitanieSzef;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_ZmianaAdresuIP += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieZmianaAdresuIP;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_NoweProduktyWSystemie += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieNoweProduktyWSystemie;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_WysylanieFormularza += WyslijPowiadomienia.PobierzInstancje.WysylanieFormularza;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_ProduktyPrzyjeteNaMagazyn += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieProduktyPrzyjeteNaMagazyn;

            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.KiedyWysylacMailaOZamowieniu == KiedyWysylacMailaOZamowieniu.WMomencieFinalizacjiKoszyka)
            {
                SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_NoweZamowienie_Finalizacja += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieNoweZamowienie;
            }

            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.KiedyWysylacMailaOZamowieniu == KiedyWysylacMailaOZamowieniu.PoImporcieDoSystemuErp)
            {
                SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_NoweZamowienie_PoImporcieDoERP += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieNoweZamowienie;
            }

            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_BladImportu += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieBladImportu;


            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_ZamowienieOdrzucone += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieSubkonta_ZamowienieOdrzucone;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_ZamowienieDoAkceptacji += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieSubkonta_ZamowienieDoAkceptacji;
            SolexBllCalosc.PobierzInstancje.Statystyki.zdarzenie_ZamowienieZaakceptowane += WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieSubkonta_ZamowienieZaakceptowane;

            Application_Start_RazDziennie();
        }

        private void StworzMenuAdmina()
        {
            AdminHelper.PobierzInstancje.DodajaEdycjePolAdmin<ZadanieBll>(AdminHelper.PobierzInstancje.PolaDoEdycjiKolumna, "ZapiszObiektBedacyPojemnikiem");
            //AdminHelper.PobierzInstancje.DodajaEdycjePolAdmin<SposobPokazywaniaStanowRegula>(AdminHelper.PobierzInstancje.PolaDoEdycjiKolumna, "ZapiszObiektReguly");

            //Belka główna
            AdminHelper.PobierzInstancje.DodajGrupeMenu("Punkty", 8);
            AdminHelper.PobierzInstancje.DodajGrupeMenu("Systemowe");
            AdminHelper.PobierzInstancje.DodajGrupeMenu("Synchronizacja", 1);
            AdminHelper.PobierzInstancje.DodajGrupeMenu("Treści", 2);
            AdminHelper.PobierzInstancje.DodajGrupeMenu("Klienci", 3);
            AdminHelper.PobierzInstancje.DodajGrupeMenu("Produkty", 4);
            AdminHelper.PobierzInstancje.DodajGrupeMenu("Koszyk", 5);
            AdminHelper.PobierzInstancje.DodajGrupeMenu("Wiadomości email", 6);
            AdminHelper.PobierzInstancje.DodajGrupeMenu("Rabaty", 7);
            
            AdminHelper.PobierzInstancje.DodajGrupeMenu("Newslettery");
            //Synchronizacja
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<KrajeBLL>("Kraje", "Synchronizacja", 1);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Waluta>("Waluty", "Synchronizacja", 3, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Magazyn>("Magazyny", "Synchronizacja", 2, klient => true,x=>true,x=>true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<HarmonogramBll>("Harmonogram", "Synchronizacja", 4, klient => true, x => false, x => false, klient => false);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<ModulSynchronizacji>("Moduły synchronizacji", "Synchronizacja", 4, klient => true, x => x.CzyAdministrator, x => x.CzyAdministrator, klient => klient.CzyAdministrator,
            AdminHelper.PobierzInstancje.PolaDoEdycjiKolumna, "ZapiszObiektBedacyPojemnikiem","WybierzModul");
        //    AdminHelper.PobierzInstancje.DodajaEdycjePolAdmin<TrescKolumnaBll>(AdminHelper.PobierzInstancje.PolaDoEdycjiModulu);

            //Treści
            AdminHelper.PobierzInstancje.DodajWlasnaAkcjeDoMenu("Treść", "/Admin/Tresci", "Treści", null);
            AdminHelper.PobierzInstancje.DodajaEdycjePolAdmin<TrescBll>(AdminHelper.PobierzInstancje.PolaDoEdycjiTresc);
            AdminHelper.PobierzInstancje.DodajaEdycjePolAdmin<TrescKolumnaBll>(AdminHelper.PobierzInstancje.PolaDoEdycjiKolumna, "ZapiszObiektBedacyPojemnikiem");

            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<TlumaczeniePole>("Tłumaczenia", "Treści", 1, klient =>true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Slajd>("Slajdy", "Treści", 2, klient => true, x => true, x => true); 
            //Blog
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<BlogWpisBll>("Wpisy Bloga", "Treści", 3, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<BlogKategoria>("Kategorie Bloga", "Treści", 4, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<BlogGrupa>("Grupy blogów", "Treści", 5, klient => true, x => true, x => true);
            //Klienci
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<KategoriaKlienta>("Kategorie klientów", "Klienci", 1, klient =>true, null, null, klient => false);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<KategoriaSklepu>("Kategorie sklepów", "Klienci", 2, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Klient>("Klienci", "Klienci", 3, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Rejestracja>("Rejestracje", "Klienci", 4, klient => true, x => true, x => false);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<SklepyBll>("Sklepy", "Klienci", 5);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<ProfilKlienta>("Profile klientów", "Klienci", 6, klient => true, null, klient => false);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Komunikaty>("Komunikaty", "Klienci", 7, klient => true, x => true, x => true);

            //Produkty
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<AtrybutBll>("Atrybuty", "Produkty", 1, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<CechyBll>("Cechy", "Produkty", 2, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<GrupaBLL>("Grupy produktów", "Produkty", 3, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Jednostka>("Jednostki", "Produkty", 4);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<PoziomCenowy>("Poziomy cenowe", "Produkty", 6, klient => true, null, x => false);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<ProduktBazowy>("Produkty", "Produkty", 7, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<KategorieBLL>("Kategoria produktu", "Produkty", 8, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<SposobPokazywaniaStanow>("Sposób pokazywanie stanów", "Produkty", 9, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Indywidualizacja>("Parametry indywidualizacji", "Produkty", 10, klient => true, x => true, x => true);

            //Maile
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<WiadomoscEmail>("Historia maili", "Wiadomości email", 0, klient =>true, x => false, x => false, klient => false);
            AdminHelper.PobierzInstancje.DodajWlasnaAkcjeDoMenu("Szablony maili", "/Admin/SzablonyMaili", "Wiadomości email", null,2);

            //Koszyk
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<StatusZamowienia>("Statusy dokumentów", "Koszyk", 3);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<KatalogSzablonModelBLL>("Szablony wydruku katalogów", "Produkty", 4, x => x.CzyAdministrator, x => x.CzyAdministrator, x => x.CzyAdministrator, x => x.CzyAdministrator);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<ZamowieniaBLL>("Zamówienia", "Koszyk", 5, klient => true, x => false, x => false, klient => false);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<DokumentyBll>("Dokumenty z ERP", "Koszyk", 5, klient => true, x => false, x => false, klient => false);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<ModulKoszyka>("Moduły koszyka", "Koszyk", 6,  klient =>true, x => x.CzyAdministrator, x => x.CzyAdministrator, klient => klient.CzyAdministrator,
                          AdminHelper.PobierzInstancje.PolaDoEdycjiKolumna, "ZapiszObiektBedacyPojemnikiem", "WybierzModul");
            //Systemowe
           // AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<AdministratorBLL>("Administratorzy", "Systemowe");
            AdminHelper.PobierzInstancje.DodajWlasnaAkcjeDoMenu("Blokada systemu", "/Admin/BlokadaSystemu", "Systemowe", null, 1);
            AdminHelper.PobierzInstancje.DodajWlasnaAkcjeDoMenu("Edytuj pliki systemowe", "/Admin/PlikiDoEdycji", "Systemowe", null, 2);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Jezyk>("Języki", "Systemowe", 3, klient => true, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<LogWpis>("Logi", "Systemowe", 4, klient =>true, x => false, x => false);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Pracownik>("Pracownicy", "Systemowe", 5, null, x => true, x => true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<DzialaniaUzytkownikow>("Statystyki", "Systemowe", 6, x=>x.CzyAdministrator,x => false, x => false);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<SzablonyEdytorow>("Szablony", "Newslettery", 7, null, klient => true, klient => true, klient => true  );
            AdminHelper.PobierzInstancje.DodajWlasnaAkcjeDoMenu("Test konfiguracji", "/Admin/StatusKonfiguracji", "Systemowe", null, 8);
            AdminHelper.PobierzInstancje.DodajWlasnaAkcjeDoMenu("Ustawienia", "/Admin/Ustawienia", "Systemowe", null, 9);
            AdminHelper.PobierzInstancje.DodajWlasnaAkcjeDoMenu("Pliki", "/Pliki/MenadzerPlikow/m", "Systemowe", null, 10);

            //Rabaty
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<RabatBLL>("Rabaty zaawansowane i promocje", "Rabaty", 2);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<Konfekcje>("Gradacje", "Rabaty", 3, walidatorEdycji: klient => false);


            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<ModulPunktowy>("Reguły punktowe", "Punkty", 1, klient =>true, x => x.CzyAdministrator, x => x.CzyAdministrator, klient => klient.CzyAdministrator,
            AdminHelper.PobierzInstancje.PolaDoEdycjiKolumna, "ZapiszObiektBedacyPojemnikiem", "WybierzModul");

            //Newslettery
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<NewsletterKampania>("Newslettery", "Newslettery", 0, klient => true, x => true, x=> true);
            AdminHelper.PobierzInstancje.DodajAutomatycznaListeObiektow<NewsletterZapisani>("Niezalogowani zapisani do newslettera", "Newslettery", 1, klient =>true, x => false, x => false);
        }

        public Jezyk ProbojPobracJezykZLinkaRequestu(Uri url)
        {
            Jezyk jakiJezyk = null;
            //czy sa w ogole jezyki w systemie
            if (Calosc.Konfiguracja.JezykiWSystemieSlownikPoSymbolu.Count > 1 && url.Segments.Length > 1)
            {
                //jezyk zawsze jest w PIERWSZYM segmencie
                bool udaloSieDopasowacJezyk = Calosc.Konfiguracja.JezykiWSystemieSlownikPoSymbolu.TryGetValue(url.Segments[1].ToLower().TrimEnd('/'), out jakiJezyk);
                if (!udaloSieDopasowacJezyk)
                {
                    //NIC nie robimy - wezmiemy pozniej z klienta bo jezyk bedzie NULL
                }
            }
            return jakiJezyk;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.IsLocal)
            {
                BundleTable.EnableOptimizations = false;
                MiniProfiler.StartNew();
            }
            else
            {
                BundleTable.EnableOptimizations = true;
            }

            var ostatniSegment = Request.Url.Segments.Last();
            if (!ostatniSegment.EndsWith(".scss") && !ostatniSegment.EndsWith(".js") && !ostatniSegment.EndsWith(".css"))
            {
                Jezyk jakiJezyk = this.ProbojPobracJezykZLinkaRequestu(Request.Url);

                SolexHelper solexHelper = new SolexHelper(jakiJezyk);
                SolexHelper.UtworzNowy(solexHelper);

                if (HttpContext.Current.Request.RawUrl.Contains("czysc-stale-filtry"))
                {
                    IKlient k = solexHelper.AktualnyKlient;

                    if (k != null)
                    {
                        SolexBllCalosc.PobierzInstancje.ProfilKlienta.UsunWartosc(k, TypUstawieniaKlienta.StalyFiltr);
                    }
                }
                log4net.ThreadContext.Properties["sesja-user-id"] = solexHelper.AktualnyKlient.Id;
                log4net.ThreadContext.Properties["url-referer"] =  Request.UrlReferrer;
                log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                log4net.ThreadContext.Properties["url"] = Request.Url?.ToString();

            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            MiniProfiler.Current?.Stop();

            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items["dbpolaczenie"] != null)
                {
                    ((IDbConnection) HttpContext.Current.Items["dbpolaczenie"]).Dispose();
                    HttpContext.Current.Items["dbpolaczenie"] = null;
                }

                HttpContext.Current.Items["htmlhelper"] = null;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException(); 

            //nie zapisujemy prostych błędów
            if (ex.Message == "File does not exist." || ex.Message.Contains("A potentially dangerous Request.Path value was detected from the client") )
            {
                Log.Warn(ex);
                return;
            }
            if (ex is OutOfMemoryException)
            {
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }

            //nie zapisujemy prostych błędów
            if (ex is HttpException && ((ex as HttpException).GetHttpCode() == 404 || (ex as HttpException).GetHttpCode() == 403 || (ex as HttpException).GetHttpCode() == 401  ))
            {
                Log.Warn(ex);
                return;
            }

            //nie chcemy loga o braku favicona
            if (ex.Message.Contains("favicon.ico"))
            {
                return;
            }
       

            if (ex is System.Data.SqlClient.SqlException)
            {
                    if (!string.IsNullOrEmpty(SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.GetLastSql()))
                    {
                        Log.Info("Ostatni SQL: " + SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.GetLastSql());
                    }
                return;
            }
            Log.Error(ex);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Log.Warn("Wyłączanie aplikacji");
            HttpRuntime runtime = (HttpRuntime)typeof(HttpRuntime).InvokeMember("_theRuntime",
                                                                                    BindingFlags.NonPublic
                                                                                    | BindingFlags.Static
                                                                                    | BindingFlags.GetField,
                                                                                    null,
                                                                                    null,
                                                                                    null);

            string shutDownMessage = (string)runtime.GetType().InvokeMember("_shutDownMessage",
                                                                    BindingFlags.NonPublic
                                                                    | BindingFlags.Instance
                                                                    | BindingFlags.GetField,
                                                                    null,
                                                                    runtime,
                                                                    null);

            string shutDownStack = (string)runtime.GetType().InvokeMember("_shutDownStack",
                                                                           BindingFlags.NonPublic
                                                                           | BindingFlags.Instance
                                                                           | BindingFlags.GetField,
                                                                           null,
                                                                           runtime,
                                                                           null);

            if (!EventLog.SourceExists(".NET Runtime"))
            {
                EventLog.CreateEventSource(".NET Runtime", "Application");
            }
            if (shutDownMessage.Contains("User code called"))
            {
                //sam user chcial - nie piszemy tego
                return;
            }
            Log.WarnFormat($"Wyłączanie aplikacji - powód: {shutDownMessage}, {shutDownStack}");

            EventLog log = new EventLog {Source = ".NET Runtime"};
            string msg = $"\r\n\r\n_shutDownMessage={shutDownMessage}\r\n\r\n_shutDownStack={shutDownStack}";
            log.WriteEntry(msg.Substring(0,msg.Length>32700? 32700:msg.Length),EventLogEntryType.Error);
        }

        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            //if (arg.IndexOf("Kategorie", StringComparison.InvariantCultureIgnoreCase) >= 0)
            //{
                return arg+ DateTime.Now.ToString("HHmmssfff");
            //}
            //return base.GetVaryByCustomString(context, arg);
        }
    }
}
