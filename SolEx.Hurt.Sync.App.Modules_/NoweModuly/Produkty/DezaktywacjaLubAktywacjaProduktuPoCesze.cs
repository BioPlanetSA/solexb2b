using System;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Wybieranie produktów które mają być synchronizowanie z ERP",
        FriendlyOpis = "Moduł, który decyduje które produkty mają być synchronizowane z ERP. Należy ręcznie ustawić kolejność modułu, która nie może być 0.")]
    public class DezaktywacjaLubAktywacjaProduktuPoCesze : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        [FriendlyName("Cecha z ERP, wymagana do ustawienia aktywności produktu")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<int> Cechy { get; set; }

        [FriendlyName("Jeśli produkt ma cechę to ma być domyślnie aktywny")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool Widocznosc { get; set; }

        public DezaktywacjaLubAktywacjaProduktuPoCesze() 
        {
            //Cecha = string.Empty;
            Widocznosc = false;
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, 
            ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, 
            ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {           
            int iloscZmian = 0;
            if (Cechy != null && Cechy.Any())
            {
                foreach (var c in Cechy)
                {
                    HashSet<long> produktyZCechas = new HashSet<long>( lacznikiCech.Values.Where(a => a.CechaId == c).Select(x => x.ProduktId) );

                    if (produktyZCechas.Count == 0)
                    {
                        Log.ErrorFormat("Wygląda na to że żaden produkt nie posiada cechy o id:{0}", c);
                    }

                    foreach (Produkt p in listaWejsciowa)
                    {
                        if (!produktyZCechas.Contains(p.Id))
                        {
                            continue;
                        }
                        ++iloscZmian;
                        p.UstawWidocznoscProduktu(Widocznosc);
                    }
                }
            }
            else
            {
                throw new Exception("Brak konfiguracji cechy do procesowania w module WidocznoscProduktowPoCesze!!");
            }


            //usuwamy produkty z ERP ktore sa NIEWIDOCZNE i NIE MA ICH na B2B - chcemy sie pozbyc produktow nieaktywnych, ktorych nigdy nie bylo na platformie (jesli ktos by je chcial miec to musi je dodać na platforme i zdealkitowac pozniej)
            if (listaWejsciowa.RemoveAll(x => x.Widoczny == false && !produktyNaB2B.ContainsKey(x.Id)) > 0)
            {
                Log.Info("Z listy produktów ERP zostały usunięte produkty niewidoczne, których nigdy nie było na B2B. Jeśli chcesz je pokazać musisz jednorazowo je aktywować na B2B.");
            }



            Log.InfoFormat("Moduł wykonał {0} zmian", iloscZmian);
        }
    }
}
