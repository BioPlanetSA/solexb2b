using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;

namespace SolEx.Hurt.Sync.App.Modules_Tests.Rozne
{
    public static class GenerowanieObiektow
    {
        public static List<Produkt> WygenerujProdukty(int ile)
        {
            List<Produkt> lista = new List<Produkt>(ile);
            for (int i = 0; i < ile; i++)
            {
                Produkt produkt = new Produkt();
                produkt.Id = i + 1;
                produkt.Nazwa = i % 8 == 0 ? "ASD" : "adfasdfsdf";
                produkt.Rodzina = i % 4 == 0 ? "ASD" : null;
                lista.Add(produkt);
            }
            return lista;
        }

        public static List<Atrybut> WygenerujLosoweAtrybuty(RodzinyOryginalyIKopie m, int ile)
        {
            List<Atrybut> lista = new List<Atrybut>(ile);
            for (int i = 0; i < ile; i++)
            {
                Atrybut atr = m.WygenerujAtrybut(i.ToString());
                lista.Add(atr);
            }
            return lista;
        }

        public static List<Cecha> WygenerujLosoweCechy(RodzinyOryginalyIKopie m, List<Atrybut> atrybuty, int ileNaJedenAtrybut)
        {
            List<Cecha> lista = new List<Cecha>(ileNaJedenAtrybut * atrybuty.Count);

            foreach (Atrybut atrybut in atrybuty)
            {
                for (int i = 0; i < ileNaJedenAtrybut; i++)
                {
                    Cecha cecha = m.WygenerujCeche(atrybut.Nazwa + i.ToString(), atrybut);
                    lista.Add(cecha);
                }
            }
            return lista;
        }

        public static List<ProduktCecha> wygenerujListeLacznikowDlaCechy(List<Produkt> produkty, Cecha cecha, RodzinyOryginalyIKopie m)
        {
            List<ProduktCecha> lista = new List<ProduktCecha>();
            foreach (Produkt produkt in produkty)
            {
                ProduktCecha lacznik = m.WygenerujLacznikCech(cecha, produkt);
                lista.Add(lacznik);
            }

            return lista;
        }
    }
}
