namespace SolEx.Hurt.Core.Importy.Koszyk
{
    class BestautomaxXls : FriscoXls
    {
        public override string LadnaNazwa
        {
            get { return "Import oferty best automax XLS"; }
        }
        protected override string NazwaKolumnyIlosc
        {
            get
            {
                return "Ilość";
            }
        }
        protected override string NazwaKolumnyKodKreskowy
        {
            get
            {
                return "Kod";
            }
        }
    }
}
