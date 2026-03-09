using SolEx.Hurt.Core;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class PoleProduktu : SzczegolyProduktuBaza, INaglowekStopka, IZastepczaNazwaWartosc, IFormatowanie, IPoleJezyk
    {
       public PoleProduktu()
        {
            PokazNazwe = false;
            Formatowanie = "";
            LinkowanieCech = false;
        }

        public override string Nazwa
        {
            get { return "Pole Produktu"; }
        }

        public override string Opis
        {
            get { return "Wyświetla wybrane pole produktu"; }
        }

        public override string Akcja
        {
            get { return "PoleProduktu"; }
        }

        [WidoczneListaAdmin(true,true,true,true)]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(ProduktKlienta),true)]
        [Niewymagane]
        public string[] Pola { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [Niewymagane]
        public int[] Atrybuty { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazuj nazwy",FriendlyOpis = "Pokazyje nazwę atrybutu, nazwę pola której wartość jest wyświetlana.")]
        public bool PokazNazwe { get; set; }

        [FriendlyName("W jakim układzie pokazywać")]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikWidokowSzczegolowProduktow))]
        public string Uklad { get; set; }

        [FriendlyName("Nagłówek sekcji")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string Naglowek { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [FriendlyName("Stopka",FriendlyOpis = "Stopka dla pola")]
        [Lokalizowane]
        public string Stopka { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [FriendlyName("Tekst gdy pole nie ma wartości",FriendlyOpis = "Tekst który ma się pokazać jeśli kategoria nie ma wprowadzonej wartości do wybranego pola")]
        [Lokalizowane]
        public string TextZastepczy { get; set; }

        [FriendlyName("Nazwa pola wyświetlana - działa tylko przy wybranym jednym polu")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string NazwaZastepcza { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [FriendlyName("Formatowanie wartości pól tekstowych - za wartość podstawiasz {0}",FriendlyOpis = "Wypełniamy jesli chcemy innaczej wyświetlić wartość")]
        [Lokalizowane]
        public string Formatowanie { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [FriendlyName("Linkuj wybrane cechy do listy produktów",FriendlyOpis = "Linkuj wybrane cechy do listy produktów. Jeśli na podstawie atrybutu są budowane kategorie, linki będą do kategorii")]
        public bool LinkowanieCech { get; set; }

        public int JezykId { get; set; }
    }
}
