using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Filtry
{
    public class DaneDoFiltrow
    {
   //     Tuple<AtrybutBll, bool, HashSet<int>, bool>(atrybut, false, wybrane,multi)
        private readonly AtrybutBll _atrybut;
        private readonly List<AtrybutBll> _wszystkie;
        private readonly Dictionary<int, HashSet<long>> _wybraneCechy;
        private readonly bool _stalyFiltr;
        private readonly bool _multiWybor;
        private readonly bool _wyswietlajZdjecie;
        private readonly bool _button;
        public DaneDoFiltrow(AtrybutBll atrybut,Dictionary<int,HashSet<long>> wybraneCechy, List<AtrybutBll> wszystkie,bool stalyFiltr,bool multiWybor, bool zdjecie=false, bool button = false)
        {
            _wybraneCechy = wybraneCechy;
            _wszystkie = wszystkie;
            _stalyFiltr = stalyFiltr;
            _atrybut = atrybut;
            _stalyFiltr = stalyFiltr;
            _multiWybor = multiWybor;
            _wyswietlajZdjecie = zdjecie;
            _button = button;
        }


        public AtrybutBll Atrybut
        {
            get { return _atrybut; }
        }

        public List<AtrybutBll> Wszystkie
        {
            get { return _wszystkie; }
        }

        public HashSet<long> WybraneCechyAtrybut
        {
            get
            {
                if (_wybraneCechy != null && _wybraneCechy.ContainsKey(_atrybut.Id))
                {
                    return _wybraneCechy[_atrybut.Id];
                }
                return null;
            }
        }

        public Dictionary<int, HashSet<long>> WszystkieWybraneCechy
        {
            get { return _wybraneCechy; }
        }

        public bool StalyFiltr
        {
            get { return _stalyFiltr; }
        }

        public bool MultiWybor
        {
            get { return _multiWybor; }
        }
        public bool WyswietlajZdjecie
        {
            get { return _wyswietlajZdjecie; }
        }
        public bool Button
        {
            get { return _button; }
        }
    }
}