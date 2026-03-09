using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.Hurt.Model.Web
{
    public class DanePlik
    {
        public DanePlik() { }

        public DanePlik(string nazwa, DateTime data, string url,string hash)
        {
            Nazwa = nazwa;
            DataModyfikacji = data;
            UrlPlik = url;
            Hash = hash;
        }
        public string Nazwa { get; set; }
        public DateTime DataModyfikacji { get; set; }
        public string UrlPlik { get; set; }
        public string Status { get; set; }
        public string Hash { get; set; }
    }
}
