using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.Text;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Przepisywanie cech dla nowych produktów", FriendlyOpis = "")]

    public class PrzypisanieCechyDlaNowychProduktow : SyncModul, IModulProdukty
    {
        public PrzypisanieCechyDlaNowychProduktow()
        {
            IloscDniWstecz = 14;
        }
        [FriendlyName("Symbole cech rozdzielone srednikiem, których towar NIE może mieć, aby przypisać mu cechę nowość")]
        [Niewymagane]
        [Obsolete("Pole wycofane zaleca się korzystanie z listy rozwijanej ListaCechWykluczenia")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Wykluczenia { get; set; }

        [FriendlyName("Cechy, których towar NIE może mieć, aby przypisać mu cechę nowość")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> ListaCechWykluczenia { get; set; }

        [FriendlyName("Symbol cechy do przypisania")]
        [Obsolete("Pole wycofane zalece się używać pola CechaDoDodania")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SymbolCechy { get; set; }

        [FriendlyName("Cecha do przypisania")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int CechaDoDodania { get; set; }

        [FriendlyName("Ile dni wstecz dodawać nowość - podajemy wartości dodatnie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int IloscDniWstecz { get; set; }

        

        //[FriendlyName("Ilu najnowszym produktom prazypisać cechę")]
        //public int IleProduktow { get; set; }
        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> poduktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii,
            ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            HashSet<long> wykluczenia=new HashSet<long>();
            List<long> idProduktowZDodanaCecha = new List<long>();
            DateTime dataOdKiedy = DateTime.Now.AddDays(-IloscDniWstecz);
            Cecha c = CechaDoDodania!=0 ? CechyNaB2B.Values.FirstOrDefault(x=>x.Id==CechaDoDodania) : CechyNaB2B.Values.FirstOrDefault(x => x.Symbol.Equals(SymbolCechy, StringComparison.InvariantCultureIgnoreCase));
            if (c == null)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak cechy o symbolu {0} do przypisania. Moduł {1} kończy działanie",SymbolCechy,GetType().Name);
                return;
            }
            lacznikiCech = lacznikiCech.Where(x => x.Value.CechaId != c.Id).ToDictionary(x => x.Key, x => x.Value);
            if (ListaCechWykluczenia!=null && ListaCechWykluczenia.Any())
            {
                wykluczenia = new HashSet<long>( ListaCechWykluczenia.Select(long.Parse) );
            }
            else if (!string.IsNullOrEmpty(Wykluczenia))
            {
                foreach (var wyk in Wykluczenia.Split(new []{";"},StringSplitOptions.RemoveEmptyEntries))
                {
                    Cecha cecha = CechyNaB2B.Values.FirstOrDefault(x => x.Symbol.Equals(wyk, StringComparison.InvariantCultureIgnoreCase));
                    if (cecha != null)
                    {
                        wykluczenia.Add(cecha.Id);
                    }
                }
            }
            //tworze słownik dat z produktów które są na B2B #10434
            Dictionary<long, DateTime> slownikDatDodania = produktyNaB2B.ToDictionary(x => x.Key, x => x.Value.DataDodania);
            //sprawdzamy datę po słowniku bo na produktach z erpa nie ma dat dodania
            var nowe = listaWejsciowa.Where(x => !slownikDatDodania.ContainsKey(x.Id) || (slownikDatDodania[x.Id] >= dataOdKiedy));//listaWejsciowa.OrderByDescending(x => x.produkt_id);
            //var nowe = listaWejsciowa.Where(x => x.DataDodania >= dataOdKiedy);//listaWejsciowa.OrderByDescending(x => x.produkt_id);
            var lacznikipoprodukcie = lacznikiCech.Values.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => new HashSet<long>( x.Select(y => y.CechaId) ));

            foreach (Produkt p in nowe)
            {

                if (wykluczenia.Any() && lacznikipoprodukcie.ContainsKey(p.Id))//być może będzie trzeba pominąć
                {
                    if (lacznikipoprodukcie[p.Id].Overlaps(wykluczenia))//produkt ma którąś z pomijanych cech
                    {
                        continue;
                    }
                }

                idProduktowZDodanaCecha.Add(p.Id);
                var cp = new ProduktCecha {ProduktId = p.Id, CechaId = c.Id};
                lacznikiCech.Add(cp.Id, cp); 
            }
            if (idProduktowZDodanaCecha.Any())
            {
                Log.DebugFormat("Cecha: {0} została dodana następującym produktom: {1}\r\n", c.Symbol, listaWejsciowa.Where(x => idProduktowZDodanaCecha.Contains(x.Id)).Select(x => x.Kod).ToCsv());
            }

        }

        private Dictionary<long, Cecha> _cechyB2b; 
        public virtual Dictionary<long, Cecha> CechyNaB2B
        {
            get { return _cechyB2b ?? (_cechyB2b = ApiWywolanie.PobierzCechy()); }
        }
        public override string uwagi
        {
            get { return "Symbol cechy już istniejącej w systemie, cecha nie zostanie stworzona automatycznie"; }
        }
    }
}
