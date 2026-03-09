using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL
{
    public interface IWidocznosciTypowBLL
    {
        /// <summary>
        /// Sprawdza czy klient ma dostêp do obiektu
        /// </summary>
        /// <param name="klient">Klient dla którego sprawdzamy dostêp</param>
        /// <param name="obiekt">Testowany obiekt</param>
        /// <returns></returns>
        bool KlientMaDostepDoObiektu(IKlient klient, IObiektWidocznyDlaOkreslonychGrupKlientow obiekt);

        /// <summary>
        /// Pobiera listê zdefiniowanych szablonów widocznoœci
        /// </summary>
        /// <returns></returns>
        IList<WidocznosciTypow> PobierzSzablony();

        /// <summary>
        /// Zwraca kolekcjê klientów, którzy spe³niaj¹ warunki widocznoœci
        /// </summary>
        /// <param name="id">Id warunków który sprawdzamy</param>
        /// <returns></returns>
        IList<IKlient> PobierzKlientowSprelniajacychWarunkiSzablonu(WidocznosciTypow widocznosc);
    }
}