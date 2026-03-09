using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin
{
    public class DaneObiekt
    {
        private readonly List<DodatkowaFunkcja> _funkcje;
        private readonly string _klucz;
        private readonly List<DanePole> _pola;

        //public string KolorTla { get; set; }

        public string Klucz
        {
            get { return _klucz; }
        }

        public IList<DanePole> Pola
        {
            get { return _pola; }
        }

        public IList<DodatkowaFunkcja> Funkcje
        {
            get { return _funkcje; }
        }

        public string KlasaCssWiersza { get; set; }

        public DaneObiekt(string daneKlucz)
        {
            MoznaEdytowac = true;  //domyslnie mozna - potem sie blokuje wewnatrz metody najwyzej
            MoznaUsuwac = true; //domyslnie mozna - potem sie blokuje wewnatrz metody najwyzej
            _klucz = daneKlucz;
            _pola = new List<DanePole>();
            _funkcje = new List<DodatkowaFunkcja>();
        }

        public void DodajPole(DanePole pole)
        {
            _pola.Add(pole);
        }

        public void DodajFunkcje(IEnumerable<DodatkowaFunkcja> funkcje)
        {
            _funkcje.AddRange(funkcje);
        }

        public bool MoznaUsuwac { get; set; }
        public bool MoznaEdytowac { get; set; }

        public string NazwaObiektu { get; set; }
        public Type TypObiektu { get; set; }
        public string PrzyjaznyOpisObiektu { get; set; }

        public List<Komunikat> Komunikaty { get; set; }
    }
}