using System.Collections.Generic;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    // WAŻŃE - jak masz zamian zmienić nazwę kontrolki musisz pamietać o zmianie skryptu ts. W pliku _listaProduktow.ts oraz _ustawieniaProfilKlienta pobieramy tylko elementy które posiadają parenta z klasą kontrolka-ListaProduktow. Jak nie zmienisz w pliku ts lista produktów przestanie działać
    public class ListaProduktow : ListaProduktowBaza, IZastepczaNazwaWartosc
    {
        public ListaProduktow()
        {
            KalfeKategorii = PokazywanieKafliKategorii.NiePokazuj;
            PrzepelnienieNaLiscie = false;
       
            PokazujFiltryGdyBrakKryteriowProduktow = false;

            base.PokazujKoszyk = true;
            this.PokazujKoszyk = true;
            FiltryWybraneNadLista = false;
        }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Stała wysokość okna listy produktów z suwakiem",FriendlyOpis = "efekt tzw. przepełnienia")]
        [GrupaAtttribute("Globalne", 3)]
        public bool PrzepelnienieNaLiscie { get; set; }

        [NieWysylajParametryDoAkcji]
        public override string Grupa => "Produkty";

        [NieWysylajParametryDoAkcji]
        public override string Nazwa => "Lista produktów";

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kategoria produktów")]
        [PobieranieSlownika(typeof(SlownikKategoriiProduktow))]
        [GrupaAtttribute("Globalne", 3)]
        public int? Kategoria { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Kafle kategorii")]
        [GrupaAtttribute("Globalne", 3)]
        public PokazywanieKafliKategorii KalfeKategorii { get; set; }

        [FriendlyName("Rozmiar dla kafla kategorii")]
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Globalne", 3)]
        [PobieranieSlownika(typeof(SlownikPresetowDoZdjec))]
        public string RozmiarKaflaKategorii { get; set; }

        [NieWysylajParametryDoAkcji]
        public override string Kontroler => "Produkty";

        [NieWysylajParametryDoAkcji]
        public override string Akcja => "Lista";

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Symbol treści z szablonem opisu kategorii.", FriendlyOpis = " Ustawienie nadpisuje identyczne ustawienie z grupy produktowej.")]
        [PobieranieSlownika(typeof(SlownikTresciSystemowych))]
        public string SymbolOpisKategorii { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Treść pokazywana zamiast opisu kategorii.", FriendlyOpis = "Treść jest pokazywana gdy opis kategorii nie występuje lub nie zdefiniowany 'Symbol treści z szablonem opisu kategorii' ")]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string OpisProduktowZamiastOpisuKategorii { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazyć filtry gdy brak kryteriów")]
        public bool PokazujFiltryGdyBrakKryteriowProduktow { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Przyciski do przechodzenia do kategorii nadrzędnej i do wszystkich produktów. ", FriendlyOpis = "Przyciski są pokazywane pod listą produktów")]
        public bool PrzyciskiNadrzednaKategoria { get; set; }

        [FriendlyName("Tekst jeśli nie ma produktów")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string TextZastepczy { get; set; }

        public string NazwaZastepcza { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazuj listę produktów dopiero po wybraniu atrybutów")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [GrupaAtttribute("Lista", 2)]
        [Niewymagane]
        public HashSet<int> IdAtrybutow { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Sposób sprawdzenia czy wymagane atrybuty są wybrane")]
        [GrupaAtttribute("Lista", 2)]
        [Niewymagane]
        public SposoobSprawdzeniaFiltrow SposoobSprawdzeniaFiltrow { get; set; }

        [WidoczneListaAdmin(false, false, false, false)]
        [FriendlyName("Czy pokazywać koszyk")]
        public new bool PokazujKoszyk { get; set; }

        [WidoczneListaAdmin(false, false, true, false)]
        [FriendlyName("Filtry wybrane pokazuj zawsze i nad listą produktów")]
        public bool FiltryWybraneNadLista { get; set; }
       
    }
}