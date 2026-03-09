using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [ModulStandardowy]
    [SynchronizowanePola(typeof(Waluta))]
    public class KtorePolaSynchronizowacWaluta : KtorePolaSynchonizowacBaza, IModulPola, IZadaniaOglone
    {
        public KtorePolaSynchronizowacWaluta()
        {
            Pola = new List<string>()
            {
                nameof(Waluta.NrKonta)
            };
        }

        [FriendlyName("Wybierz pola które będziesz edytował w adminie B2B. Pola nie wybrane będa nadpisywane wartościami z ERP (są oznaczone na zielono w adminie)")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Waluta))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

        public List<string> PobierzDostepnePola()
        {
            return Pola;
        }

        public void Przetworz(ISyncProvider provider)
        {
            //nic nie robimy
            return;
        }
    }
}
