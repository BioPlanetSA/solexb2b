using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [ModulStandardowy]
    [SynchronizowanePola(typeof(Produkt))]
    public class KtorePolaSynchronizowac : KtorePolaSynchonizowacBaza, IModulProdukty, IModulPola
    {
        public KtorePolaSynchronizowac()
        {
            Pola = new List<string>();
            Pola.Add("WyslanoMailNowyProdukt");
        }

        [FriendlyName("Wybierz pola które będziesz edytował w adminie B2B. Pola nie wybrane będa nadpisywane wartościami z ERP (są oznaczone na zielono w adminie)")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (Pola.Count == 0)
            {
                return;
            }
            PropertyInfo[] propertisy = typeof(Produkt).GetProperties();

            foreach (Produkt wzorzec in produktyNaB2B.Values)
            {
                Produkt docelowy = listaWejsciowa.FirstOrDefault(x => x.Id == wzorzec.Id);
                UstawPola(docelowy,wzorzec,propertisy,Pola);
            }
        }

        public List<string> PobierzDostepnePola()
        {
            return Pola;
        }
    }

}
