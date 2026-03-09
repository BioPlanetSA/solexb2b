using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Linq;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Porównanie ceny pozycji", FriendlyOpis = "Sprawdza czy cena NETTO pozycji spełnia warunkek.")]
    public class CenaPozycji : RegulaKoszyka, IRegulaPozycji, IRegulaCalegoKoszyka
    {
      
        [FriendlyName("Wartość do porównania")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? Wartosc { get; set; }

        [FriendlyName("Operator porównania")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Wartosc Operator { get; set; }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return SprawdzCene(pozycja);
        }

        private bool SprawdzCene(IKoszykPozycja pozycja)
        {
            if (Wartosc == null)
            {
                throw new Exception("Nie podałeś wartości do porównania w module: Porównanie ceny pozycji");
            }
            decimal cena = pozycja.CenaNetto;
            return cena.PorownajWartosc(Wartosc.Value,Operator);
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            return koszyk.PobierzPozycje.All(SprawdzCene);
        }
    }
}