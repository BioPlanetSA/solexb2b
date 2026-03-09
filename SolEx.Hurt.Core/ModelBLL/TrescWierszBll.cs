using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
      [Alias("TrescWiersz")]
    public class TrescWierszBll:TrescWiersz,IPoleJezyk
    {
        [Ignore]
        public List<TrescKolumnaBll> Kolumny
        {
            get
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz(JezykId, null, x => x.TrescWierszId == Id, new[] { new SortowanieKryteria<TrescKolumnaBll>(x => x.Kolejnosc, KolejnoscSortowania.asc, "Kolejnosc") }).ToList();
            }

        }
          [Ignore]
        public int JezykId { get; set; }
          [Ignore]
          public IObrazek Tlo
          {
              get
              {
                  if (ObrazekTla != null)
                  {
                      return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ObrazekTla.Value);
                  }
                  return null;
              }
          }
    }



    public class TrescWierszBllImport : TrescWiersz
    {
        [Ignore]
        public List<TrescKolumna> Kolumny { get; set; }
        public static explicit operator TrescWierszBllImport(TrescWierszBll b)  // explicit byte to digit conversion operator
        {
            TrescWierszBllImport tbi = new TrescWierszBllImport();
            tbi.KopiujPola(b, new { b.Kolumny });
            tbi.Kolumny=new List<TrescKolumna>(b.Kolumny);
            return tbi;

        }
    }
}
