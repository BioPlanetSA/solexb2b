using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKategoriiBloga : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                var grupy = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<BlogGrupa>(null);
                var wynik = new Dictionary<string, object>();
                foreach (var item in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<BlogKategoria>(null))
                {
                    string nazwa = string.Format("{0} [{1}]", item.Nazwa, item.Id);
                    if (item.BlogGrupaId.HasValue)
                    {
                        nazwa = grupy.First(x => x.Id == item.BlogGrupaId).Nazwa + ":"+nazwa;
                    }
                    wynik.Add(nazwa, item.Id);
                }
                return wynik;
            }
        }
    }
}
