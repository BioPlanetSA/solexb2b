using System.Collections.Specialized;

namespace SolEx.Hurt.Web.Components
{
    public class TreeItem
    {
        public int Id { get; set; }
        public string IdTable { get; set; }
        public int? ParentId { get; set; }
        public string Type { get; set; }
        public string Class { get; set; }
        public bool ReadOnly { get; set; }
        public bool ReadOnly2 { get; set; }
        public NameValueCollection Values { get; set; }
        public string LinkDoEdycji { get; set; }
        public string DodatkowyCSS { get; set; }
        public TreeItem()
        {
            ReadOnly = false;
            Values = new NameValueCollection();
        }

    }
}
