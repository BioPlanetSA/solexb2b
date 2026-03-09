using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ListyPrzewozowe
{
    public class ListyPrzewozowePocztaPolska : SyncModul, Model.Interfaces.SyncModuly.IModulListyPrzewozowe
    {
        public override string Opis
        {
            get { return "We wcześniej pobranych listach, zmienia linki krajowym listom Poczty polskiej. Numer listu przewozowego musi mieć 20 znaków"; }
        }
        [FriendlyName("Format linku do trackingu, {0} zastępuje numer listu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string FormatLinku { get; set; }
        public void Przetworz(ref List<Model.HistoriaDokumentuListPrzewozowy> listaWejsciowa, Dictionary<int, long> dokumentyWErp, Model.Interfaces.Sync.ISyncProvider provider)
        {
            foreach (var list in listaWejsciowa)
            {
                // keidys byl warunek (list.numer_listu.EndsWith("U") || list.numer_listu.EndsWith("Y") ) &&  - wywalam Bartek, nie wiem po co to?
                if ( list.NumerListu.Length == 20)
                {
                    list.Link = string.Format(FormatLinku, list.NumerListu);
                }
            }
        }

        public override string uwagi
        {
            get { return "http://emonitoring.poczta-polska.pl/?numer={0}   Maile o listach wysyłane są do dokumentów utworzonych w ciągu ostatnich x dni(ustawienie z-ilu-dni-wstecz-listy-przewozowe)"; }
        }
    }
}
