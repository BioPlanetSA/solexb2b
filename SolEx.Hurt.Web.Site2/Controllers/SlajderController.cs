using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Slajder")]
    public class SlajderController : SolexControler
    {  
        [Route("FullPage")]
        public ActionResult FullPage(long[] listaslajdow, int czasprzeskoku, int czasanimacji)
        {
            HashSet<long> listaId = new HashSet<long>( listaslajdow );
            var slajdy = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Slajd>(null, x => Sql.In(x.Id, listaId)).OrderBy(x => x.Kolejnosc).ToList();
            return PartialView("FullPage", new SlajderDane(slajdy,czasanimacji, czasprzeskoku));
        }
        [Route("FullWidthSlider")]
        public ActionResult FullWidthSlider(long[] listaslajdow, int czasprzeskoku, int czasanimacji)
        {
            HashSet<long> listaId = new HashSet<long>( listaslajdow );

            var slajdy = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Slajd>(null, x => Sql.In(x.Id, listaId)).OrderBy(x=>x.Kolejnosc).ToList();
            return PartialView("FullWidthSlider", new SlajderDane(slajdy, czasanimacji, czasprzeskoku));
        }
        [Route("Suwak")]
        public ActionResult Suwak(long[] listaslajdow, int czasprzeskoku, int czasanimacji, int iloscelementowwwierszu, string rozmiarzdjecia, string dodatkoweklasycsselementykontrolki)
        {
            HashSet<long> listaId = new HashSet<long>( listaslajdow );
            var slajdy = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Slajd>(null, x => Sql.In(x.Id,listaId)).OrderBy(x => x.Kolejnosc).ToList();
            Slajder tmpS = new Slajder(listaslajdow.Length);
            tmpS.IleElementowWWierszu = iloscelementowwwierszu;
            tmpS.CzasPrzeskoku = czasprzeskoku;
            string preset = (string.IsNullOrEmpty(rozmiarzdjecia)) ? "" : "?preset=" + rozmiarzdjecia;
            return PartialView("Suwak", new SlajderDane(slajdy, czasanimacji, czasprzeskoku, tmpS, preset, dodatkoweklasycsselementykontrolki));
        }
    }
}