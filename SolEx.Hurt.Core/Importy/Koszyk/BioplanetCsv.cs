namespace SolEx.Hurt.Core.Importy.Koszyk
{
   public class BioPlanetCsv : ImportCsv
    {
        public override string LadnaNazwa
        {
            get { return "Import oferty Bio Planet w formacie csv"; }
        }

       protected virtual string NazwaKolumnyIlosc
        {
            get { return "Zam. (szt. / kg)"; }
        }
        protected virtual string NazwaKolumnyKodKreskowy
        {
            get { return "EAN"; }
        }
    }
}
