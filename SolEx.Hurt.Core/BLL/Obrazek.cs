using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    [Alias("Plik")]
    public class Obrazek : Plik, IObrazek
    {
        public Obrazek(Plik plik) : base(plik)
        {
        }

        public Obrazek()
        {
        }

        public Obrazek(string sciezka, string nazwa)
        {
            this.Sciezka = sciezka;
            this.Nazwa = nazwa;
        }

        public string LinkOryginal
        {
            get { return Obrazek.GenerujLinkDoZdjecia(this, null); }
        }

        /// <summary>
        /// Propertis stworzony by przesyłać wybranego preseta razem ze zdjeciem
        /// </summary>
        [Ignore]
        public string DomyslnyPreset { get; set; }

        public string LinkWWersji(string i = null)
        {
            if (i == null && DomyslnyPreset != null)
            {
                i = DomyslnyPreset;
            }
            return GenerujLinkDoZdjecia(this, i);
        }

        private static string GenerujLinkDoZdjecia(IObrazek o, string numerWersji = null)
        {
            return !string.IsNullOrEmpty(numerWersji) ? $"{o.SciezkaWzgledna}?preset={numerWersji}" : o.SciezkaWzgledna;
        }
    }
}