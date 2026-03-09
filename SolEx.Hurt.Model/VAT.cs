using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model stawki podatkowej VAT
    /// </summary>
    public class VAT
    {
        public decimal Stake { get; set; }
        public string StakeDesc { get; set; }
        public decimal VatValue { get; set; }
        public decimal NettoValue { get; set; }
        public decimal BruttoValue { get; set; }
    }
}
