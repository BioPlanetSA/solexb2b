using System.Reflection;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    [SynchronizowanePola(typeof(Atrybut))]
    [ModulStandardowy]
    public  class KtorePolaSynchronizowacAtrybuty : KtorePolaSynchonizowacBaza,IModulCechyIAtrybuty, IModulPola
    {

        public KtorePolaSynchronizowacAtrybuty()
        {
            PolaAtrybutow = new List<string>();
            PolaAtrybutow.Add("Nazwa");
            PolaAtrybutow.Add("Widoczny");
            PolaAtrybutow.Add("Kolejnosc");
            PolaAtrybutow.Add("ProviderWyswietlania");
            PolaAtrybutow.Add("TolerancjaGora");
            PolaAtrybutow.Add("TolerancjaDol");
            PolaAtrybutow.Add("ZawszeWszystkieCechy");
            PolaAtrybutow.Add("NazwaOpisowa");
            PolaAtrybutow.Add("PokazujWWyszukiwaniu");
            PolaAtrybutow.Add("UkryjJednaWartosc");
            PolaAtrybutow.Add("PokazujNaLiscieProduktow");
            PolaAtrybutow.Add("CechyPokazujKatalog");
            PolaAtrybutow.Add("PokazujOpisMetki");
            PolaAtrybutow.Add("PobierajCechy");
            PolaAtrybutow.Add("PokazujNazweAtrybutuJakoNaglowekFiltra");

            PolaAtrybutow.Add("UniwersalnaMetkaOpis");
            PolaAtrybutow.Add("UniwersalnaMetkaKatalog");
            PolaAtrybutow.Add("MetkaPozycjaSzczegoly");
            PolaAtrybutow.Add("MetkaKatalog");
            PolaAtrybutow.Add("MetkaPozycjaLista");
            PolaAtrybutow.Add("MetkaPozycjaRodziny");
            PolaAtrybutow.Add("MetkaPozycjaSzczegolyWarianty");
            PolaAtrybutow.Add("MetkaPozycjaKoszykProdukty");
            PolaAtrybutow.Add("MetkaPozycjaKoszykAutomatyczne");
            PolaAtrybutow.Add("MetkaPozycjaKoszykGratisy");
            PolaAtrybutow.Add("MetkaPozycjaKoszykGratisyPopUp");
            PolaAtrybutow.Add("MetkaPozycjaKafle");
            PolaAtrybutow.Add("PokazujWPlikachIntegracji");
        }
        [FriendlyName("Wybierz pola które będziesz edytował w adminie B2B. Pola nie wybrane będa nadpisywane wartościami z ERP (są oznaczone na zielono w adminie)")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Atrybut))]
        [WidoczneListaAdminAttribute(false,false,true,false)]
        public List<string> PolaAtrybutow { get; set; }

        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            if (PolaAtrybutow.Count == 0)
            {
                return;
            }

            Dictionary<int, Atrybut> atrybutyB2B = ApiWywolanie.PobierzAtrybuty();
            PropertyInfo[] propertisyAtrybutow = typeof (Atrybut).GetProperties();

            foreach (Atrybut docelowy in atrybuty)
            {
                var wzorcowy =  atrybutyB2B.ContainsKey(docelowy.Id) ? atrybutyB2B[docelowy.Id] : null;
                UstawPola(docelowy,wzorcowy,propertisyAtrybutow,PolaAtrybutow);
            }
        }

        public List<string> PobierzDostepnePola()
        {
            return PolaAtrybutow;
        }
    }
}
