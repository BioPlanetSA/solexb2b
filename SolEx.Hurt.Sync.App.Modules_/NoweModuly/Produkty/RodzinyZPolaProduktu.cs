using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.Rozne;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class RodzinyZPolaProduktu : ProduktySMMASH, Model.Interfaces.SyncModuly.IModulProdukty
    {
        [FriendlyName("Id atrybutu rodziny np rozmiar")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int idAtrybutu { get; set; }

        [FriendlyName("Pole produktu, z którego zostanie pobrana rodzina i atrybut np rozmiar. Rodzina i rozmiar muszą być oddzielone znakiem -")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleProduktu { get; set; }

        [FriendlyName("Kopiować nazwę rodziny do nazwy produktu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool KopiowacRodzine{ get; set; }
       
        public override string Opis
        {
            get { return "Moduł do wypełniania pola rodzina na podstawie wybranego pola produktu i atrybutu np rozmiaru"; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (idAtrybutu == 0)
            {
                throw new Exception("Brak definicji atrybutu dla rozmiarów");
            }

            if (PoleProduktu.IsNullOrEmpty())
            {
                throw new Exception("Pole produktu jest puste!");
            }

            //var atrybuty = ApiWywolanie.PobierzAtrybuty().Values;
            //var cechy = ApiWywolanie.PobierzCechy().Values;

            Atrybut atrybut = atrybuty.FirstOrDefault(a => a.Id == idAtrybutu);
            if(atrybut == null)
                throw new Exception("Na platformie nie znaleziono atrybutu o ID " + idAtrybutu);

            List<Cecha> listaCech = cechy.Where(a => a.AtrybutId == atrybut.Id).ToList();

            Dictionary<long, HashSet<long>> pogrupowane =  lacznikiCech.Values.GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => new HashSet<long>( x.Select(y => y.CechaId) ) );
            foreach (var produkt in listaWejsciowa)
            {
                
                object obiekt = produkt.PobierzWartoscPolaObiektu(PoleProduktu);
                if (obiekt == null)
                {
                    Log.Error(string.Format("Pole {0} w produkcie {1} nie ma żadnej wartości!", PoleProduktu, produkt.Kod));
                    continue;
                }
                string wybranepole = obiekt.ToString();
                string rozmiar = PobierzRozmiarMod(wybranepole);
                string rozmiarlower = rozmiar.ToLower();
                string bazowyKodBezRozmiaru = PobierzBazowyKodBezRozmiaru(wybranepole, rozmiar).TrimEnd('-').TrimEnd();
                bazowyKodBezRozmiaru = NowaNazwa(bazowyKodBezRozmiaru);
                Cecha cecha = ZnajdzCeche(listaCech, rozmiarlower);
                if (cecha == null)
                {
                    Log.Error(string.Format("Dla produktu {0} nie znaleziono cechy o nazwie {1}", produkt.Kod, rozmiar));
                    continue;
                }

                ProduktCecha lacznik = new ProduktCecha(produkt.Id, cecha.Id);
                if (!pogrupowane.ContainsKey(produkt.Id))
                {
                    pogrupowane.Add(produkt.Id, new HashSet<long>());
                }

                if (!pogrupowane[produkt.Id].Contains(cecha.Id))
                {
                    lacznikiCech.Add(lacznik.Id,lacznik);
                    pogrupowane[produkt.Id].Add(cecha.Id);
                }
                produkt.Rodzina = bazowyKodBezRozmiaru;
                if (KopiowacRodzine)
                {
                    produkt.Nazwa = bazowyKodBezRozmiaru;
                }
            }
        }

        public string NowaNazwa(string staraNazwa)
        {
            string nowanazwa = "";
            string[] lista = staraNazwa.Split(new string[] {" ", "-"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in lista)
            {
                nowanazwa += s.ToUpper() + " ";
            }
            return nowanazwa.Trim();
        }

        public Cecha ZnajdzCeche(List<Cecha> lista, string nazwa)
        {
            return lista.FirstOrDefault(a => a.Nazwa.ToLower() == nazwa);
        }
    }
}
