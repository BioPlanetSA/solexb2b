using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow
{
    public class KategorieWalutoweZPoziomuCenowego : KategorieWalutoweBase, IModulKategorieKlientow
    {
        public override string Opis
        {
            get { return "Moduł tworzy kategorie klientów na podstawie poziomu cenowego klienta"; }
        }

        public KategorieWalutoweZPoziomuCenowego()
        {
            NazwaPoziomuZamiastWaluty = false;
        }

        [FriendlyName("Czy zamiast waluty ma być pobrana nazwa poziomu cenowego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool NazwaPoziomuZamiastWaluty { get; set; }

        public void Przetworz(ref List<KategoriaKlienta> kategorie, ref List<KlientKategoriaKlienta> laczniki)
        {
            IEnumerable<Klient> wszyscy = ApiWywolanie.PobierzKlientow().Values;
            Dictionary<int, PoziomCenowy> poziomycen = ApiWywolanie.PobierzPoziomyCen();
            var waluty = ApiWywolanie.PobierzWaluty();
            foreach (Klient k in wszyscy)
            {
                if (k.Id <= 0 ) continue;
                string waluta = DomyslnaWaluta;
                KeyValuePair<int, PoziomCenowy> poziomKlienta = poziomycen.FirstOrDefault(a => a.Key == k.PoziomCenowyId);
                if (poziomKlienta.Value != null )
                {

                    waluta = (NazwaPoziomuZamiastWaluty) ? poziomKlienta.Value.Nazwa : poziomKlienta.Value.WalutaId.HasValue? waluty[(long)poziomKlienta.Value.WalutaId].WalutaErp: DomyslnaWaluta;
                }

                DodajKategorie(kategorie, laczniki, k, grupa, waluta);
            }
        }
    }
}