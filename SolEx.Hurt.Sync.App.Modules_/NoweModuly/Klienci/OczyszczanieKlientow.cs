using System.Reflection;
using System.Text.RegularExpressions;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Web;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    public class OczyszczanieKlientow : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
       
        public OczyszczanieKlientow()
        {
            Regex = @"[^0-9a-zA-Z-,ążśźćńłęóŻŹŃĄŚŁĘÓ\-&\+.]+";
            ZnakDoPodmiany = "-";
            Pola = new List<string>();
        }

        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Wyrażenie regularne do oczyszczania.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Regex { get; set; }

        [FriendlyName("Znak, na który będą podmieniane niedozwolone znaki. (domyślnie spacja)")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string ZnakDoPodmiany { get; set; }

        [FriendlyName("Lista pól, które mają zostać oczyszczone")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

        public override string Opis
        {
            get { return "Moduł, który usuwa znaki specjalne z wybranych pól klienta."; }
        }
       
        private string Oczysc(string tekst)
        {
            Regex myRegex = new Regex(Regex);
            return myRegex.Replace(tekst, ZnakDoPodmiany);
        }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            PropertyInfo[] propertisy = typeof(Klient).GetProperties();

            foreach (Klient klient in listaWejsciowa.Values)
            {
                foreach (var p in propertisy)
                {
                    if (Pola.Contains(p.Name))
                    {
                        try
                        {
                            string oczyszczonanazwa = Oczysc(p.GetValue(klient, null).ToString());
                            p.SetValue(klient, oczyszczonanazwa, null);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("błąd przy przetwarzaniu nazwy klienta "+ex.Message, ex);
                        }
                    }
                }
            }
        }
    }
}
