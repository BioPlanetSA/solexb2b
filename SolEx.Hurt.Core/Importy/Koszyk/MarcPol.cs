using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
   public class MarcPol : ImportBaza
    {
        public override string LadnaNazwa
        {
            get { return "Import zamówień MarcPol w formacie html"; }
        }

        public override List<string> Rozszerzenia
        {
            get {return new List<string>{"html","htm"};}
        }
        
        public override List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, Stream stumien)
       {

           bledy = new List<Komunikat>();
           List<PozycjaKoszykaImportowana> wynik = new List<PozycjaKoszykaImportowana>();
           const string frazaPoczatek = "<TABLE BORDER CELLSPACING=0 BORDERCOLOR=\"#000080\" CELLPADDING=1>";
           const string frazaKoniec = "<TABLE BORDER CELLSPACING=0  CELLPADDING=0 >";
           const string wyrazenieDane = @"<td align=\""\S*\""><b><Font [A-Z \""=\d]*><P>(?<wartosc>.*)</FONT></B></TD>";
           const string wyrazenieNaglowek =
               @"<td bgcolor=""[#\d\w]+""><b><Font Face=""Arial"" Size=2 Color=""[#\d\w]+""><p>(?<naglowek>.*)</font></b></td>";
           int pozPoczatkuPozycji = dane.IndexOf(frazaPoczatek, StringComparison.InvariantCultureIgnoreCase);
           int pozKoncaPozycji = dane.IndexOf(frazaKoniec, pozPoczatkuPozycji + 1,
                                              StringComparison.InvariantCultureIgnoreCase);
           if (pozPoczatkuPozycji >= 0 && pozKoncaPozycji >= 0)
           {
               int pocz = pozPoczatkuPozycji + frazaPoczatek.Length;
               int iloscZnakow = pozKoncaPozycji - pocz;
               string pozycje = dane.Substring(pocz, iloscZnakow).Trim().ToLower();
               string[] wiersze = pozycje.Split(new[] {"<tr>"}, StringSplitOptions.RemoveEmptyEntries);
               const RegexOptions myRegexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;
               int kolumnaIlosc = -1;
               int kolumnaKodKreskowy = -1;
               //  int kolumnaJednostka = -1;
               if (wiersze.Length > 0)
               {
                   int idx = 0;
                   Regex myRegex = new Regex(wyrazenieNaglowek, myRegexOptions);
                   foreach (Match myMatch in myRegex.Matches(wiersze[0]))
                   {

                       if (myMatch.Success)
                       {
                           string wartosc = myMatch.Groups["naglowek"].Value;
                           if (wartosc.Equals("Ilosc[JM]", StringComparison.InvariantCultureIgnoreCase))
                           {
                               kolumnaIlosc = idx;
                           }
                           else if (wartosc.Equals("Kodkres", StringComparison.InvariantCultureIgnoreCase))
                           {
                               kolumnaKodKreskowy = idx;
                           }
                           idx++;
                       }
                   }

               }
               Regex regDane = new Regex(wyrazenieDane, myRegexOptions);
               for (int i = 1; i < wiersze.Length; i++)
               {
                   string kod = "";
                   string ilosc = "";
                   //   string jm = "";
                   int idx = 0;
                   foreach (Match myMatch in regDane.Matches(wiersze[i]))
                   {
                       if (myMatch.Success)
                       {
                           string wartosc = myMatch.Groups["wartosc"].Value;
                           if (idx == kolumnaIlosc)
                           {

                               ilosc = wartosc;
                           }
                               //else if (idx == kolumnaJednostka)
                               //{
                               //    jm = wartosc;
                               //}
                           else if (idx == kolumnaKodKreskowy)
                           {
                               kod = wartosc;
                           }
                           idx++;
                       }
                   }
                   ZnajdzProdukt(kod, ilosc, wiersze[i], wynik, bledy);
                    if (ZaDuzoElementow)
                    {
                        break;
                    }


                }
           }
           return wynik;
       }
    }
}
