using SolEx.Hurt.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly
{
    public class DodatkoweFunkcjeDokumentyBll : DodatkoweFunkcjeBaza
    {
        public override Type OblugiwanyTyp()
        {
            return typeof(DokumentyBll);
        }

        public override IList<DodatkowaFunkcja> PobierzDodatkoweFunkcjeDlaObiektu(object o)
        {
            DokumentyBll obiekt = (DokumentyBll)o;
            List<DodatkowaFunkcja> wynik = new List<DodatkowaFunkcja>();
            
            wynik.Add(new DodatkowaFunkcja { Adres = "DokumentZERP?id=" + obiekt.Id, Nazwa = "Podgląd" });
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

        public override bool? MoznaEdytowacObiekt(object o)
        {
            return null;
        }
        public override bool? MoznaUsuwacObiekt(object o)
        {
            return null;
        }
    }
}