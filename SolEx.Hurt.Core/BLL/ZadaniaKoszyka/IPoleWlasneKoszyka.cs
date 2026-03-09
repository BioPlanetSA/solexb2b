using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public interface IPoleWlasneKoszyka : IGrupaZadania
    {
        int Id { get; }
        string Symbol { get; }

        [FriendlyName("Nazwa dla klienta")]
        [Lokalizowane]
        string NazwaDlaKlienta { get; set; }

        string[] PobierzOpcje();

        TypDodatkowegoPolaKoszykowego TypPola { get; }

        bool Wymagane { get; }

        bool Multiwybor { get; }
        PozycjaNaWidokuKoszyka Pozycja { get; }

    }
}