using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    [FriendlyName("Katalog klienta na podstawie ofert (dokumentów)",FriendlyOpis = "Moduł, tworzy katalog klienta na podstawie ofert (dokumentów z wybranym statusem)")]
    public class KatalogKlientaNaPostawieOfert : BazaModulyDokumentyStatus, IModulRabaty
    {
        public KatalogKlientaNaPostawieOfert()
        {
            TypWidocznosci = KatalogKlientaTypy.MojKatalog;
        }

        [FriendlyName("Typ widoczności towarów dla produktów z oferty", FriendlyOpis = "Mój katalog oznacza mój katalog.<br/>" +
                    "Dostepny - oznacza odkrycue produktow dla klienta (np. jesli domyslnie wsystkie produkty sa ukryte).<br/>" +
                    "Wykluczenie - wykluczenie z oferty produktu dla klienta (jeśli np. domyślnie wszystkie produkty sa odkryte).<br/>" +
                    "Domyślną widocznosc produktów można ustawić modulem synchronziacji OfertaKlienta")]

        [WidoczneListaAdmin(false, false, true, false)]
        public KatalogKlientaTypy TypWidocznosci { get; set; }
 
        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryte, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechyNaPlatformie, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            if (kliencib2B == null || kliencib2B.IsEmpty())
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"Brak klientów na B2B do przetowrzenia.");
                return;
            }

            if (produktyUkryte == null)
            {
                produktyUkryte = new List<ProduktUkryty>();
            }

            //birzemy tyko klientow co sa na b2b juz
            long[] klienciDoPobrania = kliencib2B.Values.Where(x => x.Aktywny).Select(x => x.Id).ToArray();
            if (klienciDoPobrania.IsEmpty())
            {
                LogiFormatki.PobierzInstancje.LogujInfo($"Brak klientów aktywnych na B2B do przetowrzenia.");
            }

            Dictionary<long, List<HistoriaDokumentuProdukt>> pobrane = PasujaceDokumenty(klienciDoPobrania);

            LogiFormatki.PobierzInstancje.LogujInfo($"Pobrane pozycje dla klientów do przetworzenia z ERP: {pobrane.Count}. Produktów ukrytych obecnie: {produktyUkryte.Count}");

            int liczbaWPisow = produktyUkryte.Count;
            foreach (var d in pobrane)
            {
                foreach (var p in d.Value)
                {                   
                    ProduktUkryty pu=new ProduktUkryty();
                    pu.KlientZrodloId = d.Key;
                    pu.ProduktZrodloId = p.ProduktIdBazowy;
                    pu.Tryb = TypWidocznosci;
                    produktyUkryte.Add(pu);                    
                }
            }
            LogiFormatki.PobierzInstancje.LogujInfo($"Stworzono wpisów: {produktyUkryte.Count - liczbaWPisow}.");
        }
    }

}

