using System;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface ISolexHelper
    {
        bool TlumaczenieWLocie { get; }
        Jezyk AktualnyJezyk { get; }
        KoszykBll AktualnyKoszyk { get; }
        IKlient AktualnyKlient { get; }
        Guid? AktualnaSesjaID { get; set; }
    }
}