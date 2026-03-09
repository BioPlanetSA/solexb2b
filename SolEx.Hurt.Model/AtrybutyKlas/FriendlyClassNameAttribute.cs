using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public class FriendlyClassNameAttribute : Attribute
    {
        public static readonly FriendlyNameAttribute Default;
        public FriendlyClassNameAttribute() { }
        public FriendlyClassNameAttribute(string friendlyName)
        {
            FriendlyNameValue = friendlyName;
        }
        public virtual string FriendlyName { get { return FriendlyNameValue; } }
        private string FriendlyNameValue { get; set; }
    }
}