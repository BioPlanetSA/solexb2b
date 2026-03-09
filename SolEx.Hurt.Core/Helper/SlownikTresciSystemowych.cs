using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikTresciSystemowych : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                //var parent = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => x.Symbol == "StronySystemowe", null);
                //chcemy wyciagac wszystkie dzieci - nie tylko systemowe
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescBll>(null,x=>!string.IsNullOrEmpty(x.Symbol)).PobierzWszystkieDzieci(x => x.Id, x => x.NadrzednaId, null).ToDictionary(x => string.Format("{0} - {1}", x.Nazwa, x.Id), x => (object) x.Symbol);
            }
        }
    }
}
