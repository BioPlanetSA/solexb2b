using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
   public  class ParametryZamawianiaNaPodstawieCechy : SyncModul, IModulProdukty
    {
        public ParametryZamawianiaNaPodstawieCechy()
        {
            Separator = "/";
        }
        public override string uwagi
        {
            get { return "Pobieranie minimum logistycznego i opakowania zbiorczego na podstawie atrybutu. Dane w formie (minimum logistyczne)(separator)(opakowanie zbiorcze)"; }
        }
        [FriendlyName("Początek symbolu cechy x podstawie której mają być brane mimima i opakowania")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoczatekCechy { get; set; }
           
        [FriendlyName("Znak oddzialający wartości")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }
        
       private List<Cecha> _cechy; 
        public virtual List<Cecha> Cechy
        {
            get
            {
                if (_cechy == null)
                {
                    _cechy = ApiWywolanie.PobierzCechy().Values.ToList();
                }
                return _cechy;
            }
        }
        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            var lacznikwgproduktu = lacznikiCech.Values.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => x.Select(y=>y.CechaId).ToList());
            var pasujacecechy = Cechy.Where(x => x.Symbol.StartsWith(PoczatekCechy, StringComparison.InvariantCultureIgnoreCase)).ToList();
            foreach (var p in listaWejsciowa)
            {
                if (!lacznikwgproduktu.ContainsKey(p.Id))
                {
                    continue;
                }
                var cecha = pasujacecechy.FirstOrDefault(x => lacznikwgproduktu[p.Id].Contains(x.Id));
                if (cecha != null)
                {
                    if (!cecha.Nazwa.Contains(Separator))
                    {
                        LogiFormatki.PobierzInstancje.LogujInfo($"Produkt {p.Kod} ma cechę: {cecha.Nazwa} ale nie posiada ona wymaganego separatora: [{Separator}], pomijam produkt.");
                        continue;
                    }
                    var parametry = cecha.Nazwa.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);
                    decimal minlog;
                    if (parametry.Length == 0 || !TextHelper.PobierzInstancje.SprobojSparsowac(parametry[0], out minlog))
                    {
                        LogiFormatki.PobierzInstancje.LogujInfo($"Produkt {p.Kod} ma cechę, ale nie udało się sparsować minimum logistycznego - cecha nazwa: [{cecha.Nazwa}]");
                    }
                    else
                    {
                        p.IloscMinimalna = minlog;
                    }
                    decimal opakowanie;
                    if (parametry.Length < 2 || !TextHelper.PobierzInstancje.SprobojSparsowac(parametry[1], out opakowanie))
                    {
                        LogiFormatki.PobierzInstancje.LogujInfo($"Produkt {p.Kod} ma cechę, ale nie udało się sprarsować ilości w opakowaniu,  cecha nazwa: [{cecha.Nazwa}]");
                    }
                    else
                    {
                        p.IloscWOpakowaniu = opakowanie;
                    }
                }
            }
        }
    }
}
