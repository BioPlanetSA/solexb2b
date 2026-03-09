using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoGradacji: ParametrySzczegolowProduktuBaza
    {
        public ParametryDoGradacji(List<GradacjaWidok> widok, ProduktKlienta produkt, GradacjaProduktu kontrolkaGradacji) :base(produkt)
        {
            Widoki = widok;
            KontrolkaGradacji = kontrolkaGradacji;
        }
        public GradacjaProduktu KontrolkaGradacji { get; set; }
        public List<GradacjaWidok> Widoki { get; set; }
        public decimal WyliczonaIloscProduktu { get; set; }
    }
}