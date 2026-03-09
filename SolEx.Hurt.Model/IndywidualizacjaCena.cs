using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model
{
    public class IndywidualizacjaCena:IHasLongId
    {
        public IndywidualizacjaCena(){}
        public IndywidualizacjaCena(long idWaluty)
        {
            Cena = 0m;
            NarzutTyp= NarzutTyp.Brak;
            WalutaId = idWaluty;
        }
        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }

        [FriendlyName("Wartość ceny - wymagane gdy wybrano DodajStalaKwote jako sposób liczenia ceny")]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false)]
        public decimal? Cena { get; set; }

        [FriendlyName("Waluta ceny")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikWalut,SolEx.Hurt.Core")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public long WalutaId { get; set; }

        [FriendlyName("Typ narzutu")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public NarzutTyp NarzutTyp { get; set; }

        public long IdIndywidualizacji { get; set; }
    }
}
