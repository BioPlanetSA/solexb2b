using System.Reflection;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    [SynchronizowanePola(typeof(Cecha))]
    [ModulStandardowy]
    public class KtorePolaSynchronizowacCechy : KtorePolaSynchonizowacBaza,IModulCechyIAtrybuty, IModulPola
    {

        public KtorePolaSynchronizowacCechy()
        {
            PolaCech = new List<string>();
            PolaCech.Add("ObrazekId");
            PolaCech.Add("Opis");
            PolaCech.Add("Kolejnosc");
            PolaCech.Add("OpisNaProdukcie");


            PolaCech.Add("MetkaOpis");
            PolaCech.Add("MetkaPozycjaSzczegoly");
            PolaCech.Add("MetkaKatalog");
            PolaCech.Add("MetkaPozycjaLista");
            PolaCech.Add("MetkaPozycjaRodziny");
            PolaCech.Add("MetkaPozycjaSzczegolyWarianty");
            PolaCech.Add("MetkaPozycjaKoszykProdukty");
            PolaCech.Add("MetkaPozycjaKoszykAutomatyczne");
            PolaCech.Add("MetkaPozycjaKoszykGratisy");
            PolaCech.Add("MetkaPozycjaKoszykGratisyPopUp");
            PolaCech.Add("MetkaPozycjaKafle");
        }
          [WidoczneListaAdminAttribute(false, false, true, false)]
        [FriendlyName("Wybierz pola które będziesz edytował w adminie B2B. Pola nie wybrane będa nadpisywane wartościami z ERP (są oznaczone na zielono w adminie)")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Cecha))]
        public List<string> PolaCech { get; set; }
        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            if (PolaCech.Count == 0)
            {
                return;
            }
            Dictionary<long, Cecha> cechyB2B = ApiWywolanie.PobierzCechy();
            PropertyInfo[] propertisyCech = typeof(Cecha).GetProperties();
            foreach (Cecha docelowa in cechy)
            {
                var wzorcowa = cechyB2B.ContainsKey(docelowa.Id) ? cechyB2B[docelowa.Id] : null;
                UstawPola(docelowa,wzorcowa,propertisyCech,PolaCech);
            }
        }

        public List<string> PobierzDostepnePola()
        {
            return PolaCech;
        }
    }
}
