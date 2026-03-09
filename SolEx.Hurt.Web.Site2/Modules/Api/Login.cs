using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Interfaces.Modele;
using SolEx.Hurt.Web.Site2.Helper;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Zwraca identyfikator sesji, przyjmuje nazwę użytkownika i md5 hasła
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Logowanie, "Logowanie do API", "Umożliwia zalogowanie do api", "API/LoginHandler.ashx")]
    public class LoginHandler : ApiSessionBaseHandler, IDocumentApiVisible
    {
        public override bool WymagajAktywnejSesji
        {
            get { return false; }
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(KeyValuePair<string, string>); }
        }
        protected override object Handle()
        {
            Thread.Sleep(15000);
            IKlienci k = null;

            if (Data != null)
            {
                KeyValuePair<string, string> data = (KeyValuePair<string, string>)Data;
                k = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null,x=>x.Email==data.Key && x.HasloKlienta==data.Value).FirstOrDefault();

            }
            else if (HttpContext.Current.Request["hash"] != null)//a może ktoś podał hash wyliczony
            {
                string hash = HttpContext.Current.Request["hash"];
            }
            string msg = "";
            StatusApi sa = StatusApi.BladLogowania;
            if (k != null)
            {
                msg = k.KluczSesji;
                sa = StatusApi.Ok;
            }
            Status s = Status.Utworz(sa, msg);
            s.Data = msg;
            return s;
        }
    }
}
