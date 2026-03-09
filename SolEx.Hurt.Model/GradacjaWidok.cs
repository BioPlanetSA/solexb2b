using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Model
{
    public class GradacjaWidok
    {
        public bool AktualnaCena { get; set; }
        public decimal? PrzedzialOd { get; set; }
        public decimal? PrzedzialDo { get; set; }
        public bool Spelniny { get; set; }
        public decimal PrzedzialOdRzeczywisty { get; set; }
        public decimal PrzedzialDoRzeczywisty { get; set; }
        public WartoscLiczbowa CenaNetto { get; set; }
        public WartoscLiczbowa CenaBrutto { get; set; }
        public WartoscLiczbowa WartoscVAT { get { return CenaBrutto - CenaNetto; } }

        public decimal IleBrakujeDoSpelnieniaPoziomu { get; set; }
    }
}