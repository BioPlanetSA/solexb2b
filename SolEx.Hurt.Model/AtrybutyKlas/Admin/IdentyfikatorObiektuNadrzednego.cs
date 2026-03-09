using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    /// <summary>
    /// Atrybut wskażnikowy identyfikujący pole obiektu nadrzednego
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IdentyfikatorObiektuNadrzednego : Attribute
    {
        private readonly Type _rodzaj;

        public IdentyfikatorObiektuNadrzednego(Type rodzaj)
        {
            _rodzaj = rodzaj;
        }
        public IdentyfikatorObiektuNadrzednego(string rodzaj):this(Type.GetType(rodzaj,true))
        {
          
        }
        public Type Rodzaj
        {
            get { return _rodzaj; }
        }
    }
}