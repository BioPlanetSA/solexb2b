using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL
{
    public class RegionyDostep : LogikaBiznesBaza, IRegionyDostep
    {
        public RegionyDostep(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        /// <summary>
        /// Metoda pobierajaca ragiony dla określonego kraju
        /// </summary>
        /// <param name="kraj"></param>
        /// <param name="jezyk"></param>
        /// <param name="tylkoAktywne"></param>
        /// <returns></returns>
        public IList<Region> PobierzRegionyKraju(int kraj, int jezyk, bool tylkoAktywne = true)
        {
            return Calosc.DostepDane.Pobierz<Region>(jezyk, null, x => x.KrajId == kraj && (!tylkoAktywne || x.Widoczny));
        }
    }
}