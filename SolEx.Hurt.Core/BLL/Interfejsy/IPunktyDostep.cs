using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IPunktyDostep
    {
        List<PunktyWpisy> PobierzWpisyZBazy(long klient);

        void DodajPunkty(PunktyWpisy wpis);

        void ZdejmijPunkty(PunktyWpisy wpis);

        void UsunPunkty(int id);

        List<PunktyWpisy> PobierzPunktyKlienta(IKlient klient);

        decimal PobierzPunktyKlientaLacznie(IKlient klient);

        List<PunktyWpisy> PobierzPunktyKlienta(IKlient klient, DateTime odKiedy, DateTime doKiedy, string sortowanie, KolejnoscSortowania kierunek, string szukanieFraza);

        bool KlientMaDostepDoModulu(IKlient klient);

        /// <summary>
        /// Udostepnia dostep do calosci logiki
        /// </summary>
        ISolexBllCalosc Calosc { get; }
    }
}