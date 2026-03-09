using System;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    /// <summary>
    /// Atrybut opisujący walidator, który ma być wykorzystywany dla danego pola
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class WalidatorDanych:Attribute
    {
        private readonly Type _typ;
        /// <summary>
        /// Tworzy nową instancję
        /// </summary>
        /// <param name="typ">Nazwa typu, musi być podana w formie aby <c>Type.GetType</c> mógł go odnaleść </param>
      
        public WalidatorDanych(string typ):this(Type.GetType(typ))
        {
            
        }
        /// <summary>
        /// Tworzy nową instancję
        /// </summary>
        /// <param name="typ">Typ walidatora</param>
        /// <exception cref="ArgumentNullException">Gdy nie odnaleziono typu</exception>
        /// <exception cref="ArgumentException">Gdy przekazany typ nie implemetuje <see cref="IWalidatorDanych"/></exception>
        public WalidatorDanych(Type typ)
        {
            if (typ == null)
            {
                throw new ArgumentNullException("typ");
            }
            var obiekt = Activator.CreateInstance(typ);
            if (obiekt as IWalidatorDanych == null)
            {
                throw new ArgumentException(string.Format("Walidator musi implementować intefrejsc {0}", (typeof(IWalidatorDanych).Name)),"typ");
            }
            _typ = typ;
        }

        public Type Typ
        {
            get { return _typ; }
        }
    }
}
