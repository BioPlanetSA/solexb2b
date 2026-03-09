using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model parametrów strony
    /// </summary>
    public class PageInfoClass
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public AccesLevel AccessId { get; set; }
        public int LayoutId { get; set; }

    }
}
