using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Dokumenty;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoPlatnosci
    {
        public ParametryDoPlatnosci(TrescBll tresc, List<DokumentPlatnosc> dokumenty, IKlient klient)
        {
            Tresc = tresc;
            Dokumenty = dokumenty;
            Klient = klient;
        }

        public Waluta Waluta { get; set; }

        public TrescBll Tresc { get; set; }
        public List<DokumentPlatnosc> Dokumenty { get; set; }
        public IKlient Klient { get; set; }

        public HistoriaDokumentuPlatnosciOnline[] juzWTrakciePlacenia { get; set; }

        public string[] errory {get;set;}

        public List<FirmaPlaceniaOnline> ListaFormPlatnosci { get; set; }

    }

    public class FirmaPlaceniaOnline
    {
        public string Akcja { get; set; }
        public string Nazwa { get; set; }
        public decimal Koszt { get; set; }
        public string PodTytul { get; set; }
        public string Error { get; set; }

        public bool Aktywna { get; set; }

        public string LinkDoPomocy { get; set; }
    }


}