using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje kategorie produktów wysłane w obiekcie Data Lista<kategorie>
    /// </summary>
    public class AktualizujKategorieZrodlowe : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            var dane = ((List<KategoriaProduktu>) Data);
            SolexBllCalosc.PobierzInstancje.KategorieDostep.AktualizujKategorie(dane);
            return null;
        }
        public override Type PrzyjmowanyTyp => typeof(List<KategoriaProduktu>);
    }

    /// <summary>
    /// Pobiera kategorie produktów jako Słownik<klucz int ID, wartość kategorie>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Przedstawiciel, RoleType.Administrator)]
    public class PobierzKategorieZrodlowe : ApiSessionBaseHandlerPobierzDane, Model.Core.IDocumentApiVisible
    {
        protected override Type SearchCriteriaType
        {
            get { return typeof(KategorieSearchCriteria); }
        }

        protected override object Handle()
        {
            //IList<KategorieBLL> levels;
            //KategorieSearchCriteria cirt = (KategorieSearchCriteria) SearchCriteriaObject;
            //if (cirt == null)
            //{
            //    levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategorieBLL>(null);
            //}
            //else
            //{
            //    var ids = SolexBllCalosc.PobierzInstancje.KategorieDostep.ZnajdzId(cirt);
            //    levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategorieBLL>(null,x=>ids.Contains(x.Id));
            //}
            //return levels.ToDictionary(x => x.Id, x => new kategorie(x));;

            HashSet<long> ids = (HashSet<long>)Data;
            var pb = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategorieBLL>(null, x => ids == null || ids.Contains(x.Id));
            return pb.ToDictionary(x => x.Id, x => new KategoriaProduktu(x));
        }
    }

    /// <summary>
    /// Usuwa kategorie produktów wg ID wysłanego w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunKategorieZrodlowe : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long> ids = (HashSet<long>) Data;

            SolexBllCalosc.PobierzInstancje.KategorieDostep.Usun(ids);

            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
}
