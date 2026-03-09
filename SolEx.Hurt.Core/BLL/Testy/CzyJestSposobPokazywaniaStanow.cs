using ServiceStack.Text;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class CzyJestSposobPokazywaniaStanow : TestKonfiguracjiBaza
    {
        public SposobyPokazywaniaStanowBLL SposobyPokazwaniaStanow =
            SolexBllCalosc.PobierzInstancje.SposobyPokazywaniaStanowBll;

        public override string Opis
        {
            get { return "Test sprawdzajacy czy jest przynajmniej jeden sposób pokazywania stanów oraz posiada regu³ê"; }
        }

        /// <summary>
        /// Sprawdzenie czy jest przynajmniej jeden sposob pokazywania stanow
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            var slownikSposobow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SposobPokazywaniaStanow>(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski, null).Where(a => !ListExtensions.IsNullOrEmpty<SposobPokazywaniaStanowRegula>(a.Reguly)).ToList();
            if (!slownikSposobow.Any())
            {
                listaBledow.Add("Brak sposobów wyœwietlania stanów posiadaj¹cych regu³y");
            }
            return listaBledow;
        }
    }
}