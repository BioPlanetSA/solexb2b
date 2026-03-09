using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class SprawdzenieDubliAtrybutow : TestKonfiguracjiBaza
    {
        public IDaneDostep cechyAtrybuty = SolexBllCalosc.PobierzInstancje.DostepDane;
        public IConfigBLL config = SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public override string Opis
        {
            get { return "Sprawdzenie czy w systemie nie ma dwóch takich samych nazwa atrybutow"; }
        }

        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            List<string> tmp = new List<string>();
            IList<AtrybutBll> listaAtrybutow = cechyAtrybuty.Pobierz<AtrybutBll>(null);
            foreach (var atrybutyBll in listaAtrybutow)
            {
                if (tmp.Contains(atrybutyBll.Nazwa))
                {
                    listaBledow.Add(string.Format("Są przynajmniej dwa atrybuty o nazwie: {0}", atrybutyBll.Nazwa));
                }
                else
                {
                    tmp.Add(atrybutyBll.Nazwa);
                }
            }
            return listaBledow;
        }
    }
}