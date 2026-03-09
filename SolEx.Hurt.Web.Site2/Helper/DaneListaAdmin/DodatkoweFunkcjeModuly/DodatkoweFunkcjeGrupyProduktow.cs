using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly
{
    public class DodatkoweFunkcjeGrupyProduktow : DodatkoweFunkcjeBaza
    {
        public override Type OblugiwanyTyp()
        {
            return typeof(Grupa);
        }

        public override IList<DodatkowaFunkcja> PobierzDodatkoweFunkcjeDlaObiektu(object o)
        {
            Grupa obiekt = (Grupa) o;
            List<DodatkowaFunkcja> wynik = new List<DodatkowaFunkcja>();
            int jezyk = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
            string adres = string.Format("ModelujDrzewko?grupaid={0}&jezyk={1}", obiekt.Id, jezyk);
            wynik.Add(new DodatkowaFunkcja {Adres = adres, Nazwa = "Modeluj drzewko"});
            return wynik;
        }
        public override List<Komunikat> KomunitatyNaLiscieObiektu(Type o)
        {
            return null;
        }
        public override List<Komunikat> KomunitatyNaEdycjiObiektu(object o)
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
    }
}