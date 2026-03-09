using System.Collections.Generic;

namespace SolEx.Hurt.Web.Site2.UI
{
    public  interface IConfigurator
    {
        IEnumerable<int> SelectedAttributesIDs { get; }
        string QueryString { get; }
    }
}
