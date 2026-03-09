using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class TestUstawien : TestKonfiguracjiBaza
    {
        public override string Opis
        {
            get { return "Test sprawdzający poprawność ustawień"; }
        }

        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();

            List<string> listaFiltrow = PlikiDostep.PobierzInstancje.PobierzTypyFiltow();
            string domyslnyFiltr = SolexBllCalosc.PobierzInstancje.Konfiguracja.TypDomyslnyFiltru;

            if (!listaFiltrow.Contains(domyslnyFiltr))
            {
                listaBledow.Add(string.Format("Niepoprawny filtr: {0}", domyslnyFiltr));
            }

            return listaBledow;
        }
    }
}