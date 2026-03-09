using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.Sync
{
    public class AtrybutyWyszukiwanie : BLL.BllBaza<AtrybutyWyszukiwanie>
    {
        public Atrybut WyciagnijAtrybutZCechy(ref string cechaNazwa, ref string symbol, char[] separator, string cechaAuto)
        {
            foreach (char c in separator)
            {
                int a = cechaNazwa.IndexOf(c);
                if (a > 0)
                {
                    string atrybut_Nazwa = cechaNazwa.ToLower().Substring(0, a).Trim();
                    int atrybut_ID = atrybut_Nazwa.WygenerujIDObiektu();
                    Atrybut tmp = new Atrybut(atrybut_Nazwa, atrybut_ID);
                    cechaNazwa = cechaNazwa.Substring(a + 1).Trim();
                    return tmp;
                }
            }
            int id =cechaAuto.WygenerujIDObiektu();
            Atrybut tmpa = new Atrybut(cechaAuto, id) { Widoczny = false };
            symbol = cechaAuto + separator[0] + symbol;
            return tmpa;
        }
    }
}
