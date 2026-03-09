using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class BlogKategoria : IHasLongId, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        [WidoczneListaAdmin(true, true, false, false)]
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get;  set; }
        [WidoczneListaAdmin(true, true, true, true)]
        public string Nazwa { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        public bool Aktywna { get; set; }
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikGrupyBlogow,SolEx.Hurt.Core")]
        [Niewymagane]
        [WidoczneListaAdmin]
        public int? BlogGrupaId { get; set; }

        [FriendlyName("Zdjęcie")]
        [Niewymagane]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        public int? ZdjecieId { get; set; }
        public bool RecznieDodany()
        {
            return true;
        }
    }
}
