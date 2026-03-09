using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{  /// <summary>
    /// Dodaje/modyfikuje Waluty wysłanych w obiekcie Data jako Lista<poziomy_cen>
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Produkty, "Aktualizacja Walut", @"Handler uaktualnia/dodaje Waluty w systemie.")]
    public class AktualizujWalute : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Waluta>((List<Waluta>)Data);
            return null;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Waluta>); }
        }
    }

    /// <summary>
    /// Zwraca listę walut jako Słownik<klucz int, wartość Waluta>
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Produkty, "Pobieranie walut w systemie", "Handler zwraca listę walut w systemie.")]
    class PobierzWalute : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Waluta>(null).ToDictionary(x=>x.Id, x=>x);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Dictionary<int, Waluta>>); }
        }
    }

    /// <summary>
    /// Usuwa poziomy cen wysłane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunWalute : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long> ids = (HashSet<long>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Waluta, long>(ids.ToList());
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
}