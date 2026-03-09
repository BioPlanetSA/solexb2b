using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.PageBases;
using System.Web;
using ServiceStack.OrmLite;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Web.Site2.Modules;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Rejestracja")]
    public class RejestracjaController : SolexControler
    {
        [HttpGet]
        public PartialViewResult Rejestracja(string[] listapol, string odpowiedz, bool czypokazywaccaptche, string[] listapolwymaganych)
        {
            var lista = listapol.ToList();
            //lista.Add("WalidacjaNip");
            List<ParametryPola> pola = PobierzPola(lista.ToArray(), listapolwymaganych);
            return PartialView("_Rejestracja", new ParametryDoRejestracji(pola,odpowiedz,czypokazywaccaptche));
        }

        [HttpPost]
        public PartialViewResult Rejestracja(Rejestracja rejestracja, string odpowiedz, string[] listapol, bool czypokazywaccaptche)
        {
            Dictionary<string, ParametryPola> pola = PobierzPola(listapol).ToDictionary(x => x.Nazwa, x => x);
            var bledy = new List<string>();
            //do listy pół wpisujemy wartości wprowadzone do formularza 
            foreach (var p in pola)
            {
                var prop = rejestracja.GetType().GetProperty(p.Key);
                if (prop != null)
                {
                    var wartosc = prop.GetValue(rejestracja);
                    p.Value.Wartosc = wartosc;
                }
            }

            if (SprawdzPoprawnoscEmaila(rejestracja.Email, ref bledy))
            {
                //pobieramy kraj polska 
                Kraje kraj = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Kraje>(x => x.Symbol != "PL" || x.Nazwa == "Polska" || x.Nazwa == "Poland", null);
                if (czypokazywaccaptche && !SprawdzCaptcha())
                {
                    bledy.Add("Niepoprawna captcha");
                }
                else if (rejestracja != null && Model.Rejestracja.CzyRejestracjaJestPoprawna(rejestracja, kraj.Id, ref bledy))
                {
                    rejestracja.AdresIp = SesjaHelper.PobierzInstancje.IpKlienta;
                    rejestracja.DataRejestracji = DateTime.Now;
                    rejestracja.StatusEksportu = SolexBllCalosc.PobierzInstancje.Konfiguracja.AutomatyczneZatwierdzanieRejestracji ? RegisterExportStatus.Export : RegisterExportStatus.DontExport;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(rejestracja);

                    foreach (string item in Request.Files)
                    {
                        HttpPostedFileBase plik = Request.Files[item];
                        if (plik == null || plik.ContentLength == 0) continue;
                        string nazwa = Url.PobierzSciezkePlikUsera(typeof(Rejestracja), rejestracja.Id, plik.FileName, false);
                        new UploadPlikow().ZapiszPlik(plik, nazwa);
                        PropertyInfo prop = rejestracja.GetType().GetProperty(item);
                        string sciezka = Url.PobierzSciezkePlikUsera(typeof(Rejestracja), rejestracja.Id, plik.FileName, true);
                        prop.SetValue(rejestracja, sciezka);
                        //ustawienie sciezki dla maila
                        pola[item].SciezkaZalacznika = sciezka;
                    }
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(rejestracja);

                    //wysylamy jawnie zdarzenie - nie bindingi bo chcemy wyslac tylko te pola ktore uzytkownik widzial
                    SolexBllCalosc.PobierzInstancje.Statystyki.ZdarzenieNowaRejestracja(rejestracja, SolexHelper.AktualnyJezyk.Id, pola.Values.ToList());


                    if (string.IsNullOrEmpty(odpowiedz))
                    {
                        odpowiedz = "Dziękujemy za rejestracje";
                    }
                    return PartialView("_RejestracjaPodsumowanie", odpowiedz);
                }
            }

            
            return PartialView("_Rejestracja", new ParametryDoRejestracji(pola.Values.ToList(), odpowiedz, czypokazywaccaptche, bledy));
        }
        [NonAction]
        private bool SprawdzPoprawnoscEmaila(string email, ref List<string> bledy)
        {
            if (string.IsNullOrEmpty(email))
            {
                bledy.Add("Proszę wpisać email");
                return false;
            }
            long emailZBazy = Calosc.DostepDane.DbORM.Count<Model.Klient>(x=>x.Email==email.Trim());
            if (emailZBazy != 0)
            {
                bledy.Add("Wprowadzony adres e-mail jest już w systemie");
                return false;
            }
            return true;
        }

        [NonAction]
        public List<ParametryPola> PobierzPola(string[] polaRejestracji, string[] listaPolWymaganych=null)
        {
            List<ParametryPola> pola = Refleksja.WygenerujParametryPol<Rejestracja>(listaPolWymaganych);
            List<ParametryPola> wynik = new List<ParametryPola>();
            foreach (var p in pola)
            {
                //grupy zostawiamy
                if (polaRejestracji.Contains(p.Nazwa))
                {
                    wynik.Add(p);
                }
            }
            wynik = Refleksja.DodajPolaZGrupami(wynik);
            return wynik;
        }
    }
}