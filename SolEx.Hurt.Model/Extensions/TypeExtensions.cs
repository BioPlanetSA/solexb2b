using System;

namespace SolEx.Hurt.Model.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType
                   && type.GetGenericTypeDefinition().Equals(typeof (Nullable<>));
        }
        public static object Parsuj(this string input, Type t)
        {
            object a = null;
            var conv = System.ComponentModel.TypeDescriptor.GetConverter(t);
            if (conv.CanConvertFrom(typeof(string)))
            {
                try
                {
                    a = conv.ConvertFrom(input);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return a;
        }
    }



}
