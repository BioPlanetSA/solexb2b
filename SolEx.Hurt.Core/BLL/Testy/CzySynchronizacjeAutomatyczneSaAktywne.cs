using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class CzySynchronizacjeAutomatyczneSaAktywne : TestKonfiguracjiBaza
    {
        public IZadaniaBLL Zadania = SolexBllCalosc.PobierzInstancje.ZadaniaBLL;

        public override string Opis
        {
            get { return "Test sprawdzaj¹cy czy s¹ jakieœ aktywne synchronizacje w harmonogramie zadañ"; }
        }

        /// <summary>
        /// Test sprawdzaj¹cy czy minimum jedna automatyczna aktualizacja jest aktywna
        /// </summary>
        public override List<string> Test()
        {
            //throw new NotImplementedException();
            List<string> listaBledow = new List<string>();
            var zadania = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZadanieBll>(null, x => x.TypZadaniaSynchronizacji.HasValue || x.PobierzGrupyDoJakichPasujeZadanie().Contains(TypZadania.Synchronizacja))
                .Where(a => a.NumerElementuSynchronizacji != null).ToList();

            if (!zadania.Any())
            {
                listaBledow.Add("Nie ma aktywnych synchronizacji w harmonogramie zadañ");
            }
            return listaBledow;
        }
    }
}