using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public abstract class ModulPosiadajacyWarunki : ModulStowrzonyNaPodstawieZadania
    {
        public IList<ZadanieBll> Warunki()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZadanieBll>(null, x => x.ZadanieNadrzedne == Id);
        }

        /// <summary>
        /// Listy wykluczomych warunków do zadania, np p³atnoœci w zadaniu p³atnoœci
        /// </summary>
        public virtual List<Type> WykluczoneWarunki => new List<Type>();

        public abstract Type TypWarunkow { get; }
    }
}