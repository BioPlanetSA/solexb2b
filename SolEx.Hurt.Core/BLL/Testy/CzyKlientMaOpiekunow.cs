using SolEx.Hurt.Core.BLL.Interfejsy;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class CzyKlientMaOpiekunow : TestKonfiguracjiBaza
    {
        public IKlienciDostep Klienci = SolexBllCalosc.PobierzInstancje.Klienci;

        public override string Opis
        {
            get { return "Test czy przynajmniej jeden klient ma opiekuna"; }
        }

        /// <summary>
        /// Sprawdzenie czy klient ma opiekuna
        /// </summary>
        /// <returns></returns>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            int i = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null).Count(x => x.OpiekunId != null || x.PrzedstawicielId != null || ((Model.Klient)x).DrugiOpiekunId != null);

            if (i < 1)
            {
                listaBledow.Add("Brak klientów maj¹cych opiekunów");
            }
            return listaBledow;
        }
    }
}