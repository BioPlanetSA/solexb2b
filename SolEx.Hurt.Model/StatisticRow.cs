using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model wiersza statystyki
    /// </summary>
    public class StatisticRow
    {
        public List<Object> Data { get; set; }

        public StatisticRow()
        {
            Data = new List<object>();
        }
    }
}
