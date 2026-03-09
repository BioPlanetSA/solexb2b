using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class IloscAktynycgModulowKoszyka : TestKonfiguracjiBaza
    {
        public IZadaniaBLL Zadania = SolexBllCalosc.PobierzInstancje.ZadaniaBLL;
        public IConfigBLL Konfiguracja = SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public override string Opis
        {
            get { return "Test sprawdzaj¹cy czy jest wiêcej ni¿ 5 aktywnych modu³ów koszyka"; }
        }

        /// <summary>
        /// Sprawdzenie czy s¹ przynajmniej 5 aktywne modu³y koszyka
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            var zadaniaKoszyka = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZadanieBll>(null, x => x.PobierzGrupyDoJakichPasujeZadanie().Contains(TypZadania.RegulaKoszyka)).ToList();
            if (zadaniaKoszyka.Count() < 5)
            {
                listaBledow.Add(string.Format("Nie ma 5 aktywnych modu³ów koszyka, ich iloœæ wynosi:{0}", zadaniaKoszyka.Count));
            }
            return listaBledow;
        }
    }
}