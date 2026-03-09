using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly
{
    public class DodatkoweFunkcjeModulKoszyka : DodatkoweFunkcjeBaza
    {
        public override IList<DodatkowaFunkcja> PobierzDodatkoweFunkcjeDlaObiektu(object o)
        {
            return null;
        }

        public override List<Komunikat> KomunitatyNaEdycjiObiektu(object o)
        {
            return null;
        }

        public override bool? MoznaUsuwacObiekt(object o)
        {
            ModulKoszyka modul = (ModulKoszyka) o;
            if (modul.ModulWymagany)
            {
                return false;
            }
            return null;
        }

        public override Type OblugiwanyTyp()
        {
            return typeof(ModulKoszyka);
        }
        public override List<Komunikat> KomunitatyNaLiscieObiektu(Type o)
        {
            return null;
        }
        public override bool? MoznaEdytowacObiekt(object o)
        {
            return null;
        }

        public override string PobierzNazweObiektu(object o)
        {
            ModulKoszyka modulKoszyka = (ModulKoszyka) o;
           return modulKoszyka.NazwaZadanie;
        }

        public override string PobierzOpisObiektu(object o)
        {
            ModulKoszyka modulKoszyka = (ModulKoszyka) o;
            return modulKoszyka.OpisZadanie;
            
        }
    }
}