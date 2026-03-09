using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Kolekcja , ToString zawartość odzielone średnikami
    /// </summary>
   public  class CustomCollection:List<string>
    {
       /// <summary>
       /// Zawartośc kolekcji oddzielona 
       /// </summary>
       /// <returns></returns>
       public override string ToString()
       {
           StringBuilder sb = new StringBuilder();
           foreach (var s in this)
           {
               sb.Append(s.ToString() + ";");
           }
           return sb.ToString().Trim(';');
       }
       public static implicit operator CustomCollection(string i)
       {
           CustomCollection temp = new CustomCollection();
           if (!string.IsNullOrEmpty(i))
           {
               foreach (string s in i.Split(';'))
               {
                   temp.Add(s);
               }
           }
           return temp;
       }
    }
}
