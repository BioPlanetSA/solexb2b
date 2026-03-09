using System;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.OrmLite;

namespace SolEx.Hurt.Core.BLL
{
    public class CechyProduktyDostep : LogikaBiznesBaza, ICechyProduktyDostep
    {
        public CechyProduktyDostep(ISolexBllCalosc solexBllCalosc)
            : base(solexBllCalosc)
        {
        }

        public void CzyscCache(IList<object> kluczeDoKasowania = null )
        {
            _wszystkieLacznikiWgCech = null;
            _wszystkieLacznikiWgProduktow = null;
        }

        private Dictionary<long, HashSet<long>> _wszystkieLacznikiWgCech = null;
        private Dictionary<long, HashSet<long>> _wszystkieLacznikiWgProduktow = null;

        public Dictionary<long, HashSet<long>> WszystkieLacznikiWgCech
        {
            get
            {
                if (_wszystkieLacznikiWgCech == null)
                {
                    _wszystkieLacznikiWgCech = Calosc.DostepDane.Pobierz<ProduktCecha>(null).GroupBy(x => x.CechaId).ToDictionary(x => x.Key, x => new HashSet<long>(x.Select(y => y.ProduktId)));
                }
                return _wszystkieLacznikiWgCech;
            }
        }

        public Dictionary<long, HashSet<long>> WszystkieLacznikiWgProduktow
        {
            get
            {
                if (_wszystkieLacznikiWgProduktow == null)
                {
                    _wszystkieLacznikiWgProduktow = Calosc.DostepDane.Pobierz<ProduktCecha>(null).GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => new HashSet<long>(x.Select(y => y.CechaId)));

                    //todo: wszystkie id produktow - select * from Produkt p where p.Widoczny = 1 AND p.Id not in (select ProduktID from ProduktCecha) 


                }
                return _wszystkieLacznikiWgProduktow;
            }
        }

        public HashSet<long> PobierzProduktyZCechami(HashSet<long> cechyId)
        {
            HashSet<long> listaProduktow = new HashSet<long>();
            foreach (HashSet<long> set in WszystkieLacznikiWgCech.WhereKeyIsIn(cechyId))
            {
                foreach (int p in set)
                {
                    if (!listaProduktow.Contains(p))
                    {
                        listaProduktow.Add(p);
                    }
                }
            }
            return listaProduktow;
        }

        public HashSet<long> PobierzIdCechProduktu(long produktId)
        {
            if (!WszystkieLacznikiWgProduktow.TryGetValue(produktId, out HashSet<long> idLacznikow))
            {
                idLacznikow = new HashSet<long>();
            }

            return idLacznikow;
        }
    }
}