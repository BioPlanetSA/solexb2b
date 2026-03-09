using System;
using System.Collections.Generic;
using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Model.Web;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
   public class ZerowanieRabatow: SyncModul,IModulKlienci
    {
       public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
         {
            PropertyInfo pole = typeof(Klient).GetProperty(Pole);
            if (pole == null)
            {
                throw new InvalidOperationException("Coś poszło nie tak nie znaleziono pola: "+Pole);
            }
            foreach (Klient k in listaWejsciowa.Values)
            {
                object wartosc = pole.GetValue(k);
                bool usunac = (wartosc ?? "").ToString().PorownajWartosc(Wartosc, Porowanie);
                if (usunac)
                {
                    k.Rabat = 0;
                }
            }
        }
        [FriendlyName("Pole wg ktorego zerujemy rabaty")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }

        [FriendlyName("Wartosc pole ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Wartosc { get; set; }

        [FriendlyName("Sposób porównania ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Wartosc Porowanie { get; set; }
        public override string uwagi
        {
            get { return "Zeruje rabaty klientów na cała ofertę, na podstawie pola klienta. Obecnie obsługiwane tylko porówania równe, różne"; }
        }
    }
}
