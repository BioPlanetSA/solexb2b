using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using ServiceStack.Common;

namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    public abstract class BazaModulyDokumentyStatus : SyncModul
    {
        public BazaModulyDokumentyStatus()
        {
            PrzetwarzajTylkoAktualne = true;
        }

        [FriendlyName("Przetwarzaj dokumenty ze statusami")]
        [PobieranieSlownika(typeof(SlownikStatusow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Niewymagane]
        public List<int> Statusy { get; set; }

        [FriendlyName("Przetwarzaj tylko aktualne oferty")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool PrzetwarzajTylkoAktualne { get; set; }

        public Dictionary<long, List<HistoriaDokumentuProdukt>> PasujaceDokumenty( long[] listaKlientow )
        {
            Dictionary<long, List<HistoriaDokumentuProdukt>> dokumenty = ApiWywolanie.PobierzIdProduktowZDokumentowOStatusie(listaKlientow, Statusy.ToArray(), PrzetwarzajTylkoAktualne );
            LogiFormatki.PobierzInstancje.LogujInfo($"Pobieranie dokumentów o statusach id: [{Statusy.Select(x=>x.ToString()).ToList().Join(",")}], tylko aktualne oferty: {PrzetwarzajTylkoAktualne}. Pobrano produkty dla klientów: {dokumenty.Count}.");
            return dokumenty;
        }
    }
}
