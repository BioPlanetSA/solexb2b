using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Helpers
{
    public class ApiConfigProvider : IConfigDataProvider
    {
        public IAPIWywolania ApiWywolanie = APIWywolania.PobierzInstancje;
        public List<Ustawienie> PobierzWszystkieUstawienia()
        {
            return ApiWywolanie.GetSettings();
        }

        public Dictionary<int, Jezyk> PobierzJezyki()
        {
            return ApiWywolanie.GetLanguages();
        }

        public List<Tlumaczenie> PobierzSystemPolaLokalizowane(string typ, int jezyk)
        {
            return ApiWywolanie.GetSlowniki().Values.Where(x => x.Typ == typ && x.JezykId == jezyk).ToList();
        }

        public void DodajSystemPole(TlumaczeniePole sp)
        {
            ApiWywolanie.AddSystemNames(new List<TlumaczeniePole> {sp});
        }

        public void DodajSlownik(Tlumaczenie s)
        {
            ApiWywolanie.DodajTlumaczenia(new List<Tlumaczenie> {s});
        }

        public List<TlumaczeniePole> GetSystemNames()
        {
            return ApiWywolanie.GetSystemNames();
        }
        //public List<TypWSystemie> PobierzSystemTypy()
        //{
        //    return ApiWywolanie.GetSystemTypes();
        //}

        //public void DodajSystemTyp(TypWSystemie st)
        //{
        //   ApiWywolanie.DodajSystemTyp(new List<TypWSystemie>{st});
        //}

        public List<StatusZamowienia> PobierzStatusyZamowien()
        {
            return ApiWywolanie.PobierzStatusyZamowien();
        }

        public void AktualizujUstawienie(Ustawienie u)
        {
          ApiWywolanie.UpdateSetting(new List<Ustawienie>{u});
        }

        public void UsunUstawienie(Ustawienie u)
        {
            throw new NotImplementedException();
        }

        public Ustawienie PobierzUstawienie(string symbol)
        {
            return ApiWywolanie.GetSetting(symbol);
        }

        public void AktualizujUstawienie(List<Ustawienie> u)
        {
            foreach (var s in u)
            {
                AktualizujUstawienie(s);
            }
        }

        public void DodajJezyk(Jezyk tmp)
        {
            throw new NotImplementedException();
        }
    }
}
