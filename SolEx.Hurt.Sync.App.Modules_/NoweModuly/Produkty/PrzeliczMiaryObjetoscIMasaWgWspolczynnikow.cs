using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Przelicz obiętość i wagę wg współczynnika.")]
    public class PrzeliczMiaryObjetoscIMasaWgWspolczynnikow : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        public PrzeliczMiaryObjetoscIMasaWgWspolczynnikow()
        {
            MnoznikDlaObjetosci = 1;
            MnoznikDlaWag = 1;
            Zaokraglenie = 3;
        }
        [FriendlyName("Mnożnik dla wagi")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal MnoznikDlaWag { get; set; }
        
        [FriendlyName("Mnożnik dla objętości")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal MnoznikDlaObjetosci { get; set; }
        
        [FriendlyName("Ilość miejść do której zaokrąglamy wartość, maksymalnie wartość to 3")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int Zaokraglenie { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            //W badzie przechowujemy zaokrąglenie do 3 miejsc
            if (Zaokraglenie > 3)
            {
                Zaokraglenie = 3;
            }
            decimal m = (decimal)Math.Pow((double)MnoznikDlaObjetosci, 3);
            foreach (var produkt in listaWejsciowa)
            {
                if (produkt.Waga!=null && (produkt.Waga != 0 || MnoznikDlaWag!=1))
                {
                    produkt.Waga = decimal.Round(produkt.Waga.Value*MnoznikDlaWag, Zaokraglenie);
                }
                if (produkt.Objetosc != null && (produkt.Objetosc != 0 || MnoznikDlaObjetosci!=1))
                {
                    produkt.Objetosc = decimal.Round(produkt.Objetosc.Value*m,Zaokraglenie);
                }
            }
        }

        public override string uwagi
        {
            get { return ""; }
        }
    }
}
