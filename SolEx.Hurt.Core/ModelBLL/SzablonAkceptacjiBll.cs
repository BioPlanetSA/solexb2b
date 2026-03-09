using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("SzablonAkceptacji")]
    public class SzablonAkceptacjiBll:SzablonAkceptacji
    {
        public SzablonAkceptacjiBll()
        {
            Klienci=new List<Klient>();
           
        }
        [Ignore]
        public IList<Klient> Klienci { get; set; }

        [Ignore]
        public IList<SzablonAkceptacjiPoziomy> Poziomy
        {
            get
            {
             return   SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz(null, x => x.SzablonAkceptacjiId == Id, new List<SortowanieKryteria<SzablonAkceptacjiPoziomy>> { new SortowanieKryteria<SzablonAkceptacjiPoziomy>(x => x.Poziom, KolejnoscSortowania.asc, "Poziom") });
            }
        }
    
    }
}
