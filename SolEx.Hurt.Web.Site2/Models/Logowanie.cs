using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class Logowanie : IBindowalny
    {
        public string Uzytkownik { get; set; }
        public string Haslo { get; set; }
    }
}