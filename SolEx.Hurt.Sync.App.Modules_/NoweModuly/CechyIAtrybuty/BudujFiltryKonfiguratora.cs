using System;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{

    [FriendlyName("Buduj filtry konfiguratora",FriendlyOpis = "Budowanie filtrów złożonych - składających się z cech nadrzędnych które muszą być" +
                                                              "wybierane w kolejności np. Marka samochodu, model, rocznik, pojemność itp.")]
    public class BudujFiltryKonfiguratora : CechyModulBaza
    {
        public IConfigSynchro Config = SyncManager.PobierzInstancje.Konfiguracja;

        public BudujFiltryKonfiguratora()
        {
            Separator = "|";
        }

        [FriendlyName("Atrybut z którego cechy będą przetwarzane")]
        [PobieranieSlownika(typeof (SlownikAtrybutow))]
        [WidoczneListaAdmin(false, false, true, false)]
        public int Atrybut { get; set; }

        [FriendlyName("Znak którym będą rozdzielane cechy", FriendlyOpis = "Separator cech - przykładowy format cechy: Samochód:Fiat|Panda|2005|1.8|benzyna - w tym wypadku separatorem jest |. ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }

        [FriendlyName("Nazwy Atrybutów, rozdzielone tym samym separatorem co powyżej",FriendlyOpis = "Kolejność będzie zgodna z rozbiciem, np. dla cechy Samochód:Fiat|Panda|2005|1.8|benzyna powinny być atrybuty:  Marka|Model|Rocznik|Pojemność|Paliwo. Atrybuty zostaną utworzone automatycznie.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwyAtrybutow { get; set; }


        public override void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            Dictionary<long, ProduktCecha> laczniki = new Dictionary<long, ProduktCecha>();
            RozbijCeche(ref atrybuty, ref cechy, ref laczniki);
        }

        public override void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            RozbijCeche(ref atrybuty, ref cechy, ref lacznikiCech);
        }

        public void RozbijCeche(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, ref Dictionary<long, ProduktCecha> lacznikiCech)
        {

            if (string.IsNullOrEmpty(NazwyAtrybutow))
            {
                Log.Debug("Pole NazwyAtrybutow jest puste, moduł przerwie działanie");
                return;
            }


            Atrybut atr = atrybuty.FirstOrDefault(x => x.Id == Atrybut);
            if (atr == null)
            {
                Log.Debug(string.Format("Brak atrybutu:{0} na liście Atrybutów, moduł przerwie działanie", Atrybut));
                return;
            }


            string[] nazwyAtr = NazwyAtrybutow.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in nazwyAtr)
            {
                Atrybut nowy = ZnajdzAtrybut(s, atrybuty);
                if (nowy == null)
                {
                    DodajBrakujacyAtrybut(s, atrybuty);
                }
            }
            List<Cecha> lista = cechy.Where(x => x.AtrybutId == atr.Id).ToList();
            foreach (Cecha cecha in lista)
            {
                string[] noweCechy = cecha.Nazwa.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);
                if (noweCechy.Count() != nazwyAtr.Count())
                {
                    Log.Info(
                        string.Format(
                            "Bład konfiguracji nowych cech dla cechy {0}, Nie zgdadza się ilość nowych atrybutów z ilością nowych cech.",
                            cecha.Nazwa));
                    return;
                }
                HashSet<long> idCechyNadrzednej=new HashSet<long>();
                for (int i = 0; i < noweCechy.Count(); i++)
                {
                    Cecha nowa = ZnajdzCeche(nazwyAtr[i].Trim() + ":" + noweCechy[i].Trim(), cechy);
                    Atrybut atrNowy = ZnajdzAtrybut(nazwyAtr[i], atrybuty);
                    if (nowa == null)
                    {
                        nowa = DodajBrakujacaCeche(nazwyAtr[i].Trim() + ":" + noweCechy[i].Trim(), noweCechy[i], atrNowy.Id, cechy);
                    }
                    if(idCechyNadrzednej.Any()) nowa = DodajCecheNadrzedna(nowa, idCechyNadrzednej);
                    List<long> idProdList = lacznikiCech.Where(x => x.Value.CechaId == cecha.Id).Select(x => x.Value.ProduktId).ToList();
                    foreach (int idProd in idProdList)
                    {
                        DodajBrakujaceLaczniki(nowa.Id, idProd, lacznikiCech);
                    }

                    if (Config.TrybPokazywaniaFiltrow == TrybPokazywaniaFiltrow.WymuszajSciezke) idCechyNadrzednej = new HashSet<long> { nowa.Id };
                    if (Config.TrybPokazywaniaFiltrow == TrybPokazywaniaFiltrow.WszystkieAtrybutyJednoczesnie) idCechyNadrzednej.Add(nowa.Id);
                }
            }
        }
    }
}
