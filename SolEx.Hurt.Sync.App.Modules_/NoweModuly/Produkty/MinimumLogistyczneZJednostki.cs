using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class MinimumLogistyczneZJednostki : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {       
        [FriendlyName("Początek nazwy jednostki, z której zostanie pobrane minimum logistyczne.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoczatekJednostki { get; set; }

        public MinimumLogistyczneZJednostki() 
        {
            PoczatekJednostki = string.Empty;
        }

        public override string Opis
        {
            get { return "Moduł, który automatycznie przydziela minimum logistyczne pobierane z wybranej jednostki miary."; }
        }

        public override string uwagi
        {
            get { return ""; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (string.IsNullOrEmpty(PoczatekJednostki))
            {
                return;
            }

            List<JednostkaProduktu> jednostkiBezRef = jednostki;

            Parallel.ForEach(listaWejsciowa, p =>
            {
                var jednostka = jednostkiBezRef.FirstOrDefault(a => a.Nazwa.StartsWith(PoczatekJednostki) && a.ProduktId == p.Id);

                if (jednostka != null)
                {
                    p.IloscMinimalna = jednostka.Przelicznik;
                }
            });
        }
    }
}
