using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;
using System;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [ModulStandardowy]
    public class KoszykLimityIloscioweSubkont : ZadanieCalegoKoszyka, IFinalizacjaKoszyka, IModulStartowy
    {
        public KoszykLimityIloscioweSubkont()
        {
            KomunikatBlokada = "Przekroczony limit ilości zamówień";
            Komunikat = "";
        }

        public override string Opis
        {
            get { return "Sprawdzenie limitów koszykowych"; }
        }

        [FriendlyName("Komunikat gdy limit jest przekroczony, dopuszczalne paramerty {0} - wartość limitu, {1} - limit wykorzystany {2} -o ile limit przekroczony")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Lokalizowane]
        public string KomunikatBlokada { get; set; }

        [FriendlyName("Komunikat gdy limit nie jest przekroczony, dopuszczalne paramerty {0} - wartość limitu, {1} - pozostały limit")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Lokalizowane]
        public new string Komunikat { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            int? pozostalylimit = SolexBllCalosc.PobierzInstancje.Koszyk.PozostalyLimitIloscZamowien(koszyk.Klient);

            if (pozostalylimit.HasValue)
            {
                var limit = SolexBllCalosc.PobierzInstancje.Klienci.PobierzCalkowityLimitIloscZamowien(koszyk.Klient);
                int? limitCalkowity = limit==null? null: limit.IloscZamowien;
                if (pozostalylimit >= 0)
                {
                    WyslijWiadomosc(string.Format(Komunikat, limitCalkowity, pozostalylimit), KomunikatRodzaj.info);
                    return true;
                }
                WyslijWiadomosc(string.Format(KomunikatBlokada, limitCalkowity
                                              , pozostalylimit, Math.Abs(pozostalylimit.Value)), KomunikatRodzaj.danger);
                return SolexBllCalosc.PobierzInstancje.Koszyk.MoznaFinalizowacKoszykPrzezLimity(koszyk);
            }
            return true;
        }
    }
}