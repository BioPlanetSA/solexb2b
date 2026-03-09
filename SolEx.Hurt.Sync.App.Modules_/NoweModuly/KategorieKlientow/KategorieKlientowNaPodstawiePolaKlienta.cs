using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow
{
    public class KategorieKlientowNaPodstawiePolaKlienta : Rozne.KategorieKlientow, IModulKategorieKlientow//, IModulKlienci
    {
        public override string uwagi
        {
            get { return "Tworzy kategorie klientów na podstawie pola własnego"; }
        }
        public KategorieKlientowNaPodstawiePolaKlienta()
        {
            NazwaKategoriiPuste = "KlienciNiewypelnionaGrupa";
        }
        
        [FriendlyName("Pole, z którego tworzymy kategorie")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleZrodlowe { get; set; }

        [FriendlyName("Nazwa tworzonej grupy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaGrupy { get; set; }
        
        [FriendlyName("Przypisać klienta do grupy jeśli ma puste wybrane pole")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool PrzypisacPuste { get; set; }
        
        [FriendlyName("Nazwa kategorii dla klientow ktorzy maja puste pole")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaKategoriiPuste { get; set; }

        public virtual IEnumerable<Klient> WszyscyKlienci()
        {
            return ApiWywolanie.PobierzKlientow().Values;
        }

        public void Przetworz(ref List<KategoriaKlienta> kategorie, ref List<KlientKategoriaKlienta> laczniki)
        {
            PropertyInfo[] propertisy = typeof (Klient).GetProperties();
            foreach (Klient klient in WszyscyKlienci())
            {
                if (klient.Id < 0)
                {
                    continue;//nie ruszamy klientów ręcznie dodanych
                }
                var polezrodlowe = propertisy.First(a => a.Name == PoleZrodlowe);
                string starePole = (polezrodlowe.GetValue(klient, null) ?? "").ToString();

                if (!PrzypisacPuste && string.IsNullOrEmpty(starePole))
                {
                    continue;

                }
                string kategoria = starePole;
                if (string.IsNullOrEmpty(kategoria))
                {
                    kategoria = NazwaKategoriiPuste;
                }
                if (string.IsNullOrEmpty(kategoria))
                {
                    throw new Exception("Brak nazwy kategorii");
                }
                DodajKategorie(kategorie, laczniki, klient, NazwaGrupy, kategoria);
            }
        }

        //public void Przetworz(ref Dictionary<klienci, List<kategorie_klientow>> listaWejsciowa, Dictionary<int, produkty> produktyB2B, ref List<Model.Web.ParemetryLiczeniaIlosci> ilosci)
        //{
        //    List<kategorie_klientow> kategorie = new List<kategorie_klientow>();
        //    List<klienci_kategorie> laczniki = new List<klienci_kategorie>();

        //    Przetworz(ref kategorie, ref laczniki);

        //    foreach (var klient in listaWejsciowa)
        //    {
        //        var kategorieKlienta = laczniki.AsParallel().Where(a => a.klient_id == klient.Key.klient_id).Select(a=> a.kategoria_klientow_id);

        //        foreach (int klienci_Kategorie in kategorieKlienta)
        //        {
        //            kategorie_klientow kategoria = kategorie.AsParallel().FirstOrDefault(a => a.Id == klienci_Kategorie);
        //            if(kategoria != null)
        //                klient.Value.Add(kategoria);
        //        }
        //    }
        //}
    }
}
