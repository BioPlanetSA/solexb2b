using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers.API
{
    [RoutePrefix("api2/synchronizator")]
    [AutoryzacjaSolex]
    public class SynchronizatorController : SolexAPIController
    {
        [HttpPost]
        [Route("MailBladSynchronizacji")]
        public bool MailBladSynchronizacji(WiadomoscEmail wiadomosc)
        {
            Exception ex;
            SolexBllCalosc.PobierzInstancje.MaileBLL.WyslijEmaila(wiadomosc, null, TypyUstawieniaSkrzynek.Ogolne, out ex);
            return true;
        }

    }
}
