using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Enums
{
    public class GrupaZadanSynchronizacjiAttribute : Attribute
    {
        public string NazwaGrupy { get; set; }
        public GrupaZadanSynchronizacjiAttribute(string nazwa)
        {
            NazwaGrupy = nazwa;
        }
    }
    public class OpisZadanSynchronizacjiAttribute : Attribute
    {
        public string Opis { get; set; }
        public OpisZadanSynchronizacjiAttribute(string opis)
        {
            Opis = opis;
        }
    }
}
