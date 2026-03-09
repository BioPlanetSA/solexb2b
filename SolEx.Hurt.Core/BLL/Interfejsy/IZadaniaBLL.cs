using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.BLL.ZadaniaDlaZamowienia;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IZadaniaBLL
    {
        bool CzyPoleJestSynchronizowane(Type typ, string nazwaPola, out int? id);

        bool JestAktywneZadanie<T>();
        List<ZadanieBll> PobierzZadaniaWyfiltrowane<T, TT>(int jezyk, IKlient klient);

        IEnumerable<TT> PobierzZadania<T, TT>(int jezyk, IKlient klient);

        IEnumerable<ZadanieCalegoKoszyka> PobierzZadaniaCalegoKoszykaKtorePasuja<T>(IKoszykiBLL koszyk) where T : IGrupaZadania;

        Dictionary<IKoszykPozycja, List<ZadaniePozycjiKoszyka>> PobierzZadaniaPozycjiKtorePasuja<T>(IKoszykiBLL koszykKlienta) where T : IGrupaZadania;

        /// <summary>
        /// Zwraca datę zakończenia działania określonego modułu
        /// </summary>
        /// <param name="nazwa">Nazwa modułu</param>
        /// <returns>Data, jeśli brak to DateTime.Max, jeśli brak data to DateTime.Min</returns>
        DateTime TerminOstatniegoUruchomienia(string nazwa);

        void SprawdzZadaniaSystemowe();

        void UsunZdublowaneModulySystemowe();

        bool KlientMaDostep(ModelBLL.ZadanieBll arg1, IKlient arg2);

        IList<ModulSynchronizacji> BindModulySynchronizacji(int jezyk, IKlient klient);

        void UsunCache(IList<object> obj);

        IList<ModelBLL.ModulKoszyka> BindModulyKoszyka(int arg1, IKlient arg2);

        IList<ModulPunktowy> BindModulyPunktowe(int arg1, IKlient arg2);

        IList<HarmonogramBll> BindHarmonogram(int arg1, IKlient arg2);

        IList<ZadanieBll> BindingPoSelecie(int jezykId, IKlient zadajacy, IList<ZadanieBll> obj, object parametrDoMetodyPoSelect = null);

    }
}