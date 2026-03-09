using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("TrescKolumna")]
    public class TrescKolumnaBll : TrescKolumna, IPoleJezyk, IObiektPrzechowujacyKontrolke
    {
        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;
        public TrescKolumnaBll() { }
        public TrescKolumnaBll(TrescKolumnaBll bazowa):base()
        {
            this.DodatkoweKlasyCssKolumny = bazowa.DodatkoweKlasyCssKolumny;
            this.DodatkoweKlasyCssReczneKolumny = bazowa.DodatkoweKlasyCssReczneKolumny;
            this.Dostep = bazowa.Dostep;
            this.Kolejnosc = bazowa.Kolejnosc;
            this.KolorTla = bazowa.KolorTla;
            this.Marginesy = bazowa.Marginesy;
            this.ObrazekTla = bazowa.ObrazekTla;
            this.OpisKontenera = bazowa.OpisKontenera;
            this.Paddingi = bazowa.Paddingi;
            this.ParametryKontrolkiSpecyficzne = bazowa.ParametryKontrolkiSpecyficzne;
            this.RodzajKontrolki = bazowa.RodzajKontrolki;
            this.Szerokosc = bazowa.Szerokosc;
            this.TrescWierszId = bazowa.TrescWierszId;
            this.JezykId = bazowa.JezykId;
        }

        [Ignore]
        public int JezykId { get; set; }

        [Ignore]
        public IObrazek Tlo
        {
            get
            {
                if (ObrazekTla != null)
                {
                    return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ObrazekTla.Value);
                }
                return null;
            }
        }

        private KontrolkaTresciBaza _kontrolka;

        /// <summary>
        /// Tworzenie kontrolki do widoku dla klienta
        /// </summary>
        /// <returns></returns>
        public IKontrolkaTresciBaza Kontrolka()
        {
                if (_kontrolka == null)
                {
                    try
                    {
                        _kontrolka = this.StworzKontrolke<KontrolkaTresciBaza>();
                }
                    catch (Exception e)
                    {
                        _kontrolka = new KontrolkaUsunieta(RodzajKontrolki) { Tresc = string.Format("W tym miejscu znajdowała się kontrolka: {0}. <br/> Błąd: {1}", RodzajKontrolki, e.Message) };
                    }
                }
                return _kontrolka;
        }

        public  Dictionary<string, object> ParametryLokalizowane()
        {
            Dictionary<string, object> nadpisywane = null;
            if (JezykId != Calosc.Konfiguracja.JezykIDDomyslny)
            {

                string typ = typeof(KontrolkaTresciBaza).PobierzOpisTypu();
                //int idtypu = SolexBllCalosc.PobierzInstancje.Konfiguracja.GetSystemTypeId(typeof(KontrolkaTresciBaza));
                nadpisywane = Calosc.DostepDane.Pobierz<Tlumaczenie>(null, x => x.ObiektId == Id && x.JezykId == JezykId && x.Typ == typ).ToDictionary(x => x.Pole, x => (object)x.Wpis);
            }
            return nadpisywane;
        }
        public string TypKontrolki()
        {
            return RodzajKontrolki;
        }

        public string ParametrySerializowane()
        {
            return ParametryKontrolkiSpecyficzne;

        }

        public void UstawParametrySerializowane(string parametry)
        {
            ParametryKontrolkiSpecyficzne = parametry;
        }

        public string PobierzCss()
        {
            StringBuilder sb = new StringBuilder();
            if (DodatkoweKlasyCssKolumny != null)
            {
                foreach (var d in DodatkoweKlasyCssKolumny)
                {
                    sb.AppendFormat("{0} ", d);
                }
            }
            sb.Append(DodatkoweKlasyCssReczneKolumny);
            return sb.ToString();
        }
    }
}
