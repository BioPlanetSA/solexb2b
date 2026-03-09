using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces.Sync;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    public class KopiowaniePolaDoPola : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Pole, z którego będzie skopiowana wartość")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleZrodlowe { get; set; }

        [FriendlyName("Pole, do którego będzie skopiowana wartość z pola źródłowego")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
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

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            if (string.IsNullOrEmpty(PoleZrodlowe) || string.IsNullOrEmpty(PoleDocelowe))
                return;

            PropertyInfo[] propertisy = typeof(Klient).GetProperties();

            foreach (Klient klient in listaWejsciowa.Values)
            {
                var polezrodlowe = propertisy.FirstOrDefault(a => a.Name == PoleZrodlowe);
                var poledocelowe = propertisy.FirstOrDefault(a => a.Name == PoleDocelowe);
                if (polezrodlowe != null && poledocelowe != null)
                {
                    try
                    {
                        object starePole = polezrodlowe.GetValue(klient, null);
                        string nowaWartosc = "";
                        if (starePole != null)
                            nowaWartosc = starePole.ToString();

                        poledocelowe.SetValue(klient, nowaWartosc, null);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("błąd przy przetwarzaniu klienta " + ex.Message, ex);
                    }
                }
            }
        }
    }
}
