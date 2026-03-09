using SolEx.Hurt.Core.ModelBLL;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class CzyJestJednaRejestracja : TestKonfiguracjiBaza
    {
        public override string Opis
        {
            get { return "Test sprawdzaj¹cy czy jest przynajmniej jedna rejestracja"; }
        }

        /// <summary>
        /// Sprawdzenie czy jest jedna rejestracja
        /// </summary>
        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            List<Rejestracja> listaRejestracji = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Rejestracja>(null).ToList();
            if (listaRejestracji.IsEmpty())
            {
                listaBledow.Add("Nie ma ¿adnej rejestracji");
            }
            return listaBledow;
        }
    }
}