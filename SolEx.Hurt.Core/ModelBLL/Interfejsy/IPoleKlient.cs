using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    /// <summary>
    /// Obiekt implementujące ten interfejs są rozrozniane przez klientow
    /// </summary>
    public interface IPoleKlient
    {
        /// <summary>
        /// Obiekt klient dla danej instancji obiektu
        /// </summary>
        IKlient Klient { get; set; }
    }
}
