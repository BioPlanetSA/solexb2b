using System;
using System.Collections.Generic;
using ServiceStack.Common;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    /// <summary>
    /// Umożliwia wymuszenie innego typu edytora niż wynikałby z typu, np edytora zdjęć
    /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
   
    public class WymuszonyTypEdytora:Attribute
    {
        private readonly Type _rodzajDanych;
        private readonly TypEdytora _typ;

        public WymuszonyTypEdytora(TypEdytora typ)
        {
            _typ = typ;
         
        }
        public WymuszonyTypEdytora(TypEdytora typ, Type rodzajDanych)
            : this(typ)
        {
            _rodzajDanych = rodzajDanych;
        }
        public WymuszonyTypEdytora(TypEdytora typ, string rodzajDanych)
            : this(typ,Type.GetType(rodzajDanych,true))
        {
           
        }
        public TypEdytora Typ
        {
            get { return _typ; }
        }
        public Type RodzajDanych
        {
            get { return _rodzajDanych; }
        }
    }


    public class WymuszoneRoszerzeniePlikuAttribute : Attribute
    {
        public string Rozszerzenia { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rozszerzenia">Rozszerzenia po przecinku np. 'doc','jpg','png'</param>
        public WymuszoneRoszerzeniePlikuAttribute(string rozszerzenia)
        {
            Rozszerzenia = rozszerzenia.Replace("'", "\"");
        }
    }
}
