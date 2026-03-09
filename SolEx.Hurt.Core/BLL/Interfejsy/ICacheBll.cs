using System;
using System.Collections.Generic;
using System.Reflection;
using FastMember;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface ICacheBll
    {
        void UsunObiekt(string klucz, params object[] parametryKlucza);
        void DodajObiekt(string klucz, object obiekt, params object[] parametryKlucza);
        T PobierzObiekt<T>(string klucz, params object[] parametryKlucza);
        object PobierzObiekt(string klucz, params object[] parametryKlucza);
        void UsunGdzieKluczRozpoczynaSieOd(string poczatek);
        bool ZawieraKluczRozpoczynaSieOd(string poczatek);
        void WyczyscCache();
        T PobierzChwilowy<T>(string klucz);
        void DodajChwilowy(string klucz, object dane);
        bool JestObiektChwilowy(string klucz);
        T PobierzObiekt<T>(Func<T> metodaDoPobraniaWartosci, object lok, string klucz, params object[] parametryKlucza);
        void InicjalizujPrywatnySlownik( List<string> typyDlaKtorychInicjalizacja  );
        T SlownikPrywatny_PobierzObiekt<T>(Func<long, object> metodaDoPobraniaWartosci, int jezykId, long klucz, string typDanych);
        void PlikowyCache_wyczyscWszystkoStare(string[] typyDoInicjalizacji);
        string PlikowyCache_PobierzObiekt(Func<string> metodaDoPobieraniaWartosci, object lok, int jezykId, string klucz, string typDanych, int czasCache = 30*60);
        string WyliczKluczDlaKategorii(int idKontrolki, IKlient klient, string szukane);
        string WyliczKluczDlaMenu(int idKontrolki, long idKlienta, int idJezyka, bool czyPerKlient);

        /// <summary>
        /// metoda dodaje do cache, ale wyd³u¿a czas trzymania w cache - stosowaæ tylko dla krytycznych obiektów które powinny byæ d³ugo trzymane w cache
        /// </summary>
        /// <param name="klucz"></param>
        /// <param name="obiekt"></param>
        /// <param name="parametryKlucza"></param>
        void DodajObiekt_Kluczowy(string klucz, object obiekt, params object[] parametryKlucza);

        void InternujStringiWObiekcie(object obiekt, TypeAccessor akcesor, Dictionary<string, PropertyInfo> polaDoInternacji);
    }
}