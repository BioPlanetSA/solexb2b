using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using FastMember;
using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Model.Helpers
{
    public class SqlHelper
    {
        private Dictionary<Type, List<PropertyInfo>> listaPropertisow = new Dictionary<Type, List<PropertyInfo>>(50);
        public List<PropertyInfo> PobierzPolaDoSprawdzania(Type t)
        {
            return listaPropertisow.GetOrAdd(t, (typ) =>
              {
                  //wszystkie proeprtisy, bez atrybutow IGNORE i bez KEY

                  return t.GetProperties().Where(x => x.CanRead && x.GetCustomAttributes(typeof(IgnoreAttribute), true).FirstOrDefault() == null
                      && x.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).FirstOrDefault() == null).ToList();
              });
        }

        public SqlExpressionVisitor<T> ZbudujWarunekSQLDlaObiektu<T>(T obiekt, TypeAccessor akcesor)
        {
            SqlExpressionVisitor<T> warunek = new SqlServerExpressionVisitor<T>();
            string budowanyWarunekWhere = "";
            List<PropertyInfo> pola = PobierzPolaDoSprawdzania(typeof(T));
            foreach (PropertyInfo p in pola)
            {
                if (p.PropertyType is IEnumerable)
                {
                    throw new NotImplementedException();
                }
                var wartosc = akcesor[obiekt, p.Name];
                if ((p.PropertyType == typeof(int?) || p.PropertyType == typeof(int)) && wartosc is Enum)
                {
                    wartosc = (int) wartosc;
                }

                if (wartosc == null)
                {
                    budowanyWarunekWhere  += String.Format(" ({0} is null) AND ", p.Name);
                    continue;
                }
                if (p.PropertyType.IsArray)
                {
                    StringBuilder sb=new StringBuilder();
                    foreach (var o in (IEnumerable)wartosc)
                    {
                        sb.AppendFormat("{0},", o);
                    }
                    budowanyWarunekWhere += String.Format(" ({0} ='[{1}]') AND ", p.Name,sb.ToString().Trim(','));
                    continue;
                }               
                budowanyWarunekWhere  += String.Format(" ({0} = {1}) AND ", p.Name, this.ToStringDoSerializacji(wartosc));
            }
            string fraza = budowanyWarunekWhere + " (1=1)";
            fraza = fraza.Replace("{", "{{").Replace("}", "}}");
            warunek.Where(fraza);
            //warunek.Limit(1);

            if (fraza.Length > 2000)
            {
                Debug.WriteLine("Za długi obiekt do porównania SQL");
            }

            return warunek;
        }

        /// <summary>
        /// jesli NULL -> zwraca 'NULL', dla obiektu wywoluje ToStringDoSerializacji()
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private string ToStringDoSerializacji(object o)
        {
            if (o == null) return "NULL";

            Type t = o.GetType();
            if (t == typeof(bool))
            {
                return ((bool)o) ? "1" : "0";
            }
            if (t == typeof(decimal))
            {
                return ((decimal)o).ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            }
            if (t == typeof(string) || t == typeof(Adres))
            {
                return "'" + o.ToString().Replace("'", "''") + "'";
            }
            if (t == typeof(int?))
            {
                return ((int?)o).HasValue ? ((int?)o).Value.ToString(CultureInfo.InvariantCulture) : "NULL";
            }
            if (t == typeof(int))
            {
                return ((int)o).ToString(CultureInfo.InvariantCulture);
            }
            if (t == typeof(decimal?))
            {
                return ((decimal?)o).HasValue ? ((decimal?)o).Value.ToString(CultureInfo.InvariantCulture).Replace(",", ".") : "NULL";
            }
            if (t == typeof(decimal))
            {
                return ((decimal)o).ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            }
            if (t == typeof(bool?))
            {
                return ((bool?)o).HasValue ? ((bool?)o).Value ? "1" : "0" : "NULL";
            }
            if (t == typeof(bool))
            {
                return ((bool)o) ? "1" : "0";
            }
            if (t == typeof(WartoscLiczbowa))
            {
                return ((WartoscLiczbowa)o).Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            }
            if (t == typeof(DateTime?))
            {
                return ((DateTime?)o).HasValue ? "'" + ((DateTime?)o).Value.ToString("yyyyMMdd HH:mm:ss.fff") + "'" : "NULL";
            }
            if (t == typeof(DateTime))
            {
                return "'" + ((DateTime)o).ToString("yyyyMMdd HH:mm:ss.fff") + "'";
            }
            return "'" + o + "'";
        }
    }
}
