using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Dodaje dokumenty na platformie wysłane w obiekcie Data jako Lista historia_dokumenty>
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Dokumenty, "Aktualizacja dokumentów w systemie", "Dodaje/ aktualizuje faktury i zamówienia na platformie.")]
    public class DodajDokumentHandler : ApiSessionBaseHandler, IDocumentApiVisible
    {

        protected override object Handle()
        {
            List<KlasaOpakowujacaDokumentyDoWyslania> doZmiany = (List<KlasaOpakowujacaDokumentyDoWyslania>)Data;

            SolexBllCalosc.PobierzInstancje.DokumentyDostep.Aktualizuj(doZmiany);
            return null;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<KlasaOpakowujacaDokumentyDoWyslania>); }
        }
    }

    /// <summary>
    /// Pobiera dokumenty z platformy jako Lista<historia_dokumenty/>
    /// </summary>
    class PobierzDokumenty : ApiSessionBaseHandlerPobierzDane, IDocumentApiVisible
    {
        protected override object Handle()
        {
            var kryteria = (DokumentySearchCriteria) SearchCriteriaObject;
            var warunki = kryteria.ZbudujWarunek();
            return SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzSlownikDokumentowIPozycji(Customer, warunki);
            
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<HistoriaDokumentu>); }
        }

        protected override Type SearchCriteriaType
        {
            get { return typeof(DokumentySearchCriteria); }
        }
    }

    /// <summary>
    /// Usuwa wybrane dokumenty wysłane w obiekcie Data jako ListaID int
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Dokumenty, "Usunięcie dokumentów na platformie", "Handler usuwa wybrane dokumenty na platformie.")]
    public class UsunDokumentHandler : ApiSessionBaseHandler, IDocumentApiVisible
    {
        protected override object Handle()
        {
            var dane = (HashSet<int>) Data;
            if (dane != null && dane.Any())
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.Usun<HistoriaDokumentu, int>(dane.ToList());
              
            }

            return null;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<int>); }
        }
    }

    /// <summary>
    /// Pobiera z platformy id dokumentów i ich sumy kontrolne wyliczone metodą Tools.PobierzInstancje.PoliczHashDokumentu jako Słownik klucz int ID, wartość string hash
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Dokumenty, "Pobieranie listy dokumentów na platformie", @"Handler zwraca listę dokumentów w systemie wraz z hashem wyliczonym z 
(id,wartosci netto dokumentu,wartosci naleznej, informacji czy dokument jest rozliczony/zrealizowany).")]
    public class DokumentStatusHandler : ApiSessionBaseHandler, IDocumentApiVisible
    {
        protected override object Handle()
        {
            Dictionary<int, long> docHash = SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzSumyKontrolneDokumentow();

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            return docHash;
        }
    }

    ///// <summary>
    ///// Pobiera listę niezapłaconych faktur i wysyła dla nich powiadomienia
    ///// </summary>
    //public class DokumentyPlatnosciHandler : ApiSessionBaseHandler
    //{
    //    public override Type PrzyjmowanyTyp
    //    {
    //        get { return typeof(List<int?>); }
    //    }
    //    protected override object Handle()
    //    {
    //        int? dniNiezaplacone = ((List<int?>)Data)[0];
    //        int? dniprzeterminowane = ((List<int?>)Data)[1];
    //        int? ileDniPonowneWyslanie = ((List<int?>)Data)[2];

    //        if (!dniNiezaplacone.HasValue || !dniprzeterminowane.HasValue)
    //        {
    //            throw new Exception("Parametry dniprzeterminowane oraz dniNiezaplacone są wymagane");
    //        }

    //        SolexBllCalosc.PobierzInstancje.DokumentyDostep.WyslijMailaOPrzeterminowanychFakturach(dniNiezaplacone.Value,dniprzeterminowane.Value,ileDniPonowneWyslanie);
    //        return null;
    //    }

    //}


    /// <summary>
    /// Pobiera dokumenty z platformy jako Lista historia_dokumenty>
    /// </summary>
    public class PobierzPozycjeDokumentowHandler : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<int> idDokumentow = (HashSet<int>) Data;
            return SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzSumyKontrolnePozycjiDokumentow(idDokumentow);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<int>); }
        }
    }
    /// <summary>
    /// Pobiera dokumenty z platformy jako Lista<historia_dokumenty/>
    /// </summary>
    public class DodajDokumentyProduktyHandler : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<HistoriaDokumentuProdukt> dokumentyProdukty = (List<HistoriaDokumentuProdukt>)Data;
            return SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(dokumentyProdukty);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<HistoriaDokumentuProdukt>); }
        }
    }


}
