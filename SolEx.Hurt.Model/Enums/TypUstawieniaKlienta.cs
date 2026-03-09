using System.ComponentModel.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    /// <summary>
    /// typ ustawienia czyli do czego odnosi się ustawienie (Układ komumn, filtry)
    /// </summary>
    public enum TypUstawieniaKlienta
    {
        [FriendlyName("Układ kolumn")]
        UkladKolumn = 1,
        [FriendlyName("Stały filtr")]
        StalyFiltr = 2,
        [FriendlyName("Czy ukrywac ceny hurtowe")]
        UkryjCenyHurtowe = 3,
        //[FriendlyName("Aktualny jezyk")]
        //AktualnyJezykId = 4,
        [FriendlyName("Czy filtry sa widoczne")]
        WidocznoscFiltrow = 5,
        [FriendlyName("Wybrany koszyk")]
        WybranyKoszyk = 6,
        [FriendlyName("Powiadomienia mailowe")]
        PowiadomieniaMailowe = 7,
        [FriendlyName("Szablon listy produktów")]
        SzablonListy = 10,
        [FriendlyName("Ilosc produktów na stronie")]
        RozmiarStronyListaProduktow=15,
        [FriendlyName("Sposób sortowania jednostek")]
        SposobSortowaniaJednostek = 20,
        [FriendlyName("Kolumna sortowania dokumentow")]
        KolumnaSortowaniaDokumentow = 22,
        [FriendlyName("Kolumna sortowania Listy produktów")]
        KolumnaSortowaniaListyProduktow = 23,

        [FriendlyName("Wyswietlaj dokumenty tylko niezapłacone")]
        DokumentyTylkoNiezaplacone = 24,
        [FriendlyName("Wyswietlaj dokumenty tylko przeterminowane")]
        DokumentyTylkoPrzeterminowane = 25,

        [FriendlyName("Z ilu dni domyslnie pokazywac dokumenty Faktura")]
        DokumentyZIluDniDomyslniePokazywacFaktura = 66,

        [FriendlyName("Z ilu dni domyslnie pokazywac dokumenty Zamowienie")]
        DokumentyZIluDniDomyslniePokazywacZamowienie = 67,

        [FriendlyName("Z ilu dni domyslnie pokazywac dokumenty Oferta")]
        DokumentyZIluDniDomyslniePokazywacOferta = 68,

        [FriendlyName("Wyswietlaj dokumenty tylko niezrealizowane")]
        DokumentyTylkoNiezrealizowane = 27,

        [FriendlyName("Lista kolumn w adminie")]
        ListaKolumnWAdminie = 30, 
        [FriendlyName("Szablon listy kolumn w adminie")]
        ListaKolumnWAdminieSzablon = 31,
        [FriendlyName("Rozmiar strony listy w adminie")]
        ListaKolumnWAdminieRozmiarStrony = 32,
        [FriendlyName("Sortowanie Listy kolumn w adminie")]
        ListaKolumnWAdminieSortowanie = 33,

        [FriendlyName("Ostatnio wybrany import pozycji koszyka")]
        KoszykOstatnioWybranyImportPozycji = 40,

        [FriendlyName("Kolumna sortowania Listy w koszyku")]
        KolumnaSortowaniaKoszykLista = 41,
    }
}
