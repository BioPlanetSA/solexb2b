using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class BlogGrupa : IHasIntId, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        [WidoczneListaAdmin(true,true,false,false)]
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        [WidoczneListaAdmin]
          public string Nazwa { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }
    }
}
