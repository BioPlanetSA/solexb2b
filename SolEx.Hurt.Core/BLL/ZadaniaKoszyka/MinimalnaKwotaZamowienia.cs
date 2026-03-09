using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class MinimalnaKwotaZamowienia : ZadanieCalegoKoszyka, IModulStartowy, IFinalizacjaKoszyka, ITestowalna
    {
        public MinimalnaKwotaZamowienia()
        {
            Rodzaj = KomunikatRodzaj.info;
        }

        public override string Opis
        {
            get
            {
                return
                    @"Minimalna wartośc zamówienia. W komunikacie można użyć następujących symboli {0} - minimalna wartość netto zamówienia, {1} - kwota brakująca. Jeśli zaznaczone jest Pobieraj minimum z klienta i klient ma minimum >0
                        to pobierana jest wartość z niego, inaczej pobierana jest wartość z pola MinimalnaKwotaNetto. Jeśli minimum jest równe 0 to moduł nie zostanie włączony";
            }
        }

        [FriendlyName("Czy blokować złożenie zamówienia, przy zamówieniu ze zbyt małą kwotą")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool BlokadaPoPrzekroczeniu { get; set; }

        [FriendlyName("Minimalna kwota netto zamówienia")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal MinimalnaKwotaNetto { get; set; }

        [FriendlyName("Pobieraj minimum z klienta")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool PobierajMinimumZKlienta { get; set; }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = Przedzial.SpradzWartosc(MinimalnaKwotaNetto, "Minimalna kwota netto");
            return listaBledow;
        }

        [FriendlyName("Rodzaj komunikatu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public KomunikatRodzaj Rodzaj { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            decimal minumu = MinimalnaKwotaNetto;
            var k = koszyk.Klient;
            if (PobierajMinimumZKlienta)
            {
                if (k.MinimalnaWartoscZamowienia.HasValue && k.MinimalnaWartoscZamowienia > 0)
                {
                    minumu = k.MinimalnaWartoscZamowienia.Value;
                }
            }
            if (minumu <= 0)
            {
                return true;
            }
            bool spelnione = !(koszyk.CalkowitaWartoscHurtowaNettoPoRabacie() < minumu);
            if (!spelnione)
            {
                WyslijWiadomosc(string.Format(Komunikat, minumu.ToString("0.##") + " " + koszyk.WalutaKoszyka().WalutaB2b,
                    (minumu - koszyk.CalkowitaWartoscHurtowaNettoPoRabacie().Wartosc).ToString("0.##") + " " + koszyk.WalutaKoszyka().WalutaB2b), Rodzaj);
            }
            return spelnione || !BlokadaPoPrzekroczeniu;
        }
    }
}