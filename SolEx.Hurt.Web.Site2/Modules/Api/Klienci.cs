using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Odpowiada za aktualizację klientów wysłanych w obiekcie Data jako Lista<klienci>
    /// </summary>
    /// 
    [ApiTypeDescriptor(ApiGrupy.Klienci, "Uaktualnia/dodaje  klientów", "Handler uaktualnia/dodaje  klientów w systemie.")]
    [ApiUprawnioneRole(RoleType.Pracownik)]
    public class AktualizujKlientaHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            List<Klient> data = (List<Klient>) Data;

            //return SolexBllCalosc.PobierzInstancje.Klienci.UpdateCustomers(data);
            return SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Klient>(data);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof (List<Klient>); }
        }
    }

    /// <summary>
    /// Zwraca listę ID klientów, którzy mają rabaty na platformie jako ListaID<int>
    /// </summary>
    public class PobierzKlientowZRabatami : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<RabatBLL>(null,x=>x.KlientId.HasValue).Select(a => a.KlientId).Distinct().ToList();
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof (List<long>); }
        }

    }

    /// <summary>
    /// Pobiera klientów na platformie jako słownik klientów
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Klienci, "Pobieranie listy klientów", "Handler zwraca listę klientów w systemie, którzy zostali dodani automatycznie(przez api).")]
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzKlientowHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            var ids = (HashSet<long>) Data;
            IList<Klient> tmp;
            if (ids != null)
            {
                tmp = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(Customer,x =>  Sql.In(x.Id, ids));
            }
            else
            {
                tmp = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(Customer);
            }
            return tmp.ToDictionary(x => x.Id, x => x);

        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof (HashSet<long>); }
        }

    }

    public class MailePowitalne : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.Klienci.WyslijMailePowitalneOdSzefa(Customer);
            return Status.Utworz(StatusApi.Ok);

        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof (List<IKlient>); }
        }


    }
    public class PobierzKlienciHash : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Core.Klient>(null).Select(x=>x.Id).ToList();
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<long>); }
        }

    }
}