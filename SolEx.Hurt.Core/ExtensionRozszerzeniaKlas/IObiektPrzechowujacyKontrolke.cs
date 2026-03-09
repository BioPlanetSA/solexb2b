using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ExtensionRozszerzeniaKlas
{
    public static class IObiektPrzechowujacyKontrolkeExtension
    {
        public static void UstawParametry(this IObiektPrzechowujacyKontrolke pojemnik, Dictionary<string, object> parametry)
        {
            pojemnik.UstawParametrySerializowane(JSonHelper.Serialize(parametry));
        }
        public static void UstawParametry(this IObiektPrzechowujacyKontrolke pojemnik, IEnumerable<OpisPolaObiektu> wartosciPol)
        {
            pojemnik.UstawParametry(wartosciPol.PolaNaslownik());
        }

        /// <summary>
        /// Metoda stworząca slownik pól do uzupeniania obiektu
        /// </summary>
        /// <param name="wartosciPol"></param>
        /// <returns></returns>
        public static Dictionary<string, object> PolaNaslownik(this IEnumerable<OpisPolaObiektu> wartosciPol)
        {
            return wartosciPol.ToDictionary(x => x.NazwaPola, x => x.PobierzWartosc());
        }

        public static Dictionary<string, object> PolaNaslownik(this OpisPolaObiektu wartosciPol)
        {
            return new Dictionary<string, object>() { {wartosciPol.NazwaPola, wartosciPol.PobierzWartosc() } };
        }
    }
}
