using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using FastMember;
using ServiceStack.Common;
using ServiceStack.Common.Utils;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;
using ServiceStack.OrmLite.SqlServer;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using StackExchange.Profiling;
using ExpressionVisitor = System.Linq.Expressions.ExpressionVisitor;
using StringExtensions = ServiceStack.Text.StringExtensions;

namespace SolEx.Hurt.Core.DostepDane
{

    public class DostepDoDanych :BazaDostepDane, IDaneDostep
    {

        public IDbConnection DbORM
        {
            get { return this.Db; }
            protected set {  }
        }
        private readonly Dictionary<Type, List<Delegate>> _bindingiPrzedAktualizacja = new Dictionary<Type, List<Delegate>>();
        private readonly Dictionary<Type, List<Delegate>> _bindingiPoAktualizacji = new Dictionary<Type, List<Delegate>>();
        private readonly Dictionary<Type, List<Delegate>> _bindingiPoAktualizacjiObiekty = new Dictionary<Type, List<Delegate>>();
        private readonly Dictionary<Type, List<Delegate>> _bindingiPrzedUsunieciem = new Dictionary<Type, List<Delegate>>();
        private readonly Dictionary<Type, List<Delegate>> _bindingiPoUsunienciu= new Dictionary<Type, List<Delegate>>();
        private readonly Dictionary<Type, List<Delegate>> _bindingiPrzedAktualizacjaRozroznienieStareIAktualizowane = new Dictionary<Type, List<Delegate>>();
        private readonly Dictionary<Type, List<Delegate>> _bindingiPoAktualizacjiRozroznienieStareIAktualizowane = new Dictionary<Type, List<Delegate>>();

        private readonly Dictionary<Type, List<Delegate>> _bindingiPoAktualizacjiTylkoNoweElementy = new Dictionary<Type, List<Delegate>>();

       // private readonly Dictionary<Type, List<Delegate>> _bindingiPoAktualizacjiRozroznienieDanych = new Dictionary<Type, List<Delegate>>(); 
        private readonly ISolexBllCalosc _solexBllCalosc;
        private readonly Dictionary<Type, object> _slownikMapowan;  //tu musi byc object bo mamy rozne typy parmetrowBIndowania w srodku
        private readonly object _syncRoot;

        public DostepDoDanych(ISolexBllCalosc solexBllCalosc)
        {
            _solexBllCalosc = solexBllCalosc;
            _slownikMapowan=new Dictionary<Type, object>();
            _syncRoot=new object();
        }
        
        public TDane PobierzPojedynczy<TDane>(object klucz) where TDane : class ,new()
        {
            return PobierzPojedynczy<TDane>(klucz, _solexBllCalosc.Konfiguracja.JezykIDDomyslny, null); 
        }

        public TDane PobierzPojedynczy<TDane>(object klucz, int jezyk) where TDane : class ,new()
        {
            return PobierzPojedynczy<TDane>(klucz, jezyk, null);
        }

        //Dictionary< Type, Dictionary<long, TDane>> slownikCachePojedynczy

        public TDane PobierzPojedynczy<TDane>(object klucz, IKlient zadajacy) where TDane : class ,new()
        {
            //todo:slownikowanie
            return PobierzPojedynczy<TDane>(klucz, zadajacy == null ? _solexBllCalosc.Konfiguracja.JezykIDDomyslny : zadajacy.JezykId, zadajacy);
        }

        /// <summary>
        /// Gówna metoda ktora pobiera pojedyczny dane wg. klucza. Cachowanie tylkoPojedyczne lub CalaLista - oba sa buduja cache
        /// </summary>
        /// <typeparam name="TDane"></typeparam>
        /// <param name="klucz"></param>
        /// <param name="jezyk"></param>
        /// <param name="zadajacy"></param>
        /// <returns></returns>
        public TDane PobierzPojedynczy<TDane>(object klucz, int jezyk, IKlient zadajacy) where TDane : class ,new()
        {
            //sprawdzamy cache dla pojedycznych obiektów - jak nie ma to leicmy dalej - pobieramy normalnie
            ParametrBindowaniaPobieraniaDanych<TDane> bindowanie = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzBindowaniaPobieraniaDanych<TDane>();

            TDane dane = default(TDane);
            string kluczDoObiektu = null;
            Tuple<TDane, HashSet<long>> obiektWalidacjiKlienta = null;
            
            //OBA cache buduja pojedyczny cache - cala lista i pojedzyzcny. Cala lista - trzebea obserwować pemieciozernosc rozwizania
            if (bindowanie.SposobCachowania == SposobCachowania.TylkoPojedyncze || bindowanie.SposobCachowania == SposobCachowania.CalaLista)
            {
                kluczDoObiektu = KluczWszystkie<TDane>(jezyk, zadajacy, klucz);

                //jesli obiekt ma walidator to trzeba pobrać obiekt razem z walidacjami dla klientów
                if (bindowanie.Walidator == null)
                {
                    dane = _solexBllCalosc.Cache.PobierzObiekt<TDane>(kluczDoObiektu);
                }
                else
                {
                    obiektWalidacjiKlienta = _solexBllCalosc.Cache.PobierzObiekt< Tuple<TDane, HashSet<long>>>(kluczDoObiektu);

                    if (obiektWalidacjiKlienta != null)
                    {
                        if (zadajacy == null || obiektWalidacjiKlienta.Item2.Contains(zadajacy.Id))
                        {
                            dane = obiektWalidacjiKlienta.Item1;
                        }
                    }
                }

                if (dane != null)
                {
                    return dane;
                }
            }

            Expression<Func<TDane, bool>> exp = (Expression<Func<TDane, bool>>)SolexBllCalosc.PobierzInstancje.Szukanie.StworzWhereEpression<TDane>(new[] {ModelDefinition<TDane>.PrimaryKeyName}, new[] {klucz.ToString()}, false, bindowanie);
            dane = PobierzPojedynczy(exp, jezyk, zadajacy, false);

            //w bazie nie ma obiektu - NIE caachujemy tego - takich sytuacji nie powinno byc czesto i duzo!
            if (dane == null)
            {
                return null;
            }

            //dodanie do cache jak jest taka potrzeba
            if (bindowanie.SposobCachowania == SposobCachowania.TylkoPojedyncze || bindowanie.SposobCachowania == SposobCachowania.CalaLista)
            {               
                //jak jest walidator to trzeba dodac info do obiektu o walidacji
                if (bindowanie.Walidator == null)
                {
                    _solexBllCalosc.Cache.DodajObiekt(kluczDoObiektu, dane);
                }
                else
                {
                    if (obiektWalidacjiKlienta == null)
                    {
                        obiektWalidacjiKlienta = new Tuple<TDane, HashSet<long>>(dane, new HashSet<long>() );
                        _solexBllCalosc.Cache.DodajObiekt(kluczDoObiektu, obiektWalidacjiKlienta);
                    }
                    
                    if (zadajacy != null)
                    {
                        //nie trzeba nic wiecej robic - bo juz jest w cache
                        obiektWalidacjiKlienta.Item2.Add(zadajacy.Id);
                    }
                   
                }
            }

            return dane;
        }

        public TDane PobierzPojedynczy<TDane>(Expression<Func<TDane, bool>> filtr, IKlient zadajacy) where TDane : class, new()
        {
            return PobierzPojedynczy(filtr, _solexBllCalosc.Konfiguracja.JezykIDDomyslny, zadajacy, false); 
        }

        public TDane PobierzPojedynczy<TDane>(Expression<Func<TDane, bool>> filtr, int jezyk, IKlient zadajacy, bool tworzGdyNieznaleziono) where TDane : class, new()
        {
            TDane o = PobierzDaneInternal(jezyk, zadajacy, filtr,null,1,1).FirstOrDefault();
            if (o == null && tworzGdyNieznaleziono)
            {
                o = StworzPusty(typeof(TDane), zadajacy, jezyk) as TDane;
            }
            return o;
        }
        

        /// <summary>
        /// jezyk moze byc nuLL
        /// </summary>
        /// <typeparam name="TDane"></typeparam>
        /// <param name="typ"></param>
        /// <param name="zadajacy"></param>
        /// <param name="klucz"></param>
        /// <param name="jezyk"></param>
        /// <param name="tworzPustyGdyBrak"></param>
        /// <returns></returns>
        public object PobierzPojednczyWgTypu(Type typ, IKlient zadajacy, string klucz, int? jezyk, bool tworzPustyGdyBrak)
        {
            if (jezyk == null)
            {
                jezyk = _solexBllCalosc.Konfiguracja.JezykIDDomyslny;
            }

            Expression filtr = SolexBllCalosc.PobierzInstancje.Szukanie.StworzWhereEpression(typ, new[] { "Id" }, new[] { klucz }, true);
            long lacznie;

            object o = null;
            if (!string.IsNullOrEmpty(klucz))
            {
                o = PobierzWgTypu(typ, jezyk.Value, zadajacy, filtr, null, KolejnoscSortowania.asc, 1, int.MaxValue, out lacznie).FirstOrDefault();
            }
            if (o == null && tworzPustyGdyBrak)
            {
                o = StworzPusty(typ, zadajacy, jezyk.Value);
            }
            return o;
        }

        private object StworzPusty(Type typ, IKlient zadajacy, int jezyk)
        {
            object o = Activator.CreateInstance(typ);
            UzupelnijKlienta(o, zadajacy);
            UzupelnijPoleJezyk(o, jezyk);
            if (o is IObiektWidocznyDlaOkreslonychGrupKlientow)
            {
               ( (IObiektWidocznyDlaOkreslonychGrupKlientow)o).Widocznosc=new WidocznosciTypow{Typ = o.GetType().PobierzOpisTypu()};
            }
            return o;
        }
        

        public void UsunWyTypu(Type typdanych, object id)
        {
            MethodInfo metodaGeneryczna = GetType().GetMethod("UsunInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = metodaGeneryczna.MakeGenericMethod(typdanych, typeof(object));
            List<object> klucze = new List<object>() {id};
           generic.Invoke(this, new object[] {klucze, null});
        }

        /// <summary>
        /// Pobiera wszystkie elementy
        /// </summary>
        /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
        /// <param name="zadajacy">Klient ktory żada danych</param>
        /// <returns></returns>
        public virtual IList<TDane> Pobierz<TDane>(IKlient zadajacy) where TDane : class ,new()
        {
            return Pobierz<TDane>(_solexBllCalosc.Konfiguracja.JezykIDDomyslny, zadajacy);
        }

        /// <summary>
        /// Pobiera wszystkie elementy
        /// </summary>
        /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
        /// <param name="jezykPobierania">Język w których chce dane</param>
        /// <param name="zadajacy">Klient ktory żada danych</param>
        /// <returns></returns>
        public virtual IList<TDane> Pobierz<TDane>(int jezykPobierania, IKlient zadajacy) where TDane : class, new()
        {
            return Pobierz<TDane>(jezykPobierania, zadajacy, null);
        }

        /// <summary>
        /// Pobiera wszystkie elementy spełniające kryteria
        /// </summary>
        /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
        /// <param name="zadajacy">Klient ktory żada danych</param>
        /// <param name="filtr">Filtr danych</param>
        /// <returns></returns>
        public virtual IList<TDane> Pobierz<TDane>(IKlient zadajacy, Expression<Func<TDane, bool>> filtr, object parametrDoMetodyPoSelect = null) where TDane : class ,new()
        {
            return Pobierz(_solexBllCalosc.Konfiguracja.JezykIDDomyslny, zadajacy,filtr, parametrDoMetodyPoSelect);
        }

        /// <summary>
        /// Pobiera wszystkie elementy
        /// </summary>
        /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
        /// <param name="jezykPobierania">Język w których chce dane</param>
        /// <param name="zadajacy">Klient ktory żada danych</param>
        /// <param name="filtr">Filtr danych</param>
        /// <returns></returns>
        public IList<TDane> Pobierz<TDane>(int jezykPobierania, IKlient zadajacy, Expression<Func<TDane, bool>> filtr, object parametrDoMetodyPoSelect = null) where TDane : class ,new()
        {
            return Pobierz(jezykPobierania, zadajacy, filtr, null, parametrDoMetodyPoSelect);
        }

        /// <summary>
        /// Pobiera wszystkie elementy spełniające kryteria
        /// </summary>
        /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
        /// <param name="zadajacy">Klient ktory żada danych</param>
        /// <param name="filtr">Filtr danych</param>
        /// <param name="sortowania">Kolekcja sortowań</param>
        /// <returns></returns>


        public IList<TDane> Pobierz<TDane>(IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania) where TDane : class ,new()
        {
            return Pobierz(_solexBllCalosc.Konfiguracja.JezykIDDomyslny, zadajacy, filtr, sortowania);
        }

        /// <summary>
        /// Pobiera wszystkie elementy spełniające kryteria
        /// </summary>
        /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
        /// <param name="jezykPobierania">Język w których chce dane</param>
        /// <param name="zadajacy">Klient ktory żada danych</param>
        /// <param name="filtr">Filtr danych</param>
        /// <param name="sortowania">Kolekcja sortowań</param>
        /// <returns></returns>
        public IList<TDane> Pobierz<TDane>(int jezykPobierania, IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania, object parametrDoMetodyPoSelect = null) where TDane : class ,new()
        {
            return Pobierz(jezykPobierania, zadajacy, filtr, sortowania, 1, int.MaxValue, parametrDoMetodyPoSelect);
        }

        /// <summary>
        /// Pobiera wszystkie elementy spełniające kryteria
        /// </summary>
        /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
        /// <param name="zadajacy">Klient ktory żada danych</param>
        /// <param name="filtr">Filtr danych</param>
        /// <param name="sortowania">Kolekcja sortowań</param>
        /// <param name="nrStrony">Nr strony</param>
        /// <param name="rozmiarStrony">Rozmiar strony</param>
        /// <returns></returns>

        public IList<TDane> Pobierz<TDane>(IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania, int nrStrony, int rozmiarStrony) where TDane : class ,new()
        {
            return Pobierz(_solexBllCalosc.Konfiguracja.JezykIDDomyslny, zadajacy, filtr, sortowania,nrStrony,rozmiarStrony, null);
        }

        public IList<TDane> Pobierz<TDane>(IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania, int nrStrony, int rozmiarStrony, out long lacznie) where TDane : class, new()
        {
            lacznie = PobierzLacznie(zadajacy, filtr);
            return Pobierz(zadajacy, filtr, sortowania, nrStrony, rozmiarStrony);
        }


        public IList<object> PobierzWgTypu(Type typDanych, int jezyk, IKlient zadajacy, Expression filtr, string sortowanie, KolejnoscSortowania kierunek,  int numerStrony, int rozmiarStrony, out long lacznie)
        {
            MethodInfo metodaGenerycznaLacznie = GetType().GetMethod("PobierzLacznie", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo genericLacznie = metodaGenerycznaLacznie.MakeGenericMethod(typDanych);
            lacznie = (long)genericLacznie.Invoke(this, new object[] { zadajacy, filtr });
           
            MethodInfo metodaGeneryczna = GetType().GetMethod("PobierzDaneInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = metodaGeneryczna.MakeGenericMethod(typDanych);

            object sortowania = null;
            if (sortowanie != null)
            {
                MethodInfo metodaGenerycznaSortowanie = typeof(Szukanie).GetMethod("WygenerujSortowanieLista", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo genericSortowanie = metodaGenerycznaSortowanie.MakeGenericMethod(typDanych);
                sortowania = genericSortowanie.Invoke(SolexBllCalosc.PobierzInstancje.Szukanie, new object[] { sortowanie, kierunek });
            }

            List<object> wynik = ((IEnumerable<object>)generic.Invoke(this, new object[] { jezyk, zadajacy, filtr, sortowania, numerStrony, rozmiarStrony, null })).ToList();
            return wynik;
        }
        

        public IList<object> PobierzWgTypu(Type typDanych, IKlient zadajacy, Expression filtr, string sortowanie, KolejnoscSortowania kierunek, int numerStrony, int rozmiarStrony, int jezyk, out long lacznie)
        {
            if(jezyk ==0) jezyk = _solexBllCalosc.Konfiguracja.JezykIDDomyslny;
            return PobierzWgTypu(typDanych, jezyk, zadajacy, filtr, sortowanie, kierunek,  numerStrony, rozmiarStrony, out lacznie);
        }

        /// <summary>
        /// Pobiera wszystkie elementy spełniające kryteria
        /// </summary>
        /// <typeparam name="TDane">Typ pobieranego obiektu</typeparam>
        /// <param name="jezykPobierania">Język w których chce dane</param>
        /// <param name="zadajacy">Klient ktory żada danych</param>
        /// <param name="filtr">Filtr danych</param>
        /// <param name="sortowania">Kolekcja sortowań</param>
        /// <param name="nrStrony">Nr strony</param>
        /// <param name="rozmiarStrony">Rozmiar strony</param>
        /// <param name="parametrDoMetodyPoSelect"></param>
        /// <returns></returns>
        public IList<TDane> Pobierz<TDane>(int jezykPobierania, IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania, int nrStrony, int rozmiarStrony, object parametrDoMetodyPoSelect) where TDane : class ,new()
        {
            return PobierzDaneInternal(jezykPobierania, zadajacy, filtr, sortowania, nrStrony, rozmiarStrony, parametrDoMetodyPoSelect);
        }

        private IList<TDane> PobierzInternal<TDane>(ParametrBindowaniaPobieraniaDanych<TDane> bindowanie, int jezykPobierania, IKlient zadajacy, Expression<Func<TDane, bool>> filtr,
        IList<SortowanieKryteria<TDane>> sortowania, int nrStrony, int rozmiarStrony, object parametrDoMetodyPoSelect = null) where TDane : class, new()
        {
            if (bindowanie.SposobCachowania == SposobCachowania.ZakazanePobieranie)
            {
                throw new Exception(string.Format("Błąd pobieranie danych - nie można pobierać typu {0} bezpośrednio", typeof(TDane)));
            }

            if (bindowanie.SposobCachowania == SposobCachowania.CalaLista)
            {
                if (bindowanie.FiltrySql)
                {
                    //jak jest filtrowanie SQL to NIE MOZNA cachować - trzeba sie zastanowic czy chcemy cachować to pozniej
                    throw new Exception("Nie można cachować listy która jest filtrowana SQL - jeśli tu weszliśmy to znaczy że jest błąd w kodzie - nie wolno dopuścić do filtrowania SQL list które są cachowane jako cala lista");
                }

                string klucz = this.KluczWszystkie<TDane>(jezykPobierania, zadajacy);

                //nie ma w cache - pobieranie będzie z SQL danych
                IList<TDane> dane = LockHelper.PobierzInstancje.PobierzDaneWLocku_zUcyciemCache(klucz, () =>
                {
                    IList<TDane> daneWynikowe = WczytajDane(bindowanie, jezykPobierania, zadajacy, filtr, sortowania, nrStrony, rozmiarStrony, parametrDoMetodyPoSelect);
                    return daneWynikowe;
                });
                return dane;
            }

            return WczytajDane(bindowanie, jezykPobierania, zadajacy, filtr, sortowania, nrStrony, rozmiarStrony, parametrDoMetodyPoSelect);
        }

        private IList<TDane> PobierzDaneInternal<TDane>(int jezykPobierania, IKlient zadajacy, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania = null, int nrStrony = 1, int rozmiarStrony = int.MaxValue, object parametrDoMetodyPoSelect = null) where TDane : class ,new()
        {
            ParametrBindowaniaPobieraniaDanych<TDane> bindowanie = PobierzBindowaniaPobieraniaDanych<TDane>();

#if DEBUG
            Timing czasMiniProfiler = null;
            if (bindowanie.SposobCachowania == SposobCachowania.CalaLista && filtr != null)
            {
                czasMiniProfiler = MiniProfiler.Current.Step($"{typeof(TDane).Name} - cache lista ALE filtr");
            }

            if (czasMiniProfiler == null && filtr != null)
            {
                czasMiniProfiler = MiniProfiler.Current.Step($"{typeof(TDane).Name} - z filtrem");
            }
#endif

            IList<TDane> dane = PobierzInternal(bindowanie, jezykPobierania, zadajacy, filtr, sortowania, nrStrony, rozmiarStrony, parametrDoMetodyPoSelect);

            if (dane.Any())
            {
                //walidowanie wg. widocznosci typów - tylko dla NIE adminow
                if (zadajacy != null && zadajacy.Dostep == AccesLevel.Zalogowani && !zadajacy.CzyAdministrator && typeof(TDane).InheritsOrImplements<IObiektWidocznyDlaOkreslonychGrupKlientow>() )
                {
                    HashSet<TDane> doUsuniecia = new HashSet<TDane>();
                    foreach (TDane o in dane)
                    {
                        IObiektWidocznyDlaOkreslonychGrupKlientow temp = o as IObiektWidocznyDlaOkreslonychGrupKlientow;
                        if (temp.Widocznosc == null)
                        {
                            continue;
                        }

                        //nie ma dostpeu - pomijamy typa
                        if (!_solexBllCalosc.WidocznosciTypowBll.KlientMaDostepDoObiektu(zadajacy, temp))
                        {
                            doUsuniecia.Add(o);
                        }
                    }

                    if (doUsuniecia.Any())
                    {
                        dane = dane.Where(x => !doUsuniecia.Contains(x)).ToList();
                    }
                }


                if (!bindowanie.FiltrySql) //nie ma filtowania w sql robimy je w c#
                {
                    if (bindowanie.Walidator != null && zadajacy != null)
                    {
                       dane = dane.Where(x => bindowanie.WalidatorSkompilowany(x, zadajacy)).ToList();
                    }

                    dane = Filtruj(dane, filtr, sortowania, nrStrony, rozmiarStrony);

                    //tak samo sortowanie i pagowanie - jak NIE Jest w SQL zrobione

                    //czesto jest jeden elementu - albo brak - nie ma sensu sortowac / pagowac
                    if (dane.Any() && dane.Count > 1)
                    {
                        if (sortowania != null && sortowania.Any())
                        {
                            dane = Sortowanie(sortowania, dane);
                        }

                        //todo: warunek zeby nie robi stron jak nie trzeba w metodzie sortowania
                        if (rozmiarStrony < int.MaxValue) dane = Stronicowanie(nrStrony, rozmiarStrony, dane);
                    }
                }                
            }


#if DEBUG
            if (czasMiniProfiler != null)
            {
                czasMiniProfiler.Stop();

                if (dane == null || !dane.Any())
                {
                    czasMiniProfiler.Name += ". BRAK REKORDU!!";
                }
            }
#endif

            return dane;
        }

        private IList<TDane> Filtruj<TDane>(IList<TDane> dane, Expression<Func<TDane, bool>> filtr, IList<SortowanieKryteria<TDane>> sortowania, int nrStrony, int rozmiarStrony)
        {
            Func<TDane,bool> wyrazenieSkompilowane=filtr != null ? filtr.Compile():null;

            if (wyrazenieSkompilowane == null)
            {
                return dane;
            }

            IList<TDane> pofiltrze = null;
            //jesli tylko jeden element chcemy i NIE ma sortowania, to stosujemy firstaorDefoulta
            if (nrStrony == 1 && rozmiarStrony == 1 && sortowania == null)
            {
                pofiltrze = new List<TDane>{dane.FirstOrDefault(wyrazenieSkompilowane)};
            }
            else
            {
                pofiltrze = dane.Where(wyrazenieSkompilowane).ToList();
            }
            return pofiltrze;
        }

        private static IList<TDane> Sortowanie<TDane>(IList<SortowanieKryteria<TDane>> sortowania, IList<TDane> pofiltrze)
        {
            if (sortowania == null)
            {
                return null;
            }
            IOrderedEnumerable<TDane> dosortowania = pofiltrze.OrderBy(x => true);
            foreach (SortowanieKryteria<TDane> s in sortowania)
            {
                if (s == null)
                {
                    continue;
                }

                Func<TDane, object> warSortowania = (Func<TDane, object>) s.Warunek;
                if (warSortowania == null)
                {
                    continue;
                }
                dosortowania = s.Kierunek == KolejnoscSortowania.asc
                    ? dosortowania.ThenBy(warSortowania)
                    : dosortowania.ThenByDescending(warSortowania);
            }
           return dosortowania.ToList();
        }

        private static IList<TDane> Stronicowanie<TDane>(int nrStrony, int rozmiarStrony, IList<TDane> dosortowania)
        {
            return dosortowania.Skip((nrStrony - 1)*rozmiarStrony).Take(rozmiarStrony).ToList();
        }

        private void DodajCache<TDane>(IList<TDane> dane, int jezyk, IKlient zadajacy)
        {
            //jesli obiekt dziedziczy po Interfejsie InterString - to wykonujemy internacje stringow po proeprtisach ktore tego wymagaja
            if (typeof(TDane).InheritsOrImplements(typeof(IStringIntern)) )
            {
                var polaDoInternacji = typeof(TDane).Properties(typeof(StringInternuj));

                if (polaDoInternacji.IsEmpty())
                {
                    throw new Exception($"Brak pól do internacji Stringów - typ: {typeof(TDane).Name} posiada interfejs IStringIntern, więc musi mieć pola internowane.");
                }
                var akcesor = typeof(TDane).PobierzRefleksja();
                foreach (TDane obiekt in dane)
                {
                    _solexBllCalosc.Cache.InternujStringiWObiekcie(obiekt, akcesor, polaDoInternacji);
                }
            }

            string klucz = KluczWszystkie<TDane>(jezyk, zadajacy);
            _solexBllCalosc.Cache.DodajObiekt(klucz, dane);
        }

        public static Expression<Func<T1, TResult>> Bind2nd<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> source, T2 argument)
        {
            Expression arg2 = Expression.Constant(argument, typeof(T2));
            Expression newBody = new Rewriter(source.Parameters[1], arg2).Visit(source.Body);
            return Expression.Lambda<Func<T1, TResult>>(newBody, source.Parameters[0]);
        }


        private class Rewriter : ExpressionVisitor
        {
            private readonly Expression candidate_;
            private readonly Expression replacement_;

            public Rewriter(Expression candidate, Expression replacement)
            {
                candidate_ = candidate;
                replacement_ = replacement;
            }

            public override Expression Visit(Expression node)
            {
                return node == candidate_ ? replacement_ : base.Visit(node);
            }
        }

        private long PobierzLacznie<TDane>(IKlient zadajacy, Expression<Func<TDane, bool>> filtr) where TDane : class, new()
        {
            ParametrBindowaniaPobieraniaDanych<TDane> bindowanie = PobierzBindowaniaPobieraniaDanych<TDane>();
            //jesli jest filtrowanie SQL to bezposrednio na SQL spradzamy COUNT
            if (bindowanie.FiltrySql)
            {
                SqlExpressionVisitor<TDane> warunki = PobierzWarunkiSQL(bindowanie, zadajacy, filtr, null, 0, int.MaxValue);
                return Db.Count<TDane>(warunki);
            }
            else
            {
                //todo: BARDZO nieOPTYMALNE!!!
                return PobierzDaneInternal(_solexBllCalosc.Konfiguracja.JezykIDDomyslny, zadajacy, filtr).Count;
            }

        }

        private SqlExpressionVisitor<TDane> PobierzWarunkiSQL<TDane>(ParametrBindowaniaPobieraniaDanych<TDane> bindowanie, IKlient zadajacy, Expression<Func<TDane, bool>> filtr, 
            IList<SortowanieKryteria<TDane>> sortowania, int nrStrony, int rozmiarStrony) where TDane : class, new()
        {
            SqlExpressionVisitor<TDane> warunki = DbFactory.DialectProvider.ExpressionVisitor<TDane>();

            if (bindowanie.FiltrySql)
            {
                try
                {
                    if (filtr != null)
                    {
                        warunki = warunki.Where(filtr);
                    }
                    //jak jest waliator i jest filtrowanie SQL to laczymy filtr walidatora z filtrem uzytkownika ZAWSZE
                    if (bindowanie.Walidator != null && (zadajacy != null  || bindowanie.Walidator_WymuszajPodanieZadajacego) )
                    {
                        if (zadajacy == null)
                        {                          
                            throw new Exception($"Obiekt {typeof(TDane).Name} wymaga walidacji wg. klienta, ale klient nie został podany - błąd.");                         
                        }

                        Expression<Func<TDane, bool>> likwidacjaJednegoParametru = Bind2nd(bindowanie.Walidator, zadajacy);
                        
                        try
                        {
                            warunki = warunki.Where(likwidacjaJednegoParametru);
                        }
                        catch (Exception)
                        {
                            throw new Exception("Nie można zbudować warunku SQL - prawdopodobnie w warunku są pola których nie ma w SQL (ignorowane)");
                        }
                    }
                                       
                    //sortowanie w SQL - bo obiekt jest SQLowy
                    if (sortowania != null && sortowania.Any())
                    {
                       
                        for(int i=0; i < sortowania.Count; ++i)
                        {
                            var s = sortowania[i];
                            Func<TDane, object > warSortowania = (Func<TDane, object>) s.Warunek;
                            if (warSortowania == null)
                            {
                                continue;
                            }
                            if (i == 0)
                            {
                                if (s.Kierunek == KolejnoscSortowania.asc)
                                {
                                    warunki.OrderBy(s.PoleNazwa);
                                }
                                else
                                {
                                    warunki.OrderByDescending(s.PoleNazwa);
                                }
                            }
                            else
                            {
                                if (s.Kierunek == KolejnoscSortowania.asc)
                                {
                                    warunki.ThenBy(s.PoleNazwa);
                                }
                                else
                                {
                                    warunki.ThenByDescending(s.PoleNazwa);
                                }
                            }
                        }
                    }

                    if (rozmiarStrony < int.MaxValue)
                    {
                        int skip = (nrStrony - 1)*rozmiarStrony;
                        warunki.Limit(skip, rozmiarStrony);
                    }

                    return warunki;
                }
                catch (Exception e)
                {
                    _solexBllCalosc.Log.Error("Warunek niemożliwy do zmiany na SQL. Zapewne ustawiasz warunek Linq dla elementu który wymaga warunków Sql.", e);
                    throw;
                }
            }
            else
            {
                return warunki;
            }
        }

        private IList<TDane> WczytajDane<TDane>(ParametrBindowaniaPobieraniaDanych<TDane> bindowanie, int jezyk, IKlient zadajacy, Expression<Func<TDane, bool>> filtr,
            IList<SortowanieKryteria<TDane>> sortowania, int nrStrony, int rozmiarStrony, object parametrDoMetodyPoSelet = null) where TDane : class, new()
        {
            if (bindowanie.ZwracanieDanych != null )
            {
                if (bindowanie.FiltrySql)
                {
                    throw new ArgumentException($"Nie wolno używać filrów sql gdy mamy własne pobieranie danych - typ: {typeof(TDane)}");
                }
                return ((Func<int,IKlient,IList<TDane>>)bindowanie.ZwracanieDanych)(jezyk,zadajacy);
            }

            List<TDane> czyste = null;
            SqlExpressionVisitor<TDane> warunki = null;
            int i = 2;
            bool pobierac = true;

            while (pobierac && i>=0)
            {
                try
                {
                    //if ( typeof(TDane) == typeof(Produkt)  || typeof(TDane) == typeof(ProduktBazowy) )
                    warunki = PobierzWarunkiSQL(bindowanie, zadajacy, filtr, sortowania, nrStrony, rozmiarStrony);
                    czyste = Db.Select<TDane>(warunki);
                    pobierac = false;
                }
                catch (Exception e)
                {
                    SqlException sqlException = e as SqlException;
                    if (sqlException != null && sqlException.Number == 1205 && i != 0)
                    {
                        if (i == 1)
                        {
                            SolexBllCalosc.PobierzInstancje.Log.ErrorFormat("wystąpił deadlock - zabezpieczone wystąpienie");
                        }
                        Random r = new Random();
                        int ilosc = r.Next(200, 300);
                        System.Threading.Thread.Sleep(ilosc);
                        i--;
                    }
                    else
                    {
                        if (warunki != null)
                        {
                            SolexBllCalosc.PobierzInstancje.Log.Error($"Błąd zapytania SQL. Komunikat: {e.Message}. \r\n Zapytanie: {warunki.ToSelectStatement()}", e);
                        }
                        throw;
                    }
                }
                finally
                {
                    //zwalniamy polaczenie do bazy danych bo bylo duzo klopotow z tym 
                   // this.ZamknijPolaczenieDoBazy();
                }
            }


            if (czyste.Any())
            {
                UzupenijLokalizacja(czyste, bindowanie, jezyk);
                UzupelnijKlienta(czyste, zadajacy);
                UzupelnijWidocznosc(czyste);
                if (bindowanie.MetodaPrzetwarzajacaPoSelect != null)
                {
                    return ((Func<int, IKlient, IList<TDane>, object, IList<TDane>>)bindowanie.MetodaPrzetwarzajacaPoSelect)(jezyk, zadajacy, czyste, parametrDoMetodyPoSelet);
                }
            }

            return czyste;
        }


        private void UzupelnijWidocznosc<TDane>(List<TDane> czyste)
        {
            Type t = typeof (TDane);
            if (!t.InheritsOrImplements<IObiektWidocznyDlaOkreslonychGrupKlientow>())
            {
                return;
            }
            string nazwaTypu = t.PobierzOpisTypu();
            var ids = czyste.ToDictionary(x => WidocznosciTypow.ObliczKlucz(  (x as IObiektWidocznyDlaOkreslonychGrupKlientow).Id, nazwaTypu), x => x);

            var dane = Pobierz<WidocznosciTypow>(null, x => Sql.In(x.Id, ids.Keys)).ToDictionary(x => x.Id, x=> x);

        //   Pobierz<WidocznosciTypow>(x=> ids.Contains()

            //pobieramy pojedynczo z bazy bo jest cache POJEDYNCZY!! tak ma byc - bartek, optymalizacja chyba lepsza
            foreach (var o in dane)
            {
                (ids[o.Key] as IObiektWidocznyDlaOkreslonychGrupKlientow).Widocznosc = o.Value;

                //IObiektWidocznyDlaOkreslonychGrupKlientow obiektZWidocznoscia = o as IObiektWidocznyDlaOkreslonychGrupKlientow;
                //long klucz = WidocznosciTypow.ObliczKlucz(obiektZWidocznoscia.Id, nazwaTypu);

                //if (_nulloweWidocznosci.Contains(klucz))
                //{
                //    return;
                //}

                //obiektZWidocznoscia.Widocznosc = PobierzPojedynczy<WidocznosciTypow>(klucz);

                //if (obiektZWidocznoscia.Widocznosc == null)
                //{
                //    _nulloweWidocznosci.Add(klucz);
                //}
            }
        }

        public string KluczCacheTypDanych<TDane>(object kluczPojedynczegoObiektu = null)
        {
            if (kluczPojedynczegoObiektu == null)
            {
                return $"Wszystkie_{typeof(TDane).FullName}";
            }
            return $"Wszystkie_{typeof(TDane).FullName}_id{kluczPojedynczegoObiektu}";
        }

        /// <summary>
        /// Liczy klucz do cache obiektów
        /// </summary>
        /// <typeparam name="TDane"></typeparam>
        /// <param name="jezykPobierania"></param>
        /// <param name="zadajacy"></param>
        /// <param name="kluczPojedynczegoObiektu">Opjonalnie jesli chcemy pobrać klucz dla konkretnego id obiektu danego typu wg. klucza</param>
        /// <returns></returns>
        public string KluczWszystkie<TDane>(int jezykPobierania, IKlient zadajacy, object kluczPojedynczegoObiektu = null)
        {
            int jezyk = 0;
            string klient = "";
            Type t = typeof (TDane);
            if (t.Implements<IPoleJezyk>())//tylko w takiej sytuacji jest sens dodawać jezyk do klucza, w innym wypadku zawsze pobieramy w jednym języku, produkt nie jest loalizowany
            {
                jezyk = jezykPobierania;
            }
            if (t.Implements<IPoleKlient>())//obiekt jest indywidualizowany pod klienta
            {
                if (zadajacy == null)
                {
                    throw  new InvalidOperationException("Próba generowania klucza dla typu indywidualizowanego pod klienta bez podania klienta");
                }
                klient = zadajacy.Id.ToString(CultureInfo.InvariantCulture)+"-"+zadajacy.OddzialDoJakiegoNalezyKlient;
            }
            return string.Format("{0}_jezyk_{1}_klient_{2}", KluczCacheTypDanych<TDane>(kluczPojedynczegoObiektu), jezyk, klient);
        }

        private void UzupelnijPoleJezyk(object obiekt, int jezyk)
        {
            if (obiekt is IPoleJezyk)
            {
                ((IPoleJezyk)obiekt).JezykId = jezyk;
            }
        }
        private void UzupenijLokalizacja<TDane>(IList<TDane> dane, ParametrBindowaniaPobieraniaDanych<TDane> bindowanie, int jezyk) where TDane : class, new()
        {
            if (!typeof (TDane).Implements(typeof (IPoleJezyk))) //obiekt nie jest lokalizowany
            {
                return;
            }

            if (!typeof(TDane).Implements<IHasIntId>() && !typeof(TDane).Implements<IHasLongId>())
            {
               // return;
                throw  new Exception($"Można tłumaczyć tylko obiekty z kluczami INT lub LONG. Obiekt: {typeof(TDane)}");
            }

            foreach (var obiekt in dane)
            {
                UzupelnijPoleJezyk(obiekt, jezyk);
            }
            if (jezyk == _solexBllCalosc.Konfiguracja.JezykIDDomyslny)
            {
                return;
            }
            string typ = bindowanie.TypDoLokalizacji.PobierzOpisTypu();

            Dictionary<long, TDane> slownikKluczy = null;

            if (typeof(TDane).Implements<IHasIntId>())
            {
                slownikKluczy = dane.ToDictionary(x => (long)(((IHasIntId) x).Id) , x=> x);
            }

            if (typeof(TDane).Implements<IHasLongId>())
            {
                slownikKluczy = dane.ToDictionary(x => (((IHasLongId) x).Id), x => x);
            }

            List<Tlumaczenie> tlumaczenia = Db.Where<Tlumaczenie>(x => x.JezykId == jezyk && x.Typ == typ && Sql.In(x.ObiektId, slownikKluczy.Keys.ToList())  );
            TypeAccessor refleksja = typeof(TDane).PobierzRefleksja();

            foreach (var obiekt in dane)
            {
                if (obiekt is IPoleJezyk)
                {
                    object klucz = obiekt.PobierzKlucz();
                    List<Tlumaczenie> polaobiektu = tlumaczenia.Where(x => x.ObiektId == Convert.ToInt64(klucz)).ToList();
                    UzupelnijLokalizacje(obiekt, refleksja, polaobiektu);
                }
            }
        }

        public void UzupelnijLokalizacje(object obiekt, TypeAccessor akcesor, IEnumerable<Tlumaczenie> polaobiektu)
        {           
            foreach (var p in polaobiektu)
            {
                if (p.Wpis == null)
                {
                    continue;
                }
                if (akcesor.GetMembers().FirstOrDefault(x => x.Name == p.Pole) == null)
                {
                    continue;
                }

                akcesor[obiekt, p.Pole] = p.Wpis;
            }
        }

        private void UzupelnijKlienta<TDane>(IList<TDane> dane  , IKlient klient)
        {
            if (typeof(TDane).Implements(typeof(IPoleKlient) ) )
            {
                if (klient == null)
                {
                    throw new ArgumentException("Próba pobrania obiektu wymagającego klient bez podanie klienta");
                }
                foreach (var d in dane)
                {
                    UzupelnijKlienta(d, klient);
                }
            }
        }

        private void UzupelnijKlienta(object obiekt, IKlient klient) 
        {
            IPoleKlient poleKlient = obiekt as IPoleKlient;
            if (poleKlient != null)
            {
                if (klient == null)
                {
                    throw new ArgumentException("Próba pobrania obiektu wymagającego klient bez podanie klienta");
                }
                poleKlient.Klient = klient;
            }
        }
        

        /// <summary>
        /// Pobiera bindowanie dla typu, jesli nie istnieje to tworzy domyślne
        /// </summary>
        /// <returns></returns>
        public ParametrBindowaniaPobieraniaDanych<T> PobierzBindowaniaPobieraniaDanych<T>()
        {
            Type typ = typeof(T);
            object temp;
            ParametrBindowaniaPobieraniaDanych<T> param = null;
            lock (_syncRoot)
            {
                if (!_slownikMapowan.TryGetValue(typ, out temp))
                {
                    //wszystko co nie jest jawnie powiedziane jak cachowac, cachowane jest jako BRAK
                    Debug.WriteLine("Brak sposobu cachowania dla typu: {0}", typ);
                    param = new ParametrBindowaniaPobieraniaDanych<T>(SposobCachowania.Brak, null, null, typ.PobierzBazowy(), true, null);
                    _slownikMapowan.Add(typ, param);
                }
                else
                {
                    param = temp as ParametrBindowaniaPobieraniaDanych<T>;
                }
            }            

            return param;
        }

        /// <summary>
        /// Tworzy bindowanie danych
        /// </summary>
        /// <typeparam name="TDane">Typ pobierany</typeparam>
        /// <param name="cachowanie">Sposób cachowania danych</param>
        /// <param name="walidator">Funckcja walidująca dostep klient do obiektu</param>
        /// <param name="metodaZwracajacaDane">Własna metoda zwracająca dane w inny sposób</param>
        /// <param name="typLokalizacji">Typ danych do pobierania z lokalizacji</param>
        /// <param name="metodaPrzetwarzajacaPoSelect">Metoda wykonywana po select</param>
        public void BindSelect<TDane>(SposobCachowania cachowanie = SposobCachowania.CalaLista, Expression<Func<TDane, IKlient, bool>> walidator = null, Func<int, IKlient, IList<TDane>> metodaZwracajacaDane = null, Type typLokalizacji = null, Func<int, IKlient, IList<TDane>, object, IList<TDane>> metodaPrzetwarzajacaPoSelect = null)
        {
            lock (_syncRoot)
            {
                if (_slownikMapowan.ContainsKey(typeof(TDane)))
                {
                    throw new InvalidOperationException("Istnieje już mapowanie dla typu " + typeof(TDane).PobierzOpisTypu());
                }

                    bool filtrowanieSQL = true;

                    if (cachowanie == SposobCachowania.CalaLista || metodaZwracajacaDane != null)
                    {
                        filtrowanieSQL = false;
                    }

                ParametrBindowaniaPobieraniaDanych<TDane> parametr = new ParametrBindowaniaPobieraniaDanych<TDane>(cachowanie, walidator, metodaZwracajacaDane, typLokalizacji ?? typeof(TDane).PobierzBazowy(), filtrowanieSQL, metodaPrzetwarzajacaPoSelect);
                _slownikMapowan.Add(typeof(TDane), parametr);
            }
        }

        public object AktualizujWgTypu(object obiekt)
        {
            Type typ = obiekt.GetType();
            string nazwaMetody = "AktualizujPojedynczy";
            if (typ.IsGenericType)
            {
                nazwaMetody = "AktualizujListe";
            }

            MethodInfo metodaGeneryczna = GetType().GetMethods().First(m => m.Name == nazwaMetody);

            MethodInfo generic = metodaGeneryczna.MakeGenericMethod(typ);
            
            return generic.Invoke(this, new[] {obiekt});
        }

        public virtual object AktualizujPojedynczy<TDane>(TDane obiekt) where TDane : class,new()
        {
            AktualizujListe<TDane>(new List<TDane> { obiekt });
            return obiekt.PobierzKlucz();
        }

        public IList<object> AktualizujListe<TDane>(IList<TDane> obiekt) where TDane : class,new()
        {
            if (!obiekt.Any())
            {
                return new List<object>();
            }
            Type typdanych = typeof (TDane);
            List<Delegate> akcjePrzed;
            List<Delegate> akcjeprzedRozroznienie, akcjePoRozroznienieObiekty;
            List<Delegate> akcjePoTylkoNowe;

            Dictionary<TDane, bool> aktualizowanyObiektInfoCzyIstnieje = null;
            _bindingiPrzedAktualizacjaRozroznienieStareIAktualizowane.TryGetValue(typdanych, out akcjeprzedRozroznienie);
            _bindingiPoAktualizacjiRozroznienieStareIAktualizowane.TryGetValue(typdanych, out akcjePoRozroznienieObiekty);
            _bindingiPoAktualizacjiTylkoNoweElementy.TryGetValue(typdanych, out akcjePoTylkoNowe);

            IList<TDane> istniejace = null;

            if (akcjeprzedRozroznienie != null ||  akcjePoRozroznienieObiekty!=null || akcjePoTylkoNowe != null)    //Tylko jeśli jest jakiś binding to wyciągamy info o rozroznieniu
            {
                istniejace = Db.Select<TDane>().ToList();
                List<object> istniejaceKlucze = istniejace.Select(x=> x.PobierzKlucz()).ToList();

                aktualizowanyObiektInfoCzyIstnieje = new Dictionary<TDane, bool>();
                foreach (TDane aktualizowany in obiekt)
                {
                    bool czyIstniejeWBazie = istniejaceKlucze.Contains(aktualizowany.PobierzKlucz() );
                    aktualizowanyObiektInfoCzyIstnieje.Add(aktualizowany, czyIstniejeWBazie);
                }
            }
            if (_bindingiPrzedAktualizacja.TryGetValue(typdanych, out akcjePrzed))
            {
                foreach (Delegate d in akcjePrzed)
                {
                    ((Action<IList<TDane>>) d)(obiekt);
                }
            }
            if (akcjeprzedRozroznienie != null)
            {
                foreach (Delegate d in akcjeprzedRozroznienie)
                {
                    ((Action<IList<TDane>, IList<TDane>>)d)(istniejace,  obiekt );
                }
            }

            //aktualizacje tlumaczenia
            Type typ = typeof(TDane);
            if (typ.Implements(typeof(IPoleJezyk)))
            {
                var tlumaczeniaDoZapisania = new List<object>();
                for (int i = 0; i < obiekt.Count; i++)
                {
                    IPoleJezyk jezyk = (IPoleJezyk)obiekt[i];
                    if (jezyk.JezykId != _solexBllCalosc.Konfiguracja.JezykIDDomyslny)
                    {
                        //czy w ogole jest taki jezyk jak jest w obiekcie
                        if (!_solexBllCalosc.Konfiguracja.JezykiWSystemie.ContainsKey(jezyk.JezykId))
                        {
                            throw new Exception($"Brak języka w systemie o id: {jezyk.JezykId}");
                        }
                        tlumaczeniaDoZapisania.Add(jezyk);
                        obiekt.RemoveAt(i);
                        i--;
                    }
                }
                if (tlumaczeniaDoZapisania.Any())
                {
                    AktualizujLokalizacje< TDane>(tlumaczeniaDoZapisania);
                }
            }

            if (obiekt.Any())
            {
                try
                {
                    this.SaveAll_UzupelnijKlucze(obiekt);
                } catch (Exception e)
                {
                    SolexBllCalosc.PobierzInstancje.Log.Error($"Błąd zapisu SQL: {e.Message}.", e);
                    throw;
                }
            }
            AktualizujWidocznoscDlaKlientow(obiekt);
            WyczyscCache<TDane>(obiekt);

            List<Delegate> akcjePo,akcjePoObiekty;
            if (_bindingiPoAktualizacji.TryGetValue(typdanych, out akcjePo))
            {
                IList<object> klucze = obiekt.Select(x=>x.PobierzKlucz()).ToList();
                foreach (Delegate d in akcjePo)
                {
                    ((Action<IList<object>>) d)(klucze);
                }
            }
            if (_bindingiPoAktualizacjiObiekty.TryGetValue(typdanych, out akcjePoObiekty))
            {
                foreach (Delegate d in akcjePoObiekty)
                {
                    ((Action<IList<TDane>>)d)(obiekt);
                }
            }

            if ( akcjePoRozroznienieObiekty != null || akcjePoTylkoNowe != null)
            {
               // var listaAktualizowaneObiekty = aktualizowanyObiektInfoCzyIstnieje.Where(x => x.Value).Select(x => x.Key).ToList();

                if (akcjePoRozroznienieObiekty != null)
                {
                    foreach (Delegate d in akcjePoRozroznienieObiekty)
                    {
                        ((Action<IList<TDane>, IList<TDane>>) d)(istniejace, obiekt);
                    }
                }

                if (akcjePoTylkoNowe != null)
                {
                    var listaNoweObiekty = aktualizowanyObiektInfoCzyIstnieje.Where(x => !x.Value).Select(x => x.Key).ToList();
                    foreach (Delegate d in akcjePoTylkoNowe)
                    {
                        ((Action<IList<TDane>>) d)(listaNoweObiekty);
                    }
                }
            }

            return obiekt.Select(x=> x.PobierzKlucz()).ToList();            
        }

        private void AktualizujWidocznoscDlaKlientow<TDane>(IList<TDane> obiekt)
        {
            if (!typeof(TDane).Implements(typeof(IObiektWidocznyDlaOkreslonychGrupKlientow)))
            {
                return;
            }

            IList<WidocznosciTypow> widocznosci=new List<WidocznosciTypow>();
            IList<WidocznosciTypow> widocznosciDoUsuwania = new List<WidocznosciTypow>();
            string TypNazwa = typeof(TDane).PobierzOpisTypu();

            foreach (IObiektWidocznyDlaOkreslonychGrupKlientow o in obiekt)
            {
                WidocznosciTypow widocznosc = o.Widocznosc;
              
                //pusty obiekt - nie wypelniony
                if (widocznosc == null || (string.IsNullOrEmpty(widocznosc.Nazwa) && (widocznosc.KategoriaKlientaIdKtorakolwiek == null || widocznosc.KategoriaKlientaIdKtorakolwiek.IsEmpty()) &&
                    (widocznosc.KategoriaKlientaIdWszystkie == null || widocznosc.KategoriaKlientaIdWszystkie.IsEmpty())) )
                {
                    if (widocznosc == null)
                    {
                        widocznosc = new WidocznosciTypow();
                        widocznosc.ObiektId = o.Id;
                        widocznosc.Typ = TypNazwa;
                    }
                    widocznosciDoUsuwania.Add(widocznosc);
                }

                widocznosc.ObiektId = o.Id;
                widocznosc.Typ = TypNazwa;

                //wybór ze wzoru gotowego
                if (!string.IsNullOrEmpty(widocznosc.Nazwa))
                {
                    //  WidocznosciTypow wzorcowy = new WidocznosciTypow();
                    widocznosc.KategoriaKlientaIdKtorakolwiek = null;
                    widocznosc.KategoriaKlientaIdWszystkie = null;
                }
                widocznosci.Add(widocznosc);
            }

            if (widocznosci.Any())
            {
                AktualizujListe(widocznosci);
            }

            if (widocznosciDoUsuwania.Any())
            {
                List<long> ids = widocznosciDoUsuwania.Select(x => x.Id).ToList();
                Usun<WidocznosciTypow, long>(ids);
            }
        }

        //todo: do porawy to wywolanie
        public void AktualizujLokalizacjePojedyncze(object dane)
        {
            Type typ = dane.GetType();
            var metoda = this.GetType().GetMethod("AktualizujLokalizacje").MakeGenericMethod(typ);
            metoda.Invoke(this, new object[] { new List<object> { dane } });
        }

     //   private Dictionary<Type, PropertyInfo[] > _cachePropertisyLokalizacji = new Dictionary<Type, PropertyInfo[]>();

        public void AktualizujLokalizacje<TDane>( IList<object> dane)
        {
            Type typ = typeof(TDane);
            if (!typ.Implements(typeof(IPoleJezyk)) )
            {
                return;
            }

            //czy sa jakies w ogole obiekty w innych jezykach niz domyslne
            if (dane.Any(x => ((IPoleJezyk) x).JezykId != _solexBllCalosc.Konfiguracja.JezykIDDomyslny)){}
            else
            {
                //dla jezykow postawocyh nie ma sensu robic kolenych opeacji
                return;
            }

            ParametrBindowaniaPobieraniaDanych<TDane> bindowanie = PobierzBindowaniaPobieraniaDanych<TDane>();

            string typObietku = bindowanie.TypDoLokalizacji.PobierzOpisTypu();
            PropertyInfo[] lokalizowanePola = typ.Properties(typeof(Lokalizowane)).Values.ToArray();
          
            if (!lokalizowanePola.Any())
            {   //produkt nie ma tłumaczonych pól
                throw  new Exception($"Obiekt wydaje się być tłumaczony (posiada interfejs IPoleJezyk, ale nie ma pól tłumaczonych. Obiekt: {typeof(TDane)}");
            }

            var ackesor = typ.PobierzRefleksja();

            List<Tlumaczenie> slowniki = new List<Tlumaczenie>();
            for (int i = 0; i < dane.Count; i++)
            {
                object o = dane[i];
                IPoleJezyk jezyk = (IPoleJezyk)dane[i];
                bool brakKlucza = false;
                object klucz = o.PobierzKlucz(ref brakKlucza);
                if (brakKlucza)
                {
                    continue;
                }
                long kluczJakoLong = Convert.ToInt64(klucz);
                foreach (PropertyInfo p in lokalizowanePola)
                {
                    object wartosc = ackesor[o, p.Name];
                    if (wartosc == null)
                    {
                        continue;
                    }
                    Tlumaczenie s = new Tlumaczenie();
                    s.ObiektId = kluczJakoLong;
                    s.Pole = p.Name;
                    s.Wpis = wartosc.ToString();
                    s.JezykId = jezyk.JezykId;
                    s.Typ = typObietku;
                    if(p.Name != wartosc.ToString()) slowniki.Add(s);
                }
            }
            if (slowniki.Any())
            {
               this.SaveAll_UzupelnijKlucze(slowniki);
            }
        }

        private void WyczyscCache<TDane>(IList<TDane> dane = null)
        {
            ParametrBindowaniaPobieraniaDanych<TDane> bindowanie = PobierzBindowaniaPobieraniaDanych<TDane>();
            if (bindowanie.SposobCachowania == SposobCachowania.Brak)
            {
                return;
            }

            if (bindowanie.SposobCachowania == SposobCachowania.CalaLista)
            {
                _solexBllCalosc.Cache.UsunGdzieKluczRozpoczynaSieOd(KluczCacheTypDanych<TDane>()); //Czyszczenie standardowego cache
                return;
            }

            if (bindowanie.SposobCachowania == SposobCachowania.TylkoPojedyncze)
            {
                if (dane == null)
                {
                    _solexBllCalosc.Cache.UsunGdzieKluczRozpoczynaSieOd(KluczCacheTypDanych<TDane>()); //Czyszczenie CALEGO cache
                    return;
                }

                //uzyskanie klucza
                foreach (TDane t in dane)
                {
                    object id = t.GetId();
                    _solexBllCalosc.Cache.UsunGdzieKluczRozpoczynaSieOd(KluczCacheTypDanych<TDane>(id));
                }
            }
            return;
        }

        public void UsunPojedynczy<TDane>(object klucz) where TDane : class, new()
        {
            Type typKlucza = klucz.GetType();
            if (typKlucza.IsGenericType)
            {
                throw new Exception("Metoda usuń spodziewała się otrzymać obiekt, ale otrzymała liste.");
            }

            var metodaGeneryczna =  this.GetType().GetMethod("Usun").MakeGenericMethod(typeof(TDane), typKlucza);

            var listaObiektow = Refleksja.StworzListeGeneryczna(typKlucza, new[] {klucz});

            metodaGeneryczna.Invoke(this, new object[] { listaObiektow });
        }

        public void Usun<TDane,T>(List<T> klucz) where TDane : class, new()
        {
            UsunInternal<TDane, T>(klucz, null);
        }

        /// <summary>
        /// Usuwanie kluczy LUB wg. filtrów - nie można podać naraz obu rzeczy
        /// </summary>
        /// <typeparam name="TDane"></typeparam>
        /// <param name="klucze"></param>
        /// <param name="filtr"></param>
        private void UsunInternal<TDane,T>(IList<T> klucze = null, Expression<Func<TDane, bool>> filtr = null) where TDane : class, new()
        {
            if (klucze == null && filtr == null)
            {
                throw new Exception("Brak podanego klucza do usuwania");
            }

            if (klucze != null && filtr != null)
            {
                throw new Exception("Nie można podać naraz dwóch warunków usuwania");
            }

            Type typdanych = typeof(TDane);
            List<Delegate> akcjePrzed;
            if (_bindingiPrzedUsunieciem.TryGetValue(typdanych, out akcjePrzed))
            {
                foreach (Delegate d in akcjePrzed)
                {
                    ((Action<IList<T>>)d)(klucze);
                }
            }
            try
            {
                using (var trans = Db.OpenTransaction())
                {
                    if (klucze != null)
                    {
                        Db.DeleteByIds<TDane>(klucze);
                        WyczyscCache<TDane>(); //wywalamy tylko co sie zmienilo
                    }
                    else
                    {
                        Db.Delete<TDane>(filtr);
                        WyczyscCache<TDane>(); //wywalamy wszystko bo nie wiadomo co sie usunelo
                    }
                    trans.Commit();
                }
            } catch (Exception e)
            {
                if (klucze != null)
                {
                    _solexBllCalosc.Log.Error($"Błąd usuwania obiektu: {typeof(TDane)}, błąd: {e.Message} - dla kluczy: {StringExtensions.ToCsv(klucze)}", e);
                }
                else
                {
                    _solexBllCalosc.Log.Error($"Błąd usuwania obiektu: {typeof(TDane)}, błąd: {e.Message}", e);
                }
                throw new Exception($"Błąd usuwania obiektu: {typeof(TDane)}");
            }

            List<Delegate> akcjePo;
            if (_bindingiPoUsunienciu.TryGetValue(typdanych, out akcjePo))
            {
                foreach (Delegate d in akcjePo)
                {
                    ((Action<IList<T>>)d)(klucze);
                }
            }
            //podczas usuwania tez robimy po AKTUALIACJI - bo to tez jest aktualizacja przeciez
            if (_bindingiPoAktualizacji.TryGetValue(typdanych, out akcjePo))
            {
                foreach (Delegate d in akcjePo)
                {
                    if (klucze!=null)
                    {
                        ((Action<IList<object>>)d)(klucze.Cast<object>().ToList());
                    }
                    
                }
            }
        }
        
        public void UsunWybrane<TDane,T>(Expression<Func<TDane, bool>> filtr) where TDane : class, new()
        {
            UsunInternal<TDane,T>(null, filtr);
        }

        private void DodajBindingInternal<TDane>(Dictionary<Type, List<Delegate>> kolekcja, Delegate akcja)
        {
            Type dodawany = typeof (TDane);
            if (!kolekcja.ContainsKey(dodawany))
            {
                kolekcja.Add(dodawany, new List<Delegate>());
            }
            kolekcja[dodawany].Add(akcja);
        }

        public void  BindPrzedUpdate<TDane>(Action<IList<TDane>> akcja)
        {
            DodajBindingInternal<TDane>(_bindingiPrzedAktualizacja,akcja);
        }

        public void BindPoUpdate<TDane>(Action<IList<object>> akcjaOtrzymujacaKlucze)
        {
            DodajBindingInternal<TDane>(_bindingiPoAktualizacji, akcjaOtrzymujacaKlucze);
        }
        public void BindPoUpdate<TDane>(Action<IList<TDane>> akcjaOtrzymujacaObiekty)
        {
            DodajBindingInternal<TDane>(_bindingiPoAktualizacjiObiekty, akcjaOtrzymujacaObiekty);
        }
        
        public void BindPoUpdateTylkoNoweElementy<TDane>(Action<IList<TDane>> akcjaOtrzymujacaObiekty)
        {
            DodajBindingInternal<TDane>(_bindingiPoAktualizacjiTylkoNoweElementy, akcjaOtrzymujacaObiekty);
        }

        /// <summary>
        /// binding po update, otrzymuje stare obiekty (pierwsza lista) i nowe obiekty (druga lista)
        /// </summary>
        /// <typeparam name="TDane"></typeparam>
        /// <param name="akcjaOtrzymujacaObiekty"></param>
        public void BindPoUpdateRozroznienieStareIAktualizowane<TDane>(Action<IList<TDane>, IList<TDane>> akcjaOtrzymujacaObiekty)
        {
            DodajBindingInternal<TDane>(_bindingiPoAktualizacjiRozroznienieStareIAktualizowane, akcjaOtrzymujacaObiekty);
        }

        /// <summary>
        /// binding przed update, otrzymuje stare obiekty (pierwsza lista) i nowe obiekty (druga lista)
        /// </summary>
        /// <typeparam name="TDane"></typeparam>
        /// <param name="akcja"></param>
        public void BindPrzedUpdateRozroznienieStareIAktualizowane<TDane>(Action<IList<TDane>, IList<TDane>> akcja)
        {
            DodajBindingInternal<TDane>(_bindingiPrzedAktualizacjaRozroznienieStareIAktualizowane, akcja);
        }

        public void BindPrzedUsunieciem<TDane,T>(Action<IList<T>> akcjaOtrzymujacaKlucze)
        {
            DodajBindingInternal<TDane>(_bindingiPrzedUsunieciem, akcjaOtrzymujacaKlucze);
        }

        public void BindPoUsunieciu<TDane,T>(Action<IList<T>> akcjaOtrzymujacaKlucze)
        {
            DodajBindingInternal<TDane>(_bindingiPoUsunienciu, akcjaOtrzymujacaKlucze);
        }

        #region AktualizacjaBazy

        /// <summary>
        /// pobieranie aktualnych nr wersji skrptów
        /// </summary>
        /// <param name="zasoby">Zasoby w których znajdują się skrypty</param>
        /// <param name="folderName">folder w których znajdują się skrypty </param>
        /// <returns></returns>
        public List<decimal> GetAllNrSql(string folderName, string[] listaPlikow)
        {
            var lista = listaPlikow.Where(r => r.EndsWith(".sql")).Select(x => x.Replace(folderName, "").Replace(".sql", "")).ToList();
            HashSet<decimal> wynik = new HashSet<decimal>();
            foreach (var poz in lista)
            {
                decimal nr = 0;
                if (TextHelper.PobierzInstancje.SprobojSparsowac(poz, out nr))
                {
                    wynik.Add(nr);
                }
            }
            return wynik.OrderBy(x => x).ToList();
        }

        private const string SqlUpdateWersjaDb = "EXEC sys.sp_updateextendedproperty  @name = 'wersja', @value = '{0}';";
        private const string SqlWersjaDb = "SELECT value FROM fn_listextendedproperty(default, default, default, default, default, default, default) where name like 'wersja'";
        private const string SqlLastBackUp = @"SELECT DATEDIFF(d, MAX(Backup_Finish_Date), Getdate()) as Last FROM MSDB.dbo.BackupSet WHERE Type = 'd' -- AND database_name like '{0}'";
        private const string SqlCreationDB = @"SELECT DATEDIFF(d, create_date, Getdate()), * FROM sys.databases WHERE name like '{0}'";
        //private const string SqlDbOwner = @"select suser_sname(owner_sid) from sys.databases Where name like '{0}'";
        //private const string SqlCdc = @"SELECT is_cdc_enabled FROM sys.databases where name like '{0}'";
        //private const string OwnerDb = "sa";
        //private const string SqlChangeDbOwner = "EXEC sp_changedbowner '{0}'";
        //private const string SqlCdcEnable = "EXEC sys.sp_cdc_enable_db";
        //private const string SqlCdcDisable = "EXEC sys.sp_cdc_disable_db";
        //private const string SqlCdcChangeRetention = "EXEC sp_cdc_change_job @job_type='cleanup', @retention={0}";
        //private const string SqlCdcRetention = "SELECT retention FROM [msdb].[dbo].[cdc_jobs] WHERE [database_id] = DB_ID() AND [job_type] = 'cleanup'";
        private const string SqlRecoveryModelSelect = "SELECT recovery_model_desc from master.sys.databases WHERE name like '{0}'";
        private const string SqlRecoveryModelChange = "ALTER DATABASE [{0}] SET RECOVERY SIMPLE";
        private const string SqlDodajWersja = "EXEC sp_addextendedproperty @name = N'Wersja', @value = '0';";
        /// <summary>
        /// Aktualizowanie struktury bazy danych do aktualnej wersji platformy 
        /// </summary>
        /// <param name="zasoby">Zasoby w jakich znajdują się skrypty</param>
        public void AktualizujBaze(Assembly zasoby)
        {
            string folderName = $"{zasoby.GetName().Name}.App_Data.skryptySql.";
            var nrWersji = GetAllNrSql(folderName, zasoby.GetManifestResourceNames() );
            if (!nrWersji.Any()) return;

            decimal aktualnaWerSkryptu = nrWersji.Max();

            //pobieranie wersji struktury danych
            decimal wersjaBazy = 0;
            int ostatniBackUp = -1;
            try
            {
                string tmp = Db.Scalar<string>(SqlWersjaDb);
                if (tmp == null) throw new Exception("Brak wyniku odczytu wersji.");
                TextHelper.PobierzInstancje.SprobojSparsowac(tmp, out wersjaBazy);
            }
            catch
            {
                try
                {
                    Db.ExecuteNonQuery(SqlDodajWersja);
                }
                catch (Exception e)
                {
                    throw new Exception($"Nie udało się pobrać wersji bazy danych ani dodać propertisa. Dodaj Properties [wersja] w Extended Properties  bazy danych. {e.Message}");
                }
            }

            //sprawdzamy uzytkownika bazy
            //wywalam jak będzie potrzebna to wtedy dodam 
            //string oldOwner = string.Empty;
            //try
            //{
            //    var tmp = Db.Scalar<string>(string.Format(SqlDbOwner, Db.Database));
            //    if (tmp == null) throw new Exception("Brak wyniku odczytu usera.");
            //    oldOwner = tmp;
            //    if (!oldOwner.Contains(OwnerDb))
            //    {
            //        Db.ExecuteNonQuery(string.Format(SqlChangeDbOwner, OwnerDb));
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw new Exception(string.Format("Nie udało się zmienić użytkownika bazy danych z {0} na {1}. Zmiana jest potrzebna do dalszego działania. {2}", oldOwner, OwnerDb, e.Message));
            //}

            //rezygnujemy z CDC
            //włączanie mechanizmu przechwytywania zmian na bazie
            //bool czyCdcWlaczoneNaBazie = false;
            //try
            //{
            //    czyCdcWlaczoneNaBazie = Db.Scalar<bool>(string.Format(SqlCdc, Db.Database));
            //    if (!czyCdcWlaczoneNaBazie)
            //    {
            //        Db.ExecuteNonQuery(SqlCdcEnable);
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw new Exception($"Nie udało się właczyć Mechanizmu do przechwytywania zmian danych na bazie danych. {e.Message}");
            //}


            //rezygnujemy z CDC
            //// przymusowe wyłączanie CDC by przechowywać zmiany - skrypty startowe
            //if (wersjaBazy > 30.2m && wersjaBazy<32.318m) WywolajSkrypt(zasoby, folderName, "start");

            ////Wyłączamy cdc bo stwarza za dużo problemów ale żeby nie przepisywać skryptów wyłączamy od wersji skryprów 32.318 dla wcześniejszych niech jeszcze będzie właczone 
            //if (wersjaBazy > 32.317m)
            //{
            //    try
            //    {
            //        if (czyCdcWlaczoneNaBazie)
            //        {
            //            Db.ExecuteNonQuery(SqlCdcDisable);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        throw new Exception($"Nie udało się wyłaczyć Mechanizmu do przechwytywania zmian danych na bazie danych. {e.Message}");
            //    }
            //}


            //sprawdznaie wersji bazy 
            if (aktualnaWerSkryptu > wersjaBazy)
            {
                //sprawdzanie czy był zrobiony backUp bazy 
                try
                {
                    var tmp = Db.Scalar<int?>(string.Format(SqlLastBackUp, Db.Database));
                    if (tmp == null) throw new Exception("Brak wyniku odczytu ostatniego backup-u bazy.");
                    ostatniBackUp = tmp.Value;
                }
                catch (Exception e)
                {
                    throw new Exception($"Błąd odczytu daty ostatniego Backup bazy danych. {e.Message}");
                }
              
                if (ostatniBackUp != 0)
                {
                    int dniOdUtworzenia = -1;
                    try
                    {
                        var tmp = Db.Scalar<int?>(string.Format(SqlCreationDB, Db.Database));
                        if (tmp == null) throw new Exception("Brak wyniku odczytu daty utworzenia bazy.");
                        dniOdUtworzenia = tmp.Value;
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Błąd odczytu daty utworzenia bazy. {e.Message}");
                    }
                    if (dniOdUtworzenia > 0)
                    {
                        throw new Exception("Niestety dzis nie był zrobiony BACKUP Bazy danych. Przed aktualizacją bazy danych proszę o zrobienie BACKUP-u.");
                    }
                }

                //pętla aktualizująca bazę do aktualnej wersji
                foreach (decimal t in nrWersji)
                {
                    if (t > wersjaBazy)
                    {
                        WywolajSkryptAktualizacji(zasoby, folderName, t, wersjaBazy);
                    }
                }
            }

            //sprawdzanie ilości dni przechowywania zmian 
            //ZANIM to zrobimy trzeba sprawdizć czy CDC w ogole jest wlaczone
            //long czasPrzechowywania = -1;
            //try
            //{
            //    czasPrzechowywania = Db.Scalar<long>(SqlCdcRetention);
            //}
            //catch (Exception e)
            //{
            //    throw new Exception(string.Format("Nie udało się pobrać czasu przechowywania zmian. {0}",e.Message));
            //}
            //long czasZUstawien = SolexBllCalosc.PobierzInstancje.Konfiguracja.CzasPrzechowywaniaZmian*24*60;
            //if (czasZUstawien != czasPrzechowywania)
            //{
            //    string sql = string.Format(SqlCdcChangeRetention, czasZUstawien);
            //    try
            //    {
            //        Db.ExecuteNonQuery(sql);
            //    }
            //    catch (Exception e)
            //    {
            //        throw new Exception(string.Format("Nie udało się zmienić czas przechowywania zmian. {0}", e.Message));
            //    }
            //}
        }

        /// <summary>
        /// Wywołanie aktualizacji bazy do wybranej wersji skryptu
        /// </summary>
        /// <param name="zasoby">Zasoby w jakim są przechowywane skrypty</param>
        /// <param name="folderName">folder ze skryptami</param>
        /// <param name="doJakiejAktualizowac">wersja do jakiej chcemy aktualizować</param>
        /// <param name="wersjaBazy"> aktualna wersja bazy</param>
        private void WywolajSkryptAktualizacji(Assembly zasoby, string folderName, decimal doJakiejAktualizowac, decimal wersjaBazy)
        {
            try
            {
                WywolajSkrypt(zasoby, folderName, doJakiejAktualizowac.ToString().Replace(".", ","));
                AktualizujWersje(doJakiejAktualizowac);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Błąd aktualizacji bazy danych z: {0} na: {1}\r\n{2}", wersjaBazy ,doJakiejAktualizowac, e.Message));
            }
        }
        /// <summary>
        /// wywałanie skryptu z zasobów asembly
        /// </summary>
        /// <param name="zasoby">Zasoby w jakim są przechowywane skrypty</param>
        /// <param name="folderName">folder ze skryptami</param>
        /// <param name="nazwaSkryptu">nazwa skryptu</param>
        private void WywolajSkrypt(Assembly zasoby, string folderName,string nazwaSkryptu)
        {
            var skryptFs = zasoby.GetManifestResourceStream(folderName + nazwaSkryptu + ".sql");

            if (skryptFs == null) throw new Exception(string.Format("Nie można odczytać skryptu."));
            string skrypt;
            using (StreamReader reader = new StreamReader(skryptFs))
            {
                skrypt = reader.ReadToEnd();
            }
            skryptFs.Close();
            using (var transaction = Db.OpenTransaction())
            {
                try
                {
                    Db.ExecuteNonQuery(skrypt);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(string.Format("Błąd wykonywania skryptu: {0}\r\n{1}", nazwaSkryptu, e.Message));
                }
            }
        }
        /// <summary>
        /// Aktualizacja wersji struktury danych
        /// </summary>
        /// <param name="doJakiejAktualizowac">aktualna wersja</param>
        private void AktualizujWersje(decimal doJakiejAktualizowac)
        {
            using (var transaction = Db.OpenTransaction())
            {
                try
                {
                    Db.ExecuteNonQuery(string.Format(SqlUpdateWersjaDb, doJakiejAktualizowac));
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(string.Format("Błąd aktualizacji wersji bazy danych na: {0}\r\n{1}", doJakiejAktualizowac, e.Message));
                }
            }
        }

        #endregion

        #region ObslugaZmianCDC
        public Dictionary<string, List<ZmianaObiektu>> PobierzZmianyObiektuWgTypu(Type typ, object id)
        {
            MethodInfo[] metodaGeneryczna = GetType().GetMethods().Where(m => m.Name == "PobierzZmianyObiektu").ToArray();
            foreach (MethodInfo mi in metodaGeneryczna)
            {
                MethodInfo generic = mi.MakeGenericMethod(typ);
                return (Dictionary<string, List<ZmianaObiektu>>)generic.Invoke(this, new [] { id });
            }
            return null;
        }

        public bool CzyJestWlaczonyZapisZmiany(Type typ, object id)
        {
            MethodInfo[] metodaGeneryczna = GetType().GetMethods().Where(m => m.Name == "CzyJestWlaczonyZmiany").ToArray();
            foreach (MethodInfo mi in metodaGeneryczna)
            {
                MethodInfo generic = mi.MakeGenericMethod(typ);
                return (bool)generic.Invoke(this, new[] { id });
            }
            return false;
        }

        public bool CzySaZmianyNaObiekcie(Type typ, object id)
        {
            MethodInfo[] metodaGeneryczna = GetType().GetMethods().Where(m => m.Name == "CzySaZmiany").ToArray();
            foreach (MethodInfo mi in metodaGeneryczna)
            {
                MethodInfo generic = mi.MakeGenericMethod(typ);
                return (bool)generic.Invoke(this, new[] { id });
            }
            return false;
        }

        public bool CzyJestWlaczonyZmiany<TDane>(object id)
        {
            var definicja = ModelDefinition<TDane>.Definition;
            string tabela = definicja.ModelName;
            return SprawdzAktywnoscZmian(tabela);
        }

       
        public bool SprawdzAktywnoscZmian(string tabela)
        {
            var wynik = Db.CreateCommand();
            wynik.CommandText =
                string.Format(@"SELECT [Name] FROM sys.tables WHERE is_tracked_by_cdc=1 AND Name like '{0}'", tabela);
            using (var r = wynik.ExecuteReader())
            {
                while (r.Read())
                {
                    return true;
                }
            }
            return false;
        }

        ///// <summary>
        ///// Wyłącza CDC na tabeli
        ///// </summary>
        ///// <param name="nazwa">Nazwa tabeli</param>
        //private void WylaczCdc(string nazwa, out bool czyByloCdc)
        //{
        //    czyByloCdc = false;
        //    try
        //    {
        //        if (SprawdzAktywnoscZmian(nazwa))
        //        {
        //            string sql =
        //                string.Format("EXEC sys.sp_cdc_disable_table N'dbo' ,'{0}', @capture_instance= 'dbo_{0}'", nazwa);
        //            Db.ExecuteSql(sql);
        //            czyByloCdc = true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(string.Format("Bład przy wyłączaniu monitorowani zmian. {0},{1}",e.Message,e.InnerException));
        //    }
        //}

        ///// <summary>
        ///// Włącza CDC na tabeli
        ///// </summary>
        ///// <param name="nazwa">Nazwa tabeli</param>
        //private void WlaczCdc(string nazwa)
        //{   try
        //    {
        //            string sql =
        //                string.Format("EXEC sys.sp_cdc_enable_table N'dbo' ,'{0}',@role_name = NULL ,@supports_net_changes = 1", nazwa);
        //            Db.ExecuteSql(sql);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(string.Format("Bład przy włączaniu monitorowani zmian. {0},{1}", e.Message, e.InnerException));
        //    }
        //}

        public void WyczyscTabele(string nazwa)
        {
            using (var trans = Db.OpenTransaction())
            {
                Db.ExecuteSql(string.Format("DELETE FROM {0}", nazwa));
                trans.Commit();
            }
        }

        public bool CzySaZmiany<TDane>(object id = null)
        {
            var definicja = ModelDefinition<TDane>.Definition;
            string tabela = definicja.ModelName;

            string klucz = definicja.PrimaryKey.FieldName;
            string where = "";
            if (id != null)
            {
                object esc = definicja.PrimaryKey.FieldType == typeof (string) ? string.Format("'{0}'", id) : id;
                where = string.Format("where {0}={1}", klucz, esc);
            }
            var wynik = Db.CreateCommand();
            wynik.CommandText =
                string.Format(@"select t.tran_begin_time, dane.* from cdc.dbo_{0}_CT dane 
                        join cdc.lsn_time_mapping t on dane.__$start_lsn=t.start_lsn {1}",tabela, where);

            using (var r = wynik.ExecuteReader())
            {
                while (r.Read())
                {
                    return true;
                }
            }
            return false;
        }

        public bool PrzywrocWersje(Type typ, string data, string znacznik)
        {
            MethodInfo[] metodaGeneryczna = GetType().GetMethods().Where(m => m.Name == "PrzywrocDane").ToArray();
            foreach (MethodInfo mi in metodaGeneryczna)
            {
                MethodInfo generic = mi.MakeGenericMethod(typ);
                return (bool)generic.Invoke(this, new[] { data, znacznik });
            }
            return false;
        }


        public bool PrzywrocDane<TDane>(string data, string znacznik)
        {
            var definicja = ModelDefinition<TDane>.Definition;
            string tabela = definicja.ModelName;
            bool wynik = PrzywrocDaneZWersje(data, znacznik, tabela);
            WyczyscCache<TDane>();  //wszystk przeladowac trzeba
            return wynik;

        }

        private bool PrzywrocDaneZWersje(string data, string znacznik, string tabela)
        {
            var wynik = Db.CreateCommand();
            wynik.CommandText = string.Format(@"select dane.* from cdc.dbo_{0}_CT dane join cdc.lsn_time_mapping t on dane.__$start_lsn=t.start_lsn WHERE convert(varchar(MAX),tran_begin_time ,20) ='{1}' AND __$operation = 3", tabela, DateTime.Parse(data));
            List<string> columnyZmian = new List<string>();
            string wersja = "";
            try
            {
                using (var r = wynik.ExecuteReader())
                {
                    while (r.Read())
                    {
                        wersja = BitConverter.ToString((byte[]) r["__$seqval"]);
                        for (int i = 0; i < r.FieldCount; i++)
                        {
                            string pole = r.GetName(i);
                            if (pole.StartsWith("_") || pole=="Id")
                            {
                                continue;
                            }
                            object wartosc = r.IsDBNull(i) ? null : r[i];
                            if (wartosc == null) continue;
                            columnyZmian.Add("k." + pole + " = s."+pole);
                        }
                    }
                }

                if (columnyZmian.Any() && wersja == znacznik)
                {
                    string tmp = columnyZmian.Aggregate((a, b) => a + ", " + b);
                    string sql =
                        string.Format("Update k set {0}  " +
                                      "from {1} k " +
                                      "inner join  [cdc].[dbo_{1}_CT] s on  k.id = s.id " +
                                      "join cdc.lsn_time_mapping t on s.__$start_lsn=t.start_lsn " +
                                      "WHERE convert(varchar(MAX),tran_begin_time ,20) ='{2}' AND __$operation = 3", tmp,
                            tabela, DateTime.Parse(data));
                    wynik.CommandText = sql;
                    wynik.ExecuteNonQuery();
                }
            }
            catch
            {
                throw new Exception("Bład przywracania wersji.");
            }
           return true;
        }
        public Dictionary<string, List<ZmianaObiektu>> PobierzZmianyObiektu<TDane>(object id)
        {
            var definicja = ModelDefinition<TDane>.Definition;
            string tabela=  definicja.ModelName;
            
            string klucz = definicja.PrimaryKey.FieldName;
            string where = "";
            if (id != null)
            {
                object esc = definicja.PrimaryKey.FieldType == typeof(string) ? string.Format("'{0}'", id) : id;
                where = string.Format("where  {0}={1}", klucz, esc);
            }
           // object esc = definicja.PrimaryKey.FieldType == typeof (string) ? string.Format("'{0}'", id) : id;
            Dictionary<string, Dictionary<string, ZmianaObiektu>> zmianyWersja = new Dictionary<string, Dictionary<string, ZmianaObiektu>>();

                var wynik = Db.CreateCommand();
                wynik.CommandText = string.Format(@"select dataWykonaniaZmiany=t.tran_begin_time, dane.* from cdc.dbo_{0}_CT dane 
                        join cdc.lsn_time_mapping t on dane.__$start_lsn=t.start_lsn {1} ORDER BY t.tran_begin_time DESC", tabela, where);

            using (var r=   wynik.ExecuteReader())
            {
                while (r.Read())
                {
                    int typ = (int) r["__$operation"];
                    string wersja = BitConverter.ToString((byte[])r["__$seqval"]);
                    DateTime data = (DateTime)r["dataWykonaniaZmiany"];
                    string IdObiektu = r["Id"].ToString();
                    if (!zmianyWersja.ContainsKey(wersja))
                    {
                        zmianyWersja.Add(wersja, new Dictionary<string, ZmianaObiektu>());
                    }
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        string pole = r.GetName(i);
                        if (pole.StartsWith("_") || pole == "dataWykonaniaZmiany") 
                        {
                            continue;
                        }
                        ZmianaObiektu zmiana;
                        if (!zmianyWersja[wersja].ContainsKey(pole))
                        {
                            zmiana = new ZmianaObiektu { IdentyfikatorWersji = wersja, Pole = pole, DataZmiany = data , IdentyfikatoObiektu =IdObiektu, RodzajZmiany = typ, Tabela = tabela};
                            zmianyWersja[wersja].Add(pole, zmiana);
                        }
                        else
                        {
                            zmiana = zmianyWersja[wersja][pole];
                        }
                        object wartosc =r.IsDBNull(i)?null: r[i];

                        if (typ == 3)
                        {
                            zmiana.StaraWartosc = wartosc;
                        }
                        else
                        {
                            zmiana.NowaWartosc = wartosc;
                        }
                    }
                }
            }
          
          return zmianyWersja.Values.SelectMany(x => x.Values).Where(x =>!(x.NowaWartosc??"").ToString().Equals((x.StaraWartosc??"").ToString())).GroupBy(x=>x.IdentyfikatorWersji).ToDictionary(x=>x.Key,x=>x.ToList());
        }

        private Dictionary<Type, Tuple<string, string>> nazwyTabelAliasow = new Dictionary<Type, Tuple<string, string>>(50);

        public long GetAutoID(Type typ)
        {
            if (!nazwyTabelAliasow.ContainsKey(typ))
            {
                string nazwaTabeli = typ.GetCustomAttributes(typeof(AliasAttribute), true).FirstOrDefault() == null ? typ.Name
                    : ((AliasAttribute)typ.GetCustomAttributes(typeof(AliasAttribute), true).FirstOrDefault()).Name;

                string klucz = "Id";    //bartek zmiana na ID

                nazwyTabelAliasow.Add(typ, new Tuple<string, string>(nazwaTabeli, klucz));
            }
            string zapytanie = null;
            //sql server ma ISNULL składnie, sqlite ma IFNULL
            if (DbFactory.DialectProvider is SqliteOrmLiteDialectProvider)
            {
                zapytanie = string.Format("select ifnull(MIN({1}),0)-1 from {0} where {1}<0", nazwyTabelAliasow[typ].Item1, nazwyTabelAliasow[typ].Item2);
                return DbORM.QuerySingle<long>(zapytanie);
            }

            zapytanie = string.Format("select isnull(MIN({1}),0)-1 from {0} where {1}<0", nazwyTabelAliasow[typ].Item1, nazwyTabelAliasow[typ].Item2);
            return DbORM.QuerySingle<long>(zapytanie);
        }


        public Dictionary<string, long> TabelaOrazJejRozmiar()
        {
            string zapytanie = string.Format("SELECT o.NAME as nazwa,  i.rowcnt as iloscRekordow FROM sysindexes AS i  INNER JOIN sysobjects AS o ON i.id = o.id WHERE i.indid < 2  AND OBJECTPROPERTY(o.id, 'IsMSShipped') = 0 ORDER BY o.NAME");

            return DbORM.Dictionary<string, long>(zapytanie);
        }

        private SqlHelper sqlHelper = new SqlHelper();
        private Dictionary<Type, DaneOKluczu> kluczeTypow = new Dictionary<Type, DaneOKluczu>(50);

        public class DaneOKluczu
        {
            public Type TypKlucza { get; set; }
            public PropertyInfo PropertyKlucz { get; set; }
            public bool AutoInkrementacjaKlucza { get; set; }
            public bool SprawdzajPrzedDodaniemCzyIstniejeObiektOIdentycznychDanych { get; set; }

            public bool CzyUstawiacKluczPrzedZapisem()
            {
                return (this.PropertyKlucz != null && this.PropertyKlucz.CanWrite && (this.PropertyKlucz.PropertyType == typeof(int) || this.PropertyKlucz.PropertyType == typeof(long)) ) //glupi warunek bartek wywala: this.GeterKlucza == null &&
                    && (!AutoInkrementacjaKlucza || SprawdzajPrzedDodaniemCzyIstniejeObiektOIdentycznychDanych);    //zeby ustawiac klucz musi byc autoinkremtacja albo sprawdzanie juz czy jest obiekt
            }
        }

        /// <summary>
        /// metoda zpaisuje do bazy danych obiekty. Jesli ID = 0 sprawdza czy juz jest taki obiekt, jesli nie to uzupelnia identyfikator - UJEMNY, chyba ze jest autoinkrement - wtedy zostawia 0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        public void SaveAll_UzupelnijKlucze<T>(IEnumerable<T> objs) where T : new()
        {
            if (!objs.Any())
            {
                throw new Exception("Próba zapisu zero elementów");
            }

            DaneOKluczu daneKlucza = null;

            if (!kluczeTypow.TryGetValue(typeof(T), out daneKlucza))
            {
                daneKlucza = new DaneOKluczu();
                PropertyInfo klucz = null;
                //BARTEK zmienia bo że klucz musi mieć nazwe Id - i tak mamy interfejs IHaId<T> itp.
                typeof(T).Properties().TryGetValue("Id", out klucz);  //Refleksja.WyszukajPropertisaZAtrybutem(typeof(T), typeof(PrimaryKeyAttribute), out kluczGet, out kluczSet);
                daneKlucza.PropertyKlucz = klucz;
                //daneKlucza.GeterKlucza = kluczGet;
                //daneKlucza.SeterKlucza = kluczSet;

                //if (daneKlucza.PropertyKlucz == null)
                //{
                //    daneKlucza.PropertyKlucz = typeof(T).GetProperty("Id");
                //}

                if (daneKlucza.PropertyKlucz != null)
                {
                    daneKlucza.TypKlucza = daneKlucza.PropertyKlucz.PropertyType;
                }

                daneKlucza.SprawdzajPrzedDodaniemCzyIstniejeObiektOIdentycznychDanych = typeof(T).GetCustomAttributes(typeof(NieSprawdzajCzyIsnieje), true).FirstOrDefault() == null;

                if (daneKlucza.PropertyKlucz != null)
                {
                    daneKlucza.AutoInkrementacjaKlucza = daneKlucza.PropertyKlucz.GetCustomAttributes(typeof(AutoIncrementAttribute), true).FirstOrDefault() != null;
                }

                kluczeTypow.Add(typeof(T), daneKlucza);
            }

            if (daneKlucza.CzyUstawiacKluczPrzedZapisem())
            {
                TypeAccessor reflektor = typeof(T).PobierzRefleksja();
                long minimalneIDDoWlasnegoNadawanieID = 0;
                foreach (T obj in objs)
                {
                    long kluczObecny;

                    if (daneKlucza.TypKlucza == typeof(long))
                    {
                        kluczObecny = (long)reflektor[obj, daneKlucza.PropertyKlucz.Name];
                    }
                    else
                    {
                        kluczObecny = (int)reflektor[obj, daneKlucza.PropertyKlucz.Name];
                    }

                    //klucz == 0 - dodanie nowego klucza
                    if (kluczObecny == 0)
                    {
                        if (daneKlucza.SprawdzajPrzedDodaniemCzyIstniejeObiektOIdentycznychDanych)
                        {
                            T zwroconyObiekt = default(T);
                            zwroconyObiekt = this.DbORM.FirstOrDefault<T>(sqlHelper.ZbudujWarunekSQLDlaObiektu(obj, reflektor));

                            if (zwroconyObiekt != null)
                            {
                                object o = reflektor[zwroconyObiekt, daneKlucza.PropertyKlucz.Name];
                                if (daneKlucza.TypKlucza == typeof(int))
                                {
                                    reflektor[obj, daneKlucza.PropertyKlucz.Name] = (int) o;
                                }
                                else
                                {
                                    reflektor[obj, daneKlucza.PropertyKlucz.Name] = o;
                                }

                                //konczymy z tym obiektem i kolejny
                                continue;
                            }
                        }

                        //jesli nie ma go jeszcze w bazie albo nie sprawdzamy - I NIE ma atoinkremtenacji - czyli sami musimy uzupelnic klucze - tylko dodajmy klucze UJEMNE - dlatego odejmujemy caly czas
                        if (!daneKlucza.AutoInkrementacjaKlucza)
                        {
                            if (minimalneIDDoWlasnegoNadawanieID == 0)
                            {
                                minimalneIDDoWlasnegoNadawanieID = this.GetAutoID(typeof(T));
                            }
                            kluczObecny = minimalneIDDoWlasnegoNadawanieID--;
                            if (daneKlucza.TypKlucza == typeof(int))
                            {
                                reflektor[obj, daneKlucza.PropertyKlucz.Name] = (int)kluczObecny;
                            }
                            else
                            {
                                reflektor[obj, daneKlucza.PropertyKlucz.Name] = kluczObecny;
                            }
                        }
                    }
                }
            }

            try
            {
                using (var trans = DbORM.OpenTransaction())
                {
                    DbORM.SaveAll<T>(objs);
                    trans.Commit();
                }
            } catch (Exception e)
            {
                _solexBllCalosc.Log.Error($"Błąd zapisu do bazy danych: {e.Message}.");
                throw;
            }
            finally
            {
                //zwalniamy polaczenie do bazy danych bo bylo duzo klopotow z tym 
               // this.ZamknijPolaczenieDoBazy();
            }

        }



        #endregion

        /// <summary>
        /// czysty seelct za pomoca DB ORMa
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<T> Select<T>(Expression<Func<T, bool>> predicate)
        {
            return DbORM.Select<T>(predicate);
        }


        public void DEBUG_ZliczWYkorzystanieFunkcjiWRequest(string symbolUstawienia, int limitIle = 20)
        {
            //tylko dla aplikacji webowych
            if (HttpContext.Current != null)
            {
                symbolUstawienia = "__" + symbolUstawienia;
                long iloscUruchomien = 0;
                if (HttpContext.Current.Items[symbolUstawienia] != null)
                {
                    iloscUruchomien = (long)HttpContext.Current.Items[symbolUstawienia];
                }

                ++iloscUruchomien;
                if (iloscUruchomien > limitIle)
                {
                    throw new Exception($"Komponent: {symbolUstawienia} jest wywołany zbyt często - ponad {limitIle} razy w request! Popraw wywołania.");
                }
                HttpContext.Current.Items[symbolUstawienia] = iloscUruchomien;
            }
        }


    }
}
