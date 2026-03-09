using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class LockHelper
    {
        public ISolexBllCalosc solexBllCalosc = SolexBllCalosc.PobierzInstancje;

        public static LockHelper PobierzInstancje { get; } = new LockHelper();

        private readonly ConcurrentDictionary<string, object> _lockObjs = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// metoda dodaje do cache pod dany klucz pobrana wartosc z metody - metoda NIE może dodawać sama do cache !
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="klucz"></param>
        /// <param name="funkcja"></param>
        /// <param name="timeoutMiliSecound"></param>
        /// <param name="uzywajCache">jeśli nie to nie jest używany cache (odczyt / zapis)</param>
        /// <returns></returns>
        private T _pobierzDaneZCache_zLockiem<T>(string klucz, Func<T> funkcja, int timeoutMiliSecound = 9500, bool uzywajCache = true)
        {
            //!!! duza zmiana 08.2024 !!
            //BCH: !!!!! proboje pobrac z cache - jak jest to nie daje locka tylko pobieram i zwracam - locki sa dopiero jak nie ma obiektu w cache
            object rezultat = solexBllCalosc.Cache.PobierzObiekt(klucz);
            if (rezultat != null)
            {
                return (T)rezultat;
            }
            
            //BCH - wczesniej byl inny klucz do cache i inny klucz do locka - do locka był taki: string kluczLocka = $"PobierzInternal_{typeof(TDane).PobierzOpisTypu()}"; 
            bool lockWasTaken = false;

            //pobranie locka z kolekcji kluczy
            object lockObj = _lockObjs.GetOrAdd(klucz, x => new object());

            try
            {
                Monitor.TryEnter(lockObj, timeoutMiliSecound, ref lockWasTaken);
                if (lockWasTaken)
                {
                    //pobranie z cache o ile jest wg. klucza
                    rezultat = solexBllCalosc.Cache.PobierzObiekt(klucz);

                    if (rezultat != null)
                    {
                        return (T)rezultat;
                    }

                    rezultat = (T)funkcja();

#if DEBUG
                    if (rezultat == null)
                    {
                        throw new Exception("Metoda krytyczna nie może zwracać NULLA bo wtedy do cache nie można dodać wartości - zmień metode na nie krytyczną lub zmien zeby zwraca cośkolwiek");
                    }

                    if (solexBllCalosc.Cache.PobierzObiekt(klucz) != null)
                    {
                        throw new Exception($"Metoda nie może sama dodawać do cache - a widocznie dodała klucz: {klucz}");
                    }
#endif

                    //dodanie rezultatu do cache
                    solexBllCalosc.Cache.DodajObiekt_Kluczowy(klucz, rezultat);
                    return (T)rezultat;
                }
                else
                {
                    throw new Exception($"Zadanie trwało zbyt długo zostaje przerwane - klucz zadania: {klucz}. timeout ms: {timeoutMiliSecound}, uzywajCache: {uzywajCache}. \r\n " +
                                        $"Ostatnie zapytanie SQL: {solexBllCalosc.DostepDane.DbORM.GetLastSql()}. \r\n Kolekcja kluczy zadan: {  string.Join(",", _lockObjs.Keys ) }.");
                }
            }
            finally
            {
                _lockObjs.TryRemove(klucz, out _);
                if (lockWasTaken)
                {
                    Monitor.Exit(lockObj);
                }
            }
        }


        /// <summary>
        /// Metoda która tworzy locka i uruchamia funkcje zwracająca wynik T - jeżeli wiesz, że w synchronicznych wywołaniach będziesz pobierał dane z bazy danych pamiętaj o tym aby zamknąć połącznie do bazy danych
        /// </summary>
        /// <param name="klucz"></param>
        /// <param name="funkcja"></param>
        /// <param name="timeoutSecound"></param>
        /// <param name="httpContext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T PobierzDaneWLocku_zUcyciemCache<T>(string klucz, Func<T> funkcja, int timeoutSecound = 93)
        {
            //nowa metoda - bch testuje
            return this._pobierzDaneZCache_zLockiem(klucz, funkcja, timeoutSecound * 1000, true);
        }

        /// <summary>
        /// Metoda która tworzy locka i uruchamia funkcje zwracająca wynik T - jeżeli wiesz, że w synchronicznych wywołaniach będziesz pobierał dane z bazy danych pamiętaj o tym aby zamknąć połącznie do bazy danych
        /// </summary>
        /// <param name="klucz"></param>
        /// <param name="funkcja"></param>
        /// <param name="timeoutSecound"></param>
        /// <param name="httpContext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T UruchomKodWLocku_BezUzywaniaCache<T>(string klucz, Func<T> funkcja, int timeoutSecound = 95)
        {
            //pobranie locka
            object lockObj = _lockObjs.GetOrAdd(klucz, x => new object());
            bool lockWasTaken = false;

            try
            {
                Monitor.TryEnter(lockObj, timeoutSecound * 1000, ref lockWasTaken);
                if (lockWasTaken)
                {
                    return funkcja();
                }
                else
                {
                    throw new Exception($"Zadanie trwało zbyt długo zostaje przerwane - klucz zadania: {klucz}. timeout sek: {timeoutSecound}" +
                                        $"Ostatnie zapytanie SQL: {solexBllCalosc.DostepDane.DbORM.GetLastSql()}.");
                }
            }
            finally
            {
                if (lockWasTaken)
                {
                    Monitor.Exit(lockObj);
                }
            }
        }

        /// <summary>
        /// uruchamia kod w locku
        /// </summary>
        /// <param name="klucz"></param>
        /// <param name="funkcja"></param>
        /// <param name="timeoutSecound"></param>
        public void UruchomKodWLocku_BezUzywaniaCache(string klucz, Action funkcja, int timeoutSecound = 96)
        {
            //pobranie locka
            object lockObj = _lockObjs.GetOrAdd(klucz, x => new object());
            bool lockWasTaken = false;

            try
            {
                Monitor.TryEnter(lockObj, timeoutSecound * 1000, ref lockWasTaken);
                if (lockWasTaken)
                {
                    funkcja();
                }
                else
                {
                    throw new Exception($"Zadanie trwało zbyt długo zostaje przerwane - klucz zadania: {klucz}. timeout sek: {timeoutSecound} " +
                                        $"Ostatnie zapytanie SQL: {solexBllCalosc.DostepDane.DbORM.GetLastSql()}.");
                }
            }
            finally
            {
                if (lockWasTaken)
                {
                    Monitor.Exit(lockObj);
                }
            }
        }



    }
}