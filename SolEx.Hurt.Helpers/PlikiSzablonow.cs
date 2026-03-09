using System;

namespace SolEx.Hurt.Helpers
{
    public static class PlikiSzablonow
    {
        public static string WczytajToken(string text, string token, string domyslne = "")
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(token)) return domyslne;

            string symbolkonca ="\r\n";
            if (!text.Contains(symbolkonca))
            {
                symbolkonca = "\n";
            }
            string tokenPoczatek = "#poczatek " + token + symbolkonca;
            string tokenKoniec = "#koniec " + token + symbolkonca;

            int pocza = text.IndexOf(tokenPoczatek, StringComparison.InvariantCultureIgnoreCase);
            if (pocza >= 0)
            {
                pocza += tokenPoczatek.Length;
                int koniec = text.IndexOf(tokenKoniec, pocza, StringComparison.InvariantCultureIgnoreCase);
                if (koniec == -1)
                {
                    tokenKoniec = "#koniec " + token;
                    koniec = text.IndexOf(tokenKoniec, pocza, StringComparison.InvariantCultureIgnoreCase);
                }
                if (koniec >= 0)
                {
                    string wynik = text.Substring(pocza, koniec - pocza).Trim();
                    return wynik;
                }

            }
            return domyslne;
        }
    }
}
