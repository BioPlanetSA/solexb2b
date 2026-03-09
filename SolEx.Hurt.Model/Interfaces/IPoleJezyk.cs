using ServiceStack.DataAnnotations;

namespace SolEx.Hurt.Model.Interfaces
{
    /// <summary>
    /// Obiekty implementujące ten interface są lokalizowane
    /// </summary>
    public interface IPoleJezyk
    {

        /// <summary>
        /// Id języka w jakim jest dany obiekt
        /// </summary>
        [Ignore]
        int JezykId { get; set; }
    }
}
