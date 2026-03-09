using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje kategorie klientów wysłanych w obiekcie Data jako Lista<kategorie_klientow>
    /// </summary>
    public class AktualizujKategorieKlientow : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<KategoriaKlienta>((List<KategoriaKlienta>)Data);
            return null;
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<KategoriaKlienta>); }
        }
    }

    /// <summary>
    /// Pobiera kategorie klientów jako Słownik<klucz int ID, wartość kategorie_klientow>
    /// </summary>
    public class PobierzKategorieKlientow : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            Dictionary<string, object> parametry = (Dictionary<string, object>)Data;
            bool wszystkie = false;

            if (parametry != null)
            {
                if (parametry.ContainsKey("wszystkie"))
                    wszystkie = (bool)parametry["wszystkie"];
            }

            Dictionary<int, KategoriaKlienta> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(null,x => (wszystkie || x.Id > 0)).ToDictionary(x => x.Id, x => x);

            return levels;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(Dictionary<string, object>); }
        }
    }

    /// <summary>
    /// Usuwa kategorie klientów których ID zostanie przekazane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunKategorieKlientow : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<KategoriaKlienta, object>((List<object>)Data);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<object>); }
        }
    }
}
