using System.Reflection;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow
{
    [SynchronizowanePola(typeof(KategoriaProduktu))]
    [ModulStandardowy]
    public class KtorePolaSynchronizowac : KtorePolaSynchonizowacBaza, IModulKategorieProduktow, IModulPola
    {

        public KtorePolaSynchronizowac()
        {
            Pola=new List<string>();
            Pola.Add("ObrazekId");
            Pola.Add("Opis");
            Pola.Add("PokazujFiltry");
            Pola.Add("Kolejnosc");
            Pola.Add("Dostep");
            Pola.Add("MiniaturaId");
            Pola.Add("OpisNaProdukt");
            Pola.Add("KategoriaTresciSymbol");
            Pola.Add("KlasaCss");
        }

        [FriendlyName("Wybierz pola które będziesz edytował w adminie B2B. Pola nie wybrane będa nadpisywane wartościami z ERP (są oznaczone na zielono w adminie)")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(KategoriaProduktu))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

        public void Przetworz(ref Dictionary<long, KategoriaProduktu> listaWejsciowa, Dictionary<long, KategoriaProduktu> listaKategoriiB2B, ISyncProvider provider, List<Grupa> grupyPRoduktow)
        {
            if (Pola.Count == 0)
            {
                return;
            }

            PropertyInfo[] propertisy = typeof(KategoriaProduktu).GetProperties();

            foreach (KategoriaProduktu wzorcowa in listaKategoriiB2B.Values)
            {
                var docelowa = listaWejsciowa.ContainsKey(wzorcowa.Id) ? listaWejsciowa[wzorcowa.Id] : null;
                UstawPola(docelowa,wzorcowa,propertisy,Pola);
            }
        }
        public List<string> PobierzDostepnePola()
        {
            return Pola;
        }

        List<string> IModulPola.PobierzDostepnePola()
        {
            return Pola;
        }
    }

}
