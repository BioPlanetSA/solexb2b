using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Modele;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model.Interfaces;
using Rejestracja = SolEx.Hurt.Model.Rejestracja;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IStatystyki
    {
        event Delegata_NowyKlient zdarzenie_NowyKlient;

        event Delegata_PrzeterminowanePlatnosc zdarzenie_PrzeterminowanePlatnosc;

        event Delegata_ProsbaOInformacjeODostepnosci zdarzenie_ProsbaOInformacjeODostepnosci;

        event Delegata_NowyDokument zdarzenie_NoweDokumenty;

        event Delegata_Rejestracja zdarzenie_Rejestracj;

        event Delegata_ZapisDoNewslettera zdarzenie_ZapisDoNewslettera;

        event Delegata_NoweListyPrzewozowe zdarzenie_NoweListyPrzewozowe;

        event Delegata_PojawienieSieProduktow zdarzenie_PojawienieSieProduktow;

        event Delegata_PobranieFaktury zdarzenie_PobranieFaktury;

        event Delegata_ZmianaTerminuRealizacjiZamowienia zdarzenie_ZmianaTerminuRealizacjiZamowienia;

        event Delegata_ZmianaStatusDokumentu zdarzenie_ZmianaStatusDokumentu;

        event Delegata_ResetHasla zdarzenie_ResetHasla;

        event Delegata_PowitanieSzef zdarzenie_PowitanieSzef;

        event Delegata_GenerowanieKluczaApi zdarzenie_GenerowanieKluczaApi;

        event Delegata_ZmianaAdresuIP zdarzenie_ZmianaAdresuIP;

        event Delegata_NoweProduktyWSystemie zdarzenie_NoweProduktyWSystemie;

        event Delegata_WysylanieFormularza zdarzenie_WysylanieFormularza;

        event Delegata_NoweZamowienie zdarzenie_NoweZamowienie_Finalizacja;

        event Delegata_NoweZamowienie zdarzenie_NoweZamowienie_PoImporcieDoERP;

        event Delegata_BladImportu zdarzenie_BladImportu;

        event Delegata_ZamowienieOdrzucone zdarzenie_ZamowienieOdrzucone;

        event Delegata_ZamowienieZaakceptowane zdarzenie_ZamowienieZaakceptowane;

        event Delegata_ZamowienieDoAkceptacji zdarzenie_ZamowienieDoAkceptacji;
        event Delegata_Newsletter zdarzenie_WysylanieNewslettera;

        event Delegata_ProduktyPrzyjeteNaMagazyn zdarzenie_ProduktyPrzyjeteNaMagazyn;

        bool SprawdzCzestotliwoscPobieraniaPrzezApi(IKlienci tmp, out string komunikatpobieranie, int jezyk);

        void DodajInfoOPobraniuStanuProduktu(IKlienci klient, decimal oczekiwanaIlosc, IProduktBazowy produkt, decimal obecnaIlosc);

        void DodajZdarzenie(DzialaniaUzytkownikow a, IKlienci klient);

        /// <summary>
        /// Dodaje nowe zdarzenie
        /// </summary>
        /// <param name="glowne">Głowny typ</param>
        /// <param name="parametr">Klucz wartości, opiowo co jest wysyłane jako wartość </param>
        /// <param name="wartosc">Logowana wartość</param>
        /// <param name="klient"></param>
        void DodajZdarzenie(ZdarzenieGlowne glowne, string parametr, string wartosc, IKlienci klient);

        /// <summary>
        /// Wyszukuje zdarzenie o określonych parametrach
        /// </summary>
        /// <param name="glowne">Głowny typ</param>
        /// <param name="parametr">Klucz wartości, opiowo co jest wysyłane jako wartość </param>
        /// <param name="wartosc">Logowana wartość</param>
        /// <returns></returns>
        List<DzialaniaUzytkownikow> ZnajdzZdarzenie(ZdarzenieGlowne glowne, string parametr, string wartosc, DateTime? odKiedyPrzegladamyDzialania = null);

        void UsunStareStatystyki();

        void ZdarzenieGenerowanieKluczaApi(IKlient klient);

        void AktualizacjaKlientow(IKlient klient);

        void AktualizacjaKlientow_RozpoznanieNowychKlientow(IList<IKlient> starzy, IList<IKlient> nowi);

        void ZdarzenieNowaRejestracja(Rejestracja rejestracja, int jezyk, List<ParametryPola> lista);

        void ZdarzeniePobranieFaktury(DokumentyBll dokument, IKlient klient, string nazwaFormatu);

        void ZdarzenieResetHasla(IKlient klient);

        void ZdarzenieNowyFormularz(FormularzZapytanieModel model, IKlient klient);

        void ZdarzenieProsbaOInformacjeODostepnosci(IProduktKlienta produkt, IKlient klient);

        void ZdarzenieNoweDokumenty(List<DokumentyBll> dokumenty, IKlient klient);

        void ZdarzeniePojawienieSieProduktow(IList<IProduktKlienta> listaProduktow, IKlient klient);

        void ZdarzeniePowiadomieniePrzeterminowanejNadchodzacejPlatnosc(IList<DokumentyBll> dokumenty, IKlient klient);

        void ZdarzenieZapisDoNewslettera(NewsletterZapisani zapisany);

        void ZdarzenieNoweListyPrzewozowe(DokumentyBll dokument, IList<HistoriaDokumentuListPrzewozowy> listy);

        void ZdarzenieZmianaTerminuRealizacjiZamowienia(DokumentyBll dokument);

        void ZdarzenieZmianaStatusuDokumentu(DokumentyBll dokument);

        void ZdarzeniePowitanieSzef(IKlient klient);

        void ZdarzenieZmianaAdresuIP(IKlient klient, string noweIP, string stareIP);

        void ZdarzenieNoweProduktyWSystemie(IList<ProduktKlienta> listaProduktow, IKlient klient);

        void ZdarzenieNoweZamowienie_PoImporcieERP(ZamowieniaBLL zamowienie, IKlient klient, string[] sciezkadozalacznika = null);

        void ZdarzenieNoweZamowienie_Finalizacja(ZamowieniaBLL zamowienie, IKlient klient, string[] sciezkadozalacznika = null);

        void ZdarzenieBladImportu(ZamowieniaBLL zamowienie, IKlient klient, string[] sciezkadozalacznika = null);

        void ZdarzenieSubkonta_ZamowienieOdrzucone(IKoszykiBLL koszyk, IKlient klient, IKlient odrzucil);

        void ZdarzenieSubkonta_ZamowienieZaakceptowane(ZamowieniaBLL zamowienie, IKlient klient, IKlient akceptowal, string[] sciezkadozalacznika = null);

        void ZdarzenieSubkonta_ZamowienieDoAkceptacji(IKoszykiBLL koszyk, IKlient klient);
        void ZdarzenieWysylanieNewslettera(NewsletterKampania kampania, IKlient klient, IList<string> emaileNaJakiePoszedlMaile);
        IList<DzialaniaUzytkownikow> PobierzParametry(int jezykId, IKlient zadajacy, IList<DzialaniaUzytkownikow> obj, object parametryDoSelecta=null);

        void DodajParametry(IList<DzialaniaUzytkownikow> obj);
        Dictionary<long,DateTime> PobierzDzialaniaUzytkownikow(DateTime data, ZdarzenieGlowne zdarzenieGlowne);

        void ZdarzenieProduktyPrzyjeteNaMagazyn(IList<ProduktPrzyjetyNaMagazyn> produktyZmienioneCeny, IKlient klient);

        void LogujDzialanieUzytkownikowAsync(IKlient klient, string wartosc, ZdarzenieGrupa zdarzenie, ZdarzenieGlowne zdarzenieGlowne);
    }
}