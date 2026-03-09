using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Dodaj cechę na podstawie innej cechy",FriendlyOpis = "Przypisuje cechę produktowi jeśli ma już inną cechę. Obie cechy muszą istnieć w erp")]
    public class DodawanieCechyJesliProduktMaCeche : SyncModul,IModulProdukty
    {
        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            Dictionary<long, Cecha> cechyTmp = CechyNaB2B;
            Cecha c = cechyTmp.Values.FirstOrDefault(x => x.Symbol == CechaDodawanej);
            if (c == null)
            {
                throw  new Exception(string.Format("Brak cechy o symbolu {0} do przypisania. Moduł {1} kończy działanie", CechaDodawanej, GetType().Name));
            }
            Cecha cw = cechyTmp.Values.FirstOrDefault(x => x.Symbol == CechaWymagana);
            if (cw == null)
            {
               throw  new Exception(string.Format("Brak cechy o symbolu {0} którą mają mieć produkty. Moduł {1} kończy działanie", CechaWymagana, GetType().Name));
            }
            var lacznikiwgcechy = lacznikiCech.Values.GroupBy(x => x.CechaId).ToDictionary(x => x.Key, x => x.ToList());

            if (!lacznikiwgcechy.ContainsKey(cw.Id))
            {
               return;
            }
            foreach (ProduktCecha cechy_Produkty in lacznikiwgcechy[cw.Id]) //przelatujemy po produktach mająchych wymaganą cechę
            {
                ProduktCecha dodawana = new ProduktCecha {CechaId = c.Id, ProduktId = cechy_Produkty.ProduktId};
               
                if (lacznikiCech.ContainsKey(dodawana.Id))
                {
                    continue;
                }
                lacznikiCech.Add(dodawana.Id, dodawana);
            }
        }
        [FriendlyName("Symbol cechy, którą musi mieć produkt")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string CechaWymagana { get; set; }
        
        [FriendlyName("Symbol dodawanej cechy. Musi istnieć w erp")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string CechaDodawanej{ get; set; }
        

        public virtual Dictionary<long, Cecha> CechyNaB2B 
        {
            get
            {
                return ApiWywolanie.PobierzCechy();
            }
        }
    }
}
