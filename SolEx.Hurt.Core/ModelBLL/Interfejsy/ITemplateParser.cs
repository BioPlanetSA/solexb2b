using System.Collections.Generic;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface ITemplateParser
    {
        string Parse(string text, object container);
    }
}