using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    /// <summary>
    /// słownik który wyświetla Atrybuty o takich samych nazwach co grupy w kategoriach klienta 
    /// </summary>
    public class SlownikKoncesji:SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                IList<AtrybutBll> listaAtrybutow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<AtrybutBll>(null);
                IList<KategoriaKlienta> listaKategoriiKlienta = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(null);

                HashSet<string> listaNazwAtrybutow = new HashSet<string>( listaAtrybutow.Select(x=>x.Nazwa) );
                HashSet<string> listaGrup = new HashSet<string>( listaKategoriiKlienta.Select(x=>x.Grupa) );

                IEnumerable<string> czescWspolna = listaNazwAtrybutow.Intersect(listaGrup, StringComparer.OrdinalIgnoreCase);
                IList<CechyBll> listaCech = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<CechyBll>(null).Where(x=>czescWspolna.Contains(x.NazwaAtrybutu)).ToList();

                HashSet<string> nazwyKat = new HashSet<string>( listaKategoriiKlienta.Select(x => x.Nazwa) );
                HashSet<string> nazwyCechy = new HashSet<string>( listaCech.Select(x => x.Nazwa) );
                IEnumerable<string> wspolne = nazwyCechy.Intersect(nazwyKat, StringComparer.OrdinalIgnoreCase);

                var wspolneCechy = listaCech.Where(x => wspolne.Contains(x.Nazwa, StringComparer.OrdinalIgnoreCase));
                //return wynik;

                foreach (CechyBll cecha in wspolneCechy)
                {
                    wynik.Add(string.Format("{0} [{1}]", cecha.Nazwa, cecha.Id), cecha.Id);
                }
                return wynik;
            }
        }
    }
}
