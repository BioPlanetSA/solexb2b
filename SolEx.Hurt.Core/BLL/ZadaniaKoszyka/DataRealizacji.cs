using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class DataRealizacji : ZadanieCalegoKoszyka, IModulStartowy, IPoleWlasneKoszyka
    {
        public DataRealizacji()
        {
            Pozycja=PozycjaNaWidokuKoszyka.ZPrawej;
        }
        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            return true;
        }

        public string Symbol
        {
            get { return SymboleModulowKoszyka.DataRealizacjiZamowienia.ToString(); }
        }

        [FriendlyName("Nazwa dla klienta")]
        [Lokalizowane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string NazwaDlaKlienta { get; set; }

        [FriendlyName("Klient ma obowiązek wybrać wartość")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool Wymagane { get; set; }

        public string[] PobierzOpcje()
        {
            return new[] { "" };
        }

        public TypDodatkowegoPolaKoszykowego TypPola
        {
            get { return TypDodatkowegoPolaKoszykowego.Date; }
        }

        public bool Multiwybor
        {
            get { return false; }
        }

        public override string Opis
        {
            get { return "Dodaje w koszyku pole datowe"; }
        }
        [FriendlyName("Pozycja kontrolki w koszyku")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public PozycjaNaWidokuKoszyka Pozycja { get; set; }
    }
}