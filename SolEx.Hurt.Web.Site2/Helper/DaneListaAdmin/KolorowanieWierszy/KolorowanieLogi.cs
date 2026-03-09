using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using System;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.KolorowanieWierszy
{
    public class KolorowanieLogi : KolorowanieBaza
    {
        public override Type OblugiwanyTyp()
        {
            return typeof(LogWpis);
        }

        //public override string KolorWiersza(object o)
        //{
        //    //LogWpis wpis = (LogWpis)o;
        //    //if (wpis.Poziom.ToLower() == "error" || wpis.Poziom.ToLower() == "fatal")
        //    //{
        //    //    return "#FFE4E1";
        //    //}
        //    return null;
        //}

        //public override string KolorPola(object o, OpisPolaObiektu wartoscPola)
        //{
        //    return null;
        //}

        public override string KlasaCssWiersza(object o)
        {
            LogWpis wpis = (LogWpis)o;
            if (wpis.Poziom.ToLower() == "error" || wpis.Poziom.ToLower() == "fatal")
            {
                return "text-danger";
            }
            return null;
        }

        public override string KlasaCssPola(object o)
        {
          return null;
        }
    }
}