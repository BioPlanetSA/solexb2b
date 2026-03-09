using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using System;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.WarunkiRegulPunktow
{
    public class IloscProduktow : WarunekRegulyPunktowej, IWarunekRegulyPozycjiDokumentu
    {
        public bool SpelniaWarunek(DokumentuPozycjaBazowa pozycja, DokumentyBll dokument)
        {
            return pozycja.PozycjaDokumentuIlosc.Wartosc.PorownajWartosc(Ilosc, WartoscWarunek);
        }

        [FriendlyName("Ilosc")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal Ilosc { get; set; }

        [FriendlyName("Wartość koszyka ma być")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Wartosc WartoscWarunek { get; set; }
    }
}