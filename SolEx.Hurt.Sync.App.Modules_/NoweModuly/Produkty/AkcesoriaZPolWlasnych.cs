using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Pobierz akcesoria z pola własnego/atrybutu")]
    public class AkcesoriaZPolWlasnych : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        [FriendlyName("Atrybut z którego będą pobierane akcesoria")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Atrybuty { get; set; }
        
        [FriendlyName("Separator")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public char Separator { get; set; }

        [FriendlyName("Według jakiego pola wyszukiwać produkty")]
        [PobieranieSlownika(typeof(SlownikPolProduktow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole  { get; set; }

        [FriendlyName("Czy przy synchronizacji kopiować akcesoria na produkty w rodzinie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyKopiowacAkcesoriaWRodzinie { get; set; }
        
        public AkcesoriaZPolWlasnych()
        {
            Separator = ';';
        }


        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (Atrybuty == null || !Atrybuty.Any())
            {
                throw new Exception("Brak wybranego Atrybutu");
            }
            List<int> atrybutyid = Atrybuty.Select(int.Parse).ToList();
            List<Cecha> listaCechZAtrybutami = new List<Cecha>();
            for (int i = 0; i < CechyNaB2B.Count; i++)
            {
                if (atrybutyid.Contains(CechyNaB2B[i].AtrybutId.GetValueOrDefault()))
                {
                    listaCechZAtrybutami.Add(CechyNaB2B[i]);
                }
            }
            foreach (var cecha in listaCechZAtrybutami)
            {
                List<long> listaIdProduktow = listaWejsciowa.Select(x => x.Id).ToList();
                List<long> idProduktowzCecha = lacznikiCech.Where(x => x.Value.CechaId == cecha.Id && listaIdProduktow.Contains(x.Value.ProduktId)).Select(y => y.Value.ProduktId).ToList();
                if (!idProduktowzCecha.Any())
                {
                    continue;
                }
                string[] akcesoria = cecha.Nazwa.Split(Separator);
                Produkt p = new Produkt();
                PropertyInfo pi = p.GetType().GetProperties().FirstOrDefault(x => x.Name == Pole);
                if (pi == null)
                {
                    throw new InvalidOperationException("Brak pola o nazwie " + Pole);
                }
                foreach (var idProduktu in idProduktowzCecha)
                {
                    var produkt = listaWejsciowa.First(x => x.Id == idProduktu);
                    foreach (var akces in akcesoria)
                    {
                        var akcesoriaProdukty = listaWejsciowa.Where(x => (pi.GetValue(x)??"").ToString() == akces).ToList();
                        if (!akcesoriaProdukty.Any())
                        {
                            continue;
                        }
                        List<long> listaIdAkcesoriow = akcesoriaProdukty.Select(y => y.Id).ToList();
                        List<long> idProduktowWRodzinie;
                        if (CzyKopiowacAkcesoriaWRodzinie && !string.IsNullOrEmpty(produkt.Rodzina))
                        {
                            idProduktowWRodzinie = listaWejsciowa.Where(x => x.Rodzina == produkt.Rodzina).Select(x => x.Id).ToList();
                        }
                        else
                        {
                            idProduktowWRodzinie = new List<long> { produkt.Id };
                        }

                        foreach (var ak in listaIdAkcesoriow)
                        {
                           foreach (var id in idProduktowWRodzinie)
                            {
                                if (!zamienniki.Any(x => x.ProduktId == id && x.ZamiennikId == ak))
                                {
                                    zamienniki.Add(new ProduktyZamienniki(id, ak));
                                } 
                            }
                        }
                    }
                }
            }
        }
        private List<Cecha> _cechyB2B;
        public virtual List<Cecha> CechyNaB2B
        {
            get
            {
                return _cechyB2B ?? (_cechyB2B = ApiWywolanie.PobierzCechy().Values.ToList());
            }
        }


    }
}
