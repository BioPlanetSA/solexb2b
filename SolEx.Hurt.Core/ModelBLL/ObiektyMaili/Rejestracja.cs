using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class Rejestracja : SzablonMailaBaza
    {
        public Rejestracja() : base(null) { }

        public Rejestracja(Model.Rejestracja rejestracja, IKlient klient, List<ParametryPola> pola): base(klient)
        {
            Rejestracje = rejestracja;
            ListaPol = pola;
        }

        public override string NazwaFormatu()
        {
            return "Rejestracja nowego klienta";
        }

        public Model.Rejestracja Rejestracje { get; set; }

        public List<ParametryPola> ListaPol { get; set; }

        public override string OpisFormatu()
        {
            return "Mail o nowej rejestracji - wysyłany po wypełnieniu formularza rejestracyjnego";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail o nowej rejestracji";
        }
        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] {TypyPowiadomienia.Klient}; }
        }

        public Dictionary<string, List<ParametryPola>> Pola()
        {
            Dictionary<string, List<ParametryPola>> wynik = new Dictionary<string, List<ParametryPola>>();
            if(ListaPol == null || !ListaPol.Any()) return null;
            foreach (ParametryPola pole in ListaPol)
            {
                if (pole != null)
                {

                    if (pole.Wartosc.RownyWartosciDomyslnej() || pole.Nazwa == "AkceptacjaRegulaminu" || pole.Nazwa == "Captcha" || pole.Nazwa == "PrzetwarzanieDanychOsobowych")
                    {
                        continue;
                    }
                    string grupa = string.IsNullOrEmpty(pole.Grupa) ? "" : pole.Grupa;
                    if (!wynik.ContainsKey(grupa))
                    {
                        wynik.Add(grupa, new List<ParametryPola>());
                    }
                    if (pole.Typ=="Plik")
                    {
                        wynik[pole.Grupa].Add(new ParametryPola(pole.Nazwa, pole.WyswietlanaNazwa, pole.Typ, pole.Wymagane, pole.Grupa, sciezka:pole.SciezkaZalacznika));
                        continue;
                    }
                   
                   
                    wynik[grupa].Add(pole);
                }
            }
            return wynik;
        }

        public string LinkDoZalacznikaRejestracji(string zalacznik)
        {
            if (!string.IsNullOrEmpty(zalacznik))
            {
                string link = "";//string.Format("{0}{1}", SolexBllCalosc.PobierzInstancje.Konfiguracja.KatalogPliki, zalacznik.Replace("\\", "/"));
                return string.Format("<a download href=\"{0}\">Pobierz</a>", link);
            }
            return "";
        }
        
    }
}
