using SolEx.Hurt.Core.ModelBLL;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly
{
    public class DodatkoweFunkcjePracownik : DodatkoweFunkcjeBaza
    {
        public override IList<DodatkowaFunkcja> PobierzDodatkoweFunkcjeDlaObiektu(object o)
        {
            List<DodatkowaFunkcja> wynik = new List<DodatkowaFunkcja>();
            //wynik.Add(new DodatkowaFunkcja {Adres = "Admin/ZalogujJakoKlient?id=" + obiekt.klient_id, Nazwa = "Zaloguj jako klient"});
            //wynik.Add(new DodatkowaFunkcja {Adres = "Admin/RabatyEfektywne?id=" + obiekt.klient_id, Nazwa = "Pokaż rabaty efektywne"});
            return wynik;
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

        public override Type OblugiwanyTyp()
        {
            return typeof(Pracownik);
        }
        public override List<Komunikat> KomunitatyNaLiscieObiektu(Type o)
        {
            return null;
        }
    }
}