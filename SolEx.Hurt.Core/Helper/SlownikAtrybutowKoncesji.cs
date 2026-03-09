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
    public class SlownikAtrybutowKoncesji:SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                IList<AtrybutBll> listaAtrybutow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<AtrybutBll>(null);
                HashSet<string> listaNazwAtrybutow = new HashSet<string>( listaAtrybutow.Select(x=>x.Nazwa) );
                HashSet<string> listaKategoriiKlienta = new HashSet<string>( SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(null).Select(x=>x.Grupa) );
                IEnumerable<string> czescWspolna = listaNazwAtrybutow.Intersect(listaKategoriiKlienta,StringComparer.OrdinalIgnoreCase);
                var atrKoncesji = listaAtrybutow.Where(x => czescWspolna.Contains(x.Nazwa));
                foreach (AtrybutBll atrybut in atrKoncesji)
                {
                    wynik.Add(string.Format("{0} [{1}]", atrybut.Nazwa, atrybut.Id),atrybut.Id)
                ;
                }
                return wynik;
            }
        }
    }
}
