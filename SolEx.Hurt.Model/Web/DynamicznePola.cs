using System.Web;

namespace SolEx.Hurt.Model.Web
{
    public class DynamicznePola
    {
        public string Nazwa { get; set; }
        public string Typ { get; set; }
        public string Wartosc { get; set; }
        public HttpPostedFileBase  Plik { get; set; }
        public string SciezkaZalacznika { get; set; }
    } 
}


