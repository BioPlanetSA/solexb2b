using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Controllers
{

    public class PunktyController : SolexControler
    {
        // GET: Dokumenty
        public PartialViewResult PobierzListe(int klient, DateTime odKiedy, DateTime doKiedy, string sortowanie = "DataUtworzenia",
            KolejnoscSortowania kierunek = KolejnoscSortowania.desc, string szukanieFraza = "")
        {
            IKlient iklient = SolexBllCalosc.PobierzInstancje.Klienci.PobierzWgIdLubZalogowanyAktualnie(klient);
            List<PunktyWpisy> dok = SolexBllCalosc.PobierzInstancje.PunktyDostep.PobierzPunktyKlienta(iklient, odKiedy, doKiedy, sortowanie, kierunek, szukanieFraza);
            decimal lacznie = SolexBllCalosc.PobierzInstancje.PunktyDostep.PobierzPunktyKlientaLacznie(iklient);
     
            if (dok.Any())
            {
            //      DocumentSummary danedoWykresu = rodzaj == RodzajDokumentu.Faktura ? Dokumenty.PobierzInstancje.WygenerujeDaneDoWykresuFaktur(dok) : Dokumenty.PobierzInstancje.WygenerujeDaneDoWykresuZamowien(dok);
                return PartialView("_PunktyDane", new Tuple<List<PunktyWpisy>, decimal>(dok, lacznie));
            }
            return PartialView("_Punkty_brak");
        }

        public PartialViewResult Lista()
        {

            if (!SolexBllCalosc.PobierzInstancje.PunktyDostep.KlientMaDostepDoModulu(SolexHelper.AktualnyKlient))
            {
                return null;
            }
          
            return PartialView("_Lista");
        }
    }
}