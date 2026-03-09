using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [ModulStandardowy]
    [SynchronizowanePola(typeof(Jednostka))]
    public class KtorePolaSynchronizowacJednostki : KtorePolaSynchonizowacBaza, IModulProdukty, IModulPola
    {
        public KtorePolaSynchronizowacJednostki()
        {
            Pola = new List<string>();
        }

        [FriendlyName("Wybierz pola które będziesz edytował w adminie B2B. Pola nie wybrane będa nadpisywane wartościami z ERP (są oznaczone na zielono w adminie)")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Jednostka))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (Pola == null || !Pola.Any())
            {
                return;
            }
            PropertyInfo[] propertisy = typeof(Jednostka).GetProperties();
            var wzorcowe = ApiWywolanie.PobierzJednostki().Values;
            foreach (Jednostka wzorzec in wzorcowe)
            {
                Jednostka docelowy = jednostki.FirstOrDefault(x => x.Id == wzorzec.Id);
                UstawPola(docelowy, wzorzec, propertisy, Pola);
            }
        }

        public List<string> PobierzDostepnePola()
        {
            return Pola;
        }
    }
}
