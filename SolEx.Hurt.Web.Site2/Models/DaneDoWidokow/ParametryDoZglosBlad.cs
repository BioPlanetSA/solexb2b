
namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow
{
    public class ParametryDoZglosBlad
    {
        public ParametryDoZglosBlad(string email,string temat,string nazwa,string text, string naglowek="", string stopka="")
        {
            Email = email;
            NazwaObiektu = nazwa;
            Temat = temat;
            Text = text;
            Naglowek = naglowek;
            Stopka = stopka;
        }
        public string Email{ get; set; }
        public string NazwaObiektu{ get; set; }
        public string Temat { get; set; }
        public string Text { get; set; }

        public string Naglowek { get; set; }
        public string Stopka{ get; set; }
    }
}