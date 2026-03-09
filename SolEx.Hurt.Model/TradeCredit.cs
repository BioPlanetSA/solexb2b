using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model kredytu kupieckiego
    /// </summary>
    public class TradeCredit
    {
        public decimal CreditLeft { get; set; }
        public decimal CreditUsed { get; set; }
        public decimal CreditValue { get; set; }

    }
}
