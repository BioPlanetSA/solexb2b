using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Interfaces.Modele
{

   public interface IHashPola
    {
       bool LiczycHash { get; }
       decimal WartoscNettoHash { get; }
       decimal WartoscNaleznaHash { get; }
       bool ZaplaconoHash { get; } 
       int? StatusIdHash { get; }
       int PlatnikIdHash { get; }
       string NazwaPlatnosciHash { get; }
       string NazwaDokumentuHash { get; }
       DateTime? TerminPlatnosciHash { get; }
    }
}
