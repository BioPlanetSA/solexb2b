using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoFileManagera
    {
        public ParametryDoFileManagera(string link, string naglowek)
        {
            Link = link;
            NazwaNaglowka = naglowek;
        }
        public string Link { get; set; }
        public string NazwaNaglowka { get; set; }
    }
}