using System;

namespace SolEx.Hurt.Model
{
    public class StatusDokumentuPDF
    {
        public DateTime DataWystawniaDokumentu { get; set; }
        public int IdDokumentu { get; set; }
        public string Rozszerzenie { get; set; }
        public string DaneBase64{get; set; }
        public string SymbolJezykaWydruku { get; set; }
    }
}
