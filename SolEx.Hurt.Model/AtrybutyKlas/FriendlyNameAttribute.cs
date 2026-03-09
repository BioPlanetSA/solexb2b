using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public class FriendlyNameAttribute : Attribute
    {
        public static readonly FriendlyNameAttribute Default;

        public FriendlyNameAttribute() { }

        public FriendlyNameAttribute(string friendlyName):this(friendlyName,false)
        {
        }

        /// <summary>
        /// Firendly name do tłumaczenia
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <param name="frazaDoTlumaczenia">Fraza jaka będzie w tłumaczeniach</param>
        public FriendlyNameAttribute(string friendlyName, bool tlumaczony)
        {
            _friendlyNameValue = friendlyName;
            _tlumaczony = tlumaczony;
        }

        public virtual string FriendlyName { get { return _friendlyNameValue; } }

        public virtual string FriendlyOpis { get; set; }

        public bool Tlumaczony
        {
            get { return _tlumaczony; }
        }

        private readonly string _friendlyNameValue;
        private readonly bool _tlumaczony;
    }
}