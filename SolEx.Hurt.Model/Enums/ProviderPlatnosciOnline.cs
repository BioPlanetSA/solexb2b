using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum ProviderPlatnosciOnline
    {
        [FriendlyName("System płatności SolexPay 30dni")]
        SolexPay = 1,     
        [FriendlyName("System płatności SolexPay 14dni")]
        SolexPay14 = 50
    }
}
