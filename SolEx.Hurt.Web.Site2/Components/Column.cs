using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Web.Components
{
    public class Column
    {
        public string Name { get; set; }
        public string Style { get; set; }

        public Column(string name, string style)
        {
            Name = name;
            Style = style;
        }
    }
}
