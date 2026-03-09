using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using FastMember;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.DostepDane
{
   public interface IDaneDostep
   {
       void ZamknijPolaczenieDoBazy();

       IDbConnection DbORM { get; }
       void UzupelnijLokalizacje(object obiekt, TypeAccessor akcesor, IEnumerable<Tlumaczenie> polaobiektu);
       void AktualizujLokalizacje<TDane>(IList<object> dane);
       string KluczCacheTypDanych<TDane>(object kluczPojedynczegoObiektu = null);

       void AktualizujLokalizacjePojedyncze(object dane);

       string KluczWszystkie<TDane>(int jezykPobierania, IKlient zadajacy, object kluczPojedynczegoObiektu = null);

       /// <summary>
       /// Binduje zdarzenie do wykonania przed usunieciem obiektow
       /// </summary>
       /// <typeparam name="TDane">Typ danych</typeparam>
       /// <typeparam name="T">typ klucza</typeparam>
       /// <param name="akcjaOtrzymujacaKlucze">Akcja która ma się wykonać</param>
       void BindPrzedUsunieciem<TDane,T>(Action<IList<T>> akcjaOtrzymujacaKlucze);

       /// <summary>
       /// Binduje zdarzenie do wykonania po usunieciu obiektow
       /// </summary>
       /// <typeparam name="TDane">Typ danych</typeparam>
       /// <typeparam name="T">typ klucza</typeparam>
       /// <param name="akcjaOtrzymujacaKlucze">Akcja która ma się wykonać</param>
       void BindPoUsunieciu<TDane,T>(Action<IList<T>> akcjaOtrzymujacaKlucze);
       /// <summary>
       /// Binduje zdarzenie do wykonania przed aktualizację danych
       /// </summary>
       /// <typeparam name="TDane">Typ danych</typeparam>
       /// <param name="akcja">Akcja która ma się wykonać</param>
       void BindPrzedUpdate<TDane>(Action<IList<TDane>> akcja);
       /// <summary>
       /// Binduje zdarzenie do wykonania po aktualizacji danych
       /// </summary>
       /// <typeparam name="TDane">Typ danych</typeparam>
       /// <param name="akcjaOtrzymujacaKlucze">Akcja która ma się wykonać</param>
       void BindPoUpdate<TDane>(Action<IList<object>> akcjaOtrzymujacaKlucze);

       /// <summary>
       /// Binduje zdarzenie do wykonania po aktualizacji danych
       /// </summary>
       /// <typeparam name="TDane">Typ danych</typeparam>
       /// <param name="akcjaOtrzymujacaObiekty">Akcja która ma się wykonać</param>
       void BindPoUpdate<TDane>(Action<IList<TDane>> akcjaOtrzymujacaObiekty);

       /// <summary>
       /// Binduje zdarzenie do wykonania przed aktualizację danych, zwraca osobno nowe i zmodyfikowane obiekty. Użycie bindigu jest czasochłonnne
       /// </summary>
       /// <typeparam name="TDane">Typ danych</typeparam>
       /// <param name="akcja">Akcja która ma się wykonać</param>
       void BindPrzedUpdateRozroznienieStareIAktualizowane<TDane>(Action<IList<TDane>,IList<TDane>> akcja);

        /// <summary>
       /// Binduje zdarzenie do wykonania po aktualizacji danych, zwraca osobno nowe i zmodyfikowane obiekty. Użycie bindigu jest czasochłonnne
       /// </summary>
       /// <typeparam name="TDane">Typ danych</typeparam>
       /// <param name="akcjaOtrzymujacaObiekty">Akcja która ma się wykonać</param>
       void BindPoUpdateRozroznienieStareIAktualizowane<TDane>(Action<IList<TDane>, IList<TDane>> akcjaOtrzymujacaObiekty);

        /// <summary>
        /// Bidnuje zdarzenie do wykonania po aktualizacji danych, zwraca tylko kolekcje nowych elementów. Użycie bindigu jest czasochłonnne
        /// </summary>
        /// <typeparam name="TDane">Typ danych</typeparam>
        /// <param name="akcjaOtrzymujacaObiekty">Akcja która ma się wykonać</param>
        void BindPoUpdateTylkoNoweElementy<TDane>(Action<IList<TDane>> akcjaOtrzymujacaObiekty);

       /// <summary>
       /// Tworzy bindowanie dla określego typu
       /// </summary>
       /// <typeparam name="TDane">Typ danych jaki chcemy pobierać</typeparam>
       /// <param name="cachowanie">sposób cachowania danych</param>
       /// <param name="walidator">Metoda walidująca czy klient ma dostęp do konkretengo zwracanego obiektu, jak null to brak walidacji</param>
       /// <param name="metodaZwracajacaDane">Własna metoda pobierająca dane</param>
       /// <param name="typLokalizacje">Typ do lokalizacji</param>
       /// <param name="metodaPrzetwarzajacaPoSelect">Metoda wykonywana po select</param>
       void BindSelect<TDane>(SposobCachowania cachowanie = SposobCachowania.CalaLista, Expression<Func<TDane, IKlient, bool>> walidator = null, Func<int, IKlient, IList<TDane>> metodaZwracajacaDane = null, Type typLokalizacje = null, Func<int, IKlient, IList<TDane>, object, IList<TDane>> metodaPrzetwarzajacaPoSelect = null);

       void UsunPojedynczy<TDane>(object klucz) where TDane : class, new();

       /// <summary>
       /// Usuwa elementy o okreslonych kluczach
       /// </summary>
       /// <typeparam name="TDane">Typ usuwanego obiektu</typeparam>
       /// <param name="klucz"></param>
       void Usun<TDane,T>(List<T> klucz) where TDane : class, new();

        /// <summary>
        /// Pobiera ustalone bindowanie dla typu
        /// </summary>
        /// <param name="typ"></param>
        /// <returns></returns>
       ParametrBindowaniaPobieraniaDanych<T> PobierzBindowaniaPobieraniaDanych<T>();

       /// <summary>
       /// Usuwa elementy spełniające kryteria
       /// </summary>
       /// <typeparam name="TDane">Typ usuwanego obiektu</typeparam>
       /// <param name="filtr">Filtr danych</param>
       void UsunWybrane<TDane,T>(Expression<Func<TDane, bool>> filtr) where TDane : class, new();
       object AktualizujWgTypu(object obiekt);
       IList<object> AktualizujListe<TDane>(IList<TDane> obiekt) where TDane : class,new();
       object AktualizujPojedynczy<TDane>(TDane obiekt) where TDane : class,new();
       TDane PobierzPojedynczy<TDane>(object klucz) where TDane : class, new();
       TDane PobierzPojedynczy<TDane>(object klucz, int jezyk) where TDane : class ,new();
       TDane PobierzPojedynczy<TDane>(object klucz, IKlient zadajacy) where TDane : class, new();
       TDane PobierzPojedynczy<TDane>(object klucz, int jezyk, IKlient zadajacy) where TDane : class, new();
       TDane PobierzPojedynczy<TDane>(Expression<Func<TDane, bool>> filtr, IKlient zadajacy) where TDane : class, new();
       TDane PobierzPojedynczy<TDane>(Expression<Func<TDane, bool>> filtr, int jezyk, IKlient zadajacy,bool tworzGdyNieznaleziono) where TDane : class, new();


       /// <summary>
       /// Pobiera wszystkie elementy
       /// </summary>
       /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
       /// <param name="zadajacy">Klient ktory rząda danych</param>
       /// <returns></returns>
       IList<TDane> Pobierz<TDane>(IKlient zadajacy) where TDane : class , new();
       /// <summary>
       /// Pobiera wszystkie elementy
       /// </summary>
       /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
       /// <param name="jezykPobierania">Język w których chce dane</param>
       /// <param name="zadajacy">Klient ktory rząda danych</param>
       /// <returns></returns>
       IList<TDane> Pobierz<TDane>(int jezykPobierania, IKlient zadajacy) where TDane : class , new();


       /// <summary>
       /// Pobiera wszystkie elementy
       /// </summary>
       /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
       /// <param name="jezykPobierania">Język w których chce dane</param>
       /// <param name="zadajacy">Klient ktory rząda danych</param>
       /// <param name="filtr">Filtr danych</param>
       /// <returns></returns>
       IList<TDane> Pobierz<TDane>(int jezykPobierania, IKlient zadajacy, Expression<Func<TDane, bool>> filtr, object parametrDoMetodyPoSelect = null) where TDane : class , new();
      

       /// <summary>
       /// Pobiera wszystkie elementy spełniające kryteria
       /// </summary>
       /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
       /// <param name="zadajacy">Klient ktory rząda danych</param>
       /// <param name="filtr">Filtr danych</param>
       /// <returns></returns>
       IList<TDane> Pobierz<TDane>(IKlient zadajacy, Expression<Func<TDane, bool>> filtr, object parametrDoMetodyPoSelect=null) where TDane : class , new();



       /// <summary>
       /// Pobiera wszystkie elementy spełniające kryteria
       /// </summary>
       /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
       /// <param name="jezykPobierania">Język w których chce dane</param>
       /// <param name="zadajacy">Klient ktory rząda danych</param>
       /// <param name="filtr">Filtr danych</param>
       /// <param name="sortowania">Kolekcja sortowań</param>
       /// <returns></returns>
        IList<TDane> Pobierz<TDane>(int jezykPobierania, IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania, object parametrDoMetodyPoSelect = null) where TDane : class , new();
       /// <summary>
       /// Pobiera wszystkie elementy spełniające kryteria
       /// </summary>
       /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
       /// <param name="zadajacy">Klient ktory rząda danych</param>
       /// <param name="filtr">Filtr danych</param>
       /// <param name="sortowania">Kolekcja sortowań</param>
       /// <returns></returns>
       IList<TDane> Pobierz<TDane>(IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania) where TDane : class , new();
       /// <summary>
       /// Pobiera wszystkie elementy spełniające kryteria
       /// </summary>
       /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
       /// <param name="zadajacy">Klient ktory rząda danych</param>
       /// <param name="filtr">Filtr danych</param>
       /// <param name="sortowania">Kolekcja sortowań</param>
       /// <param name="nrStrony">Nr strony</param>
       /// <param name="rozmiarStrony">Rozmiar strony</param>
       /// <returns></returns>
       IList<TDane> Pobierz<TDane>(IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania, int nrStrony, int rozmiarStrony) where TDane : class , new();


       IList<TDane> Pobierz<TDane>(IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania, int nrStrony, int rozmiarStrony,out long lacznie) where TDane : class , new();

       /// <summary>
       /// Pobiera wszystkie elementy spełniające kryteria
       /// </summary>
       IList<object> PobierzWgTypu(Type typDanych, IKlient zadajacy, Expression filtr, string sortowanie, KolejnoscSortowania kierunek, int numerStrony, int rozmiarStrony, int jezyk, out long lacznie);
       /// <summary>
       /// Pobiera wszystkie elementy spełniające kryteria
       /// </summary>
       /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
       /// <param name="jezykPobierania">Język w których chce dane</param>
       /// <param name="zadajacy">Klient ktory rząda danych</param>
       /// <param name="filtr">Filtr danych</param>
       /// <param name="sortowania">Kolekcja sortowań</param>
       /// <param name="nrStrony">Nr strony</param>
       /// <param name="rozmiarStrony">Rozmiar strony</param>
       /// <returns></returns>
       IList<TDane> Pobierz<TDane>(int jezykPobierania, IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania, int nrStrony, int rozmiarStrony, object parametrDoMetodyPoSelect) where TDane : class , new();

        object PobierzPojednczyWgTypu(Type typ, IKlient zadajacy, string klucz, int? jezyk, bool tworzPustyGdyBrak);

        void UsunWyTypu(Type typdanych, object id);
       void AktualizujBaze(Assembly zasoby);
       bool CzyJestWlaczonyZapisZmiany(Type typ, object id);
       void WyczyscTabele(string nazwa);
       bool CzySaZmianyNaObiekcie(Type typ, object id);
       bool SprawdzAktywnoscZmian(string tabela);
       bool CzyJestWlaczonyZmiany<TDane>(object id);
       Dictionary<string, List<ZmianaObiektu>> PobierzZmianyObiektu<TDane>(object id);

       Dictionary<string, List<ZmianaObiektu>> PobierzZmianyObiektuWgTypu(Type typ, object id);
       bool PrzywrocWersje(Type typ, string data, string znacznik);
       long GetAutoID(Type typ);
       Dictionary<string, long> TabelaOrazJejRozmiar();

       /// <summary>
       /// metoda zpaisuje do bazy danych obiekty. Jesli ID = 0 sprawdza czy juz jest taki obiekt, jesli nie to uzupelnia identyfikator - UJEMNY, chyba ze jest autoinkrement - wtedy zostawia 0
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="objs"></param>
       void SaveAll_UzupelnijKlucze<T>(IEnumerable<T> objs) where T : new();
        List<T> Select<T>(Expression<Func<T, bool>> predicate);
    }
}
