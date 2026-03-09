using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje kategorie produktów wysłanych w obiekcie Data jako Lista<produkty_kategorie>
    /// </summary>
    public class AktualizujProduktyKategorie : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<ProduktKategoria> doZmiany = (List<ProduktKategoria>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProduktKategoria>(doZmiany);
            
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<ProduktKategoria>); }
        }
    }

    /// <summary>
    /// Pobiera kategorie produktów jako Słownik<klucz int ID, wartość produkty_kategorie>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Przedstawiciel, RoleType.Administrator)]
    public class PobierzProduktyKategorie : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible//ApiSessionBaseHandlerPobierzDane
    {
      protected override object Handle()
        {
            IDictionary<long, ProduktKategoria> kategorieProduktu = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktKategoria>(null).ToDictionary(x=>x.Id,x=>x);
            return kategorieProduktu;
        }
    }

    /// <summary>
    /// Usuwa kategorie produktów wysłane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunProduktyKategorie : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<object> ids = (List<object>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<ProduktKategoria, object>(ids);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<object>); }
        }
    }
}
