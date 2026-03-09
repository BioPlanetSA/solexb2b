using System.Collections.Generic;
using System.Collections.Specialized;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IProviderDB
    {
        string GetHtmlProductState(decimal state, decimal state2, decimal stateMin, int type, string p_1, string p_2, string p_3, string p_4, string p_5, string p1, string p2, string p3, NameValueCollection appSettings, NameValueCollection pars,SolEx.Hurt.Model.Produkt item);

        List<SolEx.Hurt.Model.FlatCeny> GetCalculatedPrices(NameValueCollection appSettings);

    }

}
