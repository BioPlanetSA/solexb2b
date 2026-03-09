using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryPrzekazywaneDoWszystkichAtrybutow : ParametrySzczegolowProduktuBaza
    {
        public ParametryPrzekazywaneDoWszystkichAtrybutow(ProduktKlienta pk, string naglowek, IList<CechyBll> atrybuty, string szerokoscWartosci, string szerokoscOpisu, string stopka) : base(pk)
        {
            Naglowek = naglowek;
            Atrybuty = atrybuty;
            SzerokoscWartosci = szerokoscWartosci;
            SzerokoscOpisu = szerokoscOpisu;
            Stopka = stopka;
        }

        public string Naglowek { get; set; }
        public string Stopka { get; set; }
        public IList<CechyBll> Atrybuty { get; set; }
        public string SzerokoscWartosci { get; set; }
        public string SzerokoscOpisu { get; set; }

    }
}