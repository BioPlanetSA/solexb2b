using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Providers
{
    public class Autoryzacja : AuthorizeAttribute
    {
        /// <summary>
        /// Służy do autoryzacji akcji np. dajemy [Autoryzacja(RoleType.Administrator)]
        /// </summary>
        /// <param name="allowedRoles"></param>
        public Autoryzacja(params RoleType[] allowedRoles)
        {
            var allowedRolesAsStrings = allowedRoles.Select(x => Enum.GetName(typeof(RoleType), x));
            Roles = string.Join(",", allowedRolesAsStrings);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext != null)
            {
                //solexhelper
                var solexHelper = SolexHelper.PobierzInstancjeZCache();
                IKlient klient = solexHelper.AktualnyKlient;
                foreach (var role in klient.Role)
                {
                    //jeżeli rola atrybutu znajduje się w roli klienta
                    if (Roles.Contains(role.ToString()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}