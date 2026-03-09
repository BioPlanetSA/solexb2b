namespace SolEx.Hurt.Sync.App.Modules_.Helpers
{
    public class KategorieKlientowHelper
    {
        public static bool SprawdzCzyPoprawnySeparator(string cecha, char[] separator)
        {
            int iloscWystapien = 0;
            foreach (var a in separator)
            {
                int index = cecha.IndexOf(a);
                if (index >= 0)
                {
                    iloscWystapien++;
                }
                if (iloscWystapien > 1)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
