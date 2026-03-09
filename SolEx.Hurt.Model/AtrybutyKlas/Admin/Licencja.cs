using System;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public class Licencja: Attribute
    {
        public Licencje[] Licencje { get; private set; }
   
        public Licencja(Licencje licencje)
        {
            Licencje = new[] { licencje };
        }

        public Licencja(Licencje[] licencje)
        {
            Licencje = licencje;
        }

        public Licencja()
        {
            Licencje = null;
        }
    }
}
