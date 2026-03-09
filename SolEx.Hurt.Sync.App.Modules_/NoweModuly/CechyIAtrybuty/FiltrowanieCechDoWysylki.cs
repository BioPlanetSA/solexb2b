using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    [Obsolete]
    [FriendlyName(FriendlyOpis = "Filtrujemy Cechy z okreslonymi atrybutami - moduł WYCOFANY. Stosować przełącznik na atrybutach")]
    public class FiltrowanieCechDoWysylki : SyncModul, IModulCechyIAtrybuty
    {
        [FriendlyName("Lista atrybutów - cechy z podanymi atrybutami zostana wysłane")]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> ListaAtrybutow { get; set; }

        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            HashSet<int> atrybutyid = new HashSet<int>( ListaAtrybutow.Select(int.Parse) );
            for (int i = 0; i < cechy.Count; i++)
            {
                if (!atrybutyid.Contains(cechy[i].AtrybutId.GetValueOrDefault()))
                {
                    cechy.RemoveAt(i);
                    i--;
                }
            }
        }

     
    }
}
