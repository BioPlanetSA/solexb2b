using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using System;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.KolorowanieWierszy
{
    public class KolorowanieJezyki : KolorowanieBaza
    {
        public override Type OblugiwanyTyp()
        {
            return typeof(Jezyk);
        }

        //public override string KolorWiersza(object o)
        //{
        //    //Jezyk jezyk = (Jezyk)o;
        //    //if (jezyk.Domyslny)
        //    //{
        //    //    return "#90EE90";   //text-success
        //    //}
        //    return null;
        //}

        //public override string KolorPola(object o, OpisPolaObiektu wartoscPola)
        //{
        //    return null;
        //}

        public override string KlasaCssWiersza(object o)
        {
            Jezyk jezyk = (Jezyk)o;
            if (jezyk.Domyslny)
            {
                return "text-success";  
            }
            return null;
        }

        public override string KlasaCssPola(object o)
        {
            return null;
        }
    }
}