using SolEx.Hurt.Helpers;
using System;
using SolEx.Hurt.Model.Interfaces;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.KolorowanieWierszy
{
    /// <summary>
    /// Klasa bazowa dla klas decydujących o kolorze
    /// </summary>
    public class KolorowanieKlient : KolorowanieBaza
    {
        public override Type OblugiwanyTyp()
        {
            return typeof(Klient);
        }

        //public override string KolorWiersza(object o)
        //{
        //    //IKlient klient = (IKlient)o;

        //    //if (!klient.Aktywny)
        //    //{
        //    //    return "salmon";
        //    //}

        //    ////if (klient.DataOstatniegoLogowania == null || klient.DataOstatniegoLogowania.Value < DateTime.Now.AddMonths(-1))
        //    ////{
        //    ////    return "salmon";
        //    ////}
        //    return null;
        //}

        //public override string KolorPola(object o, OpisPolaObiektu wartoscPola)
        //{
        //    return null;
        //}

        public override string KlasaCssWiersza(object o)
        {
            IKlient klient = (IKlient)o;
            if (!klient.Aktywny)
            {
                return "nieaktywna-linia";
            }
            return null;
        }

        public override string KlasaCssPola(object o)
        {
            return null;
        }
    }
}