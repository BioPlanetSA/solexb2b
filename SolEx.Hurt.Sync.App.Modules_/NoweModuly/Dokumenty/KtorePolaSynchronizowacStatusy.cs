using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    [SynchronizowanePola(typeof(StatusZamowienia))]
    [ModulStandardowy]
    public class KtorePolaSynchronizowacStatusy : KtorePolaSynchonizowacBaza, IModulDokumenty, IModulPola
    {

        public KtorePolaSynchronizowacStatusy()
        {
            PolaAtrybutow = new List<string>();
            PolaAtrybutow.Add("Nazwa");
            PolaAtrybutow.Add("Widoczny");
            PolaAtrybutow.Add("Kolor");
            PolaAtrybutow.Add("Importowac");
        }

        [FriendlyName("Wybierz pola które będziesz edytował w adminie B2B. Pola nie wybrane będa nadpisywane wartościami z ERP (są oznaczone na zielono w adminie)")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(StatusZamowienia))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> PolaAtrybutow { get; set; }

        public List<string> PobierzDostepnePola()
        {
            return PolaAtrybutow;
        }

        public void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy, Dictionary<int, long> hashe, ref List<Klient> klienci)
        {

            Dictionary<int, StatusZamowienia> atrybutyB2B = ApiWywolanie.PobierzStatusyZamowien().ToDictionary(x => x.Id, x => x);
            PropertyInfo[] propertisyAtrybutow = typeof(StatusZamowienia).GetProperties();

            foreach (StatusZamowienia docelowy in statusy)
            {
                var wzorcowy = atrybutyB2B.ContainsKey(docelowy.Id) ? atrybutyB2B[docelowy.Id] : null;
                UstawPola(docelowy, wzorcowy, propertisyAtrybutow, PolaAtrybutow);
            }
        }
    }
}