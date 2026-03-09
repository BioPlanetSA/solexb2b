namespace SolEx.Hurt.Core.Points.DAL
{
    using SolEx.Hurt.Core.Configuration;
    using SolEx.Hurt.Core.Points.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SolEx.Hurt.Core.Points.DAL;
    using SolEx.Hurt.DAL;

    public class RegulyDAO
    {
        internal static ListaRegul PobierzReguly()
        {
            MainDataContext DB = null;
            ListaRegul list = new ListaRegul();
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                IOrderedQueryable<punkty_reguly> result = from z in DB.punkty_regulies
                                                          orderby z.priorytet descending
                                                          select z;
                foreach (punkty_reguly x in result)
                {
                    PunktyRegula tr = new PunktyRegula {
                    DataRozpoczecia = x.data_wlaczenia,
                    DataZakonczenia = x.data_wylaczenia,
                    ID = x.regula_id,
                    Priorytet = x.priorytet,
                    PrzetwarzacKolejne = x.przetwarzac_kolejne,
                    Typ = x.typ,
                    Znak = x.znak,
                    KlientID = x.klient_id,
                    KategoriaKlientowID = x.kategoria_klientow_id
                    };
                    string[] wartosci = x.wartosci.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    tr.Wartosci = new decimal[wartosci.Length];
                    for (int i = 0; i < wartosci.Length; i++)
                    {
                        tr.Wartosci[i] = decimal.Parse(wartosci[i]);
                    }
                    list.WczytajRegule(tr);
                }
            }
            return list;
        }

        internal static void UsunReguly(List<int> ids1)
        {
            using (MainDataContext  DB = new MainDataContext(GlobalConfig.MainCS))
            {
                IQueryable<punkty_reguly> result = from r in DB.punkty_regulies
                                                   where ids1.Contains(r.regula_id)
                                                   select r;
                DB.punkty_regulies.DeleteAllOnSubmit<punkty_reguly>(result);
                DB.SubmitChanges();
            }
        }

        internal static PunktyRegula ZapiszRegule(PunktyRegula item)
        {
            MainDataContext DB = null;
            using (DB = new MainDataContext(GlobalConfig.MainCS))
            {
                punkty_reguly pr = new punkty_reguly {
                data_wlaczenia = item.DataRozpoczecia,
                data_wylaczenia = item.DataZakonczenia
                };
                if (DB.punkty_regulies.Count<punkty_reguly>() > 0)
                {
                    pr.priorytet = DB.punkty_regulies.Max<punkty_reguly, int>(p => p.priorytet) + 1;
                }
                else
                {
                    pr.priorytet = 1;
                }
                pr.przetwarzac_kolejne = item.PrzetwarzacKolejne;
                pr.typ = item.Typ;
                string wart = string.Empty;
                foreach (decimal d in item.Wartosci)
                {
                    wart = wart + d.ToString() + ";";
                }
                pr.wartosci = wart;
                pr.kategoria_klientow_id = item.KategoriaKlientowID;
                pr.klient_id = item.KlientID;
                pr.znak = item.Znak;
                DB.punkty_regulies.InsertOnSubmit(pr);
                DB.SubmitChanges();
                item.ID = pr.regula_id;
            }
            return item;
        }

        internal static void ZmienPriorytet(int id1, int id2)
        {
            using (MainDataContext DB = new MainDataContext(GlobalConfig.MainCS))
            {
                punkty_reguly result = DB.punkty_regulies.Single<punkty_reguly>(r => r.regula_id == id1);
                punkty_reguly second = DB.punkty_regulies.SingleOrDefault<punkty_reguly>(r => r.regula_id == id2);
                if (second != null)
                {
                    int fp = result.priorytet;
                    int sp = second.priorytet;
                    result.priorytet = sp;
                    second.priorytet = fp;
                }
                DB.SubmitChanges();
            }
        }
    }
}

