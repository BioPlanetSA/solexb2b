using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL
{
    public class KlasaOpakowanieTesty
    {
        //        Tuple<string, bool, string>
        public string NazwaTestu { get; set; }

        public string NazwaZadania { get; set; }
        public bool BladTestu { get; set; }
        public bool CzyMaHistorieZmian { get; set; }
        public List<string> ListaBledow { get; set; }
    }
}