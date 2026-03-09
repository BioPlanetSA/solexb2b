using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class IndywidualizacjaProduktu : ZadaniePozycjiKoszyka, IModulStartowy, IFinalizacjaKoszyka
    {
        [FriendlyName("Typ liczenia rabatu")]
        [WidoczneListaAdmin(true, true, true, false)]
        public TrybLiczeniaRabatuWKoszyku TypLiczeniaRabatu { get; set; }

        public override bool Wykonaj(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            //todo: logika
            //decimal kwota = 0;
            if (pozycja.Indywidualizacja == null || !pozycja.Indywidualizacja.Any())
            {
                return true;
            }
            decimal narzutKwota = 0;
            decimal cenaProduktu = pozycja.Produkt.FlatCeny.CenaNetto.Wartosc;
            foreach (var ip in pozycja.Indywidualizacja)
            {
                var walutaKoszyka = koszyk.WalutaKoszyka();
                var narzut = ip.Indywidualizacja.PobierzCeneDlaWaluty(walutaKoszyka.Id);
                if (narzut.NarzutTyp == NarzutTyp.Brak || !narzut.Cena.HasValue || narzut.Cena.Value == 0)
                {
                    continue;
                }
                if (narzut.NarzutTyp == NarzutTyp.Staly)
                {
                    narzutKwota += narzut.Cena.Value*pozycja.Ilosc;
                }
                else
                {
                    switch (ip.Indywidualizacja.RodzajKontrolki)
                    {
                        case RodzajKontrolki.Input:
                            narzutKwota += ip.Wartosc != null ? ip.Wartosc.ToString().Length * narzut.Cena.Value : 0;
                            break;
                        case RodzajKontrolki.Atrybut:

                            break;
                        case RodzajKontrolki.Plik:
                            narzutKwota += narzut.Cena.Value;
                            break;
                    }
                }
            }
            cenaProduktu += narzutKwota;
            decimal nowy = Kwoty.WyliczRabatDlaUzyskaniaCeny(pozycja.Produkt.FlatCeny.CenaNetto, cenaProduktu);
            pozycja.ZmienDodatkowyRabat(nowy, Komunikat, TypLiczeniaRabatu);
            return true;
        }
    }
}