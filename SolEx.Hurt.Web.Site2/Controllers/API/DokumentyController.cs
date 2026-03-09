using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers.API
{
    [RoutePrefix("api2/dokumenty")]
    [AutoryzacjaSolex]
    public class DokumentyController : SolexAPIController
    {
        [HttpGet]
        [Route("HashDokumentow")]
        public Dictionary<int, long> HashDokumentow()
        {
            return SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzSumyKontrolneDokumentow();
        }

        [HttpDelete]
        [Route("usun")]
        public bool UsunCechy(List<int> listaDokumentow)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<HistoriaDokumentu, int>(listaDokumentow);
            return true;
        }

        [AcceptVerbs("GET")]
        [Route("PobierzOfertyPozycje/{klienci}/{statusyDokumentow}/{tylkoAktualneOferty}")]
        public Dictionary<long, List<HistoriaDokumentuProdukt>> PobierzOfertyPozycje(long[] klienci, int[] statusyDokumentow = null, bool tylkoAktualneOferty = false)
        {
            //if (klienci == null || klienci.IsEmpty())
            //{
            //    throw new APIException("Podanie klientów jest wymagane - nie podano listy klientów id.");
            //}
            Dictionary<long, List < HistoriaDokumentuProdukt>> wynik = new Dictionary<long, List<HistoriaDokumentuProdukt>>();
            string zapytanieKlient = "select Id from Klient where Aktywny=1";
            if (klienci != null && klienci[0]!=0)
            {
                zapytanieKlient = string.Join(",",klienci);
            }
            string dodatkoweInformacje = string.Empty;
            if (statusyDokumentow != null && statusyDokumentow.Any())
            {
                dodatkoweInformacje = $" AND StatusId in ({string.Join(",",statusyDokumentow)}) ";
            }

            if (tylkoAktualneOferty)
            {
                dodatkoweInformacje += " AND (TerminPlatnosci IS NULL OR TerminPlatnosci >= CONVERT(date, getdate()))";
            }

            string coZDokumentow = "*";

            //StringBuilder sql = new StringBuilder("SELECT DISTINCT h.KlientId, hp.ProduktId from HistoriaDokumentuProdukt hp LEFT JOIN HistoriaDokumentu h on hp.DokumentId = h.Id WHERE  h.KlientId in ({1}) ");
            string zapytanie = "SELECT {0} from  HistoriaDokumentu  WHERE KlientId in ({1}) {2} ";

            Dictionary<long, long> walutyKlientow = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.SqlList<Klient>("select * from Klient where Aktywny=1").ToDictionary(x=>x.Id,x=>x.WalutaId);

            try
            {
                List<string>dokumentyPominiete = new List<string>();
                Dictionary<int, HistoriaDokumentu> dokumenty = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.SqlList<HistoriaDokumentu>(string.Format(zapytanie, coZDokumentow, zapytanieKlient, dodatkoweInformacje)).ToDictionary(x=>x.Id, x=>x);
                //Dictionary<string, string> dokumentye = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Dictionary<string, lostringng>(string.Format(zapytanie, coZDokumentow, zapytanieKlient, dodatkoweInformacje));
                coZDokumentow = "Id";
                Dictionary<int, List<HistoriaDokumentuProdukt>> pozycje = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.SqlList<HistoriaDokumentuProdukt>($"select * from HistoriaDokumentuProdukt where DokumentId in ({string.Format(zapytanie, coZDokumentow, zapytanieKlient, dodatkoweInformacje)} )").GroupBy(x => x.DokumentId).ToDictionary(x => x.Key, x => x.ToList());

                foreach (var d in dokumenty)
                {
                    long walutaKlienta;
                    if (walutyKlientow.TryGetValue(d.Value.KlientId, out walutaKlienta))
                    {
                        if (d.Value.WalutaId != walutaKlienta)
                        {
                            dokumentyPominiete.Add(d.Value.NazwaDokumentu);
                            continue;
                        }
                        List<HistoriaDokumentuProdukt> poz;
                        if (pozycje.TryGetValue(d.Key, out poz))
                        {
                            wynik.Add(d.Value.KlientId, poz);
                        }

                    }
                }
                if (dokumentyPominiete.Any())
                {
                    LogiFormatki.PobierzInstancje.LogujError(new Exception($"Pominięto {dokumentyPominiete.Count} dokumentów ofertowych waluta dokumentu nie zgadza się z walutą klienta. Pominięte dokumenty to: {string.Join(",",dokumentyPominiete)}"));
                }
                return wynik;

            } catch (Exception e)
            {
                SolexBllCalosc.PobierzInstancje.Log.Error($"Błąd pobierania dla SQL: {zapytanie}. Komunikat: {e.Message}.", e);
                throw new APIException("Błąd pobierania z bazy danych o produktach dla ofert.");
            }
        }
    }
}

