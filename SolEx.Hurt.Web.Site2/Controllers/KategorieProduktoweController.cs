using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.UI;
using ServiceStack.Common;
using ServiceStack.Common.Extensions;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci;
using SolEx.Hurt.Web.Site2.PageBases;
using ServiceStack.Text;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("KategorieProduktowe")]
    public class KategorieProduktoweController : SolexControler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pokazujWTabach"></param>
        /// <param name="id">Id kontrolki</param>
        /// <returns></returns>
        [Route("Drzewko")] 
        public PartialViewResult Drzewko(int id, string szukanieGlobalne = null)
        {
            //ustawienia dla kontrolki

            IDrzewoKategorii kontrolkaDrzewko = Calosc.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(id).Kontrolka() as IDrzewoKategorii;

            ParametryDoDrzewaKategorii parametryDrzewa = new ParametryDoDrzewaKategorii {Klient =  SolexHelper.AktualnyKlient,  WybraneStaleFiltry = this.StaleFiltryAktualnieWybrane(), IdKontrolki =  id, Kontrolka =  kontrolkaDrzewko};

            long kategoriaIdAktualnieWybrana;

            if (Url.AktualnaStronaToStronaProduktow(out kategoriaIdAktualnieWybrana))
            {
                parametryDrzewa.WybranaKategoriaID = kategoriaIdAktualnieWybrana;
            }

            if (szukanieGlobalne == null)
            {
                parametryDrzewa.szukanieGlobalne = this.SzukanaFrazaGlobalnie();
            }
            else
            {
                parametryDrzewa.szukanieGlobalne = szukanieGlobalne;
            }

            return PartialView("_Drzewko", parametryDrzewa);
        }

        /// <summary>
        /// metoda wykonywana przez AJAX do ladowania drzewka kategorii produktów. Do wyrenderowania obudowy drzewka uzyj Akcji Drzewko()
        /// </summary>
        /// <param name="idKontrolki"></param>
        /// <param name="szukanieGlobalne"></param>
        /// <returns></returns>
        [Route("DrzewkoKategorie/{idKontrolki}")]
        [Route("DrzewkoKategorie")]
        [Route("DrzewkoKategorie/{idKontrolki}/{wybranaGrupa}")]
        [Route("DrzewkoKategorie/{idKontrolki}/{wybranaGrupa}/{wybranaKategoria}/{szukanieGlobalne}")]
        [Route("DrzewkoKategorie/{idKontrolki}/{wybranaGrupa}/{szukanieGlobalne}")]
        [OutputCacheSolex(TypDanychDoCache.Kategorie)]
        public PartialViewResult DrzewkoKategorie(int idKontrolki, string szukanieGlobalne)
        {
            //ustawienia dla kontrolki
            IDrzewoKategorii kontrolkaDrzewko = Calosc.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(idKontrolki).Kontrolka() as IDrzewoKategorii;

            Dictionary<long, GrupaBLL> wszystkieGrupyWidoczneDlaKlienta = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<GrupaBLL>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, x => x.Widoczna).ToDictionary(x => x.Id, x => x);
            //bool pokazywacTaby = parametry.PokazujDrzewoZTabami;

            //bierzemy tylko fraze szukana GŁÓWNA - nie wewnetrzna - nie bierzemy filtrów zwykłych bo nie dzialaja na kategorie i NIE bierzemy szukania w kategorii
            var staleFiltryWybrane = StaleFiltryAktualnieWybrane();
            IList<ProduktKlienta> produktyWidoczneDlaAktualnychParametrow = Calosc.ProduktyKlienta.ProduktySpelniajaceKryteria(null, szukanieGlobalne, 
                SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id, null, staleFiltryWybrane, null);

            //Tworzymy slownik gdzie kluczem będzie kategoria id natomiast wartość to KeyValuPart gdzie kluczem jest kategoria a wartosci to ilość produktów w kategorii
            Dictionary<long, KeyValuePair<KategorieBLL, int>> slownikKategorii = new Dictionary<long, KeyValuePair<KategorieBLL, int>>();

            if (SolexHelper.AktualnyKlient.Id == 0)
            {
                slownikKategorii = produktyWidoczneDlaAktualnychParametrow.SelectMany(x => x.Kategorie).Where(y => wszystkieGrupyWidoczneDlaKlienta.ContainsKey(y.GrupaId) && y.Dostep != AccesLevel.Zalogowani).GroupBy(x => x.Id).ToDictionary(x => x.Key, x => new KeyValuePair<KategorieBLL, int>(x.First(), x.Count()));
            }
            else
            {
                slownikKategorii = produktyWidoczneDlaAktualnychParametrow.SelectMany(x => x.Kategorie).Where(y => wszystkieGrupyWidoczneDlaKlienta.ContainsKey(y.GrupaId) && y.Dostep != AccesLevel.Niezalogowani).GroupBy(x => x.Id).ToDictionary(x => x.Key, x => new KeyValuePair<KategorieBLL, int>(x.First(), x.Count()));
            }
            
            //jesli chcemy miec info ile jest produktow w kategoriach to trzeba to tu policzyc
            Dictionary<long, int> slownikIleProduktowWKategoriach = new Dictionary<long, int>();

            //Tworzymy slownik kategorii na podstawie slownika stworzonego u gory bedzie posiadał jako klucz grupe oraz liste kategori z danej grupy
            Dictionary<long, List<KategorieBLL>> kategorieProduktowRozpatrywanych = slownikKategorii.Select(x => x.Value.Key).GroupBy(x => x.GrupaId).ToDictionary(x => x.Key, x => x.OrderBy(z => z.Kolejnosc).ThenBy(z => z.Nazwa).ToList());
            //.GroupBy(x=>x.Value.Key.GrupaId).ToDictionary(x => wszystkieGrupyWidoczneDlaKlienta[x.Value.Key.GrupaId], x => x.Value.Key.OrderBy(z => z.Kolejnosc).ThenBy(z => z.Nazwa).ToList());
            
            //Jezeli nie chcemy pokazywać liczby produktów w drzewku to czyścmiy slownik, moglibyśmy tworzyć slownik w momencie jak mamy  pokazywac liczebe produktów jednak mimo wszystko musielibyśmy wuciagnąć wszystkie 
            if (kontrolkaDrzewko.PokazLiczbeProduktowWDrzewku)
            {
                //Z głownego slownika tworzymy slowniek gdzie kluczem jest  kategorie które maja odpowiednie dostęp (dla klientów zalogowanych/niezalogowanych) oraz 
                slownikIleProduktowWKategoriach = slownikKategorii.ToDictionary(x => x.Key, x => x.Value.Value);
                //slownikIleProduktowWKategoriach.Clear();
            }
            
            if (!kategorieProduktowRozpatrywanych.Any())
            {
                return null;
            }

            IList<GrupaBLL> posortowaneGrupyKategorii = wszystkieGrupyWidoczneDlaKlienta.WhereKeyIsIn(kategorieProduktowRozpatrywanych.Keys).OrderBy(x => x.Kolejnosc).ToList();

            ParametryDoDrzewaKategorii parametryDoDrzewaKategorii = new ParametryDoDrzewaKategorii(
                kategorieProduktowRozpatrywanych,
                SolexHelper.AktualnyKlient,
                StaleFiltryAktualnieWybrane(),
                slownikIleProduktowWKategoriach,
                kontrolkaDrzewko,
                posortowaneGrupyKategorii
                );

            var wynik = PartialView("_DrzewkoKategorie", parametryDoDrzewaKategorii);

            //ustawianie klucza do cache
            string cachedKey = Calosc.Cache.WyliczKluczDlaKategorii(idKontrolki, SolexHelper.AktualnyKlient, szukanieGlobalne);
            if (!string.IsNullOrEmpty(cachedKey))
            {
                Calosc.Cache.DodajObiekt(cachedKey, wynik);
            }
            return wynik;
        }
        
        [Route("Kategorie")]
        public ActionResult Kategorie(int id) 
        {
            Producenci kontrolka = this.PobierzKontrolke<Producenci>(id);
            GrupaBLL grupa = null;

            if (kontrolka.GrupaDoPokazaniaId.HasValue)
            {
                grupa= SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<GrupaBLL>(kontrolka.GrupaDoPokazaniaId.Value, SolexHelper.AktualnyKlient);
            }

            if (grupa == null)
            {
                throw new Exception("Brak grupy do pokazania. Przerywam działanie");
            }

            string symbolopisgrupy = kontrolka.SymbolOpisGrupy;
            if (string.IsNullOrEmpty(symbolopisgrupy))
            {
                symbolopisgrupy = grupa.SymbolTresciOpisuGrupy;
            }

            string styleCssElementu = kontrolka.DodatkoweKlasyCssElementyKontrolki?.Join(" ");
            
            return ListaKafle(kontrolka.KategorieProduktoweSposobRenderowania.ToString(), kontrolka.RozmiarKafla, kontrolka.TylkoZObrazkami, grupa.Id, null, symbolopisgrupy, styleCssElementu, kontrolka.PokazujNazwe, kontrolka.OgraniczajKategorieWgStalychFiltrowISzukania, kontrolka.PokazujPodkategorie);
        }

        [ChildActionOnly]
        public ActionResult ListaKafleKategorie(string kategorieProduktoweSposobRenderowania, string rozmiarKafla = "", bool tylkoZObrazkami = false, int? kategoriId = null, string DodatkoweKlasyCssElementyKontrolki = "", Dictionary<int, HashSet<long>> filtry = null)
        {
            return ListaKafle(kategorieProduktoweSposobRenderowania, rozmiarKafla, tylkoZObrazkami, null, kategoriId, DodatkoweKlasyCssElementyKontrolki, filtry:filtry);
        }
        [ChildActionOnly]
        public ActionResult ListaKafleGrupa(string kategorieProduktoweSposobRenderowania, string rozmiarKafla = "", bool tylkoZObrazkami = false, int? grupaId = null)
        {
            return ListaKafle(kategorieProduktoweSposobRenderowania, rozmiarKafla, tylkoZObrazkami, grupaId);
        }

        // GET: KategorieProduktowe
        [NonAction]
        public ActionResult ListaKafle(string kategorieProduktoweSposobRenderowania, string rozmiarKafla = "", bool tylkoZObrazkami = false,
            long? grupaId = null, int? kategoriId = null, string symbolopisugrupy = "",
            string DodatkoweKlasyCssElementyKontrolki = "", bool pokazujNazwe = true, bool ograniczajKategorieWgStalychFiltrowISzukania = true,
            bool pokazujPodkat = false, Dictionary<int, HashSet<long>> filtry = null)
        {
            if (grupaId != null && kategoriId != null)
            {
                throw new Exception("W kontrolce można podać grupę LUB kategorię - podano oba parametry. Przerywam działanie");
            }

            if (grupaId == null && kategoriId == null && filtry == null)
            {
                throw new Exception("W kontrolce można podać grupę LUB kategorię - nie pobrano żadnego parametru. Przerywam działanie");
            }

            HashSet<long> produktyDostepne = null;

            if (ograniczajKategorieWgStalychFiltrowISzukania)
            {
                produktyDostepne = new HashSet<long>(Calosc.ProduktyKlienta
                    .ProduktySpelniajaceKryteria(null, null, SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id,
                        filtry, StaleFiltryAktualnieWybrane(), null)
                    .Select(x => x.Id));

                if (!produktyDostepne.Any())
                {
                    //nie ma produktów to nie bedzie również kategorii ale z racji tego ze kontrolka ma zawezac kategoeie to NIC nie rednerujemy (nie pokazujmey również bledu)
                    return null;
                }
            }
            else
            {
                produktyDostepne = Calosc.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(SolexHelper.AktualnyKlient);

                if (!produktyDostepne.Any())
                {
                    //nie ma produktów to nie bedzie również kategorii
                    return Content("Brak produktów dla klienta");
                }
            }

            IList<KategorieBLL> kategorieDoPokazania = null;
            var klient = SolexHelper.AktualnyKlient;
            if (grupaId.HasValue)
            {
                GrupaBLL grupa;

                List<GrupaBLL> grupy = Calosc.DostepDane.Pobierz<GrupaBLL>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient).ToList();
                grupy = grupy.Where(x => x.PobierzKategorie(klient, produktyDostepne).Any()).ToList(); //specjalnie w 2 krokach, bo najpier chcemy zeby walidator zadzialal!!

                if (grupaId == 0)
                {
                    grupa = grupy.FirstOrDefault(x => !x.Producencka);
                }
                else
                {
                    grupa = grupy.FirstOrDefault(x => x.Id == grupaId.Value);
                }

                if (grupa != null)
                {
                    kategorieDoPokazania = grupa.PobierzKategorie(klient, produktyDostepne);
                    if (!pokazujPodkat)
                    {
                        kategorieDoPokazania = kategorieDoPokazania.Where(x => x.ParentId == null).ToList();
                    }
                }

            }

            if (kategoriId.HasValue)
            {
                var parent = Calosc.DostepDane.PobierzPojedynczy<KategorieBLL>(kategoriId.Value, SolexHelper.AktualnyJezyk.Id);
                kategorieDoPokazania = parent.Dzieci.Where(x => Calosc.KategorieDostep.JestWidocznaDlaKlienta(x, klient, produktyDostepne)).ToList();
            }

            //jeśli nie ma nic to znaczy że klien nie widzi wybranych kategorii lub grup 
            if (kategorieDoPokazania == null)
            {
                return null;
                //throw new Exception("Brak kategorii gid" + grupaId + " kid");
            }

            if (tylkoZObrazkami)
            {
                kategorieDoPokazania = kategorieDoPokazania.Where(x => x.Obrazek != null).ToList();
            }
            
            //sortowanie  
            //OrderByDescending(x => x.ObrazekId.HasValue).  - Bartek to wywalam bo Kamil sapie mi za uchem. Trzeba by coś z tym zrobić w końcu - najlepiej dać do kontrolki wybor dla usera :(
            kategorieDoPokazania = kategorieDoPokazania.OrderBy(x => x.Kolejnosc).ThenBy(x => x.Nazwa).ToList();

            //-------------*****************------- 
            //zmiany dla BIO planet - tylko pokazujemy kategorie ktore maja swoje strony w stronach bio planet
            Dictionary<int, string> stronyProducentow = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<Dictionary<int, string>>("producenciStrony");

            if (stronyProducentow == null)
            {
                //odczyt z bazy
                stronyProducentow = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.GetDictionary<int, string>("SELECT * FROM vStronyProducentow");
                if (stronyProducentow == null)
                {
                    throw new Exception("Brak producentów stron");
                }
                SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt_Kluczowy("producenciStrony", stronyProducentow);
            }

            //przefiltrowanie kategorii wzgledem stron producentów istniejących
            kategorieDoPokazania = kategorieDoPokazania.Where(x => stronyProducentow.ContainsKey((int)x.Id)).ToList();

            foreach (var strona in stronyProducentow)
            {
                var producent = kategorieDoPokazania.FirstOrDefault(x => x.Id == strona.Key);
                if (producent != null)
                {
                    //przepisanie linku do strony do pola tekst5 - mam nadzieje ze nie korzystamy z tego pola
                    string witryna = strona.Value.Split(new char[] {';'}).FirstOrDefault(x => x.ToLower().StartsWith("https"));
                    producent.Tekst5 = witryna;
                }
            }


            //-------------***************** 


            ParametryDoListyKategoriiProduktow model = new ParametryDoListyKategoriiProduktow
            {
                Klient = SolexHelper.AktualnyKlient,
                Kategorie = kategorieDoPokazania,
                RozmiarKafla = rozmiarKafla,
                SymbolOpisGrupy = symbolopisugrupy,
                ElementCss = DodatkoweKlasyCssElementyKontrolki,
                PokazujNazwe = pokazujNazwe
            };

            if (!kategorieDoPokazania.Any())
            {
                Log.WarnFormat("Brak kategorii do pokazania w kontrolce kategorie produktowe. GrupaID ={0}, kategoriaId={1}, tylkoZObrazkami={2}", grupaId, kategoriId, tylkoZObrazkami);
            }
            
            return PartialView(kategorieProduktoweSposobRenderowania, model);
        }
    }
}