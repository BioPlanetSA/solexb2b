using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje faktury elektroniczne wysłane w obiekcie Data jako Lista<StatusDokumentuPDF>, zapisuje pliki PDF na serwerze, wysyła powiadomienia o utworzeniu faktury w PDF
    /// </summary>
    public class AktualizujFakturyElekroniczne : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            var doaktument = (List<StatusDokumentuPDF>) Data;
            if (doaktument.IsEmpty())
            {
                return null;
            }

            Dictionary<int,DokumentyBll> dokumentyDB = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DokumentyBll>(Customer, x => Sql.In(x.Id, doaktument.Select(z => z.IdDokumentu))).ToDictionary(x=>x.Id,x=>x);

            foreach (var dok in doaktument)
            {
                if (string.IsNullOrEmpty(dok.Rozszerzenie) && SolexBllCalosc.PobierzInstancje.Konfiguracja.WysylajPowiadomienieFakturaGdyBrakPdf)
                {
                    log.DebugFormat("Brak załącznika dla dokumentu {0}", dok.IdDokumentu);
                    continue;
                }
                var dokument = dokumentyDB[dok.IdDokumentu];
                if (!string.IsNullOrEmpty(dok.Rozszerzenie))
                {
                    string sciezka = SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzSciezkePliku(dokument.Id, dok.Rozszerzenie);
                    PlikiBase64.Base64ToFile(dok.DaneBase64, sciezka);
                }
            }
            //foreach (var dok in dowylania)
            //{
            //    //throw new NotImplementedException("Brak powiadomienia");
            //    if (Dokumenty.PobierzInstancje.CzyWyslacMailaONowymDokumencie(dok))
            //    {
            //        WysylanieWiadomosciEmail.NowyDokument(dowylania);
            //    }
            //    //PowiadomieniaBLL.PobierzInstancje.WyslijMailaONowymDokumencie(dok);
            //    if (dok as DokumentyBll != null)
            //    {
            //        (dok as DokumentyBll).DataWyslaniaDokumentu = DateTime.Now;
            //    }
            //}
            //Dictionary<int, List<DokumentyBll>> wynik = new Dictionary<int, List<DokumentyBll>>();
            //foreach (var dok in dowylania.Where(dok => Dokumenty.PobierzInstancje.CzyWyslacMailaONowymDokumencie(dok)))
            //{
            //    if (!wynik.ContainsKey(dok.DokumentPlatnikId))
            //    {
            //        wynik.Add(dok.DokumentPlatnikId, new List<DokumentyBll>(){dok});
            //    }
            //    else
            //    {
            //        wynik[dok.DokumentPlatnikId].Add(dok);
            //    }
            //}
            //foreach (var dokum in wynik)
            //{
            //    WysylanieWiadomosciEmail.NowyDokument(dokum.Value);
            //    foreach (var dokument in dokum.Value)
            //    {
            //        if (dokument as DokumentyBll != null)
            //        {
            //            (dokument as DokumentyBll).DataWyslaniaDokumentu = DateTime.Now;
            //        }
            //    }
            //}


            return Status.Utworz(StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<StatusDokumentuPDF>); }
        }
    }

    public class WysylaniePowiadomienONowychFakturach : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            HashSet<int> kategorie = (HashSet<int>)Data;
            SolexBllCalosc.PobierzInstancje.DokumentyDostep.WysylaniePowiadomienONowychFakturach(kategorie);
            return Status.Utworz(StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<int>); }
        }
    }


    /// <summary>
    /// Zwraca listę faktur elektronicznych do wydrukowania w ERP jako Lista<StatusDokumentuPDF>
    /// </summary>
    public class FakturyElekroniczneDlaKogo : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            DateTime dataOdKiedy = SolexBllCalosc.PobierzInstancje.Konfiguracja.OdKiedyDrukowacPdf;
            //var nieaktywniklienci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null, x => !x.Aktywny).Select(x => x.Id).ToList();
            int maksimumwydrukow = SolexBllCalosc.PobierzInstancje.Konfiguracja.MaksimumWydrukowPDF;
            var dokumenty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentu>(Customer, x => x.Rodzaj == RodzajDokumentu.Faktura && x.DataUtworzenia>=dataOdKiedy &&
                Sql.InSubquery(x.KlientId, new SqlServerExpressionVisitor<Klient>().Select(y=>y.Id).Where(y=>y.Aktywny))).ToList();

            var wynik = new List<StatusDokumentuPDF>();
            foreach (var dokument in dokumenty)
            {
                try
                {
                    if (!SolexBllCalosc.PobierzInstancje.DokumentyDostep.IstniejeZalacznik(dokument.Id, "pdf"))
                    {
                           log.DebugFormat("Nie znalezion pdf dokumentu {0}",dokument.Id);
                        string symboljezyka = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie[SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(dokument.DokumentPlatnikId).JezykId].Symbol;
                        wynik.Add(new StatusDokumentuPDF {IdDokumentu = dokument.Id,SymbolJezykaWydruku =symboljezyka,DataWystawniaDokumentu = dokument.DataUtworzenia.Date});
                    }
                    if (wynik.Count >= maksimumwydrukow)
                    {
                        break;
                    }

                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            return wynik;
        }

        private DateTime CzasNaZmiany
        {
            get { return DateTime.Now.AddMinutes(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SferaModulOkresZmianyMinuty); }
            
        }
    }
}
