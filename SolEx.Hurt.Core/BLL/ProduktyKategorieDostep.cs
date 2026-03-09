using System;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.OrmLite;

namespace SolEx.Hurt.Core.BLL
{
    public class ProduktyKategorieDostep : LogikaBiznesBaza, IProduktyKategorieDostep
    {
        public ProduktyKategorieDostep(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        private IDictionary<long, HashSet<long>> _ProduktyKategorieGrupowanePoProdukcie = null;
        private IDictionary<long, HashSet<long>> _ProduktyKategorieGrupowanePoKategorii = null;

        private void pobierzDane()
        {
            IList<ProduktKategoria> dane = Calosc.DostepDane.Pobierz<ProduktKategoria>(null);

            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.WielowybieralnoscKategorii)
            {
              List<ProduktKategoria> dodatkoweDane =  SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.SqlList<ProduktKategoria>("SELECT *, Rodzaj=0 FROM vKategorieWieloWybieralne");

                foreach (var d in dodatkoweDane)
                {
                    dane.Add(d);
                }
            }

            //w sql jest juz sprawdzenie czy nie ma dubli - wiec nie powinno byc juz tu dubli - jesli sa to znaczy ze SQL jest zły i trzeba TAM poprawić
            _ProduktyKategorieGrupowanePoProdukcie = dane.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => new HashSet<long>( x.Select(y => y.KategoriaId)) );
            _ProduktyKategorieGrupowanePoKategorii = dane.GroupBy(x => x.KategoriaId).ToDictionary(x => x.Key, x => new HashSet<long>( x.Select(y => y.ProduktId) ) );
        }


        public IDictionary<long, HashSet<long>> ProduktyKategorieGrupowanePoProdukcie
        {
            get
            {
                if (_ProduktyKategorieGrupowanePoProdukcie == null)
                {
                    pobierzDane();
                }
                return _ProduktyKategorieGrupowanePoProdukcie;
            }
        }


        public IDictionary<long, HashSet<long>> ProduktyKategorieGrupowanePoKategorii
        {
            get
            {
                if (_ProduktyKategorieGrupowanePoKategorii == null)
                {
                    pobierzDane();
                }
                return _ProduktyKategorieGrupowanePoKategorii;
            }
        }

        public void WyczyscCacheKategorii(IList<object> obj)
        {
            _ProduktyKategorieGrupowanePoProdukcie = null;
            _ProduktyKategorieGrupowanePoKategorii = null;            
        }
    }
}