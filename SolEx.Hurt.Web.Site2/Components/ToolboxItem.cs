using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Web.Components
{
    public class ToolboxItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IconClass { get; set; }
        public string Command { get; set; }
        public string Action { get; set; }
        public string Url { get; set; }

        public ToolboxItem()
        {

        }

        public ToolboxItem(string name, string icon, string url)
        {
            Name = name;
            IconClass = icon;
            Url = url;
        }
    }
}
