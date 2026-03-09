using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class PobieranieZamiennikowErp : SyncModul,IModulProdukty
    {
        public override string uwagi
        {
            get { return "Pobieranie zamienników z erp. Wymaga, aby moduł księgowy dziedziczył po " + typeof (IPobieranieZamiennikow).Name; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            IPobieranieZamiennikow prov = provider as IPobieranieZamiennikow;
            if (prov == null)
            {
                throw new Exception("Moduł księgowy nie dziedziczy po " + typeof(IPobieranieZamiennikow).Name);
            }
            foreach (var p in listaWejsciowa)
            {
                zamienniki.AddRange(prov.PobierzZamiennikiProduktu(p.Id));
            }
        }
    }
}
