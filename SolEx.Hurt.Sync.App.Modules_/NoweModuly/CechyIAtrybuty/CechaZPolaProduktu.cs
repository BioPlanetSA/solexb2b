using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    [FriendlyName("Cecha z pola produktu",FriendlyOpis = "Moduł tworzący cechy i atrybuty na podstawie wybranego pola produktu" )]
    public class CechaZPolaProduktu : CechyModulBaza
    {
        [FriendlyName("Pole produktu z ERP wg ktorego będzie stworzona nazwa cechy")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(true, false, true, false)]
        public string Pole { get; set; }

        [FriendlyName("Atrybut, który będzie utworzony dla wybranego pola produkktu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Atrybut { get; set; }

        [Niewymagane]
        [FriendlyName("Separator oddzielający wartość dla cechy od innych danych w polu produktu (będzie brany ostatni człon)")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }

        public override void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            Dictionary<long, ProduktCecha> lacznikiCech = ApiWywolanie.PobierzCechyProdukty();
            Przetworz(ref atrybuty, ref cechy, produktyNaB2B.Values.ToList(), ref lacznikiCech);
        }
        
        public override void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            Przetworz(ref atrybuty, ref cechy, listaWejsciowa, ref lacznikiCech);
        }

        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, List<Produkt> produkty, ref Dictionary<long, ProduktCecha> lacznikiCech)
        {
            if (string.IsNullOrEmpty(Atrybut))
            {
                throw new Exception("Ustawienie Atrybut nie moze być puste. Popraw konfiguracje modułu.");
            }
            if (string.IsNullOrEmpty(Pole))
            {
                throw new Exception("Ustawienie Pole nie moze być puste. Popraw konfiguracje modułu.");
            }

            Atrybut atrybut = ZnajdzAtrybut(Atrybut, atrybuty);
            if (atrybut == null)
                atrybut = DodajBrakujacyAtrybut(Atrybut, atrybuty);

            HashSet<string> pominiete = new HashSet<string>();
            foreach (Produkt produkt in produkty)
            {
                object nazwa = produkt.PobierzWartoscPolaObiektu(Pole);

                if (nazwa == null)
                {
                    pominiete.Add(produkt.Kod);
                    //Log.ErrorFormat("Nie udało się pobrać wartości pola {0} dla produktu o symbolu {1}", Pole, produkt.Kod);
                    continue;
                }
                string nazwastring = nazwa.ToString();
                string nazwaCechy = string.Empty;

                if (!string.IsNullOrEmpty(Separator))
                {
                    string[] wartosciPola = nazwastring
                        .Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);

                    if (nazwastring.Contains(Separator) && wartosciPola.Length > 1)
                        nazwaCechy = wartosciPola.Last();
                }
                else
                {
                    nazwaCechy = nazwa.ToString();
                }

                if (string.IsNullOrEmpty(nazwaCechy))
                    continue;

                string symbolCechy = $"{Atrybut}:{nazwaCechy.Trim()}".ToLower();

                Cecha cechaProduktu = ZnajdzCeche(symbolCechy, cechy);
                if (cechaProduktu == null)
                {
                    cechaProduktu = DodajBrakujacaCeche(symbolCechy, nazwaCechy, atrybut.Id, cechy);
                }

                DodajBrakujaceLaczniki(cechaProduktu.Id, produkt.Id, lacznikiCech);
            }
            Log.ErrorFormat($"Nie udało się pobrać wartości pola {Pole} dla produktów o symbolach {pominiete.ToCsv()}");
        }
    }
}
