using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using log4net;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.SolexPay.Model;

namespace SolEx.Hurt.SolexPay
{
    public class SolexPay14Logic: SolexPayLogic
    {
        public override ProviderPlatnosciOnline Provider => ProviderPlatnosciOnline.SolexPay14;

        public SolexPay14Logic( ILog log, SolexPayConfig configSolexPay) 
            : base( log, configSolexPay) { }
    }
}