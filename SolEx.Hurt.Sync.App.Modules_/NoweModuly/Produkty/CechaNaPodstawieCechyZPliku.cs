using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    class CechaNaPodstawieCechyZPliku : SyncModul, IModulProdukty,IModulCechyIAtrybuty
    {
        public void Przetworz(ref List<produkty> listaWejsciowa, ref List<slowniki> produktyTlumaczenia, Dictionary<int, produkty> produktyNaB2B, ref List<JednostkaProduktu> jednostki,
            ref List<cechy_produkty> lacznikiCech, ref Dictionary<int, produkty_kategorie> lacznikiKategorii, Model.Interfaces.Sync.ISyncProvider provider, ref Dictionary<int, produkty_ukryte> produktuUkryteErp)
        {
            //Dictionary<int, cechy> cechy = CechyNaB2B;
            //cechy c = cechy.Values.FirstOrDefault(x => x.symbol == CechaDodawanej);
            //if (c == null)
            //{
            //    throw new Exception(string.Format("Brak cechy o symbolu {0} do przypisania. Moduł {1} kończy działanie", CechaDodawanej, GetType().Name));
            //}
            //cechy cw = cechy.Values.FirstOrDefault(x => x.symbol == CechaWymagana);
            //if (cw == null)
            //{
            //    throw new Exception(string.Format("Brak cechy o symbolu {0} którą mają mieć produkty. Moduł {1} kończy działanie", CechaWymagana, GetType().Name));
            //}
            //var lacznikiwgcechy = lacznikiCech.GroupBy(x => x.cecha_id).ToDictionary(x => x.Key, x => x.ToList());
            //var slownikstring = lacznikiCech.ZbudojSlownikZKluczemPropertisowym();

            //if (!lacznikiwgcechy.ContainsKey(cw.cecha_id))
            //{
            //    return;
            //}
            //foreach (var cechy_Produkty in lacznikiwgcechy[cw.cecha_id]) //przelatujemy po produktach mająchych wymaganą cechę
            //{
            //    cechy_produkty dodawana = new cechy_produkty { cecha_id = c.cecha_id, produkt_id = cechy_Produkty.produkt_id };
            //    string klucz = dodawana.ZbudujKlucz();
            //    if (slownikstring.ContainsKey(klucz))
            //    {
            //        continue;
            //    }
            //    slownikstring.Add(klucz, dodawana);
            //    lacznikiCech.Add(dodawana);
            //}
        }
        [FriendlyName("Ścieżka do pliku csv z danymi")]
        public string SciezkaDoPliku { get; set; }
        public override string uwagi
        {
            get { return "Przypisuje cechę produktowi jeśli ma już inną cechę. Nowa cecha jest automatycznie tworzona"; }
        }
        public virtual Dictionary<int, cechy> CechyNaB2B
        {
            get
            {
                return APIWywolania.PobierzCechy(new CechySearchCriteria());
            }
        }

        public void Przetworz(ref List<atrybuty> atrybuty, ref List<cechy> cechy)
        {
           // throw new NotImplementedException();
        }
    }
}
