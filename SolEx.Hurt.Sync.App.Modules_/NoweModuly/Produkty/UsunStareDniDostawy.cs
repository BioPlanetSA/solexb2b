using System;
using System.Collections.Generic;
using System.Globalization;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class UsunStareDniDostawy : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        public override string Opis
        {
            get { return "Moduł usuwa z pola 'dostawa' w produkcie dostawy, których data jest starsza od daty bieżącej ( zostawia dzień dzisiejszy)"; }
        }

        public UsunStareDniDostawy()
        {
            FormatDaty = "dd.MM.yyyy";
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            foreach (Produkt p in listaWejsciowa)
            {
                DateTime data;
                if (DateTime.TryParseExact(p.Dostawa, FormatDaty, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None,  out data) && DateTime.Now.Date > data)
                    p.Dostawa = null;
            }
        }

        [FriendlyName("Format daty, wg której dane z pola 'dostawa' będą parsowane")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string FormatDaty { get; set; }

        public override string uwagi
        {
            get { return "Moduł musi być uruchomiony PO module, który ustawia zawartość pola 'dostawa'"; }
        }
    }
}
