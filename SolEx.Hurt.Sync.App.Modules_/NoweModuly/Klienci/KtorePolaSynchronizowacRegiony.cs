using System.Linq;
using System.Reflection;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{

    [SynchronizowanePola(typeof(Region))]
    [ModulStandardowy]
    public class KtorePolaSynchronizowacRegiony : KtorePolaSynchonizowacBaza, IModulKlienci, IModulPola
    {
        public override string uwagi
        {
            get { return ""; }
        }

        public KtorePolaSynchronizowacRegiony()
        {
            Pola = new List<string>();
            Pola.Add("Widoczny");
        }

          [FriendlyName("Wybierz pola które będziesz edytował w adminie B2B. Pola nie wybrane będa nadpisywane wartościami z ERP (są oznaczone na zielono w adminie)")]
          [SyncSlownikNaPodstawieInnegoTypu(typeof(Region))]
          [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

          public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            if (Pola.Count == 0)
            {
                return;
            }

            IEnumerable<Region> regionywz = ApiWywolanie.PobierzRegiony().Values;
            PropertyInfo[] propertisy = typeof(Region).GetProperties();
            foreach (Region wzorzec in regionywz)
            {
                Region docelowy = regiony.FirstOrDefault(x => x.Id == wzorzec.Id);
                UstawPola(docelowy, wzorzec, propertisy, Pola);
            }
        }

        public List<string> PobierzDostepnePola()
        {
            return Pola;
        }
    }

}
