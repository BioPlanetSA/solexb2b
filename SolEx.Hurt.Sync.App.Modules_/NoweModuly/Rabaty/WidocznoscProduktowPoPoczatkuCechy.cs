using System;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
     [Obsolete("zamiaste tego użyj modułu WidocznoscProduktowPoCesze")]
    public class WidocznoscProduktowPoPoczatkuCechy : SyncModul, Model.Interfaces.SyncModuly.IModulRabaty
    {
        [FriendlyName("Początek cechy towaru z ERP")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoczatekCechy { get; set; }

        public WidocznoscProduktowPoPoczatkuCechy()
        {
            PoczatekCechy = string.Empty;
            TypWidocznosci = KatalogKlientaTypy.Dostepne;
        }

        public override string uwagi
        {
            get { return "Wymaga licencji katalog_klienta"; }
        }

        [FriendlyName("Typ widoczności towarów ustawiany dla każdego towar z określoną cechą")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KatalogKlientaTypy TypWidocznosci { get; set; }

        public override string Opis
        {
            get { return "Moduł, który ukrywa bądź pokazuje towary, które mają określoną cechę"; }
        }

        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryte, ref Dictionary<long,Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> klienci, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechyNaPlatformie, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            if (string.IsNullOrEmpty(PoczatekCechy))
            {
                Log.Error("Początek cechy jest pusty, moduł zakończy działanie!");
                return;
            }

            PoczatekCechy = PoczatekCechy.ToLower();

            if (produktyUkryte == null)
                produktyUkryte = new List<ProduktUkryty>();

                Cecha cecha =
                    cechyNaPlatformie.AsParallel()
                        .FirstOrDefault(a => a.Symbol.StartsWith(PoczatekCechy));
                if (cecha != null)
                {
                    ProduktUkryty pu = new ProduktUkryty();
                    pu.Tryb = TypWidocznosci;
                    pu.CechaProduktuId = cecha.Id;
                        produktyUkryte.Add(pu);
                }
        }
    }
}

