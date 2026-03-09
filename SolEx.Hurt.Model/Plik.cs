using System.IO;
using ServiceStack.DataAnnotations;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class Plik : IHasIntId, IStringIntern
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Nazwa { get; set; }

        [StringInternuj]
        public string Sciezka { get; set; }

        private DateTime _datetime = DateTime.MinValue;
        public int? SzerokoscOkna { get; set; }
        public int? WysokoscOkna { get; set; }
        public string KlasaCss { get; set; }

        public DateTime Data
        {
            get { return _datetime; }
            set { _datetime = value.AddMilliseconds(-value.Millisecond); }
        }

        [Ignore]
        public string DanePlikBase64 { get; set; }

        public SposobOtwierania? SposobOtwierania { get; set; }

        [Ignore]
        public string NazwaBezRozszerzenia
        {
            get
            {
                //to powoduje problem jeśli w nazwie pliku jest też więcej kropek np xcośtam.innytekst.jpg
                //to wytnie nazwę do pierwszej kropki a nie ostatniej
                //if (Nazwa.Count(a => a == '.') > 1)
                //    return Nazwa.Substring(0, Nazwa.LastIndexOf(".", System.StringComparison.Ordinal));
                try
                {
                    return System.IO.Path.GetFileNameWithoutExtension(Nazwa);
                }
                catch
                {
                    //czasem nie potrafi podac takiej sciezki - szansa jeszcze zwrocic wszystko
                    return Nazwa.Substring(0, Nazwa.LastIndexOf('.'));
                }
            }
            set
            {
                //if (!value.EndsWith(Rozszerzenie) && !Nazwa.EndsWith(Rozszerzenie))
                Nazwa = value + Rozszerzenie;

                //else Nazwa = value;
            }
        }



        public string Rozszerzenie
        {
            get
            {
                try
                {
                    return System.IO.Path.GetExtension(Nazwa);
                }
                catch
                {
                    //czasem nie dziala standard
                    return Nazwa.Substring(Nazwa.LastIndexOf('.'));
                }
            }
        }

        /// <summary>
        /// Nazwa i ścieżka do pliku NIE enkodowana HTML, Propertis zawsze zwraca sciezke z '/' na poczatku jeżeli ktoś nie chce musi zrobic trima
        /// </summary>
        [Ignore]
        public string SciezkaWzgledna => Path.Combine(Path.DirectorySeparatorChar.ToString(), this.Sciezka, this.Nazwa);

        /// <summary>
        /// Sciezka bezwględna zawiera w sobie aktualny katalog, Path.GetFullPath - zwraca sciezke z odpowiednimi linkami, ToLower jest potrzebe bo mamy containsy (przeszukujemy kolekcje) gdzie nie da sie wsadzić ignore case
        /// </summary>
        [Ignore]
        public virtual string SciezkaBezwzgledna => Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.Sciezka.TrimStart('/'), this.Nazwa)).ToLower();

        public int Rozmiar { get; set; }

        /// <summary>
        /// Interpretacja obrazkowa html pliku na froncie
        /// </summary>
        public string HtmlPrzycisku { get; set; }

        public RodzajPliku? RodzajPliku { get; set; }

        public Plik()
        {
            RodzajPliku = Enums.RodzajPliku.Zdjecie;
            SposobOtwierania = Enums.SposobOtwierania.Link;
        }

        /// <summary>
        /// konstruktor z parametrami wykorzystywany w modulach plikowych
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nazwa"></param>
        /// <param name="rozmiar"></param>
        /// <param name="sciezka"></param>
        /// <param name="id"></param>
        public Plik(DateTime data, string nazwa,int rozmiar, string sciezka, int id=0)
        {
            Data = data;
            Nazwa = nazwa;
            nazwaLokalna = nazwa;
            Rozmiar = rozmiar;
            Sciezka = sciezka;
            Id = id;
        }

        public Plik(Plik p)
        {
            if (p == null)
            {
                return;
            }
            Id = p.Id;
            Nazwa = p.Nazwa;
            Sciezka = p.Sciezka;
            Data = p.Data;
            Rozmiar = p.Rozmiar;
            RodzajPliku = p.RodzajPliku;
            DoPobrania = p.DoPobrania;
            HtmlPrzycisku = p.HtmlPrzycisku;
            KlasaCss = p.KlasaCss;
            SposobOtwierania = p.SposobOtwierania;
            SzerokoscOkna = p.SzerokoscOkna;
            WysokoscOkna = p.WysokoscOkna;
            nazwaLokalna = p.nazwaLokalna;
            lokalnaSciezkaDoZapisuPliku = p.lokalnaSciezkaDoZapisuPliku;
        }

        public bool CzyTeSamePliki(Plik y)
        {
            bool wynik = Nazwa.Equals(y.Nazwa, StringComparison.InvariantCultureIgnoreCase) 
                && Data.Year == y.Data.Year 
                && Data.Month == y.Data.Month
                && Data.Day == y.Data.Day 
                && Data.Hour == y.Data.Hour 
                && Data.Minute == y.Data.Minute 
                && Data.Second == y.Data.Second;
            return wynik;
        }


        public void PoprawNazwaPlikuDlaURL()
        {
            //tu nie może być hasha # !!!
            Regex myRegex = new Regex(@"[^0-9a-zA-Z-,$_.]+");
            NazwaBezRozszerzenia = myRegex.Replace(NazwaBezRozszerzenia, "-");
        }

        [Ignore]
        public string nazwaLokalna { get; set; }

        [Ignore]
        public bool DoPobrania { get; set; }

        [Ignore]
        public string lokalnaSciezkaDoZapisuPliku { get; set; }

    }
}
