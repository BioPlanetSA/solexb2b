using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class AutomatycznePodpinanieAkcesoriow : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        [FriendlyName("Cechy produktów dla ktorych będzie dodana lista akcesoriów.")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> CechyProduktowNadrzednych { get; set; }

        [FriendlyName("Cechy produktów które będą traktowane jako akcesoria.")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> CechyAkcesoriow { get; set; }

        [FriendlyName("Atrybut wspólny dla produktu oraz akcesorium. Nie wybrana wartość przypisze wszystkim znalezionym produktom nadrzednym zanlezione akcesoria")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Niewymagane]
        public List<string> WspolneAtrybuty { get; set; }

        [FriendlyName("Cechy wykluczjące produkt jako akcesorium")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Niewymagane]
        public List<string> CechyWykluczajace { get; set; }

        //[FriendlyName("Dwustronne działanie")]
        //public bool Dwustronne { get; set; }
        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, 
            ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, 
            ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, 
            ref List<Atrybut> atrybuty)
        {
            Dictionary<long, ProduktyZamienniki> slownikZamiennikow = zamienniki.ToDictionary(x => x.Id, x => x);
            bool czyWspolneCechy = WspolneAtrybuty!=null && WspolneAtrybuty.Any();
            //LogiFormatki.PobierzInstancje.LogujInfo("Wartosc: {0}",czyWspolneCechy);
            HashSet<long> idCechZAtrybutami = new HashSet<long>();
            if (czyWspolneCechy)
            {
                HashSet<int> atrybutyid = new HashSet<int>( WspolneAtrybuty.Select(int.Parse) );
                for (int i = 0; i < CechyNaB2B.Count; i++)
                {
                    if (atrybutyid.Contains(CechyNaB2B[i].AtrybutId.GetValueOrDefault()))
                    {
                        idCechZAtrybutami.Add(CechyNaB2B[i].Id);
                    }
                }
            }

            LogiFormatki.PobierzInstancje.LogujInfo("Ilość cech z wspólnych atrybutow: {0}", idCechZAtrybutami.Count);
            HashSet<long> cechyProduktowNadrzednychId = new HashSet<long>( CechyProduktowNadrzednych.Select(long.Parse) );
            HashSet<long> cechyAkcesoriowId = new HashSet<long>( CechyAkcesoriow.Select(long.Parse) );
            HashSet<long> cechyWykluczajaceId = new HashSet<long>();
            if (CechyWykluczajace != null && CechyWykluczajace.Any() && CechyWykluczajace[0] != "")
            {
                cechyWykluczajaceId = new HashSet<long>( CechyWykluczajace.Select(long.Parse) );
            }
            
            
            HashSet<long> idProduktyNadrzedne = new HashSet<long>();
            HashSet<long> idAkcesoriow = new HashSet<long>();

           var produktyNadrzedne = lacznikiCech.Where(x => cechyProduktowNadrzednychId.Contains(x.Value.CechaId)).Select(x=>x.Value.ProduktId).ToList();
           List<long> akcesoria = lacznikiCech.Where(x => cechyAkcesoriowId.Contains(x.Value.CechaId)).Select(x=>x.Value.ProduktId).ToList();
            var akcesoriaWykluczone = lacznikiCech.Where(x => cechyWykluczajaceId.Contains(x.Value.CechaId)).Select(x => x.Value.ProduktId).ToList();
            //usuwamy produkty ktore sa wykluczone
            akcesoria.RemoveAll(akcesoriaWykluczone.Contains);

            if (czyWspolneCechy)
            {
                idProduktyNadrzedne = new HashSet<long>( lacznikiCech.Where(x => idCechZAtrybutami.Contains(x.Value.CechaId) && produktyNadrzedne.Contains(x.Value.ProduktId)).Select(x => x.Value.ProduktId).Distinct() );
                idAkcesoriow =new HashSet<long>( lacznikiCech.Where(x => idCechZAtrybutami.Contains(x.Value.CechaId) && akcesoria.Contains(x.Value.ProduktId)).Select(x => x.Value.ProduktId).Distinct() );
            }
            else
            {
                idProduktyNadrzedne = new HashSet<long>( produktyNadrzedne );
                idAkcesoriow = new HashSet<long>( akcesoria );
            }

            LogiFormatki.PobierzInstancje.LogujInfo("Ilość produktów nadrzednych: {0}", idProduktyNadrzedne.Count);
            LogiFormatki.PobierzInstancje.LogujInfo("Ilość wybranych akcesoriow: {0}", idAkcesoriow.Count);

            Dictionary<long, HashSet<long>> SlownikProduktówNadrzednych = new Dictionary<long, HashSet<long>>();
            foreach (var i in idProduktyNadrzedne)
            {
                HashSet<long> id = new HashSet<long>();
                if (idCechZAtrybutami.Any())
                {
                    id = new HashSet<long>( lacznikiCech.Values.Where(x => x.ProduktId == i && idCechZAtrybutami.Contains(x.CechaId)).Select(y => y.CechaId) );
                }
                else
                {
                    id = new HashSet<long>( lacznikiCech.Values.Where(x => x.ProduktId == i).Select(y => y.CechaId) );
                }
                SlownikProduktówNadrzednych.Add(i, id);
            }

            Dictionary<long, HashSet<long>> SlownikAkcesoriow = new Dictionary<long, HashSet<long>>();
            foreach (var i in idAkcesoriow)
            {
                HashSet<long> id = new HashSet<long>();
                if (idCechZAtrybutami.Any())
                {
                    id = new HashSet<long>( lacznikiCech.Where(x => x.Value.ProduktId == i && idCechZAtrybutami.Contains(x.Value.CechaId)).Select(y => y.Value.CechaId) );
                }
                else
                {
                    id = new HashSet<long>( lacznikiCech.Where(x => x.Value.ProduktId == i).Select(y => y.Value.CechaId) );
                }
                SlownikAkcesoriow.Add(i, id);
            }

            if (czyWspolneCechy)
            {
                var nadrzedne = SlownikProduktówNadrzednych.Where(x => x.Value.Overlaps(idCechZAtrybutami)).ToList();
                var akcesoriaa = SlownikAkcesoriow.Where(x => x.Value.Overlaps(idCechZAtrybutami)).ToList();

                foreach (var i in nadrzedne)
                {
                    var aaa = akcesoriaa.Where(x => x.Value.Overlaps(i.Value)).Select(x=>x.Key).ToList();
                    foreach (var i1 in aaa)
                    {
                        var zam = new ProduktyZamienniki(i.Key, i1);
                        if (slownikZamiennikow.ContainsKey(zam.Id))
                        {
                            continue;
                        }
                        zamienniki.Add(zam);
                        slownikZamiennikow.Add(zam.Id, zam);
                    }
                }
            }
            else
            {
                foreach (var nad in SlownikProduktówNadrzednych.Keys)
                {
                    foreach (var akc in SlownikAkcesoriow.Keys)
                    {
                        var zam = new ProduktyZamienniki(nad, akc);
                        if (slownikZamiennikow.ContainsKey(zam.Id))
                        {
                            continue;
                        }
                        zamienniki.Add(zam);
                        slownikZamiennikow.Add(zam.Id, zam);
                    }
                }
            }
            LogiFormatki.PobierzInstancje.LogujInfo("Ilość zamiennikow ostatecznie wybranych: {0}", zamienniki.Count);
        }
        private List<Cecha> _cechyB2b;
        public virtual List<Cecha> CechyNaB2B
        {
            get
            {
                if (_cechyB2b == null)
                {
                    _cechyB2b = ApiWywolanie.PobierzCechy().Values.ToList();
                }
                return _cechyB2b;

            }
        }

    }
}
