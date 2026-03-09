using System;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany
{
    [FriendlyName("Aktywność produktu na podstawie stanu",FriendlyOpis = "Moduł, który ustawia aktywność produktu na podstawie podanego stanu lub cechy. Cecha nadpisuje stan. " +
                                 "Jeśli wartość cechy będzie wynosiła -1 to produkt ma być zawsze widoczny. Przeznaczony jest dla systemów z jednym magazynem. " +
                                 "Należy ręcznie ustawić kolejność modułu, która nie może być 0" )]
    public class DeaktywacjaZeStanu : SyncModul, IModulStany, IModulProdukty
    {
        [FriendlyName("Stan poniżej którego produkt będzie nieaktywny")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Stan { get; set; }

        [FriendlyName("ID magazynu dla którego stany są uwzględniane ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikMagazynow))]
        public int MagazynID { get; set; }
        
        [Niewymagane]
        [FriendlyName("Początek symbolu cechy z B2B, która określa minimalny stan towaru. Nadpisuje ustawienie 'Stan'")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [WidoczneListaAdmin(false, false, true, false)]
        public int? IdAtrybutu { get; set; }
         
        [Niewymagane]
        [FriendlyName("Stan, który będzie ustawiany dla produktów, które mają być zawsze widoczne")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int? StanDlaTowarowZawszeWidocznych { get; set; }

        public DeaktywacjaZeStanu()
        {
            Stan = "10";
            IdAtrybutu = null;
            StanDlaTowarowZawszeWidocznych = 10;
        }


        public void PrzetworzMain(ref List<Produkt> produkty, List<ProduktStan> stany, Dictionary<long, ProduktCecha> cechyprodukty, List<Cecha> cechy, List<Produkt> doaktualizacji = null)
        {
            Dictionary<long, Cecha> slownikCechZAtrybutem = new Dictionary<long, Cecha>();
            if (IdAtrybutu.HasValue)
            {
                slownikCechZAtrybutem = cechy.Where(x => x.AtrybutId == IdAtrybutu.Value).ToDictionary(x => x.Id, x => x);
            }
            foreach (Produkt produkt in produkty)
            {
                decimal stanmin;
                TextHelper.PobierzInstancje.SprobojSparsowac(Stan, out stanmin);
                if (IdAtrybutu!=null)
                {
                    var idCechProduktu = new HashSet<long>( cechyprodukty.Values.Where(x => x.ProduktId == produkt.Id).Select(x=>x.CechaId) );
                    Cecha cecha = slownikCechZAtrybutem.WhereKeyIsIn(idCechProduktu).FirstOrDefault();

                    //Cecha szukanaCecha = ProduktyWyszukiwanie.PobierzInstancje.PobierzWszystkieCechyProduktu(produkt.Id, cechy, cechyprodukty.Values.ToList(), Cecha).FirstOrDefault();

                    if (cecha != null && cecha.Nazwa != "0")
                    {
                      TextHelper.PobierzInstancje.SprobojSparsowac(cecha.Nazwa, out stanmin) ;
                    }
                }
                bool sztuczny = false;
                //Nie trzba sprawdzac magazynu gdyż do metody przekazane sa tylko stany z magazynu zaznaczonego w module
                ProduktStan stan = stany.FirstOrDefault(a => a.ProduktId == produkt.Id);
                if (stan == null)
                {
                    stan = new ProduktStan { ProduktId = produkt.Id, MagazynId = MagazynID };
                    sztuczny = true;
                }
                bool nowawidocznosc;

                if (stanmin == -1)
                {
                    nowawidocznosc = true;
                }
                else
                {
                    nowawidocznosc = stan.Stan >= stanmin;
                }
                if (nowawidocznosc != produkt.Widoczny)
                {
                    Log.DebugFormat($"Zmieniamy aktywnosc produktu {produkt.Id}. na {nowawidocznosc}");
                    produkt.UstawWidocznoscProduktu(nowawidocznosc, ZadanieBazowe.ModulKolejnosc);

                    //czyli odpala się w zadaniu stanów a nie produktów
                    if (doaktualizacji != null)
                        doaktualizacji.Add(produkt);
                }
                if (stanmin == -1)
                {
                    stan.Stan = StanDlaTowarowZawszeWidocznych.GetValueOrDefault();
                    if (sztuczny)
                    {
                        stany.Add(stan);
                    }
                }
            }

        }

        public void Przetworz(ref Dictionary<int,List<ProduktStan>> stany, List<Magazyn> magazyny,List<Produkt>produkty )
        {
            if (stany.Count == 0)
                return;

            Log.Debug("Przetwarzanie stanów dla magazynu o ID " + MagazynID);
            Log.Debug("Stan poniżej którego produkty będą ukryte: " + Stan);
            Log.Debug("stanów: " + stany.Count);

            if (string.IsNullOrEmpty(Stan) || MagazynID==0)
                return;

            var cechy = ApiWywolanie.PobierzCechy().Values.ToList();
            var cechyprodukty = ApiWywolanie.PobierzCechyProdukty();
            
            List<Produkt> doaktualizacji = new List<Produkt>();
            
            PrzetworzMain(ref produkty, stany[MagazynID], cechyprodukty, cechy, doaktualizacji);

            int niewidocznych = doaktualizacji.Where(a => a.Widoczny == false).Count();
            int widocznych = doaktualizacji.Where(a => a.Widoczny).Count();

            Log.Debug($"Produktów do aktualizacji: {doaktualizacji.Count}, widocznych: {widocznych}, niewidocznych: {niewidocznych}");
            if(doaktualizacji.Count > 0)
                ApiWywolanie.AktualizujProdukty(doaktualizacji);
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, 
            ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, 
            ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            //var cechy = ApiWywolanie.PobierzCechy().Values.ToList();
            var magazyny = ApiWywolanie.PobierzMagazyny().FirstOrDefault(a => a.Id == MagazynID);
            var stany = ApiWywolanie.PobierzStanyProduktow(magazyny);
            PrzetworzMain(ref listaWejsciowa, stany,lacznikiCech,cechy);
        }
    }
}
