using System;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.OperacjeZbiorcze
{
    /// <summary>
    /// Klasa bazowa dla modułów operacji zbiorczych
    /// </summary>
    public abstract class OperacjaZbiorczaBaza
    {
        public abstract string PokazywanaNazwa { get; }

        /// <summary>
        /// Jaki typ jest przetwarzany przez moduł
        /// </summary>
        /// <returns></returns>
        public abstract Type OblugiwanyTyp();

        /// <summary>
        /// Wykonuje operacje zbiorczą
        /// </summary>
        /// <param name="klucze">Kolekcja kluczy</param>
        public abstract void Wykonaj(string[] klucze);
    }
}