using System.Reflection;
using SolEx.Hurt.Core;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;
using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{

    [SynchronizowanePola(typeof(Klient))]
    [ModulStandardowy]
    public class KtorePolaSynchronizowac : KtorePolaSynchonizowacBaza, IModulKlienci, IModulPola
    {
        public override string uwagi
        {
            get { return ""; }
        }

        public KtorePolaSynchronizowac()
        {
            Pola = new List<string>();
            Pola.Add("HasloKlienta");
            Pola.Add("DataOstatniegoLogowania");
            Pola.Add("DataZmianyHasla");
            Pola.Add("JezykId");
            Pola.Add("Gid");
            Pola.Add("KluczSesji");
            Pola.Add("DataZmianyKlucza");
            Pola.Add("DataDodatnia");
            Pola.Add("ZgodaNaNewsletter");
            Pola.Add("JakieElementyMenu");
            Pola.Add("HasloOdkryte");
            Pola.Add("JezykId");
            Pola.Add("IdUlubionych");
            Pola.Add("IdInfoODostepnosci");
            //Pola.Add("WidziPunkty");
        }

        [FriendlyName("Wybierz pola które będziesz edytował w adminie B2B. Pola nie wybrane będa nadpisywane wartościami z ERP (są oznaczone na zielono w adminie)")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            if (Pola.Count == 0)
            {
                return;
            }

            IEnumerable<Klient> klienci = ApiWywolanie.PobierzKlientow().Values;
            PropertyInfo[] propertisy = typeof(Klient).GetProperties();
            foreach (Klient wzorzec in klienci)
            {
                Klient docelowy = listaWejsciowa.ContainsKey(wzorzec.Id) ? listaWejsciowa[wzorzec.Id] : null;
                UstawPola(docelowy, wzorzec, propertisy, Pola);
            }
        }

        public List<string> PobierzDostepnePola()
        {
            return Pola;
        }
    }

}
