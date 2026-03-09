using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public class ListyPrzewozoweBll: IListyPrzewozoweBll
    {
        public List<HistoriaDokumentuListPrzewozowy> TworzenieListuPrzewozowego(int nrDokumentu,string nrListu, string format)
        {
            List<HistoriaDokumentuListPrzewozowy> wynik = new List<HistoriaDokumentuListPrzewozowy>();
            string[] numery = nrListu.Split(("; ,").ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string nr in numery)
            {
                string nrDoListu = nr.Trim();
                wynik.Add(new HistoriaDokumentuListPrzewozowy(nrDokumentu, nrDoListu, string.Format(format, nrDoListu)));
            }
            return wynik;
        }

        public void WyslijPowiadomienie(IList<HistoriaDokumentuListPrzewozowy> obj)
        {
            DateTime odKiedy = DateTime.Now.AddDays(-SolexBllCalosc.PobierzInstancje.Konfiguracja.ZIluDniWsteczWysylacListyPrzewozowe);
            List<int> idsDokumentow = obj.Select(x => x.DokumentId).Distinct().ToList();

            IList<DokumentyBll> dokumentyDoWyslania = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DokumentyBll>(null, x => x.DataUtworzenia >= odKiedy && Sql.In(x.Id, idsDokumentow));

            if (dokumentyDoWyslania.IsEmpty())
            {
                SolexBllCalosc.PobierzInstancje.Log.DebugFormat("Zablokowane wysłanie listu przewozowego z powodu daty dokumentu przekraczajacej ustawienie ZIluDniWsteczWysylacListyPrzewozowe");
            }

            foreach (var dokument in dokumentyDoWyslania)
            {
                List<HistoriaDokumentuListPrzewozowy> listy = obj.Where(x => x.DokumentId == dokument.Id).ToList();
                SolexBllCalosc.PobierzInstancje.Statystyki.ZdarzenieNoweListyPrzewozowe(dokument, listy);
            }
        }
    }
}