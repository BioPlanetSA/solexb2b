using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class PoleSzczegolyDane
    {
        public string Nazwa { get; set; }
        public object Wartosc { get; set; }

        public List<CechyBll> ListaCech{get; set;}
      
        public PoleSzczegolyDane(string nazwa, string wart, string sofix = "")
        {
            Nazwa = (string.IsNullOrEmpty(nazwa))?"": nazwa;
            Wartosc = wart+sofix;
        }

        public PoleSzczegolyDane(string nazwa, object wart)
        {
            Nazwa = (string.IsNullOrEmpty(nazwa)) ? "" : nazwa;
            Wartosc = wart;
        }
        public PoleSzczegolyDane(string nazwa, decimal wart, string sofix="")
        {
            string format = "0.##";
            Nazwa = (string.IsNullOrEmpty(nazwa)) ? "" : nazwa;
            Wartosc = wart.ToString(format) + sofix;
        }

        public PoleSzczegolyDane(string nazwa, int wart, string sofix = "")
        {
            string format = "0";
            Nazwa = (string.IsNullOrEmpty(nazwa)) ? "" : nazwa;
            Wartosc = wart.ToString(format) +  sofix;
        }
        public PoleSzczegolyDane(){}
    }
}