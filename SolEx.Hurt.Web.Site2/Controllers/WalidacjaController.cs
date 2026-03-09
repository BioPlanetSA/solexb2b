using System;
using System.Web.Mvc;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    /// <summary>
    /// Kontroler odpowiedzialny za walidację danych
    /// </summary>  
    [RoutePrefix("Walidacja")]
    public class WalidacjaController : SolexControler
    {
        /// <summary>
        /// Waliduje dane
        /// </summary>
        /// <param name="walidator">Typ walidatora</param>
        /// <param name="klucz">Identyfikator obiektu</param>
        /// <param name="testowane">Testowana fraza</param>
        /// <returns>Odpowiedź w formacjie json</returns>
        [Route("Waliduj")]
        public ActionResult Waliduj(string walidator,string klucz,string testowane)
        {
            Type t = Type.GetType(walidator, true);
            IWalidatorDanych instancja = (IWalidatorDanych)Activator.CreateInstance(t);
            var wynik = instancja.Waliduj(klucz, testowane);
            return PrzeksztalcNaJson(wynik);
        }
        
       
    }
}