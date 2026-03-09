using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.PageBases
{
    public abstract class SolexAPIController: ApiController
    {
        public SolexHelper SolexHelper { get; set; }

        public SolexAPIController()
        {
            this.SolexHelper = SolexHelper.PobierzInstancjeZCache();
        }

        public int Paczka
        {
            get
            {
                int od;
                string odElemetnu = HttpContext.Current.Request.Headers["PageNumber"];
                if (int.TryParse(odElemetnu, out od))
                {
                    return od;
                }
                return 0;
            }
        }

        public int IloscElementow
        {
            get
            {
                int ilosc;
                string odElemetnu = HttpContext.Current.Request.Headers["PageSize"];
                if (int.TryParse(odElemetnu, out ilosc))
                {
                    return ilosc;
                }
                return Int32.MaxValue;
            }
        }

    }
}