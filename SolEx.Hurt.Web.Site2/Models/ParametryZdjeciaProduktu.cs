using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryZdjeciaProduktu
    {
        public ParametryZdjeciaProduktu()
        {
        }
        public ParametryZdjeciaProduktu(IObrazek obr, string roz, bool pow = true)
        {
            Zdjecie = obr;
            Rozmiar = roz;
            PowiekszeniPoNajechaniu = pow;
        }
        public IObrazek Zdjecie { get; set; }
        public string Rozmiar { get; set; }
        public bool PowiekszeniPoNajechaniu { get; set; }
    }
}