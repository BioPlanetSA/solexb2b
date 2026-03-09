using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryPrzekazywaneDoZdjec
    {
        public ParametryPrzekazywaneDoZdjec(IProduktKlienta pk, string preset, bool powiekszenie, RodzajMetki metka, string dodatkoweKlasy, string link, bool podglad = true)
        {
            Produkt = pk;
            Preset = preset;
            PokazywacPowiekszeniePoNajechaniu = powiekszenie;
            RodzajMetki=metka;
            DodatkoweKlasy = dodatkoweKlasy;
            Link = link;
            DuzyPodglad = podglad;
        }

        public IProduktKlienta Produkt { get; set; }
        public bool LazyZdjecia { get; set; }
        public string Preset { get; set; }
        public bool PokazywacPowiekszeniePoNajechaniu { get; set; }
        public RodzajMetki RodzajMetki { get; set; }
        public string DodatkoweKlasy { get; set; }
        public string Link { get; set; }
        public bool DuzyPodglad { get; set; }
    }
}