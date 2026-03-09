using System;
using System.Linq;
using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Stronicowanie")]
    public class StronicowanieController : SolexControler
    {
        [Route("Stronicowanie")]
        public PartialViewResult Stronicowanie(int lacznie,ParametryPrzekazywaneDoListyProduktow parametry, bool pokazacWyborStron)
        {
            int rozmiar = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient,TypUstawieniaKlienta.RozmiarStronyListaProduktow);
            int liczbastron = (int)Math.Ceiling((double)lacznie / rozmiar);
            int[] dostepneRozmiaryStron = SolexBllCalosc.PobierzInstancje.Konfiguracja.IleProduktowPokazacNaStronie.Split(new[] {';'},StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
            
            return PartialView(new ParametryDoStronicowania(liczbastron, parametry, lacznie, pokazacWyborStron,rozmiar,dostepneRozmiaryStron));
        }

    }
}