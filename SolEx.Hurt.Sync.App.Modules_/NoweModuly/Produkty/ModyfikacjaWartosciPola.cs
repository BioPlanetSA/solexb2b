
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class ModyfikacjaWartosciPola : KtorePolaSynchonizowacBaza, IModulProdukty
    {
   
        [FriendlyName("Pole które ma zostać zmodyfikowane")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pola { get; set; }
        
        [FriendlyName("Nowa wartośc pola, za {0} będzie podstawiona stara wartość")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NowaWartoscPola { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            PropertyInfo[] propertisy = typeof (Produkt).GetProperties();
            var akcesor = typeof(Produkt).PobierzRefleksja();
            PropertyInfo pi = propertisy.First(x => x.Name == Pola);
            foreach (Produkt p in listaWejsciowa)
            {
                object wartosc = akcesor[p, pi.Name];
                string nowa = string.Format(NowaWartoscPola, wartosc);
                akcesor[p, pi.Name] = nowa;
            }
        }

    }
}
