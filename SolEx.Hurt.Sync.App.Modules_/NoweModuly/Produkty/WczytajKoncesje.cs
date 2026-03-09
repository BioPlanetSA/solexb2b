using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Wczytaj koncesje dla produktu z atrybutu.",FriendlyOpis = "Przypisuje wymagane koncesje dla produktu z wybranego atrybutu. Produkt ten będą mogli kupić klienci którzy posiadają odpowiednią koncesje.")]
    public class WczytajKoncesje : SyncModul, IModulProdukty, IModulKlienci
    {
        [FriendlyName("Atrybut z którego będą pobierane koncesje")]
        [PobieranieSlownika(typeof(SlownikAtrybutowKoncesji))]
        [WidoczneListaAdmin(false, false, true, false)]
        public int Atrybut { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, 
            ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki,
            Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if(atrybuty== null || !atrybuty.Any()) return;
            Atrybut atr = atrybuty.FirstOrDefault(x => x.Id == Atrybut);
            if(atr== null) throw new Exception("Brak wybranego atrybutu");
            
            List<Cecha> listaCech = cechy.Where(x => x.AtrybutId == atr.Id).ToList();
            List<KategoriaKlienta> listaKategorii = ApiWywolanie.PobierzKategorieKlientow().Where(x=>x.Value.Grupa.Equals(atr.Nazwa,StringComparison.InvariantCultureIgnoreCase)).Select(x=>x.Value).ToList();
            Dictionary<string, Cecha> koncesje = ListaKoncesji(listaKategorii, listaCech);
             if (!koncesje.Any())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak koncesji w B2B.");
                return;
            }

            foreach (Cecha cecha in koncesje.Values.ToList())
            {
                List<long> listaIdProduktow = listaWejsciowa.Select(x => x.Id).ToList();
                List<long> idProduktowzCecha = lacznikiCech.Where(x => x.Value.CechaId == cecha.Id && listaIdProduktow.Contains(x.Value.ProduktId)).Select(y => y.Value.ProduktId).ToList();
                if (!idProduktowzCecha.Any())
                {
                    continue;
                }
                foreach (var idProduktu in idProduktowzCecha)
                {
                    Produkt produkt = listaWejsciowa.First(x => x.Id == idProduktu);
                    if(produkt.WymaganaKoncesja == null) produkt.WymaganaKoncesja = new HashSet<long>();
                    produkt.WymaganaKoncesja.Add(cecha.Id);
                }
            }
        }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            Dictionary<int, Atrybut> atrybuty = ApiWywolanie.PobierzAtrybuty();
            if (atrybuty == null || !atrybuty.Any())
            {
                return;
            }

            Atrybut atr = atrybuty.FirstOrDefault(x => x.Value.Id == Atrybut ).Value;

            if (atr == null)
            {
                throw new Exception("Brak wybranego atrybutu dla koncesji");
            }

            List<Cecha> listaCech = ApiWywolanie.PobierzCechy().Select(x=>x.Value).Where(x=>x.AtrybutId == Atrybut).ToList();
            List<KategoriaKlienta> listaKategorii = kategorie.Where(x => x.Grupa.Equals(atr.Nazwa, StringComparison.InvariantCultureIgnoreCase)).ToList();
            Dictionary<string, Cecha> koncesje = ListaKoncesji(listaKategorii, listaCech);

            LogiFormatki.PobierzInstancje.LogujInfo($"Koncesje odczytywane na podstawie atrybuty / grupy kategorii klientów o nazwie: [{atr.Nazwa}].\r\n" 
                                                  + $"Ilosc koncesji w produktach: {koncesje.Count} [{koncesje.Keys.ToList().Join(",")}],\r\n " +
                                                    $"ilosc koncesji w kategoriach klientów: {listaKategorii.Count}  [{listaKategorii.Select(x=> x.Nazwa).ToList().Join(",")}]");

            if (koncesje.IsEmpty() || listaKategorii.IsEmpty())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak koncesji klienta lub produktów.");
                return;
            }

            foreach (KategoriaKlienta kat in listaKategorii)
            {
                //pobranie koncesji z produktu
                var cecha = koncesje.FirstOrDefault(x => x.Key.Equals(kat.Nazwa, StringComparison.InvariantCultureIgnoreCase)).Value;
                if (cecha == null)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo($"Brak koncesji dla produktów - jest tylko dla klientów: {kat.Nazwa}. Pomijam.");
                    continue;
                }

                List<long> idKlientowZKategoria = laczniki.Where(x => x.KategoriaKlientaId == kat.Id).Select(y => y.KlientId).ToList();
                LogiFormatki.PobierzInstancje.LogujInfo($"Dla koncesji: {kat.Id} [{kat.Nazwa}] dopasowano: {idKlientowZKategoria.Count} klientów.");
                Log.Debug($"Dla koncesji: {kat.Id} [{kat.Nazwa}] dopasowano: {idKlientowZKategoria.Count} klientów: {idKlientowZKategoria.Select(x=> x.ToString()).ToList().Join(",")} ");

                if (idKlientowZKategoria.IsEmpty())
                {
                    continue;
                }
                
                foreach (var idKlienta in idKlientowZKategoria)
                {
                    Klient klient;
                    if (!listaWejsciowa.TryGetValue(idKlienta, out klient))
                    {
                        //moze byc brak bo laczniki pobiermay z providera i moga byc kliencie ktorzy juz sa usuniecie przez inne moduly itp.
                      //  LogiFormatki.PobierzInstancje.LogujInfo($"Brak klienta o ID: {idKlienta}. Nie powinno tak być - to oznacza że jest kategoria dla klienta którego nie ma na B2B jeszcze.");
                        continue;
                    }
                    if (klient.Koncesja == null)
                    {
                        klient.Koncesja = new HashSet<long>() {cecha.Id};
                    }
                    else
                    {
                        klient.Koncesja.Add(cecha.Id);
                    }
                }
            }
        }

        private Dictionary<string, Cecha> ListaKoncesji(List<KategoriaKlienta> kategorie, List<Cecha> cechy)
        {
            if (!kategorie.Any())
            {
                Log.Error("Brak kategorii dla koncesji.");
            }
            if (!cechy.Any())
            {
                Log.Error("Brak cech dla koncesji.");
            }

            HashSet<string> nazwyKat = new HashSet<string>( kategorie.Select(x => x.Nazwa) );
            HashSet<string> nazwyCechy = new HashSet<string>( cechy.Select(x => x.Nazwa) );
            IEnumerable<string> wspolne = nazwyCechy.Intersect(nazwyKat, StringComparer.InvariantCultureIgnoreCase);
            Dictionary<string, Cecha> wynik = cechy.Where(x => wspolne.Contains(x.Nazwa, StringComparer.OrdinalIgnoreCase)).ToDictionary(x => x.Nazwa, x => x);
            return wynik;
        }
    }
}
