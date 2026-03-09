namespace SolEx.Hurt.Model.Enums
{
    public enum ElementySynchronizacji
    {
        [OpisZadanSynchronizacji("Eksport produktów powinien być uruchamiany co ok 120 min")] [GrupaZadanSynchronizacji("Eksport produktów")] Produkty = 0,
        [OpisZadanSynchronizacji("Eksport płatności powinien być uruchamiany co ok 30 min")] [GrupaZadanSynchronizacji("Eksport klientów")] Platnosci = 1,
        [OpisZadanSynchronizacji("Eksport kategorii klientów powinien być uruchamiany co ok 30 min")] [GrupaZadanSynchronizacji("Eksport klientów")] KategorieKlientów = 2,
        [OpisZadanSynchronizacji("Eksport klientów powinien być uruchamiany co ok 120 min")] [GrupaZadanSynchronizacji("Eksport klientów")] Klienci = 3,
        [OpisZadanSynchronizacji("Eksport dokumentów powinien być uruchamiany co ok 120 min")] [GrupaZadanSynchronizacji("Eksport dokumentów")] Dokumenty = 4,
        [OpisZadanSynchronizacji("Eksport rabatów powinien być uruchamiany co ok 120 min")] [GrupaZadanSynchronizacji("Eksport cen i rabatów")] Rabaty = 5,
        [OpisZadanSynchronizacji("Eksport kategorii produktów powinien być uruchamiany co ok 120 min")] [GrupaZadanSynchronizacji("Eksport produktów")] KategorieProduktów = 6,
        [OpisZadanSynchronizacji("Eksport cech i atrybutów powinien być uruchamiany co ok 120 min")][GrupaZadanSynchronizacji("Eksport Cech i Atrybutów")] CechyIAtrybuty = 7,
        [OpisZadanSynchronizacji("Import zamówień powinien być uruchamiany co ok 5 min")] [GrupaZadanSynchronizacji("Import zamówień")] ImportZamówień = 8,
        [OpisZadanSynchronizacji("Eksport stanów produktów powinien być uruchamiany co ok 10 min")] [GrupaZadanSynchronizacji("Eksport stanów produktów")] StanyProduktów = 9,
        Reklamacje = 10,
        [OpisZadanSynchronizacji("Eksport cen powinien być uruchamiany co ok 60 min")] [GrupaZadanSynchronizacji("Eksport cen i rabatów")] PoziomyCenowe = 11,
        [OpisZadanSynchronizacji("Info o dostępności powinno być uruchamiany raz/ dwa razy w nocy")] [GrupaZadanSynchronizacji("Eksport stanów produktów")] InformacjaODostepnosci = 12,

        [GrupaZadanSynchronizacji("Eksport dokumentów")] [OpisZadanSynchronizacji("Wysyłanie pdf powininno być uruchamiane co ok 120 min")] WysylanieFakturPDF = 13,
        [OpisZadanSynchronizacji("Eksport zdjęć powinno być uruchamiany co ok 120 min")] [GrupaZadanSynchronizacji("Eksport plików i zdjęć")] ImportZdjec = 14,
        [OpisZadanSynchronizacji("Eksport lisów przewozowych powinien być uruchamiane co ok 120 min")] [GrupaZadanSynchronizacji("Eksport dokumentów")] ListyPrzewozowe = 15,
        [OpisZadanSynchronizacji("Import/Eksport CSV/XML powininno być uruchamiany raz/ dwa razy dziennie")] [GrupaZadanSynchronizacji("Import/Eksport CSV/XML")] ImportEksportXMLCSV = 16,
        [OpisZadanSynchronizacji("WYCOFANE Aktualizacja produktów powininno być uruchamiany raz/ dwa razy dziennie")] [GrupaZadanSynchronizacji("Aktualizacja produktów")] AktualizacjaProduktow = 17,
        [OpisZadanSynchronizacji("Import rejestracji powininno być uruchamiany co ok 30 min")] [GrupaZadanSynchronizacji("Eksport klientów")] ImportRejestracji = 18,
        [OpisZadanSynchronizacji("Liczenie gotowych cen powininno być uruchamiane co ok 500 min")] [GrupaZadanSynchronizacji("Eksport cen i rabatów")] WyliczenieGotowychCen = 19,
        [OpisZadanSynchronizacji("Różne zadania nie pasujące gdzie indziej. Zadania ogólne powinny być uruchamiane co ok 90 min")] [GrupaZadanSynchronizacji("Zadania ogólne")] ZadaniaOgolne = 20
    }
}
