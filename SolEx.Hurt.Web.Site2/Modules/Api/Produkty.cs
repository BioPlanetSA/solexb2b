using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje produkty wysłane w obiekcie Data jako Lista Produkt
    /// </summary>
    public class AktualizujProdukty : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<Produkt> doZmiany = (List<Produkt>)Data;
            log.DebugFormat("aktualizacja produkty początek");
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Produkt>(doZmiany);
            log.DebugFormat("aktualizacja produkty koniec");
            return Status.Utworz(StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Produkt>); }
        }
    }

    /// <summary>
    /// Pobiera produkty jako Słownik klucz int ID, wartość produkty>
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Produkty, "Pobieranie listy produktów", "Handler zwraca listę produktów w systemie, które zostały dodane automatycznie(przez API).")]
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel, RoleType.Klient)]
    public class PobierzProduktyHandler : ApiSessionBaseHandler, IDocumentApiVisible
    {
        /// <summary>
        /// Pobiera produkty na platformie
        /// </summary>

        protected override object Handle()
        {
            var ids = (HashSet<long>) Data;
            var pb = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(null, x => ids == null || ids.Contains(x.Id));
            return pb.Select(x => new Produkt(x)).ToDictionary(x => x.Id, x => x);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }

    /// <summary>
    /// Pobiera produkty dostępne dla klienta, którego ID zostało wysłane w obiekcie Data
    /// </summary>
    public class PobierzDostepneProduktyKlienta : ApiSessionBaseHandler, IDocumentApiVisible
    {
        protected override object Handle()
        {
            IKlient k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>((long) Data);
            return SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(k);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(long); }
        }
    }

    //[ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    //public class PobierzProduktyUkryteWybranychKlientow : ApiSessionBaseHandler, IDocumentApiVisible// ApiSessionBaseHandlerPobierzDane
    //{
    //    protected override object Handle()
    //    {
    //        //ProduktyUkryteSearchCriteria criteria = (ProduktyUkryteSearchCriteria)SearchCriteriaObject;
    //        Dictionary<long, produkty_ukryte> pu = new Dictionary<long, produkty_ukryte>();
    //        //int index = 0;
    //        //foreach (var s in criteria.klient_zrodlo_id)
    //        //{
    //        //    if (!s.HasValue)
    //        //    {
    //        //        continue;
    //        //    }
    //        //    IKlient k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(s.Value);
    //        //    HashSet<int> lista = ProduktyUkryteBll.PobierzInstancje.PobierzProduktyDostepneDlaKlienta(k);

    //        //    foreach (var i in lista)
    //        //    {
    //        //        pu.Add(++index, new produkty_ukryte() { klient_zrodlo_id = s, produkt_zrodlo_id = i });
    //        //    }
    //        //}
    //        return pu;
    //    }
    //   public override Type PrzyjmowanyTyp
    //    {
    //        get { return typeof(ProduktyUkryteSearchCriteria); }
    //    }
    //}

    /// <summary>
    /// Zwraca listę ID produktów jako ListaID int potrzebną do paczkowanego pobierania produktów
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzProduktyHash : ApiSessionBaseHandler, IDocumentApiVisible// ApiSessionBaseHandlerPobierzDane
    {
        protected override object Handle()
        {
            return ProduktyBazowe.GetIds().ToList();
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<long>); }
        }
    }

    /// <summary>
    /// Aktualizuje produkty wysłane w obiekcie Data jako Lista produkty>
    /// </summary>
    public class AktualizujZamienniki : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<ProduktyZamienniki> doZmiany = (List<ProduktyZamienniki>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProduktyZamienniki>(doZmiany);
            return Status.Utworz(StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<ProduktyZamienniki>); }
        }
    }
    /// <summary>
    /// Aktualizuje produkty wysłane w obiekcie Data jako Lista produkty>
    /// </summary>
    public class UsunZamienniki : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<object> doZmiany = (List<object>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<ProduktyZamienniki, object>(doZmiany);
            return Status.Utworz(StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<object>); }
        }
    }
    /// <summary>
    /// Aktualizuje produkty wysłane w obiekcie Data jako Lista Produkt>
    /// </summary>
    public class PobierzZamienniki : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktyZamienniki>(null).ToDictionary(x => x.Id, x => x);
        }
    }
    
}
