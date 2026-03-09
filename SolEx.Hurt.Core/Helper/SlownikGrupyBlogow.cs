using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikGrupyBlogow:SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var atrybut in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<BlogGrupa>(null))
                {
                    wynik.Add(string.Format("{0} [{1}]", atrybut.Nazwa, atrybut.Id), atrybut.Id);
                }
                return wynik;
            }
        }
    }
}