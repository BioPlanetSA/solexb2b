using System;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty
{
    public class UkrywanieProduktowBezRabatow : SyncModul, Model.Interfaces.SyncModuly.IModulRabaty
    {

        [FriendlyName("Nazwa poziomu ceny")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoziomCeny { get; set; }

        public override string uwagi
        {
            get { return ""; }
        }

        public UkrywanieProduktowBezRabatow() 
        {
            PoziomCeny = string.Empty;
        }

        public override string Opis
        {
            get { return "Moduł ukrywający produkty dla klienta jeśli klient nie ma dla nich rabatu."; }
        }

        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            if (string.IsNullOrEmpty(PoziomCeny))
                return;

            var cenypoziomy = ApiWywolanie.PobierzPoziomyCenProduktow();

            var wybranypoziom = ceny.FirstOrDefault(a => a.Nazwa.ToLower() == PoziomCeny.ToLower());

            if (wybranypoziom != null)
            {
                foreach (int klientID in kliencib2B.Keys)
                {
                    //CustomerPriceSearchCriteria criteria = new CustomerPriceSearchCriteria();
                    //criteria.klient_id.Add(klientID);

                    List<FlatCeny> flatceny = ApiWywolanie.PobierzCenyKlientow(new HashSet<long> {klientID});
                    if (!flatceny.Any())
                    {
                        throw new Exception("Brak cen po rabatach z platformy");
                    }
                    var cenyklienta = flatceny;//.Where(a => a.klient_id == klientID).ToList();

                    foreach (Produkt produkt in produkty.Values)
                    {
                        var cena = cenyklienta.FirstOrDefault(a => a.ProduktId == produkt.Id);
                        if (cena != null)
                        {
                            var cenatowaru =
                                cenypoziomy.Values.FirstOrDefault(
                                    a => a.PoziomId == wybranypoziom.Id && a.ProduktId == produkt.Id);

                            if (cenatowaru != null && cenatowaru.Netto == cena.CenaNetto)
                            {
                                DodajProduktDoUkrytych(klientID, produkt.Id, ref produktyUkryteNaB2B);
                            }
                        }
                        else DodajProduktDoUkrytych(klientID, produkt.Id, ref produktyUkryteNaB2B);
                    }
                }
            }
        }

         private void DodajProduktDoUkrytych(int klientid, long idtowaru, ref List<ProduktUkryty> listaproduktowukrytych)
        {
            ProduktUkryty pu = new ProduktUkryty {KlientZrodloId = klientid, ProduktZrodloId = idtowaru, Tryb = KatalogKlientaTypy.Wykluczenia};
             listaproduktowukrytych.Add(pu);
        }
    }
}
