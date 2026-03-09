using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model
{
    public class DzialaniaUzytkwonikowParametry : IHasStringId
    {
        [WidoczneListaAdmin(true, true, false, false)]
        public string Id {
            get
            {
                return NazwaParametru + "-" + IdDzialania;
            }
        }
        public string NazwaParametru { get; set; }

        public int IdDzialania { get; set; }

        public string Wartosc { get; set; }
        
        public DzialaniaUzytkwonikowParametry(string nazwa, int idzialania, string wartosc)
        {
            NazwaParametru = nazwa;
            IdDzialania = idzialania;
            Wartosc = wartosc;
        }

        public DzialaniaUzytkwonikowParametry()
        {
            
        }

    }
}
