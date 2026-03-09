using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ZadaniaOgolne
{
     [ModulStandardowy]
   public class MailePowiatalneWysylanie : SyncModul, IZadaniaOglone
    {
         public MailePowiatalneWysylanie()
         {
             MozeDzialacOdGodziny = 5;
             MozeDzialacDoGodziny = 7;
             IleMinutCzekacDoKolejnegoUruchomienia = 60*24;
         }

        public override string uwagi
        {
            get { return "Wysyła maile powitalne od szefa, konfiguracja w ustawieniach"; }
        }

         public override string PokazywanaNazwa
         {
             get { return "Wyślij powiadomienia od szefa dla nowych klientów"; }
         }

         public void Przetworz(Model.Interfaces.Sync.ISyncProvider provider)
        {
            ApiWywolanie.WyslijMailePowiatalne();
        }
    }
}
