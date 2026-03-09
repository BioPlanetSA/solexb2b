using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class ImportCsv:ImportBaza
    {
      
        public override string LadnaNazwa => "Import w formacie CSV";

        public override List<string> Rozszerzenia => new List<string>{"csv"};

        protected virtual bool JestNaglowek => true;

        public override List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, Stream stumien)
        {
            bledy = new List<Komunikat>();
            List<PozycjaKoszykaImportowana> wynik = WykonajPrzetwarzanie(dane, bledy);
            return wynik;
        }

        protected virtual void PrzetworzNaglowek(CsvReader csv, ref int kolumnaKod, ref int kolumnailosc)
        {
            csv.Read();
            csv.ReadHeader();
            for (int i = 0; i < csv.FieldHeaders.Length; i++)
            {
                string pole = csv.FieldHeaders[i];
                if (pole == NazwaKolumnyKodKreskowy)
                {
                    kolumnaKod = i;
                }
                if (pole == NazwaKolumnyIlosc)
                {
                    kolumnailosc = i;
                }
            }
        }
        protected List<PozycjaKoszykaImportowana> WykonajPrzetwarzanie(string tekst, List<Komunikat> bledy, string separator = ";")
        {
            List<PozycjaKoszykaImportowana> wynik = new List<PozycjaKoszykaImportowana>();
            int nrKolumnyKodKreskowy = -1;
            int nrKolumnyilosc = -1;
            CsvReader csv = new CsvReader(new StringReader(tekst));
            csv.Configuration.Delimiter = separator;
            csv.Configuration.HasHeaderRecord = JestNaglowek;
            if (JestNaglowek)
            {
                PrzetworzNaglowek(csv, ref nrKolumnyKodKreskowy, ref nrKolumnyilosc);
            }
            while (csv.Read()) //petla wyciagajaca naglowki kolumna
            {
                if (nrKolumnyilosc > -1 && nrKolumnyKodKreskowy > -1)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string t in csv.CurrentRecord)
                    {
                        sb.Append($"{t};");
                    }
                    string kod = csv.CurrentRecord.Length > nrKolumnyKodKreskowy ? csv[nrKolumnyKodKreskowy] : "";
                    string ilosc = csv.CurrentRecord.Length > nrKolumnyKodKreskowy ? csv[nrKolumnyilosc] : "";
                    if (string.IsNullOrEmpty(ilosc)) continue;

                    ZnajdzProdukt(kod, ilosc, sb.ToString(), wynik, bledy);
                    if (ZaDuzoElementow)
                    {
                        break;
                    }
                }
            }
            if (nrKolumnyKodKreskowy == -1 || nrKolumnyilosc == -1)
            {
                bledy.Add(new Komunikat($"W pliku brakuje kolumny {NazwaKolumnyIlosc} lub {NazwaKolumnyKodKreskowy}", KomunikatRodzaj.danger, "ImportCsv"));
            }
            return wynik;
        }


        protected virtual string NazwaKolumnyIlosc => "ilosc";

        protected virtual string NazwaKolumnyKodKreskowy => "kod_kreskowy";
    }
}
