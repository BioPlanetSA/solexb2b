using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoStalychFiltrow
    {
        private List<AtrybutBll> _atrybuty;
        private Dictionary<int, HashSet<long>> _wybraneCechy;
        private readonly bool _stalefiltry;

        public ParametryDoStalychFiltrow(List<AtrybutBll> atrybuty,Dictionary<int,HashSet<long>> wybraneCechy,bool stalefiltry )
        {
            _atrybuty = atrybuty;
            _wybraneCechy = wybraneCechy;
            _stalefiltry = stalefiltry;
        }

        public List<AtrybutBll> Atrybuty
        {
            get { return _atrybuty.OrderBy(p => p.Kolejnosc).ThenBy(p => p.Nazwa).ToList() ; }
        }

        public Dictionary<int, HashSet<long>> WybraneCechy
        {
            get { return _wybraneCechy; }
        }

        public bool StaleFiltry
        {
            get { return _stalefiltry; }
        }
    }
}