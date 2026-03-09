using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IPaymentProvider
    {
        ProviderPlatnosciOnline Provider { get; }
    }
}