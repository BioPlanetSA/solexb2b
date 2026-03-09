using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using HistoriaDokumentuListPrzewozowy = SolEx.Hurt.Model.HistoriaDokumentuListPrzewozowy;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ListyPrzewozowe
{
    class ListyPrzewozoweRaben : SyncModul,IModulListyPrzewozowe
    {
        public override string Opis
        {
            get { return "Pobiera listy przewozowe RABEN z erp"; }
        }
        [FriendlyName("Link do podglądu listów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string FormatLinku { get; set; }
        public void Przetworz(ref List<HistoriaDokumentuListPrzewozowy> listaWejsciowa, Dictionary<int, long> dokumentyWErp, Model.Interfaces.Sync.ISyncProvider provider)
        {
            foreach (var list in listaWejsciowa)
            {
                    if (list.NumerListu.Length > 9)
                    {
                        list.NumerListu = list.NumerListu.Substring(3);
                        list.NumerListu = list.NumerListu.Substring(0, list.NumerListu.Length - 4);
                        list.Link = string.Format(FormatLinku, list.NumerListu);
                    }
            }
        }
        public override string uwagi
        {
            get { return "Maile o listach wysyłane są do dokumentów utworzonych w ciągu ostatnich x dni(ustawienie z-ilu-dni-wstecz-listy-przewozowe)"; }
        }
    }
}
