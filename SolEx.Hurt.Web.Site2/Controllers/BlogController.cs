using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Blog")]
    public class BlogController : SolexControler
    {
       [ChildActionOnly]
        public PartialViewResult WpisBloga(long identyfikatorobiektu, string pole, string naglowek, string preset, string opakowanie, string stopka)
        {
            var wpis = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<BlogWpisBll>(identyfikatorobiektu,SolexHelper.AktualnyJezyk.Id);
            if (wpis != null)
            {
                return StworzPolePojedynczegoWpisu(wpis, pole, naglowek, preset, opakowanie, wpis.Tytul, wpis.KrotkiOpis, wpis.Tagi, stopka);
            }
            return null;
        }

        [ChildActionOnly]
        public PartialViewResult Sciezka(long identyfikatorobiektu, string symbolstronapoprzednia)
        {
            if (RouteData.Values["idTresci"] == null)
            {
                return null;
            }

            int? idtresc = null;
            idtresc = Convert.ToInt32(RouteData.Values["idTresci"]);
            TrescBll tresc = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(idtresc);

            if (tresc == null)
            {
                //jak nie ma tresci to dajemy 404
                Response.StatusCode = 404;
                return null;
            }

            BlogWpisBll wb = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<BlogWpisBll>(identyfikatorobiektu, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);

            if (wb == null)
            {
                //jak nie ma bloga to dajemy 404
                Response.StatusCode = 404;
                return null;
            }
            return WspolnaCzescSciezka(Url.ZbudujLink(wb, tresc.Symbol, SolexHelper.AktualnyJezyk),wb.Tytul,symbolstronapoprzednia);
        }

        [ChildActionOnly]
        public PartialViewResult PokazKategorieBloga(long identyfikatorobiektu, int[] grupy, bool pokazywacnazwegrupy, string naglowek, string opakowanie)
        {
            if (string.IsNullOrEmpty(opakowanie) ) opakowanie = "{0}";
            var wpis = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<BlogWpisBll>(identyfikatorobiektu,SolexHelper.AktualnyJezyk.Id);

            if (wpis == null)
            {
                return null;
            }

            Dictionary<string, List<BlogKategoria>> slownikKategorii;
            List<BlogKategoria> kategorieWpisu;
            if (grupy != null && grupy.Any())
            {
                kategorieWpisu = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<BlogKategoria>(SolexHelper.AktualnyKlient, x => Sql.In(x.Id, wpis.Kategorie)  && Sql.In(x.BlogGrupaId, grupy)).ToList();
                var grupySlownik =
                    SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<BlogGrupa>(SolexHelper.AktualnyKlient,
                        x => grupy.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Nazwa);
                slownikKategorii = kategorieWpisu.GroupBy(x => x.BlogGrupaId != null ? x.BlogGrupaId.Value : 0).ToDictionary(x => grupySlownik[x.Key], x => x.ToList());
            }
            else
            {
                kategorieWpisu = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<BlogKategoria>(null, x => wpis.Kategorie.Contains(x.Id)).ToList();
                var grupySlownik = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<BlogGrupa>(SolexHelper.AktualnyKlient).ToDictionary(x => x.Id, x => x.Nazwa);
                grupySlownik.Add(0, "");
                slownikKategorii = kategorieWpisu.GroupBy(x => x.BlogGrupaId != null ? x.BlogGrupaId.Value : 0).ToDictionary(x => x.Key == 0 ? "" : grupySlownik[x.Key], x => x.ToList());
            }
            return PartialView("KategorieBlogu", new ParametryPrzekazywaneDoSzegolow(naglowek, slownikKategorii, pokazywacnazwegrupy, null, wpis.Tytul, wpis.KrotkiOpis, wpis.Tagi, null, opakowanie));
        }

        [NonAction]
        protected List<BlogWpisBll> PosortujWpisyBloga(BlogiKolejnosc kolejnosc, long[] kategorie, int ilePobrac, int ilePominac, out int iloscLaczna)
        {
            //todo: testy jednostkowe i odrazu tu pobieranie tylko tyle ile trzeba a nie cale listy
            List<BlogWpisBll>lista;
            if (kolejnosc == BlogiKolejnosc.Losowa)
            {
                var random = new Random();
                lista = Calosc.DostepDane.Pobierz<BlogWpisBll>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, x => x.Aktywny && x.Kategorie.Overlaps(kategorie)).OrderBy(order => random.Next()).ToList();
            }
            else
            {
                SortowanieKryteria<BlogWpisBll> sortowanie = null;
                if (kolejnosc == BlogiKolejnosc.DataDodania)
                {
                    sortowanie = new SortowanieKryteria<BlogWpisBll>(x => x.DataDodania, KolejnoscSortowania.desc, "DataDodania");
                }
                if (kolejnosc == BlogiKolejnosc.Kolejnosc)
                {
                    sortowanie = new SortowanieKryteria<BlogWpisBll>(x => x.Kolejnosc, KolejnoscSortowania.desc, "Kolejnosc");
                }
                List<SortowanieKryteria<BlogWpisBll>> listaSortowan = new List<SortowanieKryteria<BlogWpisBll>>();
                listaSortowan.Add(sortowanie);
                lista = Calosc.DostepDane.Pobierz<BlogWpisBll>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, x => x.Aktywny && x.Kategorie.Overlaps(kategorie),listaSortowan).ToList();
            }

            iloscLaczna = lista.Count;

            if (ilePominac >= iloscLaczna)
            {
                return null;
            }

            if (ilePominac + ilePobrac > iloscLaczna)
            {
                ilePobrac = iloscLaczna - ilePominac;
            }

            return lista.Skip(ilePominac).Take(ilePobrac).ToList();
        }

        /// <summary>
        /// Główna akcja ktora pokazuje wpisy blogawg. ustalonych danych
        /// </summary>
        [Route("WpisyBlogaPobierzWpisy")]
        public ActionResult WpisyBloga(BlogModel model)
        {
            if (model.BlogKategorieDoPokazania == null)
            {
                //jak nie ma bloga to dajemy 404
                Response.StatusCode = 404;
                return null;
            }

            //Sortowanie wpisow bloga
            int iloscLaczna;
            int ileDoczytac = model.BlogIlePokazywacMaxWpisow;
            if (Request.IsAjaxRequest())
            {
                //jak ajax to doczytanie dynamicznie wiec doczytujemy tyle ile jest w kontrolce zdefiniowane
                ileDoczytac = model.BlogIleDynamicznieZaladowacAktualnosci;
            }

            model.BlogWpisList = PosortujWpisyBloga(model.BlogKolejnosc, model.BlogKategorieDoPokazania, ileDoczytac, model.IloscJuzPokazanaKlientowi, out iloscLaczna);

            if (model.BlogWpisList == null || !model.BlogWpisList.Any())
            {
                return null;
            }
            model.IloscJuzPokazanaKlientowi += model.BlogWpisList.Count;

            model.PokazWiecej = false;
            //wykorzystywane do sprawdzenia czy pokazywac button, jezeli nie bedzie kolejnych wpisów nie pokazemy przycisku
            if (model.BlogIleDynamicznieZaladowacAktualnosci > 0 && iloscLaczna > model.IloscJuzPokazanaKlientowi)
            {
                model.PokazWiecej = true;
            }

            if (Request.IsAjaxRequest())
            {
                model.NaglowekWpisow = null;    //nie ma naglowka przy pobieraniu wiecej tresci
                return Json(new { data = this.PartialViewToString("UkladyListy/" + model.BlogUklad, model), pokazywacPrzyciskLadowania = model.PokazWiecej, ileJuzPokazaneKlientowi = model.IloscJuzPokazanaKlientowi });
            }

            return PartialView("BlogPojemnik", model);
        }

        [ChildActionOnly]
        public PartialViewResult GaleriaZdjec(int identyfikatorobiektu, string folderzdjecdogalerii, string folderdogaleriizdjeczwpisu, string rozmiarzdjecia, string rozmiarminiatur, string naglowek, string stopka)
        {
            if (folderdogaleriizdjeczwpisu == null)
            {
                throw new Exception("Brak ścieżki do galerii - nic nie można pokazać");
            }

            var wpis = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<BlogWpisBll>(identyfikatorobiektu);

            if (wpis == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(folderzdjecdogalerii))
            {
                var type = wpis.GetType();
                var property = type.GetProperty(folderdogaleriizdjeczwpisu);
                var valueTmp = property.GetValue(wpis);
                folderzdjecdogalerii = ( valueTmp==null)?null:valueTmp.ToString();
            }
            List<IObrazek> listaZdjec = new List<IObrazek>();
            if(string.IsNullOrEmpty(folderzdjecdogalerii)) return null;
            string sciezka = Path.Combine("Zasoby", folderzdjecdogalerii);
            foreach (var c in PlikiDostep.PobierzInstancje.PobierzPlikiGraficzne(sciezka))
            {
                IObrazek newObrazek = new Obrazek(sciezka, c.SplitOnLast("\\").Last());
                newObrazek.DomyslnyPreset = rozmiarminiatur;

                listaZdjec.Add(newObrazek);
            }
            if (!listaZdjec.Any())
            {
                Log.ErrorFormat("Brak plików do pokazania w galerii - w ścieżce: {0}", folderzdjecdogalerii);
                return null;
            }

            return PartialView("GaleriaZdjec", new ParametryDoGaleriiZdjecBlog(rozmiarzdjecia,naglowek,stopka,listaZdjec.OrderBy(x=>x.Nazwa).ToList()));
        }
    }
}

