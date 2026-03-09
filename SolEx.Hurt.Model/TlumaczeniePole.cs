using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class TlumaczeniePole : IHasLongId, IPolaIDentyfikujaceRecznieDodanyObiekt, IPoleJezyk, IStringIntern
    {

        [WidoczneListaAdmin(true, true, false, false)]
        public long Id { get; set; }

        [StringInternuj]
        [FriendlyName("Tłumaczenie")]
        [Lokalizowane]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.PoleTekstoweMultiLine)]
        public string Nazwa { get; set; }

        [StringInternuj]
        [FriendlyName("Tekst w języku podstawowym")]
        [WidoczneListaAdmin(true, true, false, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleTekstoweMultiLine)]
        public string Domyslne { get; set; }

        public string PobierzPoleDoTlumaczen()
        {
            return string.IsNullOrWhiteSpace(Nazwa) ? Domyslne : Nazwa;
        }

        [FriendlyName("Miejsce wystąpienia tłumaczenia")]
        [WidoczneListaAdmin(true, true, false, false, false)]
        public MiejsceFrazy MiejsceFrazy { get; set; }

        [Ignore]
        public int JezykId { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }
    }
}
