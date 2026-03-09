namespace SolEx.Hurt.Core.Points.DAL
{
    using SolEx.Hurt.Core.Configuration;
    using SolEx.Hurt.Core.Points.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Runtime.InteropServices;
    using SolEx.Hurt.Core.Points.DAL;
    using SolEx.Hurt.DAL;

    public class PunktyDAO
    {
        internal static void DodajPunkty(long ilosc, int UserID,string desc)
        {
            MainDataContext DB = null;
            try
            {
                punkty tmp = null;
                DB = new MainDataContext(GlobalConfig.MainCS);
                tmp = new punkty {
                ilosc_punktow = ilosc,
                opis = !string.IsNullOrEmpty(desc)?desc: " Dodane przez administratora",
                klient_id = UserID,
                data_dodania = DateTime.Now,
                aktywne = true,
                dokument = ""
                };
                DB.punkties.InsertOnSubmit(tmp);
                DB.SubmitChanges();
            }
            finally
            {
                if (DB != null)
                {
                    DB.Dispose();
                }
            }
        }

        internal static List<PunktyHistroria> PobierzHistorie(int UserId)
        {
            List<PunktyHistroria> historia = new List<PunktyHistroria>();
            MainDataContext DB = new MainDataContext(GlobalConfig.MainCS);
            IQueryable<punkty> result = from p in DB.punkties
                                        where p.aktywne && ((p.klient_id == UserId) || (p.klienci.klienci1.klient_id == UserId))
                                        select p;
            foreach (punkty ph in result)
            {
                PunktyHistroria tmp = new PunktyHistroria {
                Aktywne = ph.aktywne,
                DataDodania = ph.data_dodania,
                Id = ph.punkty_id,
                Faktura = ph.dokument,
                IdKlienta = ph.klient_id,
                Ilosc = ph.ilosc_punktow,
                Opis = ph.opis
                };
                historia.Add(tmp);
            }
            return historia;
        }

        internal static int PobierzPunktyIloscWpisow(int p)
        {
            MainDataContext DB = null;
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                return (from r in DB.punkties
                        where (r.klient_id == p) || (r.klienci.klient_nadrzedny_id == p)
                        select r).Count<punkty>();
            }
        }

        internal static decimal PobierzPunktyUzytkownika(int id)
        {
            using (MainDataContext   DB = new MainDataContext(GlobalConfig.MainCS))
            {
                return (from p in DB.punkties
                        where (p.klient_id == id) && p.aktywne
                        select p).ToList<punkty>().Sum<punkty>(p => (long)p.ilosc_punktow);
            }
        }

        internal static PunktyBox PobierzPunktyUzytkownika(int UserID, int skip, int take, string QType, string Query, string SortName, string SortOrder, out int TotalCount)
        {
            MainDataContext DB = null;
            PunktyBox comp = new PunktyBox();
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                List<punkty> q = (from p in DB.punkties
                                  where p.aktywne && (p.klient_id == UserID)
                                  select p).OrderBy<punkty>(("punkty_id desc," + SortName + " " + SortOrder), new object[0]).ToList<punkty>();
                TotalCount = q.Count;
                comp.Suma = q.Sum<punkty>(p => p.ilosc_punktow);
                foreach (punkty r in q)
                {
                    PunktyHistroria ph = new PunktyHistroria {
                    DataDodania = r.data_dodania,
                    Opis = r.opis,
                    Ilosc = r.ilosc_punktow,
                    IdKlienta = r.klient_id,
                    Id = r.punkty_id
                    };
                    IQueryable<punkty> z = from p in DB.punkties
                                           where (p.aktywne && (p.punkty_id <= ph.Id)) && ((p.klient_id == ph.IdKlienta) || (p.klienci.klient_nadrzedny_id == ph.IdKlienta))
                                           select p;
                    ph.Lacznie = (z != null) ? z.Sum<punkty>(p => (long)p.ilosc_punktow) : 0L;
                    comp.Punkty.Add(ph);
                }
            }
            comp.Punkty = comp.Punkty.Skip<PunktyHistroria>(skip).Take<PunktyHistroria>(take).ToList<PunktyHistroria>();
            return comp;
        }

        internal static List<PunktyUzytkownika> PobierzPunktyUzytkownikow(string SortName, string SortOrder, string QType, string Query, int Page, int RP, out int Count)
        {
            List<PunktyUzytkownika> historia = new List<PunktyUzytkownika>();
            using (MainDataContext DB = new MainDataContext(GlobalConfig.MainCS))
            {
                var klients = ((Query == string.Empty) ? (from p in DB.kliencis
                                                          where p.aktywny && p.klient_nadrzedny_id==null
                                                          select new
                                                          { ID = p.klient_id, Nazwa = p.nazwa, suma = (from pu in DB.punkties
                                                                         where (pu.klient_id == p.klient_id) && pu.aktywne
                                                                                                       select pu).ToList().Sum<punkty>(pu => (long?)pu.ilosc_punktow)
                                                          }) : (from p in DB.kliencis
                                                                               where p.nazwa.ToLower().Contains(Query.ToLower()) && p.aktywny && p.klient_nadrzedny_id == null
                                                                                    select new
                                                                                    { ID = p.klient_id, Nazwa = p.nazwa, suma = (from pu in DB.punkties
                                                                              where (pu.klient_id == p.klient_id) && pu.aktywne
                                                                                                                                 select pu).ToList().Sum<punkty>(pu => (long?)pu.ilosc_punktow)
                                                                                    })).ToList();
                Count = klients.Count();
                var klients2 = klients.Skip(((Page - 1) * RP)).Take(RP);
                foreach (var r in klients2)
                {
                    PunktyUzytkownika tmp = new PunktyUzytkownika {
                    IdUzytkownika = r.ID,
                    NazwaUzytkownika = r.Nazwa,
                    Suma = r.suma.HasValue ? r.suma.Value : 0L
                    };
                    historia.Add(tmp);
                }
            }
            return historia;
        }

        internal static int UaktualnijPunkty(int ileDni, bool uaktualniajTylkoZaplacone)
        {
            int ile = 0;
            using (MainDataContext DB = new MainDataContext(GlobalConfig.MainCS))
            {
                /*zabezpieczenie na wypadek równoczeniego odpalenia dwóch procesów wyliczaj¹cych punkty, jeœli któryœ dokument jest kilka razy wpisany
                , to  zostawia tylko jeden wpis. Sytuacja mo¿e nast¹piæ jeœli wylicznie punktów zosta³o odpalone kilka razy równoczeœnie. Np w adminie i us³udze, lub kilka razy w adminie
*/
                List<punkty> doublety = new List<punkty>();
                foreach (var x in DB.punkties.Where(p=>p.dokument_id!=null).ToList())
                {
                    if(!doublety.Any(p=>p.punkty_id== x.punkty_id)) // sprawdzamy czy wybrany element ju¿ nie jest przeznaczony do usuniêcia
                    {
                     doublety.AddRange(DB.punkties.Where(p => p.dokument_id == x.dokument_id && p.punkty_id!=x.punkty_id)); //pobieramy punkty do tego samego dokumentu, które trzeba usun¹æ
                    }
                
                }
                DB.punkties.DeleteAllOnSubmit(doublety);
                DB.SubmitChanges();

                IQueryable<punkty> res = uaktualniajTylkoZaplacone ? (from p in DB.punkties
                                                                      where ((p.data_dodania.AddDays(ileDni) < DateTime.Now) && !p.aktywne) && (from x in DB.historia_dokumenties
                                                                                                         where x.zaplacono
                                                                                                         select x.nazwa_dokumentu).Contains<string>(p.dokument)
                                                                      select p) : (from p in DB.punkties
                                            where (p.data_dodania.AddDays(ileDni) < DateTime.Now) && !p.aktywne
                                            select p);
                foreach (punkty x in res)
                {
                    x.aktywne = true;
                    ile++;
                }
                DB.SubmitChanges();
            }
            return ile;
        }

        internal static void UsunPunkty(int id)
        {
            using (MainDataContext DB = new MainDataContext(GlobalConfig.MainCS))
            {
                punkty tmp = DB.punkties.SingleOrDefault<punkty>(p => (p.punkty_id == id) /*&& (p.ilosc_punktow != 0L)*/);
                if (tmp != null)
                {
                    //tmp.ilosc_punktow = 0L;
                    //tmp.opis = tmp.opis + " Zmienione przez administratora";
                    DB.punkties.DeleteOnSubmit(tmp);
                    DB.SubmitChanges();
                }
            }
        }

        internal static void ZapiszPunkty(List<PunktyHistroria> dodaneWpisy)
        {
            MainDataContext DB = null;
            try
            {
                DB = new MainDataContext(GlobalConfig.MainCS);
                foreach (PunktyHistroria ph in dodaneWpisy)
                {
                    punkty tmp = new punkty {
                        dokument_id = ph.Id,
                    aktywne = (ph.Ilosc > 0L) ? false : true,
                    data_dodania = ph.DataDodania,
                    dokument = ph.Faktura,
                    ilosc_punktow = ph.Ilosc,
                    klient_id = DB.kliencis.First(p=>p.zrodlo_id== ph.IdKlienta).klient_id,
                    opis = ph.Opis
                    };
                    DB.punkties.InsertOnSubmit(tmp);
                }
                DB.SubmitChanges();
            }
            finally
            {
                if (DB != null)
                {
                    DB.Dispose();
                }
            }
        }
    }
}

