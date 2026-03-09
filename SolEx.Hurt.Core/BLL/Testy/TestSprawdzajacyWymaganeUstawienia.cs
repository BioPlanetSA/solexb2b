using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class TestSprawdzajacyWymaganeUstawienia : TestKonfiguracjiBaza
    {
        public override string Opis
        {
            get { return "Test czy wszystkie wymagane ustawienia zostały wypełnione."; }
        }

        public override List<string> Test()
        {
            List<string> listaBledow = new List<string>();
            foreach (var propertyInfo in typeof(ConfigBLL).GetProperties())
            {
                var y = propertyInfo.GetCustomAttributes(true).FirstOrDefault(p => p.GetType() == typeof(Wymagane)) as Wymagane;

                if (y != null)
                {
                    if (y.WymaganeDlaProvidera(SolexBllCalosc.PobierzInstancje.Konfiguracja.ProviderERP))
                    {
                        var wartosc = propertyInfo.GetValue(SolexBllCalosc.PobierzInstancje.Konfiguracja);
                        if (wartosc == null)
                        {
                            listaBledow.Add(string.Format("Brak uzupełnionej wartości: {0}", propertyInfo.Name));
                        }
                    }
                }
            }
            return listaBledow;
        }
    }
}