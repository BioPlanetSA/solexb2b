using System;
using System.Collections.Generic;
using System.IO;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class ImportZPolaTextowego:ImportBaza
    {
      
        public override string LadnaNazwa
        {
            get {return "Import z pola textowego"; }
        }

        public override List<string> Rozszerzenia {get {return new List<string>();} } 

        public char[] Separatory = { ';', ':', ',', '|', '\t', ' ' };

        public override List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, Stream stumien)
        {
            bledy = new List<Komunikat>();
            List<PozycjaKoszykaImportowana> wynik = new List<PozycjaKoszykaImportowana>();
            
            string linia;
            StringReader strReader = new StringReader(dane);
            while ((linia = strReader.ReadLine())!=null)
            {
                if(string.IsNullOrEmpty(linia)) continue;
                string[] kolumny = linia.Split(Separatory, StringSplitOptions.RemoveEmptyEntries);
                if (kolumny.Length<2)
                {
                    bledy.Add(new Komunikat("W wierszu znajduje się zła ilości parametrów: " + linia, KomunikatRodzaj.danger, GetType().Name + " Błędne parametry"));
                    continue;
                }
                string kod = kolumny[0];
                string ilosc = kolumny[1];
                decimal tmp;
                if (!TextHelper.PobierzInstancje.SprobojSparsowac(ilosc, out tmp))
                {
                    bledy.Add(new Komunikat("W wierszu znajduje się zła ilości, wiersz: " + linia, KomunikatRodzaj.danger, GetType().Name + " Błędna Ilość"));
                    continue;
                }
                if (string.IsNullOrEmpty(ilosc)) continue;

                ZnajdzProdukt(kod, ilosc, linia, wynik, bledy);
                if (ZaDuzoElementow)
                {
                    break;
                }
            }

            return wynik;
        }
    }
}
