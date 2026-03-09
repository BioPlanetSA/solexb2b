using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    public class ConfigWzInnyPoziom : IConfigSynchro
    {
        public string KatalogZObrazkami
        {
            get { throw new NotImplementedException(); }
        }

        public int[] AtrybutyRodzin
        {
            get { throw new NotImplementedException(); }
        }

        public int? SubiektKodTransakcjiDlaKlientowEU
        {
            get { throw new NotImplementedException(); }
        }

        public ISettingCollection Settings
        {
            get { throw new NotImplementedException(); }
        }

        public string[] SeparatoryDrzewkaKategorii
        {
            get { throw new NotImplementedException(); }
        }

        public string ProduktyNieDostepnePrzezDluzszyCzasCecha
        {
            get { throw new NotImplementedException(); }
        }

        public List<string> PolaDoWyzerowania
        {
            get { throw new NotImplementedException(); }
        }

        public int DokumentyMailOnNowymIleDniWstecz
        {
            get { throw new NotImplementedException(); }
        }

        public bool WieleJezykowWSystemie { get { throw new NotImplementedException(); } }
        public int JezykIDDomyslny { get {throw new NotImplementedException();} }
        public Dictionary<int, ISettingCollection> SellerSettings
        {
            get { throw new NotImplementedException(); }
        }

        private Dictionary<int, Jezyk> _jezyki;

        public Dictionary<int, Jezyk> JezykiWSystemie
        {
            get { return _jezyki; }
            set { _jezyki = value; }
        }

        public Dictionary<string, Jezyk> JezykiWSystemieSlownikPoSymbolu
        {
            get { throw new NotImplementedException(); }
        }

        public bool TlumaczenieWLocie
        {
            get { throw new NotImplementedException(); }
        }

        public string SubiektPodmiot
        {
            get { return ""; }
        }

        public bool SubiektSzyfrujHaslo
        {
            get { return false; }
        }

        public int CoIleDniZmieniacHaslo
        {
            get { throw new NotImplementedException(); }
        }

        public bool AtrybutZCechy
        {
            get { throw new NotImplementedException(); }
        }

        public string SzablonNiestandardowyNazwa
        {
            get { throw new NotImplementedException(); }
        }

        public string SzablonNiestandardowySciezkaWzgledna
        {
            get { throw new NotImplementedException(); }
        }

        public string SzablonNiestandardowySciezkaBezwzgledna
        {
            get { throw new NotImplementedException(); }
        }

        public bool PokazywacRabatICeneWyjsciowa
        {
            get { throw new NotImplementedException(); }
        }

        public bool PokazywacZyskKlienta
        {
            get { throw new NotImplementedException(); }
        }


        public Dictionary<int, StatusZamowienia> StatusyZamowien
        {
            get { throw new NotImplementedException(); }
        }

        public int JezykIDPolski
        {
            get { return _jezyki.Values.First(x => x.Domyslny).Id; }
        }

        public bool PokazywacBelkeDostepnosci
        {
            get { throw new NotImplementedException(); }
        }

        public bool InfoPrzekroczoneStany
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public SposobPokazywaniaDodatkowegoRabatu PokazywanieRabatu
        {
            get { throw new NotImplementedException(); }
        }

        public bool SzczegolyProduktuJakoPopup
        {
            get { throw new NotImplementedException(); }
        }

        public bool RejestracjaPokazHaslo
        {
            get { throw new NotImplementedException(); }
        }

        public string ZnakWodnyTekst
        {
            get { return ""; }
        }

        public string ZnakWodnyObrazek
        {
            get { return ""; }
        }

        public bool UkryjDodawanieAdresow
        {
            get { throw new NotImplementedException(); }
        }

        public bool AutomatyczneZatwierdzanieRejestracji
        {
            get { throw new NotImplementedException(); }
        }

        public string ProduktOpis
        {
            get { throw new NotImplementedException(); }
        }


        public string B2BNazwaAngielska
        {
            get { throw new NotImplementedException(); }
        }

        //public string OptimaUkrywajProduktyZAtrybutem
        //{
        //    get { return ""; }
        //}

        public string B2BOjciec
        {
            get { return ""; }
        }

        public string B2BCenaPunkty
        {
            get { throw new NotImplementedException(); }
        }

        public string B2BDodatkowaJednostka
        {
            get { throw new NotImplementedException(); }
        }

        public string AdresStronyZProduktami
        {
            get { throw new NotImplementedException(); }
        }

        public string AdresStronyZProduktem
        {
            get { throw new NotImplementedException(); }
        }

        public bool UkryjKredytKupiecki
        {
            get { throw new NotImplementedException(); }
        }

        public bool KomunikatIloscWOpakowaniuGdyJednosc
        {
            get { throw new NotImplementedException(); }
        }

        public bool KomunikatIloscMinimalnaGdyJednosc
        {
            get { throw new NotImplementedException(); }
        }

        public string[] PolaWlasneCechy
        {
            get { throw new NotImplementedException(); }
        }

        public string AtrybutKategoriiZERP
        {
            get { throw new NotImplementedException(); }
        }

        public string AtrybutProducentaZERP
        {
            get { throw new NotImplementedException(); }
        }

        public bool PrzeladujPoDodaniuDoKoszyka
        {
            get { throw new NotImplementedException(); }
        }

        public bool PokazujRabatyTylkoZWidocznychGrup
        {
            get { throw new NotImplementedException(); }
        }

        public int ZIluDniWsteczWysylacListyPrzewozowe
        {
            get { throw new NotImplementedException(); }
        }

        public bool DokumentyPokazacNazweKlienta
        {
            get { throw new NotImplementedException(); }
        }

        public string SeparatorGrupKlientow
        {
            get { throw new NotImplementedException(); }
        }

        public HashSet<ZCzegoLiczycGradacje> ZCzegoLiczycGradacje
        {
            get { throw new NotImplementedException(); }
        }

        //public DateTime GradacjeOdKiedyLiczyc
        //{
        //    get { throw new NotImplementedException(); }
        //}


        public bool GradacjeUwzgledniajRodziny
        {
            get { throw new NotImplementedException(); }
        }

        public int OptimaIdSzablonuWydrukuDoPdf
        {
            get { throw new NotImplementedException(); }
        }


        public Szerokosci SzerokoscKafelka
        {
            get { throw new NotImplementedException(); }
        }

        public Szerokosci SzerokoscKafelkaProducenci
        {
            get { throw new NotImplementedException(); }
        }

        public string SciezkaFavicon
        {
            get { throw new NotImplementedException(); }
        }

        public int ProduktyNaZamowienieCechaID
        {
            get { throw new NotImplementedException(); }
        }

        public void SynchronizacjaPobierzWidocznoscProduktuZPola(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {

        }

        public decimal ProduktyNaWyczerpaniu_procentStanuMinimalnego
        {
            get { throw new NotImplementedException(); }
        }

        public decimal ProduktyNieDostepnePrzezDluzszyCzas_iloscDni
        {
            get { throw new NotImplementedException(); }
        }

        public bool PodczasWyszukiwaniaZmienPolskeZnaki
        {
            get { throw new NotImplementedException(); }
        }

        public bool ZamowieniaTworzRezerwacje
        {
            get { throw new NotImplementedException(); }
        }

        public string RaportyPoleProduktuPokazywaneJakoSymbol
        {
            get { throw new NotImplementedException(); }
        }

        public string EnovaPoleDoPobieraniaZDokumentow
        {
            get { throw new NotImplementedException(); }
        }

        public string EnovaPoleDoZapisywaniaUwagNaPozycjiDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        public string EnovaPole2DoZapisywaniaUwagNaPozycjiDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        public bool SortowanieNaturalneListyProduktow
        {
            get { throw new NotImplementedException(); }
        }

        public bool SprawdzajStanyMagazynowe
        {
            get { throw new NotImplementedException(); }
        }

        public int ZIleDniDomyslniePokazywacDokumenty
        {
            get { throw new NotImplementedException(); }
        }

        public bool WielowybieralnoscKategorii
        {
            get { throw new NotImplementedException(); }
        }

        public bool UkryjInfoOIndywidualizacji
        {
            get { throw new NotImplementedException(); }
        }

        public string OptimaAtrybutZeZdjeciami
        {
            get { throw new NotImplementedException(); }
        }

        public PokazywanieKomunikatu DomyslnaPozycjaKomunikatowKoszyk
        {
            get { throw new NotImplementedException(); }
        }

        public string ERPcs
        {
            get { return _cs; }
        }

        public string ERPcs2
        {
            get { throw new NotImplementedException(); }
        }

        public string ERPHaslo
        {
            get { return _haslo; }
        }

        public string ERPLogin
        {
            get { return _login; }
        }

        public bool PokazujCenyDlaNiezalogowanych
        {
            get { throw new NotImplementedException(); }
        }

        public bool WysylajPowiadomienieFakturaGdyBrakPdf
        {
            get { throw new NotImplementedException(); }
        }

        public string EnovaDoZapisuKategoriiZamowienia
        {
            get { throw new NotImplementedException(); }
        }

        public string EnovaCechaDoPobraniaJakoStatusDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        public string KatalogProgramuKsiegowego
        {
            get { throw new NotImplementedException(); }
        }

        public bool UkryjFiltrowanieKategoriiWyszukiwanie
        {
            get { throw new NotImplementedException(); }
        }

        public string EnovaPoleDoPobieraniaZDokumentow2
        {
            get { throw new NotImplementedException(); }
        }

        public string PoleZHaslem
        {
            get { throw new NotImplementedException(); }
        }

        public string KategoriaKLientowKupowaneIlosci
        {
            get { throw new NotImplementedException(); }
        }

        public bool ImportyZezwalajNaGlobalne
        {
            get { throw new NotImplementedException(); }
        }

        public int IloscProduktoWPaczceAktualizacja
        {
            get { throw new NotImplementedException(); }
        }

        public bool UkryjDrzewkoKategoriiProduktow
        {
            get { throw new NotImplementedException(); }
        }

        public bool RejestracjaCaptcha
        {
            get { throw new NotImplementedException(); }
        }

        public List<int> MplanetIdCech
        {
            get { throw new NotImplementedException(); }
        }

        public int MplanetIdPoziomuCenyDetalicznejZb2B
        {
            get { throw new NotImplementedException(); }
        }

        public string MplanetWzorWyszukiwaniaTowaru
        {
            get { throw new NotImplementedException(); }
        }

        public int ProduktyDropshipingCechaID
        {
            get { throw new NotImplementedException(); }
        }

        public bool BlokujDodawanieDoKoszykaDlaBrakujacychProduktow
        {
            get { throw new NotImplementedException(); }
        }

        public bool PokazywacProduktyCenyZerowe
        {
            get { throw new NotImplementedException(); }
        }

        public bool DadawanieAtrybutuDoKategorii
        {
            get { throw new NotImplementedException(); }
        }

        public void LoadSettings()
        {
            throw new NotImplementedException();
        }

        public void RefreshData()
        {
            throw new NotImplementedException();
        }

        public int PobierzOddzialIDWgDomeny()
        {
            throw new NotImplementedException();
        }

        public bool GetLicense(Licencje key)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> Tlumaczenia(int? lang = null)
        {
            throw new NotImplementedException();
        }

        public string PobierzTlumaczenie(string fraza, params object[] parametryFormatuFrazy)
        {
            throw new NotImplementedException();
        }

        public string PobierzTlumaczenie(int lang, string symbol, params object[] parametryFormatuFrazy)
        {
            throw new NotImplementedException();
        }

        public void ResetSystemNames()
        {
            throw new NotImplementedException();
        }

        public int GetSystemTypeId(Type type)
        {
            throw new NotImplementedException();
        }

        public int GetPriceLevelHurt
        {
            get
            {
                throw new NotImplementedException();
            }

        }

        public int GetPriceLevelDetal
        {
            get
            {
                throw new NotImplementedException();
            }

        }

        public bool StatisticsEnabled()
        {
            throw new NotImplementedException();
        }

        public bool CzyPoleJestSynchronizowane(Type type, object p)
        {
            throw new NotImplementedException();
        }

        public void WyczyszCachePolModulow()
        {
            throw new NotImplementedException();
        }

        public string DzienNaString(DayOfWeek dzien, int jezykId)
        {
            throw new NotImplementedException();
        }

        public string PobierzSerieDlaWaluty(string waluta)
        {
            throw new NotImplementedException();
        }

        public bool GradacjeUwzgledniajIloscWKoszyku
        {
            get { throw new NotImplementedException(); }
        }

        public bool ProduktyDropshipingPokazujNaStanieJesliJest
        {
            get { throw new NotImplementedException(); }
        }

        public void ResetujStatusy()
        {
            throw new NotImplementedException();
        }

        public string CechaAuto
        {
            get { throw new NotImplementedException(); }
        }

        public bool PokazujKategorieObrazkoweDlaGalezi
        {
            get { throw new NotImplementedException(); }
        }

        public string DodatkowyAdresEmail
        {
            get { throw new NotImplementedException(); }
        }

        public bool EksportTylkoKontZHaslem
        {
            get { throw new NotImplementedException(); }
        }

        public string EnovaPoleDoZapisuPlatnikaDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        public bool EnovaZamowieniaBufor
        {
            get { throw new NotImplementedException(); }
        }

        public int MaksimumDokumentowWPaczce
        {
            get { throw new NotImplementedException(); }
        }

        public string PobierzSzablonWydrukuEnova(string symboljezykawydruku)
        {
            throw new NotImplementedException();
        }

        public string PoleKlientJezyk
        {
            get { throw new NotImplementedException(); }
        }

        public string PoleKlientTekst1
        {
            get { throw new NotImplementedException(); }
        }

        public string PoleKlientTekst2
        {
            get { throw new NotImplementedException(); }
        }

        public string PoleKlientTekst3
        {
            get { throw new NotImplementedException(); }
        }

        public string PoleKlientTekst4
        {
            get { throw new NotImplementedException(); }
        }

        public string PoleKlientTekst5
        {
            get { throw new NotImplementedException(); }
        }


        public string SeparatorMail
        {
            get { throw new NotImplementedException(); }
        }

        public string StawkaVatKlienta
        {
            get { throw new NotImplementedException(); }
        }

        public char[] SeparatorAtrybutowWCechach
        {
            get { throw new NotImplementedException(); }
        }

        public string PrzedzialyCenowe
        {
            get { throw new NotImplementedException(); }
        }

        public bool CzyJestLicencjaOddzialy
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<string> DokumentyWyszukiwanie
        {
            get { throw new NotImplementedException(); }
        }

        public List<string> ProduktyWyszukiwanie
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<string> KoszykPozycjeWyszukiwanie
        {
            get { throw new NotImplementedException(); }
        }

        public int CzasPrzechowywaniaZmian { get { throw new NotImplementedException(); } }

        public int SferaMaksPobranNaOkres
        {
            get { throw new NotImplementedException(); }
        }

        public int SferaPobieranieLimitOkres
        {
            get { throw new NotImplementedException(); }
        }

        public int MaxIleDniWsteczMailePowitalne
        {
            get { throw new NotImplementedException(); }
        }

        public int IleMinutCzekacDoKolejnegoUruchomieniaPrzyMasowymWysylaniuMaili
        {
            get { throw new NotImplementedException(); }
        }

        public bool SprawdzanieAdresuIp
        {
            get { throw new NotImplementedException(); }
        }

        public string FiltrKategorii
        {
            get { throw new NotImplementedException(); }
        }

        public string PolePrzedstawiciel
        {
            get { throw new NotImplementedException(); }
        }

        public string PoleOpiekun
        {
            get { throw new NotImplementedException(); }
        }

        public string DrugiOpiekun
        {
            get { throw new NotImplementedException(); }
        }
        public HashSet<int> DeaktywujMinimumLogistyczneDlaWybranychKategoriiKlientow { get { throw new NotImplementedException();} }
        //public bool MinimumLogistyczneWymagane
        //{
        //    get { throw new NotImplementedException(); }
        //}

        public void SynchronizacjaPobierzKredytLimit(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzKredytPozostalo(Klient item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public WidcznoscProduktowWSubiekcie SubiektWidocznoscTowarow
        {
            get { return 0; }
        }

        public IEnumerable<string> KategorieKlientowPolaWlasne
        {
            get { throw new NotImplementedException(); }
        }

        public void SynchronizacjaPobierzIloscMinimlna(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzIloscWOpakowaniu(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeNazwa(Produkt item, Jezyk jezyk, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeKod(Produkt item, Jezyk jezyk, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeOpis(Produkt item, Jezyk jezyk, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeOpis2(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeOpis3(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeOpis4(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeOpis5(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeOpisKrotki(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeOpisKrotki2(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeOpisKrotki3(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeOpisKrotki4(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeOpisKrotki5(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjePoleTekst1(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjePoleTekst2(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjePoleTekst3(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjePoleTekst4(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjePoleTekst5(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeRodzina(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeKolumnaTekst1(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeKolumnaTekst2(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeKolumnaTekst3(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeKolumnaTekst4(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzCenaWPunktach(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeKolumnaTekst5(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeMetaOpis(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzLokalizacjeMetaSlowaKluczowe(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {

        }

        public void SynchronizacjaPobierzObjetoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleLiczba1(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {

        }


        public string B2BNieDlaWszystkich
        {
            get { return ""; }
        }

        public bool ZamowienieWImieniuKlientaWysylajMaile
        {
            get { throw new NotImplementedException(); }
        }

        private string _cs, _login, _haslo;

        public ConfigWzInnyPoziom(string cs, string login, string haslo)
        {
            _cs = cs;
            _login = login;
            _haslo = haslo;
        }

        public void SynchronizacjaPobierzKredytWykorzystano(Klient item, string p, Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzPoleLiczba2(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzPoleLiczba3(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzPoleLiczba4(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzPoleLiczba5(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzKolumnaLiczba1(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzKolumnaLiczba2(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzKolumnaLiczba3(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzKolumnaLiczba4(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {

        }

        public void SynchronizacjaPobierzKolumnaLiczba5(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
        }


        public string DomyslnaWaluta
        {
            get { throw new NotImplementedException(); }
        }

        public int XlTypDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        public int? IdAtrybutuDostawy
        {
            get { throw new NotImplementedException(); }
        }

        //public bool ZamowieniaZapisywaneCenyNetto
        //{
        //    get { throw new NotImplementedException(); }
        //}


        public bool BrakPlatnosciKlientaJesliTerminJestZerowy
        {
            get { throw new NotImplementedException(); }
        }


        public int? IleWczesniejZmianaDostawa
        {
            get { throw new NotImplementedException(); }
        }

        public bool ProduktyZzerowaCena
        {
            get { throw new NotImplementedException(); }
        }

        public string KatalogDoZapisuZalacznikowZFormularzy
        {
            get { throw new NotImplementedException(); }
        }

        public int CzasWyswietlaniaKoszyka
        {
            get { throw new NotImplementedException(); }
        }

        public HashSet<ModulyOptima> JakieModulyOptima
        {
            get { throw new NotImplementedException(); }
        }

        public string SapAdresSerweraLicencji
        {
            get { throw new NotImplementedException(); }
        }

        public string SapSciezkaKataloguPdf
        {
            get { throw new NotImplementedException(); }
        }

        public string WarunekFiltrowaniaKontrahentow
        {
            get { throw new NotImplementedException(); }
        }

        public string BlokadaZamowien
        {
            get { throw new NotImplementedException(); }
        }

        public string BazowaData
        {
            get { throw new NotImplementedException(); }
        }

        public string OpakowanieSql
        {
            get { throw new NotImplementedException(); }
        }

        public string IloscMinSql
        {
            get { throw new NotImplementedException(); }
        }

        public string PokazujTylkoCennik
        {
            get { throw new NotImplementedException(); }
        }

        public bool LiczonyOdCenyNetto
        {
            get { throw new NotImplementedException(); }
        }

        public string XlMagazynuZapisZamowien
        {
            get { throw new NotImplementedException(); }
        }

        public int XlPoziomKursu
        {
            get { throw new NotImplementedException(); }
        }

        public bool UzwagledniaRezerwacjeStanow 
        {
            get { throw new NotImplementedException(); }
        }


        public string MagazynDomyslnyB2B
        {
            get { throw new NotImplementedException(); }
        }

        public string WalutaDokumentB2B
        {
            get { throw new NotImplementedException(); }
        }

        public bool ImieNazwiskoKlienta
        {
            get { throw new NotImplementedException(); }
        }

        public string BlokadaZamowienB2B
        {
            get { throw new NotImplementedException(); }
        }

        public int SubiektTerminDostawy
        {
            get { throw new NotImplementedException(); }
        }

        public string WfmagPoleEmailPracownika
        {
            get { throw new NotImplementedException(); }
        }

        public string WfmagPoleHasloPracownika
        {
            get { throw new NotImplementedException(); }
        }

        public string B2BWaluta
        {
            get { throw new NotImplementedException(); }
        }


        public string B2bMinimalnaWartoscZamowienia
        {
            get { throw new NotImplementedException(); }
        }

        public int SubiektStatusDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        public string OptimaNazwaFirmy
        {
            get { throw new NotImplementedException(); }
        }

        public string B2bFaktoring
        {
            get { throw new NotImplementedException(); }
        }

        public int OptimaRodzajDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        public int OptimaTypDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        public string OptimaSeriaDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        public string ZamowienieOpis
        {
            get { throw new NotImplementedException(); }
        }

        public string ZamowienieOpisNaglowek
        {
            get { throw new NotImplementedException(); }
        }

        public string SymbolProduktu
        {
            get { throw new NotImplementedException(); }
        }

        public string SymbolMagazynow
        {
            get { throw new NotImplementedException(); }
        }

        public string StanyRezerwacji
        {
            get { throw new NotImplementedException(); }
        }

        public string OptimaNazwaDokumentu
        {
            get { throw new NotImplementedException(); }
        }



        public string NazwaFirmy
        {
            get { throw new NotImplementedException(); }
        }

        public string NazwaProduktu
        {
            get { throw new NotImplementedException(); }
        }

        public string B2bUkryty
        {
            get { throw new NotImplementedException(); }
        }


        public string ProduktStanMinimalny
        {
            get { throw new NotImplementedException(); }
        }


        public string WfmagTypProduktow
        {
            get { throw new NotImplementedException(); }
        }

        public string AtrybutyPrefiks
        {
            get { throw new NotImplementedException(); }
        }

        public bool WfmagCzyPobieracKorzenDrzewaKategorii
        {
            get { throw new NotImplementedException(); }
        }

        public string WfmagSymbolArtykulu
        {
            get { throw new NotImplementedException(); }
        }

        public string WfmagStanProduktu
        {
            get { throw new NotImplementedException(); }
        }

        public void SynchronizacjaPobierzDostepneMagazyny(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public string WfmagNazwaFirmy
        {
            get { throw new NotImplementedException(); }
        }

        public string WfmagFormatNumeracji
        {
            get { throw new NotImplementedException(); }
        }

        public string WfmagMagazynZamowienia
        {
            get { throw new NotImplementedException(); }
        }


        public bool StanRezerwacji
        {
            get { throw new NotImplementedException(); }
        }

        public bool Rezerwacja
        {
            get { throw new NotImplementedException(); }
        }



        
        public string WfmagKontrahentGrupa
        {
            get { throw new NotImplementedException(); }
        }

        public string WfmagDodatkoweParametryProduktowSelect
        {
            get { throw new NotImplementedException(); }
        }

        public string WfmagKtoreCechyKategorie
        {
            get { throw new NotImplementedException(); }
        }




        public string WmfmagProduktyTlumaczenieSelect
        {
            get { throw new NotImplementedException(); }
        }

        public string EmailFrom
        {
            get { throw new NotImplementedException(); }
        }

        public string EmailNazwaUzytkownika
        {
            get { throw new NotImplementedException(); }
        }


        public string MailingEmailFrom
        {
            get { throw new NotImplementedException(); }
        }

        public string MailingEmailNazwaUzytkownika
        {
            get { throw new NotImplementedException(); }
        }


        public string EmailHost
        {
            get { throw new NotImplementedException(); }
        }

        public string EmailHaslo
        {
            get { throw new NotImplementedException(); }
        }

        public string MailingEmailHost
        {
            get { throw new NotImplementedException(); }
        }

        public string MailingEmailHaslo
        {
            get { throw new NotImplementedException(); }
        }


        public bool MaileTylkoSolex
        {
            get { throw new NotImplementedException(); }
        }


        public bool ApiAktywneDlaKlientow
        {
            get { throw new NotImplementedException(); }
        }


        //public string OptimaWysylajTylkoProduktyZAtrybutem
        //{
        //    get { throw new NotImplementedException(); }
        //}


        public IEnumerable<string> KlienciWyszukiwanie
        {
            get { throw new NotImplementedException(); }
        }


        public IEnumerable<string> AdresyWyszukiwanie
        {
            get { throw new NotImplementedException(); }
        }


        public HashSet<string> DomyslneUprawnieniaPracownik
        {
            get { throw new NotImplementedException(); }
        }

        public HashSet<string> DomyslneUprawnieniaPrzedstawiciel
        {
            get { throw new NotImplementedException(); }
        }

        public HashSet<string> DomyslneUprawnieniaOddzial
        {
            get { throw new NotImplementedException(); }
        }


        public string DomyslneZdjecieSciezka
        {
            get { throw new NotImplementedException(); }
        }


        public JakieCenyPokazywac CenaDetalicznaKartaProduktuPokazuj
        {
            get { throw new NotImplementedException(); }
        }


        public int OfertyCzasTrwania
        {
            get { throw new NotImplementedException(); }
        }


        public void SynchronizacjaPobierzPoleTekst1(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleMagazynDomyslny(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleTekst5(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleTekst4(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleTekst3(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleTekst2(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleIndywidualnaStawaVat(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleDomyslnaWaluta(Klient item, string empty, string waluta,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleBlokadaZamowien(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzMinimalnaWartoscZamowienia(Klient item, string p,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleOpiekun(Klient item, string p, Dictionary<string, string> pars,
            string domyslny = null)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleDrugiOpiekun(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPolePrzedstawiciel(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleEmail(Klient item, string email)
        {
            throw new NotImplementedException();
        }

        public string PoleHasloPobierz(string domyslne, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleHasloZrodlowe(Klient item, string empty, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleJezyk(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaUstawPoziomCeny(Klient item, int? nullable)
        {
            throw new NotImplementedException();
        }


        public void SynchronizacjaPobierzPoleSkype(Klient item, string domyslne, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleGaduGadu(Klient item, string domyslne, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleKlientNadrzedny(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleDostawa(Produkt item, string p, Dictionary<string, string> pars,
            DateTime? data_dostawy)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleOjciec(Produkt item, string domyslne, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzPoleIndywidualnaStawaVat(Klient item, string p, Dictionary<string, string> pars,
            bool kontrahentue, bool krajue)
        {
            throw new NotImplementedException();
        }


        public string KatalogPlikowWymianySymplex
        {
            get { throw new NotImplementedException(); }
        }

        public string NazwaPlikuZKontrahentamiWymianySymplex { get { throw new NotImplementedException(); } }
        public string NazwaPlikuZDokumentamiWymianySymplex { get { throw new NotImplementedException(); } }
        public string NazwaPlikuZProduktamiWymianySymplex { get { throw new NotImplementedException(); } }
        public string NazwaPlikuZKategoriamiWymianySymplex { get { throw new NotImplementedException(); } }
        public string KatalogPlikowWymianyTema { get { throw new NotImplementedException(); } }

        public string KatalogPlikowZDokumentamiTema { get { throw new NotImplementedException(); } }
        public string NazwaPlikuZKontrahentamiTema { get { throw new NotImplementedException(); } }

        public string NazwaPlikuZPrzedstawicielamiTema { get { throw new NotImplementedException(); } }

        public string NazwaPlikuZDokumentamiTema { get { throw new NotImplementedException(); } }
        public string NazwaPlikuZProduktamiTema { get { throw new NotImplementedException(); } }
        public string NazwaPlikuZCenamiTema { get { throw new NotImplementedException(); } }
        public string NazwaPlikuZProducentamiTema { get { throw new NotImplementedException(); } }
        public string NazwaPlikuZRabatamiTema { get { throw new NotImplementedException(); } }
        public string NazwaPlikuZeStanamiTema { get { throw new NotImplementedException(); } }
        public string SeparatorDoPlikuCsvTema { get { throw new NotImplementedException(); } }
        public string ApiAdresVendo { get { throw new NotImplementedException(); } }
        public string ApiKontoVendo
        {
            get { throw new NotImplementedException(); }
        }
        public string ApiHasloVendo
        {
            get { throw new NotImplementedException(); }
        }

        public string KatalogPlikowWymianyZamowienSymplex
        {
            get { throw new NotImplementedException(); }
        }


        public ERPProviderzy ProviderERP
        {
            get { throw new NotImplementedException(); }
        }

        public System.Collections.Specialized.NameValueCollection DoKolekcjiWartosciNazw()
        {
            throw new NotImplementedException();
        }


        public bool CzyPoleJestSynchronizowane(Type type, string nazwaPola)
        {
            throw new NotImplementedException();
        }


        public string TypDomyslnyFiltru
        {
            get { throw new NotImplementedException(); }
        }


        public string EnovaTymczasowaSciezkaPDF
        {
            get { throw new NotImplementedException(); }
        }

        public WidcznoscProduktowWOptimie OptimaKtoreTowaryEksportowac
        {
            get { throw new NotImplementedException(); }
        }

        System.Collections.Specialized.NameValueCollection IConfigSynchro.DoKolekcjiWartosciNazw()
        {
            throw new NotImplementedException();
        }

        HashSet<string> IConfigSynchro.DomyslneUprawnieniaPracownik
        {
            get { throw new NotImplementedException(); }
        }

        HashSet<string> IConfigSynchro.DomyslneUprawnieniaPrzedstawiciel
        {
            get { throw new NotImplementedException(); }
        }


        string IConfigSynchro.EmailFromPrzyjaznaNazwa
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.KatalogZObrazkami
        {
            get { throw new NotImplementedException(); }
        }

        int? IConfigSynchro.SubiektKodTransakcjiDlaKlientowEU
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.DokumentyMailOnNowymIleDniWstecz
        {
            get { throw new NotImplementedException(); }
        }

        Dictionary<int, Jezyk> IConfigSynchro.JezykiWSystemie
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        Dictionary<string, Jezyk> IConfigSynchro.JezykiWSystemieSlownikPoSymbolu
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.SubiektPodmiot
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.SubiektSzyfrujHaslo
        {
            get { throw new NotImplementedException(); }
        }

        int[] IConfigSynchro.AtrybutyRodzin
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.CoIleDniZmieniacHaslo
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.AtrybutZCechy
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.SzablonNiestandardowyNazwa
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.SzablonNiestandardowySciezkaWzgledna
        {
            get { throw new NotImplementedException(); }
        }

        Dictionary<int, StatusZamowienia> IConfigSynchro.StatusyZamowien
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.JezykIDPolski
        {
            get { throw new NotImplementedException(); }
        }

        string[] IConfigSynchro.PolaWlasneCechy
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.AtrybutKategoriiZERP
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.AtrybutProducentaZERP
        {
            get { throw new NotImplementedException(); }
        }

        List<string> IConfigSynchro.PolaDoWyzerowania
        {
            get { throw new NotImplementedException(); }
        }

        char[] IConfigSynchro.SeparatorGrupKlientow
        {
            get { throw new NotImplementedException(); }
        }

        HashSet<ZCzegoLiczycGradacje> IConfigSynchro.ZCzegoLiczycGradacje
        {
            get { throw new NotImplementedException(); }
        }

        //DateTime IConfigSynchro.GradacjeOdKiedyLiczyc
        //{
        //    get { throw new NotImplementedException(); }
        //}

        bool IConfigSynchro.GradacjeUwzgledniajRodziny
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.OptimaIdSzablonuWydrukuDoPdf
        {
            get { throw new NotImplementedException(); }
        }

        int? IConfigSynchro.ProduktyNaZamowienieCechaID
        {
            get { throw new NotImplementedException(); }
        }

        decimal IConfigSynchro.ProduktyNaWyczerpaniu_procentStanuMinimalnego
        {
            get { throw new NotImplementedException(); }
        }

        decimal IConfigSynchro.ProduktyNieDostepnePrzezDluzszyCzas_iloscDni
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.PodczasWyszukiwaniaZmienPolskeZnaki
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.ZamowieniaTworzRezerwacje
        {
            get { throw new NotImplementedException(); }
        }

      
        string IConfigSynchro.EnovaPoleDoPobieraniaZDokumentow
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.EnovaPoleDoZapisywaniaUwagNaPozycjiDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.EnovaPole2DoZapisywaniaUwagNaPozycjiDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.SortowanieNaturalneListyProduktow
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.SprawdzajStanyMagazynowe
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.ZIleDniDomyslniePokazywacDokumenty
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.WielowybieralnoscKategorii
        {
            get { throw new NotImplementedException(); }
        }

    
        string IConfigSynchro.OptimaAtrybutZeZdjeciami
        {
            get { throw new NotImplementedException(); }
        }

      
        string IConfigSynchro.ERPcs
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.ERPcs2
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.ERPHaslo
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.ERPLogin
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.PokazujCenyDlaNiezalogowanych
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.WysylajPowiadomienieFakturaGdyBrakPdf
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.EnovaDoZapisuKategoriiZamowienia
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.EnovaCechaDoPobraniaJakoStatusDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.KatalogProgramuKsiegowego
        {
            get { throw new NotImplementedException(); }
        }

   

        string IConfigSynchro.EnovaPoleDoPobieraniaZDokumentow2
        {
            get { throw new NotImplementedException(); }
        }

        int? IConfigSynchro.GradacjeUzgledniaProduktyZCecha
        {
            get { throw new NotImplementedException(); }
        }

        int? IConfigSynchro.ProduktyDropshipingCechaID
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.BlokujDodawanieDoKoszykaDlaBrakujacychProduktow
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.GetLicense(Licencje key)
        {
            throw new NotImplementedException();
        }

        Dictionary<long, string> IConfigSynchro.Tlumaczenia(int? lang)
        {
            throw new NotImplementedException();
        }


        string IConfigSynchro.PobierzTlumaczenie(int lang, string symbol, params object[] parametryFormatuFrazy)
        {
            throw new NotImplementedException();
        }

        int IConfigSynchro.GetPriceLevelHurt
        {
            get { throw new NotImplementedException(); }
        }

        int? IConfigSynchro.GetPriceLevelDetal
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.PobierzSerieDlaWaluty(string waluta)
        {
            throw new NotImplementedException();
        }


        bool IConfigSynchro.ProduktyDropshipingPokazujNaStanieJesliJest
        {
            get { throw new NotImplementedException(); }
        }

        void IConfigSynchro.PrzeladujResetujStatusy()
        {
            throw new NotImplementedException();
        }

        string IConfigSynchro.CechaAuto
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.EksportTylkoKontZHaslem
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.EnovaPoleDoZapisuPlatnikaDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.EnovaZamowieniaBufor
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.MaksimumDokumentowWPaczce
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.PobierzSzablonWydrukuEnova(string symboljezykawydruku)
        {
            throw new NotImplementedException();
        }

        char[] IConfigSynchro.SeparatorAtrybutowWCechach
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.PrzedzialyCenowe
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerable<string> IConfigSynchro.DokumentyWyszukiwanie
        {
            get { throw new NotImplementedException(); }
        }

        List<string> IConfigSynchro.ProduktyWyszukiwanie
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerable<string> IConfigSynchro.KoszykPozycjeWyszukiwanie
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.SferaMaksPobranNaOkres
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.SferaPobieranieLimitOkres
        {
            get { throw new NotImplementedException(); }
        }

        //bool IConfigSynchro.MinimumLogistyczneWymagane
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //bool IConfigSynchro.MinimumLogistyczneWymagane
        //{
        //    get { throw new NotImplementedException(); }
        //}

        WidcznoscProduktowWSubiekcie IConfigSynchro.SubiektWidocznoscTowarow
        {
            get { throw new NotImplementedException(); }
        }

        WidcznoscProduktowWXl IConfigSynchro.XlWidocznoscTowarow
        {
            get { throw new NotImplementedException(); }
        }
        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeNazwa(Produkt item, Jezyk jezyk, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeKod(Produkt item, Jezyk jezyk, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeOpis(Produkt item, Jezyk jezyk, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeOpis2(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeOpis3(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeOpis4(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeOpis5(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeOpisKrotki(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeOpisKrotki2(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeOpisKrotki3(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeOpisKrotki4(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeOpisKrotki5(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjePoleTekst1(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjePoleTekst2(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjePoleTekst3(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjePoleTekst4(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjePoleTekst5(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeRodzina(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeKolumnaTekst1(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeKolumnaTekst2(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeKolumnaTekst3(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeKolumnaTekst4(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeKolumnaTekst5(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeMetaOpis(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzLokalizacjeMetaSlowaKluczowe(Produkt item, Jezyk j, string domyslnePole,
            Dictionary<string, string> pars, ref List<Tlumaczenie> tlumaczenia)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleLiczba1(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzWidocznoscProduktuZPola(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzObjetoscProduktu(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzWageProduktu(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }
        void IConfigSynchro.SynchronizacjaPobierzPoleLiczba2(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleLiczba3(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleLiczba4(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleLiczba5(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzKolumnaLiczba1(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzKolumnaLiczba2(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzKolumnaLiczba3(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzKolumnaLiczba4(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzKolumnaLiczba5(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzIloscMinimlna(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzIloscWOpakowaniu(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

    
        string IConfigSynchro.SymplexSciezkaKataloguPdf
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //int IConfigSynchro.XlTypDokumentu
        //{
        //    get { throw new NotImplementedException(); }
        //}

        int? IConfigSynchro.IdAtrybutuDostawy
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.ZamowienieWImieniuKlientaWysylajMaile
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.BrakPlatnosciKlientaJesliTerminJestZerowy
        {
            get { throw new NotImplementedException(); }
        }

        int? IConfigSynchro.IleWczesniejZmianaDostawa
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.KatalogDoZapisuZalacznikowZFormularzy
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.CzasWyswietlaniaKoszyka
        {
            get { throw new NotImplementedException(); }
        }


        string IConfigSynchro.BazowaData
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.SeparatorMail
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IConfigSynchro.OpakowanieSql
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.IloscMinSql
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.EmailCustomerError
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TrybPokazywaniaFiltrow TrybPokazywaniaFiltrow { get; set; }

        bool IConfigSynchro.LiczonyOdCenyNetto
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.XlMagazynuZapisZamowien
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.XlPoziomKursu
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.ImieNazwiskoKlienta
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.WfmagPoleEmailPracownika
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.WfmagPoleHasloPracownika
        {
            get { throw new NotImplementedException(); }
        }

        //string IConfigSynchro.B2BWaluta
        //{
        //    get { throw new NotImplementedException(); }
        //}

        int IConfigSynchro.SubiektStatusDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.OptimaNazwaFirmy
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.B2bFaktoring
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.OptimaRodzajDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.OptimaTypDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.OptimaSeriaDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.SymbolMagazynow
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.OptimaNazwaDokumentu
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.NazwaFirmy
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.B2bUkryty
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.ProduktStanMinimalny
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.WfmagTypProduktow
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.AtrybutyPrefiks
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.WfmagCzyPobieracKorzenDrzewaKategorii
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.WfmagSymbolArtykulu
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.WfmagStanProduktu
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.WfmagNazwaFirmy
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.WfmagFormatNumeracji
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.WfmagMagazynZamowienia
        {
            get { throw new NotImplementedException(); }
        }


        string IConfigSynchro.WfmagKtoreCechyKategorie
        {
            get { throw new NotImplementedException(); }
        }

        string[] IConfigSynchro.SeparatoryDrzewkaKategorii
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.EmailNazwaUzytkownika
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.MailingEmailFrom
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.MailingEmailNazwaUzytkownika
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.EmailHost
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.EmailHaslo
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.MailingEmailHost
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.MailingEmailHaslo
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.MaileTylkoSolex
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.InfoPrzekroczoneStany
        {
            get { throw new NotImplementedException(); }
        }

        bool IConfigSynchro.DadawanieAtrybutuDoKategorii
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.DomyslneZdjecieSciezka
        {
            get { throw new NotImplementedException(); }
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleTekst1(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleMagazynDomyslny(Klient item, string p,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleTekst5(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleTekst4(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleTekst3(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleTekst2(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleIndywidualnaStawaVat(Klient item, string p,
            Dictionary<string, string> pars, bool kontrahentue, bool krajue)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleDomyslnaWaluta(Klient item, string empty, string waluta,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleBlokadaZamowien(Klient item, string p,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzMinimalnaWartoscZamowienia(Klient item, string p,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleOpiekun(Klient item, string p, Dictionary<string, string> pars,
            string domyslny)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleDrugiOpiekun(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPolePrzedstawiciel(Klient item, string p,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleEmail(Klient item, string email)
        {
            throw new NotImplementedException();
        }

        string IConfigSynchro.PoleHasloPobierz(string domyslne, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleHasloZrodlowe(Klient item, string empty,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleJezyk(Klient item, string p, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaUstawPoziomCeny(Klient item, int? nullable)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleSkype(Klient item, string domyslne, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleGaduGadu(Klient item, string domyslne,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleKlientNadrzedny(Klient item, string p,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }



        void IConfigSynchro.SynchronizacjaPobierzPoleDostawa(Produkt item, string p, Dictionary<string, string> pars,
            DateTime? data_dostawy)
        {
            throw new NotImplementedException();
        }

        void IConfigSynchro.SynchronizacjaPobierzPoleOjciec(Produkt item, string domyslne,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        HashSet<ModulyOptima> IConfigSynchro.JakieModulyOptima
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.SapAdresSerweraLicencji
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.SapSciezkaKataloguPdf
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.KatalogPlikowWymianySymplex
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.KatalogPlikowWymianyZamowienSymplex
        {
            get { throw new NotImplementedException(); }
        }

        ERPProviderzy IConfigSynchro.ProviderERP
        {
            get { throw new NotImplementedException(); }
        }

        //IList<TypWSystemie> IConfigSynchro.SystemTypes
        //{
        //    get { throw new NotImplementedException(); }
        //}

        string IConfigSynchro.TypDomyslnyFiltru
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.EnovaTymczasowaSciezkaPDF
        {
            get { throw new NotImplementedException(); }
        }

        WidcznoscProduktowWOptimie IConfigSynchro.OptimaKtoreTowaryEksportowac
        {
            get { throw new NotImplementedException(); }
        }

        string IConfigSynchro.BazaXl
        {
            get { throw new NotImplementedException(); }
        }


        int IConfigSynchro.XlIdFirmy
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.XlTypDokumentuZamowienia
        {
            get { throw new NotImplementedException(); }
        }

        DateTime IConfigSynchro.DokumentyOdKiedyPobierane
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.MaksimumWydrukowPDF
        {
            get { throw new NotImplementedException(); }
        }

        DateTime IConfigSynchro.OdKiedyDrukowacPdf
        {
            get { throw new NotImplementedException(); }
        }

        int IConfigSynchro.CzasDoZamknieciaSynchronizacji
        {
            get { throw new NotImplementedException(); }
        }


        public void SynchronizacjaPobierzLiczbaSztukNaWarstwie(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzLiczbaSztukNaPalecie(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzGlebokoscOpakowaniaJednostkowego(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzSzerokoscOpakowaniaJednostkowego(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzWysokoscOpakowaniaJednostkowego(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzWageOpakowaniaJednostkowego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public HashSet<string> SlowaWymaganeWDokumencie
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public HashSet<string> SlowaZakazaneWDokumencie
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string SqlDoWyciaganiWzorcaWydrukuSubiektBioPlanet => throw new NotImplementedException();

        public void SynchronizacjaPobierzObjetoscOpakowaniaJednostkowego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzGlebokoscOpakowaniaZbiorczego(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzSzerokoscOpakowaniaZbiorczego(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzWysokoscOpakowaniaZbiorczego(Produkt item, string domyslnePole,
            Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzWageOpakowaniaZbiorczego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzObjetoscOpakowaniaZbiorczego(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzIloscWOpakowaniuZbiorczym(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzGlebokoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzSzerokoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzWysokoscPalety(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public void SynchronizacjaPobierzWagePalety(Produkt item, string domyslnePole, Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        public string PobierzTlumaczenie(int lang, string symbol, string symnbolDoHash, out long symbolHash, MiejsceFrazy miejsce = MiejsceFrazy.Brak, params object[] parametryFormatuFrazy)
        {
            throw new NotImplementedException();
        }
    }
}