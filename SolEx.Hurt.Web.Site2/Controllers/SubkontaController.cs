using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.PageBases;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Subkonta")]
    public class SubkontaController : SolexControler
    {
        [Route("Lista")]
        public PartialViewResult Lista()
        {
            if (SolexHelper.AktualnyKlient.KlientNadrzednyId.HasValue && !SolexHelper.AktualnyKlient.AdministratorSubkont)
            {
                return null;
            }
            return PartialView("Lista");
        }

        [Route("ListaDane")]
        public PartialViewResult ListaDane()
        {
            List<Klient> limity = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(SolexHelper.AktualnyKlient.KlientPodstawowy()).ToList();
            var miejsce = limity.FindIndex(x => x.Id == SolexHelper.AktualnyKlient.Id);
            if (miejsce != -1)
            {
                limity.RemoveAt(miejsce);
            }
            
            return PartialView("ListaDane", limity);
        }

        [Route("Edycja")]
        public PartialViewResult Edycja(int? id = null)
        {
            ViewBag.Adresy = SolexHelper.AktualnyKlient.KlientPodstawowy().Adresy;
            ViewBag.SzablonyLimitow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SzablonLimitow>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            ViewBag.SzablonyAkceptacji = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SzablonAkceptacjiBll>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            ViewBag.GrupySubkont = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SubkontoGrupa>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            ViewBag.MiejsceKosztow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<MiejsceKosztow>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            ViewBag.DomyslnyAdresId = null;
            Klient obiekt;
            if (id.HasValue)
            {

                obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(id, SolexHelper.AktualnyKlient.KlientPodstawowy());
                if (obiekt.DomyslnyAdres.Id != 0)
                {
                    ViewBag.DomyslnyAdresId = obiekt.DomyslnyAdres.Id;
                }
            }
            else
            {
              
                obiekt = new Klient();
                obiekt.KlientNadrzednyId = SolexHelper.AktualnyKlient.KlientPodstawowy().Id;
            }
       
            return PartialView("Edycja", obiekt);
        }

        [Route("Edycja")]
        [HttpPost]
        public void Edycja(Klient obiekt,long? domyslnyAdresId=null)
        {
            if (obiekt.Id == 0)
            {
                obiekt.Symbol = DateTime.Now.ToBinary().ToString(CultureInfo.InvariantCulture);
            }
            obiekt.WalutaId = SolexHelper.AktualnyKlient.WalutaId;
            obiekt.JezykId = SolexHelper.AktualnyKlient.JezykId;
            obiekt.Login = obiekt.Email;
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(obiekt);
            var lacznik= SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<KlientAdres>( x => x.KlientId == obiekt.Id && x.TypAdresu == TypAdresu.Domyslny,null);

            //adres sie nie zmienil
            if (lacznik != null && domyslnyAdresId.HasValue && domyslnyAdresId.Value==lacznik.AdresId)
            {
                return;
            }
            if (lacznik != null)
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.UsunPojedynczy<KlientAdres>(lacznik.Id);
            }
            if (domyslnyAdresId != null)
            {
                if (lacznik == null)
                {
                    lacznik=new KlientAdres();
                    lacznik.KlientId = obiekt.Id;
                    lacznik.TypAdresu=TypAdresu.Domyslny;
                   
                }
                lacznik.AdresId = domyslnyAdresId.Value;
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(lacznik);
            }
        }



        [Route("Podsumowanie")]
        public PartialViewResult PodsumowanieLimity(RodzajLimitu rodzaj)
        {
            SzablonLimitow szablonLimitow=null;
            dynamic limitWykorzystany = 0;
            switch (rodzaj)
            {
                    case RodzajLimitu.LimitIlosciZamowien:
                        szablonLimitow = Calosc.Klienci.PobierzCalkowityLimitIloscZamowien(SolexHelper.AktualnyKlient);
                        if (szablonLimitow == null)
                        {
                            return null;
                        }
                        limitWykorzystany = Calosc.Klienci.PobierzWykorzystanyLimit<int?>(SolexHelper.AktualnyKlient,szablonLimitow, rodzaj)??0;
                    break;
                    case RodzajLimitu.LimitWartosciZamowien:
                        szablonLimitow = Calosc.Klienci.PobierzCalkowityLimitWartosciZamowien(SolexHelper.AktualnyKlient);
                        if (szablonLimitow == null)
                        {
                            return null;
                        }
                        limitWykorzystany = Calosc.Klienci.PobierzWykorzystanyLimit<decimal?>(SolexHelper.AktualnyKlient, szablonLimitow, rodzaj)??0;
                    break;
            }
            var odKiedyLimit = Calosc.Klienci.WyliczOdKiedy(szablonLimitow);
            return PartialView("PodsumowanieLimity", new ParametryDoPodsumowaniaLimitow(szablonLimitow, limitWykorzystany, odKiedyLimit.AddMonths(szablonLimitow.IloscMiesiecy),rodzaj));

        }
    }
}