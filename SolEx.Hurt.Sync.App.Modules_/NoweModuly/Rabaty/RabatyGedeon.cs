using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty
{
    public class RabatyGedeon : SyncModul, Model.Interfaces.SyncModuly.IModulRabaty
    {
        [FriendlyName("Początek cechy rabatu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoczatekCechy { get; set; }

        public RabatyGedeon()
        {
            PoczatekCechy = string.Empty;
        }

        public override string uwagi
        {
            get { return ""; }
        }

        public override string Opis
        {
            get { return "Moduł dla Gedeona liczący rabaty."; }
        }

        public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B,
            ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B, Dictionary<long, Produkt> produkty,
            List<PoziomCenowy> ceny, List<Cecha> cechy, Dictionary<long, ProduktCecha> cechyProdukty,
            Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie,
            ref IDictionary<int, KategoriaKlienta> kategorieKlientow,
            ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {

            //żeby modul obsługiwał liczenie rabatów dla towarów ukrytych należy odkomentować resztę kodu i zakomentować dodawanie id cechy
            rabatyNaB2B = new List<Rabat>();
            // var towaryid = ApiWywolanie.PobierzProduktyUkryte(new ProduktyUkryteSearchCriteria());
            foreach (Klient k in kliencib2B.Values)
            {
                List<Cecha> cechypasujace = cechy.Where(a => a.Nazwa.StartsWith(PoczatekCechy)).ToList();
                List<KlientKategoriaKlienta> kategorieklientapowiazanie =
                    klienciKategorie.Values.Where(a => a.KlientId == k.Id).ToList();
                List<KategoriaKlienta> kategorieklienta =
                    kategorieKlientow.Values.Where(
                        a => kategorieklientapowiazanie.Any(b => b.KategoriaKlientaId == a.Id)).ToList();
                kategorieklienta = kategorieklienta.Where(a => cechypasujace.Any(b => b.Nazwa == a.Grupa)).ToList();

                foreach (KategoriaKlienta kategoriaklienta in kategorieklienta)
                {
                    decimal rabat;
                    TextHelper.PobierzInstancje.SprobojSparsowac(kategoriaklienta.Nazwa.Replace("%", ""), out rabat);

                    if (rabat == 0)
                        continue;

                    Cecha c = cechypasujace.FirstOrDefault(a => a.Nazwa == kategoriaklienta.Grupa);
                    Rabat r = new Rabat();
                    r.Aktywny = true;
                    r.CechaId = c.Id;
                    r.KlientId = k.Id;


                    r.WalutaId = k.WalutaId;

                    //  r.produkt_id = produktyklienta[j]; //jeśli jest cecha to nie trzeba podawać id towaru
                    r.Wartosc1 = r.Wartosc2 = rabat;
                    r.Wartosc3 = rabat;
                    r.Wartosc4 = rabat;
                    r.Wartosc5 = rabat;
                    r.TypWartosci = Model.Enums.RabatSposob.Procentowy;
                    r.TypRabatu = Model.Enums.RabatTyp.Zaawansowany;
                    rabatyNaB2B.Add(r);
                }
                //}
            }
        }
    }
}
