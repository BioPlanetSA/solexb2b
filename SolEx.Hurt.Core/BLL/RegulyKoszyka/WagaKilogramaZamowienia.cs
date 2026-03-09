using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class WagaKilogramaZamowienia : RegulaKoszyka, IRegulaCalegoKoszyka
    {
        public override string Opis
        {
            get { return "Czy stosunek wartość netto zamówienia/ do wagi zamówienia jest określony. Jeśli waga koszyka jest równa zero to warunek jest niespełniony"; }
        }

        [FriendlyName("Wybrany stosunek wartości do wagi")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Stosunek { get; set; }

        [FriendlyName("Czy liczymy wg netto czy brutto")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyBrutto { get; set; }

        [FriendlyName("Stosunek wartości do wagi ma być")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Wartosc WartoscWarunek { get; set; }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            decimal wartosc = CzyBrutto ? koszyk.CalkowitaWartoscHurtowaBruttoPoRabacie() : koszyk.CalkowitaWartoscHurtowaNettoPoRabacie();
            if (koszyk.WagaCalokowita() == 0) return false;

            decimal stosunek = wartosc / koszyk.WagaCalokowita();
            return stosunek.PorownajWartosc(Stosunek, WartoscWarunek);
        }
    }
}