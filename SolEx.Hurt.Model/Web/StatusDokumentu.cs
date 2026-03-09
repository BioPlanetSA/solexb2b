using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Web
{
    public class StatusDokumentu
    {
        public StatusDokumentu()
        {
            
        }

        public StatusDokumentu(int id, int platnik, string hash,decimal wartosc,DateTime data,RodzajDokumentu rodzaj)
        {
            Id = id;
            PlatnikId = platnik;
            Hash = hash;
            WartoscNetto = wartosc;
            DataWystawienia = data;
            Rodzaj = rodzaj;
        }
        public int Id { get; set; }
        public int PlatnikId { get; set; }
        public string Hash { get; set; }
        public decimal WartoscNetto { get; set; }
        public DateTime DataWystawienia { get; set; }
        public RodzajDokumentu Rodzaj { get; set; }
    }
}
