using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class KopiujOpisKategoriiDoProduktu : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {

        public override string Opis
        {
            get { return "Kopiuje opis z kategorii do opisu produktu w danej kategorii."; }
        }

        [FriendlyName("Pole produktu do którego będzie skopiowany opis z kategorii")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            PropertyInfo pole = typeof(Produkt).GetProperty(Pole);
            foreach (Produkt produkt in listaWejsciowa)
            {
                ProduktKategoria kategorieproduktu = lacznikiKategorii.FirstOrDefault(b => b.ProduktId == produkt.Id);
                if (kategorieproduktu != null)
                {
                    KategoriaProduktu kategoria =
                        kategorie.Values.FirstOrDefault(a => a.Id == kategorieproduktu.KategoriaId);
                    if (kategoria != null)
                    {
                        if (!string.IsNullOrEmpty(kategoria.Opis))
                        {
                            pole.SetValue(produkt, kategoria.Opis);
                        }
                    }
                }
            }
        
        }

        public override string uwagi
        {
            get { return ""; }
        }
    }
}
