using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy
{
    public class ImportObiektuCsv
    {
        protected virtual bool JestNaglowek
        {
            get { return true; }
        }
        public Dictionary<string, List<OpisPolaObiektu>> Przetworz(Type typ,string dane, out List<Komunikat> bledy)
        {
            bledy = new List<Komunikat>();
            Dictionary<string, List<OpisPolaObiektu>> wynik = WykonajPrzetwarzanie(typ,dane, bledy);
            return wynik;
        }

        protected virtual void PrzetworzNaglowek(CsvReader csv, ref Dictionary<string, int> listaPol)
        {
            csv.Read();
            csv.ReadHeader();
            for (int i = 0; i < csv.FieldHeaders.Length; i++)
            {
                string pole = csv.FieldHeaders[i].Trim();
                foreach (string key in listaPol.Keys)
                {
                    if (!key.Equals(pole, StringComparison.InvariantCultureIgnoreCase)) continue;
                    listaPol[key] = i;
                    break;
                }
            }
        }

        protected Dictionary<string, List<OpisPolaObiektu>> WykonajPrzetwarzanie(Type typ, string tekst, List<Komunikat> bledy, string separator = ";")
        {
            Dictionary<string, List<OpisPolaObiektu>> wynik = new Dictionary<string, List<OpisPolaObiektu>>();

            OpisObiektu obiekt = OpisObiektu.StworzOpisObiektu(typ);
            Dictionary<string, int> lokalizacjaPol = obiekt.PolaObiektu.ToDictionary(x => x.NazwaWyswietlana, x => -1);
            CsvReader csv = new CsvReader(new StringReader(tekst));
            csv.Configuration.Delimiter = separator;
            csv.Configuration.HasHeaderRecord = JestNaglowek;
            PrzetworzNaglowek(csv, ref lokalizacjaPol);

            while (csv.Read()) //petla wyciagajaca naglowki kolumna
            {
                if (lokalizacjaPol[NazwaKluczaObiektu] == -1)
                {
                    if (lokalizacjaPol[NazwaKluczaObiektu] == -1)
                    {
                        bledy.Add(new Komunikat(string.Format("W pliku brakuje kolumny {0}", NazwaKluczaObiektu), KomunikatRodzaj.danger, "ImportCsv"));
                        break;
                    }
                }

                List<OpisPolaObiektu> pola = new List<OpisPolaObiektu>();
                string klucz = null;
                foreach (var pole in lokalizacjaPol.Where(x => x.Value >= 0))
                {
                    if (pole.Key.Equals(NazwaKluczaObiektu))
                    {
                        klucz = csv[pole.Value];
                        continue;
                    }
                    var tmp1 = obiekt.PolaObiektu.FirstOrDefault(x => x.NazwaWyswietlana.Equals(pole.Key) && (x.ParamatryWidocznosciAdmin.Edytowalne || x.ParamatryWidocznosciAdmin.EdytowalneInline));
                    if (tmp1!=null)
                    {
                        OpisPolaObiektu opisPola = new OpisPolaObiektu(csv[pole.Value], tmp1);
                        pola.Add(opisPola);
                    }
                }

                if (string.IsNullOrEmpty(klucz))
                {
                    bledy.Add(new Komunikat(string.Format("W pliku brakuje klucza w wierszu {0}", csv.Row),KomunikatRodzaj.danger, "ImportCsv"));
                    continue;
                }

                if (!wynik.ContainsKey(klucz))
                {
                    wynik.Add(klucz, pola);
                }
            }
            return wynik;
        }


        protected virtual string NazwaKluczaObiektu
        {
            get { return "Id"; }
        }
    }
}
