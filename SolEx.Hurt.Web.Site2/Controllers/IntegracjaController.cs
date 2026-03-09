using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ServiceStack.Common;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.Text;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Bindowania;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci;
using SolEx.Hurt.Web.Site2.PageBases;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Export;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("xmlapi")]
    public class IntegracjaController : SolexControler
    {
        public enum KodowaniePlikowIntegracji
        {
            UTF8,
            Windows1250
        }

        private static object lokPlikiIntegracja = new object();

        /// <summary>
        /// wewnetrzna metoda renderuje dane na podstawie szablonu i danych wejściowych - dane sa cachowane plikowo
        /// </summary>
        /// <param name="szablonIntegracji">szablon inntegracji do pobrania</param>
        /// <param name="wersja"></param>
        /// <param name="klient"></param>
        /// <param name="dane">opcjonalne dane z zewnątrz - jeśli null metoda sama pobierze dane</param>
        /// <param name="exfilters">cechy id których produkty nie mogą mieć - tylko dla produktów</param>
        /// <param name="filters">cechy id które produkty mają mieć - tylko dla produktów</param>
        /// <param name="czasCache">czas trzymania w cache - jeśli 0 cache nie jest wykorzystywany. Cache jest trzymany jako pliki na dysku - nie w pamięci. Czas w sekundach</param>
        /// <returns></returns>
        [NonAction]
        private string PobierzPlik_strumien(PlikIntegracjiSzablon szablonIntegracji, int wersja, IKlient klient = null, object dane = null, long[] exfilters = null, long[] filters = null, int czasCache = 1800)
        {
            if (dane != null && (exfilters != null || filters != null) )
            {
                throw new Exception("Do metody podano dane - w takim wypadku nie można filtrować wg. parametrów exfilter i filter. Dane wejściowe muszą być już przefiltrowan.");
            }

                if (klient == null)
                {
                    klient = SolexHelper.AktualnyKlient;
                }

                //wyalczenie cache dla localhostow i tstow
                if (Request.IsLocal || klient.CzyAdministrator)
                {
                    czasCache = 0;
                }
                
                //sprawdzamy czy juz jest cache o tym kluczu - specjalnie lok po to zeby pliki generowaly sie w kolejce bo klienci puszczaja po 100 plikow narazi  zazyna nam serwer
                StringBuilder kluczCache = new StringBuilder("");

                //budowanie klucza
                if (czasCache > 0)
                {
                    if (exfilters != null)
                    {
                        kluczCache.Append($"_ex{exfilters.OrderBy(x => x).Join("_")}");
                    }
                    if (filters != null)
                    {
                        kluczCache.Append($"_f{filters.OrderBy(x => x).Join("_")}");
                    }

                    if (dane != null)
                    {
                        //obiekty musza dziedziczyc po IhasLongId lub ParametrDoKataloguProduktow
                        if (dane is ParametrDoKataloguProduktow)
                        {
                            kluczCache.Append($"_ids{(dane as ParametrDoKataloguProduktow).ListaProduktow.Select(x => x.Id).OrderBy(x => x).Join("_")}");
                            kluczCache.Append($"_ids{(dane as ParametrDoKataloguProduktow).OpisWpisanyPrzezKlienta.WygenerujIDObiektuSHAWersjaLong()}");
                        }
                        else
                        {
                            try
                            {
                                kluczCache.Append($"_ids{(dane as IEnumerable<IHasLongId>).Select(x => x.Id).OrderBy(x => x).Join("_")}");
                            } catch
                            {
                                Log.FatalFormat("Nie udało się iterować po liście obiektów IHasLongId - prawdopodobnie obiekty nie dziedziczą po tym interfejsie");
                                throw;
                            }
                        }
                    }

                    if (kluczCache.Length > 150)
                    {
                        kluczCache = new StringBuilder(kluczCache.WygenerujIDObiektuSHAWersjaLong().ToString());
                    }

                    kluczCache.Insert(0, $"{szablonIntegracji.IdSzablonu}_{wersja}_{klient.Id}");
                }

                return Calosc.Cache.PlikowyCache_PobierzObiekt(() =>
                {
                    try
                    {
                        if (dane == null)
                        {
                            switch (szablonIntegracji.typDanych)
                            {
                                case TypDanychIntegracja.Produkty:
                                    dane = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktKlienta>(SolexHelper.AktualnyJezyk.Id, klient);
                                    //są filtry
                                    if (dane != null && exfilters != null || filters != null)
                                    {
                                        dane = (dane as IList<ProduktKlienta>).Where(x => (filters != null && x.IdCechPRoduktu.Overlaps(filters)) || (exfilters != null && !x.IdCechPRoduktu.Overlaps(exfilters))).ToList();
                                    }
                                    break;
                                case TypDanychIntegracja.Sklepy:
                                    dane = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SklepyBll>(SolexHelper.AktualnyJezyk.Id, klient);
                                    break;
                            }
                        }

                        if (dane == null)
                        {
                            throw new Exception("Brak danych do wyrenderowania szablonu");
                        }

                        string strumien = this.ViewToString(szablonIntegracji.PobierzWidokPliku(wersja), "_LayoutGenerowaniePliku", dane);
                        return strumien;
                    } catch (OutOfMemoryException)
                    {
                        //kompaktuj po wykorzystaniu integracji LOHa - musimy zrobić miejsce dla stringa w pamieci
                        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                        GC.Collect();
                        throw;
                    } finally
                    {
                        //czyszczenie smieci - zaczytanych produktow klienta - po api czyscimy cache z produktow klienta - moze tu byc balagan do nieczego nam nie potrzebny juz
                       // Calosc.ProduktyKlienta.WyczyscCacheProduktyKlienta(SolexHelper.AktualnyKlient);
                    }
                }, lokPlikiIntegracja, SolexHelper.AktualnyJezyk.Id, kluczCache.ToString(), "xmlapi", czasCache);
        }

        public class UstawieniaWidokuListySzablonowIntegracji
        {
            public Dictionary<TypDanychIntegracja, List<PlikIntegracjiSzablon>> Szablony { get; set; }
            public string Opis { get; set; }
        }

        [ChildActionOnly]
        public ActionResult ListaPlikowIntegracji(int id)
        {
            Integracja kontrolka = this.PobierzKontrolke<Integracja>(id);

            if (Calosc.Konfiguracja.PlikiIntegracjiAktywne_DoPokazania == null || Calosc.Konfiguracja.PlikiIntegracjiAktywne_DoPokazania.IsEmpty())
            {
                Log.Fatal("Brak aktywnych integracji - sprawdz ustawienie 'Integracja - pliki integarcji możliwe do pobierania przez klientów'");
                return Content("Brak integracji do pokazania - zobacz logi.");
            }

            Dictionary<TypDanychIntegracja, List<PlikIntegracjiSzablon>> listaPlikowDoPokazania = Calosc.Konfiguracja.TablicaPlikowIntegracjiWgId.WhereKeyIsInDoSlownika(Calosc.Konfiguracja.PlikiIntegracjiAktywne_DoPokazania).Select(x=> x.Value).GroupBy(x => x.typDanych).ToDictionary(x => x.Key, x => x.ToList());
           
            if (listaPlikowDoPokazania.IsEmpty())
            {
                return Content("W tej chwili nie są dostępne żadne formaty integracji");
            }

            return PartialView("_ListaPlikow", new UstawieniaWidokuListySzablonowIntegracji{Szablony =  listaPlikowDoPokazania, Opis = kontrolka.OpisIntegracjiDlaKlienta} );
        }

        [Route("format/{szablonId}/{wersja}/m")]
        public ActionResult SzczegolyFormatu(int szablonId, int wersja)
        {
            PlikIntegracjiSzablon szablon = this.pobierzSzablonIntegracjiID(szablonId);

            ViewBag.Plik = szablon;
            ViewBag.Wersja = wersja;
            string widok = szablon.PobierzWidokPliku(wersja);
            return View(widok, "_LayoutSzczegolyFormatu");
        }

        /// <summary>
        /// zalaldowanie szablonu na podstaiwe id - wyrzyca wyjatek gdy jest problem
        /// </summary>
        /// <param name="szablonID"></param>
        /// <returns></returns>
        protected PlikIntegracjiSzablon pobierzSzablonIntegracjiID(int szablonID)
        {
            PlikIntegracjiSzablon szablon = null;

            if (Calosc.Konfiguracja.TablicaPlikowIntegracjiWgId.TryGetValue(szablonID, out szablon))
            {
                return szablon;
            }

            throw new Exception("Brak szablonu pliku integracji o id: " + szablonID);
        }

        /// <summary>
        /// metoda do odczytania szablonu z nazwy zrodla danych z raportu
        /// </summary>
        /// <param name="nazwaZrodla"></param>
        /// <param name="wersja"></param>
        /// <returns></returns>
        protected PlikIntegracjiSzablon pobierzSzablonIntegracjiWgNazwyZrodlaDanychKatalogu(string nazwaZrodla, out int wersja)
        {
            string[] tablica = nazwaZrodla.Split('.');

            if (!tablica[0].StartsWith("danetestowekatalogu-solexb2b"))
            {
                wersja = 0;
                return null;  //to nie nasze
            }

            int szablonId;
            try
            {
                szablonId = int.Parse(tablica[2]);
                wersja = int.Parse(tablica[1]);
            }
            catch
            {
                Log.ErrorFormat($"Nie udało się odczytać szablonu z raportu. Prawdopodobnie używałeś starego szablonu - podmień szablony danych. Błędny szablon: {nazwaZrodla}");
                throw;
            }

            return this.pobierzSzablonIntegracjiID(szablonId);
        }

        [Route("gen-client-key")]
        public ActionResult GenerowanieKluczaAktualnegoKlienta()
        {
            try
            {
                SolexBllCalosc.PobierzInstancje.Klienci.WygenerujKlucz(SolexHelper.AktualnyKlient as Klient);
            } catch (Exception e)
            {
                Log.Error(e);
                return Content("Nie udało się stworzyć klucza API");
            }
            return PartialView("_generowanieKlucza");
        }

        /// <summary>
        /// generuje testowy plik dla wybranego szablonu - potrzebne do generatora raportów / katalogów produktów
        /// </summary>
        /// <param name="wersja"></param>
        /// <param name="szablonId"></param>
        /// <returns></returns>
        [Route("test-catalog/{wersja:int}/{szablonId:int}")]
        public FileStreamResult GenerujTestowyPlikDanychKatalogu(int wersja, int szablonId)
        {
            PlikIntegracjiSzablon szablon = this.pobierzSzablonIntegracjiID(szablonId);
            if (szablon.typDanych != TypDanychIntegracja.ProduktyKatalogDrukowanie)
            {
                throw new Exception("Tą metodą można pobierać tylko dane dla typów dla szablonów. Próba pobrania typu: " + szablon.typDanych);
            }

            if (SolexHelper.AktualnyKlient.Dostep == AccesLevel.Niezalogowani)
            {
                throw new Exception("Musisz być zalogowany aby generować pliki");
            }

            object dane = null;
           
            IList<IProduktKlienta> listaProduktow = new List<IProduktKlienta>();

            List<AtrybutBll> listaAtrybutow = Calosc.DostepDane.Pobierz<AtrybutBll>(null).ToList();
            List<KategorieBLL> listaKategorii = Calosc.DostepDane.Pobierz<KategorieBLL>(null).ToList();
            for (int i = 0; i < listaAtrybutow.Count; ++i)
            {
                IProduktKlienta fakeProdukt = new TworzenieFakeObiektow(Calosc.Konfiguracja).FakeIProduktKlienta(i > 20 ? i: 0, listaAtrybutow, listaKategorii);
               // var pk = fakeProdukt as IProduktKlienta;

                if (fakeProdukt == null)
                {
                    throw new Exception("Źle stworzony fake produkt");
                }

                listaProduktow.Add(fakeProdukt);
            }

            dane = new ParametrDoKataloguProduktow { szablon = new TworzenieFakeObiektow(Calosc.Konfiguracja).FakeIKatalogSzablonModelBLL(),
                ListaProduktow = listaProduktow, OpisWpisanyPrzezKlienta = "opis klienta testowy" };


            var wynik = this.PobierzPlik_strumien(szablon, wersja, SolexHelper.AktualnyKlient, dane, czasCache: 0); //bez cache bo i tak robia sie fake ktore sa losowe

            if (wynik == null)
            {
                throw new Exception("Błąd generowania pliku danych. Plik pusty.");
            }

            Response.AddHeader("content-disposition", "attachment; filename=" + PlikIntegracjiSzablon.GenerujNazwePliku_plikDoDrukowaniaKatalogu(szablon, wersja));

            return new FileStreamResult(wynik.ToStream(), "application/json");
        }

        /// <summary>
        /// renderuje okno drukowania katalogu - z opcjami
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("print")]
        public PartialViewResult DrukujKatalogProduktow()
        {
            ParametrySzablonyKatalogow parametry = new ParametrySzablonyKatalogow();
            parametry.DostepneSzablony = SolexBllCalosc.PobierzInstancje.Klienci.PobierzSzablonyWidoczneDlaKlienta(SolexHelper.AktualnyPrzedstawiciel != null ? SolexHelper.AktualnyPrzedstawiciel : SolexHelper.AktualnyKlient);

            return PartialView("_drukujKatalogProduktow", parametry);
        }

        /// <summary>
        /// drukuje katalog wg. wytycznych
        /// </summary>
        /// <param name="parametry"></param>
        /// <param name="parametryListyProduktow"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("print")]
        public FileStreamResult DrukujKatalogProduktow(ParametrySzablonyKatalogow parametry, ParametryPrzekazywaneDoListyProduktow parametryListyProduktow)
        {
            if (parametry.Format == 0)
            {
                throw  new Exception("Brak formatu w jakim wydrukować katalog");
            }

            IKlient klient = SolexHelper.AktualnyKlient;

            if (klient.Dostep == AccesLevel.Niezalogowani || klient.Id == 0)
            {
                throw new Exception("Zaloguj się aby drukować");
            }

            KatalogSzablonModelBLL szablon = Calosc.Klienci.PobierzSzablonyWidoczneDlaKlienta(SolexHelper.AktualnyKlient).FirstOrDefault(x => x.Id == parametry.KatalogSzablon);

            IList<IProduktKlienta> produkty = null;

            //ladowanie danych dla produktow o ile podane
            if (parametryListyProduktow != null)
            {
                produkty = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.ProduktySpelniajaceKryteria(
                    parametryListyProduktow.kategoria, parametryListyProduktow.szukane, SolexHelper.AktualnyKlient,
                    SolexHelper.AktualnyJezyk.Id,
                    parametryListyProduktow.filtry, StaleFiltryAktualnieWybrane(),
                    parametryListyProduktow.szukanaWewnetrzne).Select(x=> x as IProduktKlienta).ToList();
            }
            else
            {
                produkty = SolexHelper.AktualnyKoszyk.PobierzPozycje.Select(x => x.Produkt as IProduktKlienta).Distinct().ToList();
            }
            if (produkty == null || !produkty.Any())
            {
                throw new Exception("Brak produktów do katalogu. Nie można wygenerować katalogu");
            }

            ParametrDoKataloguProduktow daneDoWidoku = new ParametrDoKataloguProduktow {szablon = szablon, ListaProduktow = produkty, OpisWpisanyPrzezKlienta = parametry.Opis};

            return this.DrukujKatalog(parametry, szablon, daneDoWidoku);
        }

        /// <summary>
        /// drukuje katalog wg. wytycznych
        /// </summary>
        /// <param name="parametry"></param>
        /// <param name="szablon"></param>
        /// <param name="dane"></param>
        /// <returns></returns>
        [NonAction]
        private FileStreamResult DrukujKatalog(ParametrySzablonyKatalogow parametry, KatalogSzablonModelBLL szablon, params object[] dane)
        {
            if (szablon == null)
            {
                throw new Exception("Brak szablony dostępnego dla klienta o id: " + parametry.KatalogSzablon);
            }

            StiReport raport = this.ZaladujRaport(szablon);

            MemoryStream rezultat = null;

            rezultat = this.GenerowanieRaportu(raport, parametry.Format, dane);
            rezultat.Seek(0, 0);

            string filename = string.Format("{0}_{1}.{2}", szablon.Nazwa, DateTime.Now.ToString().Replace(".", "-"), parametry.Format).Replace(" ", "_");
            Response.AddHeader("content-disposition", "attachment; filename=" + filename.ToLower());

            switch (parametry.Format)
            {
                case KatalogFormatZapisu.Pdf:
                    Response.ContentType = "text/pdf";
                    break;
                case KatalogFormatZapisu.Xlsx:
                    Response.ContentType = "application/xlsx";
                    break;
                case KatalogFormatZapisu.Docx:
                    Response.ContentType = "application/docx";
                    break;
                default:
                    Response.ContentType = "application/octet-stream";
                    break;
            }

            return new FileStreamResult(rezultat, Response.ContentType);
        }

        [NonAction]
        private StiReport ZaladujRaport(KatalogSzablonModelBLL szablon)
        {
            if (szablon.PlikSzablonu == null)
            {
                throw new Exception("Brak pliku szablonu wydruku dla szablonu katalogu id: " + szablon.Id);
            }

            StiReport report = new StiReport();
            report.Load(szablon.PlikSzablonu.SciezkaBezwzgledna);                  
            return report;
        }

        [NonAction]
        private MemoryStream GenerowanieRaportu(StiReport raport, KatalogFormatZapisu formatZapisu, params object[] data)
        {
            try
            {
                if (!SolexHelper.AktualnaSesjaID.HasValue)
                {
                    throw new Exception("Brak sesji - aby generowac raporty klient musi być zalogowany");
                }

                //generujemy randomowy numerek dla generacji 
                int randomNumber = new Random().Next();
                Dictionary<string, string> zrobioneSzablony = new Dictionary<string, string>(raport.Dictionary.Databases.Items.Length);

                //uzupelnianie danych
                foreach (StiFileDatabase zrodlo in raport.Dictionary.Databases.Items)
                {
                    int wersja;
                    PlikIntegracjiSzablon szablonObiekt = this.pobierzSzablonIntegracjiWgNazwyZrodlaDanychKatalogu(Path.GetFileName(zrodlo.PathData), out wersja);

                    if (szablonObiekt == null)
                    {
                        continue;
                    }

                    string klucz = $"{szablonObiekt.IdSzablonu}{wersja}";

                    if (zrobioneSzablony.TryGetValue(klucz, out string plik))
                    {
                        //szablon juz byl zrobiony
                        zrodlo.PathData = plik;
                        continue;
                    }

                    object daneDoGeneracji = null;

                    //wyszukiwanie właściwych danych do generowania
                    if (szablonObiekt.typDanych == TypDanychIntegracja.ProduktyKatalogDrukowanie)
                    {
                        foreach (var dat in data)
                        {
                            if (dat is ParametrDoKataloguProduktow)
                            {
                                daneDoGeneracji = dat;
                                break;
                            }
                        }
                    }

                    string dane = this.PobierzPlik_strumien(szablonObiekt, wersja, SolexHelper.AktualnyKlient, daneDoGeneracji);
                    string myTempFile = Path.Combine(Path.GetTempPath(), 
                        $"_del_zrodloTemp{szablonObiekt.IdSzablonu}{szablonObiekt.typDanych}{wersja}{SolexHelper.AktualnaSesjaID.Value}.{randomNumber}.{DateTime.Now.ToLongDateString()}{DateTime.Now.ToShortTimeString()}.json".Replace(":","-")  );
                    try
                    {
                        //plik nie moze istniec - jak istnieje to znaczy ze jest bardzo zle
                        if (System.IO.File.Exists(myTempFile))
                        {
                            throw new Exception($"plik nie miał istnieć wcześniej - ale istnieje: {myTempFile}");
                        }

                        System.IO.File.WriteAllText(myTempFile, dane);
                    }catch(Exception e)
                    {
                        Log.Error($"Błąd zapisu pliku: {myTempFile}", e);
                        throw;
                    }

                    zrobioneSzablony.Add(klucz, myTempFile);

                    zrodlo.PathData = myTempFile;
                }

                MemoryStream stream = new MemoryStream();
                StiExportFormat formatWydruku = StiExportFormat.Pdf;
                StiExportSettings ustawieniaExportu = null;

                switch (formatZapisu)
                {
                        case KatalogFormatZapisu.Pdf:
                        formatWydruku = StiExportFormat.Pdf;
                        ustawieniaExportu = new StiPdfExportSettings();
                        (ustawieniaExportu as StiPdfExportSettings).CreatorString = "SOLEX B2B";
                        (ustawieniaExportu as StiPdfExportSettings).ImageCompressionMethod = StiPdfImageCompressionMethod.Jpeg;
                        (ustawieniaExportu as StiPdfExportSettings).ImageFormat = StiImageFormat.Color;
                        (ustawieniaExportu as StiPdfExportSettings).ImageQuality = 1f;
                        (ustawieniaExportu as StiPdfExportSettings).ImageResolution = 300;
                        break;
                    case KatalogFormatZapisu.Docx:
                        formatWydruku = StiExportFormat.Word2007;
                        break;
                    case KatalogFormatZapisu.Xlsx:
                        formatWydruku = StiExportFormat.Excel2007;

                        ustawieniaExportu = new StiExcel2007ExportSettings();
                        (ustawieniaExportu as StiExcel2007ExportSettings).CompanyString = "SOLEX B2B";
                        (ustawieniaExportu as StiExcel2007ExportSettings).ImageQuality = 1f;
                        (ustawieniaExportu as StiExcel2007ExportSettings).ImageResolution = 300;
                        break;
                }

                raport.Render();
                raport.ExportDocument(formatWydruku, stream, ustawieniaExportu);
                raport.Dispose();
                return stream;
            }
            catch (OutOfMemoryException)
            {
                throw new Exception("Brak pamięci do drukowania katalogu. Zmniejsz ilość produktów, lub ogranicz wykorzystywanie zdjęć w raporcie. Upewnij się że używasz miniatur zdjęć a nie pełnych wielkości");
            }
            finally
            {
#if !DEBUG
                //nie kasujemy plikow z localhosta
                if (!Request.IsLocal)
                {
                    string[] filePaths = Directory.GetFiles(Path.GetTempPath(), "_del*.*");
                    foreach (string filePath in filePaths)
                    {
                        try
                        {
                            System.IO.File.Delete(filePath);
                        } catch
                        {}
                    }
                }
#endif
            }
        }

        /// <summary>
        /// główna akcja pobierania plików integracji
        /// </summary>
        /// <param name="typDanych"></param>
        /// <param name="format"></param>
        /// <param name="szablon"></param>
        /// <param name="wersja"></param>
        /// <param name="kodowanie"></param>
        /// <param name="kluczKlienta"></param>
        /// <param name="stream">czy wysyłać odpowiedz w strumieniu. Domyślnie nie - czyli zwracamy załącznik plik</param>
        /// <returns></returns>
        [Route("~/xmlapi/{szablonId}/{wersja}/{kodowanie}/{kluczKlienta?}")]
        public ActionResult PobierzPlik(int szablonId, int wersja, KodowaniePlikowIntegracji kodowanie, string kluczKlienta, bool stream = false,
            [ModelBinder(typeof(ArrayModelBinder<long>))] long[] exfilters = null, [ModelBinder(typeof(ArrayModelBinder<long>))] long[] filters = null)
        {
            ////BLOKADA w godzinach 9 - 14

            //if (DateTime.Now.Hour > 9 && DateTime.Now.Hour < 14)
            //{
            //    throw new Exception("Blokada generowania plików integracji w godzinach 9 - 14");
            //}

                IKlient klient = SolexHelper.AktualnyKlient;

            //sprawdzenie klienta wg. kLUCZA 
            if (klient.Dostep == AccesLevel.Niezalogowani)
            {
                if (string.IsNullOrEmpty(kluczKlienta))
                {
                    throw new HttpException(401, "Brak klucza API lub niepoprawny");
                }

                klient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(x => x.KluczSesji == kluczKlienta, null);

                if (klient == null)
                {
                    throw new HttpException(401, "Brak klucza API lub niepoprawny");
                }
            }

            //czy plik jest dostepny dla klientow - spradzmay NIE adminw - admini moga pobierac wszystko co chca
            if (!klient.CzyAdministrator && (Calosc.Konfiguracja.PlikiIntegracjiAktywne_DoPokazania == null || !Calosc.Konfiguracja.PlikiIntegracjiAktywne_DoPokazania.Contains(szablonId)))
            {
                throw new FileNotFoundException("Plik nie jest dostępny");
            }

            PlikIntegracjiSzablon szablon = this.pobierzSzablonIntegracjiID(szablonId);

            string wynik = this.PobierzPlik_strumien(szablon, wersja, klient, null, exfilters, filters);

            if (wynik != null)
            {

                //zapis zdarzenia do db
                Calosc.Statystyki.LogujDzialanieUzytkownikowAsync(this.SolexHelper.AktualnyKlient, $"{szablonId}/{wersja}", ZdarzenieGrupa.API, ZdarzenieGlowne.PobieranieDanych);

                Response.ContentType = Tools.PobierzInstancje.GetMimeType("." + szablon.Format);

                if (Response.ContentType == null)
                {
                    throw new Exception(string.Format("Nie można pobrać MIME type dla szablonu id: {0}, format: {1}", szablon.IdSzablonu, szablon.Format));
                }

                if (!stream)
                {
                    string fileName = PlikIntegracjiSzablon.GenerujNazwePliku_przyPobieraniu(szablon, wersja, SolexHelper.AktualnyJezyk.Symbol);
                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName);

                    //przekodowanie - zakladamy ze wszystko wchodzi jako UTF wiec tylko do ANSI robimy konwersje
                    if (kodowanie == KodowaniePlikowIntegracji.Windows1250)
                    {
                        Encoding kodowanieWyjsciowe = Encoding.GetEncoding("Windows-1250");
                        Response.ContentEncoding = kodowanieWyjsciowe;

                        byte[] bity = kodowanieWyjsciowe.GetBytes(wynik);
                        wynik = null;   //czyscimy string zeby pamiec zwolnic
                        bity = kodowanieWyjsciowe.GetPreamble().Concat(bity).ToArray();
                        Response.BinaryWrite(bity);
                        return null;
                    }
                    return new FileStreamResult(wynik.ToStream(), Response.ContentType);
                }
                else
                {
                    if (kodowanie == KodowaniePlikowIntegracji.Windows1250)
                    {
                        return Content(wynik, Response.ContentType, Encoding.GetEncoding("Windows-1250"));
                    }
                    return Content(wynik, Response.ContentType);
                }
            }

            throw new Exception("Brak formatu dla podanych parametrów");
        }


    }
}