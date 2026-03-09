using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;
using System;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [ModulStandardowy]
    public class KoszykLimityWartoscioweSubkont : ZadanieCalegoKoszyka, IFinalizacjaKoszyka, IModulStartowy
    {
        public KoszykLimityWartoscioweSubkont()
        {
            KomunikatBlokada = "Przekroczony limit wartości zamówień";
            Komunikat = "";
        }

        public override string Opis
        {
            get { return "Sprawdzenie limitów wartościowych"; }
        }

        [FriendlyName("Komunikat gdy limit jest przekroczony, dopuszczalne paramerty {0} - wartość limitu, {1} - limit wykorzystany {2}. O ile limit przekroczony")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Lokalizowane]
        public string KomunikatBlokada { get; set; }

        [FriendlyName("Komunikat gdy limit nie jest przekroczony, dopuszczalne paramerty {0} - wartość limitu, {1} - limit wykorzystany")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Lokalizowane]
        public new string Komunikat { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            decimal przekroczono;
            decimal? pozostalylimit = SolexBllCalosc.PobierzInstancje.Koszyk.PozostalyLimitWartosciZamowien(koszyk.Klient, koszyk, out przekroczono);
            if (pozostalylimit.HasValue)
            {
                decimal pozostalo = (pozostalylimit.Value - koszyk.CalkowitaWartoscHurtowaNettoPoRabacie());

                if (przekroczono == 0)
                {
                    WyslijWiadomosc(string.Format(Komunikat, pozostalylimit.Value.ToString("0.00"), pozostalo.ToString("0.00")), KomunikatRodzaj.info);
                    return true;
                }
                WyslijWiadomosc(string.Format(KomunikatBlokada, pozostalylimit.ToString("0.00"), pozostalo.ToString("0.00"), przekroczono.ToString("0.00")), KomunikatRodzaj.danger);
                return SolexBllCalosc.PobierzInstancje.Koszyk.MoznaFinalizowacKoszykPrzezLimity(koszyk);
            }
            return true;
        }
    }
}