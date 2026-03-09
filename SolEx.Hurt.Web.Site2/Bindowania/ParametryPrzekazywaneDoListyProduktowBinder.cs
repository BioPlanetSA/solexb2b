using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using log4net;
using Newtonsoft.Json;
using ServiceStack.Common;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Core;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    public class ParametryPrzekazywaneDoListyProduktowBinder : DefaultModelBinder
    {
        protected ILog Log
        {
            get
            {
                return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
        }


        /// <summary>
        /// Metoda mapuj¹ca pola z formularza na obiekt
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ParametryPrzekazywaneDoListyProduktow parametry = (ParametryPrzekazywaneDoListyProduktow) controllerContext.RouteData.Values[bindingContext.ModelName];

            //jesli juz jest w kolekcji to go wyciagamy jak nie to budujemy
            if (parametry != null) return parametry;

            parametry = new ParametryPrzekazywaneDoListyProduktow();

            SolexControler kontroler = controllerContext.Controller as SolexControler;

            ValueProviderResult wartosc = bindingContext.ValueProvider.GetValue("idKontrolki");

            if (wartosc != null)
            {
                if (wartosc.RawValue is int)
                {
                    parametry.idKontrolki = (int)wartosc.RawValue;
                }
                else
                {
                    parametry.idKontrolki = int.Parse( wartosc.AttemptedValue);
                }
            }

            if (parametry.idKontrolki == 0)
            {
                var idKontrolki = bindingContext.ValueProvider.GetValue("id");
                if (idKontrolki == null)
                {
                    //tu musi byc return null dlatego ze parametry sa opcjonalnem parametrwem w wydruku ofert i nie mozna przez to wywalac wyjatkow
                    //throw new Exception("Z³y link - nie mo¿na odczytaæ parametru id.");
                    return null;
                }
                //jesli sie tu wywai to jakis gruby blad - cos co wymaga parametroListy nie jest uzupelnione kontrolka albo nie jest Postem ajaxa
                parametry.idKontrolki = (int)idKontrolki.RawValue;
            }

            wartosc = bindingContext.ValueProvider.GetValue("kategoria");

            if (wartosc != null)
            {
                if (wartosc.RawValue is long)
                {
                    parametry.kategoria = (long)wartosc.RawValue;
                }
                else if (!string.IsNullOrEmpty(wartosc.AttemptedValue))
                {
                    parametry.kategoria = long.Parse(wartosc.AttemptedValue);
                }
            }

            //jesli jest kategoria wybrana to na koniec pobieramy obiekt kategorii
            if (parametry.kategoria.HasValue)
            {
                parametry.KategoriaObiekt = kontroler.Calosc.DostepDane.PobierzPojedynczy<KategorieBLL>(parametry.kategoria.Value, kontroler.SolexHelper.AktualnyJezyk.Id, kontroler.SolexHelper.AktualnyKlient);
            }

            wartosc = bindingContext.ValueProvider.GetValue("strona");
            if (wartosc != null)
            {
                if (wartosc.RawValue is int)
                {
                    parametry.strona = (int)wartosc.RawValue;
                }
                else
                {
                    parametry.strona = int.Parse(wartosc.AttemptedValue);
                }
            }
            else
            {
                parametry.strona = 1;
            }

            wartosc = bindingContext.ValueProvider.GetValue("szukanaWewnetrzne");
            if (wartosc != null)
            {
                parametry.szukanaWewnetrzne = System.Uri.UnescapeDataString((string)wartosc.AttemptedValue);
            }

            wartosc = bindingContext.ValueProvider.GetValue("szukane");
            if (wartosc != null)
            {
                parametry.szukane = System.Uri.UnescapeDataString((string)wartosc.AttemptedValue);
            }

            wartosc = bindingContext.ValueProvider.GetValue("filtry");
            if (wartosc != null)
            {
                //moze juz jest slownikiem w pamieci
                if (wartosc.RawValue is Dictionary<int, HashSet<long>>)
                {
                    parametry.filtry = (Dictionary<int, HashSet<long>>) wartosc.RawValue;
                }
                else
                {
                    //poprawny json: {"964055868":[1654927299]}
                    //poprawny nasz format QS: Typ[Furgon]Model[Partner+II]Marka[Honda;Mini;Nissan]
                    //sprawdzamy czy wchodzi JSON czy z QS - oba foramtu wspieramy - mozna podaæ na oba sposoby
                    string s = wartosc.AttemptedValue;
                    if (s.StartsWith("{"))
                    {
                        //json
                        parametry.filtry = JsonConvert.DeserializeObject(s, typeof(Dictionary<int, HashSet<long>>)) as Dictionary<int, HashSet<long>>;
                    }
                    else
                    {
                        //QS
                        parametry.filtry = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.SlownikFiltrow(s);
                    }
                }

                //poprawienia fitrow zeby byl null a nie pusty
                if (parametry.filtry != null && parametry.filtry.IsEmpty())
                {
                    parametry.filtry = null;
                }
            }

            parametry.KluczDoCachuFiltrow(kontroler.SolexHelper.AktualnyKlient);
            return parametry;
        }
    }
}