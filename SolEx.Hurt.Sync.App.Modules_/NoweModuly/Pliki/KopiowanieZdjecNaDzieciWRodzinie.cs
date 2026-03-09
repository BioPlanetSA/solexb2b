using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    public class KopiowanieZdjecNaDzieciWRodzinie : SyncModul, IModulPliki
    {
        public void Przetworz(IDictionary<long, Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, Model.Interfaces.Sync.ISyncProvider provider, ref List<Cecha> cechyB2B, ref List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B)
        {
            if (!plikiLokalnePowiazania.Any())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak łaczników do plików sprawdź czy moduł do łaczników jest załączony. ");
                return;
            }

            List<Produkt> listaProduktow = produktyNaB2B.Select(x => x.Value).ToList();
            Dictionary<string, List<Produkt>> slownikRodzin = SlownikRodzin.PobierzRodzinySlownik(listaProduktow);
            HashSet<long> produktyZeZdjeciem = new HashSet<long>( plikiLokalnePowiazania.Select(x => x.ProduktId) );


            if (!slownikRodzin.Any())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak rodzin - przerywam działanie. ");
                return;
            }
            int ileZmian = 0;
            LogiFormatki.PobierzInstancje.LogujInfo("Przetwarzam {0} rodzin", slownikRodzin.Count);

                foreach (var rodzinaProdukt in slownikRodzin)
                {
                    HashSet<long> idProduktowBezZdjeciaWRodzinie =new HashSet<long>( rodzinaProdukt.Value.Where(x => !produktyZeZdjeciem.Contains(x.Id)).Select(x => x.Id) );
                    HashSet<long> idProduktowZeZdjeciemRodzinie = new HashSet<long>( rodzinaProdukt.Value.Where(x => produktyZeZdjeciem.Contains(x.Id)).Select(x => x.Id) );

                    if (!idProduktowZeZdjeciemRodzinie.Any())
                    {
                        LogiFormatki.PobierzInstancje.LogujInfo("Żaden produkt w rodzinie: {0} nie posiada zdjęcia, pomijam tą rodzine.",rodzinaProdukt.Key);
                    }
                    else
                    {
                        List<ProduktPlik> powiazaniaRodzina = plikiLokalnePowiazania.Where(x => idProduktowZeZdjeciemRodzinie.Contains(x.ProduktId)).ToList();
                        Dictionary<int, bool> produktNajwiecejZdjec = powiazaniaRodzina.GroupBy(x => x.ProduktId).OrderByDescending(y => y.Count()).First().ToDictionary(x => x.PlikId, x => x.Glowny);
                        foreach (var produkt in idProduktowBezZdjeciaWRodzinie)
                        {
                            WygenereujProduktyPliki(produkt, produktNajwiecejZdjec, ref plikiLokalnePowiazania);
                        }
                        Log.DebugFormat("Uzupełniono zdjęcia dla dzieci w rodzinie: {0}", rodzinaProdukt.Key);
                        ++ileZmian;
                    }
                }
            LogiFormatki.PobierzInstancje.LogujInfo("Poprawiono {0} rodzin", ileZmian);
        }

        public override string uwagi
        {
            get { return ""; }
        }

        public void WygenereujProduktyPliki(long idProd, Dictionary<int, bool> zdjecia, ref List<ProduktPlik> plikiLokalne)
        {
            foreach (var i in zdjecia)
            {
                ProduktPlik pp = new ProduktPlik(){ProduktId = idProd, PlikId = i.Key, Glowny = i.Value};
                plikiLokalne.Add(pp);
            }
        }
    }
    
}
