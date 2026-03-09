using System;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    /// <summary>
    /// Pobieranie słownika z obiektu dziedziczącego po SlownikBaza
    /// </summary>
    public class PobieranieSlownika : Attribute
    {
        //konstruktor bazowy tylko dla refleksji bo sie sypie jak go nie ma 
        public PobieranieSlownika() { }

        public PobieranieSlownika(string name): this(Type.GetType(name))
        {

        }

        public PobieranieSlownika(Type typ)
        {
            if (typ == null)
            {
                throw new ArgumentNullException("typ");
            }

            var obiekt = Activator.CreateInstance(typ);
            if (obiekt as ISlownik == null)
            {
                throw new ArgumentException($"Walidator musi implementować intefrejsc {(typeof(ISlownik).Name)}", "typ");
            }
            Typ = typ;
        }

        public Type Typ { get; }
    }
}