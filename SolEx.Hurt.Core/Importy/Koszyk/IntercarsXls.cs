using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    class IntercarsXls : FriscoXls
    {
        public override string LadnaNazwa
        {
            get { return "Import oferty Inter cars XLS"; }
        }
        protected override string NazwaKolumnyIlosc
        {
            get
            {
                return "ILE";
            }
        }
        protected override string NazwaKolumnyKodKreskowy
        {
            get
            {
                return "INDEKS";
            }
        }
    }
}
