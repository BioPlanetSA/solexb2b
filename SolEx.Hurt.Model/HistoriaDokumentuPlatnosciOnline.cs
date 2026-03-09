using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model
{
    public class HistoriaDokumentuPlatnosciOnline : IHasLongId
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }
        public int IdDokumentu { get; set; }
        public string NazwaDokumentu { get; set; }
        public long PlatnikId { get; set; }
        public DateTime DataOperacji { get; set; }
        public string IpOperacji { get; set; }
        public decimal Kwota { get; set; }
        public StatusPlatnosci Status { get; set; }
        public string KluczPlatnosci { get; set; }
        public string NumerPlatnosci { get; set; }
        public int? MetodaPlatnosci { get; set; }
        public string TytulPlatnosci { get; set; }
    }
}
