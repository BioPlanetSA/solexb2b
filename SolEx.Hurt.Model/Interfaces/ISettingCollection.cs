using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface ISettingCollection
    {
        void AddSetting(Ustawienie set);
        void AddSettings(IEnumerable<Ustawienie> set);

        HashSet<TZwaracany> GetSettingReflekcja<T, TZwaracany>(string symbol, HashSet<TZwaracany> defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, bool multi = false, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, HashSet<TZwaracany> domyslnaWartoscNiezalogowani = null);
        HashSet<T> GetSettingSlownik<T>(string symbol, HashSet<T> defaultValue, IEnumerable<T> dostepneOpcje, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, bool multi = false, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, bool dynamiczne = false,string podgrupaTekstowa=null);

        HashSet<TZwracany> GetSettingSlownikRefleksja<T, TZwracany>(string symbol, HashSet<TZwracany> defaultValue,
            bool zalogowany, string opis = null,
            ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false,
            bool domyslnieWidoczne = true, bool multi = false, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, HashSet<TZwracany> domyslnaWartoscNiezalogowani = null);

        string GetSettingPassword(string symbol, string defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, string domyslnaWartoscNiezalogowani = null);
        string GetSettingString(string symbol, string defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, string domyslnaWartoscNiezalogowani = null, bool dynamiczne = false);

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu Int
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <returns></returns>
        int GetSettingInt(string symbol, int defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, bool dynamiczne = false, int? domyslnaWartoscNiezalogowani = null);

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu Decimal
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <returns></returns>
        decimal GetSettingDecimal(string symbol, decimal defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, decimal? domyslnaWartoscNiezalogowani = null);

        HashSet<T> GetSettingEnum<T>(string symbol, HashSet<T> defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, bool multi = false, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, HashSet<T> domyslnaWartoscNiezalogowani = null);

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu Bool
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
  
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <returns></returns>
        bool GetSettingBool(string symbol, bool defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, bool? domyslnaWartoscNiezalogowani = null);

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu DateTime
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
       
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <returns></returns>
        DateTime GetSettingDateTime(string symbol, DateTime defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, DateTime? domyslnaWartoscNiezalogowani = null);

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu HTML
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <returns></returns>
        string GetSettingHTML(string symbol, string defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, string domyslnaWartoscNiezalogowani = null);

        List<Ustawienie> GetSettingsList(bool onlyVisibleToUser, int? pracownikId);
        List<Ustawienie> GetSettingsList();
        NameValueCollection ToNameValueCollection();

        /// <summary>
        /// Ustawia wartoœæ parametru
        /// </summary>
        /// <param name="key">Klucz</param>
        /// <param name="value">Wartoœæ</param>
        /// <param name="type"></param>
        void SetSetting(string key, string value, TypUstawienia type);

        void AktualizujUstawienie(Ustawienie u);
    }
}