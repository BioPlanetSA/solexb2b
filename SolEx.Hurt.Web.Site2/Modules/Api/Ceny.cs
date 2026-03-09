using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje ceny wybranego klienta podane w obiekcie Data jako Lista<flat_ceny/>
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Klienci, "Uaktualina ceny klienta", "Handler uaktualnia ceny klientów w systemie.")]
    public class AktualizujCenyKlientaHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            List<FlatCeny> ceny = (List<FlatCeny>) Data;

            if (ceny.Any())
            {
                try
                {
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(ceny);
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Błąd zapisu przeliczonych cen (flatCeny)");
                    log.Error(e);
                    throw;
                }
            }

            return Status.Utworz(StatusApi.Ok);
        }

        public override Type PrzyjmowanyTyp => typeof(List<FlatCeny>);
    }
    [ApiTypeDescriptor(ApiGrupy.Klienci, "Usuwa ceny klienta", "Handler uaktualnia ceny klientów w systemie.")]
    public class UsunKlientaHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            List<long> ids = (List<long>) Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<FlatCeny, long>(ids);

            return Status.Utworz(StatusApi.Ok);
        }

        public override Type PrzyjmowanyTyp => typeof(List<long>);
    }


    [ApiTypeDescriptor(ApiGrupy.Klienci, "Pobierz ceny klienta wyliczone w erp", "Handler uaktualnia ceny klientów w systemie.")]
    public class PobierzCenyKlientaWyliczoneERPHandler : ApiSessionBaseHandlerPobierzDane, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            var ids = (List<long>) Data;

            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<FlatCeny>(null, x=> Sql.In(x.Id, ids)).ToDictionary(x => x.Id, x => x);
        }
        public override Type PrzyjmowanyTyp => typeof(List<long>);

        protected override Type SearchCriteriaType => typeof(CustomerPriceSearchCriteria);
    }

    [ApiTypeDescriptor(ApiGrupy.Klienci, "Pobierz ceny klienta wyliczone w erp id", "Handler uaktualnia ceny klientów w systemie.")]
    public class PobierzCenyKlientaWyliczoneERPHandlerIds : ApiSessionBaseHandlerPobierzDane, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            var tmp = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<FlatCeny>(null);
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<FlatCeny>(null).Select(x=>x.Id).ToList();
        }
        public override Type PrzyjmowanyTyp => typeof(List<long>);

        protected override Type SearchCriteriaType => typeof(CustomerPriceSearchCriteria);
    }
    ///// <summary>
    ///// Odpowiada za aktualizacje poziomów cen poszczególnych produktów podanych w obiekcie Data jako Lista ceny_poziomy
    ///// </summary>
    //[ApiTypeDescriptor(ApiGrupy.Produkty, "Aktualizacja cen towarów", "Handler uaktualnia/dodaje  ceny towarów w systemie.")]
    //public class AktualizujPoziomyCenProduktuHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    //{
    //    protected override object Handle()
    //    {
    //        SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<CenaPoziomu>((List<CenaPoziomu>)Data);
    //        return Status.Utworz(StatusApi.Ok);
    //    }

    //    public override Type PrzyjmowanyTyp => typeof(List<CenaPoziomu>);
    //}

    /// <summary>
    /// Pobiera ceny produktów dla wybranego klienta (lub logując się przedstawicielem pobiera ceny wszystkich klientów tego przedstawiciela) w formacie Lista flat_ceny
    /// Wykorzystywane np w mPlanet
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzCenyHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {
            List<FlatCeny> levels = new List<FlatCeny>();
            HashSet<long> ids = (HashSet<long>) Data;
            if (ids == null || !ids.Any())
            {
               // kl = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>Wszystkich(Customer.klient_id);
                throw new Exception("Brak podanego id klienta dla którego chcemy pobrać ceny");
            }

            var kl = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(Customer,x=> Sql.In(x.Id,ids));
            foreach (var t in kl)
            {
                levels.AddRange(SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktKlienta>(t).Select(pk => new FlatCeny(pk.FlatCeny)));
            }
            return levels;
        }

        public override Type PrzyjmowanyTyp => typeof(HashSet<long>);
    }

    ///// <summary>
    ///// Pobiera poziomy cen dla produktów w formie Słownik klucz int ID, wartość ceny_poziomy
    ///// </summary>
    //[ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    //public class PobierzCenyProduktow : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    //{
    //    public override Type PrzyjmowanyTyp => typeof(HashSet<long>);

    //    protected override object Handle()
    //    {
    //        HashSet<long> ids = (HashSet<long>)Data;
    //        Dictionary<long, CenaPoziomu> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<CenaPoziomu>(null,x=> Sql.In(x.Id, ids)).ToDictionary(x => x.Id, x => x);
    //        return levels;
    //    }
    //}

    ///// <summary>
    ///// Pobiera poziomy cen dla wszystkich produktów w formie Słownika, dla podanego ID poziomu cenowego
    ///// </summary>
    //[ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    //public class PobierzCenyProduktowLista : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    //{
    //    public override Type PrzyjmowanyTyp => typeof(int);

    //    protected override object Handle()
    //    {
    //        int poziom = (int) Data;
    //        IList<CenaPoziomu> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<CenaPoziomu>(null, x => x.PoziomId == poziom);
    //        return levels;
    //    }
    //}


    ///// <summary>
    ///// Pobiera poziomy cen dla produktów w formie Słownik klucz int ID, wartość ceny_poziomy
    ///// </summary>
    //[ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    //public class PobierzCenyProduktowID : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    //{
 
    //    protected override object Handle()
    //    {
    //        var levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<CenaPoziomu>(null).Select(x=>x.Id).ToList();

    //        return levels;
    //    }
    //}
    /// <summary>
    /// Usuwa poziomy cen wybranych produktów przekazanych w obiekcie Data jako ListaID<int/>
    /// </summary>
    //[ApiTypeDescriptor(ApiGrupy.Produkty, "Usunięcie poziomów cen na platformie", "Handler usuwa wybrane poziomy cen na platformie.")]
    //public class UsunCenyPoziomy : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    //{

    //    protected override object Handle()
    //    {
    //        var cenyDoUsuniecia = (List<object>) Data;
    //        SolexBllCalosc.PobierzInstancje.DostepDane.Usun<CenaPoziomu, object>(cenyDoUsuniecia);
    //        return null;
    //    }

    //    public override Type PrzyjmowanyTyp => typeof(List<object>);
    //}
}