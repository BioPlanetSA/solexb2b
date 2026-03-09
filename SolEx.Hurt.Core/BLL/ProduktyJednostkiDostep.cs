using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL
{
    public class ProduktyJednostkiDostep : BllBazaCalosc, IProduktyJednostkiDostep
    {
        public ProduktyJednostkiDostep(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        public IDictionary<long, List<JednostkaProduktu>> PobierzJednostkiProduktuWgProduktu(int jezyk)
        {
            string klucz = "produkty_jednostki_grupowwane+jezyk" + jezyk;
            IDictionary<long, List<JednostkaProduktu>> pogrupowane = Calosc.Cache.PobierzChwilowy<IDictionary<long, List<JednostkaProduktu>>>(klucz);

            if (pogrupowane == null)
            {
                var lacznikiPRoduktu = Calosc.DostepDane.Pobierz<ProduktJednostka>(null);
                var jednostki = Calosc.DostepDane.Pobierz<Jednostka>(jezyk, null).ToDictionary(x => x.Id, x => x);
                List<JednostkaProduktu> wynik = new List<JednostkaProduktu>();
                foreach (var lacznik in lacznikiPRoduktu)
                {
                    var j = jednostki[lacznik.JednostkaId];
                    var tmp = new JednostkaProduktu(j, lacznik);
                    wynik.Add(tmp);
                }
                pogrupowane = wynik.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => x.OrderBy(y => y.Przelicznik).ToList());
                Calosc.Cache.DodajChwilowy(klucz, pogrupowane);
            }
            return pogrupowane;
        }
    }
}