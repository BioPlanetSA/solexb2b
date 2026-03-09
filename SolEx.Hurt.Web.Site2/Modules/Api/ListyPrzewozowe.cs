using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje listy przewozowe wysłane w obiekcie Data jako Lista historia_dokumenty_listy_przewozowe>
    /// </summary>
    public class AktualizujListyPrzewozowe : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            var doZmiany = (List<HistoriaDokumentuListPrzewozowy>)Data;
            if (doZmiany.Any())
            {
                int packageNr = 0;
                const int packegeSize = 1000;
                List<HistoriaDokumentuListPrzewozowy> paczka;
                do
                {
                    paczka = doZmiany.Skip(packageNr * packegeSize).Take(packegeSize).ToList();
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<HistoriaDokumentuListPrzewozowy>(paczka);

                    packageNr++;
                }
                while (paczka.Count == packegeSize);
            }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp => typeof(List<HistoriaDokumentuListPrzewozowy>);
    }

    /// <summary>
    /// Pobiera listy przewozowe jako Słownik klucz int ID, wartość historia_dokumenty_listy_przewozowe>
    /// </summary>
    public class PobierzListyPrzewozowe : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible//ApiSessionBaseHandlerPobierzDane
    {
       
        protected override object Handle()
        {
            Dictionary<int, HistoriaDokumentuListPrzewozowy> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Model.HistoriaDokumentuListPrzewozowy>(null).ToDictionary(x => x.Id, x => x);
            return levels;
        }
    }

    /// <summary>
    /// Usuwa listy przewozowe wysłane w obiekcie Data jako ListaID int>
    /// </summary>
    class UsunListyPrzewozowe : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<int> ids = (HashSet<int>)Data;

            if (ids.Any())
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.Usun<HistoriaDokumentuListPrzewozowy, int>(ids.ToList());
            }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp => typeof(HashSet<int>);
    }
}
