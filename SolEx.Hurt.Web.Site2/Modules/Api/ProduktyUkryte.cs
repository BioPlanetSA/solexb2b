using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje produkty ukryte wysłane w obiekcie Data jako Lista<produkty_ukryte>
    /// </summary>
    public class AktualizujProduktyUkryte : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<ProduktUkryty> doZmiany = (List<ProduktUkryty>)Data;
            SolexBllCalosc.PobierzInstancje.ProduktyUkryteBll.ZapiszAktualizuj(doZmiany.ToList());
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<ProduktUkryty>); }
        }
    }

    /// <summary>
    /// Pobiera produkty ukryte jako Słownik<klucz int ID, wartość produkty_ukryte>
    /// Opcjonalnie można podać w obiekcie Data listę identyfikatorów klientów (jako Lista<int> ) dla których mają być pobrane produkty ukryte
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzProduktyUkryte : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible //ApiSessionBaseHandlerPobierzDane
    {
        protected override object Handle()
        {
            List<long> idKlientow = (List<long>)Data;
            Dictionary<long, ProduktUkryty> produktyUkryte = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktUkryty>(null,
                x => (Customer.Role.Contains(RoleType.Administrator) || x.PrzedstawicielId == Customer.Id) ).ToDictionary(x => x.Id, x => x);

            if (idKlientow != null && idKlientow.Count > 0 && idKlientow.Count!=produktyUkryte.Count)
                produktyUkryte = produktyUkryte.Where(a => a.Value.KlientZrodloId.HasValue && idKlientow.Any(b => b == a.Value.KlientZrodloId.Value)).ToDictionary(x => x.Key, x => x.Value);

            return produktyUkryte;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<long>); }
        }

      
    }

    /// <summary>
    /// Pobiera listę ID produktów ukrytych jako ListaID<int>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzIDProduktyUkryte : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible// ApiSessionBaseHandlerPobierzDane
    {
        protected override object Handle()
        {
            List<long> produktyUkryte = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktUkryty>(null, 
                x => Customer.Role.Contains(RoleType.Administrator) || x.PrzedstawicielId == Customer.Id).Select(x => x.Id).ToList();

            return produktyUkryte;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<long>); }
        }
    }

    /// <summary>
    /// Usuwa produkty ukryte wysłane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunProduktyUkryte : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long> ids = (HashSet<long>)Data;
            if (ids.Count > 0)
            {
                //ProduktyUkryteSearchCriteria criteria = new ProduktyUkryteSearchCriteria();
                //criteria.id.AddRange(ids);
                //Core.ProduktyUkryteBll.PobierzInstancje.Usun(criteria);
                SolexBllCalosc.PobierzInstancje.DostepDane.Usun<ProduktUkryty, long>(ids.ToList());

            }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
}
