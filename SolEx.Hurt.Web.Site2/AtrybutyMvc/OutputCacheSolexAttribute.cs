using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Enums;
using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.PageBases;
using Microsoft.Ajax.Utilities;

namespace SolEx.Hurt.Web.Site2.AtrybutyMvc
{
    public class OutputCacheSolexAttribute:OutputCacheAttribute
    {
        private TypDanychDoCache _typ;
        private string _cachedKey;
        
        public ISolexBllCalosc Calosc;
        
       /// <summary>
       /// 
       /// </summary>
       /// <param name="typ">Typ akcji do cachu</param>
       /// <param name="indywidualizowanaOferta">Czy tworzyć cache dla klientów z indywidualizowaną ofertą</param>
        public OutputCacheSolexAttribute(TypDanychDoCache typ)
        {
            _typ = typ;
            Duration = 360;
           Calosc = SolexBllCalosc.PobierzInstancje;
        }

        /// <summary>
        /// Metoda do pobierania zcachowanego widoku. Jak nie ma cache to zwraca NULL
        /// </summary>
        /// <param name="idKontrolki"></param>
        /// <param name="idKlienta"></param>
        /// <param name="idJezyka"></param>
        /// <returns></returns>
        public ActionResult AkcjaMenu(int idKontrolki, long idKlienta, int idJezyka)
        {
            if (idKlienta == 0)
            {
                _cachedKey = Calosc.Cache.WyliczKluczDlaMenu(idKontrolki, idKlienta, idJezyka, false);
                return Calosc.Cache.PobierzObiekt<ActionResult>(_cachedKey);
            }

            bool? jestPerKlient = Calosc.TresciDostep.SprawdzCzyKontrolaMenuMaTresciUkrywaneWgKlientow(idKontrolki);

            if (jestPerKlient == null)
            {
                return null;
            }

            _cachedKey = Calosc.Cache.WyliczKluczDlaMenu(idKontrolki, idKlienta, idJezyka, jestPerKlient.Value);
            return Calosc.Cache.PobierzObiekt<ActionResult>(_cachedKey);
        }

        public ActionResult AkcjaKategorie(int idKontrolki,IKlient klient, string szukane, out bool nieCachuj)
        {
            _cachedKey = Calosc.Cache.WyliczKluczDlaKategorii(idKontrolki, klient, szukane);
            if (string.IsNullOrEmpty(_cachedKey))
            {
                nieCachuj = true;
                return null;
            }
            nieCachuj = false;
            return Calosc.Cache.PobierzObiekt<ActionResult>(_cachedKey);
        }

        public ActionResult AkcjaFiltry(ParametryPrzekazywaneDoListyProduktow param, IKlient klient, out bool nieCachuj)
        {
            _cachedKey = param.KluczDoCachuFiltrow(klient);
            if (string.IsNullOrEmpty(_cachedKey))
            {
                nieCachuj = true;
                return null;
            }
            nieCachuj = false;
            return Calosc.Cache.PobierzObiekt<ActionResult>(_cachedKey);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ActionResult wynik = null;
            int idKontrolki;
            ISolexHelper helper = ((SolexControler) filterContext.Controller).SolexHelper;
            IDictionary<string, object> parametry = filterContext.ActionParameters;
            bool nieCachuj = false;
            if (_typ == TypDanychDoCache.Filtry)
            {
                ParametryPrzekazywaneDoListyProduktow param = PobierzParametr<ParametryPrzekazywaneDoListyProduktow>(parametry, "parametry");
                wynik = AkcjaFiltry(param, helper.AktualnyKlient, out nieCachuj);
            }
            if (_typ == TypDanychDoCache.Kategorie)
            {
                string szukane = PobierzParametr<string>(parametry, "szukanieGlobalne");
                idKontrolki = PobierzParametr<int>(parametry, "idKontrolki");
                wynik = AkcjaKategorie(idKontrolki, helper.AktualnyKlient, szukane, out nieCachuj);
            }

            if (_typ == TypDanychDoCache.Menu)
            {
                idKontrolki = PobierzParametr<int>(parametry, "id");
                if (idKontrolki == 0) { throw new Exception("Brak id kontrolki przekazanej do akcji!"); }
                wynik = this.AkcjaMenu(idKontrolki, helper.AktualnyKlient.Id, helper.AktualnyJezyk.Id);
            }

            if (nieCachuj)
            {
                //clear cache! nie cachujemy
                return;
            }

            if (wynik == null)
            {
                VaryByCustom = _cachedKey;
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = wynik;
            }
        }


        private static T PobierzParametr<T>(IDictionary<string, object> parametry, string nazwa)
        {
            object wynikObj;
            if (parametry.TryGetValue(nazwa, out wynikObj))
            {
                if(wynikObj == null)
                {                   
                    return default(T);
                }

                T wynik;
                try
                {
                    wynik = (T)wynikObj;
                }
                catch (Exception ex)
                {
                    throw;
                   // SolexBllCalosc.PobierzInstancje.Log.Error($"Błąd rzutowania parametru {nazwa} na typ: {typeof(T)}. Parametr jest tak naprawdę typu: ")
                   // throw new Exception($"Błąd rzutowania parametru {nazwa} na typ: {typeof(T)}. Parametr jest tak naprawdę typu: {wynikObj.GetType()}.");
                }
                return wynik;
            }
            return default(T);
        }
    }
}