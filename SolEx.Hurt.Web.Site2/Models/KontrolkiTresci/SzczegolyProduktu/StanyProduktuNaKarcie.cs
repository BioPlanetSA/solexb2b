using System.Collections.Generic;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class StanyProduktuNaKarcie : SzczegolyProduktuBaza, INaglowekStopka, IPoleJezyk
    {
        public override string Nazwa
        {
            get { return "Stany produktów"; }
        }

        public override string Akcja
        {
            get { return "StanyProduktu"; }
        }
        [FriendlyName("Sposób pokazywania")]
        [WidoczneListaAdmin(true, false, true, false)]
        [PobieranieSlownika(typeof(SlownikSposobowPokazywaniaStanow))]
        [Niewymagane]
        public List<long> ListaSposobow { get; set; }

        [FriendlyName("Nagłówek sekcji")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string Naglowek { get; set; }

        [FriendlyName("Stopka sekcji")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string Stopka { get; set; }

        public int JezykId { get; set; }
    }
}