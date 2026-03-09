using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    public interface ILaczenieZamowien
    {
        /// <summary>
        /// Przetwarzanie zamówienia w oparciu o parametry
        /// </summary>
        /// <param name="dokument"></param>
        /// <param name="parametry"></param>
        /// <returns></returns>
        object PrzetorzZamowieniePoPolaczeniu(object dokument, Dictionary<string, object> parametry);
    }
}
