using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class KopiowaniePolaDoPola : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Pole, z którego będzie skopiowana wartość")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleZrodlowe { get; set; }

        [FriendlyName("Pole, do którego będzie skopiowana wartość z pola źródłowego")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleDocelowe { get; set; }

        public KopiowaniePolaDoPola()
        {
            PoleZrodlowe = "";
            PoleDocelowe = "";
        }

        public override string Opis
        {
            get { return "Automatyczne kopiowanie wartości jednego pola do drugiego."; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (string.IsNullOrEmpty(PoleZrodlowe) || string.IsNullOrEmpty(PoleDocelowe))
                return;

            List<PropertyInfo> propertisy = typeof(Produkt).Properties().Values.ToList();
            var akcesor = typeof(Produkt).PobierzRefleksja();

            foreach (object produkt in listaWejsciowa)
            {
                var polezrodlowe = propertisy.FirstOrDefault(a => a.Name == PoleZrodlowe);
                var poledocelowe = propertisy.FirstOrDefault(a => a.Name == PoleDocelowe);
                if (polezrodlowe != null && poledocelowe != null)
                {
                    try
                    {
                        object starePole = akcesor[produkt, polezrodlowe.Name];
                        string nowaWartosc = "";
                        if (starePole != null)
                            nowaWartosc = starePole.ToString();

                        akcesor[produkt, poledocelowe.Name] = nowaWartosc;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("błąd przy przetwarzaniu towaru " + ex.Message, ex);
                    }
                }
            }
        }
    }
}
