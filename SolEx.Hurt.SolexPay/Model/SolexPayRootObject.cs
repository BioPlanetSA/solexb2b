using SolEx.Hurt.SolexPay.Model;
using System.Collections.Generic;

namespace SolEx.Hurt.SolexPay.Model

{
    public class SolexPayRootObject
    {
        public string partnerOrderId { get; set; }
        public string comment { get; set; }
        public int installments { get; set; }
        public SolexPayValue value { get; set; }
        public SolexPayApplicant applicant { get; set; }
        public SolexPayReceiver receiver { get; set; }
        public SolexPayReceiverBankAccount receiverBankAccount { get; set; }
        public string transferTitle { get; set; }
        public string partnerReferer { get; set; }
        public List<SolexPayAttachment> attachments { get; set; }
        public int defermentDays { get; set; }

        public string notificationUrl { get; set; }
        public string shopUrl { get; set; }
        public string invoicePaymentTerm { get; set; }

    }


}