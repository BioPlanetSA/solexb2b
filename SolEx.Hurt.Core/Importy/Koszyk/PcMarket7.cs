using System;
using System.Collections.Generic;
using System.IO;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class PcMarket7 : ImportBaza
    {
        public override string LadnaNazwa
        {
            get { return "Import zamówień z programu PC Market 7 (pliki txt)"; }
        }

        public override List<string> Rozszerzenia
        {
            get { return new List<string>{"txt"};}
        }

        private Dictionary<string, Tuple<int, int>> WyliczOdleglosci(IEnumerable<string> wiersze)
        {
            Dictionary<string, Tuple<int, int>> wynik = new Dictionary<string, Tuple<int, int>>();
            foreach (string s in wiersze)
            {
                if (s.IndexOf("kod",StringComparison.InvariantCultureIgnoreCase)<0 || s.IndexOf("ilość",StringComparison.InvariantCultureIgnoreCase)<0)
                {
                    continue;
                }
                wynik.Add("kod", ZnajdzGranice(s, "kod"));
                wynik.Add("ilość", ZnajdzGranice(s, "ilość"));
                break;
            }
            return wynik;
        }

        private List<int> PozycjeKoncowWierszy(string wiersz)
        {
            List<int> wynik=new List<int>();
            int idx = -1;
            do
            {
              idx=  wiersz.IndexOf("|",idx+1, StringComparison.Ordinal);
                if (idx> -1)
                {
                    wynik.Add(idx);
                }
            } 
            while (idx>-1 && idx+1<wiersz.Length);
            return wynik;
        }

        private Tuple<int, int> ZnajdzGranice(string wiersz, string tekst)
        {
            int idxpozycji = wiersz.ToLower().IndexOf(tekst.ToLower(), StringComparison.Ordinal);
            List<int> pozycje = PozycjeKoncowWierszy(wiersz);
            for (int i = 0; i < pozycje.Count-1; i++)
            {
                if (pozycje[i] < idxpozycji && pozycje[i + 1] > idxpozycji)
                {
                    return new Tuple<int, int>(pozycje[i],pozycje[i+1]);
                }
            }
            throw new Exception($"Nie znaleziono granic - wiersz: [ { wiersz} ],  test szuakny: [ {tekst} ]");
        }

        public override List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, Stream stumien)
        {
            dane = dane.Trim();
            List<PozycjaKoszykaImportowana> wynik=new List<PozycjaKoszykaImportowana>();
            bledy=new List<Komunikat>();
            string[] wiersze = dane.Split(new[] {dane.Contains(Environment.NewLine)? Environment.NewLine:"\n" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, Tuple<int, int>> kolumny = WyliczOdleglosci(wiersze);
          
            foreach (string s in wiersze)
            {
               if (s.Length < kolumny["kod"].Item2)
               {
                   continue;
               }
                string kod = s.Substring(kolumny["kod"].Item1 + 1, kolumny["kod"].Item2 - kolumny["kod"].Item1-1).Trim();
                if (s.Length < kolumny["ilość"].Item2)
               {
                   continue;
               }
                string ilosc = s.Substring(kolumny["ilość"].Item1 + 1, kolumny["ilość"].Item2 - kolumny["ilość"].Item1-1).Trim();
                if (!string.IsNullOrEmpty(kod) && !string.IsNullOrEmpty(ilosc))
                {
                    ZnajdzProdukt(kod, ilosc, s, wynik, bledy);
                }
                if (ZaDuzoElementow)
                {
                    break;
                }
            }
            return wynik;
        }
    }
}
