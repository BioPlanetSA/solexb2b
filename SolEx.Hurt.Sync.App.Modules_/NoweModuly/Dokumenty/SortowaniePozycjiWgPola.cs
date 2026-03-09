using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    [FriendlyName("Sortowanie pozycji wg Pola dokumenty", FriendlyOpis = "Sortuje pozycje na dokumentach wg wybranego pola. Przed włączeniem modułu należy usunąć dokumenty na platformie")]
    public class SortowaniePozycjiWgPola : SyncModul, Model.Interfaces.SyncModuly.IModulDokumenty
    {
        public SortowaniePozycjiWgPola()
        {
            KierunekSortowania = Kierunek.Rosnąco;
        }

        public void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy, Dictionary<int, long> hashe,ref List<Klient> klienci )
        {

            if (string.IsNullOrEmpty(Pole))
            {
                Log.Error("Ustawienie Pole jest puste, moduł przerwie działanie!");
                return;
            }
            System.Reflection.PropertyInfo prop = typeof (HistoriaDokumentuProdukt).GetProperty(Pole);
            ConcurrentDictionary<HistoriaDokumentu, List <HistoriaDokumentuProdukt >> wynik = new ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();
            Parallel.ForEach(listaWejsciowa, dok =>
            {
                wynik.TryAdd(dok.Key, dok.Value.OrderByWithDirection(x => prop.GetValue(x, null), KierunekSortowania == Kierunek.Malejąco).ToList());
            });
            listaWejsciowa = wynik;
        }

        public override string uwagi => "Przed włączeniem modułu należy usunąć dokumenty na platformie";

        [FriendlyName("Pole wg którego będą sortowane pozycje w dokumencie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(HistoriaDokumentuProdukt))]

        public virtual string Pole { get; set; }

        [FriendlyName("Określa czy pozycje w dokumencie będą posortowane rosnąco czy malejąco")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Kierunek KierunekSortowania { get; set; }

        public enum Kierunek
        {
            Rosnąco, Malejąco
        }
    }
}
