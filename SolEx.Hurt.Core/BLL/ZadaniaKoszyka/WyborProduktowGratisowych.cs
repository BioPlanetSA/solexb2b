using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public abstract class WyborProdoktowBaza : ZadanieCalegoKoszyka, IModulWyboruGratisow
    {
        public IRabatyBll Rabaty = SolexBllCalosc.PobierzInstancje.Rabaty;

        [FriendlyName("CenaKatalogowa - <br />używana dla wyliczenia wartości wykoszystanego limitu (jeżeli pusta to cena wg rabatów klienta)")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? CenaKatalogowa { get; set; }

        [FriendlyName("Cena - <br/>używana dla wyliczenia wartości gratisów na zamówieniu/fakturze")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? Cena { get; set; }

        public IEnumerable<OpisProduktuGratisowego> PobierzProdukty(IKlient klient)
        {
            HashSet<long> produktyids = PobierzProduktyID();

            IList<ProduktKlienta> produkty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktKlienta>(klient, x => produktyids.Contains(x.Id));
            List<OpisProduktuGratisowego> lista = new List<OpisProduktuGratisowego>();
            foreach (var item in produkty)
            {
                lista.Add(new OpisProduktuGratisowego(item.Id, PobierzCeneGratisu(item, CenaKatalogowa, Cena, klient)));
            }
            return lista;
            //return PobierzProduktyID().Select(x => new OpisProduktuGratisowego(x, PobierzCeneGratisu(x, CenaKatalogowa, Cena, klient)));
        }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            return true;
        }

        protected abstract HashSet<long> PobierzProduktyID();

        private IFlatCenyBLL PobierzCeneGratisu(IProduktKlienta produkt, decimal? netto, decimal? nettorzeczywista, IKlient aktualnyKlient)
        {
       //     var rzeczywistacena = Rabaty.WyliczCeneDlaKlientaZalogowanego(aktualnyKlient, produkt);
            decimal hurtowa = nettorzeczywista.HasValue ? nettorzeczywista.Value : produkt.FlatCeny.CenaDetalicznaNetto.Wartosc;
            decimal sprzedazy = netto.HasValue ? netto.Value : produkt.FlatCeny.CenaNetto.Wartosc;
            FlatCeny cenaRabat = new FlatCeny(aktualnyKlient.Id, produkt.Id, sprzedazy, 0, produkt.FlatCeny.WalutaId, hurtowa);
            var a = new FlatCenyBLL(cenaRabat, produkt, aktualnyKlient);
            return a;
        }
    }

    
    public class WyborProduktowGratisowych : WyborProdoktowBaza
    {
        public override string Opis
        {
            get { return "Wybór produtków gratisowych."; }
        }

        [FriendlyName("Produkty gratisowe")]
        [PobieranieSlownika(typeof(SlownikProduktow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Produkty { get; set; }

        protected override HashSet<long> PobierzProduktyID()
        {
            //TODO: czy my dalej tego potrzebuejmy - juz sa slowniki przeciez
            return new HashSet<long>(Produkty.Select(long.Parse));
        }
    }
}