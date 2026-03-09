using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("SzablonyMaili")]
    public class SzablonyMailiController : SolexControler
    {
        [Route("Lista")]
        public PartialViewResult Lista()
        {
            IList<UstawieniePowiadomienia> a = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<UstawieniePowiadomienia>(null, x => x.ParametryWysylania != null);

            //sprawdzanie czy mozna kompilowac szablon - dosyc dlugo idzie ale trze to zrobic
            foreach (UstawieniePowiadomienia m in a)
            {
                if (m.WlaczoneDlaOdbiorcow)
                {
                    var wynik = MailHelper.PobierzInstancje.GenerujPodgladMaila(m, SolexHelper.AktualnyJezyk.Id, TypyPowiadomienia.Klient);
                    if (wynik == null || string.IsNullOrEmpty(wynik.TrescWiadomosci))
                    {
                        m.BladKompilacji = true;
                    }
                    else
                    {
                        m.BladKompilacji = false;
                    }
                }
            }

            a = a.OrderBy(x => !x.WlaczoneDlaOdbiorcow).ThenBy(x => !x.BladKompilacji).ToList();

            return PartialView("_Lista", a);
        }

        [Route("AktualizujListe")]
        public PartialViewResult Aktualizuj(long idZdarzenia, TypyPowiadomienia? doKogo, string pole, string wartosc)
        {
            var z = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<UstawieniePowiadomienia>(idZdarzenia);
            if (z != null)
            {
                
                if (pole == "Aktywny" || pole == "ZgodaNaZmianyPrzezKlienta")
                {
                   
                    if (doKogo.HasValue)
                    {
                        var property = z.ParametryWysylania.First().GetType().GetProperty(pole);
                        property.SetValue(z.ParametryWysylania.First(x => x.DoKogo == doKogo), bool.Parse(wartosc));
                    }
                    else
                    {
                        var property = z.GetType().GetProperty(pole);
                        property.SetValue(z, bool.Parse(wartosc));
                    }
                }
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(z);
            }
            return PartialView("SzablonMaila", z);
        }

        [Route("Podglad/{id}/{dokogo}")]
        public PartialViewResult Podglad(long id, TypyPowiadomienia dokogo)
        {
            UstawieniePowiadomienia zdarzenie = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<UstawieniePowiadomienia>(id);
            WiadomoscEmail tresc = MailHelper.PobierzInstancje.GenerujPodgladMaila(zdarzenie, SolexHelper.AktualnyJezyk.Id, dokogo);
            return PartialView("_PodgladMaila", tresc);
        }

        [Route("UstawBcc")]
        public void UstawBcc(long id, TypyPowiadomienia doKogo, string email)
        {
            var z = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<UstawieniePowiadomienia>(id);
            if (z != null)
            {
                z.ParametryWysylania.First(x => x.DoKogo == doKogo).EmailBcc = email;
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(z);
            }
        }
    }
}