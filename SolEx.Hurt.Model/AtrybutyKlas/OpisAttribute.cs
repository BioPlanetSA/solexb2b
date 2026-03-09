using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public class OpisAttribute : Attribute
    {
        public static readonly FriendlyNameAttribute Default;
        public  OpisAttribute() { }
        public OpisAttribute(string opis)
        {
            OpisValue = opis;
        }
        public virtual string Opis { get { return OpisValue; } }
        private string OpisValue { get; set; }
    }
}