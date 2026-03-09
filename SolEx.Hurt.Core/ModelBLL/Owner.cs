using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.ModelBLL
{
    /// <summary>
    /// Model właściciela systemu / sprzedawcy
    /// </summary>
    public class Owner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public Adres Address { get; set; }
        public string NIP { get; set; }
        public string NIPEU { get; set; }
        public string IsEU { get; set; }
        public string User { get; set; }
        public Owner() { Address = new Adres(null); }
    }
}
