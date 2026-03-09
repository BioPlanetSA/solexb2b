using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    public class UsuwanieZamowienBezRezerwacji : SyncModul, IModulDokumenty
    {

        public void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy, Dictionary<int, long> hashe, ref List<Klient> klienci)
        {
            var lista = ApiWywolanie.PobierzWszystkieZamowienia();
            List<ZamowienieSynchronizacja> listaZamowien = new List<ZamowienieSynchronizacja>();
            List<HistoriaDokumentu>doUsuniecia = new List<HistoriaDokumentu>();
            foreach (var dok in listaWejsciowa)
            {
                bool zrealizowany = false;
                if (FiltrowacDokumentyZeStatusemZrealizowanym)
                {
                    if (dok.Key.StatusId == (int)StatusImportuZamowieniaDoErp.Zrealizowane)
                    {
                        zrealizowany = true;
                    }
                }
                if (dok.Key.Rodzaj == RodzajDokumentu.Zamowienie && !dok.Key.Rezerwacja && !zrealizowany)
                {
                    int id = dok.Key.Id;
                    var a = lista.Where(x => x.ListaDokumentowZamowienia.Any(c=>c.IdDokumentu == id)).ToList();

                    if (a.Any())
                    {
                        for (int j = 0; j < a.Count; j++)
                        {
                            a[j].StatusId = StatusImportuZamowieniaDoErp.Usunięte;
                            listaZamowien.Add(a[j]);
                        }
                    }
                    doUsuniecia.Add(dok.Key);
                }
            }
            foreach (var historiaDokumentu in doUsuniecia)
            {
                List<HistoriaDokumentuProdukt> test;
                listaWejsciowa.TryRemove(historiaDokumentu, out test);
                ((IDictionary)listaWejsciowa).Remove(historiaDokumentu);
            }
            ApiWywolanie.AktualizujZamowienie(listaZamowien);

            //for (int i = 0; i < listaWejsciowa.Count; i++)
            //{
            //    //jezeli ustawienie FiltrowacDokumenty... na NIE, to usuwamy zamowienia ktore nie maja rezerwacji
            //    //jezeli ustawienie FiltrowacDokumenty... na TAK, to usuwamy zamowienia krore nie maja rezerwacji  i nie maja statusu zrealizowanego, jezeli maja status zrealizowany to nie sa usuwane 
            //    bool zrealizowany = false;
            //    if (FiltrowacDokumentyZeStatusemZrealizowanym) 
            //    {
            //        if (listaWejsciowa[i]..StatusId == idStatusuZrealizowanego)
            //        {
            //            zrealizowany = true;
            //        }
            //    }

            //    if (listaWejsciowa[i].Rodzaj == RodzajDokumentu.Zamowienie && !listaWejsciowa[i].Rezerwacja && !zrealizowany)
            //    {
            //        int id = listaWejsciowa[i].Id;
            //        var a = lista.Where(x => x.DokumentyId.Contains(id)).ToList();

            //        if (a.Any())
            //        {
            //            for (int j = 0; j < a.Count; j++)
            //            {
            //                a[j].StatusId = idStatusu;
            //                listaZamowien.Add(a[j]);
            //            }
            //        }
            //        listaWejsciowa.RemoveAt(i);
            //        i--;
            //    }
            //}
        }

        public override string uwagi
        {
            get { return "Z wysyłanych zamówień usuwane są zamówienia bez rezerwacji. W razie problemów z synchronizacją dokumentów, zwiększ ilość dokumentów w ustawieniu MaksimumDokumentowWPaczce. Moduł działa dla nowych dokumentów. Dla starych nalezy je pierw usunąć"; }
        }

        [FriendlyName("Czy filtrować dokumenty ze statusem 'Zrealizowane'?")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool FiltrowacDokumentyZeStatusemZrealizowanym { get; set; }
    }
}
