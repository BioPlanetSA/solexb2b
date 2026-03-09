using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [ModulStandardowy]
    [FriendlyName("Numer własny klienta dla zamówienia", FriendlyOpis = "Dodaje w koszyku pole do wpisania własnego numeru zamówienia")]
    public class NumerWlasnyZamowienia : ZadanieCalegoKoszyka, IModulStartowy, IPoleWlasneKoszyka
    {
        public NumerWlasnyZamowienia()
        {
            this.Komunikat = "Twój numer będzie pokazywany na liście Twoich zamówień na platformie. Nie będzie drukowany na fakturze.";
;            this.Pozycja = PozycjaNaWidokuKoszyka.ZLewej;
            this.NazwaDlaKlienta = "Twój własny identyfikator zamówienia";
            this.Wymagane = false;
        }
        public override bool Wykonaj(ModelBLL.Interfejsy.IKoszykiBLL koszyk)
        {
            return true;
        }

        public string Symbol
        {
            get { return "NumerWlasny"; }
        }

        [FriendlyName("Nazwa dla klienta")]
        [Lokalizowane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaDlaKlienta { get; set; }

        [FriendlyName("Klient ma obowiązek wpisać wartość")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool Wymagane { get; set; }

        public string[] PobierzOpcje()
        {
            return new[] { "" };
        }

        public TypDodatkowegoPolaKoszykowego TypPola
        {
            get { return TypDodatkowegoPolaKoszykowego.Text; }
        }

        public bool Multiwybor
        {
            get { return false; }
        }

        //public override string Opis
        //{
        //    get { return ""; }
        //}

        [FriendlyName("Pozycja kontrolki w koszyku")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public PozycjaNaWidokuKoszyka Pozycja { get; set; }
    }
}