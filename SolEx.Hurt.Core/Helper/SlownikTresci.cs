using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikTresci : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescBll>(null,x=>!string.IsNullOrEmpty(x.Symbol)).ToDictionary(x => string.Format("{0} - {1}", x.Nazwa, x.Id), x => (object) x.Symbol);
            }
        }
    }
}
