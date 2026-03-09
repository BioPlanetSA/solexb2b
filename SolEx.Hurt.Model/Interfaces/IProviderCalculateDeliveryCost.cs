using SolEx.Hurt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SolEx.Hurt.Model.Interfaces
{
    public interface IProviderCalculateDeliveryCost
    {
        List<DeliveryCost> CalculateDeliveryCost(klienci customer, KoszykiBLL order);
        DeliveryCost GetFreeDelivery(klienci customer, KoszykiBLL order);
    }
}
