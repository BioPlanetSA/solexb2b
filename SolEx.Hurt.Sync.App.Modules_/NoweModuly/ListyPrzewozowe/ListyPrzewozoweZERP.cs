using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ListyPrzewozowe
{
    [FriendlyName("Pobieranie listów przewozowych z systemu ERP")]
    public class ListyPrzewozoweZERP : SyncModul, Model.Interfaces.SyncModuly.IModulListyPrzewozowe
    {
        [Wymagane]
        [FriendlyName("Pole z którego pobieramy list przewozowy (dla Subiekta, WFmaga, XL pole wymagane, dla Optimy pobiera zawsze z dedykowanego pola 'numer listu przewozowego')")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }

        [FriendlyName("Format linku do trackingu, {0} zastępuje numer listu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string FormatLinku { get; set; }

        public override string uwagi => "Maile o listach wysyłane są do dokumentów utworzonych w ciągu ostatnich x dni(ustawienie z-ilu-dni-wstecz-listy-przewozowe)";

        public void Przetworz(ref List<HistoriaDokumentuListPrzewozowy> listaWejsciowa, Dictionary<int, long> dokumentyWErp, ISyncProvider provider)
        {
            HashSet<int> listyerpids = new HashSet<int>( dokumentyWErp.Select(x => x.Key) );

            if (string.IsNullOrEmpty(FormatLinku))
            {
                throw new InvalidOperationException("Format linku jest pusty");
            }

            if (string.IsNullOrEmpty(Pole))
            {
                throw new InvalidOperationException("Pole jakie pobierać z ERP jest puste");
            }

            Pole = Pole.Trim();

            IPobieraniePolaDokumentu ksiegowy = provider as IPobieraniePolaDokumentu;
            if (ksiegowy == null)
            {
                throw new NotImplementedException("Aktualny provider ksiegowy nie obsuguje interfejsu IPobieraniePolaDokumentu");
            }

            Dictionary<int, Dictionary<string, string>> pola = ksiegowy.PobierzPole(listyerpids);

            if (pola.IsEmpty())
            {
                Log.DebugFormat("Brak pól w ERP do dokumentów. Przerywam działanie.");
                return;
            }

            List<string> nazwyPolWyciagnietych = pola.Values.SelectMany(x => x.Keys).Distinct().ToList();
            Log.DebugFormat($"Pobrane pola z ERP do listów przewozowych: ogólna liczba: {pola.Count}, nazwy pól: {nazwyPolWyciagnietych.ToCsv()}");

            if (!nazwyPolWyciagnietych.Contains(Pole))
            {
                throw new Exception("W wyciągnietych polach do dokumentów brak wymaganego pola: " + Pole);
            }

            List<HistoriaDokumentuListPrzewozowy> wynik = new List<HistoriaDokumentuListPrzewozowy>();
            foreach (var dok in pola)
            {
                if (!listyerpids.Contains(dok.Key))
                {
                    throw new Exception($"Z ERP pobrano pola do dokumentu którego nie ma na liście dokumentów ERP. Dokumenet ID: {dok.Key}. Przerywam działanie");
                }

                string wartosc;
                if (!dok.Value.TryGetValue(Pole, out wartosc)) continue;
                List<HistoriaDokumentuListPrzewozowy> list = SolexBllCalosc.PobierzInstancje.ListyPrzewozoweBll.TworzenieListuPrzewozowego(dok.Key, wartosc, FormatLinku);
                wynik.AddRange(list);
            }
            listaWejsciowa = wynik;
        }

    }
}
