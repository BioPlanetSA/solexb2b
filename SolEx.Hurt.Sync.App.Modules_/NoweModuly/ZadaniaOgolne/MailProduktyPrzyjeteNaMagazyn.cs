using System.Collections.Generic;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.ZadaniaOgolne
{
    [ModulStandardowy]
    [FriendlyName("MailProduktyPrzyjeteNaMagazyn - cykliczne sprawdzenie produktów przyjętych na magazyn jeśli wcześniej ich stan nie przekraczał odpowiedniej ilości oraz wysłanie maila ze zmianami stanów produktów do klientów",
        FriendlyOpis = "Przygotowuje plik .json z produktami, których stan zwiększył się o przeliczoną liczbę wg ustawień modułu. " +
                       "(wysyłanie maila powinno być włączone do kogokolwiek). " +
                       "Wysyłane są tylko produkty przypisane do kategorii, spełniające ewentualne wymagania dotyczące cech produktów (wymagane i zakazane). " +
                       "Uwaga! Moduł bardzo obciąża system - nie można go uruchamiać w godzinach pracy - zalecane godziny: 5-6. " +
                       "Zalecane włączenie modułu dopiero PO synchronizacji stanów produktów." +
                       "Minimalna cykliczność uruchamiania modułu: 1 dzień (1440 minut). Przerwy to wielokrotności tej liczby minut np. tydzień = 7 * 1440 = 10080")]
    public class MailProduktyPrzyjeteNaMagazyn : SyncModul, IZadaniaOglone
    {
        public MailProduktyPrzyjeteNaMagazyn()
        {
            this.MozeDzialacOdGodziny = 5;
            this.MozeDzialacDoGodziny = 6;
            this.IleMinutCzekacDoKolejnegoUruchomienia = 60 * 24;
            this.CechyKonieczne = null;
            this.CechyZabronione = null;
            this.IdMagazynow = null;
            this.MinimalneZwiekszenieStanuPrzelicznik = 2m;
            this.MinimalnaIloscBrakuPrzelicznik = 0m;
        }

        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikCech))]
        [FriendlyName("Wysyłaj produkty tylko z poniżej wybranymi cechami")]
        [WidoczneListaAdmin(true, true, true, false)]
        public List<long> CechyKonieczne { get; set; }

        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikCech))]
        [FriendlyName("Nie wysyłaj produktów z poniżej wybranymi cechami")]
        [WidoczneListaAdmin(true, true, true, false)]
        public List<long> CechyZabronione { get; set; }

        [PobieranieSlownika(typeof(SlownikMagazynow))]
        [FriendlyName("Ilość produktów będzie sprawdzana z tych magazynów (wymagany conajmniej jeden magazyn)")]
        [WidoczneListaAdmin(true, true, true, false)]
        public List<int> IdMagazynow { get; set; }

        [FriendlyName("Współczynnik określający minimalne zwiększenie stanu produktów wg stanu minimalnego - " +
                      "jeśli wpisana wartość = 0.1, to oznacza, że najmniejsza zmiana stanu brana pod uwagę wynosi 10% stanu minimalnego;" +
                      "jeśli produkt nie ma stanu minimalnego to wymuszany jest stan minimalny wynoszący 1." +
                      "Wpisana wartość musi być większa od 0 !")]
        [WidoczneListaAdmin(true, true, true, false)]
        public decimal MinimalneZwiekszenieStanuPrzelicznik { get; set; }

        [FriendlyName("Współczynnik określający jaki stan produktu jest uważany za brak produktu - " +
                      "jeśli wpisana wartość = 0.5, to oznacza, że każdy produkt, którego ilość nie przekracza 50% stanu minimalnego będzie uważany za brakujący;" +
                      "jeśli wpisana wartość = 0, to oznacza, że tylko produkty, których nie ma wcale (ilość = 0) będą uważane za brakujące")]
        [WidoczneListaAdmin(true, true, true, false)]
        public decimal MinimalnaIloscBrakuPrzelicznik { get; set; }

        public void Przetworz(ISyncProvider provider)
        {
            this.ApiWywolanie.WyslijMaileProduktyPrzyjeteNaMagazyn(this.CechyKonieczne, this.CechyZabronione, this.MinimalneZwiekszenieStanuPrzelicznik, this.MinimalnaIloscBrakuPrzelicznik, this.IdMagazynow);
        }
    }
}
