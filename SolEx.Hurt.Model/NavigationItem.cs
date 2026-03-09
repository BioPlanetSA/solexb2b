using System.Collections.Generic;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model obiektu nawigacyjnego wykorzystywany przy tworzeniu menu 
    /// </summary>
    public class NavigationItem
    {
        public string NazwaModuluWykrywanie { get; set; }
        public string Tytul { get; set; }
        public string Grupa { get; set; }
        public string Url { get; set; }
        public bool Widoczne { get; set; }
        public NavigationItem()
        {
            Url = "#";
            Widoczne = true;
        }

        public string Opis { get; set; }

        public int Kolejnosc { get; set; }

        public bool WidoczneOddzial { get; set; }
    }
}
