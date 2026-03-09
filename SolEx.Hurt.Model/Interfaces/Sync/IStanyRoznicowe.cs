using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    public interface IStanyRoznicowe
    {
        /// <summary>
        /// Stany do wysłania na platformę
        /// </summary>
        /// <param name="brakujaceStanyNaPlatformie">Lista stanów - produktów id i magazynów których brakuje na systemie</param>
        /// <returns></returns>
        List<ProduktStan> StanyDoWyslania(List<ProduktStan> brakujaceStanyNaPlatformie);

        List<Magazyn> PobierzIstniejaceMagazynyLokalne();
    }
}
