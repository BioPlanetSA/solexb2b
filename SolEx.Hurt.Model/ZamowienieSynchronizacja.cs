using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SolEx.Hurt.Model
{
    public class ZamowienieSynchronizacja:Zamowienie
    {
        public List<ZamowienieProdukt> pozycje { get; set; }

        public string NumerZRozbicia { get; set; }
        public string Podtytul { get; set; }
        //potrzebne dla modułów typu comarchconnector
        public bool ZamowienieImportowanePoStronieKlienta { get; set; }

        public ZamowienieSynchronizacja()
        {
            pozycje = new List<ZamowienieProdukt>();
            ZamowienieImportowanePoStronieKlienta = false;
            Rozbijaj = true;
        }

        public ZamowienieSynchronizacja(Zamowienie item):base(item)
        {
        }

        /// <summary>
        /// Tworzy nowe zamówienie do które jest używane do rozbijania zamówień. 
        /// </summary>
        /// <param name="kolejnyNumerRozbitegoZamowienia"></param>
        /// <param name="powodRozbicia"></param>
        /// <param name="dlugoscNumeruRozbicia"></param>
        /// <returns></returns>
        public virtual ZamowienieSynchronizacja StworzZamowieniaRozbite(int kolejnyNumerRozbitegoZamowienia, string powodRozbicia, int dlugoscNumeruRozbicia=20)
        {
            ZamowienieSynchronizacja z = new ZamowienieSynchronizacja(this)
            {
                LacznieRozbitych = this.LacznieRozbitych,
                ListaDokumentowZamowienia = this.ListaDokumentowZamowienia,
                Podtytul = this.Podtytul,
                Rozbijaj = this.Rozbijaj,
                ZamowienieImportowanePoStronieKlienta = this.ZamowienieImportowanePoStronieKlienta,
                NumerZRozbicia = $"{NumerTymczasowyZamowienia}/{kolejnyNumerRozbitegoZamowienia}/{powodRozbicia}",
                PochodziZRozbicia = true,
                pozycje =  new List<ZamowienieProdukt>()
            };

            //numer rozbiciaj max 20 znakow
            if (z.NumerZRozbicia.Length > dlugoscNumeruRozbicia)
            {
                z.NumerZRozbicia = z.NumerZRozbicia.Substring(0, dlugoscNumeruRozbicia);
            }
            return z;
        }

        public int? LacznieRozbitych { get; set; }

        public virtual string NumerZPlatformy => $"B2B {DoklejInfoNumer()}";

        protected string DoklejInfoNumer()
        {
            string numer = " ";
            if (string.IsNullOrEmpty(this.NumerZRozbicia))
            {
                if (!string.IsNullOrEmpty(this.NumerTymczasowyZamowienia))
                {
                    numer = this.NumerTymczasowyZamowienia;
                }
            }
            else numer = this.NumerZRozbicia;

            if (this.LacznieRozbitych.HasValue)
            {
                string koncowka = string.IsNullOrEmpty(this.NumerZRozbicia) ? " " + this.LacznieRozbitych.Value : "";

                koncowka += " z " + this.LacznieRozbitych.Value;
                if (numer.Length + koncowka.Length > 20)
                {
                    numer = numer.Substring(0, 20 - koncowka.Length);
                }
                numer += koncowka;
            }
            return numer;
        }

        public bool PochodziZRozbicia{get; set;}

        public bool PosiadaNumerRozbicia => !string.IsNullOrEmpty(NumerZRozbicia);

        public bool Rozbijaj { get; set; }

        public List<ZamowienieDokumenty> ListaDokumentowZamowienia { get; set; }

        /// <summary>
        /// Id zamówienia z ERP do którego dołaczyć pozycje.
        /// </summary>
        public long? IdZamowieniaDoPolaczenia { get; set; }

        [JsonIgnore]
        public Func<object, string, ZamowienieSynchronizacja, object> ZdarzeniePrzetworzZamowienie;
    }
}
