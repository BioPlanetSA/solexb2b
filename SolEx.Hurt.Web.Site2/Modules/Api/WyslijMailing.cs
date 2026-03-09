using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    public class WyslijMailing : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            MailHelper.PobierzInstancje.WyslijAktywneMailingi();
            return Status.Utworz(StatusApi.Ok);

        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<IKlient>); }
        }


    }
    public class WyslijPonownieMaile : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.MaileBLL.WyslijBledneMaile();
            return Status.Utworz(StatusApi.Ok);

        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<IKlient>); }
        }


    }
}