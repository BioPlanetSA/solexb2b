using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class OpakowanieZbiorczeZJednostki : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {       
        [FriendlyName("Początek nazwy jednostki, z której zostanie pobrane opakowanie zbiorcze.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoczatekJednostki { get; set; }

        public OpakowanieZbiorczeZJednostki() 
        {
            PoczatekJednostki = string.Empty;
        }

        public override string uwagi
        {
            get { return ""; }
        }

        public override string Opis
        {
            get { return "Moduł, który automatycznie przydziela opakowanie zbiorcze pobierane z wybranej jednostki miary."; }
        }

        [FriendlyName("Czy produkt ma mieć ustawione pole WymaganeOz", FriendlyOpis = "To pole może zostać ustawione tylko w przypadku, gdy moduł dotyczy jednostki KG lub L")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        public bool UstawWymaganeOz { get; set; }


        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (string.IsNullOrEmpty(PoczatekJednostki))
                return;
           
        
            foreach (Produkt p in listaWejsciowa)
            {
                JednostkaProduktu pj =
                    jednostki.FirstOrDefault(a => a.Nazwa.StartsWith(PoczatekJednostki) && a.ProduktId == p.Id);
                
                if (pj != null)
                {
                    p.IloscWOpakowaniu = pj.Przelicznik;

                    if (this.UstawWymaganeOz)
                    {
                        p.WymaganeOz = true;
                    }
                }
            }            
        }
    }
}
