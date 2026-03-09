using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    [ModulStandardowy]
    [FriendlyName("Filtruj dokumenty pokazywane na B2B",FriendlyOpis = "Filtruje dokumenty do pobierania na b2b określając słowa zakazane i/lub słowa wymagane")]
    public class FiltrujDokumenty : SyncModul, IModulDokumenty
    {
        public void Przetworz(ref Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy, Dictionary<int, long> hashe, ref List<Klient> klienci)
        {
            if (string.IsNullOrEmpty(SlowaWymagane) && string.IsNullOrEmpty(SlowaZakazane)) return;
            char[] separatory = {' ', ',', ';'};
            string[] slowaZakazane = string.IsNullOrEmpty(SlowaZakazane)? new string[0] : SlowaZakazane.Split(separatory);
            string[] slowaWymagane = string.IsNullOrEmpty(SlowaWymagane)? new string[0] : SlowaWymagane.Split(separatory);
            HashSet<HistoriaDokumentu> nazwyDokumentow = listaWejsciowa.Keys.ToHashSet();
            HashSet<HistoriaDokumentu> doUsuniecia = new HashSet<HistoriaDokumentu>();
            if (slowaZakazane.Any())
            {
                foreach (HistoriaDokumentu dok in nazwyDokumentow)
                {
                    foreach (string s in slowaZakazane)
                    {
                        if (dok.NazwaDokumentu.Contains(s))
                            doUsuniecia.Add(dok);
                    }
                }
            }
            if (slowaWymagane.Any())
            {
                foreach (HistoriaDokumentu dok in listaWejsciowa.Keys)
                {
                    foreach (string s in slowaWymagane)
                    {
                        if (!dok.NazwaDokumentu.Contains(s))
                            doUsuniecia.Add(dok);
                    }
                }
            }
            foreach (HistoriaDokumentu dokument in doUsuniecia)
            {
                listaWejsciowa.Remove(dokument);
            }
        }

       
        [FriendlyName("Słowa które NIE MOGĄ wystąpić w nazwie dokumentu. Rozdzielone spacją")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Niewymagane]
        public string SlowaZakazane{ get; set; }

        [FriendlyName("Słowa które MUSZĄ wystąpić w nazwie dokumentu. Rozdzielone spacją")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Niewymagane]
        public string SlowaWymagane { get; set; }

    }
}
