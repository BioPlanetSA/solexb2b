using System;
using System.ComponentModel;

namespace SolEx.Hurt.Model.Core
{
    public class WartoscLiczbowaConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value)
        {
            //if (value is int)
            //    return new WartoscLiczbowa((int) value);
            if (value is WartoscLiczbowa)
                return value;
            if (value is decimal)
                return new WartoscLiczbowa((decimal) value);
            //if (value is double)
            //    return new WartoscLiczbowa((double) value);
            //if (value is float)
            //    return new WartoscLiczbowa((float) value);
            if (value is string)
                return WartoscLiczbowa.Parse((String)value);
            throw new InvalidCastException();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(decimal)  ||  //sourceType == typeof (int) || || sourceType == typeof(float) ||   sourceType == typeof(double) || 
                   sourceType == typeof(string);
        }
    }
}