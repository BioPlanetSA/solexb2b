using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.ProfilKlienta;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("ProfilKlienta")]
    public class ProfilKlientaController : SolexControler
    {
        [NonAction]
        public ActionResult Index()
        {
            IList<ProfilKlienta> pk = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzProfilKlienta(SolexHelper.AktualnyKlient).OrderBy(x => x.TypUstawienia).ToList();
            return PartialView("profilKlienta",new Tuple<IList<ProfilKlienta>>(pk));
        }

        public class PowiadomieniaMailoweOpis
        {
            public string Nazwa { get; set; }
            public string Opis { get; set; }
            public bool Wartosc { get; set; }
            public string Typ { get; set; }
        }

        [ChildActionOnly]
        public PartialViewResult PowiadomieniaMailowe(string textzastepczy)
        {
            List<SzablonMailaBaza> powiadomienia = SolexBllCalosc.PobierzInstancje.MaileBLL.PobierzListeWszystkichPowiadomienMailowych();
            IList<UstawieniePowiadomienia> listaZdarzenWidocznych = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<UstawieniePowiadomienia>(null, x => x.ZgodaNaZmianyPrzezKlienta).Where(x =>x.ParametryWysylania!=null && x.ParametryWysylania.Any(y=>y.Aktywny)).ToList();
            var model = new List<PowiadomieniaMailoweOpis>();

            foreach (var item in listaZdarzenWidocznych)
            {
                SzablonMailaBaza powiadomienie = powiadomienia.First(x => x.Id == item.Id);
                foreach (var p in item.ParametryWysylania)
                {
                    if (p.DoKogo == TypyPowiadomienia.Klient && p.Aktywny)
                    {
                        var obiekt = new PowiadomieniaMailoweOpis();
                        obiekt.Nazwa = powiadomienie.NazwaFormatu();
                        obiekt.Opis = powiadomienie.OpisDlaKlienta();
                        obiekt.Typ = powiadomienie.GetType().Name;
                        obiekt.Wartosc = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.PowiadomieniaMailowe, powiadomienie.GetType().Name);                        
                        model.Add(obiekt);
                    }
                }
            }

            if (!model.Any())
            {
                if (string.IsNullOrEmpty(textzastepczy)) textzastepczy = "Brak powiadomień do wybrania";
                return PartialView("Tekst", SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(SolexHelper.AktualnyJezyk.Id,textzastepczy));
            }
            return PartialView("_PowiadomieniaMailowe", model);
        }

        [HttpPost]
        [Route("PowiadomieniaMailoweZapisz")]
        public ActionResult PowiadomieniaMailoweZapisz(IEnumerable<PowiadomieniaMailoweOpis> model)
        {
            var profilLista = new List<ProfilKlienta>();

            if (model != null)
            {
                foreach (var item in model)
                {
                    var profil = new ProfilKlienta(TypUstawieniaKlienta.PowiadomieniaMailowe, item.Wartosc, item.Typ, AccesLevel.Zalogowani);
                    profil.KlientId = SolexHelper.AktualnyKlient.Id;
                    profilLista.Add(profil);
                }
            }

            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProfilKlienta>(profilLista);
            return AkcjaPowrotu();
        }

        [NonAction]
        public ActionResult ZmianaUstawieniaBool(TypUstawieniaKlienta typUstawienia, string ikonaTrue,string ikonaFalse, string podpowiedzTrue = "", string podpowiedzFalse = "", string dodatkowe = "")
        {
            bool wartoscAktualna = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, typUstawienia, dodatkowe);
            ZmianaDanychBool daneWidok = new ZmianaDanychBool{Ikona=wartoscAktualna?ikonaTrue:ikonaFalse,Typ = typUstawienia,Wartosc =!wartoscAktualna,Tooltip = wartoscAktualna?podpowiedzTrue:podpowiedzFalse};
            return PartialView("ZmianaUstawieniaBool", daneWidok);
        }

        [Route("UstawWartoscUstawieniaBool/{typ}/{wartosc}")]
        [Route("UstawWartoscUstawieniaBool")]   //dla posta
        public ActionResult UstawWartoscUstawieniaBool(TypUstawieniaKlienta typ, bool wartosc, string dodatkowo="")
        {
            SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(SolexHelper.AktualnyKlient, typ, wartosc, dodatkowo);
           
            return AkcjaPowrotu();
        }

        [Route("UstawWartoscUstawienia/{typ}/{wartosc}")]
        [Route("UstawWartoscUstawienia")]
        public ActionResult UstawWartoscUstawienia(string typ, string wartosc)
        {
            TypUstawieniaKlienta typUstawienia;

            try
            {
                typUstawienia = (TypUstawieniaKlienta) Enum.Parse(typeof(TypUstawieniaKlienta), typ);
            }
            catch
            {
                throw new Exception("nie można odnaleźć typu ustawienia");
            }

            SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(SolexHelper.AktualnyKlient, typUstawienia, wartosc);

            return AkcjaPowrotu();
        }

    }
}