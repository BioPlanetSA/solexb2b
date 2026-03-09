using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IConfigDataProvider
    {
        List<Ustawienie> PobierzWszystkieUstawienia();
        Dictionary<int, Jezyk> PobierzJezyki();
        List<Tlumaczenie> PobierzSystemPolaLokalizowane(string typ, int jezyk);
        void DodajSystemPole(TlumaczeniePole sp);
        void DodajSlownik(Tlumaczenie s);

        List<StatusZamowienia> PobierzStatusyZamowien();
        void AktualizujUstawienie(Ustawienie u);
        void UsunUstawienie(Ustawienie u);
        Ustawienie PobierzUstawienie(string symbol);
        void AktualizujUstawienie(List<Ustawienie> u);

        List<TlumaczeniePole> GetSystemNames();

        void DodajJezyk(Jezyk tmp);
    }
}
