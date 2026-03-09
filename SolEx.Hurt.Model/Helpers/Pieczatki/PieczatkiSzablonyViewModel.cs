using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.Hurt.Model.Helpers.Pieczatki
{
    public class PieczatkiSzablonyViewModel
    {
        public int Id { get; set; }
        public int TypId { get; set; }
        public string NazwaTypu { get; set; }
        public string NazwaSzablonu { get; set; }
        public string SciezkaDoPlikuSzablonuJSON { get; set; }
        public string SciezkaDoPlikuSzablonuSVG { get; set; }

        public string SymbolTypu { get; set; }
        //1 mm = 3.779528 px; 
        //1 mm = 5,67 // px * 1,5(przelicznik)
        //nie ruszać przelicznika, jak się go zmieni to się szablony rozjadą!
        private const decimal przelicznik = 5.67m; 
        public decimal Szerokosc_px { get; set; }
        public decimal Wysokosc_px { get; set; }

        public decimal Wysokosc_mm { get; set; }
        public decimal Szerokosc_mm { get; set; }

        public string OpisSzablonu { get; set; }

        public bool Zablokowany { get; set; }

        public PieczatkiSzablonyViewModel()
        {
        }

        public PieczatkiSzablonyViewModel(int id, int typId, string nazwaTypu, string json, string svg, decimal szerokoscmm, decimal wysokoscmm, string symbolTypu, string nazwaSzablonu, string opisszablonu)
        {
            Id = id;
            TypId = typId;
            NazwaTypu = nazwaTypu;
            SciezkaDoPlikuSzablonuJSON = json;
            SciezkaDoPlikuSzablonuSVG = svg;
            Szerokosc_px = Math.Ceiling(szerokoscmm*przelicznik);
            Wysokosc_px = Math.Ceiling(wysokoscmm*przelicznik);
            Wysokosc_mm = wysokoscmm;
            Szerokosc_mm = szerokoscmm;
            SymbolTypu = symbolTypu;
            NazwaSzablonu = nazwaSzablonu;
            OpisSzablonu = opisszablonu;
        }
    }
}
