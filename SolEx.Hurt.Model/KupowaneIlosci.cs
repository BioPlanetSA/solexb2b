using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model
{
    [Alias("vKupowaneIlosci")]
    public class KupowaneIlosci:IHasLongId
    {

        public long Id
        {
            get
            {
                string klucz = KlientId + "||" + ProduktId + "||" + RodzajDokumentu + "||" + DataZakupu;
                return klucz.WygenerujIDObiektuSHAWersjaLong();
            }
        }

        public DateTime? DataZakupu { get; set; }
        public RodzajDokumentu RodzajDokumentu { get; set; }

        public decimal Ilosc { get; set; }
        
        public long KlientId { get; set; }
        public long ProduktId { get; set; }

        public KupowaneIlosci()
        {
        }
        private DateTime? PrzetworzDate(DateTime data)
        {
            DateTime? min = data == DateTime.MinValue || data == DateTime.MaxValue ? (DateTime?)null : data;
            return min;
        }
        public KupowaneIlosci(long produkt,long klient,DateTime odKiedy,RodzajDokumentu typ,decimal ilosc)
        {
            ProduktId = produkt;
            KlientId = klient;
            DataZakupu = PrzetworzDate( odKiedy);
            RodzajDokumentu = typ;
            Ilosc = ilosc;
        }
    }
}
