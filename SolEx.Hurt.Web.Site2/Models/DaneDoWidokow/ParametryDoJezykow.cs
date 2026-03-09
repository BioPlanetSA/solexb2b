using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoJezykow
    {
        public ParametryDoJezykow( bool rozwijana, IList<Jezyk> jezyki, string url)
        {
            ListaRozwijana = rozwijana;
            ListaJezykow = jezyki;
            Url = url;
        }

        public IList<Jezyk> ListaJezykow { get; set; }
        public bool ListaRozwijana { get; set; }
        public string Url { get; set; }
    }
}