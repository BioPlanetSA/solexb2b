using System;


namespace SolEx.Hurt.Model.AtrybutyKlas
{
    /// <summary>
    /// Atrybut opisujący grupę w której ma pokazać się dany element
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class GrupaAtttribute : Attribute
    {
  
        private readonly string _grupa;
        private readonly int  _kolejnosc;

        /// <summary>
        /// Tworzy nową instancję atrybutu
        /// </summary>
        /// <param name="grupa">Nazwa grupy</param>
        /// <param name="kolejnosc">Kolejnosc grupy</param>
        public GrupaAtttribute(string grupa,int kolejnosc)
        {
            _grupa = grupa;
            _kolejnosc = kolejnosc;
        }
        /// <summary>
        /// Grupa do której ma być przypisany element
        /// </summary>
        public string Grupa
        {
            get { return _grupa; }
        }

        public int Kolejnosc
        {
            get { return _kolejnosc; }
        }
    }
  
}
