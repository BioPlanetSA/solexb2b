using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Model
{
    public class BlogWpisBlogKategoria:IHasStringId
    {
       public string Id {
            get
            {
                return BlogWpisId + "_" + BlogKategoriaId;
            } 
        }
        public long BlogWpisId { get; set; }
        public long BlogKategoriaId { get; set; }

        public BlogWpisBlogKategoria(long wpisId, int katId)
        {
            BlogWpisId = wpisId;
            BlogKategoriaId = katId;
        }
        public BlogWpisBlogKategoria(){}

        public BlogWpisBlogKategoria(BlogWpisBlogKategoria bk)
        {
            BlogKategoriaId = bk.BlogKategoriaId;
            BlogWpisId = bk.BlogWpisId;
        }
    }
}
