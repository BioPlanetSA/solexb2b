using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class StanMinNaPodstawieCeny : SyncModul, SolEx.Hurt.Model.Interfaces.SyncModuly.IModulProdukty
    {
        [FriendlyName("Dla produktów o cenie netto mniejszej niż")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Cena { get; set; }

        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Wpisz stan minimalny")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string StanMinimalny { get; set; }

        [FriendlyName("Nie wpisuj stanu jeśli produkt ma stan większy od 0")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool NieWpisujStanuJesliWiekszyOdZera { get; set; }

        public StanMinNaPodstawieCeny()
        {
            Cena = string.Empty;
            StanMinimalny = string.Empty;
            NieWpisujStanuJesliWiekszyOdZera = true;
        }

        public override string Opis
        {
            get { return "Moduł wyliczający stan minimalny na podstawie ceny"; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            decimal cena = 0;
            TextHelper.PobierzInstancje.SprobojSparsowac(Cena, out cena);
            decimal stanminimalny = 0;
            TextHelper.PobierzInstancje.SprobojSparsowac(StanMinimalny, out stanminimalny);
            //na wypadek jak by ktoś "kreatywny" wpisał ujemną cenę...
            if (cena <= 0)
                return;



            var ceny = ApiWywolanie.PobierzPoziomyCenProduktow();

            List<ProduktStan> stany = new List<ProduktStan>();
            var magazyny = ApiWywolanie.PobierzMagazyny();
            foreach (Magazyn mag in magazyny)
            {
                stany.AddRange(ApiWywolanie.PobierzStanyProduktow(mag));
            }

            foreach (Produkt produkt in listaWejsciowa)
            {
                var stan = stany.Where(a => a.ProduktId == produkt.Id).Sum(a => a.Stan);
                var cenaProduktu = ceny.Values.FirstOrDefault(a => a.ProduktId == produkt.Id);
                decimal tmp;
                TextHelper.PobierzInstancje.SprobojSparsowac(Cena, out tmp);
                if (cenaProduktu != null && cenaProduktu.Netto < tmp)
                {
                    if (stan > 0 && NieWpisujStanuJesliWiekszyOdZera)
                    {
                        //to nie robimy nic
                    }
                    else produkt.StanMin = stanminimalny;
                }
            }
        }
    }
}
