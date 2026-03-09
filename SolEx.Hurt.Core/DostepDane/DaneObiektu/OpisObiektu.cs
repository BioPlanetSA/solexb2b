using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.DostepDane.DaneObiektu
{
    public class OpisObiektu
    {
        private PropertyInfo _klucz;
        private IList<OpisPolaObiektuBaza> _polaObiektuList;
        private static string _nazwaWyswietlana;
        private static string _opisWyswietlany;

        public IList<OpisPolaObiektuBaza> PolaObiektu
        {
            get { return _polaObiektuList; }
        }
        public PropertyInfo PoleKlucz { get { return _klucz; } }
        private OpisObiektu(PropertyInfo klucz, IList<OpisPolaObiektuBaza> pola)
        {
            _klucz = klucz;
            _polaObiektuList = pola;
        }

        private static Dictionary<Type, OpisObiektu> _opisyObiektow = new Dictionary<Type, OpisObiektu>();
        public static object lok = new object();

        public static OpisObiektu StworzOpisObiektu(Type typ)
        {
            OpisObiektu opisObiektu = null;

            lock (lok)
            {
                if (!_opisyObiektow.TryGetValue(typ, out opisObiektu))
                {
                    List<OpisPolaObiektuBaza> polaObiektu = new List<OpisPolaObiektuBaza>();
                    PropertyInfo[] propsty = typ.GetPublicProperties();

                    PropertyInfo klucz = propsty.FirstOrDefault(x => x.Name == "Id") ?? propsty.FirstOrDefault(x => x.GetCustomAttribute<PrimaryKeyAttribute>() != null);

                    foreach (PropertyInfo pi in propsty)
                    {
                        var atr = pi.GetCustomAttribute<WidoczneListaAdminAttribute>();
                        if (atr != null && (atr.WidocznyGdzie.IsEmpty() || atr.WidocznyGdzie.Contains(typ)))
                        {
                            polaObiektu.Add(new OpisPolaObiektuBaza(pi, null));
                        }
                    }
                    opisObiektu = new OpisObiektu(klucz, polaObiektu);
                    _opisyObiektow.Add(typ, opisObiektu);
                }
                return opisObiektu;
            }
        }
        //public static IList<OpisPolaObiektu> PobierzParametry(string modul)
        //{
        //    Type t = Type.GetType(modul, true);
        //    return PobierzParametry(t);
        //}

        public static IList<OpisPolaObiektu> PobierzParametry(Type t)
        {
            object zad = Activator.CreateInstance(t);
            List<OpisPolaObiektuBaza> pola = StworzOpisObiektu(t).PolaObiektu.ToList();
            return pola.Select(x => new OpisPolaObiektu(x, zad)).ToList();
            //pola.ForEach(x => x.PobierzWartoscPolaObiektu(zad));
            //return pola;
        }

        //ta metode musi zostac ze wzgledu na wykorzystanie
        public static List<OpisPolaObiektu> PobranieParametowObiektu(object obiekt, object id)
        {
            return PobranieParametowObiektu(obiekt, id, false);
        }
        public static List<OpisPolaObiektu> PobranieParametowObiektu(object obiekt, object id, bool usuwajPuste = false)
        {
            OpisObiektu opis = StworzOpisObiektu(obiekt.GetType());
            List<OpisPolaObiektuBaza> pola = opis.PolaObiektu.ToList();
            return PobierzOpisPolaObiektuNaPodstawieBazowego(pola, obiekt, id, usuwajPuste);
        }

        public static List<OpisPolaObiektu> PobierzOpisPolaObiektuNaPodstawieBazowego(List<OpisPolaObiektuBaza> pola, object obiekt, object id, bool usuwajPuste)
        {
            List<OpisPolaObiektu> wynik = new List<OpisPolaObiektu>();
            foreach (var p in pola)
            {
                p.TylkoDoOdczytu = !p.ParamatryWidocznosciAdmin.Edytowalne;
                p.IdentyfikatorObiektu = id == null ? "" : id.ToString();
                try
                {
                    wynik.Add(new OpisPolaObiektu(p, obiekt));
                    // p.PobierzWartoscPolaObiektu(obiekt);
                }
                catch
                {

                }
            }

            if (usuwajPuste)
            {
                wynik.RemoveAll(x => x.Wartosc == null || (x.TypPrzechowywanejWartosci == typeof(String) && string.IsNullOrEmpty(x.Wartosc as string)));
            }
            return wynik;
        }

    
        public string NazwaObiektuWyswietlanie
        {
            get { return _nazwaWyswietlana; }
        }
        public string OpisObiektuWyswietlanie(object obiekt, object id)
        {
            return _opisWyswietlany;
        }
  
    }

}
