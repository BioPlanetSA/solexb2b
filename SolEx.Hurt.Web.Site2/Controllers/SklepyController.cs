using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ServiceStack.Common;
using ServiceStack.Text;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Bindowania;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("maps")]
    public class SklepyController : SolexControler
    { 
       
        /// <summary>
        /// </summary>
        /// <param name="idKontrolki"></param>
        /// <param name="kid">kategorie id - tablica</param>
        /// <param name="miasto"></param>
        /// <returns></returns>
        [Route("markers")]
        public JsonResult PobierzSklepyNaMapeWPostaciPuntow(int idKontrolki,[ModelBinder(typeof(ArrayModelBinder<long>))] long[] kid, string miasto)
        {
            if (idKontrolki == 0)
            {
                throw new Exception("Brak kontrolki id, złe parametry");
            }

            Sklepy kontrolka = this.PobierzKontrolke<Sklepy>(idKontrolki);
            if (kontrolka == null)
            {
                throw  new Exception($"Brak kontrolki id: {idKontrolki}");
            }

            if (miasto == "")
            {
                miasto = null;
            }

            var katId = kid != null ? new HashSet<long>( kid ) : new HashSet<long>();
            var sklepy = Calosc.Sklepy.ListaSklepowDlaWybranychKategorii(katId, true, miasto, !kontrolka.PokazujWszystkieSklepyWybraneMiasto);
            Dictionary<long, string> pinezki = Calosc.Sklepy.PobierzIkonyNaMape(this.SolexHelper.AktualnyJezyk.Id);
            List<PunktNaMape> punkty = new List<PunktNaMape>();
            foreach (var sklepyBll in sklepy)
            {
                punkty.Add(new PunktNaMape(sklepyBll, pinezki.FirstOrDefault(x => sklepyBll.KategorieId.Contains(x.Key)).Value ?? Calosc.Konfiguracja.IkonaMapy));
            }

            if (punkty == null || !punkty.Any())
            {
                throw new Exception("Brak punktów do pokazania na mapie. Nie ponienno w ogóle próboweać pobierać punktów.");
            }
            return Json( punkty );
        }

        [Route("Miasta")]
        public JsonResult Miasta([ModelBinder(typeof(ArrayModelBinder<long>))] long[] kid)
        {
            var katId = kid != null ? new HashSet<long>( kid ) : new HashSet<long>();
            List<string> miasta = Calosc.Sklepy.MiastaDlaWybranychKategorii(katId);
            if(miasta.Any()) return Json(miasta.Distinct());
            return null;
        }


        [Route("Mapa")]
        public ActionResult Mapa(int id)
        {
            Sklepy kontrolka = this.PobierzKontrolke<Sklepy>(id);
            
            ParametryDoSklepu parametryDoWidoku = new ParametryDoSklepu(kontrolka);

            //kafle
            if (kontrolka.PokazujSklepy == RodzajSklepu.SklepyKafle)
            {
                parametryDoWidoku.Sklepy = Calosc.Sklepy.ListaSklepowDlaWybranychKategorii(kontrolka.SklepyKategoria, false)
                        .OrderBy(x => x.Obrazek == null)
                        .ThenBy(x => x.Nazwa)
                        .ToList();

                return PartialView("_Kafelki", parametryDoWidoku);
            }

            //mapa

            //sprawdzamy czy jest klucz google api wprowadzony a ustawieniach
            if (string.IsNullOrEmpty(Calosc.Konfiguracja.GoogleApiKey))
            {
                return Content("Brak klucza API (ustawienie 'Klucz google api').Klucz API można wygenerować pod adresem:  <a href = 'https://developers.google.com/maps/documentation/javascript/get-api-key'>https://developers.google.com/maps/documentation/javascript/get-api-key</a>");
            }
            //jeśli jest plik klm to nie renderujemy innych parametrów 
            if (kontrolka.SklepySciezkaKLMId.HasValue)
            {
                parametryDoWidoku.Kontrolka.PokazFiltrKategorii = false;
                parametryDoWidoku.Kontrolka.SklepyListaMiast = false;
                return PartialView("_Mapa", parametryDoWidoku);
            }

            //uzupełniamy parametry o kategorie, jesli metada zwróciła jakić content czyli przerywamy i zwracamy to co dostaliśmy w wyniku. 
            ActionResult content;
            if (UzupelnijParametryOKategorie(kontrolka.SklepyKategoria, parametryDoWidoku, SolexHelper.AktualnyJezyk.Id, out content))
            {
                return content;
            }


            //uzupełniamy parametry o miasta
            UzupelnijParametryOMiasta(kontrolka.SklepyKategoria, parametryDoWidoku);

            if (!kontrolka.PokazFiltrKategorii && !kontrolka.SklepyListaMiast)
            {
                var sklepy = Calosc.Sklepy.ListaSklepowDlaWybranychKategorii(kontrolka.SklepyKategoria, true);
                Dictionary<long, string> pinezki = Calosc.Sklepy.PobierzIkonyNaMape(this.SolexHelper.AktualnyJezyk.Id);
                List<PunktNaMape> punkty = new List<PunktNaMape>();
                foreach (var sklepyBll in sklepy)
                {
                    punkty.Add(new PunktNaMape(sklepyBll, pinezki.FirstOrDefault(x => sklepyBll.KategorieId.Contains(x.Key)).Value ?? Calosc.Konfiguracja.IkonaMapy));
                }
                parametryDoWidoku.Punkty = punkty.ToJson();
            }
            return PartialView("_Mapa", parametryDoWidoku);
        }

        [Route("Info")]
        public ActionResult InfoOSklepie(long idSklepu)
        {
            SklepyBll sklep = Calosc.DostepDane.PobierzPojedynczy<SklepyBll>(idSklepu);
            if (sklep == null) return null;
            return PartialView("dymekMape", sklep);
        }


        /// <summary>
        /// Uzupełniamy parametry i kategorie 
        /// </summary>
        /// <param name="sklepyKategoria"></param>
        /// <param name="parametryDoWidoku"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected bool UzupelnijParametryOKategorie(HashSet<long> sklepyKategoria, ParametryDoSklepu parametryDoWidoku, int jezykId, out ActionResult content)
        {
            content = null;
            List<KategoriaSklepu> kategorie = Calosc.Sklepy.PobierzKategorieNiepusteIPoprawneKoordynaty(sklepyKategoria, jezykId);
            if (kategorie == null || !kategorie.Any())
            {
                {
                    content = Content("Brak punktów do pokazania na mapie");
                    return true;
                }
            }
            if (kategorie.Count == 1)
            {
                parametryDoWidoku.Kontrolka.PokazFiltrKategorii = false;
                parametryDoWidoku.KategoriaId = kategorie.First().Id;
            }
            else
            {
                parametryDoWidoku.KategorieSklepowDoPokazania = kategorie;
            }
            return false;
        }

        /// <summary>
        /// Uzupełniamy miasta do pokazania w ramach kategorii
        /// </summary>
        /// <param name="sklepyKategoria">kategorie dla których pobieramy miasta</param>
        /// <param name="parametryDoWidoku"></param>
        protected void UzupelnijParametryOMiasta(HashSet<long> sklepyKategoria, ParametryDoSklepu parametryDoWidoku)
        {
            List<string> miastaDoPokazania = Calosc.Sklepy.MiastaDlaWybranychKategorii(sklepyKategoria);
            //jeśli ne ma miast to nie pokazujemy dropdowna z miastami
            if (miastaDoPokazania == null || !miastaDoPokazania.Any())
            {
                parametryDoWidoku.Kontrolka.SklepyListaMiast = false;
            }
            else
            {
                parametryDoWidoku.MiastaDlaKategorii = miastaDoPokazania;
            }
        }
    }
}