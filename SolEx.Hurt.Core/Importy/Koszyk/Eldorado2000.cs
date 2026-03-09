using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class Eldorado2000 : BioPlanetCsv
    {
        public override string LadnaNazwa
        {
            get { return "Import w formacie Eldorado 2000 / Expedient"; }
        }

        public override List<string> Rozszerzenia
        {
            get { return new List<string>{".fs2"};}
        }

        protected override string NazwaKolumnyIlosc
        {
            get { return "ILOSC"; }
        }

        protected override string NazwaKolumnyKodKreskowy
        {
            get { return "KOD_KR"; }
        }
        
        public override List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, System.IO.Stream stumien)
        {
            bledy = new List<Komunikat>();
            List<PozycjaKoszykaImportowana> wynik = new List<PozycjaKoszykaImportowana>();
            const string poczatekPozycja = "[POZYCJE]";
            int pocz = dane.IndexOf(poczatekPozycja, StringComparison.Ordinal);
            if (pocz > -1)
            {
                string pozycje = dane.Substring(pocz + poczatekPozycja.Length).Replace("(","").Replace(")","");
                wynik = WykonajPrzetwarzanie(pozycje, bledy,"|");

            }
            return wynik;
        }
    }
}
