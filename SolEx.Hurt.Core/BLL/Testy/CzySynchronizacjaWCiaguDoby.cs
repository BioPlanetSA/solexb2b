using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class CzySynchronizacjaWCiaguDoby : TestKonfiguracjiBaza
    {
        public IZadaniaBLL Zadania = SolexBllCalosc.PobierzInstancje.ZadaniaBLL;

        public override string Opis
        {
            get { return "Test sprawdzaj¹cy czy ostatnia synchronizacja odby³a sie w ci¹gu 24h"; }
        }

        /// <summary>
        /// Sprawdzenie czy wykona³a siê automatyczna synchronizacja przez ostatnie 24h
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();

            var zadania = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZadanieBll>(null, x => x.TypZadaniaSynchronizacji.HasValue || x.PobierzGrupyDoJakichPasujeZadanie().Contains(TypZadania.Synchronizacja));
            bool wynik = zadania.Any(a => a.NumerElementuSynchronizacji != null && a.OstatnieUruchomienieKoniec != null && a.OstatnieUruchomienieKoniec.Value.AddHours(24) > DateTime.Now);

            if (!wynik)
            {
                listaBledow.Add("Nie by³o sychronizacji w ostatnich 24h");
            }
            return listaBledow;
        }
    }
}