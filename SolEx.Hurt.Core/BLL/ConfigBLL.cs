using log4net;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery;
using SolEx.Hurt.Core.Importy;
using SolEx.Hurt.Core.Importy.Koszyk;
using Owner = SolEx.Hurt.Core.ModelBLL.Owner;
using StringExtensions = ServiceStack.Text.StringExtensions;

namespace SolEx.Hurt.Core.BLL
{
    

    public partial class ConfigBLL : LogikaBiznesBaza, IConfigBLL
    {
        private ILog Log => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void UstawUstawieniaSynchronizatora()
        {
            string providerProduktyWirtualne = Settings.GetSettingSlownikRefleksja<SlownikKlasProviderowWirtualnychProduktow, string>("provider_wirtualne_produkty", new HashSet<string>(), true, "Provider używany do budowy wirtualnych produktów", ustawieniaGrupa.Produkty).FirstOrDefault();

            if (!string.IsNullOrEmpty(providerProduktyWirtualne))
            {
                Type typObiektu = Type.GetType(providerProduktyWirtualne, true);
                this.WirtualneProduktyProvider = Activator.CreateInstance(typObiektu) as ProduktyWirtualneProvider;
            }

            //uzupelniamy te propertisy zeby bylo szybciej - doceleowo wywalimy obecne ustawienia  - wartości IDENTYCZNE dla zalogowanych i niezalogowanych - dlaetgo tak mozna zrobić
            this.ProduktyNieDostepnePrzezDluzszyCzasCecha =
                Settings.GetSettingSlownikRefleksja<SlownikCech, int?>("produkt-niedostepny-dluzszy-czas-cecha", new HashSet<int?>(), true, "Cecha, która powoduje, że produkt jest niedostępny przez dłuższy czas", ustawieniaGrupa.Koszyk)
                    .FirstOrDefault();

            this.ProduktyNaZamowienieCechaID =
                Settings.GetSettingSlownikRefleksja<SlownikCech, int?>("produkty-na-zamowienie-cecha-id", new HashSet<int?>(), true, "Id cechy produktów na zamówienie", ustawieniaGrupa.Produkty).FirstOrDefault();

            this.ProduktyDropshipingCechaID =
                Settings.GetSettingSlownikRefleksja<SlownikCech, int?>("produkty-dropshiping-cecha-id", new HashSet<int?>(), true, "Id cechy produktów w usłudze dropshiping", ustawieniaGrupa.Produkty).FirstOrDefault();

            this.ProduktyDropshipingPokazujNaStanieJesliJest = Settings.GetSettingBool("produkty-dropshiping-pokaz-na-stanie-jesli-jest-zamiast-sropshiping", false, true,
                "Dla produktów sropshiping zawsze domyślnie pokazujemy dropshiping, nawet jeśli produkt jest na stanie. To ustawienie może zmienić ten tryb działania.", ustawieniaGrupa.Produkty);

            this.ProduktyNieDostepnePrzezDluzszyCzas_iloscDni = Settings.GetSettingDecimal("produkty-nie-dostepne-dluzszy-czas-ilosc-dni", 0, true,
                "Ilość dni maksymalnego oczekiwania na dostawę. Dłuższe dostawy będą oznaczone jako niedostepne dłuższy czas.", ustawieniaGrupa.Produkty);

            this.DeaktywujMinimumLogistyczneDlaWybranychKategoriiKlientow = Settings.GetSettingSlownikRefleksja<SlownikKategoriiKlienta, int>("DeaktywujMinimumLogistyczneDlaWybranychKategoriiKlientow", new HashSet<int>(), true,
                "Kategorie klientów dla których nie będzie obowiązywało minimum logistyczne", ustawieniaGrupa.Produkty, multi: true);


            this.ProduktyNaWyczerpaniu_procentStanuMinimalnego = Settings.GetSettingDecimal("produkty-na-wyczerpaniu-procentstanuminimalnego", 0, true,
                "Procent stanu minimalnego ponizej jakiego towar jest uznawany na wyczerpaniu. Wprowadzać w formacie 0.22 jeśli ma byc 22%", ustawieniaGrupa.Produkty);

            this.GetPriceLevelDetal = Settings.GetSettingSlownikRefleksja<SlownikPoziomuCen, int?>("poziom_ceny_detalicznej", new HashSet<int?>(), true, "Poziom ceny detalicznej", ustawieniaGrupa.Systemowe).FirstOrDefault();

            this.BlokujDodawanieDoKoszykaDlaBrakujacychProduktow = Settings.GetSettingBool("ukryj_dodawanie_do_koszyka_gdy_produkt_niedostepny", false, true, "Jeśli jest na tak, nie można dodawać do koszyka produktów brakujących",
                ustawieniaGrupa.Produkty);

            this.BlokujDodawanieDoKoszykaDlaProduktowZCenaZerowa = Settings.GetSettingBool("ukryj_dodawanie_do_koszyka_gry_produkt_ma_cene_0", true, true, "Jeśli jest na tak, nie można dodawać do koszyka produktów z ceną zerową",
                ustawieniaGrupa.Produkty);

            this.RabatZaokraglacDoIluMiejsc = Settings.GetSettingInt("RabatZaokraglacDoIluMiejsc", 0, true, "Do ilu miejsc zaokrąglać rabat klienta", ustawieniaGrupa.Ceny_i_rabaty);

            this.TekstowySposobPokazywaniaPrzyciskuDoKoszyka = Settings.GetSettingBool("TekstowySposobPokazywaniaPrzyciskuDoKoszyka", false,
                    true, opis: "Jeśli włączone to zamiast ikonki koszyka pokazywany jest pełny tekst Do koszyka / w koszyku.", grupaUstawien: ustawieniaGrupa.Wygląd);

            this.DomyslnaDodawanaIloscKoszyka = Settings.GetSettingDecimal("produkty_dodawaj_domyslna_ilosc", 0, true, "Domyślna dodawana ilość do koszyka - jeśli 0 to musi być wpisana konkretna ilość w okienko ilości",ustawieniaGrupa.Koszyk);

            this.ZamykajKarteProduktuPoDodaniuDoKoszyka = Settings.GetSettingBool("ZamykajKarteProduktuPoDodaniuDoKoszyka", false, true, "Czy zamknąć popup po dodaniu produktu do koszyka", ustawieniaGrupa.Produkty);

            this.GradacjeUwzgledniajRodziny = Settings.GetSettingBool("gradacje_uwzgledniaj_rodziny", false, true,
                        opis: "Czy w wyliczaniu gradacji uwzględniać pozycje z rodziny", grupaUstawien: ustawieniaGrupa.Koszyk);

            this.DomyslneZdjecieSciezka = Settings.GetSettingString("image_empty", "/Zasoby/Obrazki/inne/noimage.png",
                        true, "Scieżka do domyślnego zdjęcia które ma się pokazać gdy produkt nie ma zdjęcia", ustawieniaGrupa.Produkty);

            this.ZCzegoLiczycGradacje = Settings.GetSettingEnum("ZCzegoLiczyćGradacje", new HashSet<ZCzegoLiczycGradacje>() {Model.Enums.ZCzegoLiczycGradacje.Koszyk}, true, multi: true);

            this.ZastepujCeneZerowaIkonkaTelefonu = Settings.GetSettingBool("zastepuj_cene_zerowa_ikonka_telefonu", false, true, "Jeżeli cena produktu jest 0 to czy zastąpić ją ikonką telefonu", ustawieniaGrupa.Produkty);

            this.MaxPokazywanyStan = Settings.GetSettingDecimal("maksymalny_pokazywany_stan", 1, true, "Maksymalna pokazywana ilość produktów w plikach integracyjnych oraz w drukowaniu ofert - ustawienie nie wpływa na stany pokazywane na liście produktów");

            this.PrzedzialyCenowe = Settings.GetSettingString("przedzialyCenowe", null, true);

            this.GetPriceLevelHurt = int.Parse(Settings.GetSettingSlownikRefleksja<SlownikPoziomuCen, string>("poziom_ceny_hurtowej", new HashSet<string> { "0" }, true, "Poziom ceny przed rabatem (hurtowa)", ustawieniaGrupa.Systemowe).First());

            this.AdresStronyZProduktem = Settings.GetSettingString("AdresStronyZProduktem", "produkt",true, "Adres strony ze szczegółowym opisem produtu (jest taki sam dla zalogowanych i niezalogowanych)", ustawieniaGrupa.Wygląd);

            this.SzumyWyszukiwania =  Settings.GetSettingString("szumy_wyszukiwania", "- ", true, "Określa znaki które mogą występować losowo w szukanej frazie. Wpisanie np. spacji powoduje że do szukania 'mleko' pasuje tekst 'ml e ko'  (jest taki sam dla zalogowanych i niezalogowanych)", ustawieniaGrupa.Produkty);

            this.DomyslnaZgodaNaNewsletter = Settings.GetSettingBool("DomyslnaZgodaNaNewsletter", true, true, "Czy klient domyślnie wyraża zgodę na newsletter (jest taki sam dla zalogowanych i niezalogowanych)", ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Newsletter);

            this.ProviderERP =  Settings.GetSettingEnum("provider", new HashSet<ERPProviderzy>(), true, "Nazwa providera ERP używanego przez synchronizator do eksportu i importu danych z i do platformy B2B", ustawieniaGrupa.Synchronizacja).FirstOrDefault();

            this.logo_niezalogowani = Settings.GetSettingHTML("logo", "<a href='./'><img src='/zasoby/obrazki/solexb2b.png' /></a>", false, "Logo systemu - link na logo powinine być w formacie: '/' - co gwarantuje poprawne działanie wielojęzykowości. " +
                                                                      "Link do zdjęcia należy poprawić na format '/zasoby/..' - bez początkowych kropek np. '../zasoby" +
                                                                      "Obrazek logo wrzuć w dokłanym rozmiarze jaki powinno być pokazywane - <b>nie dopasowuj rozmiaru przez style wysokość / szerokość</b> w edytorze!", ustawieniaGrupa.Wygląd, true);

            this.logo_zalogowani = Settings.GetSettingHTML("logo", "<a href='./'><img src='/zasoby/obrazki/solexb2b.png' /></a>", true, "Logo systemu - link na logo powinine być w formacie: '/' - co gwarantuje poprawne działanie wielojęzykowości. " +
                                                                      "Link do zdjęcia należy poprawić na format '/zasoby/..' - bez początkowych kropek np. '../zasoby" +
                                                                      "Obrazek logo wrzuć w dokłanym rozmiarze jaki powinno być pokazywane - <b>nie dopasowuj rozmiaru przez style wysokość / szerokość</b> w edytorze!", ustawieniaGrupa.Wygląd, true);

            this.MaileTylkoSolex =  Settings.GetSettingBool("nie_wysylaj_maili_do_klientow", true, true, opis: "Czy maile do klientów mają być przechwytywane", grupaUstawien: ustawieniaGrupa.Maile_i_powiadomienia);
           
            this.ZdjecieRozmiarWPowiadomieniach = Settings.GetSettingSlownikRefleksja<SlownikRozmiarZdjec, string>("ZdjecieRozmiarWPowiadomieniach", new HashSet<string> { "ikona60" }, true, "Rozmiar zdjęcia w wiadomościach mailowej informującej o np. nowym zamówieniu / brak produktu itp.", ustawieniaGrupa.Maile_i_powiadomienia).First();

            this.SklepyMapaNazwaPokazuj =  Settings.GetSettingBool("SklepyMapaNazwaPokazuj", true,true, "Czy pokazywać nazwę", ustawieniaGrupa.MapaSklepów);

            this.IkonaMapy=  Settings.GetSettingString("google_maps_icon", null, true,"Sciezka do pliku z grafiką która ma być używana jako znacznik na mapach, jeśli puste uzywana jest domyślna ikona google",ustawieniaGrupa.Wygląd);

            this.PokazywanieRabatu =  Settings.GetSettingEnum("dodatkowy-rabat-sposob-pokazywania", new HashSet<SposobPokazywaniaDodatkowegoRabatu> { SposobPokazywaniaDodatkowegoRabatu.RabatKwota }, true,"W jaki sposób pokazywany jest w koszyku rabat dodatkowy na pozycji", ustawieniaGrupa.Wygląd).First();

            var gradacjeUzgledniaProduktyZCecha = Settings.GetSettingSlownikRefleksja<SlownikAtrybutow, int?>("GradacjaUwzgledniaProduktyZCecha", new HashSet<int?>(), SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Atrybut dla cech wg których sumujemy kupowane ilości.", ustawieniaGrupa.Wygląd_Lista_Produktów);

            this.GradacjeUzgledniaProduktyZCecha = gradacjeUzgledniaProduktyZCecha != null && gradacjeUzgledniaProduktyZCecha.Any() ? gradacjeUzgledniaProduktyZCecha.First() : null;

            this.MaksymalnaIloscPozycjiWKoszyku =  Settings.GetSettingInt("MaksymalnaIloscPozycjiWKoszyku", 1000, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Maksymalna ilość pozycji jaka może znajdować się w koszyku", ustawieniaGrupa.Koszyk);

            this.PodczasWyszukiwaniaZmienPolskeZnaki = Settings.GetSettingBool("zastepuj-polskie-znaki-przy-wyszukiwaniu", true, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy podczas wyszukiwania pomijać polskie znaki (ą, ę =>  będą zastąpione a,e) ", ustawieniaGrupa.Produkty);

            this.WysylajPowiadomienieFakturaGdyBrakPdf= Settings.GetSettingBool("WysylajPowiadomienieFakturaGdyBrakPdf", false, true,opis: "Czy wysyłać informację o nowej fakturze gdy brakuje pdf na platformie",grupaUstawien: ustawieniaGrupa.Wygląd);

            this.SeparatorGrupKlientow = Settings.GetSettingString("customer_group_separator", ":_", true, opis: "Separator grup klientów", grupaUstawien: ustawieniaGrupa.Klienci).ToArray();

            this.SeparatorAtrybutowWCechach = Settings.GetSettingString("separator_atrybutow_w_cechach", ":", true).ToArray();

            this.PokazujDodajDoKoszykaFaktura =  Settings.GetSettingBool("PokazujDodajDoKoszykaFaktura", false, true, opis: "Czy pokazywać dodawanie do koszyka na podglądzie faktur", grupaUstawien: ustawieniaGrupa.Wygląd_Dokumenty);

            this.TypDomyslnyFiltru = Settings.GetSettingSlownikRefleksja<SlownikAtrybutSposobyPokazywania, string>("DomyslnyTypFiltru", new HashSet<string> { "_DropDownWielokrotnegoWyboru" }, true, "Typy filtrów w systemie", ustawieniaGrupa.Systemowe).First();

            this.UkryjJednostkiMiary = Settings.GetSettingBool("ukryj_jednostki_miary", false, true,"Czy ukryć wybór jednostki miary. Dane logistyczne też są ukrywane automatycznie.", ustawieniaGrupa.Produkty);

            this.WlascicielAdresRegion = Settings.GetSettingSlownikRefleksja<SlownikRegionow, string>("wlasciciel_adres_region", new HashSet<string>(), SesjaHelper.PobierzInstancje.CzyKlientZalogowany, opis: "Region właściciela", grupaUstawien: ustawieniaGrupa.Właściciel).Select(int.Parse).FirstOrDefault();

            this.WlascicielAdresKraj = Settings.GetSettingSlownikRefleksja<SlownikKrajow, string>("wlasciciel_adres_kraj", new HashSet<string>(), SesjaHelper.PobierzInstancje.CzyKlientZalogowany,opis: "Kraj właściciela", grupaUstawien: ustawieniaGrupa.Właściciel).Select(int.Parse).FirstOrDefault();

            this.LiczonyOdCenyNetto = Settings.GetSettingBool("liczony_od_ceny_netto", true, SesjaHelper.PobierzInstancje.CzyKlientZalogowany);

            this.SzablonNiestandardowyNazwa = Settings.GetSettingString("szablon_wlasny_niestandardowy", null,SesjaHelper.PobierzInstancje.CzyKlientZalogowany);

            this.SubiektWidocznoscTowarow =  Settings.GetSettingEnum("subiekt_widocznosc_produktow", new HashSet<WidcznoscProduktowWSubiekcie> { WidcznoscProduktowWSubiekcie.ZadenProduktNieJestPobierany }, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Widocznośc produktów w subiekcie.", ustawieniaGrupa.SynchronizacjaSubiekt).First();

            this.XlWidocznoscTowarow = Settings.GetSettingEnum("xl_widocznosc_produktow", new HashSet<WidcznoscProduktowWXl> { WidcznoscProduktowWXl.ZadenProduktNieJestPobierany }, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Widocznośc produktów w XL.", ustawieniaGrupa.SynchronizacjaXL).First();

            this.OptimaKtoreTowaryEksportowac = Settings.GetSettingEnum("optima_ktore_towary_eksportowac", new HashSet<WidcznoscProduktowWOptimie> { WidcznoscProduktowWOptimie.TylkoZCennikaZewnetrznego }, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Wybór które towary mają zostać wyeksportowane na platforme.", ustawieniaGrupa.SynchronizacjaOptima).First();

            this.WfmagPoleEmailPracownika = Settings.GetSettingEnum("wfmag_pole_email_pracownika", new HashSet<WfmagPola> { WfmagPola.POLE1 }, SesjaHelper.PobierzInstancje.CzyKlientZalogowany).First().ToString();

            this.WfmagPoleHasloPracownika =  Settings.GetSettingEnum("wfmag_pole_haslo_pracownika", new HashSet<WfmagPola> { WfmagPola.POLE2 }, SesjaHelper.PobierzInstancje.CzyKlientZalogowany).First().ToString();

            this.WagaKoszykaGdyRownaZero = Settings.GetSettingDecimal("WagaKoszykaGdyRownaZero", 0, true, "Waga koszyka w kg (tylko gdy wynosi 0) dla modułów wykorzystujących wagę koszyka.", ustawieniaGrupa.Koszyk);

            this.KtorePolaProduktuPowiadomienieNoweZamowienie = Settings.GetSettingEnum("KtorePolaProduktuPowiadomienieNoweZamowienie", new HashSet<PolaListaProduktowNoweZamowienie> {PolaListaProduktowNoweZamowienie.Ean, PolaListaProduktowNoweZamowienie.Symbol, PolaListaProduktowNoweZamowienie.Zdjecie}, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Które pola zamieścić w mailu informującym o nowym zamówieniu", ustawieniaGrupa.Maile_i_powiadomienia, multi: true, podgrupa: TypUstawieniaPodgrupa.Maile_o_nowym_zamowieniu);

            //jesli nie ma tego domyslnego filtra w widokach, to kasowanie ustawienia - podmiana na pierwzy jaki jest dostepny? TYLKO jesli aplikacja jest WEBOWA - nie w synchronizatorze!
            if (_provider.GetType() != typeof(ApiConfigProvider) )
            {
                var slownik = new SlownikAtrybutSposobyPokazywania().PobierzWartosci();
                if (!slownik.ContainsValue(this.TypDomyslnyFiltru))
                {
                    throw  new Exception();
                    //Settings.SetSetting("DomyslnyTypFiltru", null, TypUstawienia.Combo);
                    //this.TypDomyslnyFiltru = Settings.GetSettingSlownikRefleksja<SlownikAtrybutSposobyPokazywania, string>("DomyslnyTypFiltru", new HashSet<string>() {slownik.First().Key}, true, "Typy filtrów w systemie", ustawieniaGrupa.Systemowe).First();
                }
            }

            this.SzczegolyDokumentuKodPokazuj = Settings.GetSettingBool("SzczegolyDokumentuKodPokazuj", false, true, "Czy pokazyć kod produktu na szczegółach dokumentu", ustawieniaGrupa.Wygląd_Dokumenty);

            this.SzczegolyDokumentuKodKreskowyPokazuj = Settings.GetSettingBool("SzczegolyDokumentuKodKreskowyPokazuj", true, true, "Czy pokazyć kod kreskowy produktu na szczegółach dokumentu", ustawieniaGrupa.Wygląd_Dokumenty);

            this.TrybPokazywaniaFiltrow = Settings.GetSettingEnum("trybPokazywaniaFiltrow", new HashSet<TrybPokazywaniaFiltrow>{ TrybPokazywaniaFiltrow.WymuszajSciezke },
                        true, opis: "W jaki sposób mają pokazywać się filtry typu konfiguratora (np. marka - model - silnik) na liście produktów",
                        grupaUstawien: ustawieniaGrupa.Wygląd_Lista_Produktów).First();

            this.SymbolStronylogowanie =  Settings.GetSettingString("SymbolStronylogowanie", "logowanie", false,"Symbol strony do logowanie", ustawieniaGrupa.Wygląd);

            //pozostale ktore zaleza od powyzszych ustawien
            this.GradacjeAktywne = false;
            if (this.ZCzegoLiczycGradacje != null && this.ZCzegoLiczycGradacje.Any())
            {
                this.GradacjeAktywne = true;
            }

            this.GradacjeIleDniWsteczLiczyc= Settings.GetSettingInt("gradacje_ile_dni_wstecz_liczyc", 120,SesjaHelper.PobierzInstancje.CzyKlientZalogowany, opis: "Ile dni wstecz liczymy gradacje na podstawie poprzednich zakupów",grupaUstawien: ustawieniaGrupa.Koszyk);
            //this.GradacjeOdKiedyLiczyc =  Settings.GetSettingDateTime("gradacje_od_kiedy", DateTime.Parse("01-01-2007"),SesjaHelper.PobierzInstancje.CzyKlientZalogowany,opis: "Data od kiedy liczymy gradacje na podstawie poprzednich zakupów",grupaUstawien: ustawieniaGrupa.Koszyk);
        }


        public void UstalUstawieniaStartowe()
        {
            this.CzyFlatCenyWBazieUzupelniane = false;
            if (Calosc.DostepDane.DbORM.Count<FlatCeny>() > 0)
            {
                this.CzyFlatCenyWBazieUzupelniane = true;
            }

            var produkty = Calosc.DostepDane.DbORM.FirstOrDefault<Tresc>(x => x.Symbol == this.AdresStronyZProduktami);
            if (produkty != null)
            {
                LinkAlternatywnyStronyProduktow = produkty.LinkAlternatywny;
            }

            UstawUstawieniaSynchronizatora();

        }

        public virtual string LinkAlternatywnyStronyProduktow { get; set; }


        private IConfigDataProvider _provider;

        public ConfigBLL(ISolexBllCalosc Calosc, IConfigDataProvider provider) : base(Calosc)
        {
            _provider = provider;  
        }

        //    private DateTime _lastRefresh = DateTime.MinValue;
        //    private const int MinutesToRefreshCache = 60*10; //60 minut * 10 godzin
        private Dictionary<long, ISettingCollection> _sellersSettings;

        private SettingCollection _settingsBase;

        private Owner _owner;

        /// <summary>
        /// Obieket zwracający dane  właściciela platformy
        /// </summary>
        /// <returns></returns>
        public Owner GetOwner()
        {
            if (_owner == null)
            {
                _owner = new Owner
                {
                    Id = 1,
                    IsEU = "0",
                    NIP = wlasciciel_nip,
                    NIPEU = "PL" + wlasciciel_nip,
                    Address = new Adres(null)
                    {
                        Miasto = wlasciciel_adres_miasto,
                        KrajId = WlascicielAdresKraj,
                        RegionId = WlascicielAdresRegion,
                        KodPocztowy = wlasciciel_adres_kod,
                        UlicaNr = wlasciciel_adres_ulica,
                        Telefon = WlascicielAdresTelefon
                    },
                    Name = wlasciciel_nazwa,
                    Symbol = wlasciciel_symbol
                };
            }
            return _owner;
        }

        /// <summary>
        /// Symbol  właściciela platformy
        /// </summary>
        [Wymagane]
        public string wlasciciel_symbol
        {
            get
            {
                return Settings.GetSettingString("wlasciciel_symbol", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Symbol właściciela", grupaUstawien: ustawieniaGrupa.Właściciel);
            }
        }

        /// <summary>
        /// Nazwa właściciela platformy
        /// </summary>
        [Wymagane]
        public string wlasciciel_nazwa
        {
            get
            {
                return Settings.GetSettingString("wlasciciel_nazwa", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Nazwa właściciela", grupaUstawien: ustawieniaGrupa.Właściciel);
            }
        }

        /// <summary>
        /// Nazwa platformy wlaściciela
        /// </summary>
        [Wymagane]
        public string wlasciciel_AdresPlatformy
        {
            get
            {
                SystemSettings ss = new SystemSettings();
                try
                {
                    if (HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request.RawUrl))
                    {
                        ss.AdresHttp = "https://" + HttpContext.Current.Request.Url.Host;
                        return ss.AdresHttp;
                    }
                }
                catch
                {
                }

                return Settings.GetSettingString("wlasciciel_AdresPlatformy", "",
                   SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                  "Adres platformy http", ustawieniaGrupa.Właściciel);
            }
        }

        /// <summary>
        /// Adres właściciela platformy
        /// </summary>
        [Wymagane]
        public string wlasciciel_adres_ulica
        {
            get
            {
                return Settings.GetSettingString("wlasciciel_adres_ulica", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Ulica właściciela", grupaUstawien: ustawieniaGrupa.Właściciel);
            }
        }
        /// <summary>
        /// Telefon właściciela platformy
        /// </summary>
        [Wymagane]
        public string WlascicielAdresTelefon
        {
            get
            {
                return Settings.GetSettingString("WlascicielAdresTelefon", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Telefon właściciela", grupaUstawien: ustawieniaGrupa.Właściciel);
            }
        }
        /// <summary>
        /// NIP  właściciela platformy
        /// </summary>
        [Wymagane]
        public string wlasciciel_nip
        {
            get
            {
                return Settings.GetSettingString("wlasciciel_nip", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Nip właściciela", grupaUstawien: ustawieniaGrupa.Właściciel);
            }
        }

        /// <summary>
        /// Miasto  właściciela platformy
        /// </summary>
        [Wymagane]
        public string wlasciciel_adres_miasto
        {
            get
            {
                return Settings.GetSettingString("wlasciciel_adres_miasto", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Miasto właściciela", grupaUstawien: ustawieniaGrupa.Właściciel);
            }
        }
        /// <summary>
        /// Region  właściciela platformy
        /// </summary>
        [Wymagane]
        public int WlascicielAdresRegion { get; set; }

        /// <summary>
        /// Kraj  właściciela platformy
        /// </summary>
        [Wymagane]
        public int WlascicielAdresKraj { get; set; }
        

        /// <summary>
        /// Kod pocztowy  właściciela platformy
        /// </summary>
        [Wymagane]
        public string wlasciciel_adres_kod
        {
            get
            {
                return Settings.GetSettingString("wlasciciel_adres_kod", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Kod pocztowy właściciela", grupaUstawien: ustawieniaGrupa.Właściciel);
            }
        }

        public virtual char[] SeparatorAtrybutowWCechach { get; set; }

        public void LoadSettings()
        {
            List<Ustawienie> setts = _provider.PobierzWszystkieUstawienia();
            //  IList<ustawienia> setts = Calosc.DostepDane.Pobierz<ustawienia>(null);
            List<Ustawienie> podstawowe = setts.Where(p => p.OddzialId == null).ToList();
            _settingsBase = new SettingCollection(podstawowe, _provider);
            _sellersSettings = new Dictionary<long, ISettingCollection>();
            //if (GetLicense(Licencje.Partnerzy))
            //{
            //    foreach (var sett in setts.Where(p => p.OddzialId.HasValue))
            //    {
            //        if (!_sellersSettings.ContainsKey(sett.OddzialId.Value))
            //        {
            //            _sellersSettings.Add(sett.OddzialId.Value, new SettingCollection(_provider));
            //        }

            //        _sellersSettings[sett.OddzialId.Value].AddSetting(sett);
            //    }
            //    foreach (
            //        var item in setts.Where(p => p.OddzialId.HasValue).Select(p => p.OddzialId.Value).Distinct())
            //    {
            //        long klucz = item;
            //        List<string> istniejace = _sellersSettings[klucz].GetSettingsList().Select(p => p.Symbol).ToList();
            //        _sellersSettings[klucz].AddSettings(podstawowe.Where(p => !istniejace.Contains(p.Symbol)));
            //    }
            //}
            UstawUstawieniaSynchronizatora();
        }

        public string KatalogZObrazkami
        {
            get
            {
                return Settings.GetSettingString("domena_obrazki", "/Zasoby/Obrazki/",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Katalog w którym przechowywane są obrazki", grupaUstawien: ustawieniaGrupa.Systemowe);
            }
        }
  
        public DateTime DokumentyOdKiedyPobierane
        {
            get
            {
                int miesiace = Settings.GetSettingInt("DokumentyIleMiesiecyPobieranie", 13,SesjaHelper.PobierzInstancje.CzyKlientZalogowany,"Z ilu miesięcy wstecz pobieramy dokumenty na b2b",ustawieniaGrupa.Synchronizacja);
                return DateTime.Now.Date.AddMonths(-miesiace);
            }
        }

        public int[] AtrybutyRodzin
        {
            get
            {
                HashSet<string> def = new HashSet<string>();
                var wynik = Settings.GetSettingSlownikRefleksja<SlownikAtrybutow, string>("id_Atrybutu_rodziny", def, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Atrybuty używane w rodzinach - domyślnie wszystkie widoczne atrybuty w systemie.", ustawieniaGrupa.Produkty, multi: true);
                return wynik.Select(int.Parse).ToArray();
            }
        }

        public ProduktyWirtualneProvider WirtualneProduktyProvider { get; set; }


        public int[] AtrybutyWSekcjiLogistykaPokazuj
        {
            get
            {
                HashSet<string> def = new HashSet<string>();
                var wynik = Settings.GetSettingSlownikRefleksja<SlownikAtrybutow, string>("AtrybutyWSekcjiLogistykaPokazuj", def, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Pokazuj na karcie produktu w sekcji logistyka wybrane atrybuty.", ustawieniaGrupa.Wygląd_Karta_Produktu, multi: true);
                return wynik.Select(int.Parse).ToArray();
            }
        }

        /// <summary>
        /// Wymuszony refresh settingów
        /// </summary>
        public void RefreshData()
        {
            ResetSystemNames();
            _licenses = null;
          //  _slownikDomenPartnerow = null;
            _sellersSettings = null;
            _settingsBase = null;
        }

        private List<string> _dokumentyWyszukiwanie;

        public IEnumerable<string> DokumentyWyszukiwanie
        {
            get
            {
                if (_dokumentyWyszukiwanie == null)
                {
                    _dokumentyWyszukiwanie =
                        Settings.GetSettingString("DokumentyWyszukiwanie",
                            "NazwaDokumentu;PowiazaneZamowienieB2B.NumerWlasnyZamowieniaKlienta;DataUtworzenia;DokumentOdbiorca.Nazwa",
                            SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                             "W jakich polach dokumentu będzie wyszukiwana fraza",
                            ustawieniaGrupa.Dokumenty)
                            .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                }
                return _dokumentyWyszukiwanie;
            }
        }

        private List<string> _koszykWyszukiwanie;

        public IEnumerable<string> KoszykPozycjeWyszukiwanie
        {
            get
            {
                if (_koszykWyszukiwanie == null)
                {
                    _koszykWyszukiwanie =
                        Settings.GetSettingString("KoszykPozycjeWyszukiwanie",
                            "DataDodania;Produkt.Nazwa;Produkt.Kod;Produkt.KodKreskowy",
                            SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                            "W jakich polach pozycji koszyka będzie wyszukiwana fraza",
                            ustawieniaGrupa.Koszyk)
                            .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                }
                return _koszykWyszukiwanie;
            }
        }

        /// <summary>
        /// Settingi
        /// </summary>
        public ISettingCollection Settings
        {
            get
            {
                //todo: poprawić
                //if (HttpContext.Current != null && GetLicense(Licencje.Partnerzy))
                //{
                //    long sellerId = PobierzOddzialIDWgDomeny();

                //    if (sellerId != 0 && SellerSettings.ContainsKey(sellerId))
                //    {
                //        return SellerSettings[sellerId];
                //    }
                //}
                return SettingsBase;
            }
        }

        public long PobierzOddzialIDWgDomeny()
        {
            throw new NotImplementedException();
            //todo: porpawić
            //try
            //{
            //    long sellerId = SesjaHelper.PobierzInstancje.OddzialDoJakiegoNalezyKlient.GetValueOrDefault();
            //    if (sellerId == 0)
            //    {
            //        if (HttpContext.Current != null)
            //        {
            //            SlownikDomenPartnerow.TryGetValue(HttpContext.Current.Request.Url.Host, out sellerId);
            //        }
            //    }
            //    return sellerId;
            //}
            //catch (Exception)
            //{
            //    return 0;
            //}
        }

        public decimal WagaKoszykaGdyRownaZero { get; set; }

        private ISettingCollection SettingsBase
        {
            get
            {
                if (_settingsBase == null)
                {
                    LoadSettings();
                }
                return _settingsBase;
            }
        }

        private HashSet<Licencje> _licenses;

        public HashSet<Licencje> Licenses
        {
            get
            {
                if (_licenses == null)
                {
                    HashSet<Licencje> dododania = new HashSet<Licencje>();
                    var a = Enum.GetValues(typeof(Licencje));
                    foreach (Licencje licencja in a)
                    {
                        if (licencja < 0)
                        {
                            dododania.Add(licencja);
                        }
                    }
                    _licenses = SettingsBase.GetSettingEnum("lic", dododania, true, "Licencje w systemie", ustawieniaGrupa.Systemowe, false, true, true);
                }
                return _licenses;
            }
        }

        public IList<Ustawienie> PoprawUstawieniaPoSelect(int arg1, IKlient klient, IList<Ustawienie> listaUstawien, object arg4)
        {
            foreach (Ustawienie u in listaUstawien)
            {
                u.Symbol = u.Symbol.ToLower();
            }
            return listaUstawien;
        }

        /// <summary>
        /// Sprawdza czy dana licencja jest włączona
        /// </summary>
        /// <param name="key">Nazwa licencj</param>
        /// <returns></returns>
        public bool GetLicense(Licencje key)
        {
            if (key == Licencje.Brak) return true;
            if (Licenses.Contains(key)) return true;
            return false;
        }
        
        public virtual bool WieleJezykowWSystemie
        {
            get { return JezykiWSystemie.Count > 1; }
        }


        private Dictionary<int, Jezyk> _jezykiWSystemie;

        public virtual Dictionary<int, Jezyk> JezykiWSystemie
        {
            get
            {
                if (_jezykiWSystemie == null)
                {
                    _jezykiWSystemie = _provider.PobierzJezyki();
                    // _jezykiWSystemie = Calosc.DostepDane.Pobierz<jezyki>(null).ToDictionary(x => x.Id, x => x);
                }
                return _jezykiWSystemie;
            }
            set
            {
                _jezykiWSystemie = value;
                _jezykiWSystemieSlownikPoSymbolu = null;
            }
        }

        private Dictionary<string, Jezyk> _jezykiWSystemieSlownikPoSymbolu;

        public Dictionary<string, Jezyk> JezykiWSystemieSlownikPoSymbolu
        {
            get
            {
                if (_jezykiWSystemieSlownikPoSymbolu == null)
                {
                    _jezykiWSystemieSlownikPoSymbolu = JezykiWSystemie.ToDictionary(a => a.Value.Symbol.ToLower(), a => a.Value);
                }
                return _jezykiWSystemieSlownikPoSymbolu;
            }
        }

        private Dictionary<int, Dictionary<long, string>> _tlumaczenia = new Dictionary<int, Dictionary<long, string>>();

        public Dictionary<long, string> Tlumaczenia(int? lang = null)
        {
            try
            {
                if (lang == null)
                {
                    lang = JezykIDPolski;
                }
                Dictionary<long, string> names;
                if (_tlumaczenia == null)
                {
                    _tlumaczenia = new Dictionary<int, Dictionary<long, string>>();
                }
                _tlumaczenia.TryGetValue(lang.Value, out names);
                if (names != null)
                    return names;

                var items = new Dictionary<long, string>();

                if (JezykIDPolski == lang.Value)
                {
                    IList<TlumaczeniePole> tmp = _provider.GetSystemNames();   //Calosc.DostepDane.Pobierz<TlumaczeniePole>(null);
                    foreach (var v in tmp)
                    {
                        if (items.ContainsKey(v.Id))
                            items[v.Id] = v.Nazwa;
                        else
                            items.Add(v.Id, v.Nazwa);
                    }
                }
                else
                {
                    string type = typeof(TlumaczeniePole).PobierzOpisTypu();

                    foreach (var v in _provider.PobierzSystemPolaLokalizowane(type, lang.Value))
                    {
                        if (v.Pole == null)
                        {
                            Log.Debug("Pole jest null");
                            continue;
                        }
                        if (items.Any(p => p.Key == v.ObiektId))
                            items[v.ObiektId] = v.Wpis;
                        else
                            items.Add(v.ObiektId, v.Wpis);
                    }
                }
                if (_tlumaczenia.Any(p => p.Key == lang.Value))
                    _tlumaczenia[lang.Value] = items;
                else
                    _tlumaczenia.Add(lang.Value, items);

                return items;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public int KategoriaKlientaNieBlokujZmianaIp
        {
            get
            {
                HashSet<string> def = new HashSet<string> { "0" };
                var wynik = Settings.GetSettingSlownikRefleksja<SlownikKategoriiKlienta, string>("KategoriaKlientaNieBlokujZmianaIp", def, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Kategoria klientów dla której konto nie będzie blokowane z powodu zmiany adresu ip. Jeżeli klient posiada kategorię, jego konto nie zostanie zablokowane z powodu zmiany ip nawet gdy jest włączona licencja(zmiana ip), natomiast w sytuacji gdy konto posiada blokadę spowodowaną zmianą ip oraz posiada kategorię, logowanie do systemu będzie dozwolone", ustawieniaGrupa.Systemowe, multi: true);
                return int.Parse(wynik.First());
            }
        }


        

        public HashSet<int> PlikiIntegracjiAktywne_DoPokazania
        {
            get
            {
                HashSet<int> domyslne = new HashSet<int>();
                return Settings.GetSettingSlownikRefleksja<Slownik_Integracja_Szablony, int>("PlikiIntegracjiAktywne_DoPokazania", domyslne, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Integracja - pliki integarcji możliwe do pobierania przez klientów", ustawieniaGrupa.Systemowe, multi: true);
            }
        }

        public virtual string PobierzTlumaczenie(int lang, string symbol, params object[] parametryFormatuFrazy)
        {
            long hashSymbolu;
            return PobierzTlumaczenie(lang, symbol, symbol, out hashSymbolu, MiejsceFrazy.Brak, parametryFormatuFrazy);
        }

        public virtual string PobierzTlumaczenie(int lang, string symbol, string symnbolDoHash, out long symbolHash, MiejsceFrazy miejsce = MiejsceFrazy.Brak, params object[] parametryFormatuFrazy)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                throw new Exception("Pusty ciąg w lokalizacji!");
            }

            if (lang == 0)
            {
                throw new Exception("Brak podanego jezyka - jezyk id nie może być równy 0 (nie ma takiego języka)!");
            }
            string dodatekDoHasha = (miejsce != MiejsceFrazy.Brak) ? miejsce.ToString() : "";
            symbolHash = (symnbolDoHash + dodatekDoHasha).WygenerujIDObiektuSHAWersjaLong();
            string tlumaczenieWynikowe = PobierzTlumaczenieSlownik(lang, symbol, symbolHash, miejsce);
            if (lang != JezykIDPolski && JezykiWSystemie.Any(x => x.Value.DomyslnyDlaTlumaczen))
            //jest ustawiony jaki kolwiek jezyk jako domyslny dla tlumaczen
            {
                Jezyk domyslnytlumaczen = JezykiWSystemie.Values.First(x => x.DomyslnyDlaTlumaczen);
                string frazadomyslna = PobierzTlumaczenieSlownik(JezykIDPolski, symbol, symbolHash,miejsce);
                if (frazadomyslna == tlumaczenieWynikowe && lang != domyslnytlumaczen.Id)
                //fraza w danym jezyku jest nie przetlumaczona==frazie z jezyka zrodlowego, pobieramy fraze z jezyka domyslnego
                {
                    tlumaczenieWynikowe = PobierzTlumaczenieSlownik(domyslnytlumaczen.Id, symbol, symbolHash);
                }
            }

            if (parametryFormatuFrazy != null && parametryFormatuFrazy.Length > 0)
            {
                return string.Format(tlumaczenieWynikowe, parametryFormatuFrazy);
            }
            tlumaczenieWynikowe = HttpUtility.HtmlDecode(tlumaczenieWynikowe);
            return tlumaczenieWynikowe;
        }

        private string PobierzTlumaczenieSlownik(int lang, string symbol, long symbolHash, MiejsceFrazy miejsce = MiejsceFrazy.Brak)
        {
            string tlumaczenieWynikowe;

            Dictionary<long, string> names = Tlumaczenia(lang);
            if (!names.ContainsKey(symbolHash))
            {
                if (lang == JezykIDPolski || !(Tlumaczenia(JezykIDPolski).ContainsKey(symbolHash)))
                {
                    TlumaczeniePole sp = new TlumaczeniePole();
                    sp.Id = symbolHash;
                    sp.Nazwa = string.Intern(symbol);
                    sp.Domyslne = string.Intern(symbol);
                    sp.MiejsceFrazy = miejsce;
                    sp.JezykId = JezykIDPolski;
                    _provider.DodajSystemPole(sp);
                    ResetSystemNames();
                }

                tlumaczenieWynikowe = symbol;
            }
            else
            {
                tlumaczenieWynikowe = names[symbolHash];
            }
            return tlumaczenieWynikowe;
        }

        public void ResetSystemNames()
        {
            _tlumaczenia = null;
        }

        public void SprawdzJezyki()
        {
            if (_provider.PobierzJezyki().Any())
            {
                return;
            }
            Jezyk tmp = new Jezyk { Symbol = "pl", Nazwa = "Polski", Domyslny = true };
            _provider.DodajJezyk(tmp);
        }

        public bool DodawajSposobDostawyDoUwag
        {
            get
            {
                return Settings.GetSettingBool("DodawajSposobDostawyDoUwag", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy sposób dostawy ma być wstawiany do uwag w przypadku darmowej dostawy", ustawieniaGrupa.Zamówienia);
            }
        }

        public bool DodajIdKlientaDoTymczasowegoNumeruZamowienia
        {
            get
            {
                return Settings.GetSettingBool("DodajIdKlientaDoTymczasowegoNumeruZamowienia", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Dodawaj id klienta do tymczasowego numeru zamowienia z platformy", ustawieniaGrupa.Zamówienia);
            }
        }
        public bool DodawajSposobDostawyJakoPozycje
        {
            get
            {
                return Settings.GetSettingBool("DodawajSposobDostawyJakoPozycje", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy sposób dostawy ma być dodany jako pozycja koszyka w przypadku darmowej dostawy", ustawieniaGrupa.Zamówienia);
            }
        }

        public int CoIleDniZmieniacHaslo
        {
            get { return Settings.GetSettingInt("zmiana_hasla_co_dni", 0, SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public virtual string SzablonNiestandardowyNazwa { get; set; }
        

        /// <summary>
        /// zanim pobieramy ścieżke trzeba się upewnić że klient w ogóle ma szablony - propertis w configu. W przeciwnym wypadku będzie wyjątek wyrzucony
        /// </summary>
        public string SzablonNiestandardowySciezkaWzgledna
        {
            get { return "/TEMPLATES/" + SzablonNiestandardowyNazwa; }
        }

        /// <summary>
        /// zanim pobieramy ścieżke trzeba się upewnić że klient w ogóle ma szablony - propertis w configu. W przeciwnym wypadku będzie wyjątek wyrzucony
        /// </summary>
        public string SzablonNiestandardowySciezkaBezwzgledna
        {
            get
            {
                if (string.IsNullOrEmpty(this.SzablonNiestandardowyNazwa))
                {
                    throw new Exception("Brak zdefiniowanego szablonu klienta! Nie można pobrać ścieżki do szablonu!");
                }
                return AppDomain.CurrentDomain.BaseDirectory + "TEMPLATES/" + SzablonNiestandardowyNazwa;
            }
        }

        public bool GetPokazywacRabatICeneWyjsciowa(IKlient klient)
        {
            //NIEZALOGOWNY klient - OUt
            if (klient.Id == 0)
            {
                return false;
            }

            //czyli zalogowany KLIENT
            bool zwyklyRabat = Settings.GetSettingBool("lista_produktow_pokazywac_rabat_cenewyjsciowa", false,
                SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            bool tylkoDlaPln = Settings.GetSettingBool("lista_produktow_pokazywac_rabat_cenewyjsciowa_tylko_dla_waluty_pln", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            if (!tylkoDlaPln) return zwyklyRabat;
            return string.Compare(klient.WalutaKlienta.WalutaErp, "PLN", StringComparison.InvariantCultureIgnoreCase) != 0;
        }

        public bool PokazywacZyskKlienta
        {
            get
            {
                return Settings.GetSettingBool("pokazuj-zysk-klienta", false, SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        private Dictionary<long, Waluta> _slownikWalut = null;


        //TODO: ten slownik trzeba czysci jak zminy na walutach i poza tym optymalizwoc inne wywolania
        /// <summary>
        /// kolekcja walut
        /// </summary>
        public virtual Dictionary<long, Waluta> SlownikWalut
        {
            get
            {
                if (_slownikWalut == null)
                {
                    _slownikWalut = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Waluta>(null).ToDictionary(x => x.Id, x => x);
                }
                return _slownikWalut;
            }
        }

        private Dictionary<int, PoziomCenowy> _poziomyCen = null;  
        public virtual Dictionary<int, PoziomCenowy> SlownikPoziomowCenowych
        {
            get
            {
                if (_poziomyCen == null)
                {
                   _poziomyCen = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<PoziomCenowy>(null).ToDictionary(x => x.Id, x => x);
                }
                return _poziomyCen;
            }
        }


        public void UsunCacheWalut(IList<object> obj)
        {
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<Waluta>());
            _slownikWalut = null;
        }

        private Dictionary<int, StatusZamowienia> _statusy;

        /// <summary>
        /// koelkcja statusów - to jest dualizm bo w dostepach tez sa, ale tutaj sa tylko na potrzeby konfiguracji tylko po polsku
        /// </summary>
        public virtual Dictionary<int, StatusZamowienia> StatusyZamowien
        {
            get
            {
                if (_statusy == null)
                {
                    _statusy = _provider.PobierzStatusyZamowien().ToDictionary(x => x.Id, x => x);
                }
                return _statusy;
            }
        }

        public void PrzeladujResetujStatusy()
        {
            _statusy = null;
        }

        private int _jezykIdDomyslny;
        private int _jezykIdPolski;

        public bool UkryjJednostkiMiary { get; set; }

        /// <summary>
        /// jezyk domyslny do tlumaczen -  NIE WOLNO w trakcie dzialania systemu go zmieniać. Jezyk domyslny dla klientów to inne ustawienie
        /// </summary>
        public virtual int JezykIDPolski
        {
            get
            {
                if (_jezykIdPolski == 0)
                {
                    Jezyk temp = JezykiWSystemie.Values.FirstOrDefault(p => p.Symbol.Equals("pl", StringComparison.CurrentCultureIgnoreCase));
                    if (temp == null)
                    {
                        temp = JezykiWSystemie.Values.FirstOrDefault();
                    }
                    if (temp == null)
                    {
                        throw new Exception("Brak języków w systemie!");
                    }
                    _jezykIdPolski = temp.Id;
                }
                return _jezykIdPolski;
            }
        }


   


        public virtual Jezyk JezykDomyslny
        {
            get { return this.JezykiWSystemie[this.JezykIDDomyslny]; }
        }

        public virtual int JezykIDDomyslny
        {
            get
            {
                if (_jezykIdDomyslny == 0)
                {
                    Jezyk temp = JezykiWSystemie.Values.FirstOrDefault(p => p.Domyslny);
                    if (temp == null)
                    {
                        temp = JezykiWSystemie.Values.FirstOrDefault();
                    }
                    if (temp == null)
                    {
                        throw new Exception("Brak języków w systemie!");
                    }
                    _jezykIdDomyslny = temp.Id;
                }
                return _jezykIdDomyslny;
            }
        }

        public int GetPriceLevelHurt { get; set; }

        public int? GetPriceLevelDetal { get; set; }

        public bool StatisticsEnabled()
        {
            return Settings.GetSettingBool("statystyki_wlaczone", true, SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
        }

        public SposobPokazywaniaDodatkowegoRabatu PokazywanieRabatu { get; set; }

        public HashSet<ModulyOptima> JakieModulyOptima
        {
            get
            {
                var domyslne = new HashSet<ModulyOptima> { ModulyOptima.Handel, ModulyOptima.Kasa_bank };
                return Settings.GetSettingEnum("JakieModulyOptima", domyslne, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Jakie moduły mają być włączone", ustawieniaGrupa.SynchronizacjaOptima, multi: true);
            }
        }

        public string SapAdresSerweraLicencji
        {
            get
            {
                return Settings.GetSettingString("SapAdresSerweraLicencji", "localhost:30000",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Adres serwera licencji SAP - host:port", ustawieniaGrupa.Synchronizacja_Sap);
            }
        }

        public string SapSciezkaKataloguPdf
        {
            get
            {
                return Settings.GetSettingString("SapSciezkaKataloguPdf", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Ścieżka do katalogu, w którym zapisane są pdfy", ustawieniaGrupa.Synchronizacja_Sap);
            }
        }

        /// <summary>
        /// Sciezka do pliku xml generowanego przez SymplexSB
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.SymplexSB })]
        public string KatalogPlikowWymianySymplex
        {
            get
            {
                return Settings.GetSettingString("KatalogPlikowWymianySymplex", "",
                   SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Ścieżka do katalogu, w którym jest plik XML z którego importujemy",
                   ustawieniaGrupa.SynchronizacjaSymplexSB);
            }
        }

        /// <summary>
        /// Nazwa pliku xml generowanego przez SymplexSB z kontrahentami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.SymplexSB })]
        public string NazwaPlikuZKontrahentamiWymianySymplex
        {
            get
            {
                return Settings.GetSettingString("NazwaPlikuZKontrahentamiWymianySymplex", "",
                   SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym sa kontrahenci do importu (bez .xml)",
                   ustawieniaGrupa.SynchronizacjaSymplexSB);
            }
        }

        /// <summary>
        ///  Nazwa pliku xml generowanego przez SymplexSB z dokumentami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.SymplexSB })]
        public string NazwaPlikuZDokumentamiWymianySymplex
        {
            get
            {
                return Settings.GetSettingString("NazwaPlikuZDokumentamiWymianySymplex", "",
                   SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym sa dokumenty do importu (bez .xml)",
                   ustawieniaGrupa.SynchronizacjaSymplexSB);
            }
        }

        /// <summary>
        ///  Nazwa pliku xml generowanego przez SymplexSB z produktami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.SymplexSB })]
        public string NazwaPlikuZProduktamiWymianySymplex
        {
            get
            {
                return Settings.GetSettingString("NazwaPlikuZProduktamiWymianySymplex", "",
                   SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym sa produkty do importu (bez .xml)",
                   ustawieniaGrupa.SynchronizacjaSymplexSB);
            }
        }

        /// <summary>
        /// Nazwa pliku xml generowanego przez SymplexSB z kategoriami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.SymplexSB })]
        public string NazwaPlikuZKategoriamiWymianySymplex
        {
            get
            {
                return Settings.GetSettingString("NazwaPlikuZKategoriamiWymianySymplex", "",
                   SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym sa kategorie do importu (bez .xml)",
                   ustawieniaGrupa.SynchronizacjaSymplexSB);
            }
        }
        
        public string ApiAdresVendo
        {
            get
            {
                return Settings.GetSettingString("ApiAdresVendo", "localhost:8211",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Adres do Api vendo",
                    grupaUstawien: ustawieniaGrupa.Synchronizacja, podgrupa: TypUstawieniaPodgrupa.Vendo);
            }
        }

        [Wymagane(ERPProviderzy.Vendo)]
        public string ApiKontoVendo
        {
            get
            {
                return Settings.GetSettingString("ApiKontoVendo", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Konto do Api vendo",
                    grupaUstawien: ustawieniaGrupa.Synchronizacja, podgrupa: TypUstawieniaPodgrupa.Vendo);
            }
        }
        [Wymagane(ERPProviderzy.Vendo)]
        public string ApiHasloVendo
        {
            get
            {
                return Settings.GetSettingString("ApiHasloVendo", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Haslo do konta Api vendo",
                    grupaUstawien: ustawieniaGrupa.Synchronizacja, podgrupa: TypUstawieniaPodgrupa.Vendo);
            }
        }
        /// <summary>
        /// Sciezka do katalogu gdzie znjadują się pliki z zamówieniami generowanego przez SymplexSB
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.SymplexSB })]
        public string KatalogPlikowWymianyZamowienSymplex
        {
            get
            {
                return Settings.GetSettingString("KatalogPlikowWymianyZamowienSymplex", "",
                   SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Ścieżka do katalogu, w którym są plik z zamówieniami",
                   ustawieniaGrupa.SynchronizacjaSymplexSB);
            }
        }

        /// <summary>
        /// Sciezka do katalogu gdzie znjadują się pliki pdf z zamówieniami generowanego przez SymplexSB
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.SymplexSB })]
        public string SymplexSciezkaKataloguPdf
        {
            get
            {
                return Settings.GetSettingString("SymplexSciezkaKataloguPdf", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Ścieżka do katalogu, w którym zapisane są pdfy", ustawieniaGrupa.SynchronizacjaSymplexSB);
            }
        }

        public string ZnakWodnyTekst
        {
            get
            {
                return Settings.GetSettingString("znak-wodny-tekst", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Znak wodny jako tekst, bez znaczników HTML",
                     ustawieniaGrupa.Obrazki);
            }
        }

        public string ZnakWodnyObrazek
        {
            get
            {
                return Settings.GetSettingString("znak-wodny-sciezka-obrazek", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "ścieżka do obrazka który będzie znakiem wodnym nakładanym na zdjęcia produktów. Ścieżka musi być w formacie: ~/Zasoby/Obrazki/x.png. Wymagany restart aplikacji oraz wyczyszczeni cache plików",
                     ustawieniaGrupa.Obrazki, true);
            }
        }

        public bool AutomatyczneZatwierdzanieRejestracji
        {
            get
            {
                return Settings.GetSettingBool("automatyczne_zatwierdzanie_rejestracji", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Czy rejestracje są automatycznie zatwierdzane i możliwe do eksportu do erp. Jeśli nie, należy je zatwierdzic ręcznie w panelu administracyjnym",
                     ustawieniaGrupa.Rejestracja);
            }
        }

  
        public string AdresStronyHistoriaFaktur
        {
            get
            {
                return Settings.GetSettingString("AdresStronyHistoriaFaktur", "historia-faktur",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        public string AdresStronyHistoriaZamowien
        {
            get
            {
                return Settings.GetSettingString("AdresStronyHistoriaZamowien", "historia-zamowien",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        public string AdresStronyZProduktami
        {
            get
            {
                return Settings.GetSettingString("adres_strony_z_produktami", "p",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        public string AdresStronyZProduktem { get; set; }

        public string OpisKategoriiProduktowSymbol
        {
            get
            {
                return Settings.GetSettingString("OpisKategoriiProduktowSymbol", "OpisKategoriiProduktow",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Symbol strony z opis kategorii produktów", grupaUstawien: ustawieniaGrupa.Wygląd);
            }
        }

        public string AdresStronyKoszyk
        {
            get
            {
                return Settings.GetSettingString("AdresStronyKoszyk", "koszyk",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Adres strony z koszykiem", grupaUstawien: ustawieniaGrupa.Wygląd);
            }
        }

        public string SymbolStronyWylogowanie
        {
            get
            {
                return Settings.GetSettingString("SymbolStronyWylogowanie", "wylogowanie",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Symbol strony do pokazania po wylogowaniu", ustawieniaGrupa.Wygląd);
            }
        }
        public string SymbolStronylogowanie { get; set; }
        public bool AdresyEmailWymagany
        {
            get
            {
                return Settings.GetSettingBool("AdresyEmailWymagany", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Czy adres email jest wymagany przed dodawaniu adresów", ustawieniaGrupa.Klienci);
            }
        }

        public bool AdresyTelefonWymagany
        {
            get
            {
                return Settings.GetSettingBool("AdresyTelefonWymagany", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Czy telefon jest wymagany przed dodawaniu adresów", ustawieniaGrupa.Klienci);
            }
        }

     
        public KiedyWysylacMailaOZamowieniu KiedyWysylacMailaOZamowieniu
        {
            get
            {
                HashSet<KiedyWysylacMailaOZamowieniu> domyslne = new HashSet<KiedyWysylacMailaOZamowieniu>();
                domyslne.Add(KiedyWysylacMailaOZamowieniu.WMomencieFinalizacjiKoszyka);
                return Settings.GetSettingEnum("KiedyWysylacMailaOZamowieniu", domyslne,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, 
                    "Kiedy nastąpi wysłanie emaila o nowym zamówieniu", 
                    ustawieniaGrupa.Maile_i_powiadomienia, 
                    podgrupa: TypUstawieniaPodgrupa.Maile_o_nowym_zamowieniu).First();
            }
        }

        public bool KomunikatIloscWOpakowaniuGdyJednosc
        {
            get
            {
                return Settings.GetSettingBool("komuikat_ilosc_w_opakowanie_gdy_jednosc", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Czy pokazać info o ilości w opakowaniu gdy wartość wynosi jeden",
                    grupaUstawien: ustawieniaGrupa.Wygląd);
            }
        }

        public bool KomunikatIloscMinimalnaGdyJednosc
        {
            get
            {
                return Settings.GetSettingBool("komunikat_ilosc_minimalna_gdy_jednosc", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Czy pokazać info o ilości minimalna gdy wartość wynosi jeden",
                    grupaUstawien: ustawieniaGrupa.Wygląd_Lista_Produktów);
            }
        }

        [FriendlyName("Tryb pokazywania filtrów")]
        public TrybPokazywaniaFiltrow TrybPokazywaniaFiltrow { get; set; }


        public string[] PolaWlasneCechy
        {
            get
            {
                string fields = Settings.GetSettingString("pola_wlasne_cechy", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Lista pól własnych, z których będą utworzone cechy. Wiele pól można oddzielić średnikiem",
                    grupaUstawien: ustawieniaGrupa.Synchronizacja);
                string[] sp = { };
                if (!string.IsNullOrEmpty(fields))
                    sp = fields.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sp.Length; i++)
                {
                    sp[i] = sp[i].Trim();
                }

                sp.SprawdzDuble(new { });
                return sp;
            }
        }

        public decimal DomyslnaDodawanaIloscKoszyka { get; set; }

        public string AtrybutKategoriiZERP
        {
            get
            {
                return Settings.GetSettingString("atrybut_kategorii_z_erp", "kategoria",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Nazwa atrybutu dla cech utworzonych z kategorii z ERP",
                    grupaUstawien: ustawieniaGrupa.Synchronizacja);
            }
        }

        public string AtrybutProducentaZERP
        {
            get
            {
                return Settings.GetSettingString("atrybut_producenta_z_erp", "producent",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Nazwa atrybutu dla cech utworzonych z producentów Z ERP",
                    grupaUstawien: ustawieniaGrupa.Synchronizacja);
            }
        }

        public bool PrzeladujPoDodaniuDoKoszyka
        {
            get
            {
                return Settings.GetSettingBool("koszyk-przeladuj-po-dodaniu", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Czy przeładować stronę po dodaniu do koszyka", grupaUstawien: ustawieniaGrupa.Wygląd);
            }
        }

        public bool PokazujRabatyTylkoZWidocznychGrup
        {
            get
            {
                return Settings.GetSettingBool("pokazuj_rabaty_tylko_z_widocznych_grup", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Czy na stronie 'moje rabaty' pokazywać tylko rabaty z widocznej grupy",
                    grupaUstawien: ustawieniaGrupa.Klienci);
            }
        }

        public bool PodpowiedziWyszukiwania
        {
            get
            {
                return Settings.GetSettingBool("PodpowiedziWyszukiwania", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Czy  wyszukiwarka ma automatycznie przeładowywać zawartość",
                     ustawieniaGrupa.Wygląd_Lista_Produktów);
            }
        }

        public int NiePokazujRodzinOdIlosciDzieci
        {
            get
            {
                return Settings.GetSettingInt("NiePokazujRodzinOdIlosciDzieci", 0,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nie pokazuj rodzin na liscie jesli dzieci wiecej niz wartość ustawienia. Wpisanie 0 oznacza brak limitu", ustawieniaGrupa.Wygląd_Lista_Produktów);
            }
        }

        public DateTime PobierzDateZmianaStatusu
        {
            get
            {
                int dni = Settings.GetSettingInt("IleDniZmianaStatusu", 5,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Ile dni temu maksymalnie mógł zostać utworzony dokument do którego wysyłany maila o zmianie statusu", ustawieniaGrupa.Dokumenty);

                return DateTime.Now.Date.AddDays(-dni);
            }
        }

        public int ZIluDniWsteczWysylacListyPrzewozowe
        {
            get
            {
                return Settings.GetSettingInt("z-ilu-dni-wstecz-listy-przewozowe", 7,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Ile dni temu maksymalnie mógł zostać utworzony dokument do którego wysyłany maila o listach przewozowych",
                    grupaUstawien: ustawieniaGrupa.Dokumenty);
            }
        }

        //public bool DokumentyPokazacNazweKlienta
        //{
        //    get
        //    {
        //        return Settings.GetSettingBool("dokumenty-pokazuj-nazwe-konta", true,
        //            SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
        //            opis:
        //                "Czy w dokumentach pokazywac nazwę konta użytkownika. Działa tylko jeśli klienta ma konta podrzędne",
        //            grupaUstawien: ustawieniaGrupa.Klienci);
        //    }
        //}

        public virtual char[] SeparatorGrupKlientow { get; set; }

        /// <summary>
        /// określa czy są aktywne gradacje - ustawiane autoamtycznie w globa asax
        /// </summary>
        public bool GradacjeAktywne { get; private set; }

        /// <summary>
        /// określa czy flat ceny sa w bazie - np. jesli system ERP liczyc gotowe ceny - jka nie ma takich cen to wylanczane sa moduly pobierania flat ceny
        /// </summary>
        public bool CzyFlatCenyWBazieUzupelniane { get; private set; }

        public HashSet<ZCzegoLiczycGradacje> ZCzegoLiczycGradacje { get; set; }

      //  public DateTime GradacjeOdKiedyLiczyc { get; set; }

        public int GradacjeIleDniWsteczLiczyc { get; set; }

        public bool GradacjeUwzgledniajRodziny { get; set; }

        public int OptimaIdSzablonuWydrukuDoPdf
        {
            get
            {
                return Settings.GetSettingInt("optima-szablon-eksportu-pdf-id", 1834,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Id szablonu dokumentów z optimy, który ma być użyty przy drukowaniu pdf");
            }
        }

  
        [Wymagane]
        public string SciezkaFavicon
        {
            get
            {
                string klucz = "favicon";

                string ikona = Calosc.Cache.PobierzObiekt<string>(klucz);
                if (ikona == null)
                {
                    string sciezka = AppDomain.CurrentDomain.BaseDirectory +
                                     "/Zasoby/Obrazki/favicon.ico".Replace("/", "\\");
                    if (File.Exists(sciezka))
                    {
                        Calosc.Cache.DodajObiekt(klucz, "/Zasoby/Obrazki/favicon.ico");
                    }
                    else
                    {
                        Calosc.Cache.DodajObiekt(klucz, "");
                    }
                }
                return ikona;
            }
        }

        //uzupelniane w konstruktorze
        public virtual int? ProduktyNieDostepnePrzezDluzszyCzasCecha { get; set; }

        //uzupelniane w konstruktorze
        public virtual int? ProduktyNaZamowienieCechaID { get; set; }

        public virtual bool BlokujDodawanieDoKoszykaDlaBrakujacychProduktow { get; set; }

        public bool BlokujDodawanieDoKoszykaDlaProduktowZCenaZerowa { get; set; }

        public bool ZastepujCeneZerowaIkonkaTelefonu { get; set; }

        public virtual decimal ProduktyNaWyczerpaniu_procentStanuMinimalnego { get; set; }

        public virtual int? ProduktyDropshipingCechaID { get; set; }

        public virtual bool ProduktyDropshipingPokazujNaStanieJesliJest { get; set; }

        public virtual decimal ProduktyNieDostepnePrzezDluzszyCzas_iloscDni { get; set; }

        public virtual bool PodczasWyszukiwaniaZmienPolskeZnaki { get; set; }
    

        public bool UzwagledniaRezerwacjeStanow
        {
            get
            {
                return Settings.GetSettingBool("UzwagledniaRezerwacjeStanow", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Uwzględniaj rezerwacje przy pobieraniu stanów.",
                    ustawieniaGrupa.Synchronizacja);
            }
        }

        public bool ZamowieniaTworzRezerwacje
        {
            get
            {
                return Settings.GetSettingBool("rezerwacja", false, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                      "Czy przy zapisie dokumentów do erp rezerwować stany",
                     ustawieniaGrupa.Synchronizacja);
            }
        }

        public string EnovaPoleDoPobieraniaZDokumentow
        {
            get
            {
                return Settings.GetSettingString("EnovaPoleDopobieraniaZDokumentow", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Nazwa cechy jaka ma być pobierania z pozycji dokumentu jako opis pozycji",
                     ustawieniaGrupa.Dokumenty);
            }
        }

        public string EnovaPoleDoZapisywaniaUwagNaPozycjiDokumentu
        {
            get
            {
                return Settings.GetSettingString("EnovaPoleDoZapisywaniaUwagNaPozycjiDokumentu", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Nazwa cechy do zapisywania uwag o pozycji dokumentu",
                     ustawieniaGrupa.Dokumenty);
            }
        }

        public string EnovaPole2DoZapisywaniaUwagNaPozycjiDokumentu
        {
            get
            {
                return Settings.GetSettingString("EnovaPole2DoZapisywaniaUwagNaPozycjiDokumentu", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Nazwa cechy do zapisywania długich uwag o pozycji dokumentu",
                     ustawieniaGrupa.Dokumenty);
            }
        }

        public bool SortowanieNaturalneListyProduktow
        {
            get
            {
                return Settings.GetSettingBool("SortowanieNaturalneListyProduktow", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Czy sortować wartości na liście produktów naturalnie",
                     ustawieniaGrupa.Produkty);
            }
        }

        public bool PokazywanieUwagWPodgladzie
        {
            get { return Settings.GetSettingBool("PokazywanieUwagWPodgladzie", true, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy pokazywać uwagi w podglądzie", ustawieniaGrupa.Wygląd); }
        }

        public string PobierzSerieDlaWaluty(string waluta)
        {
            return Settings.GetSettingString("SeriaDokumentu_" + waluta, "",
                SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                 "Seria dokumentu zapisywanego w ERP dla danej waluty: " + waluta + " np. ZO",
                 ustawieniaGrupa.Zamówienia, true,dynamiczne:true);
        }

        public bool InfoPrzekroczoneStany
        {
            get
            {
                return Settings.GetSettingBool("InfoPrzekroczoneStany", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Czy pokazywać komunikat o przekroczonmych stanach przy dodawaniu do koszyka",
                     ustawieniaGrupa.Koszyk);
            }
        }

        public bool SprawdzajStanyMagazynowe
        {
            get
            {
                return Settings.GetSettingBool("sprawdzaj_stany_magazynowe", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Określa czy sprawdzać stany magazynowe przy dodawaniu produktu jako pozycję w dokumencie handlowym. Opcja na TAK powoduje że ilość zostanie ucięta do dostępnych ilości",
                     ustawieniaGrupa.Zamówienia);
            }
        }

        public int ZIleDniDomyslniePokazywacDokumenty
        {
            get
            {
                return Settings.GetSettingInt("ZIleDniDomyslniePokazywacDokumenty", 29,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Z ilu dni wstecz domyślne pokazywać dokumenty, bez uwzględniania aktualnego dnia. Np. jeśli z 30 to wpisujemy 29",
                     ustawieniaGrupa.Dokumenty);
            }
        }

        public bool WielowybieralnoscKategorii
        {
            get
            {
                return Settings.GetSettingBool("wszystkie_kategorie_wybieralne", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Wielowybieralność kategorii", ustawieniaGrupa.Produkty);
            }
        }

        public bool ZamykajKarteProduktuPoDodaniuDoKoszyka { get; set; }

        public string OptimaAtrybutZeZdjeciami
        {
            get
            {
                return Settings.GetSettingString("OptimaAtrybutZeZdjeciami", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Atrybut w jakim przechowywane są zdjęcia na platformę ",
                    grupaUstawien: ustawieniaGrupa.Synchronizacja);
            }
        }

        public string SqlDoWyciaganiWzorcaWydrukuSubiektBioPlanet
        {
            get
            {
                return Settings.GetSettingString("sql_wzorzecWydrukuBioPlanet", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "SQL do wyciągania wzorca wydruku dla dokumentu - dla Bio Planet. Parametry: @dok_id - dokument id",
                    grupaUstawien: ustawieniaGrupa.Synchronizacja);
            }

        }

        
        [Wymagane(new[] { ERPProviderzy.Subiekt, ERPProviderzy.Enova, ERPProviderzy.Optima, ERPProviderzy.WFMAG, ERPProviderzy.XL })]
        public string ERPcs
        {
            get
            {
                return Settings.GetSettingString("erp_cs", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Connectionstring do połączenia do bazy danych ERP ",
                    grupaUstawien: ustawieniaGrupa.Synchronizacja);
            }
        }

        public string ERPcs2
        {
            get
            {
                return Settings.GetSettingString("erp_cs2", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Connectionstring do połączenia do bazy danych ERP ",
                    grupaUstawien: ustawieniaGrupa.Synchronizacja);
            }
        }

        [Wymagane(new[] { ERPProviderzy.Subiekt, ERPProviderzy.Enova, ERPProviderzy.Optima, ERPProviderzy.WFMAG, ERPProviderzy.XL })]
        public string ERPHaslo
        {
            get
            {
                return Settings.GetSettingPassword("erp_haslo", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Hasło operatora w ERP ", ustawieniaGrupa.Synchronizacja);
            }
        }

        [Wymagane(new[] { ERPProviderzy.Subiekt, ERPProviderzy.Enova, ERPProviderzy.Optima, ERPProviderzy.WFMAG, ERPProviderzy.XL })]
        public string ERPLogin
        {
            get
            {
                return Settings.GetSettingString("erp_login", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                 "Login operatora w ERP ", ustawieniaGrupa.Synchronizacja);
            }
        }

        public bool PokazujCenyDlaNiezalogowanych
        {
            get
            {
                return Settings.GetSettingBool("PokazujCenyDlaNiezalogowanych", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Czy pokazywać ceny w katalogu dla niezalogowanych klientów",
                    grupaUstawien: ustawieniaGrupa.Wygląd);
            }
        }

        public bool WysylajPowiadomienieFakturaGdyBrakPdf { get; set; }

        public string EnovaDoZapisuKategoriiZamowienia
        {
            get
            {
                return Settings.GetSettingString("EnovaDoZapisuKategoriiZamowienia", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Nazwa cechy w której zapisujemy kategorie zamówienia",
                    grupaUstawien: ustawieniaGrupa.SynchronizacjaEnova);
            }
        }

        public string EnovaCechaDoPobraniaJakoStatusDokumentu
        {
            get
            {
                return Settings.GetSettingString("EnovaCechaDoPobraniaJakoStatusDokumentu", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Nazwa cechy którą pobieramy z dokumentu i pokazujemy jako status dokumentu",
                    grupaUstawien: ustawieniaGrupa.SynchronizacjaEnova);
            }
        }

        public string EnovaTymczasowaSciezkaPDF
        {
            get
            {
                return Settings.GetSettingString("EnovaTymczasowaSciezkaPDF", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Tymczasowa ścieżka do której będą generowane dokumenty pdf. Ścieżka tymczasowego katalogu musi być taka sama jak w ustawieniach drukarki.",
                    grupaUstawien: ustawieniaGrupa.SynchronizacjaEnova);
            }
        }

        [Wymagane(new[] { ERPProviderzy.Subiekt, ERPProviderzy.Enova, ERPProviderzy.Optima, ERPProviderzy.XL })]
        public string KatalogProgramuKsiegowego
        {
            get
            {
                return Settings.GetSettingString("katalog_programu_ksiegowego", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Katalog w którym zainstalowany jest program księgowy",
                    ustawieniaGrupa.Synchronizacja);
            }
        }

        public List<Sortowanie> DostepneSortowanieListyProduktow
        {
            get
            {
                string sort = Settings.GetSettingString("DostepneSortowanieListyProduktow",
                    "Nazwa asc|Nazwa;Kod asc|Kod;KodKreskowy asc|Kod kreskowy;FlatCeny.CenaNetto desc|najwyższa cena; FlatCeny.CenaNetto asc|najniższa cena;IloscLaczna desc| ilość;DataDodania desc|od najnowszych",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwy pól oddzielone ; po których można sortować listę produktów. Kilka opcji dla jednej grugy odzielone są ,",
                    ustawieniaGrupa.Produkty,false,true,TypUstawieniaPodgrupa.Brak, "Nazwa asc|Nazwa;Kod asc|Kod;KodKreskowy asc|Kod kreskowy");
                return PrzygotujSortowanie(sort);
            }
        }

        public Sortowanie SortowaniePozycjiPrzedZapisemZamowienia
        {
            get
            {
                string sort = Settings.GetSettingString("SortowaniePozycjiPrzedZapisemZamowienia", "Produkt.Kod asc|kod",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwy pola po którym będziemy sortować pozycje zamówienia",
                    ustawieniaGrupa.Koszyk);
                return PrzygotujSortowanie(sort).First();
            }
        }

        public List<Sortowanie> PrzygotujSortowanie(string sort)
        {
            List<Sortowanie> wynik = new List<Sortowanie>();
            if (!string.IsNullOrEmpty(sort) && sort!="null")
            {
                string[] opcje = sort.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string o in opcje)
                {
                    List<SortowaniePole> opcjesortowania = new List<SortowaniePole>();
                    string[] sekcje = o.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] sortowania = sekcje[0].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in sortowania)
                    {
                        string[] pojedynczy = s.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        KolejnoscSortowania kolejnosc =
                            (KolejnoscSortowania)Enum.Parse(typeof(KolejnoscSortowania), pojedynczy.Length == 2 ? pojedynczy[1] : "asc");
                        opcjesortowania.Add(new SortowaniePole(pojedynczy[0], kolejnosc));
                    }
                    wynik.Add(new Sortowanie(opcjesortowania, (sekcje.Length > 1 ? sekcje[1] : ""), o));
                }
            }

            return wynik;
        }

        public List<Sortowanie> DostepneSortowanieKoszyka
        {
            get
            {
                string sort = Settings.GetSettingString("DostepneSortowanieKoszyka",
                    "Produkt.Nazwa asc|nazwa;Produkt.Kod asc|kod;Produkt.KodKreskowy asc|kod kreskowy;Id desc|kolejność dodawania",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwy pól oddzielone ; po których można sortować koszyk. Kilka opcji dla jednej grugy odzielone są ,",
                    ustawieniaGrupa.Koszyk);
                return PrzygotujSortowanie(sort);
            }
        }

        public bool TekstowySposobPokazywaniaPrzyciskuDoKoszyka { get; set; }

        public string EnovaPoleDoPobieraniaZDokumentow2
        {
            get
            {
                return Settings.GetSettingString("EnovaPoleDoPobieraniaZDokumentow2", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Nazwa cechy jaka ma być pobierania z pozycji dokumentu jako opis 2 pozycji",
                    grupaUstawien: ustawieniaGrupa.Dokumenty);
            }
        }

     
        public HashSet<string> SlowaWymaganeWDokumencie
        {
            get
            {
                string ciag = Settings.GetSettingString("SlowaWymaganeWDokumencie", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Słowa które MUSZĄ wystąpić w nazwie dokumentu. Rozdzielone średnikami",
                    grupaUstawien: ustawieniaGrupa.Dokumenty,
                    podgrupa: TypUstawieniaPodgrupa.Dokumenty_synchronizacja
                    );

                if (string.IsNullOrEmpty(ciag))
                {
                    return new HashSet<string>();
                }

                    return new HashSet<string>( ciag.Split(';') ) ;
            }
        }

        public HashSet<string> SlowaZakazaneWDokumencie
        {
            get
            {
                string ciag= Settings.GetSettingString("SlowaZakazaneWDokumencie", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Słowa które NIE MOGĄ wystąpić w nazwie dokumentu. Rozdzielone średnikami",
                    grupaUstawien: ustawieniaGrupa.Dokumenty,
                    podgrupa: TypUstawieniaPodgrupa.Dokumenty_synchronizacja
                    );
                if (string.IsNullOrEmpty(ciag))
                {
                    return new HashSet<string>();
                }

                return new HashSet<string>(ciag.Split(';'));
            }
        }
        public int? GradacjeUzgledniaProduktyZCecha { get; set; }

        public HashSet<ElementyKoszykaPodglad> PolaKoszykaPodglad
        {
            get
            {
                HashSet<ElementyKoszykaPodglad> domyslne = new HashSet<ElementyKoszykaPodglad> { ElementyKoszykaPodglad.Nazwa, ElementyKoszykaPodglad.Ilosc };
                return Settings.GetSettingEnum("Pola_Do_Podgladu_Koszyka", domyslne, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Pole ktore beda pokazywane w podgladzie koszyka", ustawieniaGrupa.Koszyk, multi: true);
            }
        }

        public string CzyszczenieStalychFiltrow
        {
            get { return "stale-clear"; }
        }

        //na razie jest tak, nie wiem czy klient powinien mieć możliwość zmiany tego
        public virtual string CechaAuto
        {
            get { return "--auto--"; }
        }

        public bool UruchamianyNaSerwerze
        {
            get { return _provider != null && _provider.GetType() == typeof(SqlSettingProvider); }
        }

        private readonly Dictionary<string, string> _slownikUStawienia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        private string PobierzSynchronizacjaPoleUstawienie(Jezyk j, string pole, string domyslne, Dictionary<string, string> pars)
        {
            string symbolUstawienia = string.Format("jezyk_{0}_{1}", j.Symbol, pole);
            //Konieczne to jest ze względu na fakt iż mogą być rożne slowniki i musimy to sprawdzać
            long idLong = string.Join(",", pars.Keys).WygenerujIDObiektuSHAWersjaLong();
            string klucz = $"{symbolUstawienia}_{idLong}";
            if (_slownikUStawienia.ContainsKey(klucz))
            {
                return _slownikUStawienia[klucz];
            }
            string opisUstawiania = string.Format("Pole {1} produktu dla języka o symbolu {0}", j.Symbol, pole.ToUpper());

            IEnumerable<string> listaWartosciPol = pars.Keys;

            HashSet<string> dom = new HashSet<string> { domyslne };
            var s = Settings.GetSettingSlownik(symbolUstawienia, dom, listaWartosciPol,SesjaHelper.PobierzInstancje.CzyKlientZalogowany, opisUstawiania, ustawieniaGrupa.Produkty, false, true, false, TypUstawieniaPodgrupa.Brak, true, "Pola produktu w języku " + j.Nazwa);
            _slownikUStawienia.Add(klucz, s.FirstOrDefault() ?? "");
            return _slownikUStawienia[klucz];
        }

        private Tlumaczenie StworzWpisDoSlownika(Jezyk j, string pole, string wartosc, Produkt p)
        {
            long id = p.Id;
            string typ = typeof(ProduktBazowy).PobierzOpisTypu();//p.GetType().PobierzOpisTypu();
            return new Tlumaczenie { Pole = pole, JezykId = j.Id, Typ = typ, Wpis = wartosc, ObiektId = id };
        }

        private string PobierzWartosc(Jezyk jezyk, string pole, string domyslnePole, Dictionary<string, string> pars)
        {
            string klucz = PobierzSynchronizacjaPoleUstawienie(jezyk, pole, domyslnePole, pars);
            if (!string.IsNullOrEmpty(klucz))
            {
                return PobierzWartosc(klucz, pars);
            }
            return null;
        }

        private string PobierzWartosc(string klucz, Dictionary<string, string> pars)
        {
            if (!string.IsNullOrEmpty(klucz) && pars.ContainsKey(klucz))
            {
                string wartosc = pars[klucz];
                return string.IsNullOrEmpty(wartosc) ? null : wartosc;
            }
            return null;
        }

        public void SynchronizacjaPobierzLokalizacjeNazwa(Produkt item, Jezyk jezyk, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "Nazwa";
            string wartosc = PobierzWartosc(jezyk, pole, domyslnePole, pars);
            if (jezyk.Domyslny)
            {
                item.Nazwa = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(jezyk, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeKod(Produkt item, Jezyk jezyk, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "Kod";
            string wartosc = PobierzWartosc(jezyk, pole, domyslnePole, pars);
            if (jezyk.Domyslny)
            {
                item.Kod = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(jezyk, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeOpis(Produkt item, Jezyk jezyk, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "Opis";
            string wartosc = PobierzWartosc(jezyk, pole, domyslnePole, pars);
            if (jezyk.Domyslny)
            {
                item.Opis = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(jezyk, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeOpis2(Produkt item, Jezyk j, string domyslnePole,Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "Opis2";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.Opis2 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeOpis3(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "Opis3";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.Opis3 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeOpis4(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "Opis4";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.Opis4 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeOpis5(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "Opis5";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.Opis5 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeOpisKrotki(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "OpisKrotki";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.OpisKrotki = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeOpisKrotki2(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "OpisKrotki2";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.OpisKrotki2 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeOpisKrotki3(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "OpisKrotki3";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.OpisKrotki3 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeOpisKrotki4(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "OpisKrotki4";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.OpisKrotki4 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeOpisKrotki5(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "OpisKrotki5";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.OpisKrotki5 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }


        public void SynchronizacjaPobierzPoleDostawa(Produkt item, string domyslnePole, Dictionary<string, string> pars, DateTime? data)
        {
            if (data.HasValue && data >= DateTime.Now.Date)
            {
                item.Dostawa = data.Value.ToShortDateString();
            }
            else
            {
                string pole = PobierzSynchronizacjaPoleUstawienie("Dostawa", domyslnePole, pars, ustawieniaGrupa.Produkty);
                string wart = PobierzWartosc(pole, pars);
                if (!string.IsNullOrEmpty(wart))
                {
                    item.Dostawa = wart;
                }
            }
        }

        public void SynchronizacjaPobierzPoleOjciec(Produkt item, string domyslne, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("Ojciec", domyslne, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            bool wynik;
            if (!string.IsNullOrWhiteSpace(wart) && Refleksja.PrzetworzBool(wart, out wynik))
            {
                item.Ojciec = wynik;
            }
        }


        public void SynchronizacjaPobierzLokalizacjePoleTekst1(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "PoleTekst1";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);

            //Log.DebugFormat("PoleTekst1 Dla: {1}-{2}, Wartość ustawienia:{0}", wartosc,item.Id,item.Kod);
            if (j.Domyslny)
            {
                item.PoleTekst1 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjePoleTekst2(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "PoleTekst2";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);

            if (j.Domyslny)
            {
                item.PoleTekst2 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }
        public void SynchronizacjaPobierzLokalizacjePoleTekst3(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "PoleTekst3";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
           
            if (j.Domyslny)
            {
                item.PoleTekst3 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjePoleTekst4(Produkt item, Jezyk j, string domyslnePole,Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "PoleTekst4";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.PoleTekst4 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjePoleTekst5(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "PoleTekst5";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);

            if (j.Domyslny)
            {
                item.PoleTekst5 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeRodzina(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "Rodzina";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.Rodzina = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                {
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
                }
            }
        }

        public void SynchronizacjaPobierzLokalizacjeKolumnaTekst1(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "KolumnaTekst1";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.KolumnaTekst1 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeKolumnaTekst2(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "KolumnaTekst2";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.KolumnaTekst2 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeKolumnaTekst3(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "KolumnaTekst3";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.KolumnaTekst3 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeKolumnaTekst4(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "KolumnaTekst4";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.KolumnaTekst4 = wartosc;
            }else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeKolumnaTekst5(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "KolumnaTekst5";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.KolumnaTekst5 = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeMetaOpis(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "MetaOpis";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.MetaOpis = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public void SynchronizacjaPobierzLokalizacjeMetaSlowaKluczowe(Produkt item, Jezyk j, string domyslnePole, Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            const string pole = "MetaSlowaKluczowe";
            string wartosc = PobierzWartosc(j, pole, domyslnePole, pars);
            if (j.Domyslny)
            {
                item.MetaSlowaKluczowe = wartosc;
            }
            else
            {
                if (!string.IsNullOrEmpty(wartosc))
                    tlumaczenia.Add(StworzWpisDoSlownika(j, pole, wartosc, item));
            }
        }

        public bool ZamowienieWImieniuKlientaWysylajMaile
        {
            get
            {
                return Settings.GetSettingBool("ZawszeWysylajMaileZamowienia", false, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Czy wysyłać maile z zamówieniem złożonym przez pracawonika w imieniu klienta",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Maile_o_nowym_zamowieniu);
            }
        }

        //public bool ZamowieniaZapisywaneCenyNetto
        //{
        //    get { return Settings.GetSettingBool("liczony_od_ceny_netto", true, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy zamówienie zapisywane jest według cen netto z b2b", ustawieniaGrupa.Synchronizacja); }
        //}

        public int SferaPobieranieLimitOkres
        {
            get
            {
                return Settings.GetSettingInt("SferaPobieranieLimitOkres", 30,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Z ilu minut sprawdzane są statystyki przy sprawdzaniu czy klient może skrzystać z api",
                    grupaUstawien: ustawieniaGrupa.Klienci, nadpisywanyPrzezPracownika: true);
            }
        }

        public int SferaMaksPobranNaOkres
        {
            get
            {
                return Settings.GetSettingInt("SferaMaksPobranNaOkres", 5,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, opis: "Ile razy można maksymalnie pobrac dokumenty w danym okresie",
                    grupaUstawien: ustawieniaGrupa.Klienci, nadpisywanyPrzezPracownika: true);
            }
        }

        public string RozmiarZdjeciaIndywidualizacja
        {
            get
            {
                HashSet<string> def = new HashSet<string>();
                def.Add("ico82x82wp");
                return Settings.GetSettingSlownikRefleksja<SlownikRozmiarZdjec, string>("RozmiarZdjeciaIndywidualizacja", def,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Rozmiar zdjęć przy indywidualizacji.",
                    ustawieniaGrupa.Wygląd_Lista_Produktów).First();
            }
        }

        public WidcznoscProduktowWSubiekcie SubiektWidocznoscTowarow { get; set; }

        public WidcznoscProduktowWXl XlWidocznoscTowarow { get; set; }

        public WidcznoscProduktowWOptimie OptimaKtoreTowaryEksportowac { get; set; }

        public void UkryjUstawienia()
        {
            //Nie może tak być bo ukrywa ustawienia które są metodami i nie sa dynamiczne.
            //todo: komentuje bartek bałagan z ustawieniami - ustawienia sa calkiem do przemeblowania
            //var set = _provider.PobierzWszystkieUstawienia();
            //foreach (Ustawienie s in set)
            //{
            //    if (s.Dynamiczne)
            //    {
            //        continue;
            //    }
            //    s.Widoczne = false;
            //}
            //_provider.AktualizujUstawienie(set);
            //RefreshData();
            DodajUstawieniaZPropertisow();
        }

        public void DodajUstawieniaZPropertisow()
        {
            //nie wiem po co to komu??
            var t = GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in t)
            {
                if (propertyInfo.CanRead)
                {
                    try
                    {
                        //taki glulpi kod tak bo przy odczyscyei zakaldaj sie ustawienia
                        object data = propertyInfo.GetValue(this);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public string PobierzSzablonWydrukuEnova(string symbolJezykaWydruku)
        {
            return Settings.GetSettingString("enova_szablon_wydruku_jezyk_" + symbolJezykaWydruku, "sprzedaz.aspx",
                SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                 "SzablonListyProduktow wydruku w języku " + symbolJezykaWydruku,
                 ustawieniaGrupa.SynchronizacjaEnova);
        }

        [Wymagane]
        public ERPProviderzy ProviderERP { get; set; }

        public string SeparatorMail
        {
            get
            {
                return Settings.GetSettingString("customers_email_separator", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        public int MaksimumWydrukowPDF
        {
            get
            {
                return Settings.GetSettingInt("MaksimumWydrukowPDF", 500,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Ile jest maksymalnie drukowanych pdf przy jednym uruchomieniu synchronizacji pdf",
                     ustawieniaGrupa.Synchronizacja);
            }
        }

        private string logo_zalogowani = null;
        private string logo_niezalogowani = null;

        public string Logo
        {
            get
            {
                if (SesjaHelper.PobierzInstancje.CzyKlientZalogowany)
                {
                    return logo_zalogowani;
                }
                return logo_niezalogowani;                
            }
        }

        public string IkonaMapy { get; set; }

        public int DokumentyMailOnNowymIleDniWstecz
        {
            get
            {
                return Settings.GetSettingInt("faktury_elektronicze_maile_ile_dni_wstecz", 4,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Z ilu dni wstecz wysyłać maila o nowej fakturze", ustawieniaGrupa.Dokumenty);
            }
        }
        
        public string DomyslneZdjecieSciezka { get; set; }

        #region ustawienia bez opisów (potem do przeniesienia wyżej)

        public virtual string PrzedzialyCenowe { get; set; }

        public virtual string SzumyWyszukiwania { get; set; }

        #endregion ustawienia bez opisów (potem do przeniesienia wyżej)

        public int MaksimumDokumentowWPaczce
        {
            get
            {
                return Settings.GetSettingInt("dokumentow_paczka", 10000,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Ile dokumentów maksymalnie wysyłanych jest przy pojedyńczej synchronizacji dokumentów",
                     ustawieniaGrupa.Dokumenty);
            }
        }

        public int CzasPrzechowywaniaZmian
        {
            get
            {
                return Settings.GetSettingInt("CzasPrzechowywaniaZmian", 30,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Z ilu dni mają sie przechowywać historia zmian w bazie danych.",
                     ustawieniaGrupa.Systemowe);
            }
        }
        //private bool? _cenyZerowe;

        private HashSet<string> _produktyWyszukiwanie;

        public virtual List<string> ProduktyWyszukiwanie
        {
            get
            {
                if (_produktyWyszukiwanie == null)
                {
                    var domyslne = new HashSet<string>();
                    domyslne.Add("Nazwa");
                    domyslne.Add("Kod");
                    domyslne.Add("KodKreskowy");
                    domyslne.Add("DodatkoweKodyString");
                    domyslne.Add("PoleTekst1");
                    domyslne.Add("Opis");
                    domyslne.Add("OpisKrotki");
                    _produktyWyszukiwanie = Settings.GetSettingReflekcja<ProduktBazowy, string>("produkty_wyszukiwanie", domyslne, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                        "W jakich polach produktu będzie wyszukiwana fraza", ustawieniaGrupa.Wygląd_Lista_Produktów, false, true, true);
                }

                return _produktyWyszukiwanie.ToList();
            }
        }

        private HashSet<string> _polaDoWyzerowania;

        public virtual List<string> PolaDoWyzerowania
        {
            get
            {
                if (_polaDoWyzerowania == null)
                {
                    var domyslne = new HashSet<string>();
                    Produkt p = new Produkt();
                    foreach (var a in p.GetType().GetProperties())
                    {
                        if (a.CanRead && a.Name.StartsWith("opis", StringComparison.InvariantCultureIgnoreCase))
                        {
                            domyslne.Add(a.Name);
                        }
                    }
                    _polaDoWyzerowania = Settings.GetSettingReflekcja<Produkt, string>("pola_produktow_do_oczyszczenia", domyslne, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Które pola oczyścić dla produktów nie widocznych", ustawieniaGrupa.Wygląd_Lista_Produktów, false, true, true);
                }

                return _polaDoWyzerowania.ToList();
            }
        }

        public bool EksportTylkoKontZHaslem
        {
            get
            {
                return Settings.GetSettingBool("b2b_eksport_kont_z_haslem", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Eksportuje konta z ERP tylko jeśli mają wpisane hasło (pole z hasłem ustawia się w b2b_haslo)",
                    ustawieniaGrupa.Synchronizacja);
            }
        }

        public int CzasWaznosciLogowania
        {
            get
            {
                return Settings.GetSettingInt("CzasWaznosciLogowania", 48,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Ile godzin ważny jest cookies do logowania", ustawieniaGrupa.Klienci);
            }
        }
        
        public string EnovaPoleDoZapisuPlatnikaDokumentu
        {
            get
            {
                return Settings.GetSettingString("EnovaPoleDoZapisuPlatnikaDokumentu", "Kontrahent",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Pole do zapisu płatnika dokumentu",
                     ustawieniaGrupa.SynchronizacjaEnova);
            }
        }

        public bool ListyPrzewozoweDokumentPowiazany
        {
            get
            {
                return Settings.GetSettingBool("ListyPrzewozoweDokumentPowiazany", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Czy w podglądzie dokumentów pokazywać listy przewozowe z dokumentu powiązanego",
                     ustawieniaGrupa.Dokumenty);
            }
        }

        public string[] SeparatoryDrzewkaKategorii
        {
            get
            {
                var str = Settings.GetSettingString("SeparatoryDrzewkaKategorii", "/\\",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Separatory w drzewku kateogrii",
                     ustawieniaGrupa.Produkty);
                List<string> wynik = new List<string>();
                if (!string.IsNullOrEmpty(str))
                {
                    wynik.AddRange(str.Select(c => c.ToString()));
                }
                return wynik.ToArray();
            }
        }

        
        public bool EnovaZamowieniaBufor
        {
            get
            {
                return Settings.GetSettingBool("EnovaZamowieniaBufor", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Czy zamówienia zapisywane są w buforze", grupaUstawien: ustawieniaGrupa.SynchronizacjaEnova);
            }
        }

        public void UsunZbednychUstawienia()
        {
            bool czyOdswierzyc = false;
            List<Ustawienie> settings = SettingsBase.GetSettingsList();
            foreach (var setting in settings)
            {
                if (setting.Typ == TypUstawienia.Enum || setting.Typ == TypUstawienia.Refleksja)
                {
                    if (Type.GetType(setting.Slownik, false) == null)
                    {
                        _provider.UsunUstawienie(setting);
                        // Calosc.DostepDane.Usun<ustawienia>(setting.Id_ustawienia);
                        czyOdswierzyc = true;
                    }
                }
            }
            if (czyOdswierzyc)
            {
                RefreshData();
            }
        }
        

        /// <summary>
        ///
        /// </summary>
        public bool PokazujDymek
        {
            get
            {
                return Settings.GetSettingBool("PokazujDymek", true, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     opis: "Czy pokazywać dymek z informacjami o jesdnostkach",
                    grupaUstawien: ustawieniaGrupa.Produkty);
            }
        }

        public int? SubiektKodTransakcjiDlaKlientowEU
        {
            get
            {
                return Settings.GetSettingInt("SubiektKodTransakcjiDlaKlientowEU", -1,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Kod rodzaju transakcji dla klientów EU", grupaUstawien: ustawieniaGrupa.SynchronizacjaSubiekt);
            }
        }
        
        public bool MaileTylkoSolex { get; set; }

        public string ZdjecieRozmiarWPowiadomieniach { get; set; }

        public HashSet<PolaListaProduktowNoweZamowienie> KtorePolaProduktuPowiadomienieNoweZamowienie { get; set; }
       

        public HashSet<PolaListaProduktowNoweZamowienie> KtorePolaProduktuPowiadomienieDostepneProdukty
        {
            get
            {
                HashSet<PolaListaProduktowNoweZamowienie> domyslne = new HashSet<PolaListaProduktowNoweZamowienie> { PolaListaProduktowNoweZamowienie.Ean, PolaListaProduktowNoweZamowienie.Symbol, PolaListaProduktowNoweZamowienie.Zdjecie };
                return Settings.GetSettingEnum("KtorePolaProduktuPowiadomienieDostepneProdukty", domyslne,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Które pola zamieścić w mailu informującym o dostępnych produktach",
                    ustawieniaGrupa.Maile_i_powiadomienia, multi: true, podgrupa: TypUstawieniaPodgrupa.Mail_o_produktach);
            }
        }

        public bool SubiektSzyfrujHaslo
        {
            get
            {
                return Settings.GetSettingBool("SubiektSzyfrujHaslo", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis: "Czy szyfrowac haslo operatora w subiekcie",
                    grupaUstawien: ustawieniaGrupa.SynchronizacjaSubiekt);
            }
        }

        public string SubiektPodmiot
        {
            get
            {
                return Settings.GetSettingString("SubiektPodmiot", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                   "Subiekt nazwa podmiotu", ustawieniaGrupa.SynchronizacjaSubiekt);
            }
        }
        
        public bool PokazujDodajDoKoszykaFaktura { get; set; }

        public bool SprawdzanieAdresuIp
        {
            get
            {
                return Settings.GetSettingBool("SprawdzanieAdresuIp", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    opis:
                        "Czy sprawdzać adres ip klienta z którego sięloguje i wymagać aby adres był ten sam lub klient go potwierdzał ",
                    grupaUstawien: ustawieniaGrupa.Klienci);
            }
        }
        public string GoogleTranslateKey
        {
            get
            {
                return Settings.GetSettingString("GoogleTranslateKey", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, opis: "Klucz google translate",
                    grupaUstawien: ustawieniaGrupa.Systemowe);
            }
        }

        public string GoogleApiKey
        {
            get
            {
                return Settings.GetSettingString("google_api_key", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, opis: "Klucz google api",
                    grupaUstawien: ustawieniaGrupa.Systemowe);
            }
        }
       
        public string NazwaPlatformy(int jezykId)
        {
            return PobierzTlumaczenie(jezykId,"Solex B2B");
        }


        public int MailingIleNaRaz
        {
            get
            {
                return Settings.GetSettingInt("MailingIleNaRaz", 100, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Ile maili ma być wysłane w jednej paczce podczas mailingu", ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Newsletter);
            }
        }

        public bool EppKodKresowyKod
        {
            get
            {
                return Settings.GetSettingBool("epp_kod_kreskowy_kod", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Czy w plikach epp jako kod produktu wstawiać kod kreskowy, gdy go brakuje i tak będzie wstawony kod",
                    ustawieniaGrupa.Dokumenty);
            }
        }

        public HashSet<int> DeaktywujMinimumLogistyczneDlaWybranychKategoriiKlientow { get;set; }

        public virtual string PobierzSynchronizacjaPoleUstawienie(string pole, string domyslne, Dictionary<string, string> pars, ustawieniaGrupa grupa, string obiekt = "produktu", string opis = null)
        {
            string symbolUstawienia = string.Format("b2b_{0}", pole);
            if (_slownikUStawienia.ContainsKey(symbolUstawienia))
            {
                return _slownikUStawienia[symbolUstawienia];
            }
            string opisUstawiania = string.IsNullOrEmpty(opis) ?
                string.Format(
                    "Pole z którego będzie pobrane: {0} {1}.",
                    pole, obiekt) : opis;

            IEnumerable<string> listaWartosciPol = pars.Keys;

            HashSet<string> dom = new HashSet<string>();
            if (!string.IsNullOrEmpty(domyslne))
            {
                dom.Add(domyslne);
            }
            var s = Settings.GetSettingSlownik(symbolUstawienia, dom, listaWartosciPol, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, opisUstawiania, grupa, false, true, false, TypUstawieniaPodgrupa.Brak, true);
            _slownikUStawienia.Add(symbolUstawienia, s.FirstOrDefault() ?? "");
            return _slownikUStawienia[symbolUstawienia];
        }

      
        public void SynchronizacjaPobierzPoleLiczba1(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("PoleLiczba1", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.PoleLiczba1 = d;
            }
        }

        public void SynchronizacjaPobierzWidocznoscProduktuZPola(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("eksportowanie_produktow_wzgledem_pola_ERP", domyslnePole, pars, ustawieniaGrupa.Produkty, "",
                "Eksportowanie produktów na b2b wg. pola z ERP (tak/nie, yes/no, t/n, y/f, 1/0, true/false). Jeśli pole na produkcie będzie puste oznacza False czyli produkt nie jest eksportowany");
            if (string.IsNullOrEmpty(pole))
            {
                item.Widoczny = true;
                return;
            }

            item.Widoczny = false;
            string wart = PobierzWartosc(pole, pars);
            bool wynik;

            if (!string.IsNullOrWhiteSpace(wart) && Refleksja.PrzetworzBool(wart, out wynik))
            {
                item.Widoczny = wynik;
            }
        }
        
        public void SynchronizacjaPobierzGlebokoscOpakowaniaJednostkowego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpJednostkoweGlebokosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Głębokość opakowania jednostkowego");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpJednostkoweGlebokosc = d;
            }
        }

        public void SynchronizacjaPobierzSzerokoscOpakowaniaJednostkowego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpJednostkoweSzerokosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Szewrokość opakowania jednostkowego");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpJednostkoweSzerokosc = d;
            }
        }

        public void SynchronizacjaPobierzWysokoscOpakowaniaJednostkowego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpJednostkoweWysokosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Wysokość opakowania jednostkowego");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpJednostkoweWysokosc = d;
            }
        }

        public void SynchronizacjaPobierzGlebokoscOpakowaniaZbiorczego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpZbiorczeGlebokosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Głębokość opakowania zbiorszego");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpZbiorczeGlebokosc = d;
            }
        }

        public void SynchronizacjaPobierzSzerokoscOpakowaniaZbiorczego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpZbiorczeSzerokosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Szerokość opakowania zbiorszego");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpZbiorczeSzerokosc = d;
            }
        }

        public void SynchronizacjaPobierzWysokoscOpakowaniaZbiorczego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpZbiorczeWysokosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Wysokość opakowania zbiorszego");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpZbiorczeWysokosc = d;
            }
        }

        public void SynchronizacjaPobierzWageOpakowaniaZbiorczego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpZbiorczeWaga", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Waga opakowania zbiorszego");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpZbiorczeWaga = d;
            }
        }

        public void SynchronizacjaPobierzObjetoscOpakowaniaZbiorczego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpZbiorczeObjetosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Objętość opakowania zbiorszego");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpZbiorczeObjetosc = d;
            }
        }

        public void SynchronizacjaPobierzObjetoscProduktu(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("ObjetoscProduktu", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Objętość produktu");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.Objetosc = d;
            }
        }
        public void SynchronizacjaPobierzWageProduktu(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("WagaProduktu", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Waga produktu");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.Waga = Math.Round(d,3,MidpointRounding.AwayFromZero);
            }
        }

        public void SynchronizacjaPobierzIloscWOpakowaniuZbiorczym(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpZbiorczeIloscWOpakowaniu", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Ilość w opakowaniu zbiorczym");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpZbiorczeIloscWOpakowaniu = d;
            }
        }

        public void SynchronizacjaPobierzGlebokoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpPaletaGlebokosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Głębokość palety");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpPaletaGlebokosc = d;
            }
        }

        public void SynchronizacjaPobierzSzerokoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpPaletaSzerokosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Szerokość palety");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpPaletaSzerokosc = d;
            }
        }

        public void SynchronizacjaPobierzWysokoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpPaletaWysokosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Wysokość palety");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpPaletaWysokosc = d;
            }
        }

        public void SynchronizacjaPobierzWagePalety(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpPaletaWaga", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Waga palety");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpPaletaWaga = d;
            }
        }

        public void SynchronizacjaPobierzLiczbaSztukNaWarstwie(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpPaletaIloscNaWarstwie", domyslnePole, pars, ustawieniaGrupa.Produkty,"","Ilośc sztuk w warstwie na palecie");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpPaletaIloscNaWarstwie = d;
            }
        }
        public void SynchronizacjaPobierzObjetoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpPaletaObjetosc", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Objętość palety");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpPaletaObjetosc = d;
            }
        }

        public void SynchronizacjaPobierzLiczbaSztukNaPalecie(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpPaletaIloscNaPalecie", domyslnePole, pars, ustawieniaGrupa.Produkty, "", "Ilośc sztuk na palecie");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.OpPaletaIloscWOpakowaniu = d;
            }
        }
        public void SynchronizacjaPobierzPoleLiczba2(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("PoleLiczba2", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.PoleLiczba2 = d;
            }
        }

        public void SynchronizacjaPobierzCenaWPunktach(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("CenaWPunktach", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.CenaWPunktach = d;
            }
        }

        public void SynchronizacjaPobierzIloscMinimlna(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("ilosc_minimalna", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.IloscMinimalna = d;
            }
        }

        public void SynchronizacjaPobierzIloscWOpakowaniu(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("IloscWOpakowaniu", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.IloscWOpakowaniu = d;
            }
        }

        public void SynchronizacjaPobierzPoleLiczba3(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("PoleLiczba3", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.PoleLiczba3 = d;
            }
        }

        public void SynchronizacjaPobierzPoleLiczba4(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("PoleLiczba4", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.PoleLiczba4 = d;
            }
        }

        public void SynchronizacjaPobierzPoleLiczba5(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("pole_liczba5", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.PoleLiczba5 = d;
            }
        }

        public void SynchronizacjaPobierzKolumnaLiczba1(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("KolumnaLiczba1", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.KolumnaLiczba1 = (int)d;
            }
        }

        public void SynchronizacjaPobierzKolumnaLiczba2(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("KolumnaLiczba2", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.KolumnaLiczba2 = (int)d;
            }
        }

        public void SynchronizacjaPobierzKolumnaLiczba3(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("kolumna_liczba3", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.KolumnaLiczba3 = (int)d;
            }
        }

        public void SynchronizacjaPobierzKolumnaLiczba4(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("KolumnaLiczba4", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.KolumnaLiczba4 = (int)d;
            }
        }

        public void SynchronizacjaPobierzPoleTekst1(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("PoleTekst1", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            item.PoleTekst1 = wart;
        }

        public void SynchronizacjaPobierzPoleTekst2(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("PoleTekst2", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            item.PoleTekst2 = wart;
        }

        public void SynchronizacjaPobierzPoleTekst3(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("PoleTekst3", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            item.PoleTekst3 = wart;
        }

        public void SynchronizacjaPobierzPoleTekst4(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("PoleTekst4", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            item.PoleTekst4 = wart;
        }

        public void SynchronizacjaPobierzPoleTekst5(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("PoleTekst5", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            item.PoleTekst5 = wart;
        }


        public void SynchronizacjaPobierzPoleSkype(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("Skype", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            item.Skype = wart;
        }

        public void SynchronizacjaPobierzPoleGaduGadu(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("GaduGadu", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            item.GaduGadu = wart;
        }

        public void SynchronizacjaPobierzPoleMagazynDomyslny(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("MagazynDomyslny", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            if (item.DostepneMagazyny == null && wart != null)
            {
                var symbol = wart.Split(';').First();
                item.MagazynDomyslny = symbol;
            }
            else
            {
                item.MagazynDomyslny = wart;
            }
            
        }

        public void SynchronizacjaPobierzPoleBlokadaZamowien(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("BlokadaZamowien", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            if (!string.IsNullOrEmpty(wart))
            {
                bool wynik;
                Refleksja.PrzetworzBool(wart, out wynik);
                item.BlokadaZamowien = wynik;
            }
        }

        public void SynchronizacjaPobierzPoleIndywidualnaStawaVat(Model.Klient item, string domyslnePole,
            Dictionary<string, string> pars, bool kontrahentue, bool krajue)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("IndywidualnaStawaVat", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            decimal d;

            item.KlientEu = kontrahentue; //jesli klient jest NIE EU, ale kraj jest inny niz Polska to powinna tam byc 1
            item.Eksport = (!kontrahentue && !krajue); //nie jest podatnikiem zarejestowany w ue, nie jest polakiem i nie jest z kraju ue
            if (item.KlientEu || item.Eksport)
            {
                item.IndywidualnaStawaVat = 0;
            }
            else
            {
                item.IndywidualnaStawaVat = null;
            }
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.IndywidualnaStawaVat = (int)d;
            }
        }

        public void SynchronizacjaPobierzMinimalnaWartoscZamowienia(Model.Klient item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("MinimalnaWartoscZamowienia", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.MinimalnaWartoscZamowienia = d;
            }
        }

        public void SynchronizacjaPobierzPoleDomyslnaWaluta(Model.Klient item, string domyslnePole, string waluta, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("WalutaId", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            if (!string.IsNullOrEmpty(waluta))
            {
                item.WalutaId = waluta.ToLower().WygenerujIDObiektuSHAWersjaLong();
            }
            if (!string.IsNullOrEmpty(wart))
            {
                item.WalutaId = wart.ToLower().WygenerujIDObiektuSHAWersjaLong();
            }
            if (item.WalutaId == 0 && !string.IsNullOrEmpty(DomyslnaWaluta))
            {
                item.WalutaId = DomyslnaWaluta.ToLower().WygenerujIDObiektuSHAWersjaLong();
            }
        }

        public void SynchronizacjaPobierzPoleOpiekun(Model.Klient item, string domyslnePole, Dictionary<string, string> pars, string domyslny = null)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("OpiekunId", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            if (string.IsNullOrEmpty(wart))
            {
                wart = domyslny;
            }
            if (!string.IsNullOrEmpty(wart))
            {
                item.PrzedstawicieleOpiekunowie.Add(OpiekunowiePrzedstawiciele.Opiekun, wart);
            }
        }

        public void SynchronizacjaPobierzPoleDrugiOpiekun(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("DrugiOpiekunId", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            if (!string.IsNullOrEmpty(wart))
            {
                item.PrzedstawicieleOpiekunowie.Add(OpiekunowiePrzedstawiciele.DrugiOpiekun, wart);
            }
        }

        public void SynchronizacjaPobierzPolePrzedstawiciel(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("PrzedstawicielId", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            if (!string.IsNullOrEmpty(wart))
            {
                item.PrzedstawicieleOpiekunowie.Add(OpiekunowiePrzedstawiciele.Przedstawiciel, wart);
            }
        }

        public void SynchronizacjaPobierzPoleKlientNadrzedny(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("KlientNadrzednyId", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            if (!string.IsNullOrEmpty(wart))
            {
                item.PrzedstawicieleOpiekunowie.Add(OpiekunowiePrzedstawiciele.KlientNadrzedny, wart);
            }
        }

        public void SynchronizacjaPobierzPoleHasloZrodlowe(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PoleHasloPobierz(domyslnePole, pars);
            string wart = PobierzWartosc(pole, pars);
            item.HasloZrodlowe = wart;
        }

        public void SynchronizacjaPobierzPoleJezyk(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("JezykId", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);

            if (!string.IsNullOrEmpty(wart))
            {
                var jezyk = JezykiWSystemie.Values.FirstOrDefault(p => p.Symbol == wart);
                if (jezyk != null)
                {
                    item.JezykId = jezyk.Id;
                }
                Log.DebugFormat("Przypisanie języka do klienta - brak języka o symbolu: {0} - język był pobierany z pola: {1}", wart, pole);
            }
            if (item.JezykId == null)
            {
                item.JezykId = JezykiWSystemie.First().Value.Id;
            }
        }

        public void SynchronizacjaPobierzKredytWykorzystano(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("IloscWykorzystanegoKredytu", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            decimal wartosc;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out wartosc))
            {
                item.IloscWykorzystanegoKredytu = wartosc;
            }
        }

        public void SynchronizacjaPobierzDostepneMagazyny(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("DostepneMagazyny", domyslnePole, pars, ustawieniaGrupa.Klienci, "klienta");
            string wart = PobierzWartosc(pole, pars);
            if (!string.IsNullOrEmpty(wart))
            {
                item.DostepneMagazyny =new HashSet<string>( wart.Split(';') );
            }
           
        }
        
        public void SynchronizacjaPobierzKredytLimit(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("LimitKredytu", domyslnePole, pars, ustawieniaGrupa.Klienci, "Klient");
            string wart = PobierzWartosc(pole, pars);
            decimal wartosc;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out wartosc))
            {
                item.LimitKredytu = wartosc;
            }
        }

        public void SynchronizacjaPobierzKredytPozostalo(Model.Klient item, string domyslnePole, Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("IloscPozostalegoKredytu", domyslnePole, pars, ustawieniaGrupa.Klienci, "Klient");
            string wart = PobierzWartosc(pole, pars);
            decimal wartosc;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out wartosc))
            {
                item.IloscPozostalegoKredytu = wartosc;
            }
        }

        public void SynchronizacjaUstawPoziomCeny(Model.Klient item, int? poziom)
        {
            item.PoziomCenowyId = poziom ?? GetPriceLevelHurt;
        }

        public string PoleHasloPobierz(string domyslne, Dictionary<string, string> pars)
        {
            Dictionary<string, string> pol = new Dictionary<string, string>(pars);
            if (!pol.Any())
            {
                pol.Add(string.IsNullOrEmpty(domyslne) ? "solex-haslo" : domyslne, "");
            }
            return PobierzSynchronizacjaPoleUstawienie("haslo", string.IsNullOrEmpty(domyslne) ? "solex-haslo" : domyslne, pol, ustawieniaGrupa.Klienci, "klienta", "Zalecany typ pola to pole tekstowe, dla enovy ewentualnie pole tybu bool");
        }

        public void SynchronizacjaPobierzPoleEmail(Model.Klient item, string email)
        {
            bool emailMulti = !string.IsNullOrEmpty(SeparatorMail);
            string wart = email;
            string[] ss = email.Split(new[] { SeparatorMail }, StringSplitOptions.RemoveEmptyEntries);
            if (emailMulti)
            {
                if (ss.Length < 2)
                {
                    wart = ss.ElementAtOrDefault(0);
                }
                else
                {
                    if (KtoryEmailKlienta == -1)
                    {
                        wart = Serializacje.PobierzInstancje.SerializeList(ss.ToList().Where(p => string.IsNullOrEmpty(CustomerEmailNotWord) || !p.Contains(CustomerEmailNotWord)).ToList(), SeparatorMail[0]);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CustomerEmailNotWord))
                        {
                            wart = ss.Where(p => !p.Contains(CustomerEmailNotWord)).ElementAtOrDefault(KtoryEmailKlienta);
                        }
                        else
                        {
                            wart = ss.ElementAtOrDefault(KtoryEmailKlienta);
                        }
                    }
                }
            }
            item.Email = wart;
            item.Email = item.Email == null ? item.Email : item.Email.Trim();
        }

        public string DomyslnaWaluta
        {
            get { return Settings.GetSettingString("b2b_waluta_domyslna", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Symbol domyślnej waluty", ustawieniaGrupa.Synchronizacja); }
        }

        //Ustawienie określające jaki typ dokumentów pobieramy z Xl na platformę 
        //usunięte bo i tak nik nie zna id typów dokumentów więc na stałe dajemu to w providerze 
        //public int XlTypDokumentu
        //{
        //    get { return Settings.GetSettingInt("xl_typ_dokumentu", 960, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, opis: "Xl typ dokumnetu na platformie", grupaUstawien: ustawieniaGrupa.SynchronizacjaXL); }
        //}

        public void SynchronizacjaPobierzKolumnaLiczba5(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            string pole = PobierzSynchronizacjaPoleUstawienie("KolumnaLiczba5", domyslnePole, pars, ustawieniaGrupa.Produkty);
            string wart = PobierzWartosc(pole, pars);
            decimal d;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(wart, out d))
            {
                item.KolumnaLiczba5 = (int)d;
            }
        }

        public int IdDomyslnegoSablonuCsvDoEksportu
        {
            get
            {
                return Settings.GetSettingInt("csv_domyslny_szablon", 4,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Id domyślnego szablonu eksportu csv", ustawieniaGrupa.Systemowe);
            }
        }

        public bool PokazujOpiekunaWNaglowku
        {
            get
            {
                return Settings.GetSettingBool("PokazujOpiekunaWNaglowku", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy w nagłówku pokazywać opiekuna", ustawieniaGrupa.Wygląd_Nagłówek);
            }
        }

        public bool SklepyMapaNazwaPokazuj { get; set; }

        //Zmiana domyślnej wartości ze względu na prośbę Bartka.
        public int MaksymalnaIloscPozycjiWKoszyku { get; set; }
        public bool SklepyMapaUlicaINrPokazuj
        {
            get
            {
                return Settings.GetSettingBool("SklepyMapaUlicaINrPokazuj", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy pokazywać ulicę i numer", ustawieniaGrupa.MapaSklepów);
            }
        }

        public bool SklepyMapaMiastoPokazuj
        {
            get
            {
                return Settings.GetSettingBool("SklepyMapaMiastoPokazuj", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy pokazywać miasto", ustawieniaGrupa.MapaSklepów);
            }
        }

        public bool SklepyMapaWWWPokazuj
        {
            get
            {
                return Settings.GetSettingBool("SklepyMapaWWWPokazuj", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy pokazywać stronę www", ustawieniaGrupa.MapaSklepów);
            }
        }

        public bool SklepyMapaEmailPokazuj
        {
            get
            {
                return Settings.GetSettingBool("SklepyMapaEmailPokazuj", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy pokazywać adres email", ustawieniaGrupa.MapaSklepów);
            }
        }

        public bool SklepyMapaTelefonPokazuj
        {
            get
            {
                return Settings.GetSettingBool("SklepyMapatelefonPokazuj", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy pokazywać telefon", ustawieniaGrupa.MapaSklepów);
            }
        }

        public bool SklepyMapaKodPocztowyPokazuj
        {
            get
            {
                return Settings.GetSettingBool("SklepyMapaKodPocztowyPokazuj", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy pokazywać kod pocztowy", ustawieniaGrupa.MapaSklepów);
            }
        }

     
        public bool BrakPlatnosciKlientaJesliTerminJestZerowy
        {
            get { return Settings.GetSettingBool("BrakPlatnosciKlientaJesliTerminJestZerowy", false, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Jeśli klient ma ustawiony termin płatności należności na 0 to nie eksportuj jego płatności", ustawieniaGrupa.SynchronizacjaWFMag); }
        }

    
        

        public string EmailHost
        {
            get
            {
                return Settings.GetSettingString("email_host", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Adres serwera pocztowego z którego wysyłane są maile",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_ogólnej);
            }
        }

        public string EmailNazwaUzytkownika
        {
            get
            {
                return Settings.GetSettingString("email_user", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Nazwa użytkownika do logowania się dla skrzynki mailowej - zazwyczaj email",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_ogólnej);
            }
        }

        public string EmailHaslo
        {
            get
            {
                return Settings.GetSettingPassword("email_pass", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Hasło użytkownika dla skrzynki z której wysyłane są maile",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_ogólnej);
            }
        }

        public bool EmailSzyfrowanie
        {
            get
            {
                return Settings.GetSettingBool("email_ssl", false, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                   "Czy połączenie jest szyfrowane dla skrzynki z której wysyłane są maile",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_ogólnej);
            }
        }

        public int EmailPort
        {
            get
            {
                return Settings.GetSettingInt("email_port", 587, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Port dla skrzynki pocztowej z której wysyłane są maile",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_ogólnej);
            }
        }

        public int EmailTimeout
        {
            get
            {
                return Settings.GetSettingInt("email_timeout", 5000, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Timeout dla skrzynki pocztowej z której wysyłane są maile",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_ogólnej);
            }
        }

        public string EmailFromPrzyjaznaNazwa
        {
            get
            {
                return Settings.GetSettingString("email_from_przyjazna_nazwa", "SolEx B2B", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Przyjazna nazwa nadawcy dla maili - to nie jest email", ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_ogólnej);
            }
        }

        public string EmailFrom
        {
            get
            {
                return Settings.GetSettingString("email_from", "SolEx B2B", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Adres email z jakiego są wysyłane maile", ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_ogólnej);
            }
        }



        public string MailingEmailHost
        {
            get
            {
                return Settings.GetSettingString("mailing_email_host", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Adres serwera pocztowego z którego wysyłane są newslettery",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_newsletter);
            }
        }

        public string MailingEmailNazwaUzytkownika
        {
            get
            {
                return Settings.GetSettingString("mailing_email_user", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Nazwa użytkownika do logowania się dla skrzynki z której wysyłane są newslettery - zazwyczaj email",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_newsletter);
            }
        }

        public string MailingEmailHaslo
        {
            get
            {
                return Settings.GetSettingPassword("mailing_email_pass", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Hasło użytkownika dla skrzynki z której wysyłane są newslettery",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_newsletter);
            }
        }

        public bool MailingEmailSzyfrowanie
        {
            get
            {
                return Settings.GetSettingBool("mailing_email_ssl", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Czy połączenie jest szyfrowane dla skrzynki z której wysyłane są newslettery",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_newsletter);
            }
        }

        public MiejsceOpisuZbiorczegoZKategorii OpisZbiorczyZKategorii
        {
            get
            {
                HashSet<MiejsceOpisuZbiorczegoZKategorii> domyslne = new HashSet<MiejsceOpisuZbiorczegoZKategorii> { MiejsceOpisuZbiorczegoZKategorii.PodOpisami };
                return Settings.GetSettingEnum("MiejsceOpisuZbiorczegoZKategorii", domyslne, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "W którym miejscu znajdować się ma zbiorczy opis z kategorii", ustawieniaGrupa.Wygląd_Karta_Produktu).FirstOrDefault();
            }
        }

        public int MailingEmailPort
        {
            get
            {
                return Settings.GetSettingInt("mailing_email_port", 587,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Port dla skrzynki pocztowej z której wysyłane są newslettery",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_newsletter);
            }
        }

        public int MailingEmailTimeout
        {
            get
            {
                return Settings.GetSettingInt("mailing_email_timeout", 5000,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Timeout dla skrzynki pocztowej z której wysyłane są newslettery",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_newsletter);
            }
        }

        public string MailingEmailFrom
        {
            get
            {
                return Settings.GetSettingString("mailing_email_from", "SolEx B2B",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Adres email z jakiego są wysyłane maile",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_newsletter);
            }
        }


        public string MailingEmailFromPrzyjaznaNazwa
        {
            get
            {
                return Settings.GetSettingString("mailing_email_from_przyjazna_nazwa", "SolEx B2B",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Przyjazna nazwa nadawcy dla newslettera - to nie jest email z jakiego są rozsyłane wiadomości",
                    ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_newsletter);
            }
        }



        public string EmailCustomerError
        {
            get
            {
                return Settings.GetSettingString("email_customer_error", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Na jaki adres wysyłać maile o błędach synchronizacji. ",
                    ustawieniaGrupa.Maile_i_powiadomienia);
            }
        }
        
        public int MailPrzerwaPoIluBledach
        {
            get
            {
                return Settings.GetSettingInt("mailing_przerwa_po_ilu_bledach", 5,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Ilosc bledow ?",
                    grupaUstawien: ustawieniaGrupa.Maile_i_powiadomienia, podgrupa: TypUstawieniaPodgrupa.Konfiguracja_skrzynki_newsletter);
            }
        }

        public bool DolaczEtykiete
        {
            get
            {
                return Settings.GetSettingBool("DolaczacEtykiety", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Dolaczanie etykiety");
            }
        }

        public decimal MaxPokazywanyStan { get; set; }

        public int? IdAtrybutuDostawy
        {
            get { return Settings.GetSettingInt("dostawa_id_atrybutu", 0, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, grupaUstawien: ustawieniaGrupa.Zamówienia); }
        }

        public int? IleWczesniejZmianaDostawa
        {
            get { return Settings.GetSettingInt("dostawa_ile_wczesniej_zmiana", 12, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, grupaUstawien: ustawieniaGrupa.Zamówienia); }
        }

        public int AutoCompleteCount
        {
            get { return Settings.GetSettingInt("auto_complete_count", 3, SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public string TypDomyslnyFiltru { get; set; }

        /// <summary>
        /// Do wyszukiwania dołącza zamienniki i akcesoria
        /// </summary>
        public bool WyszukiwanieZamienniki
        {
            get
            {
                return Settings.GetSettingBool("WyszukiwanieZamienniki", false,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy automatycznie dołączać wszystkie zamienniki i akcesoria podczas wyszukiwania produktów", ustawieniaGrupa.Produkty);
            }
        }

        public bool DomyslnaZgodaNaNewsletter { get; set; }

        public bool LimityOdCenyNetto
        {
            get
            {
                return Settings.GetSettingBool("limity_liczone_od_cen_netto", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Limit zamowien liczony od ceny netto", ustawieniaGrupa.Zamówienia);
            }
        }

        public bool AtrybutZCechy
        {
            get { return Settings.GetSettingBool("attr_in_trait", true, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Wyciaganie atrybutu na podstawie cechy"); }
        }

        public string KatalogDoZapisuZalacznikowZFormularzy
        {
            get
            {
                return Settings.GetSettingString("KatalogDoZapisuZalacznikowFormularzy", "/Zasoby/formularze/",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        public int CzasWyswietlaniaKoszyka
        {
            get
            {
                return Settings.GetSettingInt("koszyk_popup_czas", 3000,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany, grupaUstawien: ustawieniaGrupa.Koszyk);
            }
        }

        private long? _IdDomyslnyOpiekunZalogowani;
        private long? _IdDomyslnyOpiekunNieZalogowani;

        public long IdDomyslnyOpiekun
        {
            get
            {
                if (SesjaHelper.PobierzInstancje.CzyKlientZalogowany)
                {
                    if (_IdDomyslnyOpiekunZalogowani == null)
                    {
                        _IdDomyslnyOpiekunZalogowani = Settings.GetSettingInt("domyslny_opiekun_id", 0, true, "Id domyslnego opiekuna");
                    }
                    return _IdDomyslnyOpiekunZalogowani.Value;
                }

                if (_IdDomyslnyOpiekunNieZalogowani == null)
                {
                    _IdDomyslnyOpiekunNieZalogowani = Settings.GetSettingInt("domyslny_opiekun_id", 0, false, "Id domyslnego opiekuna");
                }
                return _IdDomyslnyOpiekunNieZalogowani.Value;
            }
            set {
                Settings.SetSetting("domyslny_opiekun_id", value == 0 ? null : value.ToString(CultureInfo.InvariantCulture), TypUstawienia.Long);
            }
        }

        private long? _IdDomyslnyPrzedstawicielZalogowani;
        private long? _IdDomyslnyPrzedstawicielNiezalogowani;

        public long IdDomyslnyPrzedstawiciel
        {
            get
            {
                if (SesjaHelper.PobierzInstancje.CzyKlientZalogowany)
                {
                    if (_IdDomyslnyPrzedstawicielZalogowani == null)
                    {
                        _IdDomyslnyPrzedstawicielZalogowani = Settings.GetSettingInt("domyslny_przedstawiciel_id", 0, true);
                    }
                    return _IdDomyslnyPrzedstawicielZalogowani.Value;
                }

                if (_IdDomyslnyPrzedstawicielNiezalogowani == null)
                {
                    _IdDomyslnyPrzedstawicielNiezalogowani = Settings.GetSettingInt("domyslny_przedstawiciel_id", 0, false);
                }
                return _IdDomyslnyPrzedstawicielNiezalogowani.Value;
            }
            set { Settings.SetSetting("domyslny_przedstawiciel_id", value == 0 ? null : value.ToString(CultureInfo.InvariantCulture), TypUstawienia.Long); }
        }

        private long? _IdDrugiDomyslnyOpiekunZalogowany;
        private long? IdDrugiDomyslnyOpiekunNiezalogowany;

        public long IdDrugiDomyslnyOpiekun
        {
            get
            {
                if (SesjaHelper.PobierzInstancje.CzyKlientZalogowany)
                {
                    if (_IdDrugiDomyslnyOpiekunZalogowany == null)
                    {
                        _IdDrugiDomyslnyOpiekunZalogowany = Settings.GetSettingInt("domyslny_drugi_opiekun_id", 0, true);
                    }
                    return _IdDrugiDomyslnyOpiekunZalogowany.Value;
                }

                if (IdDrugiDomyslnyOpiekunNiezalogowany == null)
                {
                    IdDrugiDomyslnyOpiekunNiezalogowany = Settings.GetSettingInt("domyslny_drugi_opiekun_id", 0, false);
                }
                return IdDrugiDomyslnyOpiekunNiezalogowany.Value;
            }
            set { Settings.SetSetting("domyslny_drugi_opiekun_id", value == 0 ? null : value.ToString(CultureInfo.InvariantCulture), TypUstawienia.Long); }
        }

        public bool DadawanieAtrybutuDoKategorii
        {
            get { return Settings.GetSettingBool("dodawaj_atrybut_do_kategorii", false, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Dodawanie symbolu atrybutu do nazwy cechy - pomocne główniew Optimie jeśli atrybuty nie są unikalne"); }
        }

        public string CustomerEmailNotWord
        {
            get { return Settings.GetSettingString("customers_email_not_word", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public int KtoryEmailKlienta
        {
            get { return Settings.GetSettingInt("customers_email_which", 0, SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public NameValueCollection DoKolekcjiWartosciNazw()
        {
            return Settings.ToNameValueCollection();
        }

        public string BazaXl
        {
            get { return Settings.GetSettingString("xl_baza", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Baza podmiotu", ustawieniaGrupa.SynchronizacjaXL); }
        }

        public bool LiczonyOdCenyNetto { get; set; }
        

        public int XlTypDokumentuZamowienia
        {
            get { return Settings.GetSettingInt("xl_typ_dokumentu_zam", 6, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Xl typ dokumnetu zamówienia na platformie", ustawieniaGrupa.SynchronizacjaXL); }
        }

        public int XlIdFirmy
        {
            get { return Settings.GetSettingInt("xl_firma_id", 6, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Xl id firmy", ustawieniaGrupa.SynchronizacjaXL); }
        }

        public string BazowaData
        {
            get { return Settings.GetSettingString("xl_date_base", "18001228", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Bazowa data", ustawieniaGrupa.SynchronizacjaXL); }
        }

        public string OpakowanieSql
        {
            get { return Settings.GetSettingString("xl_opk", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Opakowanie sql", ustawieniaGrupa.SynchronizacjaXL); }
        }

        public string IloscMinSql
        {
            get { return Settings.GetSettingString("xl_min", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Ilość minimalna sql", ustawieniaGrupa.SynchronizacjaXL); }
        }

        public string XlMagazynuZapisZamowien
        {
            get { return Settings.GetSettingString("XlMagazynZapisZamowieni", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Symbol magazynu na którym zapisywane są zamówienia"); }
        }

        public int XlPoziomKursu
        {
            get
            {
                return Settings.GetSettingInt("xl_pozom_kursu", 4, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "numer poziomu kursów z którego bieżemy kurs dla waluty",
                    ustawieniaGrupa.SynchronizacjaXL);
            }
        }

        public int CzasDoZamknieciaSynchronizacji
        {
            get
            {
                return Settings.GetSettingInt("czas_do_zamkniecia_synchronizatora", 5,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        public Szerokosci SzerokoscMenuDlaArtykulow
        {
            get
            {
                HashSet<Szerokosci> domyslne = new HashSet<Szerokosci> { Szerokosci.Szerokosc_3 };
                return Settings.GetSettingEnum("SzerokoscMenuArtykulow", domyslne, SesjaHelper.PobierzInstancje.CzyKlientZalogowany).First();

                //return Settings.GetSettingInt("SzerokoscMenuArtykulow", 3, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, true);
            }
        }

        public string KluczGoogleAnalytycs
        {
            get
            {
                return Settings.GetSettingString("google_analytycs_klucz", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

    
        public string IleProduktowPokazacNaStronie
        {
            get { return Settings.GetSettingString("ile_pokazac_produktow_na_stronie", "100;50;200", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public bool ImieNazwiskoKlienta
        {
            get { return Settings.GetSettingBool("full_name", false, SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public string WfmagPoleEmailPracownika { get; set; }

        public string WfmagPoleHasloPracownika { get; set; }
        
        public int SubiektStatusDokumentu
        {
            get { return Settings.GetSettingInt("subiekt_status_dokumentu", 7, SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public string OptimaNazwaFirmy
        {
            get { return Settings.GetSettingString("optima_nazwa_firmy", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public string B2bFaktoring
        {
            get { return Settings.GetSettingString("b2b_faktoring", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public int OptimaRodzajDokumentu
        {
            get { return Settings.GetSettingInt("optima_rodzaj_dokumentu", 308000, SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public int OptimaTypDokumentu
        {
            get { return Settings.GetSettingInt("optima_typ_dokumentu", 308, SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public string OptimaSeriaDokumentu
        {
            get
            {
                return Settings.GetSettingString("optima_seria_dokumentu", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        private Dictionary<string, Magazyn> _slownikMagazynow = null;
        public Dictionary<string, Magazyn> SlownikMagazynowPoSymbolu
        {
            get
            {
                if (_slownikMagazynow == null)
                {
                    _slownikMagazynow = Calosc.DostepDane.Pobierz<Magazyn>(null).ToDictionary(x => x.Symbol, x => x);
                }
                return _slownikMagazynow;
            }
        }

        private Dictionary<int, Magazyn> _slownikMagazynowPoID = null;
        public Dictionary<int, Magazyn> SlownikMagazynowPoId
        {
            get
            {
                if (_slownikMagazynowPoID == null)
                {
                    _slownikMagazynowPoID = SlownikMagazynowPoSymbolu.Values.ToDictionary(x => x.Id, x => x);
                }
                return _slownikMagazynowPoID;
            }
        }

        public string SymbolMagazynow
        {
            get { return Settings.GetSettingString("symbole_magazynow", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public string OptimaNazwaDokumentu
        {
            get
            {
                return Settings.GetSettingString("optima_nazwa_dokumentu", "RO",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        public string NazwaFirmy
        {
            get { return Settings.GetSettingString("enova_firma", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        /// <summary>
        /// Pole w Erpie które pokazuje czy dany towa ma być lub nie być zaciągany na platformę 
        /// </summary>
        public string B2bUkryty => Settings.GetSettingString("b2b_ukryty", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Pole powinno być typu bool");

        public string ProduktStanMinimalny
        {
            get
            {
                return Settings.GetSettingString("produkt_stan_minimalny", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        public string WfmagTypProduktow
        {
            get
            {
                return Settings.GetSettingString("wfmag_typy_produktow", "",
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany);
            }
        }

        public string AtrybutyPrefiks
        {
            get { return Settings.GetSettingString("atrybuty_prefiks", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public bool WfmagCzyPobieracKorzenDrzewaKategorii
        {
            get { return Settings.GetSettingBool("wfmag_czypobierac_korzen_drzewa_kategorii", false, SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public string WfmagSymbolArtykulu
        {
            get { return Settings.GetSettingString("wfmag_artykul_symbol", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public string WfmagStanProduktu
        {
            get { return Settings.GetSettingString("wfmag_produkty_stan", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public string WfmagNazwaFirmy
        {
            get { return Settings.GetSettingString("wfmag_nazwa_firmy", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa podmiot w WFMAgu", ustawieniaGrupa.SynchronizacjaWFMag); }
        }

        public string WfmagFormatNumeracji
        {
            get { return Settings.GetSettingString("wfmag_format_numeracji", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public string WfmagMagazynZamowienia
        {
            get { return Settings.GetSettingString("wfmag_zamowienia_magazyn", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        

        public string WfmagKtoreCechyKategorie
        {
            get { return Settings.GetSettingString("wfmag_ktore_cechy_kategorie", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public void AktualizacjaUstawien(Ustawienie upd)
        {
            Settings.AktualizujUstawienie(upd);
        }

        public string PlikiDpPobraniaKatalogBazowyIkony
        {
            get { return Settings.GetSettingString("pliki_do_pobrania_katalog_bazowy_ikony", "/Zasoby/Obrazki/ikony_plikow", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }
        
        public int SferaModulOkresZmianyMinuty
        {
            get { return Settings.GetSettingInt("sfera_modul_okres_zmiany_minuty", 15, SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }
        
        public DateTime DataZamowieniaPakietu
        {
            get { return Settings.GetSettingDateTime("package_orders_date", DateTime.MinValue, SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
            set
            {
                Settings.SetSetting("package_orders_date", value.ToString(), TypUstawienia.Datetime); //----------
            }
        }

        public string EmailBcc
        {
            get { return Settings.GetSettingString("email_bcc", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public List<Ustawienie> ListaUstawien(bool widoczneTylkoDlaUzytkownika, int? idPracownika)
        {
            return Settings.GetSettingsList(widoczneTylkoDlaUzytkownika, idPracownika);
        }

        public HashSet<string> DomyslneUprawnieniaPracownik
        {
            get
            {
                Type typSlownika = Type.GetType("SolEx.Hurt.Web.Site2.Helper.SlownikElementowMenu, SolEx.Hurt.Web.Site2");
                MethodInfo metodaPobieraniaSlownika = typeof(SettingCollection).GetMethod("GetSettingSlownikRefleksja");
                MethodInfo generycznaMetoda = metodaPobieraniaSlownika.MakeGenericMethod(new Type[] { typSlownika, typeof(string) });

                var wartosci = generycznaMetoda.Invoke(Settings, new object[]
                {
                    "DomyslneUprawnieniaPracownik", new HashSet<string>(), SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Domyślne uprawninia dla pracowników w adminie", ustawieniaGrupa.Uprawnienia_Pracownicze, false, true,true, TypUstawieniaPodgrupa.Brak,null
                });
               
                return (HashSet<string>)wartosci;
            }
        }

        public HashSet<string> DomyslneUprawnieniaPrzedstawiciel
        {
            get
            {
                Type typSlownika = Type.GetType("SolEx.Hurt.Web.Site2.Helper.SlownikElementowMenu, SolEx.Hurt.Web.Site2");
                if (typSlownika == null)
                {
                    throw new Exception("Brak typu dla namespace: SolEx.Hurt.Web.Site2.Helper.SlownikElementowMenu, SolEx.Hurt.Web.Site2");
                }
                MethodInfo metodaPobieraniaSlownika = typeof(SettingCollection).GetMethod("GetSettingSlownikRefleksja");
                MethodInfo generycznaMetoda = metodaPobieraniaSlownika.MakeGenericMethod(new Type[] { typSlownika, typeof(string) });

                var wartosci = generycznaMetoda.Invoke(Settings, new object[]
                {
                    "DomyslneUprawnieniaPrzedstawiciel", new HashSet<string>(), SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Domyślne uprawninia dla przedstawicieli w adminie", ustawieniaGrupa.Uprawnienia_Pracownicze, false, true,true, TypUstawieniaPodgrupa.Brak,null
                });

                return (HashSet<string>)wartosci;
            }
        }
        
        public bool DomyslnaWidocznoscPunktow
        {
            get
            {
                return Settings.GetSettingBool("DomyslnaWidocznoscPunktow", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Domyślna widoczność punktów",
                    ustawieniaGrupa.Klienci);
            }
        }

        public bool KlientMozeZmianiacJezyk
        {
            get { return Settings.GetSettingBool("KlientMożeZmieniaćJęzyk", true, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy klient może samodzielnie zmieniać swój język", ustawieniaGrupa.Klienci); }
        }

        public bool PokazywanieSortowaniaJednostek
        {
            get { return Settings.GetSettingBool("PokazywanieSortowaniaJednostek", false, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Czy klient może ustawić kolejność pokazywania jednostek (funkcja 'duże opakowania')", ustawieniaGrupa.Klienci); }
        }

        public HashSet<string> AktywneWidokiListyProduktow(bool czyKlientZalogowany)
        {
            SlownikWidokowListyProduktow slownik = new SlownikWidokowListyProduktow();
            HashSet<string> domyslne = new HashSet<string>(slownik.PobierzWartosci().Values.Select(x => x.ToString()));
            return Settings.GetSettingSlownikRefleksja<SlownikWidokowListyProduktow, string>("AktywneWidokiListyProduktow", domyslne, czyKlientZalogowany,
                "Aktywne widoki listy produktów",
                ustawieniaGrupa.Wygląd_Lista_Produktów, multi: true);
        }

        public string AktywneWidokiListyProduktowRodzinowych
        {
            get
            {
                SlownikWidokowListyProduktowRodzinowych slownik = new SlownikWidokowListyProduktowRodzinowych();
                HashSet<string> domyslne = new HashSet<string> { slownik.PobierzWartosci().First(x => x.Key == "ObokSiebie").Value.ToString() };
                return Settings.GetSettingSlownikRefleksja<SlownikWidokowListyProduktowRodzinowych, string>("AktywneWidokiListyProduktowRodzinowych", domyslne, SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                    "Aktywne widoki listy produktów rodzinowych",
                    ustawieniaGrupa.Wygląd_Lista_Produktów).First();
            }
        }

        public bool CechyRodzinoweKartaProduktuPokazuj
        {
            get
            {
                return Settings.GetSettingBool("CechyRodzinoweKartaProduktuPokazuj", true,
                    SesjaHelper.PobierzInstancje.CzyKlientZalogowany,
                     "Czy pokazywać cechy rodzinowe na karcie produktu",
                     ustawieniaGrupa.Wygląd_Karta_Produktu);
            }
        }

        private long? _cechaMojKatalog = null;
        public long CechaMojKatalog
        {
            get
            {
                if (_cechaMojKatalog == null)
                {
                    _cechaMojKatalog = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzCecheOSymbolu(this.SymbolCechyMojKatalog, this.JezykIDDomyslny).Id;
                }
                return _cechaMojKatalog.Value;
            }
        }

        private CechyBll _cechaUlubione = null;
        public CechyBll CechaUlubione
        {
            get
            {
                if (_cechaUlubione == null)
                {
                    _cechaUlubione = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzCecheOSymbolu(this.SymbolCechyUlubione, this.JezykIDDomyslny);
                }
                return _cechaUlubione;
            }
        }

        private long? _cechaAkcesoria = null;
        public long CechaAkcesoria
        {
            get
            {
                if (_cechaAkcesoria == null)
                {
                    _cechaAkcesoria = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzCecheOSymbolu(this.SymbolCechyAkcesoria, this.JezykIDDomyslny).Id;
                }
                return _cechaAkcesoria.Value;
            }
        }

        private long? _cechaGradacja = null;
        public long CechaGradacja
        {
            get
            {
                if (_cechaGradacja == null)
                {
                    _cechaGradacja = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzCecheOSymbolu(this.SymbolCechyGradacja, this.JezykIDDomyslny).Id;
                }
                return _cechaGradacja.Value;
            }
        }

        public string SymbolCechyMojKatalog
        {
            get { return "automatyczne_MojKatalog:moj-katalog"; }
        }

        public string SymbolCechyUlubione
        {
            get { return "automatyczne_Ulubione:ulubione"; }
        }

        public string SymbolCechyAkcesoria
        {
            get { return "automatyczne_Akcesoria:akcesoria"; }
        }

        public string SymbolCechyGradacja
        {
            get { return "automatyczne_Gradacja:gradacja"; }
        }
        public string SymbolCechyProduktZOferty
        {
            get { return "automatyczne_OfertaSpecjalna:moja-oferta-specjalna"; }
        }

        public string SymbolAtrybutCechyAutomatyczne
        {
            get { return "automatyczne";}
        }

        public string SymbolAtrybutCechyTypStanu
        {
            get { return "Typ stanu"; }
        }

        public int RabatZaokraglacDoIluMiejsc { get; set; }

        public bool SzczegolyDokumentuKodKreskowyPokazuj { get; set; }

        public bool SzczegolyDokumentuKodPokazuj { get; set; }

        public string GoogleCaptchaKluczPubliczny
        {
            get { return Settings.GetSettingString("GoogleCaptchaKluczPubliczny", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Klucz publiczny google captcha", ustawieniaGrupa.Systemowe); }
        }

        public string GoogleCaptchaKluczPrywatny
        {
            get { return Settings.GetSettingString("GoogleCaptchaKluczPrywatny", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Klucz prywatny google captcha", ustawieniaGrupa.Systemowe); }
        }

        public int GodzinaPoczatekZakazIntegracji
        {
            get { return Settings.GetSettingInt("GodzinaPoczatekZakazIntegracji", 11, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Od której godziny nie wolno pobierać plików z api przez klientów", ustawieniaGrupa.Systemowe); }
        }

        public int GodzinaKoniecZakazIntegracji
        {
            get { return Settings.GetSettingInt("GodzinaKoniecZakazIntegracji", 13, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Do której godziny nie wolno pobierać plików z api rzez klientów", ustawieniaGrupa.Systemowe); }
        }

        public int CzasPodpowiedziSzukanie
        {
            get { return Settings.GetSettingInt("CzasPodpowiedziSzukanie", 1500, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Ile milisekund czekać podczas szukania, jeśli 0 to nie ma automatycznego ładowania wyników", ustawieniaGrupa.Wygląd_Lista_Produktów); }
        }

        public int MaksimumProbWyslaniaTegoSamegoMaila
        {
            get { return Settings.GetSettingInt("MaksimumProbWyslaniaTegoSamegoMaila", 5, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, 
                "Ile jest prób wysłania tego samego maila w wypadku błedu",
                ustawieniaGrupa.Maile_i_powiadomienia, false, true); }
        }

        public int MaksIleBledowPodczasPonownegoWysylaniaMaila
        {
            get { return Settings.GetSettingInt("MaksIleBledowPodczasPonownegoWysylaniaMaila", 1, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, 
                "Po ilu błedach przerywać wysyłanie błednych mail", 
                ustawieniaGrupa.Maile_i_powiadomienia, false, true); }
        }

        public string ArtykulyFormatDaty
        {
            get { return Settings.GetSettingString("artykuly_format_data", "dd.MM.yyyy", SesjaHelper.PobierzInstancje.CzyKlientZalogowany); }
        }

        public bool TlumaczenieWLocie
        {
            get
            {
                return Settings.GetSettingBool("tlumaczenie_w_locie", false, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Tłumaczenie w locie - działa tylko na administratorze");
            }
        }

        [NiezalecanaZmiana]
        public DateTime OdKiedyDrukowacPdf
        {
            get
            {
                int dniwstecz = Settings.GetSettingInt("IleDniWsteczDrukowacPdf", 7, SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Z ilu dni wstecz drukować i aktualizować faktury pdf, jesli 0 to drukujemy dla wszystkich istniejących dokumentów. Liczone względem daty wystawienia dokumentu", ustawieniaGrupa.Synchronizacja);
                if (dniwstecz == 0)
                {
                    return DateTime.Now.AddYears(-50);
                }
                return DateTime.Now.Date.AddDays(-dniwstecz);
            }
        }

        public Ustawienie Pobierz(string id)
        {
            return Settings.GetSettingsList().FirstOrDefault(x => x.Id == id);
        }

        public string MainCS
        {
            get
            {
                ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings["MainConnectionString"];
                if (conn == null)
                    return "";
                return conn.ConnectionString;
            }
        }

        public int Platnosci24IdKonta => Settings.GetSettingInt("Platnosci24IdKonta", 0,SesjaHelper.PobierzInstancje.CzyKlientZalogowany,"Id konta w serwicie przelewy24.pl",ustawieniaGrupa.Zamówienia);

        public string Platnosci24KluczCrc => Settings.GetSettingString("Platnosci24KluczCRC", "",SesjaHelper.PobierzInstancje.CzyKlientZalogowany,"Klucz CRC w serwicie przelewy24.pl",ustawieniaGrupa.Zamówienia);

        public void UsunLogi()
        {
            long ilosc = 0;
            do
            {
                using (var trans = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.OpenTransaction())
                {
                    SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.ExecuteNonQuery("DELETE TOP (1000) FROM LogWpis WHERE id not in(select top 1000 id from LogWpis order by Id desc)");
                    trans.Commit();
                }
                ilosc = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Count<LogWpis>();
            }
            while (ilosc > 2000);
        }

        public void KasowaniePdfBezDokumentow()
        {
            string sql = "SELECT Id FROM HistoriaDokumentu";
            List<long> idDokumentow = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.SqlList<long>(sql);
            string[] slownikDokumentow = idDokumentow.Select( x => SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzSciezkePliku((int)x, "pdf") ).ToArray();

            string sciezka = SolexBllCalosc.PobierzInstancje.DokumentyDostep.KatalogDokumentow;

            string[] fileEntries = Directory.GetFiles(sciezka);
            HashSet<string> doUsunjiecia = new HashSet<string>( fileEntries.Except(slownikDokumentow) );
            foreach (var s in doUsunjiecia)
            {
                File.Delete(s);
            }
            if (doUsunjiecia.Any())
            {
                DzialaniaUzytkownikow dz = new DzialaniaUzytkownikow(ZdarzenieGlowne.UsuniecieZbednychPdf, new Dictionary<string, string>());
                if (doUsunjiecia.Count > 10)
                {
                    dz.Parametry.Add("Dokumenty ", "Skasowano ponad 10 dokumentów pdf");
                }
                else
                {
                    string name = StringExtensions.ToCsv(doUsunjiecia.Select(x => Path.GetFileNameWithoutExtension(x)));
                    dz.Parametry.Add("Dokumenty ", name);
                }
                SolexBllCalosc.PobierzInstancje.Statystyki.DodajZdarzenie(dz, null);
            }
            
        }


        /// <summary>
        /// Sciezka do pliku wymiany
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string KatalogPlikowWymianyTema => Settings.GetSettingString("KatalogPlikowWymianyTema", "",SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Ścieżka do katalogu, w którym są pliki do importu",ustawieniaGrupa.SynchronizacjaTema);

        /// <summary>
        /// Sciezka do pliku wymiany
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string KatalogPlikowZDokumentamiTema => Settings.GetSettingString("KatalogPlikowZDokumentamiTema", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Ścieżka do katalogu, w którym są dokumenty do importu", ustawieniaGrupa.SynchronizacjaTema);

        /// <summary>
        /// Nazwa pliku z kontrahentami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string NazwaPlikuZKontrahentamiTema => Settings.GetSettingString("NazwaPlikuZKontrahentamiTema", "KONTRAHENCI",SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym są kontrahenci", ustawieniaGrupa.SynchronizacjaTema);

        /// <summary>
        /// Nazwa pliku z przedstawicielami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string NazwaPlikuZPrzedstawicielamiTema => Settings.GetSettingString("NazwaPlikuZPrzedstawicielamiTema", "PRZEDSTAWICIELE",SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym są przedstawiciele", ustawieniaGrupa.SynchronizacjaTema);

        /// <summary>
        /// Nazwa pliku z producentami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string NazwaPlikuZProducentamiTema => Settings.GetSettingString("NazwaPlikuZProducentamiTema", "PRODUCENCI", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym są producenci", ustawieniaGrupa.SynchronizacjaTema);

        /// <summary>
        /// Nazwa pliku z rabatami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string NazwaPlikuZRabatamiTema => Settings.GetSettingString("NazwaPlikuZRabatamiTema", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym są rabaty", ustawieniaGrupa.SynchronizacjaTema);

        /// <summary>
        /// Nazwa pliku ze stanami 
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string NazwaPlikuZeStanamiTema => Settings.GetSettingString("NazwaPlikuZeStanamiTema", "", SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym są stany", ustawieniaGrupa.SynchronizacjaTema);

        /// <summary>
        ///  Nazwa pliku z dokumentami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string NazwaPlikuZDokumentamiTema => Settings.GetSettingString("NazwaPlikuZDokumentamiTema", "",SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym są dokumenty", ustawieniaGrupa.SynchronizacjaTema);

        /// <summary>
        ///  Nazwa pliku z produktami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string NazwaPlikuZProduktamiTema => Settings.GetSettingString("NazwaPlikuZProduktamiTema", "",SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym są produkty do importu", ustawieniaGrupa.SynchronizacjaTema);

        /// <summary>
        ///  Nazwa pliku z cenami
        /// </summary>
        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string NazwaPlikuZCenamiTema => Settings.GetSettingString("NazwaPlikuZCenamiTema", "",SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Nazwa pliku w którym są ceny do produktów", ustawieniaGrupa.SynchronizacjaTema);

        [Wymagane(new[] { ERPProviderzy.Tema })]
        public string SeparatorDoPlikuCsvTema => Settings.GetSettingString("SeparatorDoPlikuCsvTema", "",SesjaHelper.PobierzInstancje.CzyKlientZalogowany, "Separator do pliku Csv",ustawieniaGrupa.SynchronizacjaTema);

        private IDictionary<TypDanychIntegracja, List<PlikIntegracjiSzablon>> _tablicaPlikowIntegracji = null;
        private IDictionary<int, PlikIntegracjiSzablon> _tablicaPlikowIntegracjiWgId = null;

        public virtual IDictionary<int, PlikIntegracjiSzablon> TablicaPlikowIntegracjiWgId
        {
            get
            {
                if (_tablicaPlikowIntegracjiWgId == null)
                {
                    if (this.PobierzListePlikowIntegracji == null)
                    {
                        throw new Exception("Błąd pobierania istniejących integracji - nie można pobrać szablonów integracji z dysku");
                    }
                }
                return _tablicaPlikowIntegracjiWgId;
            }
        }

        private static object lok = new object();

        public IDictionary<TypDanychIntegracja, List<PlikIntegracjiSzablon>> PobierzListePlikowIntegracji
        {
            get
            {
                if (_tablicaPlikowIntegracji == null)
                {
                    lock (lok)
                    {
                        if (_tablicaPlikowIntegracji != null)
                        {
                            return _tablicaPlikowIntegracji;
                        }

                        string sciezkaDoIntegracji = AppDomain.CurrentDomain.BaseDirectory + @"\Views\Integracja\";
                        List<string> listaPlikow = new List<string>();
                        listaPlikow.AddRange(Directory.GetFiles(sciezkaDoIntegracji, "*.cshtml", SearchOption.AllDirectories));
                        if (!string.IsNullOrEmpty(this.SzablonNiestandardowyNazwa))
                        {
                            string templateKLienta = this.SzablonNiestandardowySciezkaBezwzgledna + @"\Views\Integracja\";
                            if (Directory.Exists(templateKLienta))
                            {
                                listaPlikow.AddRange(Directory.GetFiles(templateKLienta, "*.cshtml", SearchOption.AllDirectories));
                            }
                        }

                        List<PlikIntegracjiSzablon> szablony = new List<PlikIntegracjiSzablon>();
                        _tablicaPlikowIntegracjiWgId = new Dictionary<int, PlikIntegracjiSzablon>();

                        foreach (string plik in listaPlikow)
                        {
                            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(plik));
                            string nazwaKataloguWJakimJestPlik = dir.Name;

                            //nazwa pliku musi byc zutowana na inta - wersja
                            string nazwaPliku = Path.GetFileNameWithoutExtension(plik);

                            //olewamy to co zaczyna sie od _
                            if (nazwaPliku.StartsWith("_"))
                            {
                                continue;
                            }
                            int wersja = 0;
                            if (!int.TryParse(nazwaPliku, out wersja))
                            {
                                Calosc.Log.ErrorFormat("Szablon integracji ma nieprawdiłową wersje - nie można rzutować na inta. Pomijam plik: " + plik);
                                continue;
                            }
                            string[] czlonyNazwy = nazwaKataloguWJakimJestPlik.Split(new char[] {'_'});
                            TypDanychIntegracja typDanych;
                            int idSzablonu = 0;

                            if (!int.TryParse(czlonyNazwy[0], out idSzablonu))
                            {
                                Calosc.Log.ErrorFormat("Nie można odczytać id szablonu dla szablonu integracji " + plik + ". Szablon musi zaczynać się na liczbę np. 1_nazwa. Pomijam plik");
                                continue;
                            }

                            if (!Enum.TryParse(czlonyNazwy[1], true, out typDanych))
                            {
                                Calosc.Log.ErrorFormat("Nie można odczytać typu danych dla szablonu integracji " + plik + ". Pomijam plik");
                                continue;
                            }
                            PlikIntegracjiSzablon plikSzablonu = new PlikIntegracjiSzablon();
                            plikSzablonu.IdSzablonu = idSzablonu;
                            plikSzablonu.Format = czlonyNazwy[3];
                            plikSzablonu.Szablon = czlonyNazwy[2];
                            plikSzablonu.typDanych = typDanych;
                            plikSzablonu.Wersja = new List<int>() {wersja};
                            plikSzablonu.SciezkaDoSzablonu = nazwaKataloguWJakimJestPlik + @"/";

                            //czy juz istnieje taki szablon
                            var temp = szablony.FirstOrDefault(x => x.IdSzablonu == plikSzablonu.IdSzablonu && x.Format == plikSzablonu.Format && x.typDanych == plikSzablonu.typDanych); //&& x.Szablon == plikSzablonu.Szablon 
                            if (temp != null)
                            {
                                temp.Wersja.Add(wersja);
                            }
                            else
                            {
                                szablony.Add(plikSzablonu);                                
                                _tablicaPlikowIntegracjiWgId.Add(plikSzablonu.IdSzablonu, plikSzablonu);
                            }
                        }

                        szablony = szablony.OrderBy(x => x.IdSzablonu).ToList();
                        foreach (PlikIntegracjiSzablon plikIntegracjiSzablon in szablony)
                        {
                            plikIntegracjiSzablon.Wersja = plikIntegracjiSzablon.Wersja.OrderByDescending(x => x).ToList();
                        }
                        _tablicaPlikowIntegracji = szablony.GroupBy(x => x.typDanych).ToDictionary(x => x.Key, x => x.OrderBy(z => z.Format).ToList());
                    }
                }
                return _tablicaPlikowIntegracji;
            }
        }

        /// <summary>
        /// Sprawdzamy jakie są dostępne moduły do importu oraz które z nich mają przypładowe pliki (te bez przykładowych są pomijane)
        /// </summary>
        private Dictionary<string,OpisImportera> _formatyImportowaniaPlikow;
        public Dictionary<string, OpisImportera> AktywneFormatyImportowaniaPlikow
        {
            get
            {
                if (_formatyImportowaniaPlikow!= null && _formatyImportowaniaPlikow.Any()) return _formatyImportowaniaPlikow;
                _formatyImportowaniaPlikow = new Dictionary<string, OpisImportera>();

                SlownikSposobowImportuKoszyka wszystkieModuly = new SlownikSposobowImportuKoszyka();

                foreach (var modul in wszystkieModuly.PobierzWartosci())
                {
                    Type t = Type.GetType((string)modul.Value, true);
                    ImportBaza imp = (ImportBaza) Activator.CreateInstance(t);
                    string nazwa = imp.LadnaNazwa;
                    if (string.IsNullOrWhiteSpace(nazwa) && !imp.Rozszerzenia.Any())
                    {
                        continue;
                    }
                    string nazwaPlikuWzorcowego = "Wzorcowy" + t.Name + "." + imp.Rozszerzenia.FirstOrDefault();
                    string tmp = AppDomain.CurrentDomain.BaseDirectory + "HELP\\PlikiImport\\" + nazwaPlikuWzorcowego;
                    if (File.Exists(tmp))
                    {
                        _formatyImportowaniaPlikow.Add(t.PobierzOpisTypu(),new OpisImportera() {Nazwa = nazwa,OpisTypu = t.PobierzOpisTypu(),PlikWzorcowy = nazwaPlikuWzorcowego});
                    }
                    //else
                    //{
                    //    Log.ErrorFormat("Brak pliku wzorcowego: {0} importera dla: {1}", nazwaPlikuWzorcowego, nazwa);
                    //}
                }
                return _formatyImportowaniaPlikow;
            }
        }

        public void UsunZalacznikRejestracji(IList<object> obj)
        {
            foreach (var o in obj)
            {
                var katalog = $"{AppDomain.CurrentDomain.BaseDirectory}Zasoby\\Rejestracja\\{o}";
                if (Directory.Exists(katalog))
                {
                    Directory.Delete(katalog,true);
                }
            }
        }

        /// <summary>
        /// Wylicznie kursu waluty między dwoma walutami. 
        /// </summary>
        /// <param name="walutaCeny">Symbol waluty ceny/podstawowej</param>
        /// <param name="walutaKlienta">Symbol waluty docelowej</param>
        /// <returns>Kurs zaokrąglony do 4 miejsc po przecinku</returns>
        public decimal PobierzKursWalut(string walutaCeny, string walutaKlienta)
        {
            if (walutaCeny==null || walutaCeny.Equals(walutaKlienta, StringComparison.InvariantCultureIgnoreCase))
            {
                return 1;
            }
            Waluta zWaluta = this.SlownikWalut.Values.FirstOrDefault(x => x.WalutaB2b.Equals(walutaCeny, StringComparison.InvariantCultureIgnoreCase));
            if(zWaluta == null) throw new Exception($"Brak waluty: {walutaCeny} w systemie.");
            Waluta naWaluta = this.SlownikWalut.Values.FirstOrDefault(x => x.WalutaB2b.Equals(walutaKlienta, StringComparison.InvariantCultureIgnoreCase));
            if (naWaluta == null) throw new Exception($"Brak waluty: {walutaKlienta} w systemie.");

            var kursCalkowity = PobierzKursWalut(zWaluta, naWaluta);

            return kursCalkowity;
        }

        /// <summary>
        /// Wylicznie kursu waluty między dwoma walutami. 
        /// </summary>
        /// <param name="zWaluta">Waluta ceny/podstawowa</param>
        /// <param name="naWaluta">Waluta docelowa</param>
        /// <returns>Kurs zaokrąglony do 4 miejsc po przecinku</returns>
        public decimal PobierzKursWalut(Waluta zWaluta, Waluta naWaluta)
        {
            if (zWaluta.Id == naWaluta.Id)
            {
                return 1;
            }
            if (!zWaluta.Kurs.HasValue)
            {
                throw new Exception(
                    string.Format($"Brak przelicznika kursu dla waluty: {zWaluta.WalutaErp}."));
            }
            if (!naWaluta.Kurs.HasValue)
            {
                throw new Exception(
                    string.Format($"Brak przelicznika kursu dla waluty: {naWaluta.WalutaErp}."));
            }

            if (zWaluta.Kurs.Value == 0)
            {
                throw new Exception(
                    string.Format($"Kurs dla waluty: {zWaluta.WalutaErp} nie może wynosić 0. Prosze o poprawienie kursu."));
            }

            if (naWaluta.Kurs.Value == 0)
            {
                throw new Exception(
                    string.Format($"Kurs dla waluty: {naWaluta.WalutaErp} nie może wynosić 0. Prosze o poprawienie kursu."));
            }

            decimal kursCeny = zWaluta.Kurs.Value;
            decimal kursWalutyKlienta = naWaluta.Kurs.Value;

            var kursCalkowity = kursCeny * (1M / kursWalutyKlienta);

            return Math.Round(kursCalkowity,4);
        }

    }
}