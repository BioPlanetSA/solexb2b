using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikWidokowDlaWybranychProduktow : SlownikWidokowBaza

    {
        protected override string SciezkaWidokow
        {
            get
            {
                return "\\Produkty\\Widoki";
            }
        }

        protected override Dictionary<string,string> DodatkoweWidoki
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"Lista produktów z tabami", "ZamiennikiTaby"},
                    {"Produkty rodzinowe", "Warianty"}
                };
                
            }
        }
    }
}