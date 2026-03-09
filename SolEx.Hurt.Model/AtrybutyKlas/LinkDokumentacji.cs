using System;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public class LinkDokumentacji : Attribute
    {
        public LinkDokumentacji(string link)
        {
            FriendlyNameValue = link;
        }
        public virtual string Link { get { return FriendlyNameValue; } }
        private string FriendlyNameValue { get; set; }
    }
}