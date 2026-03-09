using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
namespace SolEx.Hurt.Model
{
    public class PoziomCenowy : IHasIntId
    {
        public PoziomCenowy() { }
        public PoziomCenowy(int id, string nazwa, long? walutaId)
        {
            this.Id = id;
            this.Nazwa = nazwa;
            this.WalutaId = walutaId;
        }

        [PrimaryKey]
        [UpdateColumnKey]
        [WidoczneListaAdmin(true, true, false, false)]
        public int Id { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        public string Nazwa { get; set; }

        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikWalut,SolEx.Hurt.Core")]
        [WidoczneListaAdmin(true, true, false, false)]
        public long? WalutaId { get; set; }
    }
}
