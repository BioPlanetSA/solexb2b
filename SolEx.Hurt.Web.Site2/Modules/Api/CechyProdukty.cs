using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje łączniki cech produktów wysłane w obiekcie Data jako Lista<cechy_produkty>
    /// </summary>
    public class AktualizujCechyProdukty : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<ProduktCecha> doZmiany = (List<ProduktCecha>)Data;
            if (doZmiany.Count > 0)
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProduktCecha>(doZmiany);
            }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<ProduktCecha>); }
        }
    }

    /// <summary>
    /// Pobiera łączniki cech produktów jako Słownik<klucz int ID, wartość cechy_produkty>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzCechyProdukty : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        private ICechyProduktyDostep _cechyatrybutylaczniki = SolexBllCalosc.PobierzInstancje.CechyProduktyDostep;

        public ICechyProduktyDostep CechyAtrybutyLaczniki
        {
            get { return _cechyatrybutylaczniki; }
            set { _cechyatrybutylaczniki = value; }
        }

        protected override object Handle()
        {
            return PobierzLacznikiCechyProdukty();
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<string>); }
        }
        
        public Dictionary<long, ProduktCecha> PobierzLacznikiCechyProdukty()
        {
            var ids = (HashSet<string>) Data;
            IList<ProduktCecha> cp;
            if (ids == null || ids.IsEmpty())
            {
                cp = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktCecha>(null);
            }
            else
            {
                //poslugujemy sie zapytaniem SQL bo obiekt jest SQLowy bezposrednio
                cp = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktCecha>(null, x=> Sql.In(x.Id, ids) );
            }
            Dictionary<long, ProduktCecha> levels = cp.ToDictionary(x => x.Id, x => x);
            return levels;
        }
    }

    /// <summary>
    /// Pobiera hashset z identyfikatorami łączników cech produktów potrzebny do paczkowanego pobierania łączników
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzCechyProduktyID : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        private ICechyProduktyDostep _cechyatrybutylaczniki = SolexBllCalosc.PobierzInstancje.CechyProduktyDostep;

        public ICechyProduktyDostep CechyAtrybutyLaczniki
        {
            get { return _cechyatrybutylaczniki; }
            set { _cechyatrybutylaczniki = value; }
        }

        protected override object Handle()
        {
            return PobierzLacznikiCechyProduktyID();
        }
        public HashSet<long> PobierzLacznikiCechyProduktyID()
        {
            HashSet<long> ids = new HashSet<long>( SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktCecha>(null).Select(x=>x.Id) );
            return ids;
        }
    }

    /// <summary>
    /// Usuwa łączniki cech produktów wysłane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunCechyProdukty : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long> ids = (HashSet<long>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<ProduktCecha, long>(ids.ToList());
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
}
