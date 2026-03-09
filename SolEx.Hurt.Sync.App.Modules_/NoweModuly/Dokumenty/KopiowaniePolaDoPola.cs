using System.Collections.Concurrent;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    class KopiowaniePolaDoPola : SyncModul, IModulDokumenty
    {
        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Pole, z którego będzie skopiowana wartość")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(HistoriaDokumentu))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleZrodlowe { get; set; }

        [FriendlyName("Pole, do którego będzie skopiowana wartość z pola źródłowego")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(HistoriaDokumentu))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleDocelowe { get; set; }

        [FriendlyName("Czy kopiować jeśli w polu źródłowym jest null")]
        public bool KopiowacNulle { get; set; }
        public KopiowaniePolaDoPola()
        {
            PoleZrodlowe = "";
            PoleDocelowe = "";
        }

        public override string Opis
        {
            get { return "Automatyczne kopiowanie wartości jednego pola do drugiego."; }
        }

        public void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy, Dictionary<int, long> hashe, ref List<Klient> klienci)
        {
            if (string.IsNullOrEmpty(PoleZrodlowe) || string.IsNullOrEmpty(PoleDocelowe))
                return;

           KopiowaniePolHistoriaDokumentyBase.Przetworz(ref listaWejsciowa, CzyMoznaPrzetwarzac, PoleZrodlowe,PoleDocelowe, KopiowacNulle, new List<Klient>());
        }

        public bool CzyMoznaPrzetwarzac(List<Klient> klienciNaB2B, long idKlienta)
        {
            return true;
        }
    }
}
