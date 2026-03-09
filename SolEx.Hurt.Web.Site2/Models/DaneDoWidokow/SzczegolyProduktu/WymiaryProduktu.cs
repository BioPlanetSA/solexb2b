namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.SzczegolyProduktu
{
    public class WymiaryProduktu
    {
        public string JednostkaGabarytow { get; set; }
        public string JednostkaObjetosci { get; set; }
        public string JednostkaWagi { get; set; }
        public WymiaryOpakowania OpJednostkowe { get; set; }
        public WymiaryOpakowania OpZbiorcze { get; set; }
        public WymiaryOpakowania Paleta { get; set; }
    }
}