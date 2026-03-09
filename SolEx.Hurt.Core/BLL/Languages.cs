using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL
{
    public class Languages
    {
        public static Dictionary<int, Jezyk> PobierzSlownik()
        {
            //var wszystkie = MainDAO.db.Select<jezyki>().OrderByDescending(x => x.domyslny).ToList();
            var wszystkie = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Jezyk>(null).OrderByDescending(x => x.Domyslny).ToList();
            return wszystkie.ToDictionary(x => x.Id, x => x);
        }

        protected static void Update(Jezyk data)
        {
            //MainDAO.db.Save(data);
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(data);
        }

        protected static void Delete(int id)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.UsunWybrane<Jezyk,int>(x => x.Id == id);
            //MainDAO.db.DeleteById<jezyki>(id);
        }

        private static string klucz_lista = "jezyki_system";

        public static Dictionary<int, Jezyk> JezykiWSystemie()
        {
            Dictionary<int, Jezyk> wynik = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<Dictionary<int, Jezyk>>(klucz_lista);
            if (wynik == null)
            {
                wynik = new Dictionary<int, Jezyk>(PobierzSlownik());
                //SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt(klucz_lista,wynik);
            }
            return wynik;
        }

        public static Dictionary<int, Jezyk> JezykiWSystemieWidoczneDlaKlienta()
        {
            return JezykiWSystemie().Where(x => !x.Value.UkrytyDlaKlienta).ToDictionary(x => x.Key, x => x.Value);
        }

        public static void Usun(int id)
        {
            Delete(id);
            //SolexBllCalosc.PobierzInstancje.Cache.UsunObiekt(klucz_lista);
            // SolexBllCalosc.PobierzInstancje.Konfiguracja.RefreshData();
        }

        public static void Aktualizuj(Jezyk item)
        {
            Update(item);
            // SolexBllCalosc.PobierzInstancje.Cache.UsunObiekt(klucz_lista);
            //  SolexBllCalosc.PobierzInstancje.Konfiguracja.RefreshData();
        }
    }
}