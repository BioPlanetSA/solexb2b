using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.AtrybutyMvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AutoryzacjaSolex : AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            SolexHelper solexHelper = SolexHelper.PobierzInstancjeZCache();
            
            var klient = solexHelper.AktualnyKlient;
            if (!klient.Role.Contains(RoleType.Administrator))
            {
                HttpResponseMessage m = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                SolexBllCalosc.PobierzInstancje.Log.Info($"Brak dostępu do akcji: {actionContext.ActionDescriptor.ActionName} w kontrolerze: {actionContext.ControllerContext.ControllerDescriptor.ControllerName}");
                m.Content = new StringContent("Brak dostępu");
                throw new HttpResponseException(m);
            }
        }
    }
}