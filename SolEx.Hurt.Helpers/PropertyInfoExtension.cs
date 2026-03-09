using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SolEx.Hurt.Model.Extensions;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Helpers
{
    public static class PropertyInfoExtension
    {
        //todo: porpawka wydajnosci zapisu
        public static void SetValueExt(this PropertyInfo p, object obiekt, object wartosc)
        {
            Type real = p.PropertyType;

            if (p.PropertyType.IsNullableType())
            {
                if (wartosc == null || wartosc.ToString() == "")
                {
                    p.SetValue(obiekt, null, null);
                    return;
                }
                real = p.PropertyType.GetGenericArguments()[0];
            }
            if (wartosc != null)
            {
                object o = wartosc.KonwertujWartosc(real);
                p.SetValue(obiekt, o, null);
            }
        }

        public static bool IsList(this PropertyInfo o)
        {
            return o.PropertyType != typeof (string) && typeof (IEnumerable).IsAssignableFrom(o.PropertyType);
        }

    }
}
