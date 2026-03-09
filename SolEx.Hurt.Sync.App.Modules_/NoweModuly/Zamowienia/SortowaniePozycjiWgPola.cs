using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.Text;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{

    [FriendlyName("Sortowanie pozycji wg Pola zamówienia", FriendlyOpis = "Sortuje pozycje na zamówieniach wg wybranego pola")]
    public class SortowaniePozycjiWgPola : SyncModul, Model.Interfaces.SyncModuly.IModulZamowienia
    {

        public SortowaniePozycjiWgPola()
        {
            KierunekSortowania = Kierunek.Rosnąco;
        }

        public override string uwagi => "";

        [FriendlyName("Pole produktu wg którego będą sortowane pozycje w zamówieniu")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }

        [FriendlyName("Określa czy pozycje w zamówieniu będą posortowane rosnąco czy malejąco")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Kierunek KierunekSortowania { get; set; }

        public enum Kierunek
        {
            Rosnąco, Malejąco
        }

        public void Przetworz(ZamowienieSynchronizacja listaWejsciowa, ref List<ZamowienieSynchronizacja> wszystkie, ISyncProvider provider, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> laczniki, Dictionary<long, Produkt> produktyB2B, List<Cecha> cechy, List<ProduktCecha> cechyProduktyNaPlatfromie)
        {
            if (string.IsNullOrEmpty(Pole))
            {
                Log.Error("Ustawienie Pole jest puste, moduł przerwie działanie!");
                return;
            }

            //Log.Debug($"lista wejsciowa: {listaWejsciowa.ToJson()}");
            System.Reflection.PropertyInfo prop = typeof(Produkt).GetProperty(Pole);
            if (prop == null)
            {
                throw new Exception($"Brak pola: {this.Pole} na obiekcie produktu");
            }

            Dictionary<long, object> produkty = produktyB2B.Values.ToDictionary(x => x.Id, x => prop.GetValue(x));

            Dictionary<int, object> listaDoSortowania = listaWejsciowa.pozycje.ToDictionary(x => x.Id, x => produkty[x.ProduktIdBazowy]);

            int[] listaPosortowana = listaDoSortowania.OrderByWithDirection(x => x.Value, KierunekSortowania == Kierunek.Malejąco).Select(x=>x.Key).ToArray();

            Dictionary<int, ZamowienieProdukt> slownikPozycji = listaWejsciowa.pozycje.ToDictionary(x => x.Id, x => x);
            listaWejsciowa.pozycje = listaPosortowana.Select(x => slownikPozycji[x]).ToList();
          
            // Log.Debug($"lista wyjściowa: {listaWejsciowa.ToJson()}");
        }
    }
}
