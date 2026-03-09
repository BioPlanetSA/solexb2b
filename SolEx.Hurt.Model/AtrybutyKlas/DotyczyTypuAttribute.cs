using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Enums
{
    public class DotyczyTypuAttribute : Attribute
    {
        public Type Typ { get; set; }
        public DotyczyTypuAttribute(Type typ)
        {
            Typ = typ;
        }
    }
}
