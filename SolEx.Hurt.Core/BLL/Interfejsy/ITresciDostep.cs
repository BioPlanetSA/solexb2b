using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface ITresciDostep
    {
        /// <summary>
        /// Inicjacja danych podstawowych
        /// </summary>
        void DodajDomyslneTresci();

        /// <summary>
        /// Sprawdza czy dany klient ma dostęp do danej treści
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        bool SprawdzDostep(TrescBll arg1, IKlient arg2);

        /// <summary>
        /// Resetuje szablon
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wzor"></param>
        void ResetujSzablon(int id, List<TrescBllImport> wzor);

        /// <summary>
        /// Pobiera listę szablonów na dysku
        /// </summary>
        /// <returns></returns>
        List<string> IstniejaceSzablony();

        /// <summary>
        /// Wczytuje szablon treści z dysku
        /// </summary>
        /// <param name="nazwa"></param>
        /// <returns></returns>
        List<TrescBllImport> WczytajSzablonDyskowy(string nazwa);

        TrescBll PobierzStroneGlowna(IKlient klient, int jezyk);

        string PobierzStopke(int jezyk);

        string PobierzAutora(int jezyk);

        string PobierzStopkeMaile(IKlient klient, int jezyk);

        void Sprawdz(IList<TrescBll> dane);
        string PobierzStopkeNewsletterow(IKlient klient, int jezyk, string linkDoWypisania);

        bool CzyTrescOtwieranaJakoModal(TrescBll symbol, IKlient klient);
        //IList<TrescBll> BindingPoSelect(int jezykID, IKlient klient, IList<TrescBll> listaTresci, object opcjonalmnyParametr);
        bool? SprawdzCzyKontrolaMenuMaTresciUkrywaneWgKlientow(int id, TrescBll parent = null);
        void WyczyscCacheWierszy(IList<TrescWierszBll> obj);
        void UsunCacheWierszy(IList<object> obj);
    }
}