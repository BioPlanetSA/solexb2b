using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IConfigBLL : IConfigSynchro
    {
        string OpisKategoriiProduktowSymbol { get; }
        string SzumyWyszukiwania { get; }
       
        bool UruchamianyNaSerwerze { get; }
        decimal DomyslnaDodawanaIloscKoszyka { get; }
        long IdDomyslnyOpiekun { get; set; }
        long IdDrugiDomyslnyOpiekun { get; set; }
        long IdDomyslnyPrzedstawiciel { get; set; }
        DateTime DokumentyOdKiedyPobierane { get; }

        int? ProduktyNieDostepnePrzezDluzszyCzasCecha { get; }
        string TypDomyslnyFiltru { get; }
        bool WyszukiwanieZamienniki { get; }

        bool DomyslnaZgodaNaNewsletter { get; }

        bool PokazujRabatyTylkoZWidocznychGrup { get; }

        bool PodpowiedziWyszukiwania { get; }
        string ZdjecieRozmiarWPowiadomieniach { get; }
        bool DodawajSposobDostawyJakoPozycje { get; }
        bool DodawajSposobDostawyDoUwag { get; }

        bool DodajIdKlientaDoTymczasowegoNumeruZamowienia { get; }
        string AdresStronyKoszyk { get; }

        List<Sortowanie> DostepneSortowanieKoszyka { get; }

        List<Sortowanie> PrzygotujSortowanie(string sort);

        HashSet<PolaListaProduktowNoweZamowienie> KtorePolaProduktuPowiadomienieNoweZamowienie { get; }
        HashSet<PolaListaProduktowNoweZamowienie> KtorePolaProduktuPowiadomienieDostepneProdukty { get; }
        string AktywneWidokiListyProduktowRodzinowych { get; }

        string SymbolCechyUlubione { get; }
        string SymbolCechyMojKatalog { get; }
        string SymbolCechyProduktZOferty { get; }
        string SymbolCechyAkcesoria { get; }
        string SymbolAtrybutCechyAutomatyczne { get; }
        string SymbolAtrybutCechyTypStanu { get; }

        int MailPrzerwaPoIluBledach { get; }

        int MailingIleNaRaz { get; }

        int EmailTimeout { get; }

        int EmailPort { get; }

        int MailingEmailTimeout { get; }

        int MailingEmailPort { get; }

        bool MailingEmailSzyfrowanie { get; }

        int MaksimumProbWyslaniaTegoSamegoMaila { get; }

        int MaksIleBledowPodczasPonownegoWysylaniaMaila { get; }
        string wlasciciel_AdresPlatformy { get; }
        int NiePokazujRodzinOdIlosciDzieci { get; }

        DateTime PobierzDateZmianaStatusu { get; }

        Dictionary<long, Waluta> SlownikWalut { get; }
        void UsunCacheWalut(IList<object> obj);

        int KategoriaKlientaNieBlokujZmianaIp { get; }

        string SzablonNiestandardowySciezkaBezwzgledna { get; }

        decimal WagaKoszykaGdyRownaZero { get; set; }
        string IkonaMapy { get; }

        string wlasciciel_adres_miasto { get; }

        string wlasciciel_nazwa { get; }

        string wlasciciel_adres_kod { get; }

        string wlasciciel_adres_ulica { get; }
        string WlascicielAdresTelefon { get; }
        int WlascicielAdresKraj { get; }

        int RabatZaokraglacDoIluMiejsc { get; }

        decimal MaxPokazywanyStan { get; }

        bool LimityOdCenyNetto { get; }

        long PobierzOddzialIDWgDomeny();

        HashSet<string> AktywneWidokiListyProduktow(bool czyKlientZalogowany);

        string PlikiDpPobraniaKatalogBazowyIkony { get; }

        bool TlumaczenieWLocie { get; }

        int CzasWaznosciLogowania { get; }

        bool DomyslnaWidocznoscPunktow { get; }

        string CzyszczenieStalychFiltrow { get; }

        string AdresStronyZProduktami { get; }

        List<Sortowanie> DostepneSortowanieListyProduktow { get; }

        KiedyWysylacMailaOZamowieniu KiedyWysylacMailaOZamowieniu { get; }

        TrybPokazywaniaFiltrow TrybPokazywaniaFiltrow { get; }

        bool TekstowySposobPokazywaniaPrzyciskuDoKoszyka { get; }

        bool UkryjJednostkiMiary { get; }

        bool ListyPrzewozoweDokumentPowiazany { get; }

        void ResetSystemNames();

        int ZIluDniWsteczWysylacListyPrzewozowe { get; }
        string IleProduktowPokazacNaStronie { get; }

        void RefreshData();

        SposobPokazywaniaDodatkowegoRabatu PokazywanieRabatu { get; }

        bool EppKodKresowyKod { get; }

        int IdDomyslnegoSablonuCsvDoEksportu { get; }

        string AdresStronyZProduktem { get; }

        bool KlientMozeZmianiacJezyk { get; }

        string SciezkaFavicon { get; }

        List<Ustawienie> ListaUstawien(bool widoczneTylkoDlaUzytkownika, int? idPracownika);

        Ustawienie Pobierz(string id);

        void AktualizacjaUstawien(Ustawienie u);

        void DodajUstawieniaZPropertisow();
        string ZnakWodnyTekst { get; }

        string ZnakWodnyObrazek { get; }
        

        int GodzinaPoczatekZakazIntegracji { get; }

        int GodzinaKoniecZakazIntegracji { get; }

        int SferaModulOkresZmianyMinuty { get; }

        HashSet<ElementyKoszykaPodglad> PolaKoszykaPodglad { get; }

        bool PokazywanieSortowaniaJednostek { get; }

        bool PokazujDymek { get; }

        //HashSet<string> PolaNaKarcieProduktuOznaczenia { get; }

        MiejsceOpisuZbiorczegoZKategorii OpisZbiorczyZKategorii { get; }

        bool AutomatyczneZatwierdzanieRejestracji { get; }

        string GoogleCaptchaKluczPrywatny { get; }

        Szerokosci SzerokoscMenuDlaArtykulow { get; }
        string ArtykulyFormatDaty { get; }
        string KluczGoogleAnalytycs { get; }
        string GoogleTranslateKey { get; }
        string GoogleApiKey { get; }
        bool PokazujOpiekunaWNaglowku { get; }

        string NazwaPlatformy(int jezykId);

        string Logo { get; }


        int CzasPodpowiedziSzukanie { get; }

        string MainCS { get; }

        string GoogleCaptchaKluczPubliczny { get; }

        void SprawdzJezyki();

        bool GetPokazywacRabatICeneWyjsciowa(IKlient klient);

        bool KomunikatIloscWOpakowaniuGdyJednosc { get; }

        int[] AtrybutyWSekcjiLogistykaPokazuj { get; }
        bool PokazujDodajDoKoszykaFaktura { get; }
        bool SzczegolyDokumentuKodKreskowyPokazuj { get; }
        bool SzczegolyDokumentuKodPokazuj { get; }

        bool PokazywanieUwagWPodgladzie { get; }
        ISettingCollection Settings { get; }

        bool SklepyMapaNazwaPokazuj { get; }
        bool SklepyMapaUlicaINrPokazuj { get; }
        bool SklepyMapaKodPocztowyPokazuj { get; }

        bool SklepyMapaMiastoPokazuj { get; }
        bool SklepyMapaTelefonPokazuj { get; }
        bool SklepyMapaWWWPokazuj { get; }
        bool SklepyMapaEmailPokazuj { get; }

        bool KomunikatIloscMinimalnaGdyJednosc { get; }

        Sortowanie SortowaniePozycjiPrzedZapisemZamowienia { get; }

        string SymbolStronyWylogowanie { get; }

        bool AdresyEmailWymagany { get; }
        bool AdresyTelefonWymagany { get; }

        Owner GetOwner();

        int Platnosci24IdKonta { get; }
        string Platnosci24KluczCrc { get; }
        long CechaMojKatalog { get; }
        CechyBll CechaUlubione { get; }
        long CechaAkcesoria { get; }
        Jezyk JezykDomyslny { get; }

        void UkryjUstawienia();
        void UsunLogi();
        void KasowaniePdfBezDokumentow();
        bool BlokujDodawanieDoKoszykaDlaProduktowZCenaZerowa { get; }
        bool ZastepujCeneZerowaIkonkaTelefonu { get; }
        ProduktyWirtualneProvider WirtualneProduktyProvider { get; }
        string SymbolStronylogowanie { get; }
        int GradacjeIleDniWsteczLiczyc { get; }

        /// <summary>
        /// okreœla czy s¹ aktywne gradacje
        /// </summary>
        bool GradacjeAktywne { get; }

        string SymbolCechyGradacja { get; }
        long CechaGradacja { get; }
        Dictionary<string, Magazyn> SlownikMagazynowPoSymbolu { get; }
        Dictionary<int, Magazyn> SlownikMagazynowPoId { get; }

        /// <summary>
        /// okreœla czy flat ceny sa w bazie - np. jesli system ERP liczyc gotowe ceny - jka nie ma takich cen to wylanczane sa moduly pobierania flat ceny
        /// </summary>
        bool CzyFlatCenyWBazieUzupelniane { get; }

        bool EmailSzyfrowanie { get; }
        string EmailFrom { get; }
        string MailingEmailFromPrzyjaznaNazwa { get; }

        void UstalUstawieniaStartowe();

        bool ZamykajKarteProduktuPoDodaniuDoKoszyka { get; }
        IDictionary<TypDanychIntegracja, List<PlikIntegracjiSzablon>> PobierzListePlikowIntegracji { get; }
        int MaksymalnaIloscPozycjiWKoszyku { get; }
        string RozmiarZdjeciaIndywidualizacja { get; }
        Dictionary<int, PoziomCenowy> SlownikPoziomowCenowych { get; }
        IDictionary<int, PlikIntegracjiSzablon> TablicaPlikowIntegracjiWgId { get; }
        HashSet<int> PlikiIntegracjiAktywne_DoPokazania { get; }
        string LinkAlternatywnyStronyProduktow { get; set; }
        HashSet<Licencje> Licenses { get; }
        IList<Ustawienie> PoprawUstawieniaPoSelect(int arg1, IKlient klient, IList<Ustawienie> listaUstawien, object arg4);
        Dictionary<string, OpisImportera> AktywneFormatyImportowaniaPlikow { get; }
        void UsunZalacznikRejestracji(IList<object> obj);

        decimal PobierzKursWalut(Waluta zWaluta, Waluta naWaluta);
        decimal PobierzKursWalut(string walutaCeny, string walutaKlienta);
    }
}