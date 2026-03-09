using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Text;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty
{
   [FriendlyName("Usuwa rabaty pobrane z epr, na podstawie pola klienta (pomocne gdy chcemy mieć np. część klientów rabatowych wg. promocji a innych wg. stałych rabatów, lub np. usuwać rabaty dla klientów nie płacących w terminie)")]
   public class UsuwanieRabatow : SyncModul, IModulRabaty
    {
       public void Przetworz(ref List<Rabat> rabatyNaB2B, ref List<ProduktUkryty> produktyUkryteNaB2B, ref Dictionary<long, Konfekcje> konfekcjaNaB2B, IDictionary<long, Klient> kliencib2B, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, Dictionary<long, ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, ref IDictionary<int, KategoriaKlienta> kategorieKlientow, ref IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            PropertyInfo pole = typeof(Klient).GetProperty(Pole);
            if (pole == null)
            {
                throw new InvalidOperationException("Coś poszło nie tak nie znaleziono pola: "+Pole);
            }

            HashSet<long> idKlientowZUsunietymiRabatami = new HashSet<long>();

            foreach (Klient k in kliencib2B.Values)
            {
                object wartosc = pole.GetValue(k);
                bool usunac = (wartosc ?? "").ToString().PorownajWartosc(Wartosc, Porowanie);
                if (usunac)
                {
                    idKlientowZUsunietymiRabatami.Add(k.Id);                   
                }
            }

            if (idKlientowZUsunietymiRabatami.Any())
            {
                Log.DebugFormat("Usuwanie rabatów dla klientów id: {0}", idKlientowZUsunietymiRabatami.ToCsv());
                rabatyNaB2B.RemoveAll(x => x.KlientId.HasValue && idKlientowZUsunietymiRabatami.Contains(x.KlientId.Value));
            }
            else
            {
                Log.DebugFormat("Brak klientów do usuwania rabatów.");
            }
        }

        [FriendlyName("Pole wg ktorego usuwawamy rabaty")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }

        [FriendlyName("Wartosc pole ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Wartosc { get; set; }

        [FriendlyName("Sposób porównania ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Wartosc Porowanie { get; set; }
    }
}
