using System;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces.Helpers
{
    public interface ITextHelper
    {
        bool SprobojSparsowac(string text, out decimal wynik);

        /// <summary>
        /// Pobiera zawartosc taga body
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string GetHTMLBodyContent(string text);

        ///// <summary>
        ///// Usuwa polskie znaki i zamienia odpowiednikiem "bez ogonka"
        ///// </summary>
        ///// <param name="text">tekst</param>
        ///// <returns></returns>
        //string ReplacePolishChars(string text);

        ///// <summary>
        ///// Usuwa polskie znaki
        ///// </summary>
        ///// <param name="text">tekst</param>
        ///// <param name="defaultValue">Znak na co zamieniane s¹ polskie znaki</param>
        ///// <returns></returns>
        //object ReplacePolishChars(string text, string defaultValue);

        /// <summary>
        /// Koduje znaki Base64
        /// </summary>
        /// <param name="str">Ci¹g do zakodowania</param>
        /// <returns></returns>
        string Encode(string str);

        /// <summary>
        /// Odkodowuje string z Base64 
        /// </summary>
        /// <param name="str">Zakodowany tekst</param>
        /// <returns></returns>
        string Decode(string str);

        string OczyscNazwePliku(string nazwa);
        string WygenerujNazweZdjecia(TypyPolDoDopasowaniaZdjecia pole, string separator, int idProduktu, string kodKreskowy, string kodProduktu, bool zdjecieGlowne, int idzdjecia, string rozszerzenie);
        DateTime? ParsujDate(string data);
        bool SprobojSparsowac(string data, out DateTime wynik);
        string ParsujDateDoStringa(DateTime? value);
    }
}