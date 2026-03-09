using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class UkrywanieProduktowBezZdjec : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            List<Plik> pliki = ApiWywolanie.PlikNaB2BPobierz();
            List<ProduktPlik> laczniki = ApiWywolanie.PlikiProduktowPobierz().Values.ToList();


            List<int> zdjecia = pliki.Where(x => x.RodzajPliku == RodzajPliku.Zdjecie).Select(x => x.Id).ToList();
            var produktyZeZdjeciem = laczniki.Where(x => zdjecia.Contains(x.PlikId)).Select(x => x.ProduktId).ToList();

            foreach (var produkty in listaWejsciowa)
            {
                if (!produktyZeZdjeciem.Contains(produkty.Id))
                {
                    produkty.Widocznosc = AccesLevel.Zalogowani;
                }
            }

        }

        public override string Opis
        {
            get
            {
                return "Modul ukrywający produkty bez zdjęć dla klientów niezalogowanych ";
            }
        }
        public override string uwagi
        {
            get { return ""; }
        }

       
    }
}
