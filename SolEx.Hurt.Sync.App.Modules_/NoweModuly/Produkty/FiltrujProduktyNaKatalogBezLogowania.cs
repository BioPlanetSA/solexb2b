using System.Linq;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using ServiceStack.Net30.Collections.Concurrent;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Text;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class FiltrujProduktyNaKatalogBezLogowania : SyncModul, IModulProdukty, ITestowalna
    {
        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Cechy na podstawie których będzie ustawiana widoczność produktów")]
        [PobieranieSlownika(typeof (SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> ListaCech { get; set; }

        [FriendlyName("Widoczność produktu jeśli posiada przynajmniej jedną z określoncyh cech")]
        [PobieranieSlownika(typeof (SlownikWidocznosci))]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int? WidocznoscKtorykolwiek { get; set; }

        [FriendlyName("Widoczność produktu jeśli posiada wszystkie cechy")]
        [PobieranieSlownika(typeof (SlownikWidocznosci))]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int? WidocznoscWszystkie { get; set; }

        [FriendlyName("Widoczność produktu jeśli NIE posiada żadnej z cech")]
        [PobieranieSlownika(typeof (SlownikWidocznosci))]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int? WidocznoscZadna { get; set; }

      
        public override string Opis
        {
            get { return "Można używać modułu kilka razy, ustawiając odpowiednio kolejność (stara nazwa: filtruj widoczność produktów na katalog bez logowania)"; }
        }

        public override string PokazywanaNazwa
        {
            get { return "Ustaw poziom dostępu do produktów, na podstawie cech"; }
        }

        public HashSet<int> ListaIdCech
        {
            get
            {
                HashSet<int> wynik = new HashSet<int>();
                foreach (var cecha in ListaCech)
                {
                    wynik.Add(int.Parse(cecha));
                }
                return wynik;
            }
        }


        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            HashSet<int> idCechWybranych = ListaIdCech;

            //tylko po to zeby w paraller mogl byc NIE ref
            Dictionary<long, ProduktCecha> laczniki = lacznikiCech;

           List<long> slownikZmian =  new List<long>(listaWejsciowa.Count);

            Parallel.ForEach(listaWejsciowa, produkty =>
            {
                int ilosc = 0;
                List<long> listaCechProduktow = laczniki.Where(x => x.Value.ProduktId == produkty.Id).Select(x => x.Value.CechaId).ToList();
                foreach (var idCechy in idCechWybranych)
                {
                    if (listaCechProduktow.Contains(idCechy))
                    {
                        ilosc++;
                    }
                }
                if (ilosc != 0)
                {
                    if (ilosc == idCechWybranych.Count && WidocznoscWszystkie.HasValue)
                    {
                        if (Log.IsDebugEnabled)
                        {
                            slownikZmian.Add(produkty.Id);
                        }
                        produkty.Widocznosc = (AccesLevel) WidocznoscWszystkie;
                    }
                    else if (ilosc != idCechWybranych.Count && WidocznoscKtorykolwiek.HasValue)
                    {
                        if (Log.IsDebugEnabled)
                        {
                            slownikZmian.Add(produkty.Id);
                        }
                        produkty.Widocznosc = (AccesLevel) WidocznoscKtorykolwiek;
                    }
                }
                else
                {
                    if (WidocznoscZadna.HasValue)
                    {
                        if (Log.IsDebugEnabled)
                        {
                            slownikZmian.Add(produkty.Id);
                        }
                        produkty.Widocznosc = (AccesLevel) WidocznoscZadna;
                    }
                }
            });

            if (Log.IsDebugEnabled)
            {
                var slownikWidocznosci = listaWejsciowa.Where(x => slownikZmian.Contains(x.Id)).GroupBy(x => x.Widocznosc.Value).ToDictionary(x => x.Key, x => x.Select(z => z.Id).ToList());
                foreach (var widocznosc in slownikWidocznosci)
                {
                    Log.Debug($"Zmiany na widoczność: {widocznosc.Key} dla produktów id: [{widocznosc.Value.Join(",")}]");
                }
            }
        }

        public List<string> TestPoprawnosci()
        {
            List<string>bledy = new List<string>();
            if (WidocznoscKtorykolwiek == -1)
            {
                bledy.Add("Parametr WidocznoscKtoregokolwiek nie ma wartości");
            }
            if (WidocznoscWszystkie == -1)
            {
                bledy.Add("Parametr WidocznoscWszystkie nie ma wartości");
            }
            return bledy;
        }
    }

}
