using System;
using System.ComponentModel;

namespace SolEx.Hurt.Model.Core
{
    public class WartoscLiczbowaZaokragalnaConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value)
        {
            //if (value is int)
            //    return new WartoscLiczbowaZaokraglana((int)value);
            if (value is WartoscLiczbowaZaokraglana)
                return value;
            if (value is decimal)
                return new WartoscLiczbowaZaokraglana((decimal)value);
            //if (value is double)
            //    return new WartoscLiczbowaZaokraglana((double)value);
            //if (value is float)
            //    return new WartoscLiczbowaZaokraglana((float)value);
            if (value is string)
                return WartoscLiczbowaZaokraglana.Parse((String) value);
            throw new InvalidCastException();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(decimal)  || //sourceType == typeof(int) || || sourceType == typeof(float) || sourceType == typeof(double) ||
                   sourceType == typeof(string);
        }
    }
}