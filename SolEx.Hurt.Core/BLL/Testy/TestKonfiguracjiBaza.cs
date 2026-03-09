using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Testy
{
    public abstract class TestKonfiguracjiBaza
    {
        public abstract string Opis { get; }

        public abstract List<string> Test();
    }
}