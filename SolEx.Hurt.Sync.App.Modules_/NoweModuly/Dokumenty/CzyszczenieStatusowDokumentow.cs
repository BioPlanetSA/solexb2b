using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    [FriendlyName("Usuń statusy na dokumnecie",FriendlyOpis = "Usuwa określone statusy dokumentów")]
    public class CzyszczenieStatusowDokumentow : SyncModul, Model.Interfaces.SyncModuly.IModulDokumenty
    {
        public void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy, Dictionary<int, long> hashe, ref List<Klient> klienci)
        {
           throw new Exception("Obecnie już nie potrzebny");
            //string[] zastepniki = Statusy.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //if (zastepniki.Length > 0)
            //{
            //    foreach (historia_dokumenty d in listaWejsciowa)
            //    {
            //        if (!string.IsNullOrEmpty(d.status) && zastepniki.Contains(d.status))
            //        {
            //            d.status = "";
            //        }
            //    }
            //}
        }


        [FriendlyName("Statusy do usunięcia. Każdy nowy status musi być wpisany od nowej linijki")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Statusy { get; set; }

    }
}
