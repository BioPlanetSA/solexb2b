using SolEx.Hurt.Model.Web;
using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IObslugaDanychDrzewka
    {
        List<ElementDoDrzewka> PobierzDane(IDictionary<string, string> parametry);
        bool Aktualizuj(List<AktualizacjaDrzewka> data);

        void Usun(AktualizacjaDrzewka data);

        void Aktywnosc(AktualizacjaDrzewka data);
    }
}
