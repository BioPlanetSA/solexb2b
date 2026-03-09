using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model statystyki
    /// </summary>
    public class Statistic
    {
        public List<string> Headers { get; set; }
        public List<StatisticRow> Rows { get; set; }

        public Statistic()
        {
            Headers = new List<string>();
            Rows = new List<StatisticRow>();
        }
    }
}
