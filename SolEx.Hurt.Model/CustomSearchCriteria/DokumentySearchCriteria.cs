using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using System.Linq.Expressions;
using System.Web.Compilation;
using ServiceStack.Common;
using ServiceStack.OrmLite;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class DokumentySearchCriteria: BaseSearchCriteria
    {
        private List<RodzajDokumentu> _rodzaj;
        public DokumentySearchCriteria()
        {
            _rodzaj = new List<RodzajDokumentu>();
            TylkoAktualne = false;
        }
     
        [PomijajAtrybut]
        public List<RodzajDokumentu> Rodzaj { get { return _rodzaj; } }

        public bool TylkoAktualne { get; set; }
      
        public Expression<Func<HistoriaDokumentu, bool>> ZbudujWarunek()
        {
            Expression<Func<HistoriaDokumentu, bool>> warunek = null;

            if (_rodzaj.Any())
            {
                if (warunek == null)
                {
                    warunek = x => Sql.In(x.Rodzaj, _rodzaj);
                }
            }
            if (TylkoAktualne)
            {
                if (warunek == null)
                {
                    warunek = x => x.TerminPlatnosci!=null && x.TerminPlatnosci < DateTime.Now.Date;
                }
                else
                {
                    warunek = warunek.And(x => x.TerminPlatnosci != null && x.TerminPlatnosci < DateTime.Now.Date);
                }
            }

            return warunek;
        }
    }
}
