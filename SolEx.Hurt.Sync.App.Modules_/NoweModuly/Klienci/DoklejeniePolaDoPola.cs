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
    [FriendlyName("Połącz pola",FriendlyOpis = "Automatyczne doklejanie wartości jednego pola do drugiego.")]
    public class DoklejeniePolaDoPola : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {
     
        [FriendlyName("Pole źródłowe, z którego będzie doklejona wartość do pola docelowego")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleZrodlowe { get; set; }

        [FriendlyName("Pole docelowe, do którego na końcu będzie doklejona wartość z pola źródłowego")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleDocelowe { get; set; }

        [Niewymagane]
        [FriendlyName("Opcjonalny separator oddzielający wartości z obu pól")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }

        public DoklejeniePolaDoPola()
        {
            PoleZrodlowe = string.Empty;
            PoleDocelowe = string.Empty; 
            Separator = string.Empty;
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
                        object starePoleZrodlowe = polezrodlowe.GetValue(klient, null);
                        object starePoleDocelowe = poledocelowe.GetValue(klient, null);
                        
                        if (starePoleZrodlowe != null)
                        {
                            string nowaWartosc = string.Format("{0}{1}{2}", starePoleDocelowe == null ? "" : starePoleDocelowe.ToString(), Separator,
                                starePoleZrodlowe.ToString());
                            poledocelowe.SetValue(klient, nowaWartosc, null);
                        }
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
