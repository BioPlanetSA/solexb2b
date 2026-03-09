using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
namespace SolEx.Hurt.Core
{
    public class PoziomyCenBll :BllBazaCalosc, IPoziomyCenBll
    {
        public PoziomyCenBll(SolexBllCalosc calosc) : base(calosc)
        {
        }

        private static object lok = new object();

        private static Dictionary<long, Dictionary<int, CenaPoziomu> > _slownikCen = null;

        public Dictionary<int, CenaPoziomu> SztucznyPoziomCenowyZerowy { get; set; }

        public Dictionary<int, CenaPoziomu> PobierzCenyProduktu(long produkt)
        {
            if (_slownikCen == null)
            {
                lock (lok)
                {
                    if (_slownikCen == null)
                    {
                        _slownikCen = Calosc.DostepDane.Pobierz<CenaPoziomu>(null).GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => x.ToDictionary(z => z.PoziomId, z => z));
                         SztucznyPoziomCenowyZerowy = new Dictionary<int, CenaPoziomu>();
                      
                        foreach (var poziom in Calosc.Konfiguracja.SlownikPoziomowCenowych)
                        {
                            SztucznyPoziomCenowyZerowy.Add(poziom.Key, new CenaPoziomu(0, 0, 0, poziom.Value.WalutaId));
                        }
                        
                    }
                }
            }
            
            Dictionary<int, CenaPoziomu> wynik;
            return _slownikCen.TryGetValue(produkt, out wynik) ? wynik : SztucznyPoziomCenowyZerowy;
            //try
            //{
            //    return _slownikCen[produkt];
            //}
            //catch
            //{
            //    //BARTEK zmiana  - eksperyment
            //    return SztucznyPoziomCenowyZerowy;
            //    //throw new Exception(string.Format("Brak ceny dla produktu o id:{0}", produkt));
            //}
        }

        public void UsunCache(IList<object> obj)
        {
            _slownikCen = null;
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<ProduktBazowy>());
        }
    }
}
