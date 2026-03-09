using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model spisu tresci dla katalogu detalicznego
    /// </summary>
    public class ReportTableOfContentItem
    {
        public string Title { get; set; }
        public int Page { get; set; }
        public int Level { get; set; }
    }
}
