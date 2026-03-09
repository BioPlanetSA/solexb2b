using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Controllers;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    [FriendlyName(FriendlyOpis = "Kontrolka pokazująca pliki integracji. Zobacz koniecznie ustawienie Integracja - 'pliki integarcji możliwe do pobierania przez klientów' aby ustawić jakie pliki klient może zobaczyć / pobierać.")]
    public class Integracja: KontrolkaTresciBaza, IPoleJezyk
    {
        public Integracja()
        {
            OpisIntegracjiDlaKlienta = "Poniżej znajdują się pliki z naszą bazą danych gotowe do integracji z sklepami internetowymi lub innymi systemami sprzedaży. <br/>" +
                                       "Pliki dostępne są w kilku różnych formatach. " +
                                       "pliki XML są pełniejsze z uwagi na lepsze możliwości techniczne, dlatego zalecamy do korzystanie z tych plików. </br>Najpopularniejsze formaty to:<ul>" +
                                       "<li>CSV - może być otwierany i edytowany w MS Excel lub OpenOffice</li>" +
                                       "<li>XML i JSON - dedykowane dla informatyków</li>" +
                                       "<li>EPP - przeznaczony dla programu Subiekt GT, Nexo</li>" +
                                       "</ul>" +
                                       "<br/>" +
                                       " " +
                                       "Zanim zaczną Państwo korzystać z plików prosimy o zapoznanie się z najważniejszymi sugestiami: <ul>" +
                                       "<li>Import i synchronizacja produktów powinna być dokonywana wyłącznie wg kodów EAN lub ID produktu (nazwa i kod produktu może ulec zmianie)</li>" +
                                       "<li>Nie rozpowszechniaj dowolnie plików w internecie. Pliki udostępniamy tylko dla wybranych klientów - nie chcemy żeby były dostęne dla każdego</li>" +
                                       "<li>Nie jesteś anonimowy - każde pobieranie pliku jest odnotowane (włącznie z adresem IP komputera)</li>" +
                                       "<li>Pliki mogą zawierać Twoje ceny zakupu oraz ceny detaliczne - możesz ustawiać swoją cenę sprzedaży w oparciu o te ceny</li>" +
                                       "<li>Pliki są aktualizowane kilka razy dziennie - może zdarzyć się sytuacja kiedy sprzedasz produkt, który w pliku jeszcze jest na stanie, ale na naszym magazynie już go nie będzie</li>" +
                                       "<li>Pliki można pobierać automatycznie - potrzebujesz w tym cely klucza API - instrukcja jak go uzyskać jest dostępna na dole tej strony</li>" +
                                       "</ul> <br/><br/>";
        }

        public override string Nazwa
        {
            get { return "Wybór plików integracji dla klienta"; }
        }

        public override string Kontroler
        {
            get { return "Integracja"; }
        }

        public override string Akcja
        {
            get { return "ListaPlikowIntegracji"; }
        }       

        [FriendlyName("Tekst o integracji dla klienta - pokazywany nad kontrolką")]
        [Lokalizowane]
        [WidoczneListaAdmin(true, true, true, true)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string OpisIntegracjiDlaKlienta { get; set; }

        public int JezykId { get; set; }
    }
}