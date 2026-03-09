using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SolEx.Hurt.Model.Web
{
    /// <summary>
    /// Reprezentuje pozycję w menu
    /// </summary>
    [Serializable]
    public class MenuItem 
    {
        /// <summary>
        /// Link 
        /// </summary>
        public string Link { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public string Description { get; set; }
        public int ID { get; set; }
        public List<MenuItem> Childs { get; set; }
        public MenuItem Parent { get; set; }
        public MenuItem()
        {
            Childs = new List<MenuItem>();
        }
        public MenuItem(string name)
            : this()
        {
            Name = name;
        }
        public MenuItem(string name,string parent):this(name)
        {
            Parent = new MenuItem(parent);
        }
    }
}
