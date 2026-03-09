using System;
namespace SolEx.Hurt.Core.ModelBLL
{
    public class ZmianaObiektu
    {
        public int RodzajZmiany { get; set; }
        public object StaraWartosc { get; set; }
        public object NowaWartosc { get; set; }
        public string IdentyfikatoObiektu { get; set; }
        public DateTime DataZmiany { get; set; }
        public string IdentyfikatorWersji { get; set; }
        public string Tabela { get; set; }

        public string Pole { get; set; }
    }
}
