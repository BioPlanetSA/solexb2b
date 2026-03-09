using System;

namespace SolEx.Hurt.SolexPay.Model

{
    public class SolexPayNotification
    {
        public string id { get; set; }
        public DateTime date { get; set; }

        public string type { get; set; }
        public SolexPayNotificationObject @object { get; set; }
    }


}