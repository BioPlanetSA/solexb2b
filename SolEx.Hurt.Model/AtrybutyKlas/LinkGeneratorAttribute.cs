using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    /// <summary>
    /// Używany do rozpoznawania że obiekt ma linki i na podstawie tego tworzona jest zakładka w adminie z linkami. 
    /// </summary>


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LinkGeneratorAttribute : Attribute
    {
        private readonly Type _typ;
        public LinkGeneratorAttribute() { }

        public LinkGeneratorAttribute(Type typ)
        {
            if (typ == null)
            {
                throw new ArgumentNullException("typ");
            }
            _typ = typ;
        }
        public Type Typ { get { return _typ; } }
    }
}