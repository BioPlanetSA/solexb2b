using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using System;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.KolorowanieWierszy
{
    public class KolorowanieNewsletter : KolorowanieBaza
    {
        public override Type OblugiwanyTyp()
        {
            return typeof(NewsletterKampania);
        }

        //public override string KolorWiersza(object o)
        //{
        //    return null;
        //}

        //public override string KolorPola(object o, OpisPolaObiektu wartoscPola)
        //{
        //    return null;
        //}

        public override string KlasaCssWiersza(object o)
        {
            NewsletterKampania news = o as NewsletterKampania;
            if (news.Status == StatusNewsletter.Zakończony)
            {
                return "text-muted";
            }

            if (news.Status == StatusNewsletter.Wysyłany || news.Status == StatusNewsletter.ZaplanowanyDoWysłania)
            {
                return "text-warning";
            }

            return null;
        }

        public override string KlasaCssPola(object o)
        {
            return null;
        }
    }
}