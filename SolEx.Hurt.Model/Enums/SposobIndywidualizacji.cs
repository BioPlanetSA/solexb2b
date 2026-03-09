using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum SposobIndywidualizacji
    {
        Grupowa,
        [FriendlyName("Każdy produkt ma być unikalnie indywidualizowany")]
        Indywidualna
    }
}
