using SolEx.Hurt.Core.BLL.Interfejsy;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public class SprawdzenieDomyslnychCen : TestKonfiguracjiBaza
    {
        public IConfigBLL Konfiguracja = SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public override string Opis
        {
            get { return "Sprawdzenie czy cena detaliczna i hurtowa jest inna ni¿ domyœlna"; }
        }

        /// <summary>
        /// Sprawdzenie czy wartosc domyslnej ceny detalicznej oraz hurtowej zostala zmieniona
        /// </summary>
        public override List<string> Test()
        {
            int cenaDomyslna = 0;
            List<string> listaBledow = new List<string>();

            if (Konfiguracja.GetPriceLevelDetal == cenaDomyslna)
            {
                listaBledow.Add(string.Format("Cena detaliczna nie zosta³a ustawiona. Ustawienie: poziom_ceny_detalicznej"));
            }
            if (Konfiguracja.GetPriceLevelHurt == cenaDomyslna)
            {
                listaBledow.Add(string.Format("Cena hurtowa nie zosta³a ustawiona. Ustawienie: poziom_ceny_hurtowej "));
            }
            return listaBledow;
        }
    }
}