using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class RegulaKoszykaWaga : RegulaKoszyka, IRegulaCalegoKoszyka, ITestowalna
    {
        [FriendlyName("Waga minimalna koszyka - Jeśli pole niewypełnione to brak dolnej granicy wagi")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? WagaMinimalna { get; set; }

        [FriendlyName("Waga maksymalna koszyka - - Jeśli pole niewypełnione to brak górnej granicy wagi")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? WagaMaksymalna { get; set; }

        [FriendlyName("Cechy które musi posiadać produkt żeby był brany przy liczeniu wagi")]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikCech))]
        public List<string> ListaCech { get; set; }

        public List<long> ListaCechId
        {
            get { return ListaCech != null && ListaCech.Any() && ListaCech.First() != "" ? ListaCech.Select(long.Parse).ToList() : null; }
        }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            decimal waga = 0;
            if (ListaCechId == null)
            {
                waga = koszyk.WagaCalokowita().Wartosc;
            }
            else
            {
                foreach (var pozycja in koszyk.PobierzPozycje)
                {
                    if (pozycja.Produkt.Cechy.Any(x => ListaCechId.Contains(x.Key)))
                    {
                        if (pozycja.WagaPozycji() != null)
                        {
                            waga += pozycja.WagaPozycji().Value;
                        }
                    }
                }
            }
            bool dolnylimit = !WagaMinimalna.HasValue || waga >= WagaMinimalna.Value;
            bool gornylimic = !WagaMaksymalna.HasValue || waga < WagaMaksymalna.Value;
            return dolnylimit && gornylimic;
        }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = Przedzial.Sprawdzenie(WagaMinimalna, WagaMaksymalna, "Waga minimalna", "Waga maksymalna");
            return listaBledow;
        }

        public override string Opis
        {
            get { return "Reguła sprawdzająca wagę koszyka"; }
        }

    }
}