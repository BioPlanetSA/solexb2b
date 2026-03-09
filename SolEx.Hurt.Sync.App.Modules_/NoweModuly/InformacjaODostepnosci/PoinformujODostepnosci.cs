using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.InformacjaODostepnosci
{
     [ModulStandardowy]
    public class PoinformujODostepnosci : SyncModul, IInformacjaODostepnosci
    {
         public PoinformujODostepnosci()
         {
             MozeDzialacOdGodziny = 23;
             MozeDzialacDoGodziny = 7;
             IleMinutCzekacDoKolejnegoUruchomienia = 60*3;
         }

         public override string PokazywanaNazwa
         {
             get { return "Wyślij powiadomienia o pojawieniu się produktów na stanie"; }
         }

         public override string Opis
        {
            get { return "Wysyła powiadomienia o pojawieniu się na stanie produktów, na które czekali klienci. Uwaga! Moduł bardzo obciąża system - nie włączać w godzinach pracy"; }
        }

        public void Przetworz()
        {
            ApiWywolanie.WyslijPowiadomieniaODostepnosci();
        }

        public override string uwagi
        {
            get { return ""; }
        }
    }
}
