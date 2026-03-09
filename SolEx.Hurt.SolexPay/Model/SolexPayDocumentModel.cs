

using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.SolexPay.Model
{
        public class SolexPayDocumentModel
        {
            public DokumentyBll dokument { get; set; }
            public string ClientNIP { get; set; }
            public string ClientEmail { get; set; }
            public string ClientPhone { get; set; }
        }

}