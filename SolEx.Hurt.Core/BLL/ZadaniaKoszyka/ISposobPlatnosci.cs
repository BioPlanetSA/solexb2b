using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public interface ISposobPlatnosci : IGrupaZadania, IOpisModulu
    {
        [FriendlyName("Nazwa")]
        string Nazwa { get; set; }

        [FriendlyName("Termin")]
        int Termin { get; set; }

        [FriendlyName("Gdzie wyświetlać komunikat")]
        PokazywanieKomunikatu KomunikatPozycja { get; set; }

        [FriendlyName("Komunikat")]
        [Niewymagane]
        [Lokalizowane]
        string Komunikat { get; set; }
    }

    public interface IOpisModulu
    {
        int Id { get; }

        string PobierzOpis(IKoszykiBLL koszyk);
    }
}