using System.Collections.Generic;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    class BioPlanetXls : FriscoXls
    {
        public override List<string> Rozszerzenia
        {
            get { return new List<string>{"xls", "xlsx"};}
        }

        public override string LadnaNazwa
        {
            get { return "Import zamówień Bio Planet XLS"; }
        }
        protected override string NazwaKolumnyIlosc
        {
            get
            {
                return "Zam. (szt. / kg)";
            }
        }
        protected override string NazwaKolumnyKodKreskowy
        {
            get
            {
                return "EAN";
            }
        }
    }
}
