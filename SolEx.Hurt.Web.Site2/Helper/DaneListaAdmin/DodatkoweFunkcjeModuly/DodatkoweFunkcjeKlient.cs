using SolEx.Hurt.Core;
using System;
using System.Collections.Generic;
using System.Web.Routing;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly
{
    public class DodatkoweFunkcjeKlient : DodatkoweFunkcjeBaza
    {
        public override IList<DodatkowaFunkcja> PobierzDodatkoweFunkcjeDlaObiektu(object o)
        {
            IKlient obiekt = (IKlient)o;
            List<DodatkowaFunkcja> wynik = new List<DodatkowaFunkcja>();
            if (obiekt.Aktywny)
            {
                wynik.Add(new DodatkowaFunkcja
                {
                    Adres = Url.Action("WyslijMaile", "Admin", new RouteValueDictionary { { "idKlienta", obiekt.Id } }),
                    Nazwa = "Wyślij mail powitalny",
                    KomunikatJavascriptCzyNapewno = new Komunikat("Czy napewno chcesz wysłać wiadomość powitalną do klienta?", KomunikatRodzaj.info),
                    KlasaCssPrzycisku = "btn-warning btn-sm btn"
                });

                wynik.Add(new DodatkowaFunkcja { Adres = Url.Action("ZalogujJakoKlient", "Admin", new RouteValueDictionary { { "id", obiekt.Id }} ) , Nazwa = "Zaloguj jako klient"});
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
            return false;
        }

        public override bool? MoznaEdytowacObiekt(object o)
        {
            return null;
        }

        public override Type OblugiwanyTyp()
        {
            return typeof(Klient);
        }
    }
}