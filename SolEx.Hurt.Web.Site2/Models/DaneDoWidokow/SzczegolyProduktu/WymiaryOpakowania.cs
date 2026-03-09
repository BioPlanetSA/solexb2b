using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.SzczegolyProduktu
{
    public class WymiaryOpakowania
    {
        public WymiaryOpakowania(IObrazek przyklad,decimal? glebokosc, decimal? wysokosc, decimal? szekosc,decimal? waga, decimal? objetosc,decimal? iloscwopakowaniu = null, decimal? iloscnawarstwie=null)
        {
            Wzor = przyklad;
            Glebkosc = glebokosc;
            Wysokosc= wysokosc;
            Szerokosc = szekosc;
            IloscNaWarstwie = iloscnawarstwie;
            IloscWOpakowaniu = iloscwopakowaniu;
            Objetosc = objetosc;
            Waga = waga;
        }
        public IObrazek Wzor { get; set; }
        public decimal? Glebkosc { get; set; }
        public decimal? Wysokosc { get; set; }
        public decimal? Szerokosc { get; set; }
        public decimal? Waga { get; set; }
        public decimal? Objetosc { get; set; }

        public decimal? IloscWOpakowaniu { get; set; }

        public decimal? IloscNaWarstwie { get; set; }

        public bool CzyJestUzupelnione
        {
            get { return this.Glebkosc != null || this.Wysokosc != null || this.Szerokosc != null || this.Waga != null || this.Objetosc != null || this.IloscWOpakowaniu != null || this.IloscNaWarstwie != null; }
        }
    }
}