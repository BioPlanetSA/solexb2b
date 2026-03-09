using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty
{
    [FriendlyName("Rabaty poziom ceny",FriendlyOpis = "Ustawia cene produktu na cene z określonego poziomu cenowego, jeśli cena na tym poziomie jest różna od zero. Można wykorzystać np. jako promocja - sztywna cena, cena przekreślona<br/>" +
                                                      "Wymaga włączenia modułu: KategorieWalutowe")]
    public class RabatyPoziomCeny : SyncModul, IModulRabaty, IModulProdukty
    {
        public override string uwagi => $"Wymaga włączenia modułu {typeof(KategorieWalutowe).Name}";

        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            Wykonaj(ceny, kategorieKlientow, produkty, ref rabatyNaB2B);
        }
        private HashSet<long> Wykonaj(List<PoziomCenowy> ceny,  IDictionary<int, KategoriaKlienta> kategorieKlientow, Dictionary<long, Produkt> produkty, ref List<Rabat> rabatyNaB2B)
        {
            Dictionary<long, Waluta> waluty = ApiWywolanie.PobierzWaluty();
           HashSet<long> produktyKtorymDodanoCene=new HashSet<long>();
           PoziomCenowy pc = new PoziomCenowy();
           if (ListaPoziomowCenowych != 0)
           {
               pc = ceny.First(p => p.Id == ListaPoziomowCenowych);
           }
           else
           {
               pc = ceny.First(p => p.Nazwa == NazwaPoziomuCenyZERP);
           }
            //poziomy_cen pc = ceny.First(p => p.nazwa == NazwaPoziomuCenyZERP);
            int idPoziomu = pc.Id;
            List<CenaPoziomu> cenypoziomy = ApiWywolanie.PobierzPoziomyCenProduktow().Values.Where(x=>x.PoziomId==idPoziomu).ToList();
            Waluta waluta = null;
            if (pc.WalutaId.HasValue)
            {
                waluta = waluty[pc.WalutaId.Value];
            }
            var kategoria = kategorieKlientow.Values.FirstOrDefault(x => x.Grupa.Equals("waluta", StringComparison.InvariantCultureIgnoreCase) && x.Nazwa.Equals(waluta.WalutaErp, StringComparison.InvariantCultureIgnoreCase));
            if (kategoria == null)
            {
                throw new InvalidOperationException("Brak odpowiedniej kategorii walutowej, włącz moduł lub stwórz je ręcznie");
            }
            foreach (var produkt in produkty)
            {
                var cp = cenypoziomy.FirstOrDefault(p => p.ProduktId == produkt.Key);
                if (cp == null || cp.Netto <= 0) continue;
                produktyKtorymDodanoCene.Add(produkt.Key);
                Rabat tmp = new Rabat {TypRabatu = RabatTyp.Promocja, TypWartosci = RabatSposob.StalaCena, Aktywny = true, ProduktId = produkt.Key, KategoriaKlientowId = kategoria.Id};
                tmp.Wartosc1 = tmp.Wartosc2 = tmp.Wartosc3 = tmp.Wartosc4 = tmp.Wartosc5 = cp.Netto;
                rabatyNaB2B.Add(tmp);
            }
           return produktyKtorymDodanoCene;
        }

        [FriendlyName("Nazwa poziomu cen z ERP z którego będą brane ceny")]
        [Obsolete("Powy wycofane zalecamy korzystanie z pola ListaPoziomowCenowych")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaPoziomuCenyZERP { get; set; }
        
        [FriendlyName("Poziom cen z którego będą brane ceny")]
        [PobieranieSlownika(typeof(SlownikPoziomuCen))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int ListaPoziomowCenowych { get; set; }
       

        [FriendlyName("Czy przypisywać cechę produktom z cenami na danym poziomie cen")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool PrzypisywacCeche { get; set; }
        
        [FriendlyName("Symbol cechy do przypisania, cecha musi istnieć w systemie ERP")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SymbolCechy { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (PrzypisywacCeche)
            {
             //   Dictionary<int, Cecha> cechy = ApiWywolanie.PobierzCechy();
                Cecha c = cechy.FirstOrDefault(x => x.Symbol == SymbolCechy);
                if (c == null)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo("Brak cechy o symbolu {0} do przypisania. Moduł {1} kończy działanie", SymbolCechy, GetType().Name);
                    return;
                }
                var ceny = ApiWywolanie.PobierzPoziomyCen().Values.ToList();
                var kategorieKlientow = ApiWywolanie.PobierzKategorieKlientow();
                List<Rabat> rabaty=new List<Rabat>();
                var nowe = Wykonaj(ceny, kategorieKlientow, listaWejsciowa.ToDictionary(x => x.Id, x => x), ref rabaty);
                foreach (int p in nowe)
                {
                    if (lacznikiKategorii.All(x => x.ProduktId != p))
                    {
                        continue;
                    }
                    var cp = new ProduktCecha {ProduktId = p, CechaId = c.Id};
                    if (!lacznikiCech.ContainsKey(cp.Id) )
                    {
                        lacznikiCech.Add(cp.Id,cp);
                    }
                }
            }
        }
    }
}
