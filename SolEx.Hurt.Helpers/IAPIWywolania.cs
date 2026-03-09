using System;
using System.Collections.Generic;
using RestSharp;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Helpers
{
    public interface IAPIWywolania
    {
        string URL { get; }

        /// <summary>
        /// Czyœci koszyk zalogowanego u¿ytkownika
        /// </summary>
        /// <returns></returns>
        void ClearBasket();

        /// <summary>
        /// Dodaje uwagi do koszyka
        /// </summary>
        /// <param name="desc">Opis</param>
        /// <param name="clear">Czy usun¹æ poprzednie uwagi</param>
        /// <returns></returns>
        void AddDescription(string desc, bool clear);

        /// <summary>
        /// Dodaje pozycje do koszyka
        /// </summary>
        /// <param name="items">Pozycje do dodania</param>
        /// <returns></returns>
        void AddPRoductsToBasket(List<KeyValuePair<string, decimal>> items);

        List<long> PobierzDostepneProduktyKlienta(long klienta);
        void DodajDokumenty(List<KlasaOpakowujacaDokumentyDoWyslania> items);
        void DeleteDocuments(HashSet<int> items);
        Dictionary<int,PoziomCenowy> PobierzPoziomyCen();
        Dictionary<long, Klient> PobierzKlientow();
        void AktualizujKlientow(IEnumerable<Klient> items);
        Dictionary<long, Produkt> PobierzProdukty();
        Dictionary<int, Jezyk> GetLanguages();
        List<Ustawienie> GetSettings();
        void UpdateSetting(List<Ustawienie> items);
        Ustawienie GetSetting(string symbol);
        //List<TypWSystemie> GetSystemTypes();
        //void DodajSystemTyp(List<TypWSystemie> list);
        List<TlumaczeniePole> GetSystemNames();
        Dictionary<long, Tlumaczenie> GetSlowniki();
        Dictionary<int,Atrybut> PobierzAtrybuty();
        Dictionary<long, Cecha> PobierzCechy();
        Dictionary<long, Cecha> PobierzCechyDlaAtrybutow(HashSet<int> idAtrybutow);
        List<TlumaczeniePole> AddSystemNames(List<TlumaczeniePole> items);
        List<Zadanie> PobierzZadaniaSynchronizatora();
        void WyslijMailePowiatalne();
        void WyslijMaileNoweProdukty(List<long> idCechyKoniecznej, List<long> idCechyNieMozePosiadac, bool wysylajDoSubkont);
        void AktualizujZadania(List<Zadanie> lista);
        void WysylaniePowiadomienONowychFakturach(HashSet<int> idKategoriKlientow);
        void WykonajZadaniaAdministracyjne();
        void DodajTlumaczenia(IEnumerable<Tlumaczenie> items);
        void UsunTlumaczenia(IEnumerable<long> ids);
        void UsunMagazyny(HashSet<int> ids);
        List<Magazyn> PobierzMagazyny();
        void WyslijPowiadomieniaODostepnosci();
        void WyslijNewslettery();
        void AktualizujMagazyny(IList<Magazyn> mags);
        List<ProduktStan> PobierzStanyProduktow(Magazyn mag);
        void AktualizujProdukty(IList<Produkt> produktyDoAktualizacji);
        void AktualizujStanyProduktow(List<ProduktStan> listaStanow);
        void UsunStanyProduktow(HashSet<long> idStanow);
        void AktualizujAtrybuty(IList<Atrybut> atrybutyDoAktualizacji);
        void UsunAtrybuty(HashSet<int> atrybutyDoAktualizacji);
        void AktualizujKategorieProduktow(IList<KategoriaProduktu> atrybutyDoAktualizacji);
        void AktualizujKategorieKlientow(IList<KategoriaKlienta> atrybutyDoAktualizacji);
        void UsunKategorieProduktow(HashSet<long> atrybutyDoAktualizacji);
        void AktualizujCechy(IList<Cecha> item);
        void UsunCechy(HashSet<long> ids);
        void UsunKonfekcje(List<long> ids);
        void UsunCechyProdukty(HashSet<long> elementyDoUsuniecia);
        void AktualizujCechyProdukty(IList<ProduktCecha> cechyProduktyDoAktualizacji);
        List<FlatCeny> PobierzCenyKlientow(HashSet<long> idKlienta);
        IEnumerable<Rejestracja> PobierzRejestracje();
        void AktualizujRejestracje(List<Rejestracja> dane);
        List<long> PobierzRabatyID();
        List<long> PobierzProduktyId();
        Dictionary<long, Rabat> PobierzRabaty();
        List<Grupa> PobierzGrupy();
        Dictionary<long, KategoriaProduktu> PobierzKategorie();
        Dictionary<int,KategoriaKlienta> PobierzKategorieKlientow();
        Dictionary<long,KlientKategoriaKlienta> PobierzKlienciKategorie(Dictionary<string, object> parametry);
        void AktualizujKlienciKategorie(IList<KlientKategoriaKlienta> items);
        void UsunKlienciKategorie(HashSet<long> items);
        void UsunKategorieKlientow(HashSet<int> elementyDoUsuniecia);
        Dictionary<long, ProduktCecha> PobierzCechyProdukty(HashSet<long> idCech = null,int max = 5000);
        //HashSet<long> PobierzCechyProduktyID();

        Dictionary<int,HistoriaDokumentuListPrzewozowy> PobierzListyPrzewozowe();
        void WyslijMaileBladSynchronizatora(WiadomoscEmail wiadomoscEmail);
        Dictionary<long, ProduktKategoria> PobierzProduktyKategoriePolaczenia();

        void UsunProduktyKategoriePolaczenia(HashSet<long> elementyDoUsuniecia);
        void UsunRabaty(HashSet<long> elementyDoUsuniecia);
        void AktualizujProduktyKategoriePolaczenia(IList<ProduktKategoria> cechyProduktyDoAktualizacji);
        List<long> PobierzProduktyUkryteID();
        Dictionary<long, ProduktUkryty> PobierzProduktyUkryte(HashSet<int> klientId=null);
        void AktualizujProduktyUkryte(IList<ProduktUkryty> produktyUkryte);
        int PobierzOstatnieIDCechyProdukty();
        void UsunProduktyUkryte(HashSet<long> elementyDoUsuniecia);
        void AktualizujProduktyKategoriePolaczenia(List<ProduktUkryty> pu);
        void AktualizujCenyWyliczoneErp(List<FlatCeny> cenyDoZmiany);
        List<FlatCeny> PobierzCenyWyliczoneErp();
        void UsunCenyWyliczoneErp(List<long> cenyDoZmiany);
        void AktualizujRabaty(List<Rabat> rabatyDoAktualizacji);
        Dictionary<long, CenaPoziomu> PobierzPoziomyCenProduktow();
        List<KlientLimitIlosciowy> PobierzLimityIlosciowe();
        void UsunLimityIlosciowe(List<int> ids );
        void AktualizujLimityIlosciowe(List<KlientLimitIlosciowy> limity);
        void UsunCenyPoziomy(HashSet<long> doUsuniecia);
        void AktualizujPoziomyCenProduktow(IList<CenaPoziomu> ceny);
        Dictionary<long, Adres> PobierzAdresy();
        void AktualizujAdresy(IList<Adres> adresy);
        void UsunAdresy(HashSet<long> elementyDoUsunieciaAdresy);
        Dictionary<long, KlientAdres> PobierzLacznikiAdresow();
        void AktualizujLacznikiAdresow(IList<KlientAdres> adresy);
        void UsunLacznikiAdresow(HashSet<long> elementyDoUsunieciaAdresy);
        Dictionary<int, Kraje> PobierzKraje();
        void AktualizujKraje(IList<Kraje> kraje);
        void UsunKraje(HashSet<int> dousunieca);
        Dictionary<int, Region> PobierzRegiony();
        void UsunRegiony(HashSet<int> doUsuniecia);
        void AktualizujRegiony(IList<Region> regiony);
        void AktalizujListyPrzewozowe(IList<HistoriaDokumentuListPrzewozowy> listyDoAktualizacji);
        void UsunListyPrzewozowe(HashSet<int> elementyDoUsunieciaAdresy);
        void AktalizujZamienniki(IList<ProduktyZamienniki> listyDoAktualizacji);
        Dictionary<long, ProduktyZamienniki> PobierzZamienniki();
        void UsunZamienniki(HashSet<long> dousuniecia);
        List<StatusZamowienia> PobierzStatusyZamowien();
        void AktualizujStatusyZamowien(IList<StatusZamowienia> data);
        void UsunStatusyZamowien(HashSet<int> data);
        List<ZamowienieSynchronizacja> PobierzZamowienia();
        List<ZamowienieSynchronizacja> PobierzWszystkieZamowienia();
        void AktualizujZamowienie(List<ZamowienieSynchronizacja> zapisaneZamowienia);
        void AktualizujPoziomyCen(IList<PoziomCenowy> poziomy_cenDoAktualizacji);
        void UsunPoziomyCen(HashSet<int> doUsuniecia);
        Dictionary<string, ProduktPlik> PlikiProduktowPobierz();
        void PlikProduktowUsun(HashSet<long> DoUsuwania);
        void PlikProduktowDodaj(List<ProduktPlik> DoDodania);
        List<Plik> PlikNaB2BPobierz();
        void PlikNaB2BUsun(List<int> ids);
        List<Plik> PlikNaB2BDodaj(List<Plik> pliki);
        void PlatnosciUsun(List<int> lista);
        string PobierzEksportowaneDane(int IdSzablonu, List<int> list);
        List<Komunikat> WywolajImportEksport(int IdSzablonu, string dane);
        List<StatusDokumentuPDF> PobierzDokumentyDlaKtorychTrzebaDrukowacFaktureElektroniczna();
        void AktualizujDokumentElektroniczne(List<StatusDokumentuPDF> dane);
        Dictionary<long, Jednostka> PobierzJednostki();
        List<long> PobierzPobierzKonfekcjeId();
        List<Konfekcje> PobierzKonfekcje();
        Dictionary<long, ProduktJednostka> PobierzProduktyJednostki();
        void AktualizujJednostki(IList<Jednostka> list);
        void AktualizujKonfekcje(List<Konfekcje> list);
        void UsunJednostki(HashSet<long> jednostkiDoUsuniecia);
        void AktualizujProduktyJednostkiJednostki(IList<ProduktJednostka> jednostkiLacznikiDoAktualizacji);
        void UsunJednostkiLaczniki(HashSet<long> jednostkiLacznikiDoUsuniecia);
        void WyslijPowiadomieniaOTerminiePlatnosci(int IleDniPrzed, int IleDniPo,int? CoIleDniPonowneWyslanie, List<int> kategoriaKlientaNieWysylaj, List<int> kategoriaKlientaWysylaj);
        List<ProduktyKodyDodatkowe> PobierzKodyAlternatywne();
        void AktualizujKodyAlternatywne(List<ProduktyKodyDodatkowe> doAktualizacji);
        void UsunAlternatywneKody(List<int> elementyDoUsuniecia);
        Dictionary<int, long> PobierzHashDokumentow();
        List<int> PobierzKlientowZRabatami();
        Dictionary<int, Sklep> PobierzSklepy();
        void AktualizujSklepy(IList<Sklep> adresy);
        void UsunSklepy(HashSet<long> elementyDoUsunieciaAdresy);
        Dictionary<long, KategoriaSklepu> PobierzSklepyKategorie();
        void AktualizujSklepyKategorie(IList<KategoriaSklepu> adresy);
        void UsunSklepyKategorie(HashSet<long> elementyDoUsunieciaAdresy);

        Dictionary<long, Waluta> PobierzWaluty();
        void AktualizujWaluty(IList<Waluta> waluty);
        void UsunWaluty(HashSet<long> elementyDoUsunieciaWaluty);

        List< SklepKategoriaSklepu> PobierzSklepyKategorieLaczniki();
        void AktualizujSklepyKategorieLaczniki(IList<SklepKategoriaSklepu> adresy);
        void UsunSklepyKategorieLaczniki(HashSet<long> elementyDoUsunieciaAdresy);

        HashSet<string> PobierzMagazynySymbole();

        /// <summary>
        /// Wywo³uje metodê wysy³aj¹c¹ ponownie maile, których wczeœniej nie uda³o siê wys³aæ
        /// </summary>
        /// <returns></returns>
        void WyslijPonownieBledneMaile();

        Dictionary<int, long> PobierzHashPozycjiDokumentow(HashSet<int> idDokumentow);
        List<Plik> PlikiNaB2BDodajPaczkowanie(List<Plik> pliki, Action<string> infoBledy);
        void WylogujKlienta();
        void ZalogujKlienta();
        Dictionary<long, List<HistoriaDokumentuProdukt>> PobierzIdProduktowZDokumentowOStatusie(long[] klienci, int[] statusyDokumentow = null, bool tylkoAktualneOferty = false);

        void WyslijMaileProduktyPrzyjeteNaMagazyn(List<long> idCechKoniecznych, List<long> idCechZabronionych,
            decimal minimalneZwiekszenieStanuPrzelicznik, decimal minimalnaIloscBrakuPrzelicznik, List<int> idMagazynow);
    }
}