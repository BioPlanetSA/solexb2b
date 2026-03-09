using System.Collections.Generic;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class WszystkieAtrybutyProduktu : SzczegolyProduktuBaza, INaglowekStopka, IZastepczaNazwaWartosc, IPoleJezyk
    {
        private string _nazwaZastepcza;

        public override string Nazwa
        {
            get { return "Wszystkie Atrybut"; }
        }

        public override string Opis
        {
            get { return "Wyświetla wszystkie atrybuty przypisane do produktu"; }
        }

        public override string Akcja
        {
            get { return "WszystkieAtrybuty"; }
        }

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

        public string NazwaZastepcza
        {
            get { return _nazwaZastepcza; }
            set { _nazwaZastepcza = value; }
        }

        [FriendlyName("Atrybuty któych NIE pokazywać (pomijać)")]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [Niewymagane]
        public int[] AtrybutyDoPominiecia { get; set; }

        public int JezykId { get; set; }
    }

}