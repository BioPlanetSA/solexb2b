using System;

namespace SolEx.Hurt.Core.DostepDane.DaneObiektu
{
    /// <summary>
    /// Obiekt edytowalny w adminie
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EdytowalnyAdmin:Attribute
    {
        public EdytowalnyAdmin()
        {
            
        }
    }
}
