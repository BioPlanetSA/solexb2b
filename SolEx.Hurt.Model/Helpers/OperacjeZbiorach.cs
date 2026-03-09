using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Common;

namespace SolEx.Hurt.Model.Helpers
{
    public static class OperacjeZbiorach
    {
        public static HashSet<T> PobierzUnikalneElementy<T>(IList<HashSet<T>> KolekcjaSetow)
        {
            HashSet<T> cechyUnikalne = new HashSet<T>();

            var czescWspolna = PobierzPowtarzajaceElementy(KolekcjaSetow);

            foreach (HashSet<T> pb in KolekcjaSetow)
            {
                foreach (T element in pb)
                {
                    if (!czescWspolna.Contains(element))
                    {
                        if (!cechyUnikalne.Contains(element))
                        {
                            cechyUnikalne.Add(element);
                        }
                    }
                }
            }
            return cechyUnikalne;
        }

        public static HashSet<T> PobierzPowtarzajaceElementy<T>(IList<HashSet<T>> KolekcjaSetow)
        {
            HashSet<T> cechyPowtarzane = new HashSet<T>();
            //startowa lista z pierwszego  i kasowanie pierwszego z listy do porownania
            cechyPowtarzane = KolekcjaSetow.First();

            foreach (HashSet<T> pb in KolekcjaSetow.Skip(1))
            {
                cechyPowtarzane = new HashSet<T>(cechyPowtarzane.Intersect(pb));
            }
            return cechyPowtarzane;
        }

    }
}
