using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Modele;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Text;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model.Interfaces;
using Rejestracja = SolEx.Hurt.Model.Rejestracja;

namespace SolEx.Hurt.Core.BLL
{
    public delegate void Delegata_NowyKlient(IKlient nowyklient);

    public delegate void Delegata_PrzeterminowanePlatnosc(IList<DokumentyBll> dokumenty, IKlient klient);

    public delegate void Delegata_ProsbaOInformacjeODostepnosci(IProduktKlienta produkt, IKlient klient);

    public delegate void Delegata_NowyDokument(List<DokumentyBll> dokumenty, IKlient klient);

    public delegate void Delegata_Rejestracja(Rejestracja rejestracja, IKlient klient, List<ParametryPola> pola);

    public delegate void Delegata_ZapisDoNewslettera(NewsletterZapisani zapisany);

    public delegate void Delegata_NoweListyPrzewozowe(DokumentyBll dokument, IList<HistoriaDokumentuListPrzewozowy> listyPrzewozowe);

    public delegate void Delegata_PojawienieSieProduktow(IList<IProduktKlienta> listaProduktow, IKlient klient);

    public delegate void Delegata_PobranieFaktury(DokumentyBll dokument, IKlient klient);

    public delegate void Delegata_ZmianaTerminuRealizacjiZamowienia(DokumentyBll dokument);

    public delegate void Delegata_ZmianaStatusDokumentu(DokumentyBll dokument);

    public delegate void Delegata_ResetHasla(IKlient klient);

    public delegate void Delegata_PowitanieSzef(IKlient klient);

    public delegate void Delegata_GenerowanieKluczaApi(IKlient klient);

    public delegate void Delegata_ZmianaAdresuIP(IKlient klient, string noweIP, string stareIP);

    public delegate void Delegata_NoweProduktyWSystemie(IList<ProduktKlienta> listaProduktow, IKlient klient);

    public delegate void Delegata_WysylanieFormularza(FormularzZapytanieModel formularz, IKlient klient);

    public delegate void Delegata_NoweZamowienie(ZamowieniaBLL zamowienie, IKlient klient, string[] sciezkadozalacznika = null);

    public delegate void Delegata_BladImportu(ZamowieniaBLL zamowienie, IKlient klient, string[] sciezkadozalacznika = null);

    public delegate void Delegata_ZamowienieOdrzucone(IKoszykiBLL koszyk, IKlient klient, IKlient odrzucil);

    public delegate void Delegata_ZamowienieZaakceptowane(ZamowieniaBLL zamowienie, IKlient klient, IKlient akceptowal, string[] sciezkadozalacznika = null);

    public delegate void Delegata_ZamowienieDoAkceptacji(IKoszykiBLL koszyk, IKlient klient);

    public delegate void Delegata_Newsletter(NewsletterKampania kampania, IKlient klient);

    public delegate void Delegata_ProduktyPrzyjeteNaMagazyn(IList<ProduktPrzyjetyNaMagazyn> produkty, IKlient klient);

    public class Statystyki : BllBazaCalosc, IStatystyki
    {
        private const string Cache = "statystyki_lista";

        public event Delegata_NowyKlient zdarzenie_NowyKlient;

        public event Delegata_PrzeterminowanePlatnosc zdarzenie_PrzeterminowanePlatnosc;

        public event Delegata_ProsbaOInformacjeODostepnosci zdarzenie_ProsbaOInformacjeODostepnosci;

        public event Delegata_NowyDokument zdarzenie_NoweDokumenty;

        public event Delegata_Rejestracja zdarzenie_Rejestracj;

        public event Delegata_ZapisDoNewslettera zdarzenie_ZapisDoNewslettera;

        public event Delegata_NoweListyPrzewozowe zdarzenie_NoweListyPrzewozowe;

        public event Delegata_PojawienieSieProduktow zdarzenie_PojawienieSieProduktow;

        public event Delegata_PobranieFaktury zdarzenie_PobranieFaktury;

        public event Delegata_ZmianaTerminuRealizacjiZamowienia zdarzenie_ZmianaTerminuRealizacjiZamowienia;

        public event Delegata_ZmianaStatusDokumentu zdarzenie_ZmianaStatusDokumentu;

        public event Delegata_ResetHasla zdarzenie_ResetHasla;

        public event Delegata_PowitanieSzef zdarzenie_PowitanieSzef;

        public event Delegata_GenerowanieKluczaApi zdarzenie_GenerowanieKluczaApi;

        public event Delegata_ZmianaAdresuIP zdarzenie_ZmianaAdresuIP;

        public event Delegata_NoweProduktyWSystemie zdarzenie_NoweProduktyWSystemie;

        public event Delegata_WysylanieFormularza zdarzenie_WysylanieFormularza;

        public event Delegata_NoweZamowienie zdarzenie_NoweZamowienie_Finalizacja;

        public event Delegata_NoweZamowienie zdarzenie_NoweZamowienie_PoImporcieDoERP;

        public event Delegata_BladImportu zdarzenie_BladImportu;

        public event Delegata_ZamowienieOdrzucone zdarzenie_ZamowienieOdrzucone;

        public event Delegata_ZamowienieZaakceptowane zdarzenie_ZamowienieZaakceptowane;

        public event Delegata_ZamowienieDoAkceptacji zdarzenie_ZamowienieDoAkceptacji;

        public event Delegata_Newsletter zdarzenie_WysylanieNewslettera;

        public event Delegata_ProduktyPrzyjeteNaMagazyn zdarzenie_ProduktyPrzyjeteNaMagazyn;

        public void ZdarzenieGenerowanieKluczaApi(IKlient klient)
        {
            if (zdarzenie_GenerowanieKluczaApi == null)
            {
                return;
            }
            try
            {
                DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.GenerowanieNowegoKluczaApi, null);
                ak.Parametry = new Dictionary<string, string>();
                ak.Parametry.Add("WiadomoscEmail", klient.Email);
                DodajZdarzenie(ak, klient);
            }
            finally
            {
                zdarzenie_GenerowanieKluczaApi(klient);
            }
        }

        public void ZdarzenieNowaRejestracja(Rejestracja rejestracja, int jezyk, List<ParametryPola> lista)
        {
            if (zdarzenie_Rejestracj == null)
            {
                return;
            }
            
            IKlient klient = new Klient(null) { JezykId = jezyk, Email = rejestracja.Email, Id = -333333333};

            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.GenerowanieNowegoKluczaApi, null);
            ak.Parametry.Add("WiadomoscEmail", klient.Email);
            DodajZdarzenie(ak, klient);
            zdarzenie_Rejestracj(rejestracja, klient, lista);
        }

        public void ZdarzeniePobranieFaktury(DokumentyBll dokument, IKlient klient, string nazwaFormatu)
        {
            if (zdarzenie_PobranieFaktury == null)
            {
                return;
            }

            var slownik = new Dictionary<string, string>() { { "nazwa", dokument.NazwaDokumentu } };
            slownik.Add("Format", nazwaFormatu);

            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.PobieranieDokumentu, slownik);
            DodajZdarzenie(ak, klient);
            zdarzenie_PobranieFaktury(dokument, klient);
        }
        public void ZdarzenieWysylanieNewslettera(NewsletterKampania kampania, IKlient klient, IList<string> emaileNaJakiePoszedlMaile )
        {
            if (zdarzenie_WysylanieNewslettera == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.WysylanieNewslettera, null);
            ak.Parametry = new Dictionary<string, string>();
            foreach (var email in emaileNaJakiePoszedlMaile)
            {
                ak.Parametry.Add("WiadomoscEmail", email);
            }

            DodajZdarzenie(ak, klient);
            zdarzenie_WysylanieNewslettera(kampania, klient);
        }
        public void ZdarzenieResetHasla(IKlient klient)
        {
            if (zdarzenie_ResetHasla == null)
            {
                return;
            }

            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.ResetHasla, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("WiadomoscEmail",klient.Email);
            DodajZdarzenie(ak, klient);
            zdarzenie_ResetHasla(klient);
        }

        public void ZdarzenieNowyFormularz(FormularzZapytanieModel model, IKlient klient)
        {
            if (zdarzenie_WysylanieFormularza == null)
            {
                return;
            }

            var slownik = new Dictionary<string, string>();
            slownik.Add("Tytuł", model.Tytul);

            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.WyslanieFormularza, slownik);
            DodajZdarzenie(ak, klient);
            zdarzenie_WysylanieFormularza(model, klient);
        }

        public void ZdarzenieProsbaOInformacjeODostepnosci(IProduktKlienta produkt, IKlient klient)
        {
            if (zdarzenie_ProsbaOInformacjeODostepnosci == null)
            {
                return;
            }

            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.ProsbaOInformacjeODostepnosci, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("Produkt id", produkt.Id.ToString());
            ak.Parametry.Add("Produkt kod", produkt.KodKreskowy.IsNullOrEmpty() ? produkt.Kod : produkt.KodKreskowy);
            DodajZdarzenie(ak, klient);
            zdarzenie_ProsbaOInformacjeODostepnosci(produkt, klient);
        }

        public void ZdarzenieNoweDokumenty(List<DokumentyBll> dokumenty, IKlient klient)
        {
            if (zdarzenie_NoweDokumenty == null)
            {
                return;
            }

            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.NoweDokumentyDlaKlienta, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("Dokumenty", dokumenty.Select(x => x.NazwaDokumentu).ToCsv());
            DodajZdarzenie(ak, klient);
            zdarzenie_NoweDokumenty(dokumenty, klient);
        }

        public void ZdarzeniePowiadomieniePrzeterminowanejNadchodzacejPlatnosc(IList<DokumentyBll> dokumenty, IKlient klient)
        {
            if (zdarzenie_PrzeterminowanePlatnosc == null)
            {
                return;
            }

            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.PrzypomnienieNiezaplaconejFakturze, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("Dokumenty", dokumenty.Select(x => x.NazwaDokumentu).ToCsv());
            ak.Parametry.Add("Dokumenty id", dokumenty.Select(x => x.Id).ToCsv());
            DodajZdarzenie(ak, klient);
            zdarzenie_PrzeterminowanePlatnosc(dokumenty, klient);
        }

        public void ZdarzeniePojawienieSieProduktow(IList<IProduktKlienta> listaProduktow, IKlient klient)
        {
            if (zdarzenie_PojawienieSieProduktow == null)
            {
                return;
            }

            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.PojawienieSieProduktowNaMagazynieInformacjaODostepnosci, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("Produkty id", listaProduktow.Select(x => x.Id).ToCsv());
            ak.Parametry.Add("Produkty kody", listaProduktow.Select(x => string.IsNullOrEmpty(x.KodKreskowy) ? x.Kod : x.KodKreskowy).ToCsv());
            DodajZdarzenie(ak, klient);
            zdarzenie_PojawienieSieProduktow(listaProduktow, klient);
        }

        public void ZdarzenieZapisDoNewslettera(NewsletterZapisani zapisany)
        {
            if (zdarzenie_ZapisDoNewslettera == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.ZapisanieDoNewslettera, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("email", zapisany.Email);

            DodajZdarzenie(ak, null);
            zdarzenie_ZapisDoNewslettera(zapisany);
        }

        public void ZdarzenieNoweListyPrzewozowe(DokumentyBll dokument, IList<HistoriaDokumentuListPrzewozowy> listy)
        {
            if (zdarzenie_NoweListyPrzewozowe == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.NoweListyPrzywozowe, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("Dokument", dokument.NazwaDokumentu);
            ak.Parametry.Add("Dokument id", dokument.Id.ToString());
            ak.Parametry.Add("Listy przewozowe", listy.Select(x => x.NumerListu).ToCsv());

            DodajZdarzenie(ak, null);
            zdarzenie_NoweListyPrzewozowe(dokument, listy);
        }

        public void ZdarzenieZmianaTerminuRealizacjiZamowienia(DokumentyBll dokument)
        {
            if (zdarzenie_ZmianaTerminuRealizacjiZamowienia == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.ZmianaTerminuRealizacji, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("Dokument", dokument.NazwaDokumentu);
            ak.Parametry.Add("Dokument id", dokument.Id.ToString());
            if (dokument.DokumentTerminRealizacji.HasValue)
            {
                ak.Parametry.Add("Termin", dokument.DokumentTerminRealizacji.Value.ToString("G"));
            }
            DodajZdarzenie(ak, null);
            zdarzenie_ZmianaTerminuRealizacjiZamowienia(dokument);
        }

        public void ZdarzenieZmianaStatusuDokumentu(DokumentyBll dokument)
        {
            if (zdarzenie_ZmianaStatusDokumentu == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.ZmianaStatusuDokumentu, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("Dokument", dokument.NazwaDokumentu);
            ak.Parametry.Add("Dokument id", dokument.Id.ToString());
            if (dokument.StatusId.HasValue)
            {
                ak.Parametry.Add("Status",  Calosc.Konfiguracja.StatusyZamowien[dokument.StatusId.Value].Nazwa);
            }
            DodajZdarzenie(ak, null);
            zdarzenie_ZmianaStatusDokumentu(dokument);
        }

        public void ZdarzeniePowitanieSzef(IKlient klient)
        {
            if (zdarzenie_PowitanieSzef == null)
            {
                return;
            }
            try
            {
                DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(new DzialaniaUzytkownikow(ZdarzenieGlowne.PowitanieOdSzefa, null));
                ak.Parametry = new Dictionary<string, string>();
                ak.Parametry.Add("Klient", klient.Email);
                DodajZdarzenie(ak, klient);
            }
            finally
            {
                zdarzenie_PowitanieSzef(klient);
            }
        }

        public void ZdarzenieZmianaAdresuIP(IKlient klient, string noweIP, string stareIP)
        {
            if (zdarzenie_ZmianaAdresuIP == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.ZmianaAdresuIP, null);
            DodajZdarzenie(ak, klient);
            zdarzenie_ZmianaAdresuIP(klient, noweIP, stareIP);
        }

        public void ZdarzenieNoweProduktyWSystemie(IList<ProduktKlienta> listaProduktow, IKlient klient)
        {
            if (zdarzenie_NoweProduktyWSystemie == null)
            {
                return;
            }

            //var czyWysylacPowiadomienie = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(k, TypUstawieniaKlienta.PowiadomieniaMailowe, "NoweProduktyWSystemie");
            //if (czyWysylacPowiadomienie)


                DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.NoweProduktyWSystemie, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("Produkty id", listaProduktow.Select(x => x.Id).ToCsv());
            ak.Parametry.Add("Produkty kody", listaProduktow.Select(x => string.IsNullOrEmpty(x.KodKreskowy) ? x.Kod : x.KodKreskowy).ToCsv());

            DodajZdarzenie(ak, klient);
            zdarzenie_NoweProduktyWSystemie(listaProduktow, klient);
        }

        public void ZdarzenieNoweZamowienie_Finalizacja(ZamowieniaBLL zamowienie, IKlient klient, string[] sciezkadozalacznika = null)
        {
            if (zdarzenie_NoweZamowienie_Finalizacja == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.NoweZamowienie_Finalizacja, PodajParametryZamowienia(zamowienie));
            DodajZdarzenie(ak, klient);
            zdarzenie_NoweZamowienie_Finalizacja(zamowienie, klient, sciezkadozalacznika);
        }

        public void ZdarzenieNoweZamowienie_PoImporcieERP(ZamowieniaBLL zamowienie, IKlient klient, string[] sciezkadozalacznika = null)
        {
            if (zdarzenie_NoweZamowienie_PoImporcieDoERP == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.NoweZamowienie_PoImporcieERP, PodajParametryZamowienia(zamowienie));
            DodajZdarzenie(ak, klient);
            zdarzenie_NoweZamowienie_PoImporcieDoERP(zamowienie, klient, sciezkadozalacznika);
        }

        public void ZdarzenieBladImportu(ZamowieniaBLL zamowienie, IKlient klient, string[] sciezkadozalacznika = null)
        {
            if (zdarzenie_BladImportu == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.BladImportu, PodajParametryZamowienia(zamowienie));
            DodajZdarzenie(ak, klient);
            zdarzenie_BladImportu(zamowienie, klient, sciezkadozalacznika);
        }

        //subkonta
        public void ZdarzenieSubkonta_ZamowienieOdrzucone(IKoszykiBLL koszyk, IKlient klient, IKlient odrzucil)
        {
            if (zdarzenie_ZamowienieOdrzucone == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.ZamówienieOdrzucone, PodajParametryKoszyka(koszyk));
            DodajZdarzenie(ak, klient);
            zdarzenie_ZamowienieOdrzucone(koszyk, klient, odrzucil);
        }

        public void ZdarzenieSubkonta_ZamowienieZaakceptowane(ZamowieniaBLL zamowienie, IKlient klient, IKlient akceptowal, string[] sciezkadozalacznika = null)
        {
            if (zdarzenie_ZamowienieZaakceptowane == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.ZamówienieZaakceptowane, PodajParametryZamowienia(zamowienie));
            DodajZdarzenie(ak, klient);
            zdarzenie_ZamowienieZaakceptowane(zamowienie, klient, akceptowal, sciezkadozalacznika);
        }

        public void ZdarzenieSubkonta_ZamowienieDoAkceptacji(IKoszykiBLL koszyk, IKlient klient)
        {
            if (zdarzenie_ZamowienieDoAkceptacji == null)
            {
                return;
            }
            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.ZamówienieWysłaneDoAkceptacji, PodajParametryKoszyka(koszyk));
            DodajZdarzenie(ak, klient);
            zdarzenie_ZamowienieDoAkceptacji(koszyk, klient);
        }

        private Dictionary<string, string> PodajParametryZamowienia(ZamowieniaBLL zamowienie)
        {
            var slownik = new Dictionary<string, string>();
            slownik.Add("Zamówienie id", zamowienie.Id.ToString());
            slownik.Add("Zamówienie nazwa", zamowienie.DokumentNazwa);
            if (zamowienie.CzyZlozonePrzezPracownika)
            {
                slownik.Add("Pracownik id", zamowienie.PracownikSkladajacyId.ToString());
                slownik.Add("Pracownik nazwa", zamowienie.ZlozonePrzezPracownikaNazwa);
            }
            return slownik;
        }

        private Dictionary<string, string> PodajParametryKoszyka(IKoszykiBLL koszyk)
        {
            var slownik = new Dictionary<string, string>();
            slownik.Add("Koszyk id", koszyk.Id.ToString());
            slownik.Add("Klient id", koszyk.KlientId.ToString());
            return slownik;
        }

        public void ZdarzenieProduktyPrzyjeteNaMagazyn(IList<ProduktPrzyjetyNaMagazyn> produktyZmienioneCeny, IKlient klient)
        {
            if (zdarzenie_ProduktyPrzyjeteNaMagazyn == null)
            {
                return;
            }

            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.ProduktyPrzyjeteNaMagazyn, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("Produkty id", produktyZmienioneCeny.Select(x => x.ProduktId).ToCsv());
            ak.Parametry.Add("Produkty kody", produktyZmienioneCeny.Select(x => string.IsNullOrEmpty(x.KodKreskowy) ? x.KodProduktu : x.KodKreskowy).ToCsv());

            DodajZdarzenie(ak, klient);
            zdarzenie_ProduktyPrzyjeteNaMagazyn(produktyZmienioneCeny, klient);
        }

        /// <summary>
        /// Zdarzenie dodawania nowego klienta - musi być podawane ID bo i tak z bazy danych wyciągamy na nowo cały obiekt - żeby być pewnym że wszystko jest ustawione poprawnie - np. trigery itp
        /// </summary>
        /// <param name="klient1"></param>
        public virtual void AktualizacjaKlientow(IKlient klient)
        {
            if (string.IsNullOrEmpty(klient.Email) || !klient.Aktywny)
            {
                SolexBllCalosc.PobierzInstancje.Log.WarnFormat("Omijam dodawanie statystyk dla nowego klienta id: {0} z powodu braku emaila", klient.Id);
                return;
            }

            DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.DodanieNowegoKlienta, null);
            ak.Parametry = new Dictionary<string, string>();
            ak.Parametry.Add("WiadomoscEmail", klient.Email);
            DodajZdarzenie(ak, klient);

            if (zdarzenie_NowyKlient != null)
            {
                zdarzenie_NowyKlient(klient);
            }
        }

        //public void AktualizacjaKlientow_RozpoznanieNowychKlientow(IList<Klient> klienci)
        //{
        //    foreach (var k in klienci)
        //    {
        //        this.AktualizacjaKlientow_RozpoznanieNowychKlientow(k.Id);
        //    }
        //}

        /// <summary>
        /// metoda rozpoznajaca nowych klientow i klientow nowo aktywowanych
        /// </summary>
        /// <param name="klienciJuzSaWBazie"></param>
        /// <param name="klienciAktualizowaniNoweDane"></param>
        public void AktualizacjaKlientow_RozpoznanieNowychKlientow(IList<IKlient> klienciJuzSaWBazie, IList<IKlient> klienciAktualizowaniNoweDane)
        {
            if (klienciAktualizowaniNoweDane == null)
            {
                return;
            }
            foreach(var k in klienciAktualizowaniNoweDane)
            {
                //zeby czasem niezalagowni nie przyplatali sie tu
                if (k.Id == 0 || !k.Aktywny)
                {
                    continue;
                }

                IKlient istniejacy = klienciJuzSaWBazie.FirstOrDefault(x => x.Id == k.Id);

                //jesli istnieje to sprawczamy czy teraz jest aktywny a wczesniej NIE byl aktywny
                if (istniejacy == null || !istniejacy.Aktywny)
                {
                    this.AktualizacjaKlientow(k);
                    continue;
                }
            }
        }

        public Statystyki(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        /// <summary>
        /// Dodaje nowe zdarzenie
        /// </summary>
        /// <param name="glowne">Głowny typ</param>
        /// <param name="parametr">Klucz wartości, opiowo co jest wysyłane jako wartość </param>
        /// <param name="wartosc">Logowana wartość</param>
        /// <param name="klient"></param>
        public void DodajZdarzenie(ZdarzenieGlowne glowne, string parametr, string wartosc, IKlienci klient)
        {
            DodajZdarzenie(new DzialaniaUzytkownikow(glowne, new Dictionary<string, string> { { parametr, wartosc } }), klient);
        }


        public List<DzialaniaUzytkownikow> ZnajdzZdarzenie(ZdarzenieGlowne glowne, string parametr, string wartosc, DateTime? odKiedyPrzegladamyDzialania = null)
        {
            var data = (DateTime)SqlDateTime.MinValue;
            if (odKiedyPrzegladamyDzialania.HasValue)
            {
                data = odKiedyPrzegladamyDzialania.Value;
            }

            if (string.IsNullOrEmpty(parametr))
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DzialaniaUzytkownikow>(null, x => x.ZdarzenieGlowne == glowne && x.Data>=data).ToList();
            }

            var idDzialanZParametrami = new HashSet<int>( SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DzialaniaUzytkwonikowParametry>(null, x => x.NazwaParametru == parametr && x.Wartosc.Contains(wartosc)).Select(x => x.IdDzialania) );
            if (idDzialanZParametrami!=null && idDzialanZParametrami.Any())
            {
                return Calosc.DostepDane.Pobierz<DzialaniaUzytkownikow>(null, x => x.Data >= data && Sql.In(x.Id, idDzialanZParametrami) ).ToList();
            }
            return null;
        }

        public void DodajZdarzenie(DzialaniaUzytkownikow akcja, IKlienci klient)
        {
            try
            {
                akcja.IpKlienta = SesjaHelper.PobierzInstancje.IpKlienta;
            }
            catch
            {
                // ignored
            }
            if(klient != null)
            {
                akcja.EmailKlienta = klient.Email;
            }
           
            Calosc.DostepDane.AktualizujPojedynczy(akcja);
            Calosc.Cache.UsunObiekt(Cache);
        }

        public bool SprawdzCzestotliwoscPobieraniaPrzezApi(IKlienci tmp, out string komunikatpobieranie, int jezyk)
        {
            string str = Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, "Przekroczona ilosc pobieranych plikow w ostatanim czasie. Aktualny limit to {0} wywołań przez {1} minut.");
            DateTime poczatekprzednialu = DateTime.Now.AddMinutes(-Calosc.Konfiguracja.SferaPobieranieLimitOkres);
            var wszystkie =
                Calosc.DostepDane.Pobierz<DzialaniaUzytkownikow>(null, x => (x.EmailKlienta == tmp.Email) && x.ZdarzenieGlowne == ZdarzenieGlowne.PobieranieDanych && x.Data > poczatekprzednialu).Count();
            bool mozePobrac = wszystkie < Calosc.Konfiguracja.SferaMaksPobranNaOkres;
            komunikatpobieranie = string.Format(str, Calosc.Konfiguracja.SferaMaksPobranNaOkres, Calosc.Konfiguracja.SferaPobieranieLimitOkres);
            return mozePobrac;
        }

        public void DodajInfoOPobraniuStanuProduktu(IKlienci klient, decimal oczekiwanaIlosc, IProduktBazowy produkt, decimal obecnaIlosc)
        {
            var slownik = new Dictionary<string, string>(3);
            slownik.Add("Produkt", string.IsNullOrEmpty(produkt.KodKreskowy) ? produkt.Kod : produkt.KodKreskowy);
            slownik.Add("Oczekiwana ilość", oczekiwanaIlosc.ToString(CultureInfo.InvariantCulture));
            slownik.Add("Obecna ilość", obecnaIlosc.ToString(CultureInfo.InvariantCulture));

            //z.DodajParametr("Produkt nazwa", produkt.nazwa);
            //z.DodajParametr("Produkt EAN", produkt.kod_kreskowy);

            var z = new DzialaniaUzytkownikow(ZdarzenieGlowne.ZapytanieOStan, slownik);

            DodajZdarzenie(z, klient);
        }

        public void UsunStareStatystyki()
        {
            DateTime data = DateTime.Now.AddDays(-90);
            long ilosc = 0;
            do
            {
                using (var trans = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.OpenTransaction())
                {
                    SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.ExecuteNonQuery("delete from DzialaniaUzytkownikow where id in(select top 1000 id from DzialaniaUzytkownikow where Data<@data)", new Dictionary<string, object> {{"@data", data}});
                    trans.Commit();
                }
                ilosc = Calosc.DostepDane.Pobierz<DzialaniaUzytkownikow>(null, x => x.Data < data).Count();
            }
            while (ilosc > 0);
        }

        public Dictionary<long, DateTime> PobierzDzialaniaUzytkownikow(DateTime data, ZdarzenieGlowne zdarzenieGlowne)
        {

            Dictionary<string, long> slownikKlientow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Model.Klient>(null,x=>x.Aktywny && x.Email!=null && x.Id>0).ToDictionary(x => x.Email, x => x.Id);
            Dictionary<long, DateTime> slownikDzialanUzytkownikow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<DzialaniaUzytkownikow>(null,
               x => x.ZdarzenieGlowne == zdarzenieGlowne && x.Data>data && Sql.In(x.EmailKlienta,slownikKlientow.Keys)).GroupBy(x => x.EmailKlienta).ToDictionary(x =>slownikKlientow[x.Key], x =>Sql.Max(x.Max(y=>y.Data)) );

            return slownikDzialanUzytkownikow;
            
        }

        public IList<DzialaniaUzytkownikow> PobierzParametry(int jezykId, IKlient zadajacy, IList<DzialaniaUzytkownikow> dzialania, object parametryDoSelecta = null)
        {
            if (dzialania.IsEmpty())
            {
                return dzialania;
            }

            List<int> dzialaniaID = dzialania.Select(x => x.Id).Distinct().ToList();
            Dictionary<int, List<DzialaniaUzytkwonikowParametry>> parametry = new Dictionary<int, List<DzialaniaUzytkwonikowParametry>>();

            //jak jest za duzo dzialaniaID to paczkowanie musi byc
            if (dzialaniaID.Count < 1000)
            {
                parametry = Calosc.DostepDane.Pobierz<DzialaniaUzytkwonikowParametry>(null, x => Sql.In(x.IdDzialania, dzialaniaID)).GroupBy(x => x.IdDzialania).ToDictionary(x => x.Key, x => x.Select(z => z).ToList());
            }
            else
            {
                //paczkowanie
                int rozmiarPaczki = 500;
                for (int i = 0; i < dzialaniaID.Count; i = i + rozmiarPaczki)
                {
                    List<int> tempIDs = dzialaniaID.Skip(i).Take(rozmiarPaczki).ToList();

                    Dictionary<int, List<DzialaniaUzytkwonikowParametry>> tempData = Calosc.DostepDane.Pobierz<DzialaniaUzytkwonikowParametry>(null, x => Sql.In(x.IdDzialania, tempIDs)).GroupBy(x => x.IdDzialania).ToDictionary(x => x.Key, x => x.Select(z => z).ToList());

                    parametry.AddRange(tempData); //dodajemy do słownika - jak sie wywali to znaczy ze ID zdublowane?
                }
            }
                foreach (var o in dzialania)
                {
                    List<DzialaniaUzytkwonikowParametry> par = null;
                    if (parametry.TryGetValue(o.Id, out par))
                    {
                        o.Parametry = par.GroupBy(x => x.NazwaParametru).ToDictionary(x => x.Key, x => string.Join(", ", x.Select(y => y.Wartosc)));
                    }
                }
            return dzialania;
        }

        public void DodajParametry(IList<DzialaniaUzytkownikow> obj)
        {
            List<DzialaniaUzytkwonikowParametry>parametry = new List<DzialaniaUzytkwonikowParametry>();
            foreach (var dzialanie in obj)
            {
                if (dzialanie.Parametry != null && dzialanie.Parametry.Any())
                {
                    foreach (var parametr in dzialanie.Parametry)
                    {
                        parametry.Add(new DzialaniaUzytkwonikowParametry(parametr.Key,dzialanie.Id,parametr.Value));
                    }
                }
            }
            Calosc.DostepDane.AktualizujListe<DzialaniaUzytkwonikowParametry>(parametry);
        }

        /// <summary>
        /// Raz dziennie dodawanie zdarzenie dla klient. Jeżeli wystapi wywołanie metody dla tego samego zdarzenia głównego w tym samym dniu to nie zostanie dodane nowe DziałanieUzutkownika tylko dodany zostanie nowy parametr do istniejącego działnia użytkowika z konkretna wartością
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="wartosc"></param>
        /// <param name="zdarzenie"></param>
        /// <param name="zdarzenieGlowne"></param>
        public void LogujDzialanieUzytkownikowAsync(IKlient klient, string wartosc, ZdarzenieGrupa zdarzenie, ZdarzenieGlowne zdarzenieGlowne)
        {
            string zapytanie = $"EXEC sp_LogujDzialanieUzytkownika '{klient.Email}', '{zdarzenie}','{zdarzenieGlowne}','{DateTime.Now.Date:yyyy-MM-dd H:m}','{wartosc.Trim()}'";
            Calosc.DostepDane.DbORM.ExecuteSql(zapytanie);
        }
    }
}