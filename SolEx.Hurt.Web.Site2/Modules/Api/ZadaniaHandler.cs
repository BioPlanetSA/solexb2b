using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System.Linq;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Pobiera zadania dla synchronizatora jako Lista<Zadania>
    /// </summary>
    public class ZadaniaSynchronizatoraPobierz : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZadanieBll>(null, x => (x.TypZadaniaSynchronizacji.HasValue || x.PobierzGrupyDoJakichPasujeZadanie().Contains(TypZadania.Synchronizacja)) && x.Aktywne);
        }
    }

    /// <summary>
    /// Aktualizuje listę zadań dla synchronizatora wysłanych w obiekcie Data jako Lista<Zadania>
    /// </summary>
    public class ZadaniaAktualizuj : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            var zadania = (List<Zadanie>) Data;
            var istniejace = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Zadanie>(null,y=>zadania.Select(x => x.Id).Contains(y.Id));
            foreach (var i in istniejace)
            {
               var akt= zadania.FirstOrDefault(x => x.Id == i.Id);
                if (akt != null)
                {
                    i.OstatnieUruchomienieStart = akt.OstatnieUruchomienieStart;
                    i.OstatnieUruchomienieKoniec = akt.OstatnieUruchomienieKoniec;
                }
            }

            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(istniejace);
            return null;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Zadanie>); }
        }
    }
}
