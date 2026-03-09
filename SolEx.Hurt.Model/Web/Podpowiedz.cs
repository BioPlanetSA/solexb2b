using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Web
{
    /// <summary>
    /// Reprezentuje podpowiedź w modułach wyszukiwania
    /// </summary>
    public class Podpowiedz
    {
        public Podpowiedz(string klucz, string wartosc, PodpowiedziModul modulPodpowiedzi)
        {
            Klucz = klucz;
            Wartosc = wartosc;
            Modul = modulPodpowiedzi;
        }


        public Podpowiedz() { }
        public string Klucz { get; set; }
        public string Wartosc { get; set; }

        public PodpowiedziModul Modul
        {
            get; set;
        }
    }
}
