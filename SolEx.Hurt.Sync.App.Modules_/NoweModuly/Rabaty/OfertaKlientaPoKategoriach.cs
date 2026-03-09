using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    public class OfertaKlientaPoKategoriach : SyncModul, IModulRabaty
    {
        [FriendlyName("Cecha klienta w ERP określająca grupę produktów do ukrycia/pokazania")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string CechaUkrywajaca { get; set; }

        public OfertaKlientaPoKategoriach()
        {
            CechaUkrywajaca = string.Empty;
            TypWidocznosci = KatalogKlientaTypy.Dostepne;
        }

        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Typ widoczności towarów ustawiany dla każdego połączenia klient-kategoria")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KatalogKlientaTypy TypWidocznosci { get; set; }

        public override string Opis
        {
            get { return "Moduł, który ukrywa bądź pokazuje towary dla klientów w wybranych kategoriach na podstawie cechy klienta"; }
        }


        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            int iloscPowiazanPoczatek = produktyUkryteNaB2B!=null && produktyUkryteNaB2B.Any()?produktyUkryteNaB2B.Count:0;
            if (string.IsNullOrEmpty(CechaUkrywajaca))
            {
                Log.Error("Początek cechy jest pusty, moduł zakończy działanie!");
                return;
            }

            if(produktyUkryteNaB2B == null)
                produktyUkryteNaB2B = new List<ProduktUkryty>();
            CechaUkrywajaca = CechaUkrywajaca.ToUpper();
            
            List<KategoriaKlienta> wybranaKategoria = kategorieKlientow.Values.Where(a => a.Grupa == CechaUkrywajaca).ToList();
            if (wybranaKategoria.Count == 0)
            {
                Log.InfoFormat("Nie znaleziono kategorii klienta: {0}. Moduł przerywa działanie.", CechaUkrywajaca);
                return;
            }

            List<string> listamaili = new List<string>();
            foreach (Klient k in kliencib2B.Values)
            {
                if (string.IsNullOrEmpty(k.Email) || listamaili.Contains(k.Email))
                    continue;
                
                listamaili.Add(k.Email);

                foreach (KategoriaKlienta kat in wybranaKategoria)
                {
                    string kategoria = kat.Nazwa.Trim().ToLower();
                    if (
                        klienciKategorie.Values.Any(
                            a => a.KategoriaKlientaId == kat.Id && a.KlientId == k.Id))
                    {
                        KategoriaProduktu kategoriaKlienta =
                            kategorie.Values.FirstOrDefault(a => a.Nazwa.Trim().ToLower() == kategoria);


                        if (kategoriaKlienta != null)
                        {
                            ProduktUkryty pu = new ProduktUkryty();
                            pu.KlientZrodloId = k.Id;
                            pu.Tryb = TypWidocznosci;
                            pu.KategoriaId = kategoriaKlienta.Id;
                            produktyUkryteNaB2B.Add(pu);
                        }
                    }
                }
            }

            Log.DebugFormat("Moduł stworzył {0} powiązań", produktyUkryteNaB2B.Count - iloscPowiazanPoczatek);
            if (Log.IsDebugEnabled)
            {
                if (produktyUkryteNaB2B.Count - iloscPowiazanPoczatek < 5)
                {
                    Log.DebugFormat("Moduł NIC nie wybrał pracującego do warunków. Cała lista nazw kategorii produktowych: {0}\r\n, cała lista kategorii klientów: {1}",
                        kategorie.Values.Select(x => x.Nazwa).ToCsv(),
                        kategorieKlientow.Values.Select(x => x.Nazwa).ToCsv()
                        );
                }
            }

        }

    }
}


