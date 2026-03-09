namespace SolEx.Hurt.Core.Importy.Koszyk
{
    class AutodakolXls : FriscoXls
    {
        public override string LadnaNazwa
        {
            get { return "Import oferty AUTO DAKOL XLS"; }
        }
        protected override string NazwaKolumnyIlosc
        {
            get
            {
                return "ILOŚĆ";
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
