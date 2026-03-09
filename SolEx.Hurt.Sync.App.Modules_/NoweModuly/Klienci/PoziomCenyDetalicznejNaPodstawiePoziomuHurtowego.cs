using System.Collections.Generic;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    public class PoziomCenyDetalicznejNaPodstawiePoziomuHurtowego : SyncModul, IModulKlienci
    {
        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            foreach (var k in listaWejsciowa)
            {
                if (k.Value.PoziomCenowyId == PoziomHurtowy)
                {
                    k.Value.CenaDetalicznaPoziomID = PoziomDetaliczny;
                }
            }
        }
        [PobieranieSlownika(typeof(SlownikPoziomuCen))]
        [FriendlyName("Poziom ceny hurtowej jaki ma mieć klient")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int PoziomHurtowy { get; set; }
        
        [PobieranieSlownika(typeof(SlownikPoziomuCen))]
        [FriendlyName("Poziom ceny detalicznej jaki ma zostać ustawiony")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int PoziomDetaliczny { get; set; }
        public override string uwagi
        {
            get { return "Ustawia poziom cen detalicznych na podstawie poziomu cen hurtowych"; }
        }
    }
}
