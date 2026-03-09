using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using log4net;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{
    /// <summary>
    /// Klasa pomocnicza do zarządzania parametrami sesji
    /// </summary>
    public class SesjaHelper : ISesjaHelper
    {
          private static SesjaHelper _instancja = new SesjaHelper();
          public static SesjaHelper PobierzInstancje
          {
              get { return _instancja; }
          }

        public  virtual long KlientID
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Items["klientID"] != null)
                {
                    return (long)HttpContext.Current.Items["klientID"];
                }
                return 0;
            }
        }

        

      
        public  bool CzyKlientZalogowany
        {
            get
            {
                //nie WWW albo nie zalogowany
                return KlientID != 0;
            }
        }

        public string IpKlienta
        {
            get
            {
                if (HttpContext.Current == null) return "";
                string ipList = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipList))
                {
                    return ipList.Split(',')[0];
                }

                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }

    }
}
