using System;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using SolEx.DBHelper;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.ERP.SubiektGT
{
    public enum TypObiektu{
        Towar = -14,
        Klient = 1
    }

    public class PoleWlasneWartosc{
        public int IdObiektu {get;set;}
        public TypObiektu Typ {get;set;}
        public string Wartosc {get;set;}
        public decimal WartoscDecimal {
            get
            {


                decimal temp = 0;
                if (!TextHelper.PobierzInstancje.SprobojSparsowac(Wartosc, out temp))
                {
                   // throw new Exception("Błąd parsowania wartości, wartość "+Wartosc+ " obiekt" +IdObiektu);
                }
                return temp;
            }
        }
        public int WartoscInt{get { return int.Parse(Wartosc); } } 
        public DateTime WartoscData{get{ return DateTime.Parse(Wartosc); } }
    }
      
    
    public static class PolaWlasne
    {
        private static Dictionary<TypObiektu, Dictionary<string, string>> _slownikPolWlasnychRozszerzonychIProstych = null;

        public static Dictionary<TypObiektu, Dictionary<string, string>> SlownikPolWlasnychRozszerzonychIProstych
        {

            get
            {
                if (_slownikPolWlasnychRozszerzonychIProstych == null)
                {
                    _slownikPolWlasnychRozszerzonychIProstych = new Dictionary<TypObiektu, Dictionary<string, string>>(2);
                    foreach (TypObiektu t in Enum.GetValues(typeof(TypObiektu)))
                    {
                        _slownikPolWlasnychRozszerzonychIProstych.Add(t, Baza.db.Dictionary<string, string>("select  pwp_Nazwa as nazwa , pwp_Pole as kolumna from pw_pole where pwp_TypObiektu =" + (int)t));

                        if (t == TypObiektu.Towar)
                        {

                            for (int i = 1; i <= 8; ++i)
                            {
                                string nazwa = Baza.db.Scalar<string>(string.Format("select Twp_Nazwa{0} from tw_Parametr", i));
                                if (!string.IsNullOrEmpty(nazwa))
                                {
                                    if (!_slownikPolWlasnychRozszerzonychIProstych.ContainsKey(t))
                                    {
                                        _slownikPolWlasnychRozszerzonychIProstych.Add(t, new Dictionary<string, string>());
                                    }
                                    _slownikPolWlasnychRozszerzonychIProstych[t].Add(nazwa, "Twp_Nazwa" + i);
                                }
                            }
                        }
                    }
                }
                return _slownikPolWlasnychRozszerzonychIProstych;
            }
        }


        public static PoleWlasneWartosc PobierzPole(TypObiektu typ, int idObiektu, string nazwaPola)
        {
            string kolumna = null;
            if (SlownikPolWlasnychRozszerzonychIProstych[typ].ContainsKey(nazwaPola))
            {
                kolumna = SlownikPolWlasnychRozszerzonychIProstych[typ][nazwaPola];
            }
            else
            {
                return null;
            }

            if (kolumna.StartsWith("Twp_Nazwa") && typ == TypObiektu.Towar)
            {
                string sql = string.Format("select tw_id as IdObiektu, {2} as Typ, {0} as Wartosc from tw__towar where tw_id = {1}", kolumna, idObiektu, (int)typ);
                return Baza.db.Single<PoleWlasneWartosc>(sql);
            }
            else
            {
                string sql = string.Format("select pwd_IdObiektu as IdObiektu, pwd_TypObiektu as Typ,cast(  {0} as varchar(max)) as Wartosc from pw_dane where pwd_TypObiektu = {1} AND pwd_IdObiektu = {2} AND {0} is not null",
                    kolumna, (int)typ, idObiektu);
                return Baza.db.Single<PoleWlasneWartosc>(sql);
            }
        }

        public static List<PoleWlasneWartosc> PobierzPoleDlaWszystkichObiektow(TypObiektu typ, string nazwaPola)
        {
             string kolumna =  null;
            if (SlownikPolWlasnychRozszerzonychIProstych[typ].ContainsKey(nazwaPola)){
            kolumna = SlownikPolWlasnychRozszerzonychIProstych[typ][nazwaPola];
            }else{
                return null;
            }

            if (kolumna.StartsWith("Twp_Nazwa") && typ == TypObiektu.Towar)
            {
                kolumna = kolumna.Replace("Twp_Nazwa", "Tw_pole");
                string sql = string.Format("select tw_id as IdObiektu, {1} as Typ, {0} as Wartosc from tw__towar", kolumna, (int)typ);
                return Baza.db.Select<PoleWlasneWartosc>(sql);
            }
            else
            {
                string sql = string.Format("select pwd_IdObiektu as IdObiektu, pwd_TypObiektu as Typ, cast(  {0} as varchar(max)) as Wartosc from pw_dane where pwd_TypObiektu = {1} AND {0} is not null",
                    kolumna, (int)typ);

                return Baza.db.Select<PoleWlasneWartosc>(sql);
            }
        }
    }



    
    
}
