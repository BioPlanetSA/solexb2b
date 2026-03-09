using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using System;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.KolorowanieWierszy
{
    public class KolorowanieHarmonogram : KolorowanieBaza
    {
        public override Type OblugiwanyTyp()
        {
            return typeof(HarmonogramBll);
        }

        //public override string KolorWiersza(object o)
        //{
        //    //HarmonogramBll harmonogram = (HarmonogramBll)o;
        //    //if (harmonogram.CzyPowinnoBycUruchomioneTeraz)
        //    //{
        //    //    return "#FA8072";
        //    //}
        //    //else
        //    //{
        //    //    return "#90EE90";
        //    //}
        //    return null;
        //}

        //public override string KolorPola(object o, OpisPolaObiektu wartoscPola)
        //{
        //    return null;
        //}

        public override string KlasaCssWiersza(object o)
        {
            HarmonogramBll harmonogram = (HarmonogramBll) o;
            if (harmonogram.CzyPowinnoBycUruchomioneTeraz)
            {
                return "bg-warning";
            }
            return null;
        }

        public override string KlasaCssPola(object o)
        {
            return null;
        }
    }
}