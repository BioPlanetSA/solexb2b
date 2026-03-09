using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class PoleTeskstoweKoszyk : ZadanieCalegoKoszyka, IModulStartowy, IPoleWlasneKoszyka
    {
        public PoleTeskstoweKoszyk()
        {
            Pozycja=PozycjaNaWidokuKoszyka.ZPrawej;
        }
        public override bool Wykonaj(ModelBLL.Interfejsy.IKoszykiBLL koszyk)
        {
            return true;
        }

        [FriendlyName("Symbol")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Symbol { get; set; }

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

        public override string Opis
        {
            get { return "Dodaje w koszyku pole tekstowe"; }
        }
        [FriendlyName("Pozycja kontrolki w koszyku")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public PozycjaNaWidokuKoszyka Pozycja { get; set; }
    }
}