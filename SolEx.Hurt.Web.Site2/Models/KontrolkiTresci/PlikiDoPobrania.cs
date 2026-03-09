using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class PlikiDoPobrania:KontrolkaTresciBaza
    {
        public PlikiDoPobrania()
        {
            KatalogPlikowWZasoby = "Pliki";
            NieWyswietlajPodkatalogow = false;
            PokazujDate = false;
            pokazujNaglowek = true;
        }

        public override string Nazwa
        {
            get { return "Pliki do pobrania"; }
        }

        public override string Kontroler
        {
            get { return "Tresci"; }
        }

        public override string Akcja
        {
            get { return "Pliki"; }
        }

        public override string Opis
        {
            get { return "Kontrolka wyświetlająca pliki do pobrania"; }
        }

        [FriendlyName("Katalog z plikami")]
        [WidoczneListaAdmin(true,true,true,true)]
        [Wymagane]
        [PobieranieSlownika(typeof(SlownikKatalogZasoby))]
        public string KatalogPlikowWZasoby { get; set; }

        [FriendlyName("Nie wyświetlaj podkatalogów")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool NieWyswietlajPodkatalogow { get; set; }

        [FriendlyName("Pokazuj datę pliku")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazujDate { get; set; }

        [FriendlyName("Pokazuj nagłówek nad tabelą plików")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool pokazujNaglowek { get; set; }

        [FriendlyName("Sortowanie")]
        [WidoczneListaAdmin(true, true, true, true)]
        public SortowaniePlikow Sortowanie { get; set; }
    }
}