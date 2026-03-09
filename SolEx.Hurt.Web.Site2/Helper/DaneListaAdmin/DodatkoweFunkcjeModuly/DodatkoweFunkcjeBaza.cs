using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly
{
    /// <summary>
    /// Klasa bazowa dla modułów generujące listy dodatkowych funkcji na liście w adminie
    /// </summary>
    public abstract class DodatkoweFunkcjeBaza
    {
        /// <summary>
        /// Jaki typ jest przetwarzany przez moduł
        /// </summary>
        /// <returns></returns>
        public abstract Type OblugiwanyTyp();

        /// <summary>
        /// Zwraca kolekcję dodatkowych funkcji dla modułu
        /// </summary>
        /// <param name="o">Obiekt dla którego sprawdzamy dodatkowe funkcjie</param>
        /// <returns></returns>
        public abstract IList<DodatkowaFunkcja> PobierzDodatkoweFunkcjeDlaObiektu(object o);

        /// <summary>
        /// Zwraca kolekcję komunikatów dla obiektu
        /// </summary>
        /// <param name="o">Obiekt dla którego generujemy komuniaty</param>
        /// <returns></returns>
        public abstract List<Komunikat> KomunitatyNaEdycjiObiektu(object o);

        /// <summary>
        /// Zwraca kolekcję komunikatów dla Listy
        /// </summary>
        /// <param name="o">Obiekt dla którego generujemy komuniaty</param>
        /// <returns></returns>
        public abstract List<Komunikat> KomunitatyNaLiscieObiektu(Type o);

        public abstract bool? MoznaUsuwacObiekt(object o);
        public abstract bool? MoznaEdytowacObiekt(object o);
        public virtual string PobierzNazweObiektu(object o)
        {
            FriendlyNameAttribute opisy = o.GetType().GetCustomAttribute<FriendlyNameAttribute>();
            return (opisy != null) ? opisy.FriendlyName : o.GetType().Name;
        }

        public virtual string PobierzOpisObiektu(object o)
        {
            FriendlyNameAttribute opisy = o.GetType().GetCustomAttribute<FriendlyNameAttribute>();
            return (opisy != null) ? opisy.FriendlyOpis : "";
        }


        public UrlHelper Url { get; set; }
    }
}