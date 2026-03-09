using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Extensions;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.DostepDane
{
    public class OpisPolaObiektu:OpisPolaObiektuBaza
    {
        public object Wartosc { get; set; }
        public object[] Wartosci { get; set; }

        public OpisPolaObiektu() { }

        /// <summary>
        /// Pobiera wartoœæ na postawien nazwy pola z ca³ego obiektu
        /// </summary>
        /// <param name="baza"></param>
        /// <param name="obiekt"></param>
        public OpisPolaObiektu(OpisPolaObiektuBaza baza, object obiekt):base(baza)
        {
            if (Property == null)
            {
                Property = obiekt.GetType().GetProperty(NazwaPola);
            }
            if (Property == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola o nazwie " + NazwaPola);
            }
            Wartosc = Property.GetValue(obiekt);    //todo: do poprawy
        }
        /// <summary>
        /// Konstruktor w którym podajemy konkretn¹ wartoœæ
        /// </summary>
        /// <param name="wartosc"></param>
        /// <param name="baza"></param>
        public OpisPolaObiektu(object wartosc, OpisPolaObiektuBaza baza) : base(baza)
        {
            Wartosc =wartosc;
        }

        /// <summary>
        /// Tworzymy obiekt bez uzupelnionej wartosci - niezbede 
        /// </summary>
        /// <returns></returns>
        public OpisPolaObiektu(OpisPolaObiektuBaza baza) : base(baza)
        {

        }


        public override string ToString()
        {
            return this.NazwaWyswietlana + ":" + Refleksja.OpisWartosci(this.Wartosc);
        }

        public object PobierzWartosc()
        {
            if (Wartosc == null)
            {
                if (Wartosci != null)
                {
                    return Refleksja.PobierzWartosc(Wartosci, TypPrzechowywanejWartosci);
                }
                return null;
            }
            return Refleksja.PobierzWartosc(Wartosc, TypPrzechowywanejWartosci);
        }

    }
}