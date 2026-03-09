using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Modules;
using SolEx.Hurt.Web.Site2.PageBases;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Klienci")]
    public class KlienciController : SolexControler
    {
        [Route("Rabaty")]
        public PartialViewResult Rabaty(bool pokazujRabatNaProdukty = true, bool pokazujRabatNaKategorie = true, bool pokazujRabatOgolnyKlienta = true)
        {
            if (!pokazujRabatNaProdukty && !pokazujRabatNaKategorie && (!pokazujRabatOgolnyKlienta || SolexHelper.AktualnyKlient.Rabat == 0))
            {
                Log.WarnFormat("Kontrolka Rabaty nie może nic pokazać - brak podanych parametrów");
                return null;
            }

            HashSet<RabatBLL> rabatyProduktow = null;
            HashSet<RabatBLL> rabatyKategorii = null;

            if (pokazujRabatNaProdukty || pokazujRabatNaKategorie)
            {
                //TODO: wwyala sie bo kategorai moze byc pusta - trzba to zmienic
                var items =  SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<RabatBLL>(null,x => x.Aktywny && (x.OdKiedy == null ||  x.OdKiedy <= DateTime.Now) 
                        && (x.DoKiedy ==null || x.DoKiedy >= DateTime.Now ) && 
                        (x.KlientId == SolexHelper.AktualnyKlient.Id || Sql.In(x.KategoriaKlientowId, SolexHelper.AktualnyKlient.Kategorie) || (x.KlientId==null && x.KategoriaKlientowId==null))).ToList();
                List<RabatBLL> pojedyncze = new List<RabatBLL>();
                foreach (var rabatBll in items)
                {
                        if (!pojedyncze.Any(
                                x => x.ProduktId == rabatBll.ProduktId && x.KategoriaProduktowId == rabatBll.KategoriaProduktowId))
                        {
                            pojedyncze.Add(rabatBll);
                        }
                }

                HashSet<long> prod = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(SolexHelper.AktualnyKlient);
                if (pokazujRabatNaKategorie)
                {
                    //pobranei wprost wszystkich kategorii - poza ORM bo nie mozemy walidatora uzywac bo czesc kategorii moze byc niewidoczna dla klienta normalnie
                   // rabatyKategorii = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.Select<KategorieBLL>(x=> )
                    rabatyKategorii = new HashSet<RabatBLL>( pojedyncze.Where(x => x.KategoriaProduktowId.HasValue) );
                }

            
                if (pokazujRabatNaProdukty)
                {
                    rabatyProduktow = new HashSet<RabatBLL>(pojedyncze.Where(x => x.ProduktId.HasValue && prod.Contains(x.ProduktId.Value)));
                }
               
            }

            Tuple<HashSet<RabatBLL>, HashSet<RabatBLL>, bool> listaRabatow = new Tuple<HashSet<RabatBLL>, HashSet<RabatBLL>, bool>(rabatyKategorii, rabatyProduktow, pokazujRabatOgolnyKlienta);

            return PartialView("_Rabaty", listaRabatow);
        }


        // http://localhost/klienci/zmianaip/685385b67e8928959dd20d3d282e5879
        [Route("ZmianaIp/{hash}")]
        public void ZmianaIp(String hash)
        {
            bool wynik = SolexBllCalosc.PobierzInstancje.Klienci.ResetGid(hash);
            if (!wynik)
            {
                SolexHelper.Wyloguj();
            }
            Response.Redirect(Url.LinkLogowania());
        }

        [HttpGet]
        public PartialViewResult FormularzZapytania(bool pokazTytul = true, bool pokazujAdresEmailNaJakiOdeslacOdpowiedz = false, string mailDoOdpowiedzi="", string tytul = " ",
            string tresc = " ", string mailDoWysylki = "", bool doOpiekuna = false, bool doPrzedstawiciela = false, bool doDrugiegoOpiekuna = false,
            string pola = "")
        {           
            PlikiDostep pd = new PlikiDostep();
            List<string> dostepneTypy = pd.PobierzWidokiZKatalogu("/Shared/EditorTemplates/Pola");
            List<ParametryPola> listaPol = new List<ParametryPola>();

            if (dostepneTypy.Count > 0 && !String.IsNullOrEmpty(pola)) 
            {
                try
                {
                    var plik = pola.Split(';');
                    foreach (var x in plik)
                    {
                        var concat = x.Split('|'); 
                        string nazwa = concat[0]; 
                        string typ = concat[1];

                        var typZnaleziony = dostepneTypy.FirstOrDefault(y => typ.Equals(y));
                        if (typZnaleziony == null)
                        {
                            throw new InvalidOperationException("Próba stworzenia w formularzy zapytania pola o typie którego nie ma zdefiniowanego. Ten typ to: "+typ);
                        }
                        var p = new ParametryPola { Nazwa = nazwa, Typ = typ, Wartosc = "", WyswietlanaNazwa = nazwa };
                        listaPol.Add(p);
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Błąd w parametrach");
                } 
            }
            ViewBag.Blad = false;
            var model = new FormularzZapytanieModel(pokazTytul, pokazujAdresEmailNaJakiOdeslacOdpowiedz,mailDoOdpowiedzi, tytul, tresc, mailDoWysylki, doOpiekuna, doPrzedstawiciela, doDrugiegoOpiekuna, listaPol);
            model.TytulWartosc = "";
            model.TrescWartosc = "";
            return PartialView("_FormularzZapytania", model);
        }

        [HttpPost]
        public PartialViewResult FormularzZapytania(FormularzZapytanieModel model, List<ParametryPola> dpola)
        {
            if (!SprawdzCaptcha())
            {
                throw new Exception("Niepoprawna captcha w kontakcie");
            }

            if (!model.PokazujAdresEmailNaJakiOdeslacOdpowiedz)
            {
                if (SolexHelper.AktualnyKlient.Id != 0)
                {
                    model.MailDoOdpowiedzi = SolexHelper.AktualnyKlient.Email;
                }
            }
            
            model.DPola = dpola;
            if (dpola != null)
            {
                foreach (var item in dpola)
                {
                    //Potrzebne jest to bo mimo że w poscie dla ParametruPolap
                    if (item.Typ.Equals(typeof(string).Name,StringComparison.InvariantCultureIgnoreCase)  && (item.Wartosc as IEnumerable) != null)
                    {
                        item.Wartosc = String.Join(" ", (string[])item.Wartosc);
                    }
                    if (item.Typ == "Plik" && item.Plik != null)
                    { 
                        string sciezka = Url.PobierzSciezkePlikUsera(typeof(Formularz), DateTime.Now.ToString("yyyyMMddHHmmssf") + "_"+model.MailDoWysylki, item.Plik.FileName, false);
                        item.Nazwa = item.Plik.FileName;
                        new UploadPlikow().ZapiszPlik(item.Plik, sciezka);
                        item.SciezkaZalacznika = Url.PobierzSciezkePlikUsera(typeof(Formularz), DateTime.Now.ToString("yyyyMMddHHmmssf") + "_" + model.MailDoWysylki, item.Plik.FileName, true); ;
                    }
                }
            }

            ViewBag.Blad = true;
            bool wynik = false;
            try
            {
                SolexBllCalosc.PobierzInstancje.Statystyki.ZdarzenieNowyFormularz(model, SolexHelper.AktualnyKlient);
                wynik = true;
                
            }
            catch (Exception e)
            {
                Log.Error("Błąd wysyłania formularza");
                Log.Error(e);
            }

            string komunikat = wynik
                ?  "Wiadomość została wysłana"
                : "Niestety mail nie został wysłany spróbuj ponownie";
            ViewBag.Komunikat = komunikat;
            ViewBag.Rezultat = wynik;
           
            return PartialView("_FormularzZapytania", model);
        }
        [Route("PoleKlienta")]
        public ActionResult PoleKlienta(string wartoscDoPokazania, string przedrostek, string przyrostek, string frazaDoPokazania)
        {
            KontrolkaPokazywaniaPrzedrostekModel model = new KontrolkaPokazywaniaPrzedrostekModel(new List<string>() {""},przedrostek, przyrostek, frazaDoPokazania);
            IKlient klient = SolexHelper.AktualnyKlient;

            string doPokazania = "";
            if (!string.IsNullOrEmpty(wartoscDoPokazania))
            {
                object val = klient.GetType().GetProperty(wartoscDoPokazania).GetValue(klient, null);

                if (val != null)
                {
                    doPokazania = val.ToString();
                }
            }

            model.WartoscDoPokazania = new List<string>() { doPokazania};
            return PartialView("_PoleKlienta", model);
        }

        [Route("KategoriaKlienta")]
        public ActionResult KategoriaKlienta(string grupaKategoriiDoPokazania, string przedrostek, string przyrostek, string frazaDoPokazania)
        {
            IList<KategoriaKlienta> kategoria = new List<KategoriaKlienta>();
            if (SolexHelper.AktualnyKlient.Kategorie.Any())
            {
               kategoria =  SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(SolexHelper.AktualnyKlient, x=> Sql.In(x.Id, SolexHelper.AktualnyKlient.Kategorie) && x.Grupa == grupaKategoriiDoPokazania);
            }
            KontrolkaPokazywaniaPrzedrostekModel model = new KontrolkaPokazywaniaPrzedrostekModel(kategoria.Select(x=>x.Nazwa).ToList(), przedrostek, przyrostek, "");
            return PartialView("_KategoriaKlienta", model);
        }

        [Route("Pracownik")]
        public ActionResult Pracownik(int id)
        {
            Models.KontrolkiTresci.Pracownik kontrolka = this.PobierzKontrolke<Models.KontrolkiTresci.Pracownik>(id);
            var aktualnyKlient = SolexHelper.AktualnyKlient;
            IKlient klient = null;

            if (kontrolka.WybranyReczniePracownikID.HasValue)
            {
                Core.Klient obketKlient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Core.Klient>(kontrolka.WybranyReczniePracownikID.Value, null);
                IKlient klientInterfejs = new Core.Klient(obketKlient);
                klient = klientInterfejs;
            }
            else
            {
                if (kontrolka.TypPracownika == TypPracownika.Opiekun)
                {
                    klient = aktualnyKlient.Opiekun;
                }
                else if (kontrolka.TypPracownika == TypPracownika.DrugiOpiekun)
                {
                    klient = aktualnyKlient.DrugiOpiekun;
                }
                else if (kontrolka.TypPracownika == TypPracownika.Przedstawiciel)
                {
                    klient = aktualnyKlient.Przedstawiciel;
                }
            }

            if (klient==null || klient.Id == 0)
            {
                return null;
            }

            if (kontrolka.FormaSkrocona)
            {
                return PartialView("_PracownikWersjaSkrocona", new ParametryDoPracownika(klient, kontrolka.DodatkowyTelefon)); 
            }
            return PartialView("_PracownikWersjaPelna",
                new ParametryDoPracownika(klient, kontrolka.DodatkowyTelefon, kontrolka.PolaDodatkowe, kontrolka.RozmiarZdjecia, kontrolka.OpiekunKlientaTekstNadZdjeciem));
        }
    }
}