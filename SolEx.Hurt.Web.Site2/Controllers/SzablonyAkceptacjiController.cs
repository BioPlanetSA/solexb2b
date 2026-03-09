
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("SzablonyAkceptacji")]
    public class SzablonyAkceptacjiController : SolexControler
    {
        // GET: SzablonyAkceptacji
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
            var limity = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SzablonAkceptacjiBll>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            return PartialView("ListaDane", limity);
        }
        [Route("Usun/{id}")]
        public void Usun(long id)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.UsunPojedynczy<SzablonAkceptacjiBll>(id);
        }
        [Route("Edycja")]
        public PartialViewResult Edycja(int? id = null)
        {

            SzablonAkceptacjiBll obiekt;
            if (id.HasValue)
            {
                obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<SzablonAkceptacjiBll>(id, SolexHelper.AktualnyKlient.KlientPodstawowy());
            }
            else
            {
                obiekt = new SzablonAkceptacjiBll();
                obiekt.Tworca = SolexHelper.AktualnyKlient.KlientPodstawowy().Id;
            }
            obiekt.Klienci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            //obiekt.Klienci.Add((Klient)SolexHelper.AktualnyKlient.KlientPodstawowy());
            return PartialView("Edycja", obiekt);
        }

        [Route("Edycja")]
        [HttpPost]
        public void Edycja(SzablonAkceptacjiBll obiekt, int[] poziomAkceptacji, int[] klient, int[] poziom)
        {
            if (obiekt.Id != 0)
            {
                if (poziomAkceptacji != null)
                {
                    SolexBllCalosc.PobierzInstancje.DostepDane.UsunWybrane<SzablonAkceptacjiPoziomy, int>(x => x.SzablonAkceptacjiId == obiekt.Id && !poziomAkceptacji.Contains(x.Id));
                }
                else
                {
                    throw new Exception("Nie można zapisać szablonu akceptacji bez ustawionych poziomów");
                    //SolexBllCalosc.PobierzInstancje.DostepDane.UsunWybrane<SzablonAkceptacjiPoziomy, int>(x => x.SzablonAkceptacjiId == obiekt.Id);
                }
                
            }
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(obiekt);
        
            List<SzablonAkceptacjiPoziomy> nowe=new List<SzablonAkceptacjiPoziomy>();
            for (int i = 0; i < poziomAkceptacji.Length; i++)
            {
                SzablonAkceptacjiPoziomy tmp=new SzablonAkceptacjiPoziomy();
                tmp.Id = poziomAkceptacji[i];
                tmp.SzablonAkceptacjiId = obiekt.Id;
                tmp.Poziom = i + 1;

                nowe.Add(tmp);

            }
            for (int i = 0; i < poziom.Length; i++)
            {
                if (poziom[i] < 0)
                {
                    break;
                }
                var ktory = poziom[i] - 1;
                nowe[ktory].Klienci.Add(klient[i]);
            }
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<SzablonAkceptacjiPoziomy>(nowe);
            //var obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<SzablonLimitow>(id, SolexHelper.AktualnyKlient);
            //return PartialView("Edycja", obiekt);
        }
    }
}