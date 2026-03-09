using System;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    internal class PytanieTakNie : ZadanieCalegoKoszyka, IModulStartowy, IPoleWlasneKoszyka
    {
        public PytanieTakNie()
        {
            Pozycja=PozycjaNaWidokuKoszyka.ZPrawej;
        }
        public override string Opis
        {
            get { return "Dodaje w koszyku pytanie tak/nie"; }
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

        public string[] PobierzOpcje()
        {
            return new[] { "Tak", "Nie" };
        }

        public TypDodatkowegoPolaKoszykowego TypPola
        {
            get { return TypDodatkowegoPolaKoszykowego.Bool; }
        }

        [FriendlyName("Klient ma obowiązek wybrać wartość")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool Wymagane { get; set; }

        public bool Multiwybor
        {
            get { return false; }
        }
        [FriendlyName("Pozycja kontrolki w koszyku")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public PozycjaNaWidokuKoszyka Pozycja { get; set; }
    }
}