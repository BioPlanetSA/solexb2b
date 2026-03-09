namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class SmallBussines : RcHurt
    {
        public override string LadnaNazwa
        {
            get { return "Import w formacie SmallBussines (plik dbf)"; }
        }
        protected override string KolumnaKodKReskowy
        {
            get { return "SYMBOL_TOW"; }
        }
    }
}
