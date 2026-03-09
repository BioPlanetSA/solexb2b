using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface ISesjaHelper
    {
        long KlientID { get; }
      //  long? PrzedstawicielID { get; }
    //    long? OddzialDoJakiegoNalezyKlient { get; }
        bool CzyKlientZalogowany { get; }
        string IpKlienta { get; }
    }
}