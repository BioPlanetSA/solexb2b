using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public class FriendlyPropertyName : Attribute
    {
        public static readonly FriendlyNameAttribute Default;
        public  FriendlyPropertyName() { }
        public FriendlyPropertyName(string friendlyName, string replaceTo)
        {
            FriendlyNameValue = friendlyName;
            ReplaceTo = replaceTo;
        }
        public virtual string FriendlyName { get { return FriendlyNameValue; } }
        public virtual string ReplaceTo { get; set; }
        private string FriendlyNameValue { get; set; }
    }
}