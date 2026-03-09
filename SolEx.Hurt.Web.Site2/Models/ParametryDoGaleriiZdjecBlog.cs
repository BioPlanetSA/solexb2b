using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoGaleriiZdjecBlog
    {
        public ParametryDoGaleriiZdjecBlog()
        {
        }

        public ParametryDoGaleriiZdjecBlog(string rozmiar, string naglowek, string stopka, List<IObrazek> zdjecia)
        {
            RozmiarZdjecia = rozmiar;
            Naglowek = naglowek;
            Stopka = stopka;
            Zdjecia = zdjecia;
        }
        
        public List<IObrazek> Zdjecia { get; set; }
        public string RozmiarZdjecia { get; set; }
        public string Naglowek { get; set; }
        public string Stopka { get; set; }
    }
}