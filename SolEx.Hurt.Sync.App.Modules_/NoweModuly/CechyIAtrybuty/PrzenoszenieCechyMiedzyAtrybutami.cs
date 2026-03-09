using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    public class PrzenoszenieCechyMiedzyAtrybutami : SyncModul, IModulCechyIAtrybuty, IModulProdukty
    {
        [FriendlyName("Atrybut, z którego będzie kopiowana cecha")]
        public string AtrybutZrodlo { get; set; }

        [FriendlyName("Atrybut, z którego będzie kopiowana cecha")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        public string AtrybutDocelowy { get; set; }

        [FriendlyName("Jaki separator w postawłej cesze, jeżeli nic nie wpiszesz wybrany zostanie separator z cechy")]
        public string Separator { get; set; }

        

        public void Przetworz(ref List<Model.Atrybut> atrybuty, ref List<Model.Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
           // Dictionary<int, Produkt> produkty = ApiWywolanie.PobierzProdukty();
            Dictionary<long, ProduktCecha> lacznikiCech = ApiWywolanie.PobierzCechyProdukty();

            Przetworz(atrybuty, cechy, produktyNaB2B.Values.ToList(), lacznikiCech);
        }


        public void Przetworz(List<Atrybut> atrybuty, List<Cecha> cechy, List<Produkt> produkty, Dictionary<long, ProduktCecha> lacznikiCech)
        {
            //var listaCechZAtrybutem = cechy.Where
            //LogiFormatki.PobierzInstancje.LogujInfo("nazwa atrybutu o id:839469115 - {0}", atrybuty.First(x => x.atrybut_id == 839469115).nazwa);

            List<Cecha> listaCech = cechy;
            char separatorAtrybutow = SyncManager.PobierzInstancje.Konfiguracja.SeparatorAtrybutowWCechach[0];

            var atrybutZrodlowy = atrybuty.FirstOrDefault(x => x.Nazwa == AtrybutZrodlo);
            if (atrybutZrodlowy == null)
            {
                Log.ErrorFormat("Brak atrybutu zrodłowego: {0}", AtrybutZrodlo);
                return;
            }
            var atrybutDocelowy = atrybuty.FirstOrDefault(x => x.Id == int.Parse(AtrybutDocelowy));
            if (atrybutDocelowy == null)
            {
                Log.ErrorFormat("Brak atrybutu docelowego: {0}", AtrybutDocelowy);
                return;
            }


            string atr = string.Empty;
            atr = string.IsNullOrEmpty(Separator) ? AtrybutZrodlo.ToLower() : string.Format("{0}{1}", AtrybutZrodlo, separatorAtrybutow).ToLower();
            //string atr = string.Format("{0}{1}", AtrybutZrodlo, separatorAtrybutow).ToLower();
            //var cechyZAtrybutemZrod = cechy.Where(x => x.atrybut_id == atrybutZrodlowy.atrybut_id).ToList();
            foreach (var cechy1 in listaCech)
            {
                if (cechy1.AtrybutId != atrybutZrodlowy.Id)
                {
                    continue;
                }
                cechy1.AtrybutId = atrybutDocelowy.Id;
                cechy1.Symbol = cechy1.Symbol.Replace(atr, string.IsNullOrEmpty(Separator) ? atrybutDocelowy.Nazwa.ToLower() : string.Format("{0}{1}", atrybutDocelowy.Nazwa.ToLower(), Separator));
            }
            cechy = listaCech;
        }



        public override string uwagi
        {
            get { return ""; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            //var atrybuty = ApiWywolanie.PobierzAtrybuty();
            //var cechy = ApiWywolanie.PobierzCechy();
            Przetworz(atrybuty, cechy, listaWejsciowa, lacznikiCech);
        }
    }
}
