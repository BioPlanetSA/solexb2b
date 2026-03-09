using SolEx.Hurt.Core.BLL.Interfejsy;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class SprawdzenieIkoscAktywnychKlientow : TestKonfiguracjiBaza
    {
        public IKlienciDostep Klienci = SolexBllCalosc.PobierzInstancje.Klienci;

        public override string Opis
        {
            get { return "Test sprawdzaj¹cy czy jest wiêcej ni¿ 10 aktywnych klientów"; }
        }

        /// <summary>
        /// Sprawdzenie czy ilosc aktynych klientow jest wieksza niz 10 z rola klient
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            int i = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null).Count(x => x.Aktywny && x.Role.Count == 1);

            if (i < 10)
            {
                listaBledow.Add(string.Format("Nie ma wiêcej ni¿ 10 aktywnych klientów, ich iloœæ wynosi {0}", i));
            }
            return listaBledow;
        }
    }
}