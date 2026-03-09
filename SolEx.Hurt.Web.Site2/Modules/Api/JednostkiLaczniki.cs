using System;
using System.Collections.Generic;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using  System.Linq;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje łączniki jednostek produktów przekazanych w obiekcie Data jako Lista<ProduktyJednostki>
    /// </summary>
    public class AktualizujJednostkiLaczniki : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
          SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProduktJednostka>((List<ProduktJednostka>)Data);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<ProduktJednostka>); }
        }
    }

    /// <summary>
    /// Pobiera łączniki jednostek produktów jako Słownik<klucz int ID, wartość ProduktyJednostki>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzJednostkiLaczniki : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {
            IDictionary<long, ProduktJednostka> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktJednostka>(null).ToDictionary(x => x.Id, x => x);

            return levels;
        }
    }

    /// <summary>
    /// Usuwa łączniki jednostek produktów podanych w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunJednostkiLaczniki : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long> ids = (HashSet<long>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<ProduktJednostka, long>(ids.ToList());
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
}