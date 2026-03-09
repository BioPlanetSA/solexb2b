using System;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using ServiceStack.Common;

namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    public abstract class ProduktySMMASH : SyncModul
    {
        public override string uwagi
        {
            get { return ""; }
        }

        public string PobierzRozmiar(string nazwa)
        {
            return nazwa.Trim().Split(' ').LastOrDefault().Trim();
        }

        public string PobierzRozmiarMod(string nazwa)
        {
            return nazwa.Trim().Split('-').LastOrDefault().Trim();
        }

        public string PobierzBazowyKodBezRozmiaru(string kod, string rozmiar)
        {
            if (!rozmiar.IsNullOrEmpty())
            {
                return kod.TrimEnd(rozmiar).Trim();
            }
            return kod;
        }
    }
}
