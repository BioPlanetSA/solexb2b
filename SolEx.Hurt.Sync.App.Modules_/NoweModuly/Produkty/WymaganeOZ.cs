using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class WymaganeOZ : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        [FriendlyName("Symbol lub ID cechy, która określa czy opakowanie zbiorcze jest obowiązkowe")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string CechaOZ { get; set; }

        public WymaganeOZ() 
        {
            CechaOZ = string.Empty;
        }

        public override string uwagi
        {
            get { return ""; }
        }

        public override string Opis
        {
            get { return "Moduł, który automatycznie ustawia wymóg opakowania zbiorczego na podstawie wybranej cechy."; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (!string.IsNullOrEmpty(CechaOZ))
            {
               // List<Cecha> cechy = ApiWywolanie.PobierzCechy().Values.ToList();
                int id;
                var cecha = cechy.FirstOrDefault(a => (int.TryParse(CechaOZ, out id) && a.Id == id) || a.Symbol.ToLower().StartsWith(CechaOZ.ToLower()));

                if (cecha != null)
                {
                    foreach (Produkt p in listaWejsciowa)
                    {
                        ProduktCecha tymczasowyLacznik = StworzTymczasowyLacznik(cecha.Id, p.Id);
                      
                        bool wymaganeOZ = lacznikiCech.ContainsKey(tymczasowyLacznik.Id);
                        p.WymaganeOz = wymaganeOZ;
                    }
                }
            }
        }

        private ProduktCecha StworzTymczasowyLacznik(long cechaid, long produktid)
        {
            ProduktCecha cp = new ProduktCecha();
            cp.ProduktId = produktid;
            cp.CechaId = cechaid;
            return cp;
        }
    }
}
