using System;
using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly
{
    public class DodatkoweFunkcjeSzablonowEdytorow : DodatkoweFunkcjeBaza
    {
        public override IList<DodatkowaFunkcja> PobierzDodatkoweFunkcjeDlaObiektu(object o)
        {
            return null;
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
            SzablonyEdytorow szablon = o as SzablonyEdytorow;
            if (szablon.Id > 0)
            {
                return false;
            }
            return null;
        }

        public override bool? MoznaEdytowacObiekt(object o)
        {
            SzablonyEdytorow szablon = o as SzablonyEdytorow;
            if (szablon.Id > 0)
            {
                return false;
            }

            return null;
        }

        public override Type OblugiwanyTyp()
        {
            return typeof(SzablonyEdytorow);
        }
    }
}