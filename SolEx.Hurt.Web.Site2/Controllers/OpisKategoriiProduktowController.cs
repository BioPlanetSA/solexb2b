using System.Web.Mvc;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("OpisKategoriiProduktow")]
    public class OpisKategoriiProduktowController : SolexControler
    {
        [Route("OpisKategorii")]
        public PartialViewResult OpisKategorii(object identyfikatorobiektu, string pole, string naglowek, string preset, string opakowanie, string textdomyslny, string stopka)
        {
            if(identyfikatorobiektu==null) return null;
            var kategoria = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<KategorieBLL>(identyfikatorobiektu, SolexHelper.AktualnyJezyk.Id);
            if (kategoria == null)
            {
                kategoria = new KategorieBLL();
                textdomyslny = "Nie można wczytać aktualnej kategorii. Upewnij się że otwierasz stronę kategorii lub czy kategoria istnieje.";
            }
            return StworzPolePojedynczegoWpisu(kategoria, pole, naglowek, preset, opakowanie, kategoria.Nazwa, kategoria.MetaOpis, kategoria.MetaSlowaKluczowe, stopka, textdomyslny);
        }
     }
}