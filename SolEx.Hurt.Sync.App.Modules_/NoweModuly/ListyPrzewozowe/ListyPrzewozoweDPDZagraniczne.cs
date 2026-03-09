using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ListyPrzewozowe
{
    public class ListyPrzewozoweDPDZagraniczne : SyncModul, IModulListyPrzewozowe
    {
        public override string Opis
        {
            get { return "We wcześniej pobranych listach, zmienia linki zagranicznym listom DPD. Numer listu przewozowego musi zaczynac się od 13"; }
        }
        [FriendlyName("Format linku do trackingu, {0} zastępuje numer listu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string FormatLinku { get; set; }
        
        public void Przetworz(ref List<Model.HistoriaDokumentuListPrzewozowy> listaWejsciowa, Dictionary<int, long> dokumentyWErp, Model.Interfaces.Sync.ISyncProvider provider)
        {
            foreach (var list in listaWejsciowa)
            {
                if (list.NumerListu.StartsWith("13"))
                {
                    list.Link = string.Format(FormatLinku, list.NumerListu);
                }
            }
        }

        public override string uwagi 
        {
            get { return "link powinien być w formie http://tracking.dpd.de/cgi-bin/delistrack?pknr={0} Maile o listach wysyłane są do dokumentów utworzonych w ciągu ostatnich x dni(ustawienie z-ilu-dni-wstecz-listy-przewozowe)"; }
        }
    }
}
