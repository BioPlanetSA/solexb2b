using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public class MaksymalnaLiczbaZnakowAttribute : Attribute
    {
        public static readonly MaksymalnaLiczbaZnakowAttribute Default;

        public MaksymalnaLiczbaZnakowAttribute() { }

       
        public MaksymalnaLiczbaZnakowAttribute(int liczba)
        {
            _liczba = liczba;
        }

        public virtual int MaksymalnaLiczbaZnakow { get { return _liczba; } }

        private readonly int _liczba;
    }
}