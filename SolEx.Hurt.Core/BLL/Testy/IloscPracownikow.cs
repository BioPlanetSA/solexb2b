using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class IloscPracownikow : TestKonfiguracjiBaza
    {
        public IKlienciDostep Klienci = SolexBllCalosc.PobierzInstancje.Klienci;

        public override string Opis
        {
            get { return "Test sprawdzajacy czy jest minimum 2 pracowników"; }
        }

        /// <summary>
        /// Sprawdzenie czy jest minimum 2 pracowników
        /// </summary>
        /// <returns></returns>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            int i = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null).Count(x => x.Role.Contains(RoleType.Pracownik));

            if (i < 3)
            {
                listaBledow.Add(string.Format("Nie ma wiêcej ni¿ 2 pracowników, ich iloœæ wynosi: {0}", i - 1));
            }
            return listaBledow;
        }
    }
}