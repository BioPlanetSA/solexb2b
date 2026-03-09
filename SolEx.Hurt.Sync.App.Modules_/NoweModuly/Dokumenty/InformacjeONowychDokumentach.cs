using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    [FriendlyName("Wyślij powiadomienie o nowych dokumentach")]
    public class InformacjeONowychDokumentach : SyncModul, Model.Interfaces.SyncModuly.IZadaniaOglone
    {
        public InformacjeONowychDokumentach()
        {
           MozeDzialacOdGodziny = 15;
           MozeDzialacDoGodziny = 20;
           IleMinutCzekacDoKolejnegoUruchomienia = 60*24;
        }
        //public override string PokazywanaNazwa
        //{
        //    get { return "Wyślij powiadomienie o nowych dokumentach"; }
        //}

        public override string Opis
        {
            get { return "Wyslanie informacji o pojawieniu sie nowych dokumentów."; }
        }
        [FriendlyName("Kategorie klientów do których mają być wysłane maila. Jeżeli wiadomości maja być wysłane do wszystkich klientów pole to pozostaw puste")]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Kategorie { get; set; }

        public HashSet<int> IdKategorii
        {
            get
            {
                if (Kategorie!=null && Kategorie.Any() && Kategorie[0] != "")
                {
                    return new HashSet<int>( Kategorie.Select(int.Parse) ) ;
                }
                return new HashSet<int>();
            }
        } 
        public override string uwagi
        {
            get { return ""; }
        }

        public void Przetworz(Model.Interfaces.Sync.ISyncProvider provider)
        {
            ApiWywolanie.WysylaniePowiadomienONowychFakturach(IdKategorii);
        }
    }
}
