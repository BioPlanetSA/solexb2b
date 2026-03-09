namespace SolEx.Hurt.SolexPay.Model

{
    public class SolexPayNotificationObject
    {
        public string order_id { get; set; }
        public SolexPayStatus status { get; set; }
    }


}