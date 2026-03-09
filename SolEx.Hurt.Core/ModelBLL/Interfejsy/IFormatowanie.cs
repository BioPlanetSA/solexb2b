using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface IFormatowanie
    {
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [FriendlyName("Formatowanie wartości pól tekstowych - za wartość podstawiasz {0}",FriendlyOpis = "Wypełniamy jesli chcemy innaczej wyświetlić wartość")]
        string Formatowanie { get; set; }
    }
}
