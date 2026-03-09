using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Pobiera łaczniki adresów z klientami w formie Słownik<klucz string ID, wartość KlientAdres>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzLacznikiAdresow : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {


        protected override object Handle()
        {
            Dictionary<long, KlientAdres> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KlientAdres>(null).ToDictionary(x => x.Id, x => x);

            return levels;
        }
    }

    /// <summary>
    /// Aktualizuje adresy klientów podanych w obiekcie Data w formie Lista<Adresy>
    /// </summary>
    public class AktualizujLacznikiAdresow : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<KlientAdres> doZmiany = (List<KlientAdres>)Data;

            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<KlientAdres>(doZmiany);

            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<KlientAdres>); }
        }
    }


    /// <summary>
    /// Usuwa łączniki adresów podanych w obiekcie Data jako ListaID<string>
    /// </summary>
    public class UsunLacznikiAdresow : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long> ids = (HashSet<long>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<KlientAdres, long>(ids.ToList());
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
}