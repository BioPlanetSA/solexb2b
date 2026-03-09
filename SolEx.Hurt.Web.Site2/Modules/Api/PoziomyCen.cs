using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Dodaje/modyfikuje poziom cen w tabeli poziomy cen wysłanych w obiekcie Data jako Lista<poziomy_cen>
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Produkty, "Aktualizacja listy poziomów cen", @"Handler uaktualnia/dodaje  poziomy cen w systemie.Produkty mogę mieć ceny,
 dodawane wyłacznie dla poziomów cen już zdefiniowanych, w przeciwnym wypadku system zwraca błąd")]
    public class DodajPoziomyCenHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {
          SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<PoziomCenowy>((List<PoziomCenowy>)Data);
            return null;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<PoziomCenowy>); }
        }
    }

    /// <summary>
    /// Zwraca listę poziomów cen jako Słownik<klucz int ID, wartość poziomy_cen>
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Produkty, "Pobieranie poziomów cen w systemie", "Handler zwraca listę poziomów cen w systemie.")]
    class PobierzPoziomyCenHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<PoziomCenowy>(null).ToDictionary(x => x.Id, x => x);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Dictionary<int, PoziomCenowy>>); }
        }
    }

    /// <summary>
    /// Usuwa poziomy cen wysłane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunPoziomyCen : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<object> ids = (List<object>)Data;

            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<PoziomCenowy, object>(ids);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<object>); }
        }
    }
}