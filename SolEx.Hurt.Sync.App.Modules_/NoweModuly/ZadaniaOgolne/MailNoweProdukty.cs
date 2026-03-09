using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ZadaniaOgolne
{
         [ModulStandardowy]
    public class MailNoweProdukty : SyncModul, IZadaniaOglone
    {
        public MailNoweProdukty()
        {
            this.MozeDzialacOdGodziny = 22;
            this.MozeDzialacDoGodziny = 24;
            this.IleMinutCzekacDoKolejnegoUruchomienia = 60*24;
            CechyMusiPosiadac = null;
            CechyNieMozePosiadac = null;
            WysylajDoSubkont = true;
        }
             
        [FriendlyName("Lista cech, które musi mieć towar (towar musi posiadać wszystkie cechy")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<long> CechyMusiPosiadac { get; set; }

        [FriendlyName("lista cech, które nie może mieć towar (towar musi posiadać wszystkie cechy")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<long> CechyNieMozePosiadac { get; set; }

        [FriendlyName("Czy powiadomienie ma być wysyłane również do subkont")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool WysylajDoSubkont { get; set; }

        //[FriendlyName("Maksymalnie ile produktow moze byc w mailu o nowych produktach, maile z więszą ilością nie są wysyłane")]
        //[Niewymagane]
        //[WidoczneListaAdminAttribute(false, false, true, false)]
        //public int MaxIleProduktowMailNoweProdukty { get; set; }

        public override string uwagi
        {
            get { return "Wysyła maile o nowych produktach, konfiguracja modułu w ustawieniach. " +
                            "Wysyłane są tylko produkty przypisane do kategorii, mające cene > 0, z zdjęciem." +
                            "" +
                            "Uwaga! Moduł bardzo obciąża system - nie można go uruchamiać w godzinach pracy."; }
        }
        public override string PokazywanaNazwa
        {
            get { return "Wyślij maila o nowych produktach do klientów"; }
        }

        public void Przetworz(Model.Interfaces.Sync.ISyncProvider provider)
        {
            ApiWywolanie.WyslijMaileNoweProdukty(CechyMusiPosiadac, CechyNieMozePosiadac, WysylajDoSubkont);
        }
    }
}
