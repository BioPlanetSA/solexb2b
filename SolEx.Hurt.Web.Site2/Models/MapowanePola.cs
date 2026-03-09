using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class MapowanePola
    {
        private HashSet<string> pola=new HashSet<string>();

        public void DodajPole(string pole)
        {
            pola.Add(pole);
        }

        public bool PoleMaByMapowane(string pole)
        {
            return pola.Contains(pole);
        }

        /// <summary>
        /// Uzupelnia pola które nie są mapowane
        /// </summary>
        /// <typeparam name="T">Mapowany typ</typeparam>
        /// <param name="doktoregoPiszemy">Obiekt w którym uzupełnimy pola</param>
        /// <param name="zktoregoPobieramyDane">Obiekt z którego pobieramy pola</param>
        public void PrzepiszNieMapowanePola<T>(T doktoregoPiszemy, T zktoregoPobieramyDane)
        {
            if (zktoregoPobieramyDane == null || doktoregoPiszemy == null)
            {
                return;
            }
            Type t = typeof (T);
            PropertyInfo[] prop = t.GetProperties();
            foreach (PropertyInfo p in prop)
            {
                if (PoleMaByMapowane(p.Name))
                {
                    continue;
                }
                if (p.SetMethod != null && p.GetMethod != null)
                {
                    object val = p.GetValue(zktoregoPobieramyDane);
                    p.SetValue(doktoregoPiszemy, val);
                }
            }
        }
    }
}