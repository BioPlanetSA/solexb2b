using System.Collections.Generic;
using System.Linq;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using Rejestracja = SolEx.Hurt.Model.Rejestracja;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public class WyslijPowiadomienia : BllBaza<WyslijPowiadomienia>
    {
        public void WyslijPowiadomienieNowyKlient(IKlient nowyklient)
        {
            NowyKlient obiekt = new NowyKlient(nowyklient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieNowaRejestracja(Rejestracja rejestracja, IKlient klient, List<ParametryPola> pola)
        {
            Core.ModelBLL.ObiektyMaili.Rejestracja obiekt = new Core.ModelBLL.ObiektyMaili.Rejestracja(rejestracja, klient, pola);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomieniePobranieFaktury(DokumentyBll dokument, IKlient klient)
        {
            PobranieFaktury obiekt = new PobranieFaktury(dokument, klient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijMailaResetHaslaKlienta(IKlient klient)
        {
            ResetHasla obiekt = new ResetHasla(klient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WysylanieFormularza(FormularzZapytanieModel model, IKlient klient)
        {
            Formularz obiekt = new Formularz(model, klient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieOZmienieKluczaApi(IKlient klient)
        {
            GenerowanieKluczaApi obiekt = new GenerowanieKluczaApi(klient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieNoweProduktyWSystemie(IList<ProduktKlienta> listaProduktow, IKlient klient)
        {
            NoweProduktyWSystemie obiekt = new NoweProduktyWSystemie(listaProduktow, klient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieNoweZamowienie(ZamowieniaBLL zamowienie, IKlient klient, string[] sciezkadozalacznika = null)
        {
            if (!zamowienie.PracownikSkladajacyId.HasValue || SolexBllCalosc.PobierzInstancje.Konfiguracja.ZamowienieWImieniuKlientaWysylajMaile)
            {
                NoweZamowienie obiekt = new NoweZamowienie(zamowienie, klient);
                obiekt.DodajZalaczniki(sciezkadozalacznika);
                MailHelper.PobierzInstancje.WyslijMaile(obiekt);
            }
        }

        public void WyslijPowiadomienieBladImportu(ZamowieniaBLL zamowienie, IKlient klient, string[] sciezkadozalacznika = null)
        {
            BladImportu obiekt = new BladImportu(zamowienie, klient);
            obiekt.DodajZalaczniki(sciezkadozalacznika);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieSubkonta_ZamowienieOdrzucone(IKoszykiBLL koszyk, IKlient klient, IKlient odrzucil)
        {
            ZamowienieOdrzucone obiekt = new ZamowienieOdrzucone(koszyk, klient, odrzucil);
            obiekt.KlientKtoryOdrzucil = odrzucil;
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieSubkonta_ZamowienieZaakceptowane(ZamowieniaBLL zamowienie, IKlient klient, IKlient akceptowal, string[] sciezkadozalacznika = null)
        {
            ZamowienieZaakceptowane obiekt = new ZamowienieZaakceptowane(zamowienie, klient);
            obiekt.DodajZalaczniki(sciezkadozalacznika);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieSubkonta_ZamowienieDoAkceptacji(IKoszykiBLL koszyk, IKlient klient)
        {
            ZamowienieDoAkceptacji obiekt = new ZamowienieDoAkceptacji(koszyk, klient);
            var klienci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Core.Klient>(null, x => Sql.In(x.Id, koszyk.KlienciMogacyAkceptowacKoszyk)).Select(x => x as IKlient).ToList();
            obiekt.KlienciMogacyAkceptowac = klienci;
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }


        public void WyslijProsbeOInformacjeODostepnosci(IProduktKlienta produkt, IKlient klient)
        {
            ProsbaOInformacjeODostepnosci obiekt = new ProsbaOInformacjeODostepnosci(produkt, klient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieNoweDokumenty(List<DokumentyBll> dok, IKlient klient)
        {
            NowyDokument obiekt = new NowyDokument(dok);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomieniePojawienieSieProduktow(IList<IProduktKlienta> listaProduktow, IKlient klient)
        {
            PojawienieSieProduktow obiekt = new PojawienieSieProduktow(listaProduktow, klient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieOZapisieDoNewslettera(NewsletterZapisani zapisany)
        {
            ZapisDoNewslettera obiekt = new ZapisDoNewslettera(zapisany);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieOZmianieTerminuRealizacjiZamowienia(DokumentyBll dokument)
        {
            ZmianaTerminuRealizacjiZamowienia obiekt = new ZmianaTerminuRealizacjiZamowienia(dokument);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieZmianieStatusuDokumentu(DokumentyBll dokument)
        {
            ZmianaStatusuDokumentu obiekt = new ZmianaStatusuDokumentu(dokument);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomieniePowitanieSzef(IKlient klient)
        {
            PowitanieSzef obiekt = new PowitanieSzef(klient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieZmianaAdresuIP(IKlient klient, string noweIP, string stareIP)
        {
            ZmianaAdresuIP obiekt = new ZmianaAdresuIP(klient, noweIP, stareIP);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieOListachPrzewozowych(DokumentyBll dokument, IList<HistoriaDokumentuListPrzewozowy> listy)
        {
            ListyPrzewozowe obiekt = new ListyPrzewozowe(dokument, listy);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomieniePrzeterminowanePlatnosc(IEnumerable<DokumentyBll> dokumenty, IKlient klient)
        {
            PrzeterminowanePlatnosci obiekt = new PrzeterminowanePlatnosci(dokumenty, klient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }

        public void WyslijPowiadomienieProduktyPrzyjeteNaMagazyn(IList<ProduktPrzyjetyNaMagazyn> produkty, IKlient klient)
        {
            ProduktyPrzyjeteNaMagazyn obiekt = new ProduktyPrzyjeteNaMagazyn(produkty, klient);
            MailHelper.PobierzInstancje.WyslijMaile(obiekt);
        }
    }
}