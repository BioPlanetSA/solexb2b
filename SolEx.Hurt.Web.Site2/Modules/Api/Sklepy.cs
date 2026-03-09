using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje sklpey
    /// </summary>
    public class AktualizujSklepy : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            IList<Sklep> doZmiany = (List<Sklep>)Data;
           
            SolexBllCalosc.PobierzInstancje.Sklepy.Zapisz(doZmiany,Customer);

            return null;
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Sklep>); }
        }
    }

    /// <summary>
    /// Pobiera sklpey
    /// </summary>
    public class PobierzSklepy : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            IDictionary<long, Sklep> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Sklep>(null).ToDictionary(x => x.Id, x => x);

            return levels;
        }
    }

    /// <summary>
    /// Usuwa sklepy o wybranych id
    /// </summary>
    public class UsunSklepy : ApiSessionBaseHandler
    {
        protected override object Handle()
        {

            HashSet<long> ids = (HashSet<long>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Sklep, long>(ids.ToList());
           return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
    /// <summary>
    /// Aktualizuje sklpey kategorie
    /// </summary>
    public class AktualizujSklepyKategorie : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<KategoriaSklepu> doZmiany = (List<KategoriaSklepu>)Data;


            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(doZmiany);

            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<KategoriaSklepu>); }
        }
    }

    /// <summary>
    /// Pobiera sklpey kategorie
    /// </summary>
    public class PobierzSklepyKategorie : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {


        protected override object Handle()
        {
            IDictionary<long, KategoriaSklepu> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaSklepu>(null).ToDictionary(x => x.Id, x => x);

            return levels;
        }
    }

    /// <summary>
    /// Usuwa sklepy kategorie o wybranych id
    /// </summary>
    public class UsunSklepyKategorie : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long> ids = (HashSet<long>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<KategoriaSklepu, long>(ids.ToList());
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
    /// <summary>
    /// Aktualizuje sklpey kategorie
    /// </summary>
    public class AktualizujSklepyKategorieLaczniki : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<SklepKategoriaSklepu> doZmiany = (List<SklepKategoriaSklepu>)Data;


            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<SklepKategoriaSklepu>(doZmiany);

            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<SklepKategoriaSklepu>); }
        }
    }

    /// <summary>
    /// Pobiera sklpey kategorie
    /// </summary>
    public class PobierzSklepyKategorieLaczniki : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible//ApiSessionBaseHandlerPobierzDane
    {
        //protected override Type SearchCriteriaType
        //{
        //    get { return typeof(SklepyKategoriePolaczeniaCriteria); }
        //}

        protected override object Handle()
        {
            IList<SklepKategoriaSklepu> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SklepKategoriaSklepu>(null);

            return levels;
        }
    }

    /// <summary>
    /// Usuwa sklepy kategorie o wybranych id
    /// </summary>
    public class UsunSklepyKategorieLaczniki : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long> ids = (HashSet<long>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<SklepKategoriaSklepu,long>(ids.ToList());
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
}
