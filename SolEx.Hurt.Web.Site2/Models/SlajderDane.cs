using System.Collections.Generic;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class SlajderDane
    {
        public SlajderDane() { }

        public SlajderDane(IList<Slajd> lista, int czasanimacji, int czasPrzeskoku = 0, Slajder ustawienia = null, string rozmiar = "", string DodatkoweKlasyCssElementyKontrolki = "")
        {
            ListaSlajdow = lista;
            CzasPrzeskoku = czasPrzeskoku*1000;
            UstawieniaSlajder = ustawienia;
            CzasAnimacji = czasanimacji*1000;
            RozmiarZdjecia = rozmiar;
            this.DodatkoweKlasyCssElementyKontrolki = DodatkoweKlasyCssElementyKontrolki;
        }
        public IList<Slajd> ListaSlajdow { get; set; }
        public int CzasPrzeskoku { get; set; }
        public int CzasAnimacji { get; set; }
        public Slajder UstawieniaSlajder { get; set; }
        public string RozmiarZdjecia { get; set; }

        public string DodatkoweKlasyCssElementyKontrolki { get; set; }
    }
}