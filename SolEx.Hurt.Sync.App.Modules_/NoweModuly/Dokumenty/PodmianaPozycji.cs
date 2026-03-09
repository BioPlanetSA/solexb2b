using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    public class PodmianaPozycji : SyncModul,IModulDokumenty
    {
        public override string uwagi
        {
            get { return "moduł smmash"; }
        }
        public override string Opis
        {
            get { return "Podmienia pozycję na dokumencie na podstawie opisu pozycji"; }
        }
        /// <summary>
        /// Metoda podmieniajaca id, kod oraz nazwe produktu na dane z listy wejsciowej dla produktow z opisem zawierajacym visual
        /// </summary>
        /// <param name="listaWejsciowa"></param>
        /// <param name="statusy"></param>
        /// <param name="hashe"></param>
        /// <param name="klienci"></param>
        public void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy, Dictionary<int, long> hashe, ref List<Klient> klienci)
        {
     
           Dictionary<long, Produkt> wszystkieprodukty= ApiWywolanie.PobierzProdukty();
           Dictionary<long, HashSet<long>> ofertyklientow = new Dictionary<long, HashSet<long>>();
            foreach (var d in listaWejsciowa)
            {
                if (!ofertyklientow.ContainsKey(d.Key.KlientId))
                {
                    HashSet<long> ids = new HashSet<long>( ApiWywolanie.PobierzDostepneProduktyKlienta(d.Key.KlientId) );
                    ofertyklientow.Add(d.Key.KlientId,ids);
                }
                foreach (HistoriaDokumentuProdukt produkty in d.Value)
                {
                    int idx = produkty.Opis.LastIndexOf("visual", StringComparison.OrdinalIgnoreCase);
                    if (idx > -1)
                    {
                        string symbol = produkty.Opis.Substring(idx).ToLower();
                        long rzecz = produkty.ProduktId;
                        if (!string.IsNullOrEmpty(symbol))
                        {
                            symbol = symbol.Trim().ToLower();
                            var pb2B = wszystkieprodukty.WhereKeyIsIn(ofertyklientow[d.Key.KlientId]).FirstOrDefault(x => x.PoleLiczba1 == rzecz && x.PoleTekst2.ToLower() == symbol.ToLower());
                            if (pb2B != null)
                            {
                                produkty.ProduktId = pb2B.Id;
                                produkty.KodProduktu = pb2B.Kod;
                                produkty.NazwaProduktu = pb2B.Nazwa;
                            }
                        }
                    }
                }
            }
        }
    }
}
