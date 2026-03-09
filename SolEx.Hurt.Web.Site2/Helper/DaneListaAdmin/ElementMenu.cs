using System;
using System.Collections.Generic;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin
{
    /// <summary>
    /// Reperezentuje element w menu
    /// </summary>
    public class ElementMenu
    {
        public ElementMenu()
        {
            Dzieci = new List<ElementMenu>();
        }

        public ElementMenu(string nazwa, string link, List<ElementMenu> dzieci, Type t)
        {
            Dzieci = dzieci;
            Nazwa = nazwa;
            Link = link;
            Typ = t;
        }

        /// <summary>
        /// Wyświetlana nazwa
        /// </summary>
        public string Nazwa { get; set; }

        /// <summary>
        /// Opis elementu
        /// </summary>
        public string Opis { get; set; }

        /// <summary>
        /// Link
        /// </summary>
        public string Link { get; set; }

        public Type Typ { get; set; }

        public List<ElementMenu> Dzieci { get; set; }
    }
}