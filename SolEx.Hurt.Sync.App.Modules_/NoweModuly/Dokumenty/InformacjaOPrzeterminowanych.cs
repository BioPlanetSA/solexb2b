using System.Collections.Concurrent;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    [FriendlyName("Wyślij maile o przeterminowanych fakturach", 
        FriendlyOpis = "Wysłanie informacji zbliżających sie terminach płatności i o przeterminowanych fakturach (powiadomienie mailowe o przeterminowanych dokumentach).")]
    public class InformacjaOPrzeterminowanych:  SyncModul, Model.Interfaces.SyncModuly.IModulDokumenty
    {
        [FriendlyName("Kategorie klientów dla których nie będzie wysyłane info o przeterminowanych płatnościach")]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false)]
        public List<int> KategoriaKlientaNieWysylaj { get; set; }

        [FriendlyName("Kategorie klientów dla których będzie wysyłane info o przeterminowanych płatnościach")]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false)]
        public List<int> KategoriaKlientaWysylaj { get; set; }

        [FriendlyName("Termin płatności dokumentu jest w ciągu najbliższych x dni. Wprowadzenie 0 powoduje brak wysłania niezapłaconych dokumentów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int IleDniPrzed { get; set; }
        
        [FriendlyName("Mineło już conajmniej x dni o terminu płatności.Wprowadzenie 0 powoduje wysłanie wszystkich przeterminowanych dokumentów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int IleDniPo { get; set; }
        
        [FriendlyName("Powtarzaj wysyłanie nie zapłaconych maili co X dni. Pozostawienie pustego pola oznacza brak powtarzania. Uwaga! zalecana wartość to przynajmniej 7 dni!!")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int? CoIleDniPonowneWyslanie { get; set; }
        public void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy, Dictionary<int, long> hashe,ref List<Klient> klienci )
        {
            ApiWywolanie.WyslijPowiadomieniaOTerminiePlatnosci(IleDniPrzed, IleDniPo,CoIleDniPonowneWyslanie, KategoriaKlientaNieWysylaj,KategoriaKlientaWysylaj);
        }
        
        public override string uwagi => "";
    }

}
