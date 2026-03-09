using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces.Modele
{
    public interface IProfilKlienta: IHasLongId
    {
        long Id{get;}

        long? KlientId { get; set; }
       
        string Dodatkowe { get; set; }

        TypUstawieniaKlienta TypUstawienia { get; set; }

        string Wartosc { get; set; }

        /// <summary>
        /// dodatkowy dopisek do Id np jesli wartość jest dla niezalogowanych
        /// </summary>
        string Dopisek { get; set; }
    }
}
