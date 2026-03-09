using SolEx.Hurt.Core;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryGaleriiZdjecProduktu
    {
        public ParametryGaleriiZdjecProduktu()
        {
        }

        public ParametryGaleriiZdjecProduktu(ProduktKlienta prod, string rozmiar, string naglowek, string stopka,string rozmiarMiniatury, string textZastepczy="")
        {
            RozmiarZdjecia = rozmiar;
            Naglowek = naglowek;
            Stopka = stopka;
            Produkt = prod;
            RozmiarMiniatury = rozmiarMiniatury;
            TextZastepczy = textZastepczy;
        }
        public ParametryGaleriiZdjecProduktu(ProduktKlienta prod, string rozmiar, string naglowek, string stopka, bool pobierz, bool podglad, string textZastepczy = "")
        {
            RozmiarZdjecia = rozmiar;
            Naglowek = naglowek;
            Stopka = stopka;
            Produkt = prod;
            PelnaWersja = pobierz;
            TextZastepczy = textZastepczy;
            DuzyPodglad = podglad;
        }
        
        public ProduktKlienta Produkt { get; set; }
        public bool PelnaWersja { get; set; }
        public bool DuzyPodglad { get; set; }
        public string RozmiarZdjecia { get; set; }
        public string RozmiarMiniatury { get; set; }
        public string TextZastepczy { get; set; }
        public string Naglowek { get; set; }
        public string Stopka { get; set; }
    }
}