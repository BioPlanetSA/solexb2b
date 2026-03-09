using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty
{
    [FriendlyName("Ukryj lub pokaz produkty wybranym klientom", FriendlyOpis = "Moduł, który ukrywa bądź pokazuje towary dla wybranych klientów. Moduł działa na produktach które są już na platformie, moduł nie decyduję które produkty mają być synchronizowane.")]
    public class WidocznoscProduktowPoCesze : SyncModul, IModulRabaty
    {
        [FriendlyName("Cecha towaru z ERP")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Cecha { get; set; }

        public WidocznoscProduktowPoCesze()
        {
            TypWidocznosci = KatalogKlientaTypy.Dostepne;
        }
        public override string uwagi => "Wymaga licencji katalog_klienta";

        [FriendlyName("Typ widoczności towarów ustawiany dla każdego towar z określoną cechą")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KatalogKlientaTypy TypWidocznosci { get; set; }
        
        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryte, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> klienci, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechyNaPlatformie, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            HashSet<int> CechyId = new HashSet<int>( Cecha.Select(int.Parse) );
            if (Cecha == null || (Cecha.Count == 1 && Cecha[0] == ""))
            {
                Log.Error("Nie wybrano cechy, moduł zakończy działanie!");
                return;
            }

            if (produktyUkryte == null)
                produktyUkryte = new List<ProduktUkryty>();

            foreach (var c in CechyId)
            {
                var cecha = cechyNaPlatformie.FirstOrDefault(a => a.Id == c);
                if (cecha != null)
                {
                    ProduktUkryty pu = new ProduktUkryty {Tryb = TypWidocznosci, CechaProduktuId = cecha.Id};
                    produktyUkryte.Add(pu);
                }
            }
        }
    }
}
