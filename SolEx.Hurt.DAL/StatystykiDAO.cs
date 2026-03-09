namespace SolEx.Hurt.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.SqlClient;
    using SolEx.Hurt.Helpers;
    using SolEx.Hurt.Model;

    public class StatystykiDAO
    {
        public static List<Wpis> PobierzZdarzenia(int typ, DateTime? dataPoczatkowa, DateTime? dataKoncowa)
        {

            List<Wpis> wynik = null;
                DateTime min = dataPoczatkowa.HasValue ? dataPoczatkowa.Value : DateTime.MinValue;
                DateTime max = dataKoncowa.HasValue ? dataKoncowa.Value : DateTime.MaxValue;
                using (MainDataContext DB = MainDAO.GetContext())
                {
                    List<statystyki> q = (from p in DB.statystykis
                                          where ((p.typ_zdarzenia == typ) && (p.data_dodania.Date > min)) && (p.data_dodania.Date <= max)
                                          select p).ToList<statystyki>();
                    wynik = new List<Wpis>(q.Count);
                    foreach (statystyki s in q)
                    {
                        Wpis w = new Wpis
                        {
                            DataDodania = s.data_dodania,
                            Typ = (StatystykiZdarzenie)s.typ_zdarzenia,
                            Wartosc1 = (s.wartosc1 ?? "").Replace("\"", "").Replace("'", ""),
                            Wartosc2 = (s.wartosc2 ?? "").Replace("\"", "").Replace("'", ""),
                            Wartosc3 = (s.wartosc3 ?? "").Replace("\"", "").Replace("'", ""),
                            Wartosc4 = (s.wartosc4 ?? "").Replace("\"", "").Replace("'", ""),
                            Wartosc5 = (s.wartosc5 ?? "").Replace("\"", "").Replace("'", ""),
                            Wartosc6 = (s.wartosc6 ?? "").Replace("\"", "").Replace("'", ""),
                            Wartosc7 = (s.wartosc7 ?? "").Replace("\"", "").Replace("'", ""),
                            Wartosc8 = (s.wartosc8 ?? "").Replace("\"", "").Replace("'", ""),
                            Wartosc9 = (s.wartosc9 ?? "").Replace("\"", "").Replace("'", ""),
                            Wartosc10 = (s.wartosc10 ?? "").Replace("\"", "").Replace("'", "")
                        };
                        wynik.Add(w);
                    }
                }
            return wynik;
        }

        public static List<Wpis> PobierzZdarzenia(string sql,List<string> pars)
        {
            List<Wpis> list = new List<Wpis>();
            SqlConnection conn = new SqlConnection(Config.MainCS);
            SqlCommand cmd = new SqlCommand(sql, conn);
            
            for (int i = 0; i < pars.Count; i++)
            {
                cmd.Parameters.AddWithValue("@p"+(i+1).ToString(),pars[i]);
            }
            conn.Open();
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                Wpis tmp = new Wpis();
                tmp.Wartosc1 = DataHelper.dbs("wartosc1", r);
                tmp.Wartosc2 = DataHelper.dbs("wartosc2", r);
                tmp.Wartosc3 = DataHelper.dbs("wartosc3", r);
                tmp.Wartosc4 = DataHelper.dbs("wartosc4", r);
                tmp.Wartosc5 = DataHelper.dbs("wartosc5", r);
                tmp.Wartosc6 = DataHelper.dbs("wartosc6", r);
                tmp.Wartosc7 = DataHelper.dbs("wartosc7", r);
                tmp.Wartosc8 = DataHelper.dbs("wartosc8", r);
                tmp.Wartosc9 = DataHelper.dbs("wartosc9", r);
                tmp.Wartosc10 = DataHelper.dbs("wartosc10", r);
                list.Add(tmp);
            }
            r.Close();
            conn.Close();
            conn.Dispose();

            return list;
        }
        public static void ZapiszZdarzenie(int typ, List<string> dane)
        {
            if (dane.Count != 0)
            {
                using (MainDataContext DB = new MainDataContext(Config.MainCS))
                {
                    statystyki s = new statystyki {
                    typ_zdarzenia = typ,
                    wartosc1 = dane[0],
                    wartosc2 = (dane.Count > 1) ? dane[1] : "",
                    wartosc3 = (dane.Count > 2) ? dane[2] : "",
                    wartosc4 = (dane.Count > 3) ? dane[3] : "",
                    wartosc5 = (dane.Count > 4) ? dane[4] : "",
                    wartosc6 = (dane.Count > 5) ? dane[5] : "",
                    wartosc7 = (dane.Count > 6) ? dane[6] : "",
                    wartosc8 = (dane.Count > 7) ? dane[7] : "",
                    wartosc9 = (dane.Count > 8) ? dane[8] : "",
                    wartosc10 = (dane.Count > 9) ? dane[9] : "",
                    data_dodania = DateTime.Now
                    };
                    DB.statystykis.InsertOnSubmit(s);
                    DB.SubmitChanges();
                }
            }
        }

        public static List<Wpis> PobierzZdarzenia(int typ, DateTime? dataPoczatkowa, DateTime? dataKoncowa, int strona, int ilosc, string sortowanieKolumna, string sortowanieKierunekt, out int count)
        {
            MainDataContext DB = null;
            List<Wpis> wynik = null;
            try
            {
                DateTime min = dataPoczatkowa.HasValue ? dataPoczatkowa.Value : DateTime.MinValue;
                DateTime max = dataKoncowa.HasValue ? dataKoncowa.Value : DateTime.MaxValue;
                DB = new MainDataContext(Config.MainCS);
                List<statystyki> q = (from p in DB.statystykis
                                      where ((p.typ_zdarzenia == typ) && (p.data_dodania.Date > min)) && (p.data_dodania.Date <= max)
                                      select p).Skip((strona-1)*ilosc).Take(ilosc).ToList<statystyki>();
                count= DB.statystykis.Where(p=>p.typ_zdarzenia == typ && p.data_dodania.Date > min && p.data_dodania.Date <= max).Count();
                wynik = new List<Wpis>(q.Count);
                foreach (statystyki s in q)
                {
                    Wpis w = new Wpis
                    {
                        DataDodania = s.data_dodania,
                        Typ = (StatystykiZdarzenie)s.typ_zdarzenia,
                        Wartosc1 = s.wartosc1.Replace("\"", "").Replace("'", ""),
                        Wartosc2 = s.wartosc2.Replace("\"", "").Replace("'", ""),
                        Wartosc3 = s.wartosc3.Replace("\"", "").Replace("'", ""),
                        Wartosc4 = s.wartosc4.Replace("\"", "").Replace("'", ""),
                        Wartosc5 = s.wartosc5.Replace("\"", "").Replace("'", ""),
                        Wartosc6 = s.wartosc6.Replace("\"", "").Replace("'", ""),
                        Wartosc7 = s.wartosc7.Replace("\"", "").Replace("'", ""),
                        Wartosc8 = s.wartosc8.Replace("\"", "").Replace("'", ""),
                        Wartosc9 = s.wartosc9.Replace("\"", "").Replace("'", ""),
                        Wartosc10 = s.wartosc10.Replace("\"", "").Replace("'", "")
                    };
                    wynik.Add(w);
                }
            }
            finally
            {
                if (DB != null)
                {
                    DB.Dispose();
                }
            }
            return wynik;
        }
    }
}

