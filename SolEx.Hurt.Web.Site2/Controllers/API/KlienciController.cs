using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;
using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers.API
{
    [RoutePrefix("api2/klienci")]
    public class KlienciController : SolexAPIController
    {
        [HttpGet]
        [Route("")]
        [ObslugaStronnicowania]
        [AutoryzacjaSolex]
        public List<Klient> Klienci()
        {
            SortowanieKryteria<Klient> sortowanie = new SortowanieKryteria<Klient>(x => x.Id, KolejnoscSortowania.asc, "Id");
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(SolexHelper.AktualnyKlient, x => x.Id > 0, new List<SortowanieKryteria<Klient>> { sortowanie }, Paczka, IloscElementow).ToList();
        }

        [HttpPost]
        [Route("aktualizuj")]
        [AutoryzacjaSolex]
        public bool AktualizujKlientow(List<Klient> listaKlientow)
        {
            try
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Klient>(listaKlientow);
            }
            catch (Exception ex)
            {
                SolexBllCalosc.PobierzInstancje.Log.Error( $"Błąd aktualizacji klientów: " + ex.Message);
                HttpResponseMessage m = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                m.Content = new StringContent(string.Format("Błąd przy aktualizacji klientów. Sprawdz logi serwera."));
                throw new HttpResponseException(m);
            }
            return true;
        }

        [HttpPost]
        [Route("Zaloguj/{id}/{nazwaKomputera}")]
        public Guid Zaloguj(string id, string nazwaKomputera)
        {
            Core.Klient klient = null;
            try
            {
                klient = KlienciHelper.PobierzInstancje.PobierzKlientaDoZalogowania(null, false, id);
            }
            catch (Exception )
            {
                HttpResponseMessage m = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                m.Content = new StringContent(string.Format("Brak klienta o kluczu sesji: {0}", id));
                throw new HttpResponseException(m);
            }
            klient.DataOstatniegoLogowania = DateTime.Now;
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(klient);
            return SolexHelper.ZalogujKlienta(klient, SesjaHelper.PobierzInstancje.IpKlienta, nazwaKomputera);

        }
        [HttpPost]
        [Route("wyloguj/{id}")]
        public bool Wyloguj(Guid id)
        {
            SolexHelper.Wyloguj(id);
            return true;

        }
    }
}
