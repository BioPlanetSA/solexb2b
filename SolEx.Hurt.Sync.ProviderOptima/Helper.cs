using System;
using System.Linq;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Sync.ProviderOptima
{
    public class Helper
    {

        public static void SprawdzCzyFunkcjaIstnieje<T>(ref T obj, string nazwa, object[] parametry, bool? wartosc, CoSprawdzamyOptima coSprawdzamy)
        {
            switch (coSprawdzamy)
            {
                case CoSprawdzamyOptima.Metoda:
                {
                    var metoda = obj.GetType().GetMethods().FirstOrDefault(x => x.Name == nazwa && x.GetParameters().Count() == parametry.Count());
                    if (metoda != null)
                    {
                        metoda.Invoke(obj, parametry);
                    }
                    break;
                }
                case CoSprawdzamyOptima.Properties:
                {
                    var properties = obj.GetType().GetProperty(nazwa);
                    if (properties != null)
                    {
                        properties.SetValue(obj,wartosc);
                    }
                    break;
                }
                
            }
        }

        public static object TworzenieLoginu<T>(ref T obj, string nazwa, object[] parametry, CoSprawdzamyOptima coSprawdzamy)
        {
            var metoda = obj.GetType().GetMethods().FirstOrDefault(x => x.Name == nazwa && x.GetParameters().Count() == parametry.Count());
            if (metoda != null)
            {
                return metoda.Invoke(obj, parametry);
            }
            return "";
        }


    }
}
