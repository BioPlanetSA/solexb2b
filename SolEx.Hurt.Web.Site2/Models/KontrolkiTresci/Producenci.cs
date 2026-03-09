using System.Web;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class Producenci : KontrolkaTresciBaza
    {
        public Producenci()
        {
            TylkoZObrazkami = false;
            PokazujNazwe = true;
            KategorieProduktoweSposobRenderowania = ProducenciSposobRenderowania.ListaKafle;
            OgraniczajKategorieWgStalychFiltrowISzukania = false;
            PokazujPodkategorie = true;
        }

        public override string Nazwa => "Kategorie produktów";

        public override string Kontroler => "KategorieProduktowe";

        public override string Akcja => "Kategorie";

        [FriendlyName("Tylko z obrazkami",FriendlyOpis = "Pokazuje kategorie które posiadają obrazki")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool TylkoZObrazkami { get; set; }

        [FriendlyName("Pokazuj nazwę kategorii")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazujNazwe { get; set; }

        [FriendlyName("Rozmiar kafla")]
        [PobieranieSlownika(typeof(SlownikPresetowDoZdjec))]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        public string RozmiarKafla { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Symbol treści z szablonem opisu grupy")]
        [PobieranieSlownika(typeof(SlownikTresciSystemowych))]
        public string SymbolOpisGrupy { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Dodatkowe klasy css  dla poszczególnych kategorii")]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlassCss,SolEx.Hurt.Core")]
        public string[] DodatkoweKlasyCssElementyKontrolki { get; set; }

        private int? _grupaid;

        [FriendlyName("Grupa do pokazania")]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof (SlownikGrupy))]
        [Niewymagane]
        public int? GrupaDoPokazaniaId
        {
            get
            {
                if (_grupaid == null)
                {
                    if (HttpContext.Current.Request.Url.AbsoluteUri.Contains("Admin"))
                    {
                        return null;
                    }
                    object gdip = PobierzIdentyfikator("gpid", false);
                    if (gdip == null)
                    {
                        throw new HttpException(500,"Nie znaleziono id grupy");
                    }
                    int grupaid = int.Parse(gdip.ToString());
                    return grupaid;
                }
                return _grupaid;
            }
            set
            {
                _grupaid = value;
            }
        }

        [FriendlyName("Sposób renderowania kategorii produktowych")]
        [WidoczneListaAdmin(true, true, true, true)]
        public ProducenciSposobRenderowania KategorieProduktoweSposobRenderowania { get; set; }

        [FriendlyName("Cyz pokazywać kategorie ograniczone stałymi filtrami i innymi warunkami jakie klient wybrał.", FriendlyOpis = "Jeśli kategorie mają być pokazywane w menu powino być na NIE")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool OgraniczajKategorieWgStalychFiltrowISzukania { get; set; }

        [FriendlyName("Cyz pokazywać podkategorie")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazujPodkategorie { get; set; }


        public enum ProducenciSposobRenderowania
        {
            ListaKolumn = 1,
            ListaKafle = 2
        }
    }
}