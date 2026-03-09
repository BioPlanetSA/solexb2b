using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Sync
{
    public class ProduktyWyszukiwanie : BLL.BllBaza<ProduktyWyszukiwanie>
    {
        public List<Cecha> PobierzWszystkieCechyProduktu(long produktID, List<Cecha> listacech, List<ProduktCecha> cechylaczniki, string grupa)
        {
            HashSet<long> lacznikiproduktu = null;
            try
            {
                lacznikiproduktu = SolexBllCalosc.PobierzInstancje.CechyProduktyDostep.WszystkieLacznikiWgProduktow[produktID];
            }
            catch (KeyNotFoundException)
            {
                return new List<Cecha>();
            }

            if (!lacznikiproduktu.Any())
            {
                return new List<Cecha>();
            }
            if (grupa.Equals(SolexBllCalosc.PobierzInstancje.Konfiguracja.CechaAuto, StringComparison.InvariantCultureIgnoreCase))
                return null;
            grupa = grupa.ToLower();
            List<string> separatory = SolexBllCalosc.PobierzInstancje.Konfiguracja.SeparatorAtrybutowWCechach.ToArray().Select(a => a.ToString(CultureInfo.InvariantCulture)).ToList();
            Dictionary<long, Cecha> cechyFiltrowane = new Dictionary<long, Cecha>();
            foreach (string separator in separatory)
            {

                string separator1 = separator;
                bool koniecsep = grupa.EndsWith(separator1);
                string poczsym = string.Format("{0}{1}", grupa, separator1);
                var pobrane = listacech.Where(a =>
                    (koniecsep && a.Symbol.Contains(separator1) && a.Symbol.StartsWith(poczsym))
                    || (!koniecsep && a.Symbol.StartsWith(grupa))
                    );

                foreach (var c in pobrane)
                {
                    if (!cechyFiltrowane.ContainsKey(c.Id))
                    {
                        cechyFiltrowane.Add(c.Id, c);
                    }
                }
            }

            List<Cecha> finalnaListaCech = cechyFiltrowane.WhereKeyIsIn(lacznikiproduktu);

            return finalnaListaCech;
        }


        public void SprawdzProduktyUkryte(List<ProduktUkryty> ukryte, Dictionary<long,Model.Klient> klienci, Dictionary<long, Produkt> produkty, Dictionary<long, Cecha> cechy, Dictionary<long, KategoriaProduktu> kategorie, IDictionary<int, KategoriaKlienta> kategorieklienci)
        {
            ukryte.RemoveAll(x => x.ProduktZrodloId != null && !produkty.ContainsKey(x.ProduktZrodloId.Value));
            ukryte.RemoveAll(x => x.KlientZrodloId != null && !klienci.ContainsKey(x.KlientZrodloId.Value));
            ukryte.RemoveAll(x => x.CechaProduktuId != null && !cechy.ContainsKey(x.CechaProduktuId.Value));
            ukryte.RemoveAll(x => x.KategoriaId != null && !kategorie.ContainsKey(x.KategoriaId.Value));
            ukryte.RemoveAll(x => x.KategoriaKlientowId != null && !kategorieklienci.ContainsKey(x.KategoriaKlientowId.Value));
        }
    }
}
