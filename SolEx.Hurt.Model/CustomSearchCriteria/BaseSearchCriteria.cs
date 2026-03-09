using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    /// <summary>
    /// Klasa bazowa dla wszystkich kryteriów wyszukiwania
    /// </summary>
    public abstract class BaseSearchCriteria
    {
        /// <summary>
        /// Dodatkowy sql, który dodawany jest na sztywno na końcu zapytania, musi zacznać się od add
        /// </summary>
        //public string AddtionalSQL { get; set; }
        //public string Sort { get; set; }
        //protected virtual string DodatowySql()
        //{
        //    return "";
        //}

        /// <summary>
        /// Zwraca parametry wyszukiwania w formie sql
        /// </summary>
        /// <returns></returns>
        //public override string ToString()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    //sb.Append("where 1=1 ");
        //    Type t = GetType();
        //    foreach (PropertyInfo property in t.GetProperties())
        //    {
        //        if (property.GetCustomAttributes(typeof (PomijajAtrybut), true).Length > 0)
        //        {
        //            continue; ;//pomijamy te pola w automatycznej budowie
        //        }

        //        //jak zaczyna sie od _ to olewamy
        //        if (property.Name.StartsWith("_"))
        //        {
        //            continue;
        //        }
        //        object data = property.Get(this);

        //        //nie jest lista
        //        if (!(data is IList))
        //        {
        //            if (data is Boolean){
        //                sb.Append( string.Format("  and ( {0} = {1} )", property.Name, ( (bool)data ) ? "1" : "0" ) );
        //            }
        //        }

        //        IList coll = data as IList;
        //        List<object> incol = new List<object>();
        //        List<object> likes = new List<object>();
        //        if (coll != null && coll.Count > 0)
        //        {
        //            sb.Append(" and (  ");
                  
        //            for (int i = 0; i < coll.Count; i++)
        //            {
        //                if (coll[i]!=null && coll[i].ToString().Length>1 && coll[i].ToString().StartsWith("%") && coll[i].ToString().EndsWith("%"))
        //                {
        //                    likes.Add(coll[i]);
        //                }
        //                else
        //                {
        //                   incol.Add(coll[i]);
        //                }
                     
        //            }
                 
        //            if (incol.Count > 0)
        //            {
        //                sb.Append(string.Format("    {0} in (", property.Name));
        //                for (int i = 0; i < incol.Count; i++)
        //                {
        //                    sb.Append(incol[i].ToStringDoSerializacji());
        //                    if (i < incol.Count - 1)
        //                    {
        //                        sb.Append(",");
        //                    }
        //                }
        //                sb.Append(") ");
        //            }
        //            if (likes.Count > 0 && incol.Count > 0)
        //            {
        //                sb.Append(" or ");
        //            }
        //            if (likes.Count > 0)
        //            {
        //                foreach(var o in likes)
        //                {
        //                    sb.AppendFormat(" {0} like  {1} ", property.Name, o.ToStringDoSerializacji());
        //                }
        //            }
        //            sb.Append(" ) ");
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(AddtionalSQL))
        //    {
        //        if (!AddtionalSQL.Trim().StartsWith("and"))
        //        {
        //            sb.Append(" and ");
        //        }
        //        sb.Append(AddtionalSQL);
        //    }
        //    if (!string.IsNullOrEmpty(DodatowySql()))
        //    {
        //        if (!DodatowySql().Trim().StartsWith("and"))
        //        {
        //            sb.Append(" and ");
        //        }
        //        sb.Append(DodatowySql());
        //    }
        //    if (sb.Length > 0)
        //    {
        //        sb.Insert(0, " where 1=1 ");
        //    }
        //    if (!string.IsNullOrEmpty(Sort))
        //    {
        //        sb.Append(" order by ");
        //        sb.Append(Sort);
        //    }
        //    return sb.ToString();
        //}
    }
}
