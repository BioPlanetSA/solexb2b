using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    class PrzepisanieOdbiorcyJakoKlienta : SyncModul, IModulDokumenty
    {
        public override string uwagi
        {
            get { return ""; }
        }



        private string poleZrodlowe = "OdbiorcaId";
        private string poleDocelowe = "klient_id";
        private bool kopiowacNulle = false;

        public override string Opis
        {
            get { return "Automatyczne kopiuje id odbiorcy jako id klienta w dokumencie."; }
        }

        public void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy, Dictionary<int, long> hashe, ref List<Klient> klienci)
        {
            List<Klient> klienciNaB2B = ApiWywolanie.PobierzKlientow().Values.ToList();
            KopiowaniePolHistoriaDokumentyBase.Przetworz(ref listaWejsciowa, CzyMoznaPrzetwarzac, poleZrodlowe, poleDocelowe, kopiowacNulle, klienciNaB2B);
        }

        public bool CzyMoznaPrzetwarzac(List<Klient> klienciNaB2B, long idKlienta)
        {
            return klienciNaB2B.Any(a => a.Id == idKlienta);
        }

       

    }
}
