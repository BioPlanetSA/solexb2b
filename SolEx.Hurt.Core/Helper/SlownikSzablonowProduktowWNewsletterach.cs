using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikSzablonowProduktowWNewsletterach : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var c in PlikiDostep.PobierzInstancje.PobierzWidokiZKatalogu("/SzablonyMaili/Newsletter/ListaProduktow"))
                {
                    string sciezka = "~/Views/SzablonyMaili/Newsletter/ListaProduktow/" + c + ".cshtml";
                    wynik.Add( c,sciezka);
                }
                string nazwaSzablonu = SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowyNazwa;
                if (string.IsNullOrEmpty(nazwaSzablonu))
                {
                    return wynik;
                }

                var pliki = PlikiDostep.PobierzInstancje.PobierzWidokiNiestandardowe(string.Format("/templates/{0}/views/SzablonyMaili/Newsletter/ListaProduktow/", nazwaSzablonu));
                foreach (var c in pliki)
                {
                    string sciezka = string.Format("/templates/{0}/views/SzablonyMaili/Newsletter/ListaProduktow/", nazwaSzablonu) + c + ".cshtml";
                    if (wynik.ContainsKey(c))
                    {
                        SolexBllCalosc.PobierzInstancje.Log.ErrorFormat("Błąd duble nazwa szablonów newslettera dubel dla nazwy: {0}",c);
                        continue;
                    }
                    wynik.Add(c, sciezka);
                }
                
                return wynik;
            }
        }
    }
}