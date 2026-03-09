using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [ModulStandardowy]
    public class PoleUploadPlikow : ZadanieCalegoKoszyka, IModulStartowy, IPoleWlasneKoszyka
    {

        public PoleUploadPlikow()
        {
            this.Wymagane = false;
            this.Symbol = "Listy przewozowe";
            this.NazwaDlaKlienta = "Przeciągnij i upuść tutaj pliki które chcesz wysłać razem z zamówieniem (np. list przewozowy)";
            Pozycja=PozycjaNaWidokuKoszyka.ZLewej;
        }

        public override bool Wykonaj(ModelBLL.Interfejsy.IKoszykiBLL koszyk)
        {
            return true;
        }

        string IPoleWlasneKoszyka.Symbol
        {
            get { return "PlikiDodaneDoZamowienia" + Symbol; }
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
            get { return TypDodatkowegoPolaKoszykowego.Pliki; }
        }

        public bool Multiwybor
        {
            get { return true; }
        }

        public override string Opis
        {
            get { return "Dodaje w koszyku pole do załanczania plików razem z zamówieniem"; }
        }

        [FriendlyName("Pozycja kontrolki w koszyku")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public PozycjaNaWidokuKoszyka Pozycja { get; set; }
    }
}