using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ServiceStack.Common.Extensions;
using ServiceStack.Text;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Filtry;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.ProfilKlienta;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Filtry")]
    public class FiltryController : SolexControler
    {
        [ChildActionOnly]
        public PartialViewResult PokazUkryjFiltry(ParametryPrzekazywaneDoListyProduktow parametry)
        {
            //czy w ogoel pokazywac filtry
            //todo: dać ustawienia na kontrolce liscie produtk czy w ogole pokazywac przelacznik do ukrywania / pokazyacni filtrow - moze byc bez sensu jesli ktos nie ma filtrow
            bool pokazac = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.WidocznoscFiltrow);
            if (pokazac)
            {
                //czy w ogole sa jakie filtry do pokazania
                if (!parametry.KontrolkaProduktowJakoListaProduktow.PokazujFiltryGdyBrakKryteriowProduktow && parametry.BrakKryteriowWyboru)
                {
                    return null;
                }

                if (parametry.kategoria.HasValue && !parametry.KategoriaObiekt.PokazujFiltry)
                {
                    return null;
                }

                //nie spradzmy tu czy w ogole sa jakies filtry bo to by zamulalo strasznie liste - zakladamy ze jak doszlo tu to sa jakies filtry i przelacznik trzeba pokazac
            }

            string klasyCssPrzycisku = pokazac ? "ukrywanie-pokazywanie-filtrow" : "ukrywanie-pokazywanie-filtrow filtry-niepokazywane";
            
            if (SolexHelper.AktualnyKlient.Role.Contains(RoleType.Przedstawiciel))
            {
                klasyCssPrzycisku += " hidden-md-down";
            }
            
            ZmianaDanychBool zmianaDanych = new ZmianaDanychBool
            {
                Typ = TypUstawieniaKlienta.WidocznoscFiltrow,
                Ikona = pokazac ? "fa-angle-up" : "fa-angle-down filtr-wybrany",
                Tooltip = pokazac ? "UKRYJ FILTRY" : "POKAŻ FILTRY",
                Wartosc = !pokazac,
                OpisPrzycisku = pokazac ? "UKRYJ FILTRY" : "POKAŻ FILTRY",
                klasaCss = klasyCssPrzycisku
            };

            zmianaDanych.PokazTylkoLink = true;

            return PartialView("../ProfilKlienta/ZmianaUstawieniaBool", zmianaDanych);
        }


        [Route("FiltrowanieDane")]
        [OutputCacheSolex(TypDanychDoCache.Filtry)]
        public PartialViewResult FiltrowanieDane(ParametryPrzekazywaneDoListyProduktow parametry )
        {
            if (!SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.WidocznoscFiltrow))
            {
                return null;
            }

            //todo: tymczasowo 
            //if (!parametry.KontrolkaProduktowJakoListaProduktow.PokazujFiltryGdyBrakKryteriowProduktow && parametry.BrakKryteriowWyboru)
            //{
            //    return null;
            //}
            
            List<AtrybutBll> atrybuty = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWyfiltrowane(parametry.filtry, parametry.kategoria, StaleFiltryAktualnieWybrane(), parametry.szukane, SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id, parametry.szukanaWewnetrzne);

            if (atrybuty.IsEmpty())
            {
                return null;
            }
            var wynik = PartialView("Filtry/FiltryWspolnaCzesc", new ParametryDoStalychFiltrow(atrybuty, parametry.filtry, false));

            //cachowanie wyniku
            string kluczCache = parametry.KluczDoCachuFiltrow(SolexHelper.AktualnyKlient);
            SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt(kluczCache, wynik);

            return wynik;
        }

        [Route("PokazAktywneFiltry")]
        public PartialViewResult PokazAktywneFiltry(string naglowek="",string stopka="", ParametryPrzekazywaneDoListyProduktow parametry = null, bool brakProduktow = true)
        {
            Dictionary<int, HashSet<long>> stale = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzStaleFiltry(SolexHelper.AktualnyKlient);
            Dictionary<Atrybut, List<Cecha>> wybraneStale = new Dictionary<Atrybut, List<Cecha>>();
            Dictionary<string, string> szukane = new Dictionary<string, string>();
            foreach (KeyValuePair<int, HashSet<long>> atrId in stale)
            {
                Atrybut atr = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Atrybut>(atrId.Key);

                if (atr == null)
                {
                    //juz nie istnieje taki filtr jaki byl wybrany - trzeba go pomijac - chcemy wywalic blad bo tak nie powinno byc
                    throw new Exception($"Klient id: {SolexHelper.AktualnyKlient.Id} ma stały filtr który już nie istnieje w systemie - pomijam. Filtr atrybutID: {atrId.Key} cechy: {atrId.Value.ToCsv()}");
                }

                IList<Cecha> cechy = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Cecha>(SolexHelper.AktualnyKlient, x => atrId.Value.Contains(x.Id));
                wybraneStale.Add(atr,cechy.ToList());
            }

            Dictionary<Atrybut, List<Cecha>> wybraneFiltry = new Dictionary<Atrybut, List<Cecha>>();
            KategorieBLL wybraneKatgorie = null;
            if (parametry != null)
            {
                if (parametry.filtry != null)
                {
                    foreach (var filtr in parametry.filtry)
                    {
                        if (filtr.Value != null && filtr.Value.Any())
                        {
                            Atrybut atr = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Atrybut>(filtr.Key,SolexHelper.AktualnyJezyk.Id);
                            IList<Cecha> cechy = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Cecha>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, x => filtr.Value.Contains(x.Id));
                            wybraneFiltry.Add(atr, cechy.ToList());
                        }
                    }
                }

                if (!string.IsNullOrEmpty(parametry.szukane) || !string.IsNullOrEmpty(parametry.szukanaWewnetrzne))
                {
                    if (!string.IsNullOrEmpty(parametry.szukane))
                        szukane.Add("SzukanaFrazaGlobalnie", parametry.szukane);
                    if (!string.IsNullOrEmpty(parametry.szukanaWewnetrzne))
                        szukane.Add("SzukanaFrazaWewnatrzKategorii", parametry.szukanaWewnetrzne);
                }

                wybraneKatgorie = parametry.KategoriaObiekt;
            }

            if (!wybraneFiltry.Any() && !wybraneStale.Any() && !szukane.Any() && wybraneKatgorie == null)
                return null;

            return PartialView("PokazAktywneFiltry", new ParametryDoAktywnychFiltrow(wybraneStale, wybraneFiltry,wybraneKatgorie, szukane, naglowek,stopka, brakProduktow));
        }
    }
}