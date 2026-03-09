using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    /// <summary>
    /// Pobieranie słownika z propertisów obiektu
    /// </summary>
    public class SyncSlownikNaPodstawieInnegoTypu : Attribute
    {
        public SyncSlownikNaPodstawieInnegoTypu(string nazwaTypu):this(Type.GetType(nazwaTypu,true))
        {
            PobierajWszystkie = false;
        }
        public SyncSlownikNaPodstawieInnegoTypu(Type typ)
        {
            Typ = typ;
        }
        public SyncSlownikNaPodstawieInnegoTypu(Type typ, bool wszystko)
        {
            Typ = typ;
            PobierajWszystkie = wszystko;
        }

        public Type Typ { get; set; }
        public bool PobierajWszystkie { get; set; }
    }
}
