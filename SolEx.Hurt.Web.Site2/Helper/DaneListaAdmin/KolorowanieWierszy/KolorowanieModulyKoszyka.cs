using SolEx.Hurt.Helpers;
using System;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.KolorowanieWierszy
{
    /// <summary>
    /// Klasa bazowa dla klas decydujących o kolorze
    /// </summary>
    public class KolorowanieModulyKoszyka : KolorowanieBaza
    {
        public override Type OblugiwanyTyp()
        {
            return typeof(ModulKoszyka);
        }

        //public override string KolorWiersza(object o)
        //{          
        //        return null;
        //}

        //public override string KolorPola(object o, OpisPolaObiektu wartoscPola)
        //{
        //    return null;
        //}

        public override string KlasaCssWiersza(object o)
        {
            ModulKoszyka modul = (ModulKoszyka)o;

            if (modul.ModulWymagany && modul.Aktywne)
            {
                return "text-primary";
            }

            return null;
        }

        public override string KlasaCssPola(object o)
        {
            return null;
        }
    }
}