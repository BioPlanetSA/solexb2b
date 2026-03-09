using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class Slajder
    {
        public int IleElementowWWierszu { get; set; }
        public int IleWierszy { get; set; }
        public int CzasPrzeskoku { get; set; }

        public int IileSlajdow { get; set; }

        public Slajder(int ileSlajdow)
        {
            IleElementowWWierszu = 4;
            IleWierszy = 1;
            CzasPrzeskoku = 0;
            IileSlajdow = ileSlajdow;
        }

        public string Konfiguracja
        {
            get
            {
                string iloscSlajdow = "";
                if (IleElementowWWierszu == 0)
                {
                    iloscSlajdow = @"""variableWidth"": true,  ""slidesToShow"":  " + (int)(IileSlajdow / 2);
                }
                else
                {
                    iloscSlajdow = string.Format(@"""slidesToShow"": {0}", IleElementowWWierszu);
                }

                string animacja = "";
                //animacja wlacza sie tylko jesli jest JEDEN slajd do pokazania
                if (IleElementowWWierszu == 1)
                {
                    animacja = @", ""fade"": true, ""cssEase"": ""linear""";
                }
                // ""slide"": {2},

                return string.Format(@"{{{0}, ""slidesToScroll"": 1, ""rows"": {1} {2},""autoplay"": {3}}}",
                    iloscSlajdow, IleWierszy, animacja, CzasPrzeskoku >0 ? "true" : "false");
            }
        }
    }
}