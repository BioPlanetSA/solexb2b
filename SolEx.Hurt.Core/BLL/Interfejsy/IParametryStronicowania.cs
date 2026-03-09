using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IParametryStronicowania
    {
        int AktualnaStrona(string prefix);

        int RozmiarStrony(string prefix);

        IEnumerable<int> DostepneRozmiaryStron { get; }

        string WygenrujLinkDoStrony(int nrStrony, string prefix, string docelowa = "");

        string WygenrujLinkDoStrony(int nrStrony, int rozmiar, string prefix, string docelowa = "");

        object WygenerujQsDoStrony(int nrStrony, string prefix);

        string WygenerujQsDoStrony(int nrStrony, int rozmiar, string prefix);
    }
}