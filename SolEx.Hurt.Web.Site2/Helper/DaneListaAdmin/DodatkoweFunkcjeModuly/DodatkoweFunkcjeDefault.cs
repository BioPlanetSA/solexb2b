using SolEx.Hurt.Core;
using System;
using System.Collections.Generic;
using System.Web.Routing;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly
{
    /// <summary>
    /// Dodatkowe funkcje które mają być dostępnie dla wszystkich
    /// </summary>
    public class DodatkoweFunkcjeDefault : DodatkoweFunkcjeBaza
    {
        public override IList<DodatkowaFunkcja> PobierzDodatkoweFunkcjeDlaObiektu(object obj)
        {
            DaneObiekt obiekt = (DaneObiekt) obj;
            List<DodatkowaFunkcja> wynik = new List<DodatkowaFunkcja>();
            Type tmp = obiekt.GetType();
            RouteValueDictionary routerValue = new RouteValueDictionary(new {typ = obiekt.TypObiektu.PobierzOpisTypu(), id = obiekt.Klucz});
            if (obiekt.MoznaEdytowac)
            {
                wynik.Add(new DodatkowaFunkcja
                {
                    Adres = Url.Action("Edycja", "Admin", routerValue),
                    Nazwa = "Edycja",
                    KlasaCssPrzycisku = "btn-warning btn-sm btn"
                });
            }

            if (obiekt.MoznaUsuwac)
            {
                wynik.Add(new DodatkowaFunkcja
                {
                    Adres = Url.Action("Usuwanie", "Admin", routerValue),
                    Nazwa = "Usuń",
                    KomunikatJavascriptCzyNapewno = new Komunikat("Czy napewno chcesz usunąć obiekt?", KomunikatRodzaj.info),
                    KlasaCssPrzycisku = "btn-warning btn-sm btn"
                });
            }

            return wynik;
        }

        public override List<Komunikat> KomunitatyNaEdycjiObiektu(object o)
        {
           return null;
        }
        public override List<Komunikat> KomunitatyNaLiscieObiektu(Type o)
        {
            return null;
        }
        public override bool? MoznaUsuwacObiekt(object o)
        {
            return null;
        }

        public override bool? MoznaEdytowacObiekt(object o)
        {
            return null;
        }

        public override Type OblugiwanyTyp()
        {
            return typeof(DaneObiekt);
        }
    }
}