using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public class KlienciHelper : BllBaza<KlienciHelper>
    {
        public Core.Klient PobierzKlientaDoZalogowania(Models.Logowanie logowanie, bool logowaniepokazujcaptcha = false, string sesjaId = null)
        {
            if ((logowanie == null || string.IsNullOrEmpty(logowanie.Haslo) || string.IsNullOrEmpty(logowanie.Uzytkownik)) && string.IsNullOrEmpty(sesjaId))
            {
                throw new Exception("Bląd logowania - brak danych");
            }

            Core.Klient klientDoZalogowania = null;
            string ipKlienta = SesjaHelper.PobierzInstancje.IpKlienta;
            if (string.IsNullOrEmpty(sesjaId))
            {
                logowanie.Uzytkownik = logowanie.Uzytkownik.Trim();
                logowanie.Haslo = logowanie.Haslo.Trim();
                SolexBllCalosc.PobierzInstancje.Klienci.CzyMoznaZalogowacKlienta(logowanie.Uzytkownik, logowanie.Haslo, ipKlienta, out klientDoZalogowania);

            }
            else
            {
                klientDoZalogowania = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Core.Klient>(x => x.KluczSesji == sesjaId, null);
                SolexBllCalosc.PobierzInstancje.Klienci.CzyKlientMozeSieZalogowac(klientDoZalogowania);
            }

            if (klientDoZalogowania == null)
            {
                throw new Exception("Bląd logowania - niepoprawne dane");
            }
            return klientDoZalogowania;
        }

    }
}