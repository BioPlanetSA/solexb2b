using SolEx.Hurt.Helpers;
using System;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.KolorowanieWierszy
{
    /// <summary>
    /// Klasa bazowa dla klas decydujących o kolorze
    /// </summary>
    public abstract class KolorowanieBaza
    {
        /// <summary>
        /// Jaki typ jest przetwarzany przez moduł
        /// </summary>
        /// <returns></returns>
        public abstract Type OblugiwanyTyp();
        
        // rezygnacja z koloru pisanego w formie szesnastkowej na rzecz bootstrapowych klass
        ///// <summary>
        ///// Kolor na jaki należy kolorować cały wiersz
        ///// </summary>
        ///// <param name="o">Sprawdzany obiekt</param>
        ///// <returns>Kolor w formacie #xxxxxx lub null</returns>
        //public abstract string KolorWiersza(object o);

        /// <summary>
        /// Kolor na jaki kolorować pole
        /// </summary>
        /// <param name="o">Cały obiekt</param>

        ///// <param name="wartoscPola">Dane sprawdzanego pola</param>
        ///// <returns>Kolor w formacie #xxxxxx lub null</returns>
        //public abstract string KolorPola(object o, OpisPolaObiektu wartoscPola);

        /// <summary>
        /// styl css dla wiersza
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public abstract string KlasaCssWiersza(object o);

        /// <summary>
        /// styl css dla wiersza
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public abstract string KlasaCssPola(object o);
    }
}