using System.Collections.Generic;
using CsvHelper;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class ZabkaTxt:BioPlanetCsv
    {
        public override List<string> Rozszerzenia
        {
            get { return new List<string> { "txt" }; }
        }

        protected override void PrzetworzNaglowek(CsvReader csv, ref int kolumnaKod, ref int kolumnailosc)
        {
            kolumnailosc = 1;
            kolumnaKod = 0;
        }
        protected override bool JestNaglowek
        {
            get { return false; }
        }
        public override string LadnaNazwa
        {
            get { return "Import oferty zabka w formacie txt"; }
        }

    }
}
