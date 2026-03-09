using System;

namespace SolEx.Hurt.Core.ModelBLL
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RepreznetowyTypModulowAttribute : Attribute
    {
        private readonly Type _typ;

        public RepreznetowyTypModulowAttribute(Type typ)
        {
            _typ = typ;
        }

        public Type Typ
        {
            get { return _typ; }
        }
    }
}
